import axios from 'axios'
import { Button, Table } from 'components'
import React, { useEffect, useState } from 'react'
import { DatePicker, Tabs } from 'antd'
import { SearchOutlined } from '@ant-design/icons'
import { Link, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import useQuery from 'utils/useQuery'
import ls from 'utils/ls'

const { RangePicker } = DatePicker;

function RoomBooking() {
  const [ data, setData ] = useState([])
  const [ selectedDate, setSelectedDate ] = useState([dayjs().startOf('month'), dayjs().endOf('month')])
  const [ loading, setLoading ] = useState(false)

  const { employeeId } = useParams()

  const query = useQuery()

  useEffect(() => {
    ls.set('pp_rt', 'roombooking')
    if(query.size > 0){
      let startDate = query.get('startDate')
      let endDate = query.get('endDate')
      setSelectedDate([dayjs(startDate), dayjs(endDate)])
      axios({
        method: 'get',
        url: `tas/employeestatus/employeebooking?employeeId=${employeeId}${`&startDate=${startDate}&endDate=${endDate}`}`
      }).then((res) => {
        setData(res.data)
      }).catch((err) => {
  
      })
    }else{
      getData()
    }
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employeestatus/employeebooking?employeeId=${employeeId}${(selectedDate[0] && selectedDate[1]) ? `&startDate=${dayjs(selectedDate[0]).format('YYYY-MM-DD')}&endDate=${dayjs(selectedDate[1]).format('YYYY-MM-DD')}` : ''}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    })
  }

  const handleSearch = () => {
    if(selectedDate[0] && selectedDate[1]){
      setLoading(true)
      axios({
        method: 'get',
        url: `tas/employeestatus/employeebooking?employeeId=${employeeId}&startDate=${dayjs(selectedDate[0]).format('YYYY-MM-DD')}&endDate=${dayjs(selectedDate[1]).format('YYYY-MM-DD')}`,
      }).then((res) => {
        setData(res.data)
      }).catch((err) => {
  
      }).then(() => setLoading(false))
    }
  }

  const column = [
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
    {
      label: '',
      name: 'action',
      width: '140px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <Link to={`/tas/roomcalendar/${e.data.RoomId}/${dayjs(e.data.DateIn).startOf('month').format('YYYY-MM-DD')}?empId=${employeeId}`}>
            <button type='button' className='edit-button'>Room Calendar</button>
          </Link>
        </div>
      )
    },
  ]

  return (
    <div className='bg-white rounded-ot px-3 col-span-12 shadow-md'>
      <div className='flex justify-between items-center py-2'>
        <div className='text-lg font-bold'>Room Occupancy Enquiry</div>
        <div className='flex gap-5'>
          <RangePicker 
            value={selectedDate}
            onChange={(e) => setSelectedDate(e)}
          />
          <Button 
            onClick={handleSearch} 
            loading={loading} 
            icon={<SearchOutlined/>}
          >
            Search
          </Button>
        </div>
      </div>
      <Table
        data={data}
        columns={column}
        allowColumnReordering={false}
        loading={loading}
        pager={{showPageSizeSelector: data?.length > 100}}
        containerClass='shadow-none px-0 border-t rounded-none'
        tableClass='max-h-[calc(100vh-400px)]'
        keyExpr='Id'
      />
    </div>
  )
}

export default RoomBooking