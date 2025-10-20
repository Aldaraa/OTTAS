import { Tabs } from 'antd'
import { ByPersonExtended } from 'components'
import React, { useMemo, useState } from 'react'
import { useNavigate, useRouteLoaderData } from 'react-router-dom'
import Ownership from './components/Ownership'
import Temporary from './components/Temporary'
import RemoveOwenership from './components/RemoveOwenership'

function RoomAssignDetail() {
  const data = useRouteLoaderData('assignRoomLayout')
  const [ mode, setMode ] = useState('roombooking')

  const items = useMemo(() => {
    let tabs = [
      {
        label: 'Room Booking',
        key: 'roombooking',
        children: <ByPersonExtended empData={data} changeTab={(key) => setMode(key)}/>,
      },
      {
        label: 'Assign Room Ownership',
        key: 'ownership',
        children: <Ownership data={data} changeTab={(key) => setMode(key)}/>,
      },
      {
        label: 'Temporary Room Move',
        key: 'temporary',
        children: <Temporary data={data} changeTab={(key) => setMode(key)}/>,
      },
    ]
    if(data?.RoomId) tabs.push({
      label: 'Remove Ownership',
      key: 'remove',
      children: <RemoveOwenership data={data} changeTab={(key) => setMode(key)}/>,
    })

    return tabs

  }, [data])

  return (
    <div className='bg-white p-4 rounded-ot '>
      <Tabs
        activeKey={mode}
        items={items}
        destroyInactiveTabPane={true}
        onChange={(e) => setMode(e)}
        type='card'
      />
    </div>
  )
}

export default RoomAssignDetail