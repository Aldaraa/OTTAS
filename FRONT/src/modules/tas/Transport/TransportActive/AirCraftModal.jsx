import { SaveOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import React, { useCallback, useEffect, useState } from 'react'

const fields = [
  {
   name: 'AircraftCode',
   label: 'Air Craft Code',
   className: 'col-span-12 mb-2'
  }
 ]

function AirCraftModal({editData, refresh,...restprops}) {
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ form ] = Form.useForm()

  useEffect(() => {
    form.setFieldsValue(editData)
  },[editData])

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: 'tas/activetransport/aircraftcode',
      data: {
        ...values,
        id: editData?.Id,
      }
    }).then((res) => {
      restprops.onCancel()
      refresh()
    }).catch((err) => {

    }).then(() => {
      setActionLoading(false)
    })
  }

  const handleCancel = useCallback(() => {
    restprops?.onCancel()
  },[])

  return (
    <Modal width={600} title={<div>Change Air Craft Code <span className='text-secondary2'>/ {editData?.Code} /</span></div>} {...restprops}>
      <Form form={form} fields={fields} layout='vertical' noLayoutConfig={true} onFinish={handleSubmit}>
        <div className='col-span-12 flex justify-end items-center gap-2'>
          <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
          <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
        </div>
      </Form>
    </Modal>
  )
}

export default AirCraftModal