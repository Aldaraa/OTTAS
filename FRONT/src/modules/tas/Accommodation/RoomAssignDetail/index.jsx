import React, { useContext, useEffect, useState } from 'react'
import { Outlet, useLoaderData } from 'react-router-dom'
import EmployeeProfile from './components/EmployeeProfile'
import { AuthContext } from 'contexts'

function RoomAssignLayout() {
  const data = useLoaderData()
  const { action } = useContext(AuthContext)
  useEffect(() => {
    if(data){
      action.saveUserProfileData(data)
    }
  },[data])
  return (
    <div className=''>
      <EmployeeProfile profileData={data}/>
      <Outlet/>
    </div>
  )
}

export default RoomAssignLayout