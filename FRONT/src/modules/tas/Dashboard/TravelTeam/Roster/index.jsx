import React, { useCallback, useEffect, useMemo, useState } from 'react'
import PersonByWeek from './PersonByWeek'
import axios from 'axios'
import dayjs from 'dayjs'
import { DatePicker, Segmented } from 'antd'
import { LoadingOutlined, SearchOutlined, SwapRightOutlined } from '@ant-design/icons'
import { Button, Skeleton } from 'components'
import FlightGroup from './FlightGroup'

function Roster() {
  const [ currentWeek, setCurrentWeek ] = useState(dayjs().startOf('isoWeek'))
  const [ picker, setPicker ] = useState('week')
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  useEffect(() => {
    getData()
  },[])

  const getData = useCallback(() => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/dashboardtransportadmin/roster/${picker}?startDate=${currentWeek.format('YYYY-MM-DD')}`,
    }).then((res) => {
      setData(res.data)
    }).finally(() => {
      setLoading(false)
    })
  },[picker, currentWeek])

  const fixedData = useMemo(() => {
    const drilldownList = []
    const newSeries = []
    data?.RosterData?.map((item) => {
      newSeries.push({name: item.Description, y: item.Count, drilldown: JSON.stringify(item.Drilldown)})
      let details = item.Details.map((d_item) => ([d_item.Name, d_item.Count]))
      drilldownList.push({name: item.Description, id: JSON.stringify(item.Drilldown), data: details})
    })
    return {
      series: newSeries,
      drilldown: drilldownList
    }
  },[data])

  return (
    <div>
      <div className='flex justify-end items-center gap-5 py-2'>
        <Segmented 
          options={[{label: 'Weekly', value: 'week'}, {label: 'Monthly', value: 'month'}, {label: 'Quarterly', value: 'quarter'}, {label: 'Yearly', value: 'year'},]}
          value={picker}
          onChange={(e) => setPicker(e)}
        />
        <div className='text-secondary2'>
          <span className='mr-1'>{currentWeek.startOf('isoWeek').format('MMM DD')}</span>
          —
          <span className='ml-1'>{currentWeek.endOf('isoWeek').format('MMM DD')}</span>
        </div>
        <div className='flex items-center gap-3'>
          <DatePicker
            picker={picker}
            placeholder='Start Week'
            value={currentWeek}
            onChange={(date) => setCurrentWeek(date)}
            allowClear={false}
          />
          <Button type='primary' className='py-[7px]' icon={<SearchOutlined/>} onClick={getData}/>
        </div>
      </div>
      <div className='grid grid-cols-12 gap-6'>
        <div className='col-span-12 relative border rounded-ot overflow-hidden'>
          {
            loading ?
            <div className='absolute inset-0 flex justify-center items-center z-10'>
              <LoadingOutlined style={{fontSize: 32}}/>
            </div> : null
          }
          <PersonByWeek data={fixedData} startDate={currentWeek} picker={picker}/>
        </div>
        <div className='col-span-12 relative'>
          {
            loading ?
            <div className='absolute inset-0 flex justify-center items-center z-10'>
              <LoadingOutlined style={{fontSize: 32}}/>
            </div> : null
          }
          <FlightGroup data={data?.FlightGroupData} startDate={currentWeek} picker={picker}/>
        </div>
      </div>
    </div>
  )
}

export default Roster