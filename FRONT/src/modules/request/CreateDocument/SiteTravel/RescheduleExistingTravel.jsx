import { Button, ButtonDrive, Form, Modal, Table, TransportSearch } from 'components'
import { Checkbox, DatePicker, Input, Select, Tag } from 'antd'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { useParams } from 'react-router-dom'
import COLORS from 'constants/colors'

const { RangePicker } = DatePicker;

function RescheduleExistingTravel({form}) {
  const [ flights, setFlights ] = useState([])
  const [ selectedDate, setSelectedDate ] = useState([null, null])
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ rescheduleData, setRescheduleData ] = useState(null)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ reTransport, setReTransport ] = useState(null)

  const { state } = useContext(AuthContext)
  const { empId } = useParams()
  const dataGrid = useRef(null)

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

  useEffect(() => {
    if(rescheduleData){
      setReTransport({
        ...rescheduleData,
        FromLocationId: rescheduleData.fromLocationId,
        ToLocationId: rescheduleData.toLocationId
      })
    }
  },[rescheduleData])

  const handleSelectionButtonLT = () => {
    let tmp = {
      StartDate: form.getFieldValue('endDate') ? form.getFieldValue('endDate') : null,
      EndDate: form.getFieldValue('endDate') ? form.getFieldValue('endDate') : null,
    }
    if(reTransport){
      tmp.DepartLocationId = reTransport.FromLocationId
      tmp.ArriveLocationId = reTransport?.ToLocationId
    }else{
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
    }
    setTransportSelectionInitValues({...tmp})
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

  const handleChangeDrive = (data) => {
    form.setFieldValue('ReScheduleDescription', `${data.Code} ${data.Description}`)
    form.setFieldValue('endDate', dayjs(data.EventDate))
    form.setFieldValue('ReScheduleId', data.Id)
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
          <Form.Item noStyle name='seats'>
          </Form.Item>
          <Form.Item name={'startFlight'} className='mb-0 flex-1' rules={[{required: true, message: 'Flight is required'}]}>
            <Input disabled className='py-[2px]'/>
          </Form.Item>
          <Form.Item name={'existingScheduleId'} noStyle>
          </Form.Item>
          <Form.Item
            name='ExistingScheduleIdNoShow'
            labelCol={{flex: '150px'}}
            className='col-span-12 mb-0'
            valuePropName="checked"
            getValueFromEvent={(e) => e.target?.checked ? 1 : 0}
          >
            <Checkbox>No Show</Checkbox>
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label={<span><span className='text-red-400'>*</span>Reschedule Transport</span>} className='col-span-12 mb-2'>
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
          <Form.Item name={'ReScheduleDescription'} rules={[{required: true, message: 'ReSchedule Transport is required'}]} className='flex-1 mb-0'>
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
          <Form.Item
            className='col-span-1 mb-0'
            shouldUpdate={(pre, cur) => pre.endDate !== cur.endDate}
          >
            {
              ({getFieldValue}) => {
                const endDate = getFieldValue('endDate');
                const searchValues = {
                  eventDate: endDate,
                  direction: getFieldValue('Direction'),
                }
                return(
                  <ButtonDrive
                    onRecieve={handleChangeDrive}
                    searchValues={searchValues}
                    name='lastTransport'
                    disabled={!endDate}
                  />
                )
              }
            }
          </Form.Item>
          <Form.Item
            className='col-span-1 mb-0'
            shouldUpdate={(pre, cur) => pre.endDate !== cur.endDate}
          >
            {
              ({getFieldValue}) => {
                const endDate = getFieldValue('endDate');
                const searchValues = {
                  eventDate: endDate,
                  direction: getFieldValue('Direction'),
                }
                return(
                  <ButtonDrive
                    onRecieve={handleChangeDrive}
                    searchValues={searchValues}
                    name='lastTransport'
                    disabled={!endDate}
                    mode='evening'
                  />
                )
              }
            }
          </Form.Item>
          <Form.Item
            name='ReScheduleGoShow'
            labelCol={{flex: '150px'}}
            className='col-span-12 mb-0'
            valuePropName="checked"
            getValueFromEvent={(e) => e.target?.checked ? 1 : 0}
          >
            <Checkbox>Go Show</Checkbox>
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
            if(dayjs(endDate).diff(dayjs(rescheduleData.EventDate)) > 0){
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
      className: 'col-span-12 mb-2',
      type: 'textarea',
      rules: [{required: true, message: 'Reason is required'}],
      inputprops: {
        showCount: true,
        maxLength: 300,
      }
    },
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
      ShiftId: null,
      endDate: null,
      ReScheduleId: null,
      ReScheduleDescription: null,
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
        <Tag color={COLORS.Directions[value]?.tagColor}>{value}</Tag>
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
        e.data.Direction !== 'EXTERNAL' ?
        <button type='button' onClick={() => handleClickReschedule(e.data)} className='edit-button'>
          Select
        </button>
        : null
      )
    },
  ]

  const handleSelect = (event) => {
    setReTransport(event)
    form.setFieldValue('ReScheduleDescription', `${event.Code} ${event.Description}`)
    form.setFieldValue('endDate', dayjs(event.EventDate))
    form.setFieldValue('ReScheduleId', event.Id)
    setShowLTSelectionModal(false)
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
          ref={dataGrid}
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
        // rescheduleData?.Direction === 'EXTERNAL' ?
        // <div className='px-5 max-w-[700px]'>
        //   <div className='text-lg font-bold mb-3'>Reschedule Existing Travel</div>
        //   <Form
        //     size='small'
        //     labelCol={{flex: '150px'}}
        //     form={form}
        //     fields={externalFields}
        //     editData={rescheduleData}
        //     className='grid grid-cols-12 gap-y-2'
        //     initValues={inititalValues}
        //   >
        //   </Form>
        // </div>
        // :
        <div className='px-5 max-w-[1000px]'>
          <div className='text-lg font-bold mb-3'>Reschedule Existing Travel</div>
          <Form
            size='small'
            labelCol={{flex: '150px'}}
            form={form}
            fields={fields}
            editData={rescheduleData}
            className='grid grid-cols-12 gap-y-2'
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
          isExternal={rescheduleData?.Direction === 'EXTERNAL'}
        />
      </Modal>
    </div>
  )
}

export default RescheduleExistingTravel