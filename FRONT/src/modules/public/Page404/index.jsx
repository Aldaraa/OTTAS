import React from 'react'
import ERROR404 from 'assets/illustrations/404.webp'

function Page404() {
  return (
    <div className='relative rounded-ot bg-white flex flex-col h-[calc(100vh-100px)] justify-center items-center z-10'>
      <img alt='error-404' src={ERROR404} className='h-[400px]'/>
    </div>
  )
}

export default Page404