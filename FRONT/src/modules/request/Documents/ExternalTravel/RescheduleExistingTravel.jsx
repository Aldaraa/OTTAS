import { Button, Form, Tooltip } from 'components'
import { Form as AntForm, DatePicker, Input, Select } from 'antd'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'

function RescheduleExistingTravel({data, refreshData, disabled, currentGroup}) {
  const [ flights, setFlights ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ isEdit, setIsEdit ] = useState(false)

  const { state, action } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()

  const initialValues = useMemo(() => {
    if(data){
      return {
        startDate: data.ExistingScheduleDate ? dayjs(data.ExistingScheduleDate) : null, 
        endDate: data.ReScheduleDate ? dayjs(data.ReScheduleDate) : null, 
        ShiftId: data.shiftId, 
        Direction: data.ExistingScheduleDirection, 
        ExistingScheduleDescription: data.ExistingScheduleDescription,
        ReScheduleId: data.ReScheduleId,
        RoomId: state.userProfileData?.RoomId ? state.userProfileData?.RoomId : state.referData.noRoomId?.Id,
        RoomNumber: state.userProfileData?.RoomNumber ? state.userProfileData?.RoomNumber : 'Virtual room',
      }
    }
  },[data]) 

  useEffect(() => {
    if(data){
      axios({
        method: 'get',
        url: `tas/ActiveTransport/getdatetransport?eventDate=${data.ReScheduleDate}&Direction=${data.ExistingScheduleDirection}`
      }).then((res) => {
        let tmp = []
        res.data.map((item) => {
          tmp.push({ value: item.ScheduleId, label: `${item.Description} • ${item.Seat}/${item.BookedCount}`})
        })
        setFlights(tmp)
      })
    }
  },[data])

  const getFlights = (date) => {
    if(date){
      axios({
        method: 'get',
        url: `tas/ActiveTransport/getdatetransport?eventDate=${date}&Direction=${data.ExistingScheduleDirection}`
      }).then((res) => {
        let tmp = []
        res.data.map((item) => {
          tmp.push({ value: item.ScheduleId, label: `${item.Description} • ${item.Seat}/${item.BookedCount}`})
        })
        form.setFieldValue('ReScheduleId', null)
        setFlights(tmp)
      })
    }
  }

  const fields = [
    {
      type: 'component',
      component: <Form.Item label='Existing Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={'startDate'} className='mb-0 w-[115px]'>
            <DatePicker disabled className='w-full'/>
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0 w-[60px]'>
            <Input disabled className='w-full py-[1px]'/>
          </Form.Item>
          <Tooltip
            title={
              <div>
                <div className='flex gap-2'>
                  <span className='text-gray-500'>Seats:</span> {data?.ExistingScheduleSeat}
                </div>
                <div className='flex gap-2'>
                  <span className='text-gray-500'>Booked:</span> {data?.ExistingScheduleBookedCount}
                </div>
                <div className='flex gap-2'>
                  <span className='text-gray-500'>Available Seats:</span>
                  <span>
                    {
                      data?.ExistingScheduleSeat > data?.ExistingScheduleBooked 
                      ? 
                      data?.ExistingScheduleSeat - data?.ExistingScheduleBooked 
                      : 0
                    }
                  </span>
                </div>
              </div>
            }
          >
            <Form.Item name={'ExistingScheduleDescription'} className='flex-1 mb-0' rules={[{required: true, message: 'Flight is required'}]}>
              <Input disabled className='py-[1px]'/>
            </Form.Item>
          </Tooltip>
        </div>
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item label='Reschedule Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.startDate !== cur.startDate || prev.endDate !== cur.endDate}>
            {({getFieldValue}) => {
              let defaultPickerValue = false
              if(!getFieldValue('endDate')){
                defaultPickerValue = getFieldValue('startDate')
              }
              return(
                <Form.Item name={'endDate'} className='mb-0 w-[115px]'>
                  <DatePicker
                    defaultPickerValue={defaultPickerValue}
                    showWeek
                    className='w-[115px]'
                    onChange={(date, string) => getFlights(string)}
                  />
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0 w-[60px]'>
            <Input disabled className='w-[60px] py-[1px]'/>
          </Form.Item>
          <Form.Item name={'ReScheduleId'} className='mb-0 flex-1 w-full max-w-[360px]' rules={[{required: true, message: 'Flight is required'}]}>
            <Select
              options={flights}
              popupMatchSelectWidth={false}
              allowClear
              style={{width: '100%'}}
              showSearch
              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            />
          </Form.Item>
        </div>
      </Form.Item>
    },
  ]

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestexternaltravel/reschedule',
      data: {
        id: data?.Id,
        newScheduleId: values.ReScheduleId,
        CostCodeId: state.userProfileData?.CostCodeId,
        DepartmentId: state.userProfileData?.DepartmentId,
      }
    }).then((res) => {
      refreshData()
      setIsEdit(false)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCancel = () => {
    setIsEdit(false)
  }

  return (
    <div className='w-full'>
      <Form
        form={form}
        fields={fields}
        onFinish={handleSubmit}
        editData={initialValues}
        disabled={!isEdit}
        className='grid grid-cols-12 gap-x-5 max-w-[700px]'
        size='small'
        labelAlign='left'
        labelCol={{ flex: '150px' }}
      >
        {
          !disabled &&  currentGroup?.GroupTag !== 'Completed' &&
          <div className='col-span-12 flex justify-end gap-5 text-xs'>
            {
              isEdit ?
              <>
                <Button htmlType='button' onClick={() => form.submit()} type='success' loading={loading}>Save</Button>
                <Button htmlType='button' onClick={handleCancel} disabled={loading}>Cancel</Button>
              </>
              :
              <>
                <Button 
                  htmlType='button' 
                  onClick={() => setIsEdit(true)} 
                  type='primary' 
                >
                  Edit
                </Button>
              </>
            }
          </div>
        }
      </Form>
    </div>
  )
}

export default RescheduleExistingTravel