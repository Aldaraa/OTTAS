import { Form, Table } from 'components'
import { Checkbox, DatePicker, Input, Select, Tag } from 'antd'
import React, { useContext, useEffect, useState } from 'react'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { useParams } from 'react-router-dom'
import COLORS from 'constants/colors'

const { RangePicker } = DatePicker;

function RemoveExistingTravel({form}) {
  const [ flights, setFlights ] = useState([])
  const [ selectedDate, setSelectedDate ] = useState([null, null])
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ rescheduleData, setRescheduleData ] = useState(null)
  
  const { state } = useContext(AuthContext)
  const { empId } = useParams()

  useEffect(() => {
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/transport?employeeId=${empId}${selectedDate[0] ? `&startDate=${dayjs(selectedDate[0])?.format('YYYY-MM-DD')}` : ''}${selectedDate[1] ?  `&endDate=${dayjs(selectedDate[1]).format('YYYY-MM-DD')}` : ''}`,
    }).then((res) => {
      setFlights(res.data.map((item) => ({...item, label: dayjs(item.EventDate).format('YYYY-MM-DD')})))
    }).catch((err) => {
  
    }).then(() => setSearchLoading(false))
  },[selectedDate])
  
  const handleRemoveButton = (dataItem, e) => {
    if(dataItem.Direction === 'EXTERNAL'){
      setRescheduleData(dataItem)
      form.setFieldsValue(state.userProfileData)
      form.setFieldValue(['firstTransport', 'Date'],dayjs(dataItem.EventDate))
      form.setFieldValue(['firstTransport', 'Direction'],dataItem.Direction)
      form.setFieldValue(['firstTransport', 'Description'],dataItem.Description)
      form.setFieldValue(['firstTransport', 'firstScheduleId'], dataItem.Id)
    }else{
      let lastTransport = flights[e.rowIndex+1]
      setRescheduleData(dataItem)
      form.setFieldsValue(state.userProfileData)
      form.setFieldValue('ShiftId', null)
      form.setFieldValue(['firstTransport', 'Date'],dayjs(dataItem.EventDate))
      form.setFieldValue(['firstTransport', 'Direction'],dataItem.Direction)
      form.setFieldValue(['firstTransport', 'Description'],dataItem.Description)
      form.setFieldValue(['firstTransport', 'firstScheduleId'],dataItem.ScheduleId)
      form.setFieldValue(['lastTransport', 'Date'],dayjs(lastTransport.EventDate).format('YYYY-MM-DD'))
      form.setFieldValue(['lastTransport', 'Direction'],lastTransport.Direction)
      form.setFieldValue(['lastTransport', 'Description'],lastTransport.Description)
      form.setFieldValue(['lastTransport', 'lastScheduleId'],lastTransport.ScheduleId)
    }
  }

  const column = [
    {
      label: 'Date',
      name: 'EventDate',
      cellRender: (e) => (
        <div>{dayjs(e.data.EventDate).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Transport Mode',
      name: 'TransportMode',
    },
    {
      label: 'Code',
      name: 'Code',
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Direction',
      name: 'Direction',
      cellRender: ({value}) => (
        <Tag className='text-xs' color={COLORS.Directions[value]?.tagColor}>{value}</Tag>
      )
    },
    {
      label: 'Status',
      name: 'Status',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'success' : 'orange'} className='text-xs'>{e.value}</Tag>
      )
    },
    {
      label: 'Reschedule',
      name: '',
      alignment: 'center',
      cellRender: (e) => (
        flights.length-1 !== e.rowIndex && e.data.Direction !== 'EXTERNAL' &&
        <button type='button' onClick={() => handleRemoveButton(e.data, e)} className='edit-button'>
          Select
        </button>
      )
    },
  ]

  const handleChangeLastTransport = (schedule) => {
    form.setFieldValue(['lastTransport', 'Description'], schedule.Description)
    form.setFieldValue(['lastTransport', 'Direction'], schedule.Direction)
    form.setFieldValue(['lastTransport', 'lastScheduleId'], schedule.ScheduleId)
  }

  const getOnSiteShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/onsite/${empId}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const getOutSiteShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/offsite/${empId}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  return (
    <div className='flex flex-col gap-6'>
      <div className='flex flex-col border rounded-ot'>
        <div className='flex px-2 py-2 justify-between items-center leading-none'>
          <div className='items-start relative font-bold text-lg'>Choose Travel</div>
          <RangePicker
            value={selectedDate}
            onChange={(e) => setSelectedDate(e)}
          />
        </div>
        <Table
          data={flights}
          columns={column}
          allowColumnReordering={false}
           loading={searchLoading}
          pager={flights.length > 20}
          containerClass='shadow-none border-t rounded-t-none'
          keyExpr='EventDate'
        />
      </div>
      {
        rescheduleData ?
        <div className='px-5 max-w-[700px]'>
          <div className='text-lg font-bold mb-3'>Remove Existing Travel</div>
          <div className='grid grid-cols-12 gap-y-2'>
            <Form.Item label='First Transport' labelCol={{flex: '150px'}} className='col-span-12 mb-2'>
              <div className='flex gap-2'>
                <Form.Item name={['firstTransport', 'Date']} className='mb-0 w-[120px]'>
                  <DatePicker disabled/>
                </Form.Item>
                <Form.Item name={['firstTransport', 'Direction']} className='mb-0 w-[70px]'>
                  <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]}/>
                </Form.Item>
                <Form.Item className='mb-0 flex-1' name={['firstTransport', 'Description']} rules={[{required: true, message: 'Flight is required'}]}>
                  <Input disabled className='w-full'/>
                </Form.Item>
                <Form.Item name={['firstTransport', 'firstScheduleId']} noStyle className='mb-0' rules={[{required: true, message: 'Flight is required'}]}>
                </Form.Item>
                <Form.Item 
                  className='mb-0' 
                  name={['firstTransport', 'NoShow']}
                  label=''
                  getValueFromEvent={(e) => e.target.checked ? 1 : 0}
                >
                  <Checkbox>
                    No Show
                  </Checkbox>
                </Form.Item>
              </div>
            </Form.Item>
            <Form.Item label='Last Transport' labelCol={{flex: '150px'}} className='col-span-12 mb-2'>
              <div className='flex gap-2'>
                <Form.Item name={['lastTransport', 'Date']} className='mb-0 w-[120px]'>
                  <Select
                    options={flights.filter((item) => item.Direction !== rescheduleData?.Direction && dayjs(item.EventDate).diff(dayjs(rescheduleData.EventDate)) > 0)}
                    fieldNames={{value: 'EventDate', label: 'label'}}
                    onChange={(e, option) => handleChangeLastTransport(option)}
                  />
                </Form.Item>
                <Form.Item name={['lastTransport', 'Direction']} className='mb-0 w-[70px]'>
                  <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/>
                </Form.Item>
                <Form.Item noStyle name={['lastTransport', 'lastScheduleId']}>
                </Form.Item>
                <Form.Item name={['lastTransport', 'Description']} className='mb-0 flex-1'>
                  <Input disabled/>
                </Form.Item>
                <Form.Item 
                  className='mb-0' 
                  name={['lastTransport', 'NoShow']}
                  label=''
                  getValueFromEvent={(e) => e.target.checked ? 1 : 0}
                >
                  <Checkbox>
                    No Show
                  </Checkbox>
                </Form.Item>
              </div>
            </Form.Item>
            <Form.Item noStyle shouldUpdate={(prev, cur) => prev.firstTransport.Description !== cur.firstTransport.Description || prev.lastTransport.Date !== cur.lastTransport.Date}>
              {({getFieldValue}) => {
                let shiftType = null
                const firstDate = getFieldValue(['firstTransport', 'Date'])
                const firstDirection = getFieldValue(['firstTransport', 'Direction'])
                if(firstDirection === 'IN'){
                  shiftType = 0
                  if(firstDate) getOutSiteShift(dayjs(firstDate).format('YYYY-MM-DD'))
                }else{
                  shiftType = 1
                  if(firstDate) getOnSiteShift(dayjs(firstDate).format('YYYY-MM-DD'))
                }

                return(
                  <Form.Item 
                    className='col-span-12 mb-2' 
                    name='ShiftId' 
                    label='Shift Status'
                    labelCol={{flex: '150px'}}
                    rules={[{required: true, message: 'Shift Status is required'}]}
                  >
                    <Select 
                      options={typeof shiftType === 'number' ? state.referData?.roomStatuses.filter((row) => row.OnSite === shiftType) : null}
                      showSearch
                      allowClear
                    />
                  </Form.Item>
                )
              }}
            </Form.Item>
            <Form.Item 
              className='col-span-12 mb-2' 
              name='CostCodeId' 
              label='Cost Code'
              labelCol={{flex: '150px'}}
              rules={[{required: true, message: 'Cost Code is required'}]}
            >
              <Select 
                options={state.referData?.costCodes}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                showSearch
                allowClear
              />
            </Form.Item>
            <Form.Item 
              className='col-span-12 mb-2' 
              name='Reason' 
              label='Reason'
              labelCol={{flex: '150px'}}
              rules={[{required: true, message: 'Reason is required'}]}
            >
              <Input.TextArea
                maxLength={300}
                showCount
              />
            </Form.Item>
          </div>
        </div>
        :
        null
      }
    </div>
  )
}

export default RemoveExistingTravel