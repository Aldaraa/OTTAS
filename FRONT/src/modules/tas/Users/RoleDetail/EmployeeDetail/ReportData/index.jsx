import React from 'react'
import Department from './Department'
import Employer from './Employer'

function ReportData({employeeData, refreshData}) {

  return (
    <div className='mt-3 px-0 flex flex-col gap-8'>
      <Department employeeData={employeeData} refreshData={refreshData}/>
      <Employer employeeData={employeeData} refreshData={refreshData}/>
    </div>
  )
}

export default ReportData