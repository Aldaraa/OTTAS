import React from 'react'
import ERROR401 from 'assets/illustrations/401.webp'
import { Button } from 'components'
import { useNavigate } from 'react-router-dom'
function Page401() {
  const navigate = useNavigate()
  return (
    <div className='relative bg-white h-screen flex flex-col justify-center items-center z-10'>
      <img alt='error-401' src={ERROR401} className='h-[400px]'/>
      <Button type='primary' onClick={() => navigate('/login')}>Login</Button>
    </div>
  )
}

export default Page401