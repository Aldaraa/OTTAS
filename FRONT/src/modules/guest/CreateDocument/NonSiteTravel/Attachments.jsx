import axios from 'axios'
import { Button, Form, Table, Modal } from 'components'
import { CheckBox } from 'devextreme-react'
import React, { useState } from 'react'
import { Form as AntForm, Upload } from 'antd'
import { InboxOutlined, PlusOutlined } from '@ant-design/icons'

const { Dragger } = Upload;

function Attachments({mainForm, getMaster}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ files, setFiles ] = useState([])
  const [ form ] = AntForm.useForm()

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
          // onDownload={(e) => console.log('download')}
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
      label: 'Include E-Mail',
      name: 'includeEmail',
      className: 'col-span-12 mb-2',
      type: 'check'
    },
    {
      label: 'Comment',
      name: 'comment',
      className: 'col-span-12 mb-2',
      type: 'textarea'
    },
  ]

  const handleDeleteButton = (row, event) => {
    event.stopPropagation()
    let tmp = mainForm.getFieldValue('files')?.filter((item) => row.data.fileAddressId !== item.fileAddressId)
    mainForm.setFieldValue('files', tmp)
  }

  const columns = [
    {
      label: 'File',
      name: 'file',
      alignment: 'left',
      cellRender: (e) => (
        <div>File</div>
      )
    },
    {
      label: 'Comment',
      name: 'comment',
      alignment: 'left',
    },
    {
      label: 'Full name',
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
      name: 'includeEmail',
      alignment: 'left',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '100px', 
      alignment: 'center',
      cellRender: (e) => (
        <button type='button' className='dlt-button' onClick={(event) => handleDeleteButton(e, event)}>Delete</button>
      )
    },
  ]

  const handleSubmit = (values) => {
    const formData = new FormData()
    files.map((item) => {
      formData.append('files', item)
    })
    axios({
      method: 'post',
      url: `/tas/SysFile/multi`,
      data: formData,
    }).then((res) => {
      let tmp = []
      if(mainForm.getFieldValue('files')){
        tmp = [
          ...mainForm.getFieldValue('files'),
          ...res.data.map((item) => ({fileAddressId: item.Id, comment: values.comment, includeEmail: values.includeEmail}))
        ]
      }else{
        tmp = res.data.map((item) => ({fileAddressId: item.Id, comment: values.comment, includeEmail: values.includeEmail}))
      }
      mainForm.setFieldValue('files', tmp)
      form.resetFields()
      handleCloseModal(false)
    }).catch((err) => {

    })
  }

  const handleCloseModal = () => {
    form.resetFields()
    setShowModal(false)
  }

  return (
    <>
      <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.files !== curValues.files} className='col-span-12'>
        {
          ({}) => {
            return(
              <Table
                data={mainForm.getFieldValue('files')}
                columns={columns}
                allowColumnReordering={false}
                containerClass='shadow-none border border-gray-300'
                keyExpr='Id'
                pager={mainForm.getFieldValue('files')?.length > 20}
                title={
                  <div className='flex justify-between items-center py-2 gap-3 border-b'>
                    <div className='text-md font-bold pl-2'>Attachments</div>
                    <div className='flex gap-4 items-center'>
                      <Button 
                        icon={<PlusOutlined />} 
                        onClick={() => setShowModal(true)}
                        className='text-xs'
                        htmlType='button'
                      >
                        Add Attachment
                      </Button>
                    </div>
                  </div>
                }
              />
            )
          }
        }
      </Form.Item>
      <Modal title='Add Attachment' open={showModal} onCancel={()=>setShowModal(false)}>
        <Form
          form={form}
          fields={fields}
          size='small' 
          className='grid grid-cols-12 gap-x-8'
          onFinish={handleSubmit}
          noLayoutConfig={true}
          layout='vertical'
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='submit' type='primary' icon={<PlusOutlined/>}>Add</Button>
            <Button htmlType='button' onClick={handleCloseModal} >Cancel</Button>
          </div>
        </Form>
      </Modal>
    </>
  )
}

export default Attachments