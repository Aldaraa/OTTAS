import React, { useContext, useEffect, useRef, useState } from 'react'
import { Table, Button, Modal, SearchPeopleForm } from 'components';
import { CheckBox } from 'devextreme-react';
import { BsAirplaneFill } from 'react-icons/bs'
import { GoPrimitiveDot } from 'react-icons/go'
import axios from 'axios';
import { Form as AntForm, DatePicker, notification } from 'antd';
import { useLocation } from 'react-router-dom';
import { AuthContext } from 'contexts';
import CustomStore from 'devextreme/data/custom_store';
import dayjs from 'dayjs';
import isArray from 'lodash/isArray';

function RoomAudit() {
  const routeLocation = useLocation();
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ showActionModal, setShowActionModal ] = useState(false)
  const [ store ] = useState(new CustomStore({
    key: 'Id',
    load: (loadOptions) => {
      let params = '';
      if(loadOptions.sort?.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      if(loadOptions.filter && !isArray(loadOptions.filter)){
        dataGrid.current?.instance.beginCustomLoading();
        return axios({
          method: 'post',
          url: `tas/employee/search${params}`,
          data: {
            model: {
              ...loadOptions.filter,
              Active: 1,
            },
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          setData(res.data.data)
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => dataGrid.current?.instance.endCustomLoading())
      }
      else if(!isArray(loadOptions.filter) && typeof loadOptions.filter === 'undefined'){
        dataGrid.current?.instance.beginCustomLoading();
        return axios({
          method: 'post',
          url: `tas/employee/search${params}`,
          data: {
            model: {
              CampId: '',
              DepartmentId: '',
              Firstname: "",
              FlightGroupMasterId: "",
              Lastname: "",
              LocationId: "",
              NRN: "",
              roomNumber: "",
              RoomTypeId: "",
              costCodeId: "",
              employerId: "",
              futureBookingId: "",
              group: "",
              id: "",
              peopleTypeId: "",
              roomAssignment: "",
              rosterId: "",
              sapId: "",
              mobile: "",
              hasRoom: "",
              futureBooking: "",
              Active: 1, 
            },
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          setData(res.data.data)
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => dataGrid.current?.instance.endCustomLoading())
      }
    }
  }))

  const { state } = useContext(AuthContext)
  const [ api, contextHolder] = notification.useNotification();
  const [ searchForm ] = AntForm.useForm()
  const [ form ] = AntForm.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
  },[])

  useEffect(() => {
    let tmp = []
    const rosters = form.getFieldsValue().employeeIds
    selectedData?.map((item) => {
      if(rosters){
        const isField = rosters?.find((row) => row.Id === item.Id)
        if(isField){
          tmp.push(isField)
        }
        else{
          tmp.push(item)
        }
      }else{
        tmp.push(item)
      }
    })
    form.setFieldsValue({employeeIds: tmp})
  },[selectedData])

  const columns = [
    {
      label: 'Person #',
      name: 'Id',
      dataType: 'string',
      width: '70px',
    },
    {
      label: '',
      name: 'Active',
      width: '21px',
      alignment: 'center',
      showInColumnChooser: false,
      allowSorting: false,
      cellRender:(e) => (
        <GoPrimitiveDot color={e.value === 1 ? 'lime' : 'lightgray'}/>
      )
    },
    {
      label: 'Firstname',
      name: 'Firstname',
      dataType: 'string',
    },
    {
      label: 'Lastname',
      name: 'Lastname',
      dataType: 'string',
    },
    {
      label: 'Department',
      name: 'DepartmentName',
      dataType: 'string',
      width: '220px',
    },
    {
      label: 'Employer',
      name: 'EmployerName',
      dataType: 'string',
    },
    {
      label: 'Roster',
      name: 'RosterName',
      dataType: 'string',
      width: '70px',
      enableFilter: true,
    },
    {
      label: 'Resource Type',
      name: 'PeopleTypeName',
      dataType: 'string',
    },
    {
      label: 'Room',
      name: 'RoomNumber',
      cellRender: (e) => (
        <span>{e.value}</span>
      )
    },
    {
      label: 'Gender',
      name: 'Gender',
      alignment: 'center',
      width:'70px',
      cellRender: (e) => (
        <span>{e.data.Gender === 1 ? 'M' : 'F'}</span>
      ),
      groupCellRender: (e) => (
        <span>{e.value === 1 ? 'M' : 'F'} {e.groupContinuesMessage ? `- ${e.groupContinuesMessage}` : ''}</span>
      )
    },
    {
      label: '',
      name: 'HasFutureTransport',
      width: '50px',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
      headerCellRender: (he, re) => (
        <BsAirplaneFill></BsAirplaneFill>
      )
    },
    {
      label: '',
      name: 'HasFutureRoomBooking',
      width: '50px',
      cellRender: (e, r) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
      headerCellRender: (he, re) => (
        <i className="dx-icon-home"></i>
      )
    },
  ]

  const handleSearch = (values) => {
    dataGrid.current?.instance.filter(values)
  }

  const handleSelect = (e) => {
    setSelectedRowKeys(e.selectedRowKeys)
    setCurrentSelectedData(e.selectedRowsData.map((item) => ({...item, Fullname: `${item.Firstname} ${item.Lastname}`})))
  }
  
  const handleAddSelection = () => {
    setSelectedData(currentSelectedData)
  }

  const handleReturn = () => {
    if(selectedData?.length !== 50){
      handleAddSelection()
    }
    setShowActionModal(true)
  }

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/audit/roomaudit',
      responseType: 'blob',
      data: {
        ...values,
        startDate: dayjs(values.startDate).format('YYYY-MM-DD'),
        endDate: dayjs(values.endDate).format('YYYY-MM-DD'),
        employeeIds: values.employeeIds.map((item) => item.Id),
      }
    }).then((res) => {
      if(res.status === 200){
        const url = window.URL.createObjectURL(res.data); 
        const a = document.createElement('a');
        a.href = url;
        a.download = `TAS_ROOM_AUDIT_${dayjs().format('YYYY-MM-dd_HH-mm-ss')}.xlsx`
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        setShowActionModal(false)
        setSelectedData([])
        setSelectedRowKeys([])
      }else{
        api.error({
          message: 'Audit data not found',
          duration: 5,
          // description: ''
        });
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCloseModal = () => {
    setShowActionModal(false)
    setSelectedRowKeys(form.getFieldValue('employeeIds')?.map((item) => (item.Id)))
    setSelectedData(form.getFieldValue('employeeIds'))
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3'>
        <SearchPeopleForm
          containerClass='bg-white rounded-ot py-2 mb-3'
          onSearch={handleSearch}
          hideFields={['Active']}
        />
      </div>
      <Table
        ref={dataGrid}
        data={store}
        columns={columns}
        allowColumnReordering={false}
        remoteOperations={true}
        id="room"
        showRowLines={true}
        selection={{mode: 'multiple', recursive: false, showCheckBoxesMode: 'always', allowSelectAll: true, selectAllMode: 'page'}}
        rowAlternationEnabled={false}
        selectedRowKeys={selectedRowKeys}
        onSelectionChanged={handleSelect}
        tableClass='max-h-[calc(100vh-305px)]'
        title={!state.userInfo?.ReadonlyAccess ? <div className='flex justify-between items-center py-2 gap-3 border-b'>
          <div className='ml-2'><span className='font-bold'>{selectedData?.length}</span> people selected {selectedData?.length === 50 && <span className='text-red-400'>(full)</span>}</div>
          <div className='flex items-center gap-3'>
            <Button
              onClick={handleAddSelection} 
              disabled={selectedData?.length === 50 || selectedRowKeys?.length === 0}
            >
              Add Selection
            </Button>
            <Button 
              onClick={handleReturn} 
              disabled={selectedData?.length === 0 && selectedRowKeys?.length === 0}
            >
              Add Selection & Return
            </Button>
          </div>
        </div> : ''}
      />
      <Modal 
        open={showActionModal} 
        onCancel={handleCloseModal} 
        title={<div>Room Audit <span className='text-gray-400 font-normal'>({selectedData?.length} people selected)</span></div>} 
        width={1000}
        forceRender={true}
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
                          <button type='button' className='dlt-button text-xs' onClick={() => remove(name)}>Remove</button>
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
            <AntForm.Item label='StartDate' key='form-startdate' className='mb-0' name='startDate' rules={[{required: true}]}>
              <DatePicker/>
            </AntForm.Item>
            <AntForm.Item label='endDate' key='form-enddate' className='mb-0' name='endDate' rules={[{required: true}]}>
              <DatePicker/>
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
              Download
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
      {contextHolder}
    </div>
  )
}

export default RoomAudit