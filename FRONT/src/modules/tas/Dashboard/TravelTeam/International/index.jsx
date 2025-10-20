import React, { useState, useEffect } from 'react'
import Booking from './Booking'
import IssueTicket from './IssueTicket'
import RequestByTicketAgent from './RequestByTicketAgent'
import RequestByActionAndDoc from './RequestByActionAndDoc'
import { DatePicker } from 'antd'
import { SearchOutlined, SwapRightOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { Button } from 'components'
import axios from 'axios'

function International() {
  const [ startWeek, setStartWeek ] = useState(dayjs().startOf('isoWeek').subtract(5, 'week'))
  const [ endWeek, setEndWeek ] = useState(dayjs().endOf('isoWeek'))
  const [ data, setData ] = useState(null)
  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboardtransportadmin/international?startDate=${startWeek.format('YYYY-MM-DD')}&endDate=${endWeek.format('YYYY-MM-DD')}`,
    }).then((res) => {
      setData(res.data)
    })
  }
  
  return (
    <div>
      <div className='flex justify-end items-center gap-5 py-2'>
        <div className='text-secondary2'>
          <span className='mr-1'>{startWeek.format('MMM DD')}</span>
          —
          <span className='ml-1'>{endWeek.format('MMM DD')}</span>
        </div>
        <div className='flex items-center gap-3'>
          <DatePicker.WeekPicker placeholder='Start Week' value={startWeek} onChange={(date) => setStartWeek(date)} allowClear={false}/>
          <SwapRightOutlined/>
          <DatePicker.WeekPicker placeholder='End Week' value={endWeek} onChange={(date) => setEndWeek(date)} allowClear={false}/>
          <Button type='primary' className='py-[7px]' icon={<SearchOutlined/>} onClick={getData}/>
        </div>
      </div>
      <div className='grid grid-cols-12 gap-4'>
        <div className='col-span-6 border rounded-ot overflow-hidden'>
          <IssueTicket startDate={startWeek} endDate={endWeek} data={data?.TravelPurpose}/>
        </div>
        <div className='col-span-6 border rounded-ot overflow-hidden'>
          <Booking startDate={startWeek} endDate={endWeek} data={data?.Hotel}/>
        </div>
        <div className='col-span-6 border rounded-ot overflow-hidden'>
          <RequestByTicketAgent startDate={startWeek} endDate={endWeek} data={data?.TravelAgent}/>
        </div>
        <div className='col-span-6 border rounded-ot overflow-hidden'>
          <RequestByActionAndDoc startDate={startWeek} endDate={endWeek} data={data?.Documents}/>
        </div>
      </div>
    </div>
  )
}

export default International