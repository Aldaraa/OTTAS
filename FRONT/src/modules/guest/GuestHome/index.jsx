import { Tabs } from 'antd'
import { FlightList, VerticalCalendar } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import { ActiveTransportSchedule } from 'modules/request'
import React, { useContext } from 'react'
import RoomOccupancy from '../GuestProfile/RoomOccupancy'

function GuestHome() {
  const { state } = useContext(AuthContext)

  const items = [
    {
      label: 'Schedule',
      key: 1,
      children: <div className='flex items-start gap-5 pt-3'>
        <VerticalCalendar employeeId={state.userInfo.Id} containerClass='shadow-none'/>
        <FlightList profileData={state.userProfileData} employeeId={state.userInfo.Id} className='shadow-none'/>
      </div>
    },
    {
      label: 'Active Transport',
      key: 2,
      children: <ActiveTransportSchedule/>,
    },
    {
      key: 4,
      label: `Room Occupancy Enquiry`,
      children: <RoomOccupancy firstDate={dayjs().subtract(15, 'd')} lastDate={dayjs().add(1, 'M')} empId={state.userInfo.Id}/>
    },
  ]

  return (
    <Tabs
      items={items}
      type='card'
    />
  )
}

export default GuestHome