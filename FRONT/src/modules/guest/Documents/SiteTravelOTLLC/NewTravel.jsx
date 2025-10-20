import { CloseCircleOutlined, EllipsisOutlined } from '@ant-design/icons';
import { DatePicker, Select, TreeSelect, Form as AntForm, Input } from 'antd';
import axios from 'axios';
import { Button, Form, Modal, SeatTooltip, Tooltip, TransportSearch } from 'components'
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import React, { useCallback, useContext, useEffect, useState } from 'react'

function NewTravel({refreshData, disabled, currentGroup, travelData}) {
  const [ loading, setLoading ] = useState(false)
  const [ isEdit, setIsEdit ] = useState(false)
  const [ defaultValues, setDefaultValues ] = useState(null)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)

  const { state } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()

  const InLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
  }
  const OutLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
  }

  const initRoom = () => {
    let returnRoomData = {
      RoomId: null,
      roomNumber: null
    }

    if(state.userProfileData){
      returnRoomData = {
        roomNumber: state.userProfileData?.RoomNumber ? state.userProfileData?.RoomNumber : 'Virtual Room',
        RoomId: state.userProfileData?.RoomId ? state.userProfileData?.RoomId : state.referData?.noRoomId.Id,
      }
    }
    return returnRoomData
  }

  useEffect(() => {
    let initialvalues;
    if(travelData && state.userProfileData){
      if(dayjs(travelData.inScheduleDate).diff(dayjs(travelData.outScheduleDate)) > 0){
        initialvalues = {
          ...travelData,
          ShiftId: travelData?.shiftId,
          Room: travelData.RoomId ? {
            RoomId: travelData.RoomId,
            roomNumber: travelData.RoomNumber,
          } : initRoom(),
          firstTransport: {
            seats: {
              Seats: travelData?.OUTScheduleSeatsCount,
              Booked: travelData?.OUTScheduleBookedCount,
              Available: travelData?.OUTScheduleAvailableCount,
            },
            flightId: travelData?.outScheduleId,
            flightInfo: '',
            Direction: travelData?.outScheduleDirection,
            Date: travelData?.outScheduleDate ? dayjs(travelData?.outScheduleDate) : null,
            Description: travelData?.OUTScheduleDescription,
          },
          lastTransport: {
            seats: {
              Seats: travelData?.INScheduleSeatsCount,
              Booked: travelData?.INScheduleBookedCount,
              Available: travelData?.INScheduleAvailableCount,
            },
            flightId: travelData?.inScheduleId,
            flightInfo: '',
            Direction: travelData.inScheduleDirection,
            Date: travelData.inScheduleDate ? dayjs(travelData.inScheduleDate) : null,
            Description: travelData?.INScheduleDescription,
          },
        }
      }else{
        initialvalues = {
          ...travelData,
          ShiftId: travelData?.shiftId,
          Room: travelData.RoomId ? {
            RoomId: travelData.RoomId,
            roomNumber: travelData?.RoomNumber,
            // RoomId: state.userProfileData?.RoomId ? state.userProfileData?.RoomId : state.referData?.noRoomId?.Id,
            // roomNumber: state.userProfileData?.RoomNumber ? state.userProfileData?.RoomNumber : 'Virtual room',
          } : initRoom(),
          firstTransport: {
            seats: {
              Seats: travelData?.INScheduleSeatsCount,
              Booked: travelData?.INScheduleBookedCount,
              Available: travelData?.INScheduleAvailableCount,
            },
            flightId: travelData?.inScheduleId,
            Direction: travelData.inScheduleDirection,
            Date: travelData.inScheduleDate ? dayjs(travelData.inScheduleDate) : null,
            Description: travelData?.INScheduleDescription,
          },
          lastTransport: {
            seats: {
              Seats: travelData?.OUTScheduleSeatsCount,
              Booked: travelData?.OUTScheduleBookedCount,
              Available: travelData?.OUTScheduleAvailableCount,
            },
            flightId: travelData?.outScheduleId,
            Direction: travelData?.outScheduleDirection,
            Date: travelData?.outScheduleDate ? dayjs(travelData?.outScheduleDate) : null,
            Description: travelData?.OUTScheduleDescription,
          },
        }
      }

      getDefaultTransportValues(initialvalues)
      form.setFieldsValue(initialvalues)
      setDefaultValues(initialvalues)
    }
  },[travelData, state.userProfileData])

  const getDefaultTransportValues = (data) => {
    let firstdate = dayjs(data.firstTransport.Date).format('YYYY-MM-DD')
    let lastdate = dayjs(data.lastTransport.Date).format('YYYY-MM-DD')
    if(data.inScheduleDate){
      axios({
        method: 'get',
        url: `tas/ActiveTransport/getdatetransport?eventDate=${firstdate}&Direction=${data.firstTransport.Direction}`
      }).then((res) => {
        let tmp = []
        res.data.map((item) => {
          tmp.push({...item, value: item.ScheduleId, label: `${item.Description} ${item.Seat}/${item.BookedCount}`})
        })
        form.setFieldValue(['firstTransport', 'flightId'], data.firstTransport.flightId)
      })
    }
    if(data.outScheduleDate){
      axios({
        method: 'get',
        url: `tas/ActiveTransport/getdatetransport?eventDate=${lastdate}&Direction=${data.lastTransport.Direction}`
      }).then((res) => {
        let tmp = []
        res.data.map((item) => {
          tmp.push({...item, value: item.ScheduleId, label: `${item.Description} ${item.Seat}/${item.BookedCount}`})
        })
        form.setFieldValue(['lastTransport', 'flightId'], data.lastTransport.flightId)
      })
    }
    
  }

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestsitetravel/addtravel',
      data: {
        inScheduleId: values.firstTransport.Direction === 'IN' ? values.firstTransport.flightId : values.lastTransport.flightId,
        outScheduleId: values.firstTransport.Direction === 'OUT' ? values.firstTransport.flightId : values.lastTransport.flightId,
        departmentId: values.DepartmentId,
        shiftId: values.ShiftId,
        employerId: values.EmployerId,
        CheckRoom: currentGroup?.GroupTag === 'accomodation' ? true : false,
        positionId: values.PositionId,
        costcodeId: values.CostCodeId,
        roomId: values.Room?.RoomId,
        id: travelData.Id,
      }
    }).then((res) => {
      refreshData()
      setIsEdit(false)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCancel = () => {
    setIsEdit(false)
    form.setFieldsValue(defaultValues)
  }

  const handleDirectionChange = (e) => {
    if(e === 'IN'){
      form.setFieldValue(['lastTransport', 'Direction'], 'OUT')
    }else{
      form.setFieldValue(['lastTransport', 'Direction'], 'IN')
    }
  }

  const handleSelectionButtonFT = () => {
    let tmp = {
      StartDate: form.getFieldValue(['firstTransport', 'Date']) ? form.getFieldValue(['firstTransport', 'Date']) : null,
      EndDate: form.getFieldValue(['firstTransport', 'Date']) ? form.getFieldValue(['firstTransport', 'Date']) : null,
    }
    if(form.getFieldValue(['firstTransport', 'Direction']) === 'IN'){
      tmp = {
        ...tmp,
        ...InLocations,
      }
    }else if(form.getFieldValue(['firstTransport', 'Direction']) === 'OUT'){
      tmp = {
        ...tmp,
        ...OutLocations,
      }
    }else{
      tmp = {
        ...tmp,
        DepartLocationId: null,
        ArriveLocationId: null,
      }
    }
    setTransportSelectionInitValues({...tmp, transportType: 'first'})
    setShowLTSelectionModal(true)
  }

  const handleSelectionButtonLT = () => {
    const lastDirection = form.getFieldValue(['lastTransport', 'Direction'])
    const firstDirection = form.getFieldValue(['firstTransport', 'Direction'])
    let tmp = {
      StartDate: form.getFieldValue(['lastTransport', 'Date']) ? form.getFieldValue(['lastTransport', 'Date']) : null,
      EndDate: form.getFieldValue(['lastTransport', 'Date']) ? form.getFieldValue(['lastTransport', 'Date']) : null,
    }
    if(lastDirection === 'IN' || firstDirection === 'OUT'){
      tmp = {
        ...tmp,
        ...InLocations,
      }
    }else if(lastDirection === 'OUT' || firstDirection === 'IN'){
      tmp = {
        ...tmp,
        ...OutLocations,
      }
    }else{
      tmp = {
        ...tmp,
        DepartLocationId: null,
        ArriveLocationId: null,
      }
    }
    setTransportSelectionInitValues({...tmp, transportType: 'last'})
    setShowLTSelectionModal(true)
  }

  const handleSelect = (event) => {
    if(transportSelectionInitValues.transportType === 'first'){
      form.setFieldValue(['firstTransport', 'seats'], {Seats: event.Seats, Booked: event.Confirmed, Available: event.Seats-event.Confirmed < 0 ? 0 : event.Seats-event.Confirmed})
      form.setFieldValue(['firstTransport', 'Direction'],event.Direction)
      form.setFieldValue(['firstTransport', 'Description'], `${event.Code} ${event.Description} ${event.TransportMode}`)
      form.setFieldValue(['firstTransport', 'flightId'], event.Id)
      form.setFieldValue(['firstTransport', 'Date'], dayjs(event.EventDate))
      setShowLTSelectionModal(false)
    }else{
      form.setFieldValue(['lastTransport', 'seats'], {Seats: event.Seats, Booked: event.Confirmed, Available: event.Seats-event.Confirmed < 0 ? 0 : event.Seats-event.Confirmed})
      form.setFieldValue(['lastTransport', 'Direction'], event.Direction)
      form.setFieldValue(['lastTransport', 'Description'], `${event.Code} ${event.Description} ${event.TransportMode}`)
      form.setFieldValue(['lastTransport', 'flightId'], event.Id)
      form.setFieldValue(['lastTransport', 'Date'], dayjs(event.EventDate))
      setShowLTSelectionModal(false)
    }
  }

  const handleChangeFirstTransportDate = useCallback((e) => {
    form.setFieldValue('RoomId', '')
    form.setFieldValue(['firstTransport', 'Description'], null);
    form.setFieldValue(['firstTransport', 'flightId'], null);
  },[])

  const handleChangeLastTransportDate = useCallback(() => {
    form.setFieldValue(['lastTransport', 'Description'], null);
    form.setFieldValue(['lastTransport', 'flightId'], null);
  },[])

  return (
    <div className='w-full max-w-[1200px] col-span-12'>
      <Form
        className='grid grid-cols-12 col-span-12 gap-x-5' 
        form={form}
        onFinish={handleSubmit}
        disabled={!isEdit}
        size='small'
        labelCol={{ xs: { span: 24 }, sm: { span: 5 } }}
      >
        <Form.Item label='First Transport' className='col-span-12 mb-2' shouldUpdate={(prev, cur) => prev.firstTransport?.Date !== cur.firstTransport?.Date}>
          {({getFieldValue, getFieldsValue}) => {
            const date = getFieldValue(['firstTransport', 'Date'])
            return(
              <div className='flex gap-2'>
                <Form.Item name={['firstTransport', 'Date']} noStyle={!date} className='mb-0 w-[115px]'>
                  { date ? <DatePicker showWeek onChange={handleChangeFirstTransportDate} className='w-full'/> : null}
                </Form.Item>
                <Form.Item name={['firstTransport', 'Direction']} noStyle={!date} className='mb-0 w-[65px]'>
                  { date ? <Select options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full' onChange={handleDirectionChange}/> : null}
                </Form.Item>
                <Form.Item noStyle name={['firstTransport', 'flightId']}>
                </Form.Item>
                <Form.Item noStyle name={['firstTransport', 'seats']}>
                </Form.Item>
                <Form.Item noStyle shouldUpdate={(prev, cur) => prev.firstTransport?.flightId !== cur.firstTransport?.flightId}>
                  {({getFieldValue}) => {
                    const scheduleId = getFieldValue(['firstTransport', 'flightId'])
                    return(
                      <Form.Item noStyle={!date} name={['firstTransport', 'Description']} className='flex-1 mb-0'>
                        {date ? <Input readOnly addonAfter={<SeatTooltip id={scheduleId}/>}/> : null} 
                      </Form.Item>
                    )
                  }}
                </Form.Item>
                {
                  !date ?
                  <Tooltip title='Removed Schedule'>
                    <Form.Item name={'inScheduleIdDescr'} className='mb-0 w-full'>
                      <Input disabled status='error' prefix={<CloseCircleOutlined/>} className='w-full'/>
                    </Form.Item>
                  </Tooltip>
                  : null
                }
                <Form.Item className='col-span-1 mb-0'>
                  <Button
                    className='text-xs py-[4px] px-2'
                    type={'primary'}
                    onClick={handleSelectionButtonFT}
                    disabled={!isEdit}
                    icon={<EllipsisOutlined />}
                  />
                </Form.Item>
              </div>
            )
          }}
        </Form.Item>
        <Form.Item label='Last Transport' className='col-span-12 mb-2' shouldUpdate={(prev, cur) => prev.lastTransport?.Date !== cur.lastTransport?.Date || prev?.firstTransport?.Direction !== cur?.firstTransport?.Direction}>
          {({getFieldValue}) => {
            const date = getFieldValue(['lastTransport', 'Date'])
            return(
              <div className='flex gap-2'>
                <Form.Item noStyle shouldUpdate={(prev, cur) => prev.firstTransport?.Date !== cur.firstTransport?.Date || prev.lastTransport?.Date !== cur.lastTransport?.Date}>
                  {({getFieldValue}) => {
                    let defaultPickerValue = false
                    if(!getFieldValue(['lastTransport', 'Date'])){
                      defaultPickerValue = getFieldValue(['firstTransport', 'Date'])
                    }
                    return(
                      <Form.Item name={['lastTransport', 'Date']} noStyle={!date} className='mb-0 w-[115px]'>
                        {
                          date ?
                          <DatePicker
                            defaultPickerValue={defaultPickerValue}
                            showWeek
                            onChange={handleChangeLastTransportDate}
                            className='w-full'
                          />
                          : null
                        }
                      </Form.Item>
                    )
                  }}
                </Form.Item>
                <Form.Item className='mb-0 w-[65px]' noStyle shouldUpdate={(prev, cur) => prev?.firstTransport?.Direction !== cur?.firstTransport?.Direction}>
                  {({getFieldValue, setFieldValue}) => {
                    const firstDirection = getFieldValue(['firstTransport', 'Direction'])
                    if(firstDirection === 'IN'){
                      setFieldValue(['lastTransport', 'Direction'], 'OUT')
                    }
                    if(firstDirection === 'OUT'){
                      setFieldValue(['lastTransport', 'Direction'], 'IN')
                    }
                  }}
                </Form.Item>
                <Form.Item name={['lastTransport', 'Direction']} noStyle={!date} className='mb-0 w-[65px]'>
                  { date ? <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full' /> : null}
                </Form.Item>
                <Form.Item noStyle name={['lastTransport', 'flightId']}>
                </Form.Item>
                <Form.Item noStyle name={['lastTransport', 'seats']}>
                </Form.Item>
                <Form.Item noStyle shouldUpdate={(prev, cur) => prev.lastTransport?.flightId !== cur.lastTransport?.flightId}>
                  {({getFieldValue}) => {
                    const scheduleId = getFieldValue(['lastTransport', 'flightId'])
                    return(
                      <Form.Item name={['lastTransport', 'Description']} noStyle={!date} className='flex-1 mb-0'>
                        { date ? <Input readOnly addonAfter={<SeatTooltip id={scheduleId}/>}/> : null }
                      </Form.Item>
                    )
                  }}
                </Form.Item>
                {
                  !date ?
                  <Tooltip title='Removed Schedule'>
                    <Form.Item name={'outScheduleIdDescr'} className='mb-0 w-full'>
                      <Input disabled status='error' prefix={<CloseCircleOutlined/>} className='w-full'/>
                    </Form.Item>
                  </Tooltip>
                  : null
                }
                <Form.Item className='col-span-1 mb-0'>
                  <Button 
                    className='text-xs py-1 px-2'
                    type={'primary'}
                    onClick={handleSelectionButtonLT}
                    disabled={!isEdit}
                    icon={<EllipsisOutlined />}
                  />
                </Form.Item>
              </div>
            )
          }}
        </Form.Item>
        <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.firstTransport?.Direction !== curValues.firstTransport?.Direction}>
          {({getFieldValue, setFieldValue}) => {
            return(
              getFieldValue(['firstTransport', 'Direction']) === 'OUT' ?
              <Form.Item label='Shift' name={'ShiftId'} className='col-span-12 mb-2' rules={[{required: true, message: 'Shift is required'}]}>
                <Select
                  options={state.referData?.roomStatuses?.filter((item) => item.OnSite === 0)}
                  className='w-full'
                  filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                  showSearch
                  allowClear
                />
              </Form.Item>
              :
              <Form.Item label='Shift' name={'ShiftId'} className='col-span-12 mb-2' rules={[{required: true, message: 'Shift is required'}]}>
                <Select
                  options={state.referData?.roomStatuses?.filter((item) => item.OnSite === 1)}
                  className='w-full'
                  filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                  showSearch
                  allowClear
                />
              </Form.Item>
            )
          }}
        </Form.Item>
        <Form.Item
          key={`form-item-DepartmentId`}
          label='Department'
          name='DepartmentId'
          className='col-span-12 mb-2'
          rules={[{required: true, message: 'Department is required'}]}
        >
          <TreeSelect 
            treeData={state.referData.departments} 
            fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
            filterTreeNode={(input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase())}
            allowClear
            showSearch
          />
        </Form.Item>
        <Form.Item
          key={`form-item-PositionId`}
          label='Position'
          name={'PositionId'}
          className='col-span-12 mb-2'
          rules={[{required: true, message: 'Position is required'}]}
        >
          <Select 
            allowClear
            showSearch
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            options={state.referData?.positions}
          />
        </Form.Item>
        <Form.Item
          key={`form-item-CostCodeId`}
          label='Cost Code'
          name={'CostCodeId'}
          className='col-span-12 mb-2'
          rules={[{required: true, message: 'Cost Code is required'}]}
        >
          <Select 
            allowClear
            showSearch
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            options={state.referData?.costCodes}
          />
        </Form.Item>
        <Form.Item
          key={`form-item-EmployerId`}
          label='Employer'
          name={'EmployerId'}
          className='col-span-12 mb-2'
          rules={[{required: true, message: 'Employer is required'}]}
        >
          <Select 
            allowClear
            showSearch
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            options={state.referData?.employers}
          />
        </Form.Item>
        {
          !disabled &&  currentGroup?.GroupTag !== 'Completed' &&
          <div className='col-span-12 flex justify-end gap-5'>
            {
              isEdit ? 
              <>
                <Button htmlType='button' onClick={() => form.submit()} type='success' loading={loading}>Save</Button>
                <Button htmlType='button' onClick={handleCancel}>Cancel</Button>
              </>
              :
              <>
                <Button htmlType='button' onClick={() => setIsEdit(true)} type='primary'>Edit</Button>
              </>
            }
          </div>
        }
      </Form>
      <Modal 
        title='Select Transport'
        open={showLTSelection}
        width={900}
        onCancel={() => setShowLTSelectionModal(false)}
        destroyOnClose={true}
      >
        <TransportSearch
          initialSearchValues={transportSelectionInitValues}
          handleSelect={handleSelect}
        />
      </Modal>
    </div>
  )
}

export default NewTravel