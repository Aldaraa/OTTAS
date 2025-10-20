import { DownloadOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import dayjs from 'dayjs'
import React, { useState } from 'react'
import { useParams } from 'react-router-dom'
import { saveAs } from 'file-saver-es'

function AuditModal({open, onCancel, roomData}) {
  const { roomId, startDate, endDate } = useParams()
  const [ form ] = Form.useForm()
  const [ loading, setLoading ] = useState(false)

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/audit/roomauditbyroom',
      responseType: 'blob',
      data: {
        roomId: roomId,
        startDate: values.startDate,
        endDate: values.endDate,
      }
    }).then((res) => {   
      const fn = `TAS_RoomAuditByRoom_${roomData?.Number}_${roomData?.CampName}_${dayjs(values.startDate).format('YYYY-MM-DD')}_${dayjs(values.endDate).format('YYYY-MM-DD')}.xlsx`
      saveAs(res.data, fn)
      onCancel()
    }).catch(() => {

    }).finally(() => setLoading(false))
  }

  const fields = [
    {
      name: 'startDate', 
      label: 'StartDate',
      className: 'col-span-6 mb-0',
      type: 'date'
    },
    {
      name: 'endDate', 
      label: 'EndDate',
      className: 'col-span-6 mb-0',
      type: 'date'
    },
  ]
  return (
    <Modal title='Audit' open={open} onCancel={onCancel}>
      <Form 
        form={form}
        fields={fields}
        onFinish={handleSubmit}
        initValues={{
          startDate: startDate ? dayjs(startDate) : dayjs().subtract(1, 'month'), 
          endDate: endDate ? dayjs(endDate) : dayjs()
        }}
      >
        <div className='col-span-12 flex justify-end gap-4 mt-4'>
          <Button loading={loading} htmlType='submit' type='primary' icon={<DownloadOutlined/>}>Download</Button>
          <Button onClick={onCancel} disabled={loading}>Cancel</Button>
        </div>
      </Form>
    </Modal>
  )
}

export default AuditModal