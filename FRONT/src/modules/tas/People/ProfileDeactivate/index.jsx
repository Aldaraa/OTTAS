import React, { useEffect, useRef, useState } from 'react'
import { Button, Modal, PeolpeSelectionTable, Tooltip } from 'components';
import axios from 'axios';
import { Form as AntForm, DatePicker, Input, Select } from 'antd';
import { useLocation } from 'react-router-dom';
import dayjs from 'dayjs';
import CellRenderDatePicker from './CellRenderDatePicker';
import SkippedEmployees from './SkippedEmployees';
import columns from './columns';
import { BsAirplaneFill } from 'react-icons/bs';
import { CheckCircleFilled, CloseCircleFilled } from '@ant-design/icons';

const disabledDate = (current) => {
  // Can not select days before today and today
  return current < dayjs().subtract(1, 'day').endOf('day') || current > dayjs().add(7, 'day').endOf('day');
};

function ProfileDeactivate() {
  const routeLocation = useLocation();
  const [ loading, setLoading ] = useState(false)
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ showActionModal, setShowActionModal ] = useState(false)
  const [ terminationType, setTerminationType ] = useState([])
  const [ skippedEmployees, setSkippedEmployees ] = useState([])
  const [ showSkippedData, setShowSkippedData ] = useState(false)
  const [ dateLoading, setDateLoading ] = useState(false)

  const [ searchForm ] = AntForm.useForm()
  const [ form ] = AntForm.useForm()
  const employees = AntForm.useWatch('rosters', form)
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }

    axios({
      method: 'get',
      url: 'tas/requestdemobilisationtype?active=1'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({label: `${item.Code} - ${item.Description}`, value: item.Id, ...item})
      })
      setTerminationType(tmp)
    }).catch((err) => {

    })
  },[])

  useEffect(() => {
    form.setFieldsValue({rosters: selectedData})
  },[selectedData])

  const handleSelect = (rows) => {
    setCurrentSelectedData(rows)
  }

  const handleReturn = () => {
    const newSelectedData = currentSelectedData.map((item) => ({
      ...item,
      Fullname: `${item.Firstname} ${item.Lastname}`,
    }))
    setSelectedData(newSelectedData)
    setShowActionModal(true)
  }

  const checkDates = (data) => {
    setDateLoading(true)
    axios({
      method: 'put',
      url: 'tas/employee/deactive/check/multiple',
      data: {data}
    }).then((res) => {
      res.data.forEach((item) => {
        form.setFieldValue(['rosters', item.Index, 'DateValidationStatus'], item.DateValidationStatus)
        form.setFieldValue(['rosters', item.Index, 'FutureTransportValidationStatus'], item.FutureTransportValidationStatus)
      })
    }).catch((err) => {

    }).finally(() => {
      setDateLoading(false)
    })
  }

  const handleBulkChange = (value, name) => {
    form.getFieldsValue()?.rosters?.map((field, index) => {
      form.setFieldValue(['rosters', index, name], value);
    })
  }
  const handleBulkDateChange = (value, name) => {
    let checkData = []
    form.getFieldsValue()?.rosters?.map((field, index) => {
      checkData.push({EmployeeId: field.Id, EventDate: value.format('YYYY-MM-DD'), Index: index})
      form.setFieldValue(['rosters', index, name], value);
    })
    checkDates(checkData)
  }

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/employee/deactive',
      data: {
        employees: values.rosters.map((item) => {
          return {
            employeeId: item.Id,
            eventDate: item.eventDate ? dayjs(item.eventDate).format('YYYY-MM-DD') : '',
            comment: item.comment,
            demobTypeTypeId: item.demobTypeTypeId,
          }
        })
      }
    }).then((res) => {
      if(res.data.length > 0){
        setSkippedEmployees(res.data)
        setShowSkippedData(true)
      }
      setShowActionModal(false)
      setSelectedData([])
      dataGrid.current?.instance.clearSelection()
      searchForm.submit()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCloseModal = () => {
    setShowActionModal(false)
    setSelectedData(form.getFieldValue('rosters'))
  }

  const handleRemoveRow = (id) => {
    dataGrid.current?.instance.deselectRows([id])
  }

  return (
    <div>
      <PeolpeSelectionTable
        ref={dataGrid}
        onSelect={handleSelect}
        onReturn={handleReturn}
        selectedRowsData={currentSelectedData}
        columns={columns}
        searchDefaultValues={{Active: 1}}
        hideFields={['Active']}
      />
      <Modal 
        open={showActionModal} 
        onCancel={handleCloseModal} 
        title={<div>Deactivate <span className='text-gray-400 font-normal'>({employees?.length} people selected)</span></div>} 
        width={1000}
      >
        <div className='flex justify-end gap-4 text-xs mb-1'>
          <div className='flex gap-2 items-center'>
            <BsAirplaneFill style={{color: '#ff4d4f'}}/> A future transport is scheduled
          </div>
          <div className='flex gap-2 items-center'>
            <BsAirplaneFill style={{color: '#52c41a'}}/> No Future transport
          </div>
          <div className='flex gap-2 items-center'>
            <CheckCircleFilled style={{color: '#52c41a'}}/> The Date is available
          </div>
          <div className='flex gap-2 items-center'>
            <CloseCircleFilled style={{color: '#ff4d4f'}}/> The Date is unavailable
          </div>
        </div>
        <AntForm 
          form={form}
          size='small'
          onFinish={handleSubmit}
        >
          <AntForm.List name='rosters'>
            {(fields, { remove }) => (
              <div className='border rounded-ot'>
              <table className='table-auto overflow-hidden w-full'>
                <thead className='text-[#959595]'> 
                  <tr className='text-left'>
                    <th className='border-b px-1 font-normal w-[30px]'>#</th>
                    <th className='border-b px-1 font-normal'>#Person</th>
                    <th className='border-b px-1 font-normal'>Fullname</th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ?
                        <Select 
                          placeholder='Change All' 
                          className='w-full' 
                          options={terminationType}
                          onChange={(value) => handleBulkChange(value, 'demobTypeTypeId')}
                          allowClear
                          showSearch
                          popupMatchSelectWidth={false}
                          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        />
                        :
                        'Demobilization Type'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <Input
                          placeholder='Change comments'
                          onChange={(event) => handleBulkChange(event.target.value, 'comment')}
                        />
                        : 
                        'Comment'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      <DatePicker placeholder='Event Date' disabledDate={disabledDate} onChange={(date) => handleBulkDateChange(date, 'eventDate')}/>
                    </th>
                    <th className='border-b px-1 font-normal'></th>
                  </tr>
                </thead>
                <tbody >
                  {
                    fields.map(({key, name, ...restField}) => (
                      <tr className={`${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'white'}`} key={name}>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} name={[name, 'Id']} className='mb-0'>
                            <div className='text-[13px] w-[30px]'>{name+1}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} name={[name, 'Id']} className='mb-0'>
                            <div className='text-[13px]'>{form.getFieldValue(['rosters', name, 'Id'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} className='mb-0'>
                            <div className='text-[13px]'>{form.getFieldValue(['rosters', name, 'Fullname'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item
                            {...restField}
                            name={[name, 'demobTypeTypeId']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Demobilization type is required',
                              },
                            ]}
                          >
                            <Select
                              options={terminationType}
                              placeholder='Demobilization Type'
                              allowClear
                              showSearch
                              popupMatchSelectWidth={false}
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                            />
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item
                            {...restField}
                            name={[name, 'comment']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Missing first name',
                              },
                            ]}
                          >
                            <Input placeholder='Comment'/>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item key={`eventDate-${key}`} name={[name, 'eventDate']} className='mb-0'>
                            <CellRenderDatePicker dateLoading={dateLoading} name={name} form={form} rowData={selectedData[name]}/>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <button
                            type='button'
                            className='dlt-button text-xs'
                            onClick={() => {
                              handleRemoveRow(form.getFieldValue(['rosters', name, 'Id']))
                              remove(name);
                            }}
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
          </AntForm.List>
        </AntForm>
        {
          selectedData.length > 0 &&
          <div className='flex items-center justify-end gap-5 mt-4'>
            <Button 
              type='primary' 
              onClick={() => form.submit()}
              loading={loading}
            >
              Process
            </Button>
            <Button 
              onClick={handleCloseModal}
              disabled={loading}
            >
              Cancel
            </Button>
          </div>
        }
      </Modal>
      <Modal open={showSkippedData} onCancel={() => setShowSkippedData(false)} title='Skipped Employees'>
        <SkippedEmployees data={skippedEmployees}/>
      </Modal>
    </div>
  )
}

export default ProfileDeactivate