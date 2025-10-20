import React from 'react'
import Error500 from 'assets/illustrations/500.webp'

function Page500() {
  return (
    <div className='relative z-10 h-screen flex justify-center items-center text-2xl font-bold bg-white'>
      <img alt='error-500' src={Error500}/>
    </div>
  )
}

export default Page500