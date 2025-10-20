import React from 'react'
import Admin from './Admin'
import Employer from './Employer'

function SystemData({employeeData}) {
  return (
    <div className='mt-3 px-0 flex flex-col gap-8'>
      <Admin employeeData={employeeData} />
      <Employer employeeData={employeeData}/>
    </div>
  )
}

export default SystemData