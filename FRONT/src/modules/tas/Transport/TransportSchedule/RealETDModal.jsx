import { Button, Form, Modal } from 'components'
import React, { useState } from 'react'
import { SaveOutlined } from '@ant-design/icons'
import axios from 'axios'

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

function RealETDModal({rowData, onReset, onCancel, ...restprops}) {
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ form ] = Form.useForm()

  const fields = [
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
      url: 'tas/transportschedule/realetd',
      data: {
        ...values,
        id: rowData.Id,
      }
    }).then((res) => {
      onReset()
      onCancel()
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
        editData={rowData}
        onFinish={handleSubmit}
        labelCol={{flex: '100px'}}
      >
        <div className='col-span-12 gap-3 flex justify-end mt-3'>
          <Button type='primary' onClick={() => form.submit()} loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
          <Button onClick={onCancel} disabled={submitLoading}>Cancel</Button>
        </div>
      </Form>
    </Modal>
  )
}

export default RealETDModal