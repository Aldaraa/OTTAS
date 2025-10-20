import { Button, ButtonDrive, Form, Modal, TransportSearch } from 'components'
import { Checkbox, DatePicker, Input, Select } from 'antd'
import React, { useContext, useEffect, useState } from 'react'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { SaveOutlined } from '@ant-design/icons'
import { useParams } from 'react-router-dom'

function RescheduleForm({data, handleShowModal, refreshData, reScheduleForm}) {
  const [ flights, setFlights ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ reTransport, setReTransport ] = useState(null)

  const { state, action } = useContext(AuthContext)
  const {employeeId} = useParams()

  useEffect(() => {
    if(data){
      setReTransport({
        ...data,
        FromLocationId: data?.fromLocationId,
        ToLocationId: data?.toLocationId,
      })
      reScheduleForm?.setFieldsValue({
        startDate: dayjs(data.EventDate),
        shiftId: null,
        Direction: data.Direction,
        startFlight: `${data.Code} ${data.Description} ${data.TransportMode}`,
        DepartmentId: state?.userProfileData?.DepartmentId,
        CostCodeId: state?.userProfileData?.CostCodeId,
      })
    }
  },[data])

  const getFlights = (date) => {
    if(date){
      axios({
        method: 'get',
        url: `tas/ActiveTransport/getdatetransport?eventDate=${date}&Direction=${data.Direction}`
      }).then((res) => {
        let tmp = []
        res.data.map((item) => {
          tmp.push({ value: item.ScheduleId, label: `${item.Description} ${item.Seat}/${item.BookedCount}`})
        })
        if(res.data[0]?.ScheduleId){
          reScheduleForm.setFieldValue('reschdeleFlightId', tmp[0]?.value)
        }else{
          reScheduleForm.setFieldValue('reschdeleFlightId', null)
        }
        setFlights(tmp)
      })
    }
  }

  const handleSelectionButtonLT = () => {
    let tmp = {
      StartDate: reScheduleForm.getFieldValue('endDate') ? reScheduleForm.getFieldValue('endDate') : null,
      EndDate: reScheduleForm.getFieldValue('endDate') ? reScheduleForm.getFieldValue('endDate') : null,
    }
    if(reTransport){
      tmp.DepartLocationId = reTransport.FromLocationId
      tmp.ArriveLocationId = reTransport.ToLocationId
    }else{
      if(reScheduleForm.getFieldValue('Direction') && reScheduleForm.getFieldValue('Direction') === 'IN'){
        tmp = {
          ...tmp,
          DepartLocationId: data?.fromLocationId,
          ArriveLocationId: data?.toLocationId,
        }
      }else{
        tmp = {
          ...tmp,
          DepartLocationId: data?.fromLocationId,
          ArriveLocationId: data?.toLocationId,
        }
      }
    }
    setTransportSelectionInitValues({...tmp})
    setShowLTSelectionModal(true)
  }

  const getOnSiteShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/onsite/${state?.userProfileData.Id}/${date}`,
    }).then((res) => {
      reScheduleForm.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const getOutSiteShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/offsite/${state?.userProfileData.Id}/${date}`,
    }).then((res) => {
      reScheduleForm.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const getOnSiteFutureShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/futurefirststatus/onsite/${state?.userProfileData.Id}/${date}`,
    }).then((res) => {
      reScheduleForm.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const getOutSiteFutureShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/futurefirststatus/offsite/${state?.userProfileData.Id}/${date}`,
    }).then((res) => {
      reScheduleForm.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const handleChangeRescheduleEndDate = (date, string) => { 
    getFlights(string)
    reScheduleForm.setFieldValue('reschdeleFlightId', null)
    reScheduleForm.setFieldValue('ReScheduleDescription', null)
  }

  const handleChangeDrive = (data) => {
    reScheduleForm.setFieldValue('ReScheduleDescription', `${data.Code} ${data.Description}`)
    reScheduleForm.setFieldValue('reschdeleFlightId', data.Id)
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='Existing Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={'startDate'} className='mb-0 w-[130px]'>
            <DatePicker disabled className='w-full'/>
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0 w-[70px]'>
            <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/>
          </Form.Item>
          <Form.Item name={'startFlight'} className='mb-0 flex-1' rules={[{required: true, message: 'Flight is required'}]}>
            <Input disabled/>
          </Form.Item>
          <Form.Item
            name='existingScheduleIdNoShow'
            className='mb-0'
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
      <Form.Item label='Reschedule Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.startDate !== cur.startDate || prev.endDate !== cur.endDate}>
            {({getFieldValue}) => {
              let defaultPickerValue = false
              if(!getFieldValue('endDate')){
                defaultPickerValue = getFieldValue('startDate')
              }
              return(
                <Form.Item name={'endDate'} className='mb-0 w-[130px]'>
                  <DatePicker defaultPickerValue={defaultPickerValue} showWeek className='w-full' onChange={handleChangeRescheduleEndDate}/>
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0 w-[70px]'>
            <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/>
          </Form.Item>
          <Form.Item noStyle name={'reschdeleFlightId'}>
          </Form.Item>
          <Form.Item name={'ReScheduleDescription'} className='flex-1 mb-0'>
            <Input readOnly/>
          </Form.Item>
          <Form.Item 
            noStyle
            shouldUpdate={(prevValues, curValues) => prevValues.endDate !== curValues.endDate}
          >
            {({getFieldValue}) => {
              const isDisabled = !getFieldValue('endDate')
              return(
                <Form.Item className='col-span-1 mb-0'>
                  <Button disabled={isDisabled} className='text-xs py-[5px]' type={'primary'} onClick={handleSelectionButtonLT}>...</Button>
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item 
            noStyle
            shouldUpdate={(prevValues, curValues) => prevValues.endDate !== curValues.endDate}
          >
            {({getFieldValue}) => {
              const date = getFieldValue('endDate')
              const searchValues = {
                eventDate: date,
                direction: getFieldValue('Direction'),
              }
              return(
                <>
                  <Form.Item className='col-span-1 mb-0'>
                    <ButtonDrive
                      onRecieve={handleChangeDrive}
                      searchValues={searchValues}
                      className='py-[5px]'
                      disabled={!date}
                    />
                  </Form.Item>
                  <Form.Item className='col-span-1 mb-0'>
                    <ButtonDrive
                      onRecieve={handleChangeDrive}
                      searchValues={searchValues}
                      className='py-[5px]'
                      disabled={!date}
                      mode='evening'
                    />
                  </Form.Item>
                </>
              )
            }}
          </Form.Item>
          <Form.Item
            name='reScheduleGoShow'
            className='mb-0'
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
          const startDate = getFieldValue('startDate');
          const endDate = getFieldValue('endDate');
          if(endDate && startDate){
            if(dayjs(endDate) - dayjs(startDate) > 0){
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
      label: 'Department',
      name: 'DepartmentId',
      className: 'col-span-12 mb-2',
      type: 'treeSelect',
      rules: [{required: true, message: 'Department is required'}],
      inputprops: {
        treeData: state.referData?.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      }
    },
    {
      label: 'Cost Code',
      name: 'CostCodeId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Cost Code is required'}],
      inputprops: {
        options: state.referData?.costCodes,
      }
    },
  ]

  const getProfileData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
    }).catch((err) => {

    })
  }

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/transport/reschedule',
      data: {
        shiftId: values.ShiftId,
        scheduleId: values.reschdeleFlightId,
        oldTransportId: data.Id,
        departmentId: values.DepartmentId,
        costCodeId: values.CostCodeId,
        existingScheduleIdNoShow: values.existingScheduleIdNoShow,
        reScheduleGoShow: values.reScheduleGoShow,
      }
    }).then((res) => {
      handleShowModal(false)
      reScheduleForm.resetFields()
      refreshData()
      getProfileData()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSelect = (event) => {
    setReTransport(event)
    reScheduleForm.setFieldValue('ReScheduleDescription', `${event.Code} ${event.Description} ${event.TransportMode}`)
    reScheduleForm.setFieldValue('reschdeleFlightId', event.Id)
    setShowLTSelectionModal(false)
  }

  return (
    <div>
      <Form
        form={reScheduleForm}
        fields={fields}
        onFinish={handleSubmit}
        labelCol={{flex: '140px'}} 
        wrapperCol={{flex: 1}}
      >
        <div className='col-span-12 gap-3 flex justify-end mt-3'>
          <Button type='primary' onClick={() => reScheduleForm.submit()} loading={loading} icon={<SaveOutlined/>}>Reschedule</Button>
          <Button onClick={() => handleShowModal(false)} disabled={loading}>Cancel</Button>
        </div>
      </Form>
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
          directionDisabled={true}
        />
      </Modal>
    </div>
  )
}

export default RescheduleForm