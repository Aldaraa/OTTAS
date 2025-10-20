import { Button, Form, Modal, TransportSearch } from 'components'
import { DatePicker, Input, Select } from 'antd'
import React, { useContext, useEffect, useState } from 'react'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { SaveOutlined } from '@ant-design/icons'
import { useParams } from 'react-router-dom'

function RescheduleForm({data, handleShowModal, refreshData, reScheduleForm}) {
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

  const handleChangeRescheduleEndDate = () => { 
    reScheduleForm.setFieldValue('reschdeleFlightId', null)
    reScheduleForm.setFieldValue('ReScheduleDescription', null)
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='Old Transport' className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item name={'startDate'} className='mb-0 col-span-3'>
            <DatePicker
              disabled
              className='w-full'
            />
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0 col-span-2'>
            <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/>
          </Form.Item>
          <Form.Item name={'startFlight'} className='mb-0 col-span-7' rules={[{required: true, message: 'Flight is required'}]}>
            <Input disabled/>
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='New Transport' className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
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
                    onChange={handleChangeRescheduleEndDate}
                  />
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item name={'Direction'} className='mb-0 col-span-2'>
            <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/>
          </Form.Item>
          <Form.Item noStyle name={'reschdeleFlightId'} rules={[{required: true, message: 'New Transport is required'}]}>
          </Form.Item>
          <Form.Item name={'ReScheduleDescription'} className='col-span-6 mb-0'>
            <Input readOnly/>
          </Form.Item>
          <Form.Item 
            noStyle
            shouldUpdate={(prevValues, curValues) => prevValues.endDate !== curValues.endDate}
          >
            {({getFieldValue, setFieldValue}) => {
              return(
                <Form.Item className='col-span-1 mb-0'>
                  <Button disabled={!getFieldValue('endDate')} className='text-xs py-[5px]' type={'primary'} onClick={handleSelectionButtonLT}>...</Button>
                </Form.Item>
              )
            }}
          </Form.Item>
        </div>
      </Form.Item>
      </>
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
      url: 'tas/safemode/transport',
      data: {
        scheduleId: values.reschdeleFlightId,
        TransportId: data.Id,
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