import React from 'react'
import AccommodationConfig from './AccommodationConfig'
import FlightConfig from './FlightConfig'

const title = 'Non Site Travel Configuration'

function NonSiteTravelConfig() {

  return (
    <div>
      <div className='rounded-ot bg-white px-5 py-3 mb-5 shadow-md'>
        <div className='text-[16px] font-bold'>{title}</div>
      </div>
      <div className='flex gap-8 items-start'>
        <FlightConfig/>
        <AccommodationConfig/>
      </div>
    </div>
  )
}

export default NonSiteTravelConfig