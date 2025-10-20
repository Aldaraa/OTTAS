import { SearchOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, ExpandRowRoomDetail, Form, Table } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import SearchRoomResultDetail from 'modules/tas/Accommodation/RoomCalendarDetail/SearchRoomResultDetail'
import React, { useContext, useEffect, useState } from 'react'
import { FaFemale, FaMale } from 'react-icons/fa'
import { Link, useNavigate } from 'react-router-dom'
import { Form as AntForm } from 'antd'
import { CheckBox } from 'devextreme-react'

function ChangeRoom({form, closeModal, startDate, endDate, handleSelect}) {
  const [ empRoomData, setEmpRoomData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ rLoading, setRLoading ] = useState(false)
  const [ availableRooms, setAvailableRooms ] = useState([])

  const { state } = useContext(AuthContext)
  const [ searchForm ] = AntForm.useForm()
  const navigate = useNavigate()
  
  useEffect(() => {
    if(state.userProfileData){
      getEmployeeRoomDetail()
    }
  },[state.userProfileData])

  const getEmployeeRoomDetail = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/room/employeeprofile/${state.userProfileData?.Id}`
    }).then((res) => {
      setEmpRoomData(res.data)
    }).finally(() => setLoading(false))
  }

  const empRoomHistoryCols = [
    {
      label: 'Room Number',
      name: 'RoomNumber',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
    {
      label: 'Camp',
      name: 'Camp',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'End Date',
      name: 'EndDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
  ]

  const roomSearchFields = [
    {
      label: 'Camp',
      name: 'CampId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.camps,
      }
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
      className: 'col-span-6 mb-2'
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      depindex: 'CampId',
      // rules: [{required: true, message: 'Room Type is required'}],
      inputprops: {
        optionsurl: 'tas/roomtype?active=1&campId=',
        loading: state.customLoading,
        optionvalue: 'Id', 
        optionlabel: 'Description',
      }
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      className: 'col-span-6 mb-2',
      type: 'number',
    },
    {
      label: 'Private',
      name: 'Private',
      className: 'col-span-6 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

  const handleSearch = (values) => {
    setRLoading(true)
    axios({
      method: 'post',
      url: 'tas/room/findavailablebydates',
      data: {
        ...values,
        Private: values.Private,
        StartDate: dayjs(startDate).format('YYYY-MM-DD'),
        EndDate: dayjs(endDate).format('YYYY-MM-DD'),
      }
    }).then((res) => {
      setAvailableRooms(res.data)
    }).catch((err) => {

    }).then(() => setRLoading(false))
  }

  const handleSelectRoom = (row) => {
    if(form) form.setFieldValue('Room', row)
    if(handleSelect) handleSelect(row)
    if(closeModal) closeModal()
  }

  const roomColumns = [
    {
      label: 'Room Number',
      name: 'roomNumber',
      width: '150px'
    },
    {
      label: 'Bed #',
      name: 'BedCount',
      alignment: 'left',
    },
    {
      label: 'Owners',
      name: 'RoomOwners',
      alignment: 'left',
    },
    {
      label: 'Employees',
      name: 'Employees',
      alignment: 'left',
    },
    {
      label: 'Owner In Date',
      name: 'OwnerInDate',
      alignment: 'center',
      cellRender:({value}) => (
        <div>{value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '-'}</div>
      )
    },
    // {
    //   label: 'No Room',
    //   name: 'VirtualRoom',
    //   width: '100px',
    //   alignment: 'center',
    //   cellRender: (e) => (
    //     <CheckBox disabled iconSize={18} value={e.data.VirtualRoom === 1 ? true : 0}/>
    //   )
    // },
    {
      label: '',
      name: 'action',
      width: '90px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleSelectRoom(e.data)}>Select</button>
        </div>
      )
    },
  ]

  return (
    <div>
      <div className='p-4 border rounded-ot mb-5'>
        <div className='flex justify-between'>
          <div className='flex flex-wrap gap-x-8 gap-y-2'>
            <div>
              <label className='text-xs text-gray-500'>Fullname:</label>
              <Link to={`/tas/people/search/${state.userProfileData?.Id}`}>
                <span className='cursor-pointer text-blue-500 hover:underline flex gap-2 items-center'>{state.userProfileData?.Firstname} {state.userProfileData?.Lastname}</span>
              </Link>
            </div>
            <div>
              <label className='text-xs text-gray-500'>Department:</label>
              <div>{state.userProfileData?.DepartmentName || '-'}</div>
            </div>
            <div>
              <label className='text-xs text-gray-500'>Employer:</label>
              <div>{state.userProfileData?.EmployerName || '-'}</div>
            </div>
            <div>
              <label className='text-xs text-gray-500'>Resource Type:</label>
              <div>{state.userProfileData?.PeopleTypeCode || '-'}</div>
            </div>
            <div>
              <label className='text-xs text-gray-500'>Own Room:</label>
              <div>{state.userProfileData?.RoomNumber || '-'}</div>
            </div>
            <div>
              <label className='text-xs text-gray-500'>Gender:</label>
              <div>
                {
                  state.userProfileData?.Gender === 1 ? 
                  <div className='flex items-center gap-1'>
                    <FaMale className='text-blue-600'/> 
                    Male
                  </div>
                  : 
                  <div className='flex items-center gap-1'>
                    <FaFemale className='text-pink-500'/>
                    Female
                  </div>
                } 
              </div>
            </div>
          </div>
        </div>
        {
          empRoomData?.employeeRoomProfileRoomHistories?.length > 0 ?
          <div className='mt-5'>
            <Table
              containerClass='shadow-none border'
              columns={empRoomHistoryCols}
              data={empRoomData?.employeeRoomProfileRoomHistories}
              allowColumnReordering={false}
              loading={loading}
              keyExpr='RoomId'
              pager={empRoomData?.employeeRoomProfileRoomHistories?.length > 20}
              focusedRowEnabled={false}
              showRowLines={true}
              title={<div className='border-b text-sm font-bold py-2'>Stay History</div>}
            />
          </div>
          : null
        }
      </div>
      <Form
        form={searchForm}
        fields={roomSearchFields} 
        initValues={{Private: null}}
        className={'border rounded-ot p-4 gap-x-8 mb-5'} 
        onFinish={handleSearch}
      >
        <div className='col-span-12 flex justify-end'>
          <Button htmlType='button' icon={<SearchOutlined/>} loading={rLoading} onClick={() => searchForm.submit()}>Search</Button>
        </div>
      </Form>
      <Table 
        containerClass='shadow-none border'
        columns={roomColumns}
        data={availableRooms}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='RoomId'
        focusedRowEnabled={false}
        showRowLines={true}
        pager={true}
        tableClass='max-h-[calc(100vh-400px)]'
        renderDetail={{
          enabled: true, 
          component: (data) => (
            <ExpandRowRoomDetail 
              propData={data}
              startDate={startDate}
              endDate={endDate}
            />
          )
        }}
        onRowDblClick={(e) => navigate(`/tas/room/${e.data.Id}`)}
      />
    </div>
  )
}

export default ChangeRoom