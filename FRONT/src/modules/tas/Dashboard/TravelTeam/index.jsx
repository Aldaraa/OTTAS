import React from 'react'
import Domestic from './Domestic'
import International from './International'
import Roster from './Roster'
import { Tabs } from 'antd'

function TransportSection() {
  const items = [
    {
      label: 'Domestic',
      key: 1,
      children: <Domestic/>
    },
    {
      label: 'International',
      key: 2,
      children: <International/>
    },
    {
      label: 'Roster',
      key: 3,
      children: <Roster/>
    },
  ]
  return (
    <div className='bg-white rounded-ot shadow-md px-3 col-span-12'>
      <Tabs 
        items={items}
        size='small'
      />
    </div>
  )
}

export default TransportSection