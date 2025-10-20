import { SaveOutlined } from '@ant-design/icons'
import { DatePicker, Input, InputNumber } from 'antd'
import axios from 'axios'
import { Button, Form } from 'components'
import dayjs from 'dayjs'
import React, { useState } from 'react'

function EditOptionForm({editData=null, onCancel, getOptionsData, getData}) {
  const [ loading, setLoading ] = useState(false)
  const [ form ] = Form.useForm()

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestnonsitetraveloption/fulldata',
      data: {
        ...values,
        id: editData.Id,
        DueDate: dayjs(values.DueDate).format('YYYY-MM-DD HH:mm'),
      }
    }).then((res) => {
      if(onCancel){
        onCancel()
      }
      getOptionsData()
      getData()
      form.resetFields()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  return (
    <div>
      <Form
        form={form}
        editData={editData}
        size='small' 
        className='grid grid-cols-12 gap-x-4 gap-y-3' 
        onFinish={handleSubmit}
        disabled={false}
        layout='vertical'
      >
        <Form.Item
          className='col-span-6 mb-0'
          name='DueDate'
          label='Deadline'
          rules={[{required: true, message: 'Due Date is required !'}]}
        >
          <DatePicker className='w-full' showTime showSecond={false} format='YYYY-MM-DD HH:mm'/>
        </Form.Item>
        <Form.Item label='Cost' name={'Cost'} className='col-span-6 mb-0'>
          <InputNumber className='w-full' controls={false} formatter={value => `${new Intl.NumberFormat().format(value)}`}/>
        </Form.Item>
        <Form.Item label='Itinerary' name={'OptionData'} className='col-span-12 mb-2'>
          <Input.TextArea 
            autoSize={{minRows: 3, maxRows: 10}}
          />
        </Form.Item>
        <div className='col-span-12 flex justify-end gap-3'>
          <Button htmlType='submit' loading={loading} type={'primary'} icon={<SaveOutlined/>}>Save</Button>
          <Button htmlType='button' onClick={onCancel} disabled={loading}>Cancel</Button>
        </div>
      </Form>
    </div>
  )
}

export default EditOptionForm