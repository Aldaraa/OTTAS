import { InfoCircleFilled } from '@ant-design/icons'
import { Input, InputNumber, Select } from 'antd'
import axios from 'axios'
import { Button, Form } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useState } from 'react'
import { useParams } from 'react-router-dom'

function UpdateItinerary({getData, setTabKey, getFinalOptionsData}) {
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ form ] = Form.useForm()

  const { documentId } = useParams()
  const { state } = useContext(AuthContext)

  const updateOptions = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/requestnonsitetraveloption/updateitinerary',
      data: {
        documentId: documentId,
        optionText: values.optionText,
        additionalCost: values.additionalCost,
        comment: values.Comment,
      }
    }).then((res) => {
      getData()
      getFinalOptionsData()
      form.resetFields()
      setTabKey(2)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  return (
    <div className='p-2 rounded-bl-ot rounded-br-ot border border-gray-300'>
      <div className='flex flex-col gap-2'>
        <div className='w-full my-2 text-gray-400'>
          <InfoCircleFilled className='mr-3'/>
          To enter the final itinerary travel option set, enter the option before [ok] in the format
        </div>
        <Form
          form={form}
          onFinish={updateOptions}
          noLayoutConfig={true}
          layout='vertical'
          disabled={state.userInfo?.Role !== 'TravelAdmin'}
          className='max-w-[600px]'
        >
          <Form.Item label='Itinerary' name={'optionText'} className='col-span-12 mb-2' rules={[{required: true, message: 'Itinerary is required'}]}>
            <Input.TextArea 
              autoSize={{minRows: 3, maxRows: 10}}
            />
          </Form.Item>
          <div className='flex gap-5 col-span-12'>
            <Form.Item label='Additional Cost' name={'additionalCost'} className='w-[200px]' rules={[{required: true, message: 'Additional Cost is required'}]}>
              <InputNumber className='w-full' controls={false} formatter={value => `${new Intl.NumberFormat().format(value)}`}/>
            </Form.Item>
            <Form.Item label='Comment' name={'Comment'} className='w-[200px]' rules={[{required: true, message: 'Comment is required'}]}>
              <Select

              //TODO Sorted
              options = {[
                {value: 'AB-Car rent', label: 'AB-Car rent'},
                {value: 'AB-Train', label: 'AB-Train'},
                {value: 'Date changed', label: 'Date changed'},
                {value: 'Fully refunded', label: 'Fully refunded'},
                {value: 'International hotel', label: 'International hotel'},
                {value: 'Refunded ARR', label: 'Refunded ARR'},
                {value: 'Refunded DEP', label: 'Refunded DEP'},
                {value: 'Route Added', label: 'Route Added'},
                {value: 'Route changed', label: 'Route changed'}
              ]}
              />
            </Form.Item>
          </div>
          {
            state.userInfo?.Role === 'TravelAdmin' &&
            <div className='col-span-12 flex justify-end items-center gap-2'>
              <Button
                htmlType='button' 
                type='primary' 
                onClick={() => form.submit()} 
                loading={actionLoading}
                className='text-xs'
              >
                Save
              </Button>
            </div>
          }
        </Form>
        
      </div>
    </div>
  )
}

export default UpdateItinerary