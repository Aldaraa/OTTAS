import axios from 'axios'
import { Button, Modal, Form, Table, Tooltip } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form as AntForm, Upload } from 'antd'
import { DownloadOutlined, InboxOutlined, PlusOutlined, QuestionCircleOutlined, SaveOutlined, SyncOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { useParams } from 'react-router-dom'

const { Dragger } = Upload;

function Attachments({disabled, documentDetail, hasUpdatePermission}) {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ files, setFiles ] = useState([])
  const [ downloadLoading, setDownloadLoading ] = useState(false)
  const dataGrid = useRef(null)

  const { state, action } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const { documentId } =  useParams()

  const hasActionPermission = useMemo(() => {
    if(documentDetail?.CurrentStatus === 'Completed' && (state.userInfo?.Role === 'TravelAdmin' || state.userInfo?.Role === 'SystemAdmin')){
      return false
    }
    return disabled
  },[documentDetail, state])

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocumentattachment/${documentId}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then((res) => {
      setLoading(false)
    })
  }

  const handleChangeFile = (event) => {
    let tmp = []
    event.fileList.map((item) => {
      tmp.push(item.originFileObj)
    })
    setFiles(tmp)
  }

  const fields = [
    {
      type: 'component',
      component: <Form.Item
        name='files'
        valuePropName="file"
        key={`form-item-c file-1`}
        label='File'
        className='col-span-12 mb-2'
      >
        <Dragger 
          beforeUpload 
          multiple
          maxCount={5}
          onChange={(e) => handleChangeFile(e)}
          accept='.doc, .docx, .pdf, .txt, .rtf, .ppt, .pptx, .xls, .xlsx, .jpg, .jpeg, .png, .gif, .bmp, .tiff, .svg, .eml, .msg'
        >
          <InboxOutlined color='blue' style={{fontSize: 32, marginBottom: 8}}/>
          <p className="text-sm pb-1">Click or drag file to this area to upload</p>
          <p className="opacity-50 text-sm">
            Support for a single or bulk upload.
          </p>
        </Dragger>
      </Form.Item>
    },
    {
      label: 'Comment',
      name: 'Description',
      className: 'col-span-12 mb-2',
      type: 'textarea'
    },
    {
      label: 'Include E-Mail',
      name: 'IncludeEmail',
      className: 'col-span-12 mb-2',
      type: 'check'
    },
  ]

  const editFields = [
    {
      label: 'Comment',
      name: 'Description',
      className: 'col-span-12 mb-2',
      type: 'textarea'
    },
    {
      label: 'Include E-Mail',
      name: 'IncludeEmail',
      className: 'col-span-12 mb-2',
      type: 'check'
    },
  ]

  const handleEditButton = (row) => {
    setEditData(row)
    setShowModal(true)
  }

  const columns = [
    {
      label: 'File',
      name: 'FileAddress',
      alignment: 'left',
      cellRender: (e) => (
        <Tooltip title={`${process.env.REACT_APP_CDN_URL}${e.value}`}>
          <a className='edit-button' href={`${process.env.REACT_APP_CDN_URL}${e.value}`} target='_blank'>View File</a>
        </Tooltip>
      )
    },
    {
      label: 'Uploader name',
      name: 'FullName',
      alignment: 'left',
    },
    {
      label: 'Created Date',
      name: 'CreatedDate',
      alignment: 'left',
      cellRender:(e) =>(
        <div>{dayjs(e.value).format('YYYY-MM-DD HH:mm')}</div>
      )
    },
    {
      label: 'Include E-Mail',
      name: 'IncludeEmail',
      alignment: 'left',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    {
      label: 'Comment',
      name: 'Description',
      alignment: 'left',
    },
    {
      label: '',
      name: 'action',
      width: `${hasActionPermission ? '0px' : '150px'}`,
      cellRender: (e) => (
        (!hasActionPermission || hasUpdatePermission) ?
        <div className='flex gap-4'>
          {
            e.data.Id !== 0 &&
            <>
              <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
              <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
            </>
          }
        </div>
        : null
      )
    },
  ]

  const handleDelete = () => {
    axios({
      method: 'delete',
      url: `tas/requestdocumentattachment/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    })
  }

  const handleSubmit = (values) => {
    setSubmitLoading(true)
    if(editData){
      axios({
        method: 'put',
        url:'tas/requestdocumentattachment',
        data: {
          ...values,
          id: editData.Id,
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setSubmitLoading(false))
    }else{
      const formData = new FormData()
      files.map((item) => {
        formData.append('files', item)
      })
      axios({
        method: 'post',
        url: `/tas/SysFile/multi`,
        data: formData,
      }).then((res) => {
        axios({
          method: 'post',
          url:'tas/requestdocumentattachment',
          data: {
            ...values,
            fileAddressIds: res.data.map((item) => item.Id),
            DocumentId: documentId,
          }
        }).then((res) => {
          getData()
          handleCancel()
        }).catch((err) => {
    
        }).then(() => setSubmitLoading(false))
      }).catch((err) => {

      }).then(() => setSubmitLoading(false))
    }
  }

  const handleCancel = () => {
    form.resetFields()
    setShowModal(false)
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const handleAddButton = () => {
    setEditData(null)
    setShowModal(true)
    form.setFieldsValue({IncludeEmail: 1, Description: 'Flight and Hotel'})
  }

  const handleDownloadAll = useCallback(() => {
    setDownloadLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocumentattachment/download/${documentId}`,
      responseType: 'blob',
    }).then(async (res) => {
      const filename = res.headers['content-disposition'].split("; ")[1].replace('filename=', '')
      const url = await window.URL.createObjectURL(new Blob([res.data]));
      const link = await document.createElement('a');
      link.href = url;
      await link.setAttribute('download', filename); //or any other extension
      await document.body.appendChild(link);
      await link.click();
      await setDownloadLoading(false)
    }).catch((err) => {
      setDownloadLoading(false)
    })
  },[])

  return (
    <div className='col-span-12'>
      <Table
        ref={dataGrid}
        data={data}
        columns={columns}
        allowColumnReordering={false}
        id="room"
        className={`overflow-hidden ${!state.userInfo?.ReadonlyAccess && 'border-t'}`}
        showRowLines={true}
        rowAlternationEnabled={false}
        loading={loading}
        pager={data.length > 20}
        containerClass='shadow-none'
        title={
          <div className='flex justify-between items-center py-2 gap-3'>
            <div className='flex items-center gap-2'>
              {
                ((!hasActionPermission) && data.length > 1) ?
                <Button 
                  className='text-xs'
                  loading={downloadLoading}
                  onClick={handleDownloadAll}
                  icon={<DownloadOutlined/>}
                >
                  Download All
                </Button>
                : null
              }
              {
                !hasActionPermission &&
                (
                  documentDetail?.RuleAction?.split(',').includes('Create Flight Option') ? 
                  <Tooltip title='Attachment is a required'>
                    <QuestionCircleOutlined className='text-gray-400 hover:text-gray-500 hover:cursor-help'/>
                  </Tooltip>
                  : null
                )
              }
            </div>
            <div className='flex gap-4 items-center'>
              {
                (!hasActionPermission || hasUpdatePermission) ?
                <>
                  <Button 
                    icon={<PlusOutlined />} 
                    onClick={handleAddButton}
                    className='text-xs'
                    htmlType='button'
                  >
                    Add Attachment
                  </Button>
                  <Button 
                    icon={<SyncOutlined />} 
                    onClick={getData}
                    loading={loading}
                    className='text-xs'
                    htmlType='button'
                  >
                    Refresh
                  </Button>
                </>
                : null
              }
            </div>
          </div>
        }
      />
      <Modal title='Add Attachment' open={showModal} onCancel={handleCancel}>
        <Form
          form={form}
          fields={editData ? editFields : fields}
          editData={editData}
          size='small' 
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSubmit}
          disabled={hasActionPermission}
          noLayoutConfig={true}
          layout='vertical'
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='submit' loading={submitLoading} type={'primary'} icon={<SaveOutlined/>}>Save</Button>
            <Button htmlType='button' onClick={handleCancel}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button htmlType='button' type='danger'onClick={handleDelete}>Yes</Button>
          <Button htmlType='button' onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default Attachments