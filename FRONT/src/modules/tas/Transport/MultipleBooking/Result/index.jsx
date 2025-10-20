import React, { useMemo } from 'react'
import ConfirmedRosters from './ConfirmedRosters'
import SkippedRosters from './SkippedRosters'
import { Tabs } from 'antd'

function Result({data}) {
  const items = useMemo(() => {
    const employeeCount = [...new Set(data?.RosterExecutedEmployees.map(item => item.EmployeeId))]
    return [
      {
        key: '1',
        label: `Executed Employee (${employeeCount.length})`,
        children: <ConfirmedRosters data={data?.RosterExecutedEmployees}/>
      },
        
      {
        key: '2',
        label: `Skipped (${data?.RosterSkippedEmployees?.length})`,
        children: <SkippedRosters data={data.RosterSkippedEmployees}/>
      }
    ]
  },[data])
  return (
    <Tabs items={items} type='card'/>
  )
}

export default Result