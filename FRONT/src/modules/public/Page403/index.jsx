import React from 'react'
import ERROR403 from 'assets/illustrations/403.webp'

function Page403() {
  return (
    <div className='relative bg-white flex flex-col justify-center items-center z-10 rounded-ot h-[calc(100vh-50px-70px)] shadow-md'>
      <img alt='error-403' src={ERROR403} className='h-[400px]'/>
    </div>
  )
}

export default Page403