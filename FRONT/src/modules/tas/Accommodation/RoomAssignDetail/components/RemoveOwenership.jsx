import { DatePicker } from 'antd'
import { Button, Form } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useState } from 'react'
import axios from 'axios'
import { AuthContext } from 'contexts'

function RemoveOwenership({data, changeTab}) {
  const [ actionLoading, setActionLoading ] = useState(false)
  const { action, state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  const ownershipFields = [
    {
      type: 'component',
      component: <Form.Item name={'StartDate'} className='mb-2 col-span-12' label='Start Date' rules={[{required: true, message: 'Start date is required'}]}>
        <DatePicker/>
      </Form.Item>
    },
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/roomassignment/removeownership',
      data: {
        employeeId: data.Id,
        startDate: dayjs(values.StartDate).format('YYYY-MM-DD'), 
      }
    }).then((res) => {
      form.resetFields()
      action.changedFlight(state.ChangedFlight + 1)
    }).catch((err) => {
      
    }).then(() => setActionLoading(false))
  }


  return (
    <>
      <Form
        className={`mt-4 max-w-[500px] gap-x-4`} 
        fields={ownershipFields}
        onFinish={handleSubmit}
        form={form}
        labelCol={{flex: '150px'}}
        wrapperCol={{flex: 1}}
        initValues={{StartDate: dayjs()}}
      >
        <Form.Item className='col-span-12 flex justify-end mt-2'>
          <Button htmlType='submit' type={'primary'} loading={actionLoading}>Process</Button>
        </Form.Item>
      </Form>
    </>
  )
}

export default RemoveOwenership