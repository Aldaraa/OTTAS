import React, { useContext, useEffect, useRef, useState } from 'react'
import { Table, Button, Modal, PeolpeSelectionTable } from 'components';
import axios from 'axios';
import { Form as AntForm, DatePicker, Select, Tag } from 'antd';
import { Link, useLocation } from 'react-router-dom';
import { AuthContext } from 'contexts';
import columns from './columns';
import dayjs from 'dayjs';

function BulkShiftStatus() {
  const routeLocation = useLocation();
  const [ loading, setLoading ] = useState(false)
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ showActionModal, setShowActionModal ] = useState(false)
  const [ resultData, setResultData ] = useState([])
  const [ showResult, setShowResult ] = useState(false)

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
    form.setFieldsValue({employeeIds: selectedData})
  },[selectedData])

  const handleSelect = (rows) => {
    setCurrentSelectedData(rows)
  }

  const handleReturn = () => {
    const selectedData = currentSelectedData.map((item) => ({
      ...item,
      Fullname: `${item.Firstname} ${item.Lastname}`,
    }))
    setSelectedData(selectedData)
    setShowActionModal(true)
  }

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/employeestatus/visualstatusdatechangebulk',
      data: {
        ...values,
        startDate: dayjs(values.startDate).format('YYYY-MM-DD'),
        endDate: dayjs(values.endDate).format('YYYY-MM-DD'),
        employeeIds: values.employeeIds.map((item) => item.Id),
      }
    }).then((res) => {
      setShowActionModal(false)
      setSelectedData([])
      searchForm.submit()
      dataGrid.current?.instance.clearSelection()
      if(res.data?.length > 0){
        setResultData(res.data)
        setShowResult(true)
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCloseModal = () => {
    setShowActionModal(false)
    setSelectedData(form.getFieldValue('employeeIds'))
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
        title={<div>Change Shift Status <span className='text-gray-400 font-normal'>({selectedData?.length} people selected)</span></div>} 
        width={1000}
      >
        <AntForm 
          form={form}
          size='small'
          onFinish={handleSubmit}
        >
          <AntForm.List name='employeeIds'>
            {(fields, { remove }) => (
              <div className='border rounded-ot'>
              <table className='table-auto overflow-hidden w-full'>
                <thead className='text-[#959595]'> 
                  <tr className='text-left'>
                    <th className='border-b px-1 font-normal w-[30px]'>#</th>
                    <th className='border-b px-1 font-normal'>#Person</th>
                    <th className='border-b px-1 font-normal'>Fullname</th>
                    <th className='border-b px-1 font-normal'>Department</th>
                    <th className='border-b px-1 font-normal'></th>
                  </tr>
                </thead>
                <tbody className='max-h-[400px]'>
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
                            <div className='text-[13px]'>{form.getFieldValue(['employeeIds', name, 'Id'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} className='mb-0'>
                            <div className='text-[13px]'>{form.getFieldValue(['employeeIds', name, 'Fullname'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} className='mb-0'>
                            <div>{form.getFieldValue(['employeeIds', name, 'DepartmentName'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                        <button
                            type='button'
                            className='dlt-button text-xs'
                            onClick={() => {
                              handleRemoveRow(form.getFieldValue(['employeeIds', name, 'Id']))
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
          <div className='flex gap-4 mt-4'>
            <AntForm.Item label='Shift Status' key='form-shiftstatus' className='mb-0' name='shiftId' rules={[{required: true, message: 'Shift status is required'}]}>
              <Select
                showSearch
                options={state?.referData?.roomStatuses}
                style={{width: 160}}
                popupMatchSelectWidth={false}
                allowClear
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </AntForm.Item>
            <AntForm.Item
              label='Start Date'
              key='form-startdate'
              className='mb-0'
              name='startDate'
              rules={[{required: true, message: 'Start date is required'}]}
            >
              <DatePicker/>
            </AntForm.Item>
            <AntForm.Item noStyle shouldUpdate={(prev, cur) => { return prev.startDate !== cur.startDate}}>
              {({getFieldValue}) => {
                let defaultPickerValue = false
                if(!getFieldValue('endDate')){
                  defaultPickerValue = getFieldValue('startDate')
                }
                return(
                  <AntForm.Item
                    label='End Date'
                    key='form-enddate'
                    className='mb-0'
                    name='endDate'
                    rules={[{required: true, message: 'End date is required'}]}

                  >
                    <DatePicker defaultPickerValue={defaultPickerValue}/>
                  </AntForm.Item>
                )
              }}

            </AntForm.Item>
          </div>
        </AntForm>
        {
          selectedData?.length > 0 &&
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
      <Modal title='Shift updated employees' width={800} open={showResult} onCancel={() => setShowResult(false)}>
        <Table
          containerClass='shadow-none -mx-2'
          columns={[
            {
              label: 'Person #',
              name: 'Id',
              dataType: 'string',
              width: '70px',
            },
            {
              label: 'Fullname',
              name: 'FullName',
              dataType: 'string',
              cellRender:(e) => (<Link to={`/tas/people/search/${e.data.Id}`}><span className='text-blue-400 hover:underline'>{e.value}</span></Link>)
            },
            {
              label: 'Reason',
              name: 'SkippedReason',
              dataType: 'string',
              width: 100,
              cellRender: ({value}) => (
                <div>{value?.Name}</div>
              )
            },
            {
              label: 'Date',
              name: 'SkippedReason',
              dataType: 'string',
              cellRender: ({value}) => (
                <div>
                  {value?.Days?.map((date, i) => <Tag color={value?.Name === 'On site days' ? 'processing' : ''} key={i} className='my-[2px]'>{date}</Tag>)}
                </div>
              )
            },
          ]}
          data={resultData}
        />
      </Modal>
    </div>
  )
}

export default BulkShiftStatus