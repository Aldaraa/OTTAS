import { SearchOutlined } from '@ant-design/icons'
import { Button, Form, Modal, Table } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useContext, useState } from 'react'
import { FaFemale, FaMale } from 'react-icons/fa'
import { Link, useActionData } from 'react-router-dom'

function ChangeRoomModal({open, onCancel, dates, profileData}) {
  const [ loading, setLoading ] = useState(false)
  const [ rLoading, setRLoading ] = useState(false)

  const [ form ] = Form.useForm()
  const { state } = useContext(AuthContext)

  const selectRoom = () => {

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
      label: 'On Site Employees',
      name: 'Employees',
      alignment: 'left',
    },
    {
      label: '',
      name: 'action',
      width: '90px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => selectRoom(e.data)}>Select</button>
        </div>
      )
    },
  ]

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
      // rules: [{required: true, message: 'Camp is required'}],
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
      name: 'bedCount',
      className: 'col-span-6 mb-2',
      type: 'number',
      inputprops: {
        min: 0
      }
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

  return (
    <Modal
      open={open} 
      onCancel={onCancel} 
      title={`Change Room /${dates?.StartDate} - ${dates?.EndDate}/`}
      width={900}
    >
      <div>
        <div className='p-4 border rounded-ot mb-5'>
          <div className='flex justify-between'>
            <div className='flex flex-col'>
              <div className='flex gap-2'>
                <div>Fullname:</div>
                <Link to={`/tas/people/search/${profileData?.Id}`} >
                  <span className='cursor-pointer text-blue-500 hover:underline flex gap-2 items-center'>
                    {profileData?.Firstname} {profileData?.Lastname}
                  </span>
                </Link>
              </div>
              <div className='flex gap-2'>
                <div>Department:</div>
                <div>{profileData?.DepartmentName}</div>
              </div>
              <div className='flex gap-2'>
                <div>Employer:</div>
                <div>{profileData?.EmployerName}</div>
              </div>
              <div className='flex gap-2'>
                <div>Resource Type:</div>
                <div>{profileData?.PeopleTypeCode}</div>
              </div>
              <div className='flex gap-2'>
                <div>Position:</div>
                <div>{profileData?.PositionName}</div>
              </div>
              <div className='flex gap-2'>
                <div>Own Room:</div>
                <div>{profileData?.RoomNumber}</div>
              </div>
            </div>
            <div>
              {
                profileData?.RoomOwner ? 
                <div><i className="dx-icon-home text-green-500"></i> Owner</div>
                :
                <div><i className="dx-icon-home text-gray-400"></i> Not Owner</div>
              } 
              {
                profileData?.Gender === 1 ? 
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
          <div className='mt-5'>
            <Table 
              containerClass='shadow-none border'
              columns={empRoomHistoryCols}
              data={profileData?.employeeRoomProfileRoomHistories}
              allowColumnReordering={false}
              loading={loading}
              keyExpr='RoomId'
              pager={profileData?.employeeRoomProfileRoomHistories?.length > 20}
              focusedRowEnabled={false}
              showRowLines={true}
              title={<div className='border-b text-sm font-bold py-2'>Stay History</div>}
            />
          </div>
        </div>
        <Form 
          form={form}
          fields={roomSearchFields}
          className={'border rounded-ot p-4 gap-x-8 mb-5'}
          onFinish={findAvailableRoom}
          initValues={{Private: null}}
        >
          <div className='col-span-12 flex justify-end'>
            <Button htmlType='submit' icon={<SearchOutlined/>} disabled={!searchable} loading={rLoading}>Search</Button>
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
          scrolling={true}
          pager={availableRooms.length > 20}
          renderDetail={{
            enabled: true, 
            component: (data) => {
              return (
                <SearchRoomResultDetail 
                  propData={data} 
                  startDate={dates?.StartDate} 
                  endDate={dates?.EndDate}
                />
              )
            }
          }}
          // onRowDblClick={(e) => navigate(`/tas/room/${e.data.Id}`)}
        />
      </div>
    </Modal>
  )
}

export default ChangeRoomModal