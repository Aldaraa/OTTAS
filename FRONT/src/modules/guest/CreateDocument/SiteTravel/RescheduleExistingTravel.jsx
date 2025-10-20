import { Button, Form, Modal, Table, TransportSearch } from 'components'
import { Form as AntForm, DatePicker, Input, Select, Tag } from 'antd'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { useParams } from 'react-router-dom'

const { RangePicker } = DatePicker;

function RescheduleExistingTravel({form}) {
  const [ flights, setFlights ] = useState([])
  const [ selectedDate, setSelectedDate ] = useState([null, null])
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ rescheduleData, setRescheduleData ] = useState(null)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ inititalValues, setInitialValues ] = useState(null)
  const [ loadingShift, setLoadingShift ] = useState(false)

  const { state, action } = useContext(AuthContext)
  const { empId } = useParams()

  const DefaultOnSiteShift = state.referData?.roomStatuses.find((item) => item.Code === 'DS')
  const DefaultOutSiteShift = state.referData?.roomStatuses.find((item) => item.Code === 'RR')
  const InLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
  }
  const OutLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
  }

  useEffect(() => {
    if(selectedDate){
      setSearchLoading(true)
      axios({
        method: 'get',
        url: `tas/transport?employeeId=${empId}${selectedDate[0] ? `&startDate=${dayjs(selectedDate[0])?.format('YYYY-MM-DD')}` : ''}${selectedDate[1] ?  `&endDate=${dayjs(selectedDate[1]).format('YYYY-MM-DD')}` : ''}`,
      }).then((res) => {
        setFlights(res.data)
      }).catch((err) => {
    
      }).then(() => setSearchLoading(false))
    }
  },[selectedDate])

  const handleSelectionButtonLT = () => {
    let tmp = {
      StartDate: form.getFieldValue('endDate') ? form.getFieldValue('endDate') : null,
      EndDate: form.getFieldValue('endDate') ? form.getFieldValue('endDate') : null,
    }
    if(form.getFieldValue('Direction') && form.getFieldValue('Direction') === 'IN'){
      tmp = {
        ...tmp,
        DepartLocationId: rescheduleData.fromLocationId,
        ArriveLocationId: rescheduleData.toLocationId,
      }
    }else{
      tmp = {
        ...tmp,
        DepartLocationId: rescheduleData.fromLocationId,
        ArriveLocationId: rescheduleData.toLocationId,
      }
    }
    setTransportSelectionInitValues({...tmp, transportType: 'last'})
    setShowLTSelectionModal(true)
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

  const getOnSiteFutureShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/futurefirststatus/onsite/${empId}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const getOutSiteFutureShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/futurefirststatus/offsite/${empId}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='Existing Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={'startDate'} className='mb-0 col-span-3'>
            <DatePicker disabled className='w-full'/>
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0 col-span-2'>
            <Input disabled className='w-[50px] py-[2px]'/>
            {/* <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/> */}
          </Form.Item>
          <Form.Item name={'startFlight'} className='mb-0 flex-1' rules={[{required: true, message: 'Flight is required'}]}>
            <Input disabled className='py-[2px]'/>
          </Form.Item>
          <Form.Item name={'existingScheduleId'} noStyle>
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='Reschedule Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.startDate !== cur.startDate || prev.endDate !== cur.endDate}>
            {({getFieldValue}) => {
              let defaultPickerValue = false
              if(!getFieldValue('endDate')){
                defaultPickerValue = getFieldValue('startDate')
              }
              return(
                <Form.Item name={'endDate'} className='mb-0 col-span-3'>
                  <DatePicker
                    defaultPickerValue={defaultPickerValue}
                    showWeek
                    className='w-full'
                    onChange={() => {
                      form.setFieldValue('ReScheduleDescription', null);
                      form.setFieldValue('ReScheduleId', null)
                    }}
                  />
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0'>
            <Input disabled className='w-[50px] py-[2px]'/>
            {/* <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/> */}
          </Form.Item>
          <Form.Item noStyle name={'ReScheduleId'}>
          </Form.Item>
          <Form.Item name={'ReScheduleDescription'} className='flex-1 mb-0'>
            <Input readOnly className='py-[2px]'/>
          </Form.Item>
          <Form.Item noStyle shouldUpdate={(pre, cur) => pre.endDate !== cur.endDate}>
            {({getFieldValue}) => {
              const endDate = getFieldValue('endDate');
              return(
                <Form.Item className='col-span-1 mb-0'>
                  <Button disabled={!endDate} className='text-xs py-[3px]' type={'primary'} onClick={handleSelectionButtonLT}>...</Button>
                </Form.Item>
              )
            }}
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.endDate !== curValues.endDate}>
        {({getFieldValue, setFieldValue}) => {
          let shiftType = null
          const endDate = getFieldValue('endDate')
          if(endDate && rescheduleData.EventDate){
            if(dayjs(endDate) - dayjs(rescheduleData.EventDate) > 0){
              // extend the schedule
              if(getFieldValue('Direction') === 'IN'){
                shiftType = 0
                getOutSiteShift(dayjs(endDate).format('YYYY-MM-DD'))
              }
              else{
                shiftType = 1
                getOnSiteShift(dayjs(endDate).format('YYYY-MM-DD'))
              }
            }else{
              // shorten the schedule
              if(getFieldValue('Direction') === 'IN'){
                shiftType = 1
                getOnSiteFutureShift(dayjs(endDate).format('YYYY-MM-DD'))
              }
              else{
                shiftType = 0
                getOutSiteFutureShift(dayjs(endDate).format('YYYY-MM-DD'))
              }
            }
          }
          return(
            <Form.Item 
              label='Shift Status'
              name={'ShiftId'}
              className='col-span-12 mb-2'
              rules={[{required: true, message: 'Shift is required'}]}
            >
              <Select
                options={typeof shiftType === 'number' ? state.referData?.roomStatuses?.filter((item) => item.OnSite === shiftType) : state.referData?.roomStatuses}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                className='w-full'
                showSearch
                allowClear
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      label: 'Reason',
      name: 'Reason',
      className: 'col-span-12',
      type: 'textarea',
      inputprops: {
        showCount: true,
        maxLength: 300,
      }
    }
  ]

  const handleClickReschedule = (rowData) => {
    setRescheduleData(rowData)
    let prevValues = form.getFieldsValue()
    prevValues={
      ...prevValues,
      startDate: dayjs(rowData.EventDate), 
      Direction: rowData.Direction,
      existingScheduleId: rowData.ScheduleId,
      startFlight: `${rowData.Code} ${rowData.Description}`,
      ShiftId: rowData.Direction === 'IN' ? DefaultOnSiteShift.Id : DefaultOutSiteShift.Id,
      // ShiftId: null,
      endDate: null,
      ReScheduleId: null,
    }
    form.setFieldsValue(prevValues)
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
        <button type='button' onClick={() => handleClickReschedule(e.data)} className='edit-button'>
          Select
        </button>
      )
    },
  ]

  const handleSelect = (event) => {
    if(transportSelectionInitValues.transportType === 'first'){
      form.setFieldValue('ReScheduleDescription', `${event.Code} ${event.Description}`)
      form.setFieldValue('ReScheduleId', event.Id)
      setShowLTSelectionModal(false)
    }else{
      form.setFieldValue('ReScheduleDescription', `${event.Code} ${event.Description}`)
      form.setFieldValue('ReScheduleId', event.Id)
      setShowLTSelectionModal(false)
    }
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
        rescheduleData &&
        <div className='px-5 max-w-[700px]'>
          <div className='text-lg font-bold mb-3'>Reschedule Existing Travel</div>
          <Form
            size='small'
            labelCol={{flex: '150px'}}
            form={form}
            fields={fields}
            editData={rescheduleData}
            className='grid grid-cols-12 gap-y-2'
            initValues={inititalValues}
          >
          </Form>
        </div>
      }
      <Modal 
        title='Select Transport'
        open={showLTSelection}
        width={900}
        onCancel={() => setShowLTSelectionModal(false)}
        destroyOnClose={false}
      >
        <TransportSearch
          initialSearchValues={transportSelectionInitValues}
          handleSelect={handleSelect}
          directionDisabled={false}
        />
      </Modal>
    </div>
  )
}

export default RescheduleExistingTravel