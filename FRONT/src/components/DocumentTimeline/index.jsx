import { Tag, Timeline } from 'antd'
import { Table } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'

const getTimelineColor = (type) => {
  switch (type) {
    case 'Submitted':
      return <Tag color='purple' >{type}</Tag>
      // return <div className='text-[#1777ff] bg-blue-100 p-1 border border-[#1777ff] rounded'>{type}</div>
    case 'Confirmed':
      return <Tag color='orange'>{type}</Tag>
    case 'Saved':
      return <Tag color='cyan'>{type}</Tag>
    case 'Approved':
      return <Tag color='blue'>{type}</Tag>
    case 'Waiting Agent':
      return <Tag color='yellow'>{type}</Tag>
    case 'Cancelled':
      return <Tag color='red'>{type}</Tag>
    case 'Declined':
      return <Tag>{type}</Tag>
    case 'Completed':
      return <Tag color='success'>{type}</Tag>
  }
}

function DocumentTimeline({data}) {
  const [ items, setItems ] = useState([])

  useEffect(() => {
    if(data){
      let tmp = []
      data.map((item) => {
        tmp.push({
          children: <div className='flex flex-col items-start'>
            <div className='text-gray-500 text-xs'>{dayjs(item.CreateDate).format('YYYY-MM-DD ddd HH:mm')}</div>
            <div className='text-sm font-bold mt-2'>{item.CurrentAction}</div>
            <div className='rounded-ot border p-2 px-3'>
              <div className='text-xs mb-2'>{item.CurrentAction} by {item.ActionEmployeeFullName}</div>
              <div className='text-gray-400 font-bold text-xs italic'>{item.Comment ? `"${item.Comment}"` : ''}</div>
            </div>
          </div>,
          color: getTimelineColor(item.CurrentAction)
        })
      })
      setItems(tmp)
    }
  },[data])

  const columns = [
    {
      label: 'Action',
      name: 'CurrentAction',
      alignment: 'left',
      width: 100,
      cellRender: (e) => (
        <div className='flex'>
          {getTimelineColor(e.value)}
        </div>
      )
    },
    {
      label: 'Employee',
      name: 'ActionEmployeeFullName',
      alignment: 'left',
    },
    {
      label: 'Appoval Group',
      name: 'AssignedGroupName',
      alignment: 'left',
    },
    {
      label: 'Comment',
      name: 'Comment',
      alignment: 'left',
    },
    {
      label: 'Date',
      name: 'CreateDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm:ss')}</div>
      )
    },
  ]
  return (
    <Table
      data={data}
      columns={columns}
      containerClass='col-span-12 shadow-none border border-gray-300'
      pager={false}
    />
  )
}

export default DocumentTimeline