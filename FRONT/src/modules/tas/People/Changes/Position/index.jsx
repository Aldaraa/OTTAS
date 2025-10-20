import React, { useContext, useEffect, useRef, useState } from 'react'
import { Button, Modal, PeolpeSelectionTable } from 'components';
import axios from 'axios';
import { Form as AntForm, Checkbox, DatePicker, Select } from 'antd';
import { useLocation } from 'react-router-dom';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import columns from './columns';

function ChangesPosition() {
  const routeLocation = useLocation();
  const [ loading, setLoading ] = useState(false)
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ showActionModal, setShowActionModal ] = useState(false)

  const { state } = useContext(AuthContext)
  const [ searchForm ] = AntForm.useForm()
  const [ form ] = AntForm.useForm()
  const dataGrid = useRef(null)

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
    const selectedData = currentSelectedData.map((item) => ({
      Fullname: `${item.Firstname} ${item.Lastname}`,
      Id: item.Id,
      PositionName: item.PositionName,
      PositionId: item.PositionId,
    }))
    setSelectedData(selectedData)
    setShowActionModal(true)
  }

  const handleBulkChange = (value, name) => {
    form.getFieldsValue()?.rosters?.forEach((field, index) => {
      form.setFieldValue(['rosters', index, name], value);
    })
  }

  const handleSubmit = (values) => {
    setLoading(true)
    let tmp = values.rosters.map((item) => {
      return {
        DataId: item.PositionId,
        StartDate: item.StartDate ? dayjs(item.StartDate).format('YYYY-MM-DD') : '',
        EndDate: item.profileUpdate ? null : dayjs(item.EndDate).format('YYYY-MM-DD'),
        EmployeeId: item.Id,
        profileUpdate: item.profileUpdate ? 1 : 0
      }
    })
    axios({
      method: 'put',
      url: `tas/employee/changedata/position`,
      data: tmp,
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
    setCurrentSelectedData(form.getFieldValue('rosters'))
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
        title={<div>Change Position <span className='text-gray-400 font-normal'>({selectedData.length} people selected)</span></div>} 
        width={1000}
        forceRender={true}
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
                      Current Position
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <Select
                          className='w-full'
                          options={state.referData?.positions}
                          placeholder='Position'
                          onChange={(value) => handleBulkChange(value, 'PositionId')}
                          allowClear
                          showSearch
                          popupMatchSelectWidth={false}
                          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                        />
                        : 
                        'Replacement Position'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <DatePicker onChange={(date) => handleBulkChange(date, 'StartDate')} placeholder='Start Date'/>
                        : 
                        'Start Date'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <DatePicker onChange={(date) => handleBulkChange(date, 'EndDate')} placeholder='End Date'/>
                        : 
                        'End Date'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'>
                      {
                        fields.length > 1 ? 
                        <Checkbox onChange={(e) => handleBulkChange(e.target.checked, 'profileUpdate')}>Permanent</Checkbox>
                        : 
                        'Permanent'
                      }
                    </th>
                    <th className='border-b px-1 font-normal'></th>
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
                            className='mb-0'
                          >
                            <div className='text-[13px]'>{form.getFieldValue(['rosters', name, 'PositionName'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item
                            {...restField}
                            name={[name, 'PositionId']}
                            className='mb-0'
                            rules={[
                              {
                                required: true,
                                message: 'Missing position',
                              },
                            ]}
                          >
                            <Select
                              options={state.referData?.positions}
                              placeholder='Position'
                              allowClear
                              showSearch
                              popupMatchSelectWidth={false}
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                            />
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item key={`startDate-${key}`} name={[name, 'StartDate']} className='mb-0'>
                            <DatePicker placeholder='Start Date'/>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item key={`endDate-${key}`} name={[name, 'EndDate']} className='mb-0'>
                            <DatePicker  placeholder='End Date'/>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1 text-center'>
                          <AntForm.Item 
                            key={`profileUpdate-${key}`} 
                            name={[name, 'profileUpdate']} 
                            className='mb-0' 
                            valuePropName="checked"
                          >
                            <Checkbox/>
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
    </div>
  )
}

export default ChangesPosition