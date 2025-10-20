import { Button } from 'components'
import React from 'react'
import { Link, useRouteError } from 'react-router-dom'
import { Page401, Page404 } from '..'
import Page403 from '../Page403'

function LocalError() {
  const error = useRouteError()

  const renderError = () => {
    if(error.status === 400){
      return <div className='text-xl bg-white rounded-ot h-full w-full flex flex-col justify-center items-center gap-4'>
        <div className='font-bold'>
          {error?.data}
        </div>
        <Link to='/tas'>
          <Button type='primary'>Back To Home</Button>
        </Link>
      </div>
    }else{
      return <div className='text-xl'>
        NOT FOUND
        <Page401/>
        <Page403/>
        <Page404/>
      </div>
    }
  }
  return (
    <div className='h-full'>
      {renderError()}
    </div>
  )
}

export default LocalError