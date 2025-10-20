import React from 'react'

function ErrorTabElement({children}) {
  return (
    <span className='bg-red-400 rounded-md px-[5px] py-[2px] text-xs font-light text-white ml-2'>{children}</span>
  )
}

export default ErrorTabElement