import { CloseCircleTwoTone, LeftOutlined } from '@ant-design/icons'
import { Form, InputNumber, Select, TreeSelect } from 'antd'
import axios from 'axios'
import { Button, Modal, Table, DatePicker } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { lazy, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import WarningDetail from './WarningDetail'
import Changing from './Changing'

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

  const { state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    form.setFieldsValue({rosters: data})
  },[data])

  const handleRemoveFromModal = (row) => {
    let tmp = data.filter((item) => item.Id !== row.EmpId)
    handleChangeData(tmp)
    setPreviewData(previewData.filter((item) => item.EmpId !== row.EmpId))
    setRosterExecuteData(rosterExecuteData.filter((item) => item.employeeId !== row.EmpId))
  }

  const handleOnSiteDelete = (row) => {
    setActionLoading(row.EmpId)
    axios({
      method: 'delete',
      url: `tas/employee/deletetransport/${row.EmpId}/${dayjs(row.OnsiteData.EventDate).format('YYYY-MM-DD')}`
    }).then(() => {
      setPreviewData(previewData.filter((item) => item.EmpId !== row.EmpId))
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const errorColumns = [
    {
      label: 'Fullname',
      name: 'Fullname',
      alignment: 'left',
      width: '200px',
      // cellRender: (e) => (
      //   <Link to={`/tas/people/search/${e.data.Id}`}>{e.value}</Link>
      // )
    },
    {
      label: 'On Site Status',
      name: 'EmpOnSiteStatus',
      alignment: 'center',
      cellRender:(e) => (
        <div>{e.value ? <CloseCircleTwoTone twoToneColor='red' style={{fontSize: '16px'}}/> : ''}</div>
      )
    },
    {
      label: 'Off Site Status',
      name: 'EmpOffSiteStatus',
      alignment: 'center',
      cellRender:(e) => (
        <div>{e.value ? <CloseCircleTwoTone twoToneColor='red' style={{fontSize: '16px'}}/> : ''}</div>
      )
    },
    {
      label: '',
      name: 'action',
      alignment: 'right',
      width: 220,
      cellRender: (e) => (
        <div className='flex gap-3'>
          {
            e.data.EmpOnSiteStatus ?
            <Button
              type='danger'
              onClick={() => handleOnSiteDelete(e.data)}
              disabled={actionLoading}
              loading={e.data.EmpId === actionLoading}
            >
              On Site Delete
            </Button>
            : null
          }
          <button type='button' className='dlt-button' onClick={() => handleRemoveFromModal(e.data)} >Remove</button>
        </div>
      )
    },
  ]

  const handleSubmit = useCallback((values) => {
    if(values.rosters?.length !== 0){
      setLoading(true)
      let sendData = []
      values.rosters?.map((item) => {
        if(item.Id){
          sendData.push({
            employeeId: item.Id,
            rosterId: item.RosterId,
            costCodeId: item.CostCodeId,
            positionId: item.PositionId,
            departmentId: item.Departmentid,
            EmployerId: item.EmployerId,
          })
        }
      })
      setRosterExecuteData(sendData)
      setDuration(values.durationMonth)
      setStartDate(values.startDate)
      setFlightGM(values.flightGroupMasterId)
      axios({
        method: 'post',
        url: 'tas/bulkroster/preview',
        data: {
          startDate: dayjs(values.startDate).format('YYYY-MM-DD'),
          durationMonth: values.durationMonth,
          employees: sendData
        },
      }).then((res) => {
        setPreviewData(res.data)
        setShowModal(true)
      }).catch((err) => {
  
      }).then(() => setLoading(false))
    }
  },[])

  const clearDatas = useCallback(() => {
    setFlightGM(null)
    setDuration(null)
    setRosterExecuteData([])
    handleChangeData([])
    changeIsEditing(false)
  },[])

  const handleProcess = () => {
    setProcessLoading(true)
    axios({
      method: 'post',
      url: 'tas/bulkroster',
      data: {
        startDate: startDate ? dayjs(startDate).format('YYYY-MM-DD') : null,
        durationMonth: duration,
        employees: rosterExecuteData,
        flightGroupMasterId: flightGM,
      },
    }).then((res) => {
      setResultData(res.data)
      if(res.data?.RosterExecutedEmployees?.length > 0 || res.data?.RosterSkippedEmployees?.length > 0){
        setShowResultModal(true)
      }
      // clearDatas()
      setShowModal(false)
    }).catch((err) => {

    }).then(() => setProcessLoading(false))
  }
  
  const handleBulkChangeField = (value, name) => {
    form.getFieldsValue()?.rosters?.map((field, index) => {
      form.setFieldValue(['rosters', index, name], value);
    })
    if(name === 'Departmentid'){
      let selectedDepartment = state.referData.departments?.find((department) => department.Id === value)
      if(selectedDepartment && selectedDepartment.CostCodeId){
        form.getFieldsValue()?.rosters?.map((field, index) => {
          form.setFieldValue(['rosters', index, 'CostCodeId'], selectedDepartment.CostCodeId);
        })
      }

    }
  }

  const handleBack = () => {
    handleChangeData(form.getFieldValue('rosters'))
    changeIsEditing(false)
  }

  const removeRow = (id) => {
    selectionTableRef.current.instance.deselectRows([id])
  }

  const handleDeleteAllOnSite = () => {
    setActionLoading(true)
    const onSiteEmployeeList = previewData.filter((item) => item.EmpOnSiteStatus)
    const ids = onSiteEmployeeList.map((item) => item.EmpId)
    const onsiteDate = form.getFieldValue('startDate').format('YYYY-MM-DD')
    axios({
      method: 'delete',
      url: `tas/employee/deletetransportbulk`,
      data: {
        employeeIds: ids,
        onsiteDate: onsiteDate,
      }
    }).then(() => {
      setPreviewData(previewData.filter((item) => !ids.includes(item.EmpId)))
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const disabledStatusOnSiteDeleteAllBtn = useMemo(() => {
    const onSiteEmployeeList = previewData.filter((item) => item.EmpOnSiteStatus)
    return onSiteEmployeeList.length === 0
  },[previewData])

  const isLoadingDeleteAllBtn = useMemo(() => {
    return (typeof actionLoading === 'boolean') && (actionLoading === true)
  },[actionLoading])

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
                      <Select 
                        style={{width: 180}}
                        options={state.referData?.rosters} placeholder="Roster" 
                        onChange={(e) => handleBulkChangeField(e, 'RosterId')}
                        allowClear
                        showSearch
                        filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                      />
                    </th>
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
                        onChange={(e) => handleBulkChangeField(e, 'Departmentid')}
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
                        onChange={(e) => handleBulkChangeField(e, 'PositionId')}
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
                        onChange={(e) => handleBulkChangeField(e, 'CostCodeId')}
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
                        onChange={(e) => handleBulkChangeField(e, 'EmployerId')}
                      />
                    </th>
                    <th className='border-b px-1'>Room Type</th>
                    <th className='border-b px-1'>Room Number</th>
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
                              <Link to={`/tas/people/search/${form.getFieldValue(['rosters', name, 'Id'])}`}>
                                <span className='text-blue-500 hover:underline text-xs'>{form.getFieldValue(['rosters', name, 'Fullname'])}</span>
                              </Link>
                            {/* <div className='w-[200px] text-[13px]'></div> */}
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t w-[200px]'>
                          <Form.Item
                            {...restField}
                            name={[name, 'RosterId']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Roster is required',
                              },
                            ]}
                          >
                            <Select 
                              style={{width: 180}}
                              options={state.referData?.rosters} placeholder="Roster" 
                              allowClear
                              showSearch
                              tokenSeparators={[',']}
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                            />
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item
                            {...restField}
                            name={[name, 'Departmentid']}
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
                            name={[name, 'PositionId']}
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
                          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.rosters[name]?.Departmentid !== cur.rosters[name]?.Departmentid}>
                            {({getFieldValue, setFieldValue}) => {
                              let selectedDepartment = state.referData.departments?.find((department) => department.Id === getFieldValue(['rosters', name, 'Departmentid']))
                              if(selectedDepartment && selectedDepartment.CostCodeId){
                                setFieldValue(['rosters', name, 'CostCodeId'], selectedDepartment.CostCodeId)
                              }
                              return (
                                <Form.Item
                                  {...restField}
                                  name={[name, 'CostCodeId']}
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
                            name={[name, 'EmployerId']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Employer is required',
                              },
                            ]}
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
                          <Form.Item className='mb-0'>
                            <div className='w-[180px] text-xs'>{form.getFieldValue(['rosters', name, 'RoomTypeName'])}</div>
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <Form.Item className='mb-0'>
                            <div className='w-[100px] text-xs'>{form.getFieldValue(['rosters', name, 'RoomNumber'])}</div>
                          </Form.Item>
                        </td>
                        <td className='p-1 border-t'>
                          <button
                            type='button'
                            className='dlt-button text-xs'
                            onClick={() => {removeRow(form.getFieldValue(['rosters', name, 'Id'])); remove(name);}}
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
        <div className='col-span-12 flex gap-5 mt-8'>
          <Form.Item name={'startDate'} label='Start Date' rules={[{required: true, message: 'Start date is required'}]}>
            <DatePicker showWeek size='middle'/>
          </Form.Item>
          <Form.Item name={'durationMonth'} label='Month Duration'>
            <InputNumber min={1} max={18} controls={false} size='middle'/>
          </Form.Item>
          <Form.Item name={'flightGroupMasterId'} label='Flight Group' rules={[{required: true, message: 'Flight Group is required'}]}>
            <Select 
              options={state.referData?.transportGroups}
              style={{width: 180}}
              allowClear
              showSearch
              size='middle'
              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            />
          </Form.Item>
          <Form.Item shouldUpdate={(prev, next) => prev.startDate !== next.startDate || prev.durationMonth !== next.durationMonth || prev.flightGroupMasterId !== next.flightGroupMasterId}>
            {
              ({getFieldValue}) => (
                <Form.Item>
                  <Button 
                    type='primary' 
                    disabled={
                      !getFieldValue('startDate') || 
                      !getFieldValue('durationMonth') || 
                      getFieldValue('rosters').length === 0 || 
                      !getFieldValue('flightGroupMasterId')}
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
      <Modal title='Changing' open={showModal} onCancel={()=>setShowModal(false)} width={900}>
        
        {/* <Changing data={previewData}
          onChangeData={(data) => setPreviewData(data)}
          onCancel={() => setShowModal(false)}
          handleProcess={handleProcess}
          handleRemoveFromModal={handleRemoveFromModal}
          form={form}
        /> */}
        <div className='mt-0 text-xs'>
          <div className='flex justify-end items-center mb-2'>
            <Button type='danger' onClick={handleDeleteAllOnSite} disabled={disabledStatusOnSiteDeleteAllBtn} loading={isLoadingDeleteAllBtn}>
              On Site Delete all
            </Button>
          </div>
          <Table
            data={previewData}
            columns={errorColumns}
            containerClass='shadow-none border overflow-hidden' 
            renderDetail={{enabled: true, component: WarningDetail}}
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