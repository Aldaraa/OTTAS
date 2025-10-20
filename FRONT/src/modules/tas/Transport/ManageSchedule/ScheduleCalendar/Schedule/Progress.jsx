import React from 'react'
import { twMerge } from 'tailwind-merge'

const progressColor = (percent) => {
  if(percent === 100){
    return 'bg-green-400'
  }else if(percent > 100){
    return 'bg-red-500'
  }else{
    return 'bg-[#ffe005]'
  }
}
function Progress({percent}) {
  return (
    <div className='bg-[#e1e1e1] relative h-[6px] w-full'>
      <div className={twMerge('absolute inset-0', progressColor(percent))} style={{width: `${percent}%`}}></div>
    </div>
  )
}

export default Progress