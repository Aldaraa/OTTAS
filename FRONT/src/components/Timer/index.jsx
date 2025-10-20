import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
var duration = require('dayjs/plugin/duration')
dayjs.extend(duration)

const checkExpired = (event) => {
  if(dayjs(event).diff(dayjs()) >= 0){
    return false
  }else{
    return true
  }
}

const Timer = React.memo(({eventDate, className='', whenEnd=''}) => {
  const [ currentTime, setCurrentTime ] = useState(eventDate ? !checkExpired(eventDate) ? dayjs(eventDate).diff(dayjs()) : null : null)
  const [ isExpired, setIsExpired ] = useState(checkExpired(eventDate))

  useEffect(() => {
    if(dayjs(eventDate).diff(dayjs()) >= 0){
      const interval = setInterval(() => {
        setCurrentTime(dayjs(eventDate).diff(dayjs()))
      }, 1000)
  
      return () => clearInterval(interval)
    }
    else{
      setIsExpired(true)
      setCurrentTime(null)
    }
  },[eventDate])

  return (
    <div className={className}>
      {
        eventDate &&
        (
          isExpired ?
          whenEnd :
          currentTime 
          ? 
          dayjs.duration(currentTime).format(`${parseInt(dayjs.duration(currentTime).format('DD')) > 0 ? 'D[d]' : ''} HH:mm:ss`) 
          : 
          null
        )
      }
    </div>
  )
}, (prev, next) => prev.eventDate === next.eventDate)

export default Timer