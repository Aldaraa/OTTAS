import { DatePicker, Input, Select } from 'antd';
import axios from 'axios';
import { Button, Form, Modal, RoomSelection, TransportSearch } from 'components';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import React, { useContext, useState } from 'react'
import { useLocation } from 'react-router-dom';

function AddTransport({onCancel, refresh, employeeId}) {
  const { state, action } = useContext(AuthContext)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ firstTransport, setFirstTransport ] = useState(null)
  const [ lastTransport, setLastTransport ] = useState(null)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ loading, setLoading ] = useState(false)
  const location = useLocation()
  const [ form ] = Form.useForm()

  const InLocations = {
    DepartLocationId: state.userProfileData?.LocationId,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
  }

  const OutLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
    ArriveLocationId: state.userProfileData?.LocationId,
  }

  const handleSelectionButtonFT = () => {
    let tmp = {
      StartDate: form.getFieldValue(['firstTransport', 'Date']) ? form.getFieldValue(['firstTransport', 'Date']) : null,
      EndDate: null,
    }
    if(firstTransport){
      tmp.DepartLocationId = firstTransport.FromLocationId
      tmp.ArriveLocationId = firstTransport.ToLocationId
    }else{
      if(form.getFieldValue(['firstTransport', 'Direction']) && form.getFieldValue(['firstTransport', 'Direction']) === 'IN'){
        tmp = {
          ...tmp,
          ...InLocations,
        }
      }else{
        tmp = {
          ...tmp,
          ...OutLocations,
        }
      }
    }
    setTransportSelectionInitValues({...tmp, transportType: 'first'})
    setShowLTSelectionModal(true)
  }

  const handleSelectionButtonLT = () => {
    let tmp = {
      StartDate: form.getFieldValue(['lastTransport', 'Date']) ? form.getFieldValue(['lastTransport', 'Date']) : null,
      EndDate: null,
    }
    if(lastTransport){
      tmp.DepartLocationId = lastTransport?.FromLocationId
      tmp.ArriveLocationId = lastTransport?.ToLocationId
    }else{
      if(form.getFieldValue(['lastTransport', 'Direction']) && form.getFieldValue(['lastTransport', 'Direction']) === 'IN'){
        tmp = {
          ...tmp,
          ...InLocations,
        }
      }else{
        tmp = {
          ...tmp,
          ...OutLocations,
        }
      }
    }
    setTransportSelectionInitValues({...tmp, transportType: 'last'})
    setShowLTSelectionModal(true)
  }

  const handleDirectionChange = (e) => {
    setFirstTransport(null)
    setLastTransport(null)
    if(e === 'IN'){
      form.setFieldValue(['lastTransport', 'Direction'], 'OUT')
      form.setFieldValue(['firstTransport', 'Description'], null)
      form.setFieldValue(['firstTransport', 'flightId'], null)
      form.setFieldValue(['lastTransport', 'Description'], null)
      form.setFieldValue(['lastTransport', 'flightId'], null)
    }else{
      form.setFieldValue(['lastTransport', 'Direction'], 'IN')
      form.setFieldValue(['firstTransport', 'flightId'], null)
      form.setFieldValue(['firstTransport', 'Description'], null)
      form.setFieldValue(['lastTransport', 'flightId'], null)
      form.setFieldValue(['lastTransport', 'Description'], null)
    }
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='Transport' className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item name={['firstTransport', 'Date']} className='mb-0 col-span-3' rules={[{required: true, message: 'First Transport Date is required'}]}>
            <DatePicker showWeek onChange={(e) => {form.setFieldValue('RoomId', ''); action.setFlightCalendarDate(dayjs(e).format('YYYY-MM-DD'))}} className='w-full'/>
          </Form.Item>
          <Form.Item name={['firstTransport', 'Direction']} className='mb-0 col-span-2' rules={[{required: true, message: 'Direction is required'}]}>
            <Select options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full' onChange={handleDirectionChange}/>
          </Form.Item>
          <Form.Item noStyle name={['firstTransport', 'flightId']}>
          </Form.Item>
          <Form.Item name={['firstTransport', 'Description']} className='col-span-6 mb-0'>
            <Input readOnly/>
          </Form.Item>
          <Form.Item
            className='col-span-1 mb-0'
            shouldUpdate={(pre, cur) => pre.firstTransport?.Date !== cur.firstTransport?.Date || pre.firstTransport?.Direction !== cur.firstTransport?.Direction}
          >
            {
              ({getFieldValue}) => (
                <Button
                  className='text-xs py-[5px]'
                  type={'primary'}
                  onClick={handleSelectionButtonFT}
                  disabled={!getFieldValue(['firstTransport', 'Date']) || !getFieldValue(['firstTransport', 'Direction'])}
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
      label: 'Position',
      name: 'PositionId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Position is required'}],
      inputprops: {
        options: state.referData?.positions,
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
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Employer is required'}],
      inputprops: {
        options: state.referData?.employers,
      }
    },
  ]

  const handleSelect = (event) => {
    if(transportSelectionInitValues.transportType === 'first'){
      setFirstTransport(event)
      form.setFieldValue(['firstTransport', 'Date'], dayjs(event.EventDate))
      form.setFieldValue(['firstTransport', 'Description'], `${event.Code} ${event.Description} ${event.TransportMode}`)
      form.setFieldValue(['firstTransport', 'flightId'], event.Id)
      setShowLTSelectionModal(false)
      }else{
      setLastTransport(event)
      form.setFieldValue(['lastTransport', 'Date'], dayjs(event.EventDate))
      form.setFieldValue(['lastTransport', 'Description'], `${event.Code} ${event.Description} ${event.TransportMode}`)
      form.setFieldValue(['lastTransport', 'flightId'], event.Id)
      setShowLTSelectionModal(false)
    }
  }

  const handleSubmitAdd = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/safemode/transport',
      data: {
        employeeId: parseInt(employeeId),
        ScheduleId: values.firstTransport.flightId,
        departmentId: values.DepartmentId,
        positionId: values.PositionId,
        employerId: values.EmployerId,
        costCodeId: values.CostCodeId,
      }
    }).then((res) => {
      onCancel()
      refresh()
      form.resetFields()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  return (
    <>
      <Form
        form={form} 
        fields={fields} 
        editData={{...state.userProfileData}}
        onFinish={handleSubmitAdd}
        labelCol={{flex: '120px'}} 
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
        />
      </Modal>
    </>
  )
}

export default AddTransport