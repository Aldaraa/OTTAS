import dayjs from 'dayjs'
import React from 'react'

function Header({dayOfWeek, date}) {
  return (
    <div className='basis-full rounded-t font-base bg-[#f5f5f5] flex items-end justify-center py-1 gap-2 h-7 relative border-b'>
      <span className='font-medium text-base leading-none uppercase'>{dayOfWeek}</span>
      <span className='absolute text-xs leading-none left-0 top-0 p-1 text-gray-500'>{dayjs(date).format('MMM DD')}</span>
    </div>
  )
}

export default Header