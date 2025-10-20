import { DownOutlined } from '@ant-design/icons'
import React, { useState } from 'react'
import { twMerge } from 'tailwind-merge'

function Accordion({title='', children, defaultOpen=true, className='', titleClass, whenClosedTitle, ...restprops}) {
  const [ isActive, setIsActive ] = useState(defaultOpen)

  return (
    <div className={twMerge(`rounded-ot overflow-hidden`, className)} {...restprops}>
      <div className={`py-[6px] px-3 border-b text-[14px] cursor-pointer flex justify-between items-center bg-white ${titleClass}`} onClick={() => setIsActive(!isActive)}>
        <div className='flex'>
          <div className='text-xs text-gray-600'>{title}</div>
          {
            whenClosedTitle && !isActive ?
            <div className='ml-[100px] text-[14px]'>
              {whenClosedTitle}
            </div>
            : null
          }
        </div>
        <DownOutlined rotate={isActive ? 180 : 0} />
      </div>
      <div className={`overflow-hidden bg-white ${isActive ? 'h-full' : 'h-0'} transition-all `}>
        {children}
      </div>
    </div>
  )
}

export default Accordion