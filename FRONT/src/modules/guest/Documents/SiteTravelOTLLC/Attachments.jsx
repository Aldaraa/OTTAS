import axios from 'axios'
import { Button, Form, Table, Tooltip, Modal } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useEffect, useState } from 'react'
import { Form as AntForm, Upload } from 'antd'
import { InboxOutlined, PlusOutlined } from '@ant-design/icons'
import { useParams } from 'react-router-dom'

const { Dragger } = Upload;

function Attachments({mainForm, getMaster, disabled, showTitle=true}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ loading, setLoading ] = useState(true)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ addLoading, setAddLoading ] = useState(false)
  const [ data, setData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ files, setFiles ] = useState([])

  const [ form ] = AntForm.useForm()
  const { documentId } = useParams()

  useEffect(() => { 
    getFileData()
  },[])

  const getFileData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `/tas/requestdocumentattachment/${documentId}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleChangeFile = (event) => {
    let tmp = []
    event.fileList.map((item) => {
      tmp.push(item.originFileObj)
    })
    setFiles(tmp)
  }

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
      label: 'Description',
      name: 'Description',
      className: 'col-span-12 mb-2',
      type: 'textarea'
    },
    {
      label: 'Include E-Mail',
      name: 'includeEmail',
      className: 'col-span-12 mb-2',
      type: 'check'
    },
  ]

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

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
      width: `${disabled ? '0px' : '150px'}`,
      alignment: 'center',
      cellRender: (e) => (
        !disabled &&
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          <button type='button' className='dlt-button' onClick={(event) => handleDeleteButton(e.data)}>Delete</button>
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setAddLoading(true)
    if(editData){
      axios({
        method: 'put',
        url:'tas/requestdocumentattachment',
        data: {
          ...values,
          id: editData.Id,
        }
      }).then((res) => {
        getFileData()
        handleCloseModal()
      }).catch((err) => {
  
      }).then(() => setAddLoading(false))
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
          url: '/tas/requestdocumentattachment',
          data: {
            ...values,
            fileAddressIds: res.data.map((item) => item.Id),
            documentId: documentId ,
          },
        }).then((res) => {
          getFileData()
        }).catch((err) => {
    
        }).then(() => handleCloseModal())
  
      }).catch((err) => {
        handleCloseModal()
      })
    }
  }

  const handleDelete = () => {
    axios({
      method: 'delete',
      url: `tas/requestdocumentattachment/${editData.Id}`,
    }).then(() => {
      getFileData()
      setShowPopup(false)
    }).catch((err) => {

    })
  }

  const handleCloseModal = () => {
    form.resetFields()
    setShowModal(false)
    setAddLoading(false)
  }

  const handleAddButton = () => {
    setEditData(null)
    setShowModal(true)
  }

  return (
    <div className='col-span-12'>
      <Table
        data={data}
        columns={columns}
        allowColumnReordering={false}
        containerClass='shadow-none'
        keyExpr='Id'
        pager={data.length > 20}
        title={
          <div className='flex justify-between items-center py-2 gap-3 border-b'>
            <div className='text-md font-bold pl-2'>{showTitle ? 'Attachments' : null}</div>
            {
              !disabled &&
              <div className='flex gap-4 items-center'>
                <Button 
                  icon={<PlusOutlined />} 
                  onClick={handleAddButton}
                  className='text-xs'
                  htmlType='button'
                >
                  Add Attachment
                </Button>
              </div>
            }
          </div>
        }
      />
      <Modal title='Add Attachment' open={showModal} onCancel={()=>setShowModal(false)}>
        <Form
          form={form}
          fields={editData ? editFields : fields}
          size='small' 
          className='grid grid-cols-12 gap-x-8'
          onFinish={handleSubmit}
          noLayoutConfig={true}
          layout='vertical'
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='submit' type='primary' icon={<PlusOutlined/>} loading={addLoading}>Save</Button>
            <Button htmlType='button' onClick={handleCloseModal} disabled={addLoading}>Cancel</Button>
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
          <Button type={'danger'} onClick={handleDelete}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default Attachments