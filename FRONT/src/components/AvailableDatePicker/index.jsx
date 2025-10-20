import { DatePicker } from 'antd'
import React, { useCallback, useState } from 'react'
import { reportInstance } from 'utils/axios'

function AvailableDatePicker(props) {
  const [ availableTimes, setAvailableTimes ] = useState([])

  const getAvailableTimes = useCallback((e) => {
    if(e.format('HH') !== props.value?.format('HH'))
    reportInstance({
      method: 'get',
      url: `reportjob/inactivetime/${e.format('YYYY-MM-DD HH:mm')}`
    }).then((res) => {
      setAvailableTimes(res.data)
    }).catch((err) => {

    })
  },[])

  return (
    <DatePicker
      showTime={{format: 'HH:mm'}}
      format={'YYYY-MM-DD HH:mm'}
      disabledTime={() => ({
        disabledMinutes: (e) => availableTimes,
      })}
      onSelect={(e) => getAvailableTimes(e)} 
      {...props}
    />
  )
}

export default AvailableDatePicker