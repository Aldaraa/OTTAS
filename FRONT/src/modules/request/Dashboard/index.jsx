import React, { useContext, useEffect, useState } from 'react'
import { AuthContext } from 'contexts'
import axios from 'axios'
import TotalRequest from './TotalRequest'
import TotalReqByMonth from './TotalReqByMonth'
import { DatePicker } from 'antd'
import { SearchOutlined, SwapRightOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { Button } from 'components'
import DaysAway from './DaysAway'
import DeclinedSiteTravel from './DeclinedSiteTravel'

function Dashboard() {
  const { action } = useContext(AuthContext)
  const [ data, setData ] = useState(null)
  const [ startDate, setStartDate ] = useState(dayjs().subtract(1, 'M'))
  const [ endDate, setEndDate ] = useState(dayjs())

  useEffect(() => {
    action.changeMenuKey('/request/dashboard')
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboardrequest/documentdata?startDate=${startDate.format('YYYY-MM-DD')}&endDate=${endDate.format('YYYY-MM-DD')}`
    }).then((res) => {
      setData(res.data)
    })
  }

  return (
    <div className='grid grid-cols-12 gap-5'>
      <div className='col-span-12 flex'>
        <div className='flex items-center gap-5 py-2 px-4 bg-white rounded-ot'>
          <div className='text-secondary2'>
            <span className='mr-1'>{startDate.format('MMM DD')}</span>
            —
            <span className='ml-1'>{endDate.format('MMM DD')}</span>
          </div>
          <div className='flex items-center gap-3'>
            <DatePicker.WeekPicker placeholder='Start Week' value={startDate} onChange={(date) => setStartDate(date)} allowClear={false}/>
            <SwapRightOutlined/>
            <DatePicker.WeekPicker placeholder='End Week' value={endDate} onChange={(date) => setEndDate(date)} allowClear={false}/>
            <Button type='primary' className='py-[7px]' icon={<SearchOutlined/>} onClick={getData}/>
          </div>
        </div>
      </div>
      <div className='bg-white rounded-ot col-span-6 overflow-hidden shadow-lg'>
        <TotalRequest data={data?.totalRequests}/>
      </div>
      <div className='bg-white rounded-ot col-span-6 overflow-hidden shadow-lg'>
        <TotalReqByMonth data={data?.monthData}/>
      </div>
      <div className='bg-white rounded-ot col-span-6 overflow-hidden shadow-lg'>
        <DeclinedSiteTravel data={data?.SiteTravelDeclined}/>
      </div>
      <div className='bg-white rounded-ot col-span-6 overflow-hidden shadow-lg'>
        <DaysAway data={data?.PendingDocumentDaysAway}/>
      </div>
    </div>
  )
}

export default Dashboard