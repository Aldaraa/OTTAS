import { Tag } from 'antd'
import { Table } from 'components'
import dayjs from 'dayjs'
import React from 'react'
import { Link } from 'react-router-dom'

function BookingDetail({data}) {

  const memberColumns = [
    {
      label: 'Fullname',
      name: 'FullName',
      alignment: 'left',
      cellRender: (e) => (
        <Link to={`/tas/people/search/${e.data.EmployeeId}`}>
          <span className='text-blue-500 hover:underline'>{e.value}</span>
        </Link>
      )
    },
    {
      label: 'Department',
      name: 'Department',
    },
    {
      label: 'Employer',
      name: 'Employer',
      width: '100px',
    },
    {
      label: 'Description',
      name: 'Description',
      alignment: 'left',
    },
    {
      label: 'Status',
      name: 'Status',
      alignment: 'left',
      width: '100px',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'success' : 'orange'} className='text-xs'>{e.value}</Tag>
      )
    },
    {
      label: 'Completion Date',
      name: 'DateCreated',
      alignment: 'left',
      cellRender: ({value}) => (
        <div>{dayjs(value).format('YYYY-MM-DD HH:mm')}</div>
      )
    },
  ]

  return (
    <Table
      data={data}
      columns={memberColumns}
      keyExpr='Id'
      containerClass='shadow-none'
      tableClass='max-h-[calc(100vh-80px)]'
    />
  )
}

export default BookingDetail