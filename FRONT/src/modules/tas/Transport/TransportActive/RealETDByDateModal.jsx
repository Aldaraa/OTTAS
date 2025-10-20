import { Button, Form, Modal } from 'components'
import React, { useEffect, useState } from 'react'
import { SaveOutlined } from '@ant-design/icons'
import axios from 'axios'
import dayjs from 'dayjs'

const formLayout = {
  labelCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 10,
    },
  },
  wrapperCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 16,
    },
  },
}

function RealETDByDateModal({rowData, scheduleDate, onReset, onCancel, ...restprops}) {
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ form ] = Form.useForm()

  useEffect(() => {
    form.setFieldsValue({
      ...rowData,
      scheduleDate: scheduleDate})
  },[scheduleDate, rowData])

  const fields = [
    {
      name: 'scheduleDate',
      label: 'Schedule Date',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Schedule Date is required'}],
      type: 'date',
      inputprops: {
        maxLength: 4,
        disabled: true,
        className: 'w-[200px]'
      }
    },
    {
      name: 'RealETD',
      label: 'Real ETD',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Real ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}],
      inputprops: {
        maxLength: 4,
        className: 'w-[100px]'
      }
    },
    {
      name: 'Remark',
      label: 'Remark',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: [
          { label: "Airport Operation OT", value: "Airport Operation OT" },
          { label: "Airport Operation UB", value: "Airport Operation UB" },
          { label: "Aircraft shortage", value: "Aircraft shortage" },
          { label: "Connection flight delay", value: "Connection flight delay" },
          { label: "Crew Delay", value: "Crew Delay" },
          { label: "Crew Shortage", value: "Crew Shortage" },
          { label: "DE-Icing", value: "DE-Icing" },
          { label: "Due to bad weather condition", value: "Due to bad weather condition" },
          { label: "Due to PAX delay", value: "Due to PAX delay" },
          { label: "Due to Traffic jam", value: "Due to Traffic jam" },
          { label: "Unplanned maintenance", value: "Unplanned maintenance" }
        ]
      }
    },
  ]

  const handleSubmit = (values) => {
    setSubmitLoading(true)
    axios({
      method: 'put',
      url: 'tas/transportschedule/realetdbydate',
      data: {
        ...values,
        scheduleDate: scheduleDate ? dayjs(scheduleDate).format('YYYY-MM-DD') : null,
        activeTransportId: rowData.Id,
      }
    }).then((res) => {
      onCancel()
      onReset()
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
  }

  return (
    <Modal
      {...restprops}
      destroyOnClose={false}
    >

      <Form
        fields={fields}
        className='gap-x-5'
        form={form}
        onFinish={handleSubmit}
        labelCol={{flex: '100px'}}
      >
        <div className='col-span-12 gap-3 flex justify-end mt-3'>
          <Button type='primary' htmlType='submit' loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
          <Button onClick={onCancel} disabled={submitLoading}>Cancel</Button>
        </div>
      </Form>
    </Modal>
  )
}

export default RealETDByDateModal