import { CloseCircleTwoTone, LeftOutlined } from '@ant-design/icons'
import { Checkbox, Form, Input, InputNumber, Select, TreeSelect } from 'antd'
import axios from 'axios'
import { Button, Modal, Table, DatePicker, TransportSearch } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { lazy, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import WarningDetail from './WarningDetail'
import { IoEllipseOutline } from 'react-icons/io5'

const Result = lazy(() => import('./Result'))

function BulkingView({data=[], handleChangeData, changeIsEditing, className, selectionTableRef }) {
  const [ processLoading, setProcessLoading ] = useState(false)
  const [ startDate, setStartDate ] = useState(dayjs())
  const [ duration, setDuration ] = useState(null)
  const [ flightGM, setFlightGM ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ previewData, setPreviewData ] = useState([]) 
  const [ rosterExecuteData, setRosterExecuteData ] = useState([])
  const [ showModal, setShowModal ] = useState(false)
  const [ showResultModal, setShowResultModal ] = useState(false)
  const [ resultData, setResultData ] = useState([])
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ firstTransport, setFirstTransport ] = useState(null)
  const [ lastTransport, setLastTransport ] = useState(null)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)

  const { state, action } = useContext(AuthContext)
  const [ form ] = Form.useForm()
  const firstTransportValue = Form.useWatch('firstTransport', form)
  const lastTransportValue = Form.useWatch('lastTransport', form)
  const employeeList = Form.useWatch('rosters', form)

  const InLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
  }

  const OutLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
  }

  const onSiteShifts = useMemo(() => {
    const filtered = state.referData?.roomStatuses?.filter((item) => item.OnSite)
    const defualtValue = filtered?.find((item) => item.Code === 'DS')
    return {
      list: filtered || [],
      defualtValue: defualtValue,
    }
  },[state.referData?.roomStatuses])

  useEffect(() => {
    const initedData = data.map((item) => ({
      "employeeId": item.Id,
      "Fullname": item.Fullname,
      "departmentId": item.Departmentid,
      "positionId": item.PositionId,
      "employerId": item.EmployerId,
      "costCodeId": item.CostCodeId,
      "firsScheduleGoShow": 0,
      "lastSheduleGoShow": 0,
      "shiftId": onSiteShifts.defualtValue?.value}))
    form.setFieldsValue({
      rosters: initedData, 
      firstTransport: {Direction: 'IN'},
      lastTransport: {Direction: 'OUT'}
    })
  },[data, onSiteShifts])

  const handleRemoveFromModal = ({data}) => {
    setPreviewData(previewData.filter((item) => item.EmployeeId !== data.EmployeeId))
  }

  const errorColumns = [
    {
      label: 'Fullname',
      name: 'FullName',
      alignment: 'left',
      width: '200px',
      cellRender: (e) => (
        <div className='text-blue-500 hover:underline'>
          <Link to={`/tas/people/search/${e.data.EmployeeId}`}>{e.value}</Link>
        </div>
      )
    },
    {
      label: 'Error',
      name: 'Error',
    },
    {
      label: '',
      name: 'action',
      alignment: 'right',
      width: 100,
      cellRender: (e) => (
        <button type='button' className='dlt-button' onClick={() => handleRemoveFromModal(e)} >Remove</button>
      )
    },
  ]

  const handleSubmit = useCallback((values) => {
    if(values.rosters?.length !== 0){
      setLoading(true)
      let sendData = []
      values.rosters?.map((item) => {
        sendData.push(item.employeeId)
      })
      axios({
        method: 'post',
        url: 'tas/transport/multiplebooking/preview',
        data: {
          firsScheduleId: values.firstTransport?.flightId,
          lastSheduleId: values.lastTransport?.flightId,
          employeeIds: sendData
        },
      }).then((res) => {
        setPreviewData(res.data)
        setShowModal(true)
      }).catch((err) => {
  
      }).then(() => setLoading(false))
    }
  },[])

  const clearDatas = useCallback(() => {
    handleChangeData([])
    changeIsEditing(false)
  },[])

  const handleProcess = () => {
    const employeeIds = previewData.map((item) => item.EmployeeId)
    const sendEmployeeList = employeeList.filter((item) => employeeIds.includes(item.Id))
    setProcessLoading(true)
    axios({
      method: 'post',
      url: 'tas/transport/multiplebooking',
      data: {
        firsScheduleId: firstTransportValue?.flightId,
        lastSheduleId: lastTransportValue?.flightId,
        employeeData: employeeList
      },
    }).then((res) => {
      setResultData(res.data)
      if(res.data?.length > 0){
        setShowResultModal(true)
      }
      setShowModal(false)
    }).catch((err) => {

    }).then(() => setProcessLoading(false))
  }
  
  const handleBulkChangeField = (value, name) => {
    if(name === 'departmentId'){
      let selectedDepartment = state.referData.departments?.find((department) => department.Id === value)
      if(selectedDepartment && selectedDepartment.CostCodeId){
        employeeList?.map((field, index) => {
          form.setFieldValue(['rosters', index, 'costCodeId'], selectedDepartment.CostCodeId);
        })
      }
    }else{
      employeeList?.map((field, index) => {
        form.setFieldValue(['rosters', index, name], value);
      })
    }
  }

  const handleBack = () => {
    handleChangeData(employeeList)
    changeIsEditing(false)
  }

  const removeRow = (index) => {
    const empId = form.getFieldValue(['rosters', index, 'Id'])
    selectionTableRef.current.instance.deselectRows([empId])
  }

  const handleSelectionButtonFT = () => {
    let tmp = {
      StartDate: firstTransportValue?.Date || null,
      EndDate: null,
    }
    if(firstTransport){
      tmp.DepartLocationId = firstTransport.FromLocationId
      tmp.ArriveLocationId = firstTransport.ToLocationId
    }else{
      tmp = {
        ...tmp,
        ...InLocations,
      }
    }
    setTransportSelectionInitValues({...tmp, transportType: 'first'})
    setShowLTSelectionModal(true)
  }

  const handleSelectionButtonLT = () => {
    let tmp = {
      StartDate: lastTransportValue?.Date || null,
      EndDate: null,
    }
    if(lastTransport){
      tmp.DepartLocationId = lastTransport?.FromLocationId
      tmp.ArriveLocationId = lastTransport?.ToLocationId
    }else{
      if(firstTransport){
        tmp = {
          ...tmp,
          DepartLocationId: firstTransport?.ToLocationId,
          ArriveLocationId: firstTransport?.FromLocationId
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

  const onChangeFTDate = () => {
    setFirstTransport(null)
    form.setFieldValue(['firstTransport', 'Description'], null)
    form.setFieldValue(['firstTransport', 'flightId'], null)
  }

  const onChangeLTDate = () => {
    setLastTransport(null)
    form.setFieldValue(['lastTransport', 'Description'], null)
    form.setFieldValue(['lastTransport', 'flightId'], null)
  }

  return (
    <div className={`rounded-ot bg-white p-5 mb-5 shadow-md ${className}`}>
      <Button className='mb-3' onClick={handleBack} icon={<LeftOutlined/>}>Back</Button>
      <Form 
        form={form} 
        size='small' 
        onFinish={handleSubmit} 
        initialValues={{
          rosters: [], 
          startDate: null, 
          durationMonth: 0, 
          flightGroupMasterId: null
        }}
      >
        <Form.List name='rosters'>
          {(fields, {remove}) => (
            <div className='border rounded-ot overflow-auto'>
              <table className='table-auto overflow-scroll w-full text-xs'>
                <thead className='text-[#959595]'> 
                  <tr className='text-left font-normal'>
                    <th className='border-b px-1 sticky left-0 z-10 bg-white'>#</th>
                    <th className='border-b px-1 sticky left-[38px] z-10 bg-white'>Fullname</th>
                    <th className='border-b px-1'>
                      <TreeSelect 
                        style={{width: 200}}
                        treeData={state.referData?.departments}
                        virtual={true}
                        treeLine
                        popupMatchSelectWidth={false}
                        showSearch
                        fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
                        filterTreeNode={(input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase())}
                        placeholder="Department" 
                        onChange={(e) => handleBulkChangeField(e, 'departmentId')}
                        allowClear
                      />
                    </th>
                    <th className='border-b px-1'>
                      <Select 
                        style={{width: 200}}
                        options={state.referData?.positions}
                        popupMatchSelectWidth={false}
                        placeholder="Position" 
                        allowClear
                        showSearch
                        filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        onChange={(e) => handleBulkChangeField(e, 'positionId')}
                      />
                    </th>
                    <th className='border-b px-1'>
                      <Select 
                        style={{width: 200}} 
                        options={state.referData?.costCodes} 
                        placeholder="Cost Code" 
                        allowClear
                        showSearch
                        filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        onChange={(e) => handleBulkChangeField(e, 'costCodeId')}
                      />
                    </th>
                    <th className='border-b px-1'>
                      <Select 
                        style={{width: 200}} 
                        options={state.referData?.employers} 
                        placeholder="Employer" 
                        allowClear
                        showSearch
                        filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        onChange={(e) => handleBulkChangeField(e, 'employerId')}
                      />
                    </th>
                    <th className='border-b px-1'>
                      <Select 
                        style={{width: 200}} 
                        options={onSiteShifts.list} 
                        placeholder="Shift" 
                        allowClear
                        showSearch
                        filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        onChange={(e) => handleBulkChangeField(e, 'shiftId')}
                      />
                    </th>
                    <th className='border-b px-1 font-normal'>
                      <Checkbox
                        style={{width: 110}} 
                        onChange={(e) => handleBulkChangeField(e.target.checked, 'firsScheduleGoShow')}
                      >
                        <b>IN</b> Go show
                      </Checkbox>
                    </th>
                    <th className='border-b px-1 font-normal'>
                      <Checkbox
                        style={{width: 120}}
                        onChange={(e) => handleBulkChangeField(e.target.checked, 'lastSheduleGoShow')}
                      >
                        <b>OUT</b> Go show
                      </Checkbox>
                    </th>
                    <th className='border-b px-1'></th>
                  </tr>
                </thead>
                <tbody>
                  {
                    fields.map(({key, name, ...restField}) => (
                      <tr>
                        <td className={`p-1 border-t sticky left-0 z-10 ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <Form.Item className='mb-0'>
                            <div className='w-[30px]'>{name+1}</div>
                          </Form.Item>
                        </td>
                        <td className={`p-1 border-t sticky left-[38px] z-10 ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <Form.Item className='mb-0 text-xs'>
                              <Link to={`/tas/people/search/${form.getFieldValue(['rosters', name, 'employeeId'])}`}>
                                <span className='text-blue-500 hover:underline text-xs'>{form.getFieldValue(['rosters', name, 'Fullname'])}</span>
                              </Link>
                            {/* <div className='w-[200px] text-[13px]'></div> */}
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item
                            {...restField}
                            name={[name, 'departmentId']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Department is required',
                              },
                            ]}
                          >
                            <TreeSelect 
                              style={{width: 200}}
                              treeData={state.referData?.departments}
                              virtual={true}
                              treeLine
                              popupMatchSelectWidth={false}
                              allowClear
                              showSearch
                              fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
                              filterTreeNode={(input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase())}
                              placeholder="Department" 
                            />
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item
                            {...restField}
                            name={[name, 'positionId']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Position is required',
                              },
                            ]}
                          >
                            <Select 
                              style={{width: 200}} 
                              options={state.referData?.positions} placeholder="Position" 
                              popupMatchSelectWidth={false}
                              allowClear
                              showSearch
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                            />
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.rosters[name]?.departmentId !== cur.rosters[name]?.departmentId}>
                            {({getFieldValue, setFieldValue}) => {
                              let selectedDepartment = state.referData.departments?.find((department) => department.Id === getFieldValue(['rosters', name, 'departmentId']))
                              if(selectedDepartment && selectedDepartment.CostCodeId){
                                setFieldValue(['rosters', name, 'costCodeId'], selectedDepartment.CostCodeId)
                              }
                              return (
                                <Form.Item
                                  {...restField}
                                  name={[name, 'costCodeId']}
                                  className='mb-0'
                                  rules={[
                                    {
                                      required: true,
                                      message: 'Cost Code is required',
                                    },
                                  ]}
                                >
                                  <Select 
                                    style={{width: 200}} 
                                    options={state.referData?.costCodes} placeholder="Cost Code" 
                                    allowClear
                                    showSearch
                                    popupMatchSelectWidth={false}
                                    filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                                  />
                                </Form.Item>
                              )
                            }}
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item
                            {...restField}
                            name={[name, 'employerId']}
                            className='mb-0'
                            rules={[{ required: true, message: 'Employer is required', }]}
                          >
                            <Select 
                              style={{width: 200}} 
                              options={state.referData?.employers} placeholder="Employer" 
                              popupMatchSelectWidth={false}
                              allowClear
                              showSearch
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                            />
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item
                            {...restField}
                            name={[name, 'shiftId']}
                            className='mb-0'
                            rules={[{ required: true, message: 'Shift is required'},]}
                          >
                            <Select 
                              style={{width: 200}} 
                              options={onSiteShifts.list} placeholder="Shift" 
                              popupMatchSelectWidth={false}
                              allowClear
                              showSearch
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                            />
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item
                            {...restField}
                            name={[name, 'firsScheduleGoShow']}
                            className='mb-0'
                            // getValueProps={(value) => ({value: value ? 1 : 0})}
                            valuePropName='checked'
                          >
                            <Checkbox style={{width: 110}}><b>IN</b> Go Show</Checkbox>
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item
                            {...restField}
                            name={[name, 'lastSheduleGoShow']}
                            className='mb-0'
                            // getValueProps={(value) => ({value: value ? 1 : 0})}
                            valuePropName='checked'
                          >
                            <Checkbox style={{width: 120}} ><b>OUT</b> Go Show</Checkbox>
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <button
                            type='button'
                            className='dlt-button text-xs'
                            onClick={() => {removeRow(name); remove(name);}}
                          >
                            Remove
                          </button>
                        </td>
                      </tr>
                    ))
                  }
                </tbody>
              </table>
            </div>
          )}
        </Form.List>
        <div className='col-span-12 w-[800px] gap-5 mt-8'>
          <Form.Item label='First Transport' className='col-span-12 mb-2'>
            <div className='flex gap-2'>
              <Form.Item name={['firstTransport', 'Date']} className='mb-0 w-[130px]' rules={[{required: true, message: 'First Transport Date is required'}]}>
                <DatePicker size='medium' showWeek onChange={onChangeFTDate} className='w-full'/>
              </Form.Item>
              <Form.Item name={['firstTransport', 'Direction']} className='mb-0 w-[70px]' rules={[{required: true, message: 'Direction is required'}]}>
                <Input size='medium' disabled/>
              </Form.Item>
              <Form.Item noStyle name={['firstTransport', 'flightId']}>
              </Form.Item>
              <Form.Item name={['firstTransport', 'Description']} className='flex-1 mb-0'>
                <Input size='medium' readOnly/>
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
          <Form.Item label='Last Transport' className='col-span-12 mb-2'>
            <div className='flex gap-2'>
              <Form.Item noStyle shouldUpdate={(prev, cur) => prev.firstTransport?.Date !== cur.firstTransport?.Date}>
                {({getFieldValue}) => {
                  let defaultPickerValue = false
                  if(!getFieldValue(['lastTransport', 'Date'])){
                    defaultPickerValue = getFieldValue(['firstTransport', 'Date'])
                  }
                  return(
                    <Form.Item name={['lastTransport', 'Date']} className='mb-0 w-[130px]' rules={[{required: true, message: 'Last Transport Date is required'}]}>
                      <DatePicker size='medium' defaultPickerValue={defaultPickerValue} showWeek onChange={onChangeLTDate} className='w-full'/>
                    </Form.Item>
                  )
                }}
              </Form.Item>
              <Form.Item name={['lastTransport', 'Direction']} className='mb-0 w-[70px]'>
                <Input disabled size='medium' className='w-full'/>
              </Form.Item>
              <Form.Item noStyle name={['lastTransport', 'flightId']}>
              </Form.Item>
              <Form.Item name={['lastTransport', 'Description']} className='flex-1 mb-0'>
                <Input size='medium' readOnly/>
              </Form.Item>
              <Form.Item
                className='col-span-1 mb-0'
                shouldUpdate={(pre, cur) => pre.lastTransport?.Date !== cur.lastTransport?.Date}
              >
                {
                  ({getFieldValue}) => (
                    <Button
                      className='text-xs py-[5px]'
                      type={'primary'}
                      onClick={handleSelectionButtonLT}
                      disabled={!getFieldValue(['lastTransport', 'Date'])}
                    >
                      ...
                    </Button>
                  )
                }
              </Form.Item>
            </div>
          </Form.Item>
          <Form.Item shouldUpdate={(prev, next) => prev.startDate !== next.startDate || prev.durationMonth !== next.durationMonth || prev.flightGroupMasterId !== next.flightGroupMasterId}>
            {
              ({getFieldValue}) => (
                <Form.Item>
                  <Button 
                    type='primary'
                    onClick={() => form.submit()}
                    loading={loading}
                  >
                    Submit
                  </Button>
                </Form.Item>
              )
            }
          </Form.Item>
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
      <Modal title='Changing' open={showModal} onCancel={()=>setShowModal(false)} width={900}>
        <div className='mt-0 text-xs'>
          <Table
            data={previewData}
            columns={errorColumns}
            containerClass='shadow-none border overflow-hidden' 
            pager={previewData?.length > 20}
          />
        </div>
        <div className='flex justify-end gap-5 items-center mt-3'>
          <Button 
            type='primary'
            loading={processLoading}
            onClick={() => handleProcess()}
          >
            Process
          </Button>
          <Button  
            onClick={() => setShowModal(false)}
            disabled={processLoading}
          >
            Cancel
          </Button>
        </div>
      </Modal>
      <Modal 
        open={showResultModal} 
        onCancel={() => {setShowResultModal(false); clearDatas()}} 
        title='Roster response'
        width={900}
      >
        <Result data={resultData}/>
      </Modal>
    </div>
  )
}

export default BulkingView