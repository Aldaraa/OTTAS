import { PlusOutlined, SearchOutlined } from '@ant-design/icons'
import { Segmented } from 'antd'
import React, { useCallback, useState } from 'react'
import ManageScheduleList from './ManageSchedule'
import ScheduleCalendar from './ScheduleCalendar'

const segmentOptions = [
  {
    label: (
      <div className='flex items-center gap-2 justify-center'>
        Manage Schedule
      </div>
    ),
    value: 'list', 
    key: 'list',
  },
  {
    label: (
      <div className='flex items-center gap-2 justify-center'>
        Schedule Calendar
      </div>
    ),
    value: 'schedule', 
    key: 'schedule',
  },
]

function ManageSchedule() {
  const [ mode, setMode ] = useState('list')

  const onChange = useCallback((e) => {
    setMode(e)
  },[])

  return (
    <>
      <div className='mb-2 w-[300px]'>
        <Segmented
          value={mode}
          block
          options={segmentOptions}
          onChange={onChange}
          className='shadow-md rounded-ot'
        />
      </div>
      <div className={mode === 'list' ? 'block' : 'hidden'}>
        <ManageScheduleList/>
      </div>
      <div className={mode === 'schedule' ? 'block' : 'hidden'}>
        <ScheduleCalendar/>
      </div>
    </>
  )
}

export default ManageSchedule