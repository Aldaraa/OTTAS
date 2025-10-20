import { Modal, Table } from 'components'
import dayjs from 'dayjs'
import { CheckBox } from 'devextreme-react'
import React from 'react'

function ExistingBookingModal({existingBookings, ...restProps}) {

  const nonSiteTravelCols = [
    {
      label: 'Travel Date',
      name: 'TravelDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd ')}</div>
      )
    },
    {
      label: 'Favor Time',
      name: 'FavorTime',
    },
    {
      label: 'Depart Location',
      name: 'DepartLocationName',
      alignment: 'left',
    },
    {
      label: 'Arrive Location',
      name: 'ArriveLocationName',
      alignment: 'left',
    },
    {
      label: 'Comment',
      name: 'Comment',
    },
    {
      label: 'ETD',
      name: 'ETD',
      alignment: 'left',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
  ]
  
  const siteTravelCols = [
    {
      label: '',
      name: 'Direction',
      cellRender: (e) => (
        <div>1</div>
      )
    },
    {
      label: 'Date',
      name: 'RequestedDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm  ')}</div>
      )
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Status',
      name: 'Status',
    },
  ]
  
  const pendingRequestCols = [
    {
      label: 'Type',
      name: 'DocumentType',
    },
    {
      label: 'Date',
      name: 'RequestedDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
      )
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: '#',
      name: 'Id',
      alignment: 'left'
    },
    {
      label: 'Status',
      name: 'CurrentStatus',
    },
  ]
  
  const otherRequestCols = [
    {
      label: 'Type',
      name: 'Type',
    },
    {
      label: 'Date',
      name: 'Date',
    },
    {
      label: '#',
      name: '#',
    },
    {
      label: 'Status',
      name: 'Status',
    },
  ]

  return (
    <Modal
      title='Existing Bookings'
      width={800}
      {...restProps}
    >
      <div className='flex flex-col gap-5'>
        <div></div>
        <Table
          data={existingBookings?.NonSiteTravel}
          columns={nonSiteTravelCols}
          pager={false}
          containerClass='shadow-none border border-gray-300'
          title={<div className='border-b py-1 font-bold'>Non Site Travel Requests</div>}
        />
        <Table
          data={existingBookings?.NonSiteTravel}
          columns={siteTravelCols}
          pager={false}
          containerClass='shadow-none border border-gray-300'
          title={<div className='border-b py-1 font-bold'>Site Travel Requests</div>}
        />
        <Table
          data={existingBookings?.PendingRequest}
          columns={pendingRequestCols}
          pager={false}
          containerClass='shadow-none border border-gray-300'
          title={<div className='border-b py-1 font-bold'>Pending Requests</div>}
        />
        <Table
          data={existingBookings?.OtherRequest}
          columns={otherRequestCols}
          pager={false}
          containerClass='shadow-none border border-gray-300'
          title={<div className='border-b py-1 font-bold'>Other Requests</div>}
        />
      </div>
    </Modal>
  )
}

export default ExistingBookingModal