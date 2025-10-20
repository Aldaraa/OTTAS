import { Button } from 'components'
import React, { useMemo } from 'react'
import { Link, useRouteError } from 'react-router-dom'

function LocalError({listUrl}) {
  const error = useRouteError()
  if(error.status === 499) {
    return(
      <div className='h-full'>
        <div className='bg-white rounded-ot h-full w-full flex flex-col justify-center items-center gap-4'>
          <div className='font-bold text-xl'>
            {error.data}
          </div>
          {
            listUrl ?
            <Link to={listUrl}>
              <Button type='primary' >Back To List</Button>
            </Link>
            : null
          }
        </div>
      </div>
    )
  }
  else{
    return(
      <div className='h-full'>
        <div className='bg-white rounded-ot h-full w-full flex flex-col justify-center items-center gap-4'>
          <div className='font-bold text-xl '>
            {error.data}
          </div>
          {
            listUrl ?
            <Link to={listUrl}>
              <Button type='primary'>Back To List</Button>
            </Link>
            : null
          }
        </div>
      </div>
    )
  }
}

export default LocalError