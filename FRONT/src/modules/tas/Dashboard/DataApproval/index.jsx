import React from 'react'
import { EmployeeStatus, PackMeal, OnSiteVisitor, OnSite, OffSite, ProfileChanges, Flight } from './views';

function DataApproval() {
  return (
    <div className='grid grid-cols-12 gap-4'>
      <div className='col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
        <OnSite/>
      </div>
      <div className='col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
        <OffSite/>
      </div>
      <div className='col-span-4 bg-white rounded-ot overflow-hidden shadow-md'>
        <PackMeal/>
      </div>
      <div className='col-span-4 bg-white rounded-ot overflow-hidden shadow-md'>
        <EmployeeStatus/>
      </div>
      <div className='col-span-4 bg-white rounded-ot overflow-hidden shadow-md'>
        <Flight/>
      </div>
      <div className='col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
        <OnSiteVisitor/>
      </div>
      <div className='col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
        <ProfileChanges/>
      </div>
    </div>
  )
}

export default DataApproval