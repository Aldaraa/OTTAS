import React, { useEffect, useMemo, useState } from 'react'
import { DatePicker, Tabs } from 'antd'
import dayjs from 'dayjs'
import ls from 'utils/ls'
import NoShow from './NoShow'
import GoShow from './GoShow'

const fields = [
  {
    name: 'EventDate',
    label: 'Event Date',
    type: 'date',
    className: 'col-span-12 mb-2'
  },
  {
    name: 'Direction',
    label: 'Direction',
    type: 'select',
    className: 'col-span-12 mb-2',
    inputprops: {
      options: [{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]
    }
  },
  {
    name: 'Description',
    label: 'Description',
    className: 'col-span-12 mb-2',
  },
  {
    name: 'Reason',
    label: 'Reason',
    className: 'col-span-12 mb-2',
    type: 'textarea'
  },
]

function NoShowAndGoShow() {
  const [ selectedDate, setSelectedDate ] = useState(dayjs())

  useEffect(() => {
    ls.set('pp_rt', 'noshow')
  },[])

  

  const items = useMemo(() => [
    {
      key: 'noShow',
      label: 'No Show',
      children: <NoShow selectedDate={selectedDate} fields={fields}/>
    },
    {
      key: 'goShow',
      label: 'Go Show',
      children: <GoShow selectedDate={selectedDate} fields={fields}/>,
    }
  ], [selectedDate])

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      <Tabs
        type='card'
        items={items}
        tabBarExtraContent={<DatePicker value={selectedDate} onChange={(e) => setSelectedDate(e)} picker='year'/>}
      />
    </div>
  )
}

export default NoShowAndGoShow