import React, { useContext, useEffect, useRef, useState } from 'react'
import { Button, Modal, PeolpeSelectionTable } from 'components';
import axios from 'axios';
import { Form as AntForm, DatePicker, Select, TreeSelect } from 'antd';
import { useLocation } from 'react-router-dom';
import dayjs from 'dayjs';
import columns from './columns';
import { AuthContext } from 'contexts';

function ProfileReactivate() {
  const routeLocation = useLocation();
  const [ loading, setLoading ] = useState(false)
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ showActionModal, setShowActionModal ] = useState(false)

  const [ searchForm ] = AntForm.useForm()
  const [ form ] = AntForm.useForm()
  const employees = AntForm.useWatch('rosters', form)
  const dataGrid = useRef(null)
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
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

  const handleBulkChange = (value, name) => {
    form.getFieldsValue()?.rosters?.map((field, index) => {
      form.setFieldValue(['rosters', index, name], value);
    })
  }

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/employee/reactive',
      data: {
        employees: values.rosters.map((item) => {
          return {
            employeeId: item.Id,
            eventDate: item.eventDate ? dayjs(item.eventDate).format('YYYY-MM-DD') : '',
            comment: item.comment,
            terminationTypeId: item.terminationTypeId,
            CostCodeId: item.CostCodeId,
            DepartmentId: item.DepartmentId,
            EmployerId: item.EmployerId,
          }
        })
      }
    }).then((res) => {
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
        searchDefaultValues={{Active: 0}}
        hideFields={['Active']}
      />
      <Modal 
        open={showActionModal} 
        onCancel={handleCloseModal} 
        title={<div>Reactivate <span className='text-gray-400 font-normal'>({employees?.length} people selected)</span></div>} 
        width={1100}
      >
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
                        <TreeSelect
                          style={{width: 200}}
                          treeData={state.referData?.departments}
                          virtual={true}
                          treeLine
                          showSearch
                          fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
                          filterTreeNode={(input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase())}
                          placeholder="Department" 
                          onChange={(value) => handleBulkChange(value, 'DepartmentId')}
                          popupMatchSelectWidth={false}
                        />
                        : 
                        'Replacement Department'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <Select
                          className='w-[150px]'
                          options={state.referData?.employers}
                          placeholder='EmployerId'
                          onChange={(value) => handleBulkChange(value, 'EmployerId')}
                          allowClear
                          showSearch
                          popupMatchSelectWidth={false}
                          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        />
                        : 
                        'Replacement Employer'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <Select
                          className='w-full'
                          options={state.referData?.costCodes}
                          placeholder='Cost Code'
                          onChange={(value) => handleBulkChange(value, 'CostCodeId')}
                          allowClear
                          showSearch
                          popupMatchSelectWidth={false}
                          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        />
                        : 
                        'Replacement Cost Code'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <DatePicker
                          placeholder='Change Event Dates'
                          className='w-[120px]'
                          onChange={(date) => handleBulkChange(date, 'eventDate')}
                        />
                        : 
                        'Event Date'
                      }
                    </th>
                    <th className='border-b px-1 font-normal text-right'></th>
                  </tr>
                </thead>
                <tbody >
                  {
                    fields.map(({key, name, ...restField}) => (
                      <tr className={`${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'white'}`}>
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
                            name={[name, 'DepartmentId']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Missing department',
                              },
                            ]}
                          >
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
                            />
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item
                            {...restField}
                            name={[name, 'EmployerId']}
                            className='mb-0 w-[150px]'
                            rules={[
                              {
                                required: true,
                                message: 'Missing employer',
                              },
                            ]}
                          >
                            <Select
                              options={state.referData?.employers}
                              placeholder='Employer'
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
                            name={[name, 'CostCodeId']}
                            className='mb-0 w-[200px]'
                            rules={[
                              {
                                required: true,
                                message: 'Missing cost code',
                              },
                            ]}
                          >
                            <Select
                              options={state.referData?.costCodes}
                              placeholder='Cost Code'
                              allowClear
                              showSearch
                              popupMatchSelectWidth={false}
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                            />
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item key={`eventDate-${key}`} name={[name, 'eventDate']} className='mb-0'>
                            <DatePicker className='w-[120px]'/>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1 text-right'>
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
    </div>
  )
}

export default ProfileReactivate