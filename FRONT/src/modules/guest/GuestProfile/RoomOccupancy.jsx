import React, { useEffect, useState } from 'react'
import axios from 'axios'
import { Table } from 'components'
import dayjs from 'dayjs'
import { Link, useParams } from 'react-router-dom'

const columns = [
  {
    label: 'Owner', 
    name: 'RoomOwner', 
    alignment: 'center', 
    width: '55px',
    cellRender: (e) => (
      <span className='text-[12px] px-1'>
        {
          e.value ? 
          <i className="dx-icon-home text-green-500"></i> 
          :
          <i className="dx-icon-home text-gray-400 text-[14px]"></i>
        }
      </span>
    )
  },
  {
    label: 'Room Number',
    name: 'RoomNumber',
  },
  {
    label: 'Camp',
    name: 'Camp',
  },
  {
    label: 'Room Type',
    name: 'RoomType',
  },
  {
    label: 'Date In',
    name: 'DateIn',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
    )
  },
  {
    label: 'Last Night',
    name: 'LastNight',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
    )
  },
  {
    label: 'Days',
    name: 'Days',
  },
]

function RoomOccupancy({firstDate, lastDate, empId}) {
  const [ loading, setLoading ] = useState(false)
  const [ data, setData ] = useState([])

  useEffect(() => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/employeestatus/employeebooking?employeeId=${empId}${`&startDate=${firstDate}&endDate=${lastDate}`}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  },[firstDate, lastDate])

   return (
     <div className='py-4 '>
       <Table
         data={data}
         columns={columns}
         allowColumnReordering={false}
         loading={loading}
         containerClass='shadow-none border border-gray-300'
         keyExpr='Id'
         pager={data.length > 20}
       />
     </div>
   )
 }

export default RoomOccupancy