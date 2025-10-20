import React, { useContext, useEffect } from 'react'
import { Form } from 'components'
import { Checkbox, DatePicker, Input } from 'antd'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'

function RemoveExistingTravel({form, editData, disabled}) {
  const { state, action } = useContext(AuthContext)

  useEffect(() => {
    let initialvalues = {}
    if(editData){
      initialvalues = {
        firstTransport: {
          flightId: editData.ScheduleId,
          Direction: editData.ScheduleDirection,
          Date: editData.ScheduleDate ? dayjs(editData.ScheduleDate) : null,
          Description: editData.ScheduleDescription,
        },
      }
      form.setFieldsValue(initialvalues)
    }
  },[editData])


  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='First Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={['firstTransport', 'Date']} className='mb-0 w-[115px]'>
            <DatePicker disabled className='w-full'/>
          </Form.Item>
          <Form.Item name={['firstTransport', 'Direction']} className='mb-0 w-[100px]'>
            <Input disabled className='w-full'/>
          </Form.Item>
          <Form.Item className='mb-0' name={['firstTransport', 'Description']} rules={[{required: true, message: 'Flight is required'}]}>
            <Input disabled/>
          </Form.Item>
          <Form.Item noStyle name={['firstTransport', 'flightId']}>
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
  ]

  return (
    <div className='w-full max-w-[1200px]'>
      <Form
        form={form} 
        fields={fields} 
        size='small'
        labelAlign='right'
        labelCol={{span: 4}}
        wrapperCol={{span:14}}
        disabled={disabled}
      >
        <Form.Item
          key={`form-item-check`}
          valuePropName="checked"
          label='No Show'
          name='noShow'
          className='col-span-12 mb-2'
        >
          <Checkbox
            disabled
            onChange={(e) => {form.setFieldValue('noShow', e.target.checked ? 1 : 0);}}
          />
        </Form.Item>
      </Form>
    </div>
  )
}

export default RemoveExistingTravel