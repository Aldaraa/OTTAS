import React from 'react'
import { twMerge } from 'tailwind-merge'

function Skeleton({className}) {
  return (
    <div 
      className={twMerge(`overflow-hidden relative rounded-2xl bg-[#eee]
        before:absolute before:inset-0 before:-translate-x-full 
        before:animate-[shimmer_1.5s_infinite] before:bg-gradient-to-r 
        before:from-transparent before:via-gray-300 before:to-transparent`, 
        className
      )}
    >

    </div>
  )
}

export default Skeleton