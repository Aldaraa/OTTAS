import { Button, Form, Modal, RemovedTransportField, SeatTooltip, Tooltip, TransportSearch } from 'components'
import { Form as AntForm, DatePicker, Input, Select } from 'antd'
import React, { useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { CloseCircleOutlined } from '@ant-design/icons'

const RescheduleExistingTravel = ({data, refreshData, disabled, currentGroup}) => {
  const [ loading, setLoading ] = useState(false)
  const [ isEdit, setIsEdit ] = useState(false)
  const [ inited, setInited ] = useState(false)
  const [ showTransportSelection, setShowTransportSelection ] = useState(false)
  const [ selectionDefaultValue, setSelectionDefaultValue ] = useState(null)

  const { state } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()

  const initialValues = useMemo(() => {
    if(data){
      return {
        startDate: data.ExistingScheduleDate ? dayjs(data.ExistingScheduleDate) : null,
        endDate: data.ReScheduleDate ? dayjs(data.ReScheduleDate) : null,
        ShiftId: data.shiftId,
        Direction: data.ExistingScheduleDirection || data.ReScheduleDirection,
        ExistingScheduleDescription: data.ExistingScheduleDescription,
        ExistingScheduleDirection: data.ExistingScheduleDirection,
        ExistingScheduleIdDescr: data.ExistingScheduleIdDescr,
        existingScheduleId: data.ExistingScheduleId,
        ReScheduleIdDescr: data.ReScheduleIdDescr,
        ReScheduleId: data.ReScheduleId,
        ReScheduleDescription: data.ReScheduleDescription,
        ReScheduleDirection: data.ReScheduleDirection,
        FromLocationId: data.ReScheduleFromLocationid,
        ToLocationId: data.ReScheduleToLocationid,
        RoomId: data?.RoomId,
        RoomNumber: data.RoomnNumber,
      }
    }
  },[data])


  useEffect(() => {
    if(initialValues){
      form.setFieldsValue(initialValues)
    }
  },[initialValues])

  const handleSelectTR = () => {
    const fromLocationId = form.getFieldValue('FromLocationId')
    const toLocationId = form.getFieldValue('ToLocationId')
    let tmp = {
      StartDate: form.getFieldValue('endDate') ? form.getFieldValue('endDate') : null,
      EndDate: form.getFieldValue('endDate') ? form.getFieldValue('endDate') : null,
    }
    tmp.DepartLocationId = fromLocationId
    tmp.ArriveLocationId = toLocationId
    setSelectionDefaultValue({...tmp})
    setShowTransportSelection(true)
  }

  const getOnSiteShift = useCallback((date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/onsite/${state.userProfileData?.Id}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  },[state, form])

  const getOutSiteShift = useCallback((date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/offsite/${state.userProfileData?.Id}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  },[state, form])

  const getOnSiteFutureShift = useCallback((date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/futurefirststatus/onsite/${state.userProfileData?.Id}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  },[state, form])

  const getOutSiteFutureShift = useCallback((date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/futurefirststatus/offsite/${state.userProfileData?.Id}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  },[state, form])

  const fields = [
    {
      type: 'component',
      component: <Form.Item label='Existing Transport' shouldUpdate={(prev, cur) => prev?.startDate !== cur?.startDate} className='col-span-12 mb-2'>
        {
          ({getFieldValue}) => {
            const startDate = getFieldValue('startDate')
            return(
              <div className='flex gap-2'>
                <Form.Item name={'startDate'} noStyle={!startDate} className='mb-0 w-[115px]'>
                  { 
                    startDate ? 
                    <DatePicker disabled className='w-full'/>
                    : null
                  }
                </Form.Item>
                <Form.Item name={'ExistingScheduleDirection'} noStyle={!startDate} className='mb-0 w-[60px]'>
                  {
                    startDate ? 
                    <Input disabled className='w-full py-[1px]'/>
                    : null
                  }
                </Form.Item>
                <Form.Item noStyle name={'existingScheduleId'}>
                </Form.Item>
                <Form.Item noStyle shouldUpdate={(prev, cur) => prev.existingScheduleId !== cur.existingScheduleId}>
                  {({getFieldValue}) => {
                    const existingScheduleId = getFieldValue('existingScheduleId')
                    return(
                      <Form.Item name={'ExistingScheduleDescription'} noStyle={!startDate} className='flex-1 mb-0' rules={[{required: true, message: 'Flight is required'}]}>
                        {startDate ? 
                          <Input disabled className='py-[1px]' addonAfter={<SeatTooltip id={existingScheduleId}/>}/>
                          : null
                        }
                      </Form.Item>
                    )
                  }}
                </Form.Item>
                { startDate ? null :
                  <RemovedTransportField description={data?.ExistingScheduleIdDescr} form={form}/>
                }
              </div>
            )
          }
        }
      </Form.Item>
    },
    {
      type: 'component',
      component:<Form.Item label={<span><span className='text-red-400'>*</span>Reschedule Transport</span>} className='col-span-12 mb-2' shouldUpdate={(prev, cur) => prev.ReScheduleId !== cur.ReScheduleId}>
        {({getFieldValue}) => {
          const isDeleted = getFieldValue('ReScheduleId') === 0
          return(
            <div className='flex gap-2'>
              <Form.Item noStyle shouldUpdate={(prev, cur) => prev.startDate !== cur.startDate || prev.endDate !== cur.endDate}>
                {({getFieldValue}) => {
                  let defaultPickerValue = false
                  if(!getFieldValue('endDate')){
                    defaultPickerValue = getFieldValue('startDate') ? getFieldValue('startDate') : false
                  }
                  return(
                    <Form.Item name={'endDate'} className='mb-0 col-span-3' noStyle={isDeleted}>
                      {!isDeleted ?
                        <DatePicker
                          defaultPickerValue={defaultPickerValue}
                          showWeek
                          className='w-full'
                          onChange={() => {
                            form.setFieldValue('ReScheduleDescription', null);
                            form.setFieldValue('ReScheduleId', null)
                          }}
                        />
                        : null
                      }
                    </Form.Item>
                  )
                }}
              </Form.Item>
              <Form.Item name={'ReScheduleDirection'} className='mb-0' noStyle={isDeleted}>
                { !isDeleted ? 
                  <Input disabled className='w-[50px] py-[2px]'/>
                  : null
                }
              </Form.Item>
              <Form.Item noStyle name={'ReScheduleId'}>
              </Form.Item>
              <Form.Item noStyle name={'FormLocationId'}>
              </Form.Item>
              <Form.Item noStyle name={'ToLocationId'}>
              </Form.Item>
              <Form.Item noStyle shouldUpdate={(prev, cur) => prev.ReScheduleId !== cur.ReScheduleId}>
                {({getFieldValue}) => {
                  const reScheduleId = getFieldValue('ReScheduleId')
                  return(
                    <>
                      <Form.Item name={'ReScheduleDescription'} rules={[{required: true, message: 'ReSchedule Transport is required'}]} className='flex-1 mb-0' noStyle={isDeleted}>
                        {
                          !isDeleted ?
                          <Input readOnly className='py-[2px]' addonAfter={<SeatTooltip id={reScheduleId}/>}/>
                          : null
                        }
                      </Form.Item>
                    </>
                  )
                }}
              </Form.Item>
              {
                isDeleted ?
                <Tooltip title='Removed schedule'>
                  <Form.Item className='w-full mb-0' name='ReScheduleIdDescr'>
                    <Input disabled status='error' prefix={<CloseCircleOutlined/>} className='w-full' />
                  </Form.Item>
                </Tooltip>
                : null
              }
              <Form.Item noStyle shouldUpdate={(pre, cur) => pre.endDate !== cur.endDate}>
                {({getFieldValue}) => {
                  const endDate = getFieldValue('endDate');
                  return(
                    <Form.Item className='col-span-1 mb-0'>
                      <Button 
                        disabled={!endDate && !isDeleted}
                        className='text-xs py-[3px]'
                        type={'primary'}
                        onClick={handleSelectTR}
                      >...</Button>
                    </Form.Item>
                  )
                }}
              </Form.Item>
            </div>
          )
        }}
    </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.endDate !== curValues.endDate}>
        {({getFieldValue, setFieldValue}) => {
          let shiftType = null
          const endDate = getFieldValue('endDate')
          if(endDate && data.ExistingScheduleDate){
            if(dayjs(endDate).diff(dayjs(data.ExistingScheduleDate)) > 0){
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
  ]


  const formFields = useMemo(() => {
    let tmp = []
    if(currentGroup?.GroupTag){
      tmp = fields
    }
    return tmp
  },[currentGroup, fields])

  useEffect(() => {
    if(initialValues && formFields.length > 0){
      setInited(true)
    }
  },[initialValues, formFields])

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestsitetravel/reschedule',
      data: {
        id: data?.Id,
        shiftId: values.ShiftId,
        roomId: values.RoomId,
        reScheduleId: values.ReScheduleId,
        existingScheduleId: values.existingScheduleId,
        Reason: data.Reason,
      }
    }).then((res) => {
      setIsEdit(false)
      refreshData()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCancel = () => {
    setIsEdit(false)
    form.setFieldsValue(initialValues)
  }

  const handleSelect = (rowData) => {
    form.setFieldValue('endDate', dayjs(rowData.EventDate));
    form.setFieldValue('FormLocationId', rowData.FromLocationId);
    form.setFieldValue('ToLocationId', rowData.ToLocationId);
    form.setFieldValue('ReScheduleDescription', `${rowData.Code} ${rowData.Description}`);
    form.setFieldValue('ReScheduleDirection', rowData.Direction);
    form.setFieldValue('ReScheduleId', rowData.Id);
    setShowTransportSelection(false)
  }

  return (
    <div className='w-full'>
      {
        inited ? 
        <Form
          form={form}
          fields={formFields}
          onFinish={handleSubmit}
          disabled={!isEdit}
          className='grid grid-cols-12 gap-x-5 max-w-[700px]'
          size='small'
          labelAlign='left'
          labelCol={{ flex: '150px' }}
        >
          {
            !disabled &&  currentGroup?.GroupTag !== 'Completed' && initialValues?.ReScheduleId !== 0 && initialValues?.existingScheduleId !== 0 &&
            <div className='col-span-12 flex justify-end gap-5'>
              {
                isEdit ?
                <>
                  <Button 
                    htmlType='button'
                    onClick={() => form.submit()}
                    type='success'
                    loading={loading}
                    className='text-xs'
                  >
                    Save
                  </Button>
                  <Button
                    htmlType='button'
                    onClick={handleCancel}
                    disabled={loading}
                    className='text-xs'
                  >
                    Cancel
                  </Button>
                </>
                :
                <>
                  <Button 
                    htmlType='button' 
                    onClick={() => setIsEdit(true)} 
                    type='primary' 
                    className='text-xs'
                  >
                    Edit
                  </Button>
                </>
              }
            </div>
          }
        </Form>
        : null
      }
      <Modal
        title='Select Transport'
        open={showTransportSelection}
        width={900}
        onCancel={() => setShowTransportSelection(false)}
        destroyOnClose={false}
      >
        <TransportSearch
          initialSearchValues={selectionDefaultValue}
          handleSelect={handleSelect}
          directionDisabled={false}
          // isExternal={rescheduleData?.Direction === 'EXTERNAL'}
        />
      </Modal>
    </div>
  )
}

export default RescheduleExistingTravel