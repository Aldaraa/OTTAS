import { EditOutlined, SaveOutlined } from '@ant-design/icons'
import { notification } from 'antd'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import React, { useCallback, useEffect, useMemo, useState } from 'react'

const modalFields = [
  {
    label: 'SMTP server',
    name: 'smtpServer',
    className: 'col-span-12 mb-2',
    rules: [{required: true, message: 'Code is required'}], 
  },
  {
    label: 'PORT',
    name: 'smtpPort',
    className: 'col-span-12 mb-2',
    rules: [{required: true, message: 'Description is required'}], 
  },
  {
    label: 'PORT',
    name: 'email',
    className: 'col-span-12 mb-2',
    rules: [{required: true, message: 'Description is required'}], 
  },
  {
    label: 'Password',
    name: 'password',
    className: 'col-span-12 mb-2',
    rules: [{required: true, message: 'Password is required'}], 
  },
]

function SMTPConfig() {
  const [ data, setData ] = useState(null)
  const [ openModal, setOpenModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ isEdit, setIsEdit ] = useState(false)

  const [api, contextHolder] = notification.useNotification();
  const [ form ] = Form.useForm()

  useEffect(() => {
    getConfigData()
  },[])

  const getConfigData = useCallback(() => {
    axios({
      method: 'get',
      url: 'tas/mailsmtpconfig'
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {
      
    })
  },[])

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/mailsmtpconfig/create',
      data:{
        ...values
      }
    }).then(() => {
      setIsEdit(false)
      getConfigData()
    }).catch(({err}) => {
      if(err.response.status === 400){
        api.error({
          message: 'Error',
          duration: 5,
          description: <div>
            {err.response?.data?.message}
          </div>
        });
      }
    }).then(() => setActionLoading(false))
  }
  const handleCloseModal = useCallback(() => {
    setOpenModal(false)
  },[])

  const fields = useMemo(() => {
    if(isEdit){
      return([
        {
          label: 'SMTP server',
          name: 'smtpServer',
          className: 'col-span-12 mb-2',
          rules: [{required: true, message: 'SMTP Server is required'}], 
        },
        {
          label: 'PORT',
          name: 'smtpPort',
          className: 'col-span-12 mb-2',
          rules: [{required: true, message: 'PORT is required'}], 
        },
        {
          label: 'Email',
          name: 'email',
          className: 'col-span-12 mb-2',
          rules: [{required: true, message: 'Email is required'}, {type: 'email', message: `The input is not valid E-mail!`}], 
        },
        {
          label: 'Password',
          name: 'password',
          type: 'password',
          className: 'col-span-12 mb-2',
          rules: [{required: true, message: 'Password is required'}], 
        },
      ])
    }else{
      return([
        {
          label: 'SMTP server',
          name: 'smtpServer',
          className: 'col-span-12 mb-2',
          rules: [{required: true, message: 'Code is required'}], 
        },
        {
          label: 'PORT',
          name: 'smtpPort',
          className: 'col-span-12 mb-2',
          rules: [{required: true, message: 'Description is required'}], 
        },
        {
          label: 'PORT',
          name: 'email',
          className: 'col-span-12 mb-2',
          rules: [{required: true, message: 'Description is required'}], 
        },
      ])
    }
  }, [isEdit]);

  return (
    <div className='rounded-ot shadow-card bg-white px-3 py-2'>
      <div className='text-lg font-bold mb-3'>SMTP Server Configuration</div>
      <Form 
        form={form}
        fields={fields}
        editData={data}
        onFinish={handleSubmit}
        className='w-[500px]'
        disabled={!isEdit}
      >
        <div className='col-span-12 flex justify-end items-center gap-2'>
          {
            isEdit ? 
            <>
              <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
              <Button onClick={() => setIsEdit(false)} disabled={actionLoading}>Cancel</Button>
            </>
            :
            <Button type='primary' onClick={() => setIsEdit(true)} icon={<EditOutlined/>}>Edit</Button>
          }
        </div>
      </Form>
      <Modal open={openModal} onCancel={handleCloseModal}>
        <Form 
          form={form}
          fields={modalFields}
          editData={data}
          onFinish={handleSubmit}
          className='w-[500px]'
          disabled={!isEdit}
        >
          <div className='col-span-12 flex justify-end items-center gap-2'>
            {
              isEdit ? 
              <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
              :
              <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<EditOutlined/>}>Edit</Button>
            }
            <Button onClick={handleCloseModal} disabled={actionLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      {contextHolder}
    </div>
  )
}

export default SMTPConfig