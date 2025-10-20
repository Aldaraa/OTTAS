import { DatePicker, Input, Select } from 'antd';
import axios from 'axios';
import { Button, Form, Modal, TransportSearch } from 'components';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import React, { useContext, useEffect, useState } from 'react'
import formDefaultLayouts from 'utils/formDefaultLayouts';

function ExternalReSchedule({onCancel, refreshData, data}) {
  const [ loading, setLoading ] = useState(false)
  const [ formFirstDate, setFormFirstDate ] = useState(null)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ formLastDate, setFormLastDate ] = useState(null)

  const { state, action } = useContext(AuthContext)
  const [ form ] = Form.useForm()


  useEffect(() => {
    form.setFieldsValue({
      firstTransport: {
        Date: dayjs(data.EventDate),
        flight: data.ScheduleId,
        Description: `${data.Code} ${data.Description}`
      }
    })
  },[data])


  const handleSelectionButtonFT = () => {
    let tmp = {
      StartDate: form.getFieldValue(['firstTransport', 'Date']) ? form.getFieldValue(['firstTransport', 'Date']) : null,
      EndDate: null,
    }
    setTransportSelectionInitValues({...tmp, transportType: 'first'})
    setShowLTSelectionModal(true)
  }

  const onChangeFirstDate = (e) => {
    action.setFlightCalendarDate(dayjs(e).format('YYYY-MM-DD'))
    form.setFieldValue(['firstTransport', 'flightId'], null)
    form.setFieldValue(['firstTransport', 'Description'], null)
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='Current Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item className='mb-0 col-span-3'>
            <DatePicker disabled showWeek value={dayjs(data.EventDate)} className='w-full'/>
          </Form.Item>
          <Form.Item className='mb-0 w-[70px]'>
            <Input disabled value={'OUT'} className='w-full'/>
          </Form.Item>
          <Form.Item className='flex-1 mb-0'>
            <Input disabled readOnly value={`${data.Code} ${data.Description}`}/>
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='ReSchedule Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={['firstTransport', 'Date']} className='mb-0 col-span-3' rules={[{required: true, message: 'First Transport Date is required'}]}>
            <DatePicker showWeek onChange={onChangeFirstDate} className='w-full'/>
          </Form.Item>
          <Form.Item className='mb-0 w-[70px]' rules={[{required: true, message: 'Direction is required'}]}>
            {/* <Select  options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full' onChange={handleDirectionChange}/> */}
            <Input disabled value={'OUT'} className='w-full'/>
          </Form.Item>
          <Form.Item noStyle name={['firstTransport', 'flightId']}>
          </Form.Item>
          <Form.Item name={['firstTransport', 'Description']} className='flex-1 mb-0'>
            <Input readOnly/>
          </Form.Item>
          <Form.Item
            className='col-span-1 mb-0'
            shouldUpdate={(pre, cur) => pre.firstTransport?.Date !== cur.firstTransport?.Date}
          >
            {
              ({getFieldValue}) => (
                <Button
                  className='text-xs py-[5px]'
                  type={'primary'}
                  onClick={handleSelectionButtonFT}
                  disabled={!getFieldValue(['firstTransport', 'Date'])}
                >
                  ...
                </Button>
              )
            }
          </Form.Item>
        </div>
      </Form.Item>
      </>
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
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.DepartmentId !== cur.DepartmentId}>
        {({getFieldValue, setFieldValue}) => {
          let selectedDepartment = state.referData.departments?.find((department) => department.Id === getFieldValue('DepartmentId'))
          if(selectedDepartment && selectedDepartment.CostCodeId){
            setFieldValue('CostCodeId', selectedDepartment.CostCodeId)
          }
          return (
            <Form.Item 
              name='CostCodeId' 
              label='CostCodeId'
              rules={[{required: true, message: 'Cost Code is required'}]}
              className='col-span-12 mb-2'
            >
              <Select
                options={state.referData.costCodes}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
  ]

  const handleSubmitAdd = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/transport/rescheduleexternaltransport',
      data: {
        scheduleId: values.firstTransport.flightId,
        oldTransportId: data.Id,
        departmentId: values.DepartmentId,
        costCodeId: values.CostCodeId,
      }
    }).then((res) => {
      onCancel()
      refreshData()
      form.resetFields()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSelect = (event) => {
    if(transportSelectionInitValues.transportType === 'first'){
      form.setFieldValue(['firstTransport', 'Date'], dayjs(event.EventDate))
      form.setFieldValue(['firstTransport', 'Description'], `${event.Code} ${event.Description}`)
      form.setFieldValue(['firstTransport', 'flightId'], event.Id)
      setShowLTSelectionModal(false)
    }else{
      form.setFieldValue(['lastTransport', 'Date'], dayjs(event.EventDate))
      form.setFieldValue(['lastTransport', 'Description'], `${event.Code} ${event.Description}`)
      form.setFieldValue(['lastTransport', 'flightId'], event.Id)
      setShowLTSelectionModal(false)
    }
  }

  return (
    <div>
      <Form
        form={form} 
        fields={fields} 
        editData={{...state.userProfileData}}
        onFinish={handleSubmitAdd}
        labelCol={{flex: '140px'}} 
        wrapperCol={{flex: 1}}
      >
        <div className='col-span-12 gap-3 flex justify-end mt-3'>
          <Button type='primary' onClick={() => form.submit()} loading={loading}>Create</Button>
          <Button onClick={onCancel}>Cancel</Button>
        </div>
      </Form>
      <Modal
        title='Select Transport'
        open={showLTSelection}
        zIndex={1100}
        width={900}
        onCancel={() => setShowLTSelectionModal(false)}
        destroyOnClose={false}
      >
        <TransportSearch
          initialSearchValues={transportSelectionInitValues}
          handleSelect={handleSelect}
          isExternal={true}
        />
      </Modal>
    </div>
  )
}

export default ExternalReSchedule