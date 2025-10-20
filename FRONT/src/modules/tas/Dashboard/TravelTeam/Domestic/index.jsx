import { SearchOutlined, SwapRightOutlined } from '@ant-design/icons'
import { Checkbox, DatePicker, Segmented } from 'antd'
import axios from 'axios'
import { Button } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import UtilizationsByWeek from './UtilizationsByWeek'
import TransportInfo from './TransportInfo'
import NoShowGoShow from './NoShowGoShow'
import AirCraft from './AirCraft'
import { BsAirplaneFill } from 'react-icons/bs'
import { twMerge } from 'tailwind-merge'
import { CheckBox } from 'devextreme-react'

const colors = ['#FFF0AA', '#FFC69A', '#FFB3A7', '#D4D9F8', '#E1C4E8', '#FFC3DC', '#C0E0F9', '#B2E6E2', '#B3E8B4', '#B8D4AA', '#F9B8B8', '#D9F8B8', '#FFF0AA', '#FFC69A', '#FFB3A7', '#D4D9F8', '#E1C4E8', '#FFC3DC', '#C0E0F9', '#B2E6E2', '#B3E8B4', '#B8D4AA', '#F9B8B8', '#D9F8B8']

function Domestic() {
  const [ currentWeek, setCurrentWeek ] = useState(dayjs().startOf('isoWeek'))
  const [ endWeek, setEndWeek ] = useState(dayjs().endOf('isoWeek'))
  const [ data, setData ] = useState(null)
  const [ picker, setPicker ] = useState('Airplane')

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboardtransportadmin/domestic?startDate=${currentWeek.startOf('isoWeek').format('YYYY-MM-DD')}`,
    }).then((res) => {
      setData(res.data)
    })
  }

  const options = [
    { label: 'Airplane', value: 'Airplane' },
    { label: 'Bus', value: 'Bus' },
    { label: 'Drive', value: 'Drive' },
  ];
  return (
    <div>
      
      <TransportInfo data={data?.DateTransport}/>
      <div className='border rounded-ot mb-4 overflow-hidden'>
        <div className='flex justify-between items-center gap-5 py-2 px-3 border-b'>
          <div>
            <Segmented options={options} size='small' onChange={(e) => setPicker(e)} value={picker}/>
          </div>
          <div className='flex items-center gap-5'>
            <div className='text-secondary2'>
              <span className='mr-1'>{currentWeek.startOf('isoWeek').format('MMM DD')}</span>
              —
              <span className='ml-1'>{currentWeek.endOf('isoWeek').format('MMM DD')}</span>
            </div>
            <div className='flex items-center gap-3'>
              <DatePicker.WeekPicker value={currentWeek} onChange={(date) => setCurrentWeek(date)} allowClear={false}/>
              <Button type='primary' className='py-[7px]' icon={<SearchOutlined/>} onClick={getData}/>
            </div>
          </div>
        </div>
        <AirCraft data={data?.AircraftData} date={currentWeek} picker={picker}/>
      </div>
      <div className='grid grid-cols-12 gap-4'>
        <div className='col-span-4 border rounded-ot overflow-hidden'>
          <UtilizationsByWeek data={data?.WeekUtil} date={currentWeek}/>
        </div>
        <div className='col-span-4 border rounded-ot overflow-hidden'>
          <NoShowGoShow data={data?.NoShow} date={currentWeek}/>
        </div>
      </div>
    </div>
  )
}

export default Domestic