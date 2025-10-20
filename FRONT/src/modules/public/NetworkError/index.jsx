import React from 'react'
import NetworkErrorSVG from 'assets/illustrations/Network Error.webp'
function NetworkError() {
  return (
    <div className='relative z-10 h-screen flex justify-center items-center text-2xl font-bold bg-white'>
      <img alt='network-error'/>
      <NetworkErrorSVG/>
    </div>
  )
}

export default NetworkError