import React, { useContext, useEffect, useMemo, useState } from 'react'
import TravelTeam from './TravelTeam'
import { AuthContext } from 'contexts'
import DataApproval from './DataApproval'
import AccommodationDashboard from './Accommodation'
import SystemAdmin from './SystemAdmin'
import { Segmented } from 'antd'

function Dashboard() {
  const [ currentTab, setCurrrentTab ] = useState('SystemAdmin')
  const { action, state } = useContext(AuthContext);

  useEffect(() => {
    action.changeMenuKey('/tas/dashboard');
  },[])

  const renderDashBoard = useMemo(() => {
    switch (currentTab) {
      case 'DASHBOARD_101':
        return <SystemAdmin/>
      case 'DASHBOARD_104':
        return <TravelTeam/>
      case 'DASHBOARD_103':
        return <AccommodationDashboard/>
      case 'DASHBOARD_102':
        return <DataApproval/>
      default:
        return <div className='bg-white p-4 rounded-ot'>No Dashboard</div>;
    }
  },[currentTab])

  const options = useMemo(() => {
    let tmp = [];
    if(state.userInfo.Dashboard?.length > 0){
      setCurrrentTab(state.userInfo.Dashboard[0].Code)
    }
    tmp = state.userInfo.Dashboard?.map((item) => ({label: item.Name, value: item.Code}))
    return tmp
  },[state.userInfo])

  return (
    <div className='flex flex-col gap-5'>
      {
        options.length > 1 ?
        <div className='rounded-ot overflow-hidden'>
          <Segmented
            value={currentTab}
            options={options}
            onChange={(value) => setCurrrentTab(value)}
          />
        </div>
        : null
      }
      {renderDashBoard}
    </div>
  )
}

export default Dashboard