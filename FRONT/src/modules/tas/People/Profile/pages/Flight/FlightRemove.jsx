import React, { useContext, useEffect, useState } from 'react'
import { SaveOutlined } from '@ant-design/icons'
import { Button, Form } from 'components'
import { Form as AntForm, Checkbox, DatePicker, Input, Select } from 'antd'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import axios from 'axios'
import { useParams } from 'react-router-dom'

function FlightRemove({data, editData, refreshData, handleCancel, flights}) {
  const [ loading, setLoading ] = useState(false)

  const [form] = AntForm.useForm()
  const { state, action } = useContext(AuthContext)
  const { employeeId } = useParams()

  useEffect(() => {
    if(editData){
      form.setFieldValue('CostCodeId', data?.CostCodeId)
      let lastTransport = flights[editData.rowIndex+1]
      form.setFieldValue(['firstTransport', 'Date'],dayjs(editData?.EventDate))
      form.setFieldValue(['firstTransport', 'Direction'],editData?.Direction)
      form.setFieldValue(['firstTransport', 'Description'],`${editData.Code} ${editData.Description} ${editData.TransportMode}`)
      form.setFieldValue(['firstTransport', 'ScheduleId'],editData.Id)
      if(lastTransport){
        form.setFieldValue(['lastTransport', 'Date'],dayjs(lastTransport.EventDate).format('YYYY-MM-DD'))
        form.setFieldValue(['lastTransport', 'Direction'],lastTransport.Direction)
        form.setFieldValue(['lastTransport', 'Description'],`${lastTransport.Code} ${lastTransport.Description} ${editData.TransportMode}`)
        form.setFieldValue(['lastTransport', 'ScheduleId'],lastTransport.Id)
      }
    }
  },[editData])

  const getOnSiteShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/onsite/${data.Id}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const getOutSiteShift = (date) => {
    axios({
      method: 'get',
      url: `tas/employeestatus/lasterstatus/offsite/${data.Id}/${date}`,
    }).then((res) => {
      form.setFieldValue('ShiftId', res.data.ShiftId)
    }).catch((err) => {

    })
  }

  const handleChangeLastTransport = (schedule) => {
    form.setFieldValue(['lastTransport', 'Description'], `${schedule.Code} ${schedule.Description}`)
    form.setFieldValue(['lastTransport', 'ScheduleId'], schedule.Id)
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='First Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={['firstTransport', 'Date']} className='mb-0 w-[130px]'>
            <DatePicker disabled className='w-full'/>
          </Form.Item>
          <Form.Item name={['firstTransport', 'Direction']} className='mb-0 w-[70px]'>
            <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/>
          </Form.Item>
          <Form.Item className='mb-0 flex-1' name={['firstTransport', 'Description']} rules={[{required: true, message: 'Flight is required'}]}>
            <Input disabled className='w-full'/>
          </Form.Item>
          <Form.Item name={['firstTransport', 'ScheduleId']} noStyle rules={[{required: true, message: 'Flight is required'}]}>
          </Form.Item>
          <Form.Item
            name={['firstTransport', 'NoShow']}
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
      <Form.Item label='Last Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={['lastTransport', 'Date']} className='mb-0 w-[130px]'>
            <Select
              options={flights.filter((item) => item.Direction !== editData?.Direction && dayjs(item.EventDate).diff(dayjs(editData.EventDate)) > 0)}
              fieldNames={{value: 'EventDate', label: 'label'}}
              onChange={(e, option) => handleChangeLastTransport(option)}
            />
          </Form.Item>
          <Form.Item name={['lastTransport', 'Direction']} className='mb-0 w-[70px]'>
            <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/>
          </Form.Item>
          <Form.Item noStyle name={['lastTransport', 'ScheduleId']} rules={[{required: true, message: 'Last Transport is required'}]}>
          </Form.Item>
          <Form.Item name={['lastTransport', 'Description']} className='mb-0 flex-1'>
            <Input disabled/>
          </Form.Item>
          <Form.Item
            name={['lastTransport', 'NoShow']}
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
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.firstTransport?.Date !== cur.firstTransport?.Date}>
        {({getFieldValue}) => {
          const firstDate = getFieldValue(['firstTransport', 'Date'])
          let shiftType = null
          if(getFieldValue(['firstTransport', 'Direction']) === 'IN'){
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
              rules={[{required: true, message: 'Shift Status is required'}]}
            >
              <Select 
                options={typeof shiftType === 'number' ? state.referData?.roomStatuses.filter((row) => row.OnSite === shiftType) : []}
                showSearch
                allowClear
              />
            </Form.Item>
          )
        }}
      </Form.Item>
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
    {
      label: 'No Show',
      name: 'noShow',
      className: 'col-span-12 mb-2',
      type: 'check',
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

  const handleSubmitRemove = (values) => {
    setLoading(true)
    axios({
      method: 'delete',
      url: 'tas/transport/removetransport',
      data: {
        startScheduleId: values.firstTransport.ScheduleId,
        endScheduleId: values.lastTransport.ScheduleId,
        firstScheduleNoShow: values.firstTransport.NoShow,
        lastScheduleNoShow: values.lastTransport.NoShow,
        shiftId: values.ShiftId,
        noShow: values.noShow,
      }
    }).then((res) => {
      handleCancel()
      refreshData()
      getProfileData()
    }).catch((err) => {

    }).then(() => setLoading(false))

  }

  return (
    <>
      <Form
        form={form} 
        fields={fields}
        onFinish={handleSubmitRemove}
        labelCol={{ flex: '150px' }}
        wrapperCol={{ flex: 1 }}
      />
      <div className='gap-3 flex justify-end mt-3'>
        <Button type='primary' onClick={() => form.submit()} loading={loading} icon={<SaveOutlined/>}>Remove</Button>
        <Button onClick={handleCancel}>Cancel</Button>
      </div>
    </>
  )
}

export default FlightRemove