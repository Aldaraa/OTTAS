import dayjs from 'dayjs';
import React, { useMemo } from 'react'

const dateFormat = 'YYYY-MM-DD'

const getBgColor = (status) => {
  switch (status) {
    case 'full': return 'bg-[#ebedf0] border-[#ebedf0]'
    case 'available': return 'bg-[#fadb14] border-[#fadb14]'
    case 'empty': return 'bg-[#00c75e] border-[#00c75e]'
    case 'double': return ' bg-[#ff5b5b] border-[#ff5b5b] text-white'
  }
}

const CellRender = React.memo(({event, index, currentDate, handleSelectDate, selectedDate}) => {
  const cDate = dayjs(currentDate).date(index).format(dateFormat);
  
  let currentStatus = event.value === event.data?.OccupancyData[cDate]?.BedCount 
  ? 'empty' 
  : (event.value < event.data?.OccupancyData[cDate]?.BedCount) 
  && (event.value > 0) ? 'available' : (event.value < 0) ? 'double' : 'full'

  return(
    <div 
      // className={`relative rounded select-none ${currentStatus !== 'empty' ? 'cursor-pointer hover:border-slate-400' : 'cursor-default'} py-1 text-xs border ${getBgColor(currentStatus)}`} 
      className={`relative rounded select-none cursor-pointer hover:border-slate-400 py-1 text-xs border ${getBgColor(currentStatus)}`} 
      onClick={(e) => handleSelectDate(event.data.RoomId, cDate, event , currentStatus, e)}
    >
      {
        (selectedDate?.roomId === event.data?.RoomId) && (selectedDate?.date === cDate)
        ?
        <div className='absolute inset-0 border border-black rounded'></div>
        :
        ''
      }
        {currentStatus === 'empty' ? <span className='select-none'>&nbsp;</span> :  event.value}
    </div>
  )
}, (prev, next) => {
  if(JSON.stringify(prev.selectedDate) !== JSON.stringify(next.selectedDate)){
    return false
  }
  return true
})

export default CellRender