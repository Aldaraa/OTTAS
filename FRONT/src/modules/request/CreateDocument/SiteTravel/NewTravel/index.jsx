import { CarOutlined, SaveOutlined } from '@ant-design/icons';
import { Checkbox, DatePicker, Input, Select, TreeSelect } from 'antd';
import axios from 'axios';
import { Form, Modal, RoomSelection, Table, Button, TransportSearch, ButtonDrive } from 'components'
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import React, { useContext, useEffect, useState } from 'react'
import { FaCarSide } from 'react-icons/fa';
import { useLocation, useParams } from 'react-router-dom';
var duration = require('dayjs/plugin/duration')
dayjs.extend(duration)

const getLabel = (_field) => {
  return (
    <span
      className={`${
        _field.inputprops?.disabled ? 'text-[#888]' : ''
      }`}
    >
      {_field.label}
    </span>
  );
};

function NewTravel({form, onSubmit}) {
  const { state } = useContext(AuthContext)
  const [ formFirstDate, setFormFirstDate ] = useState(null)
  const [ formLastDate, setFormLastDate ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ annualYearData, setAnnualYearData ] = useState([])
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ firstTransport, setFirstTransport ] = useState(null)
  const [ lastTransport, setLastTransport ] = useState(null)

  const location = useLocation()
  const { empId } = useParams()

  const DefaultShiftId = state.referData?.roomStatuses.find((item) => item.Code === 'DS')
  const InLocations = {
    DepartLocationId: state.userProfileData?.LocationId,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
  }
  const OutLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
    ArriveLocationId: state.userProfileData?.LocationId,
  }

  useEffect(() => {
    form.setFieldsValue(state.userProfileData)
    form.setFieldValue('ShiftId', DefaultShiftId.value)
  },[state.userProfileData])

  useEffect(() => {
    if(formFirstDate && formLastDate){
      let startDate = formFirstDate?.format('YYYY-MM-DD')
      let duration = formLastDate.diff(formFirstDate, 'month')
      axios({
        method: 'get',
        url: `tas/employeestatus/annualyear/${empId}/${startDate}/${duration + 1}`
      }).then((res) => {
        setAnnualYearData(res.data)
      }).catch((err) => {
  
      })
    }
  },[formFirstDate, formLastDate])

  const annualCols = [
    {
      label: 'Shift',
      name: 'ShiftCode',
      width: '50px'
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Date',
      name: 'EvenDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
  ]

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
      DepartLocationId: null,
      ArriveLocationId: null
    }
    if(lastTransport){
      tmp.DepartLocationId = lastTransport.FromLocationId
      tmp.ArriveLocationId = lastTransport.ToLocationId
    }else{
      if(firstTransport){
        tmp.DepartLocationId = firstTransport.ToLocationId
        tmp.ArriveLocationId = firstTransport.FromLocationId
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
    }
    setTransportSelectionInitValues({...tmp, transportType: 'last'})
    setShowLTSelectionModal(true)
  }

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

  const handleChangeFirstDate = (e) => {
    setFormFirstDate(e)
    setFirstTransport(null)
    form.setFieldValue('RoomId', '')
    form.setFieldValue(['firstTransport', 'flightId'], null)
    form.setFieldValue(['firstTransport', 'Description'], null)
  }

  const handleChangeLastDate = (e) => {
    setFormLastDate(e)
    setLastTransport(null)
    form.setFieldValue('RoomId', '')
    form.setFieldValue(['lastTransport', 'Description'], null); 
    form.setFieldValue(['lastTransport', 'flightId'], null);
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

  const handleChangeDrive = (data, name) => {
    form.setFieldValue([name, 'Date'], dayjs(data.EventDate))
    form.setFieldValue([name, 'Description'], `${data.Code} ${data.Description}`)
    form.setFieldValue([name, 'flightId'], data.Id)
  }

  return (
    <div className='max-w-[1000px] grid grid-cols-12 px-5 gap-5 items-start'> 
      <div className='col-span-12'>
        <div className='text-lg font-bold mb-3'>New Travel Request</div>
        <div className='grid grid-cols-12'>
          <Form.Item label={<span><span className='text-red-500'>*</span> First Transport</span>} labelCol={{flex: '150px'}} className='col-span-12 mb-3'>
            <div className='flex gap-2'>
              <Form.Item name={['firstTransport', 'Date']} className='mb-0 col-span-3'>
                <DatePicker showWeek onChange={handleChangeFirstDate} className='w-full'/>
              </Form.Item>
              <Form.Item name={['firstTransport', 'Direction']} className='mb-0 w-[70px]'>
                <Select 
                  options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]}
                  onChange={handleDirectionChange}
                />
              </Form.Item>
              <Form.Item noStyle name={['firstTransport', 'flightId']} rules={[{required: true, message: 'First Transport is required !'}]}>
              </Form.Item>
              <Form.Item name={['firstTransport', 'Description']} className='flex-1 mb-0'>
                <Input readOnly className='py-[1px]'/>
              </Form.Item>
              <Form.Item
                className='col-span-1 mb-0'
                shouldUpdate={(pre, cur) => pre.firstTransport?.Date !== cur.firstTransport?.Date || pre.firstTransport?.Direction !== cur.firstTransport?.Direction}
              >
                {
                  ({getFieldValue}) => (
                    <Button
                      className='text-xs py-[2px]'
                      type={'primary'}
                      onClick={handleSelectionButtonFT}
                      disabled={!getFieldValue(['firstTransport', 'Date']) || !getFieldValue(['firstTransport', 'Direction'])}
                    >
                      ...
                    </Button>
                  )
                }
              </Form.Item>
              <Form.Item
                className='col-span-1 mb-0'
                shouldUpdate={(pre, cur) => pre.firstTransport?.Date !== cur.firstTransport?.Date || pre.firstTransport?.Direction !== cur.firstTransport?.Direction}
              >
                {
                  ({getFieldValue, setFieldValue}) => {
                    const searchValues = {
                      eventDate: getFieldValue(['firstTransport', 'Date']),
                      direction: getFieldValue(['firstTransport', 'Direction']),
                      morning: true,
                    }
                    return(
                      <ButtonDrive
                        onRecieve={handleChangeDrive}
                        searchValues={searchValues}
                        name='firstTransport'
                        disabled={!getFieldValue(['firstTransport', 'Date']) || !getFieldValue(['firstTransport', 'Direction'])}
                      />
                    )
                  }

                }
              </Form.Item>
              <Form.Item
                className='col-span-1 mb-0'
                shouldUpdate={(pre, cur) => pre.firstTransport?.Date !== cur.firstTransport?.Date || pre.firstTransport?.Direction !== cur.firstTransport?.Direction}
              >
                {
                  ({getFieldValue}) => {
                    const searchValues = {
                      eventDate: getFieldValue(['firstTransport', 'Date']),
                      direction: getFieldValue(['firstTransport', 'Direction']),
                      morning: false,
                    }
                    return(
                      <ButtonDrive
                        onRecieve={handleChangeDrive}
                        searchValues={searchValues}
                        name='firstTransport'
                        disabled={!getFieldValue(['firstTransport', 'Date']) || !getFieldValue(['firstTransport', 'Direction'])}
                        mode='evening'
                      />
                    )
                  }
                }
              </Form.Item>
              <Form.Item
                name={['firstTransport', 'GoShow']}
                labelCol={{flex: '150px'}}
                className='col-span-12 mb-0'
                valuePropName="checked"
                getValueFromEvent={(e) => e.target?.checked ? 1 : 0}
              >
                <Checkbox>Go Show</Checkbox>
              </Form.Item>
            </div>
          </Form.Item>
          <Form.Item label={<span><span className='text-red-500'>*</span> Last Transport</span>} labelCol={{flex: '150px'}} className='col-span-12 mb-3'>
            <div className='flex gap-2'>
              <Form.Item noStyle shouldUpdate={(prev, cur) => prev.firstTransport?.Date !== cur.firstTransport?.Date || prev.lastTransport?.Date !== cur.lastTransport?.Date}>
                {({getFieldValue}) => {
                  let defaultPickerValue = false
                  if(!getFieldValue(['lastTransport', 'Date'])){
                    defaultPickerValue = getFieldValue(['firstTransport', 'Date'])
                  }
                  return(
                    <Form.Item name={['lastTransport', 'Date']} className='mb-0 col-span-3'>
                      <DatePicker
                        defaultPickerValue={defaultPickerValue}
                        showWeek
                        className='w-full'
                        onChange={() => handleChangeLastDate()}
                      />
                    </Form.Item>
                  )
                }}
              </Form.Item>
              <Form.Item name={['lastTransport', 'Direction']} className='mb-0 w-[70px]'>
                <Input className='py-[1px]' disabled/>
                {/* <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/> */}
              </Form.Item>
              <Form.Item noStyle name={['lastTransport', 'flightId']} rules={[{required: true, message: 'First Transport is required !'}]}>
              </Form.Item>
              <Form.Item name={['lastTransport', 'Description']} className='flex-1 mb-0'>
                <Input readOnly className='py-[1px]'/>
              </Form.Item>
              <Form.Item
                className='col-span-1 mb-0'
                shouldUpdate={(pre, cur) => pre.lastTransport?.Date !== cur.lastTransport?.Date || pre.lastTransport?.Direction !== cur.lastTransport?.Direction}
              >
                {
                  ({getFieldValue}) => (
                    <Button
                      className='text-xs py-[2px]'
                      type={'primary'}
                      onClick={handleSelectionButtonLT}
                      disabled={!getFieldValue(['lastTransport', 'Date']) || !getFieldValue(['lastTransport', 'Direction'])}
                    >
                      ...
                    </Button>
                  )
                }
              </Form.Item>
              <Form.Item
                className='col-span-1 mb-0'
                shouldUpdate={(pre, cur) => pre.lastTransport?.Date !== cur.lastTransport?.Date || pre.lastTransport?.Direction !== cur.lastTransport?.Direction}
              >
                {
                  ({getFieldValue}) => {
                    const searchValues = {
                      eventDate: getFieldValue(['lastTransport', 'Date']),
                      direction: getFieldValue(['lastTransport', 'Direction']),
                      morning: true,
                    }
                    return(
                      <ButtonDrive
                        onRecieve={handleChangeDrive}
                        searchValues={searchValues}
                        name='lastTransport'
                        disabled={!getFieldValue(['lastTransport', 'Date']) || !getFieldValue(['lastTransport', 'Direction'])}
                      />
                    )
                  }
                }
              </Form.Item>
              <Form.Item
                className='col-span-1 mb-0'
                shouldUpdate={(pre, cur) => pre.lastTransport?.Date !== cur.lastTransport?.Date || pre.lastTransport?.Direction !== cur.lastTransport?.Direction}
              >
                {
                  ({getFieldValue}) => {
                    const searchValues = {
                      eventDate: getFieldValue(['lastTransport', 'Date']),
                      direction: getFieldValue(['lastTransport', 'Direction']),
                      morning: false,
                    }
                    return(
                      <ButtonDrive
                        onRecieve={handleChangeDrive}
                        searchValues={searchValues}
                        name='lastTransport'
                        disabled={!getFieldValue(['lastTransport', 'Date']) || !getFieldValue(['lastTransport', 'Direction'])}
                        mode='evening'
                      />
                    )
                  }
                }
              </Form.Item>
              <Form.Item
                name={['lastTransport', 'GoShow']}
                labelCol={{flex: '150px'}}
                className='col-span-12 mb-0'
                valuePropName="checked"
                getValueFromEvent={(e) => e.target?.checked ? 1 : 0}
              >
                <Checkbox>Go Show</Checkbox>
              </Form.Item>
            </div>
          </Form.Item>
          <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.firstTransport?.Direction !== curValues.firstTransport?.Direction}>
            {({getFieldValue}) => {
              return(
                getFieldValue(['firstTransport', 'Direction']) === 'OUT' ?
                <Form.Item label='Shift' labelCol={{flex: '150px'}} name={'ShiftId'} className='col-span-12 mb-3' rules={[{required: true, message: 'Shift is required'}]}>
                  <Select
                    options={state.referData?.roomStatuses?.filter((item) => item.OnSite === 0)}
                    className='w-full'
                    filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                    showSearch
                    allowClear
                  />
                </Form.Item>
                :
                <Form.Item label='Shift' labelCol={{flex: '150px'}} name={'ShiftId'} className='col-span-12 mb-3' rules={[{required: true, message: 'Shift is required'}]}>
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
            className='col-span-12 mb-3'
            labelCol={{flex: '150px'}}
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
            labelCol={{flex: '150px'}}
            className='col-span-12 mb-3'
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
            labelCol={{flex: '150px'}}
            className='col-span-12 mb-3'
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
            labelCol={{flex: '150px'}}
            className='col-span-12 mb-3'
            rules={[{required: true, message: 'Employer is required'}]}
          >
            <Select 
              allowClear
              showSearch
              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              options={state.referData?.employers}
            />
          </Form.Item>
          <Form.Item
            key={`form-item-Reason`}
            label='Reason'
            name={'Reason'}
            labelCol={{flex: '150px'}}
            className='col-span-12 mb-2'
            rules={[{required: true, message: 'Reason is required'}]}
          >
            <Input.TextArea
              showCount
              maxLength={300}
            />
          </Form.Item>
        </div>
      </div>
      {
        annualYearData.length > 0 &&
        <div className='col-span-4'>
          <Table
            data={annualYearData}
            columns={annualCols}
            containerClass='shadow-none border'
            tableClass='border-t'
            pager={annualYearData.length > 20}
            title={<div className='py-1 font-bold'>Future Annual Leave</div>}
          />
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
        />
      </Modal>
    </div>
  )
}

export default NewTravel