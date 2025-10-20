import React from 'react'
import image from 'assets/icons/ot-icon.png'
import { twMerge } from 'tailwind-merge'

function LoadingOT({className='', iconClass=''}) {
  return (
    <div className='relative'>
      <div className={twMerge(`spinner h-12 w-12 border-slate-300 overflow-hidden`, className)}/>
      <div className='absolute inset-0 flex justify-center items-center'>
        <img src={image} className={twMerge('w-[31px]',iconClass)}/>
      </div>
    </div>
  )
}

export default LoadingOT