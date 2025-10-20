import { LoadingOutlined, SaveOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import dayjs from 'dayjs'
import React, { useCallback, useEffect, useMemo, useState } from 'react'

const formLayout = {
  labelCol: {
    xs: { span: 24 },
    sm: { span: 10 },
  },
  wrapperCol: {
    xs: { span: 24 },
    sm: { span: 16 },
  },
}

function ExtendModal({open=false, transportMode, carrier, location, onReload, onCancel, editData}) {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ form ] = Form.useForm()

  useEffect(() => {
    if(editData && open){
      getData()
    }
  },[editData, open])

  const getData = useCallback(() => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/activetransport/extendinfo/${editData.Id}`,
    }).then((res) => {
      setData(res.data)
      form.setFieldsValue({
        ...res.data,
      endDate: null,
      startDate: dayjs(res.data.ScheduleStartDate)
      })
    }).catch((err) => {

    }).finally(() => {
      setLoading(false)
    })
  },[editData])

  const fields = useMemo(() => {
    return [
      {
        label: 'Mode',
        name: 'TransportMode',
        className: 'col-span-6 mb-2',
        inputprops: {
          disabled: true,
        }
      },
      {
        label: 'Carrier',
        name: 'Carrier',
        className: 'col-span-6 mb-2',
        inputprops: {
          disabled: true,
        }
      },
      {
        label: 'Transport Code',
        name: 'Code',
        className: 'col-span-6 mb-2',
        inputprops: {
          disabled: true,
        }
      },
      {
        label: 'Seat',
        name: 'Seat',
        className: 'col-span-6 mb-2',
        rules: [{required: true, message: 'Seat is required'}],
        inputprops: {
        }
      },
      {
        label: 'Port Of Departure',
        name: 'FromLocation',
        className: 'col-span-6 mb-2',
        inputprops: {
          disabled: true,
        }
      },
      {
        label: 'Port Of Arrive',
        name: 'ToLocation',
        className: 'col-span-6 mb-2',
        inputprops: {
          disabled: true,
        }
      },
      {
        label: 'Week day',
        name: 'DayNum',
        className: 'col-span-6 mb-2',
        inputprops: {
          disabled: true,
        }
      },
      {
        label: 'Frequency Weeks',
        name: 'FrequencyWeeks',
        className: 'col-span-6 mb-2',
        type: 'number',
        inputprops: {
          min: 0,
          disabled: true
        }
      },
      {
        label: 'ETD',
        name: 'ETD',
        className: 'col-span-6 mb-2',
        rules: [{required: true, message: 'ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}],
        inputprops: {
          maxLength: 4
        }
      },
      {
        label: 'ETA',
        name: 'ETA',
        className: 'col-span-6 mb-2',
        rules: [{required: true, message: 'ETA is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}],
        inputprops: {
          maxLength: 4
        }
      },
      {
        label: 'Start Date',
        name: 'startDate',
        className: 'col-span-6 mb-2',
        rules: [{required: true, message: 'Start Date is required'}],
        type: 'date',
        inputprops: {
          className: 'w-full',
          showWeek: true,
          disabled: true
        }
      },
      {
        label: 'End Date',
        name: 'endDate',
        className: 'col-span-6 mb-2',
        rules: [{required: true, message: 'End Date is required'}],
        type: 'date',
        inputprops: {
          className: 'w-full',
          showWeek: true,
          disabledDate: (current) => current < dayjs(data?.ScheduleStartDate).subtract(1, 'day').endOf('day') || current > dayjs(data?.ScheduleStartDate).add(18, 'month').endOf('day'),
          defaultPickerValue: dayjs(data?.ScheduleStartDate).add(18, 'month'),
        }
      },
    ]
  },[transportMode, carrier, location, data])


  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/activetransport/extend',
      data: {
        ...values,
        seats: values.Seat,
        activeTransportId: editData?.Id,
        startDate: dayjs(values.startDate).format('YYYY-MM-DD'),
        endDate: dayjs(values.endDate).format('YYYY-MM-DD'),
      }
    }).then((res) => {
      onCancel()
      onReload()
    }).catch((err) => {

    }).finally((err) => {
      setActionLoading(false)
    })
  }

  return (
    <Modal open={open} onCancel={onCancel} width={900} title={'Extend Transport'}>
      {
        loading ? 
        <div className='flex flex-col justify-center items-center h-[240px]'>
          <LoadingOutlined style={{fontSize: 24}}/>
          <span>Loading...</span>
        </div>
        :
        <Form
          form={form}
          fields={fields} 
          onFinish={handleSubmit}
          className={'gap-x-4'}
          {...formLayout}
        >
          <div className='col-span-12 flex justify-end items-center gap-2 mt-4'>
            <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={onCancel} disabled={actionLoading}>Cancel</Button>
          </div>
        </Form>
      }
    </Modal>
  )
}

export default ExtendModal