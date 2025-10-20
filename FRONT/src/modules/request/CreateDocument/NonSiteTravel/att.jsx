import axios from 'axios'
import { Button, Modal, Form, Table } from 'components'
import { CheckBox } from 'devextreme-react'
import React, { useState } from 'react'
import { Upload } from 'antd'
import { PlusOutlined, UploadOutlined } from '@ant-design/icons'

function Attachments({mainForm, getMaster}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ form ] = Form.useForm()


  const handleChangeFile = (e) => {
    const formData = new FormData()
    formData.append('file', e.file)
    axios({
      method: 'post',
      url: `/tas/SysFile/multi`,
      data: formData,
    }).then((res) => {
      form.setFieldValue('fileAddressId', res.data.Id)
    }).catch((err) => {

    })
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
        <Upload 
          beforeUpload 
          multiple
          maxCount={5}
          onChange={(e) => handleChangeFile(e)}
          // onDownload={(e) => console.log('download')}
          accept='.png, .jpeg, .jpg, .doc, .docx, .pdf'
        >
          <Button htmlType='button' icon={<UploadOutlined />}>Upload File</Button>
        </Upload>
      </Form.Item>
    },
    {
      label: 'Comment',
      name: 'comment',
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

  const handleDeleteButton = (row, event) => {
    event.stopPropagation()
    let tmp = [...mainForm.getFieldValue('files')]
    tmp.splice(row.rowIndex, 1)
    mainForm.setFieldValue('files', tmp)
    getMaster()
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
    mainForm.setFieldValue('files', [...mainForm.getFieldValue('files'), values])
    getMaster()
    handleCloseModal(false)
  }

  const handleCloseModal = () => {
    form.resetFields()
    setShowModal(false)
  }

  return (
    <>
      <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.files !== curValues.files} className='col-span-12'>
        {
          ({getFieldValue}) => {
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
                    <div className='text-md font-bold pl-2'>Attachment</div>
                    <div className='flex gap-4 items-center'>
                      <Button 
                        htmlType='button'
                        icon={<PlusOutlined />} 
                        onClick={() => setShowModal(true)}
                        className='text-xs'
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
      <Modal title='Add Attachment' open={showModal} onCancel={handleCloseModal}>
        <Form
          form={form}
          fields={fields}
          size='small' 
          className='grid grid-cols-12 gap-x-8'
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='button' onClick={() => form.submit()} type='primary' icon={<PlusOutlined/>}>Add</Button>
            <Button htmlType='button' onClick={handleCloseModal}>Cancel</Button>
          </div>
        </Form>
      </Modal>
    </>
  )
}

export default Attachments