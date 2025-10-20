import { LoadingOutlined } from '@ant-design/icons'
import { Tooltip } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import { reportInstance } from 'utils/axios'

function AvailableTimeTooltip() {
  const [ loading, setLoading ] = useState(false)
  const [ availableTimeslots, setAvailableTimeslots ] = useState([])

  
  useEffect(() => {
    const getTimeslots = () => {
      setLoading(true)
      reportInstance({
        method: 'get',
        url: 'reportjob/getavailabletimeslots'
      }).then((res) => {
        setAvailableTimeslots(res.data.map((item) => dayjs(item).format('HH:mm')))
      }).catch(() => {
  
      }).finally(() => setLoading(false))
    }

    getTimeslots()
  },[])


  return (
    <Tooltip trigger='click' title={
      <div>
        { loading ? 
          <LoadingOutlined/> 
          : 
          <ul className=' list-none max-h-[200px] overflow-y-auto px-2'>
            {
              availableTimeslots.map((item, i) => <li key={`time_slot_${i}`}>{item}</li>)
            }
          </ul>
        }
      </div>
    }>
      <div className="text-blue-400 hover:underline cursor-pointer">Available Timeslots</div>
    </Tooltip>
  )
}

export default AvailableTimeTooltip