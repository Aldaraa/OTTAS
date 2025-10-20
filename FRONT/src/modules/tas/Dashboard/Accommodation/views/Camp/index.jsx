import React, { useMemo } from 'react'
import { Tabs } from 'antd'
import CampChart from './CampChart'

function Camp({data=[]}) {

  const items = useMemo(() => {
    return data.map((item, i) => ({
      key: i,
      label: item.Camp,
      children: <CampChart data={item}/>
    })) 
  },[data])

  return (
    <div className='px-2'>
      <Tabs size='small' items={items} tabPosition='bottom'/>
    </div>
  )
}

export default Camp