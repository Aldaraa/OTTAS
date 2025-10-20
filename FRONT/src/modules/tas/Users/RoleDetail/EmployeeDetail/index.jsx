import { DashboardOutlined, FileTextOutlined,  MenuUnfoldOutlined, SnippetsOutlined, TeamOutlined } from '@ant-design/icons'
import { Drawer, Tabs } from 'antd'
import React, { useMemo } from 'react'
import EmployeeMenu from './EmployeeMenu'
import Template from './Template'
import ReportData from './ReportData'
import Manager from './Manager'
import DashboardPermission from './DashboardPermission'
import SystemData from './SystemData'

function EmployeeDetail({selectedEmployee, open, onClose, roleData, refreshData}) {

  const items = useMemo(() => {
    let tabs = [
      {
        label: 'Menu Permission',
        key: 'Menu',
        children: <EmployeeMenu employeeData={selectedEmployee} refreshData={refreshData}/>,
        icon: <MenuUnfoldOutlined/>
      },
      {
        label: 'Report Permission',
        key: 'Report',
        children: <Template employeeData={selectedEmployee} refreshData={refreshData}/>,
        icon: <SnippetsOutlined />
      },
      {
        label: 'Report Data Permission',
        key: 'ReportData',
        children: <ReportData employeeData={selectedEmployee} refreshData={refreshData}/>,
        icon: <FileTextOutlined />
      },
      {
        label: 'Dashboard',
        key: 'Dashboard',
        children: <DashboardPermission employeeData={selectedEmployee} refreshData={refreshData}/>,
        icon: <DashboardOutlined />
      },
    ]
    
    if(roleData?.RoleTag === 'DepartmentManager'){
      tabs.push({
        label: 'System Data Permission',
        key: 'DepartmentManeger',
        children: <Manager employeeData={selectedEmployee}/>,
        icon: <TeamOutlined />
      })
    }else if(roleData?.RoleTag === 'DepartmentAdmin'){
      tabs.push({
        label: 'System Data Permission',
        key: 'DepartmentAdmin',
        children: <SystemData employeeData={selectedEmployee}/>,
        icon: <TeamOutlined />
      })
    }

    return tabs
  },[selectedEmployee])

  return (
    <Drawer
      title={<div>{selectedEmployee?.Firstname} {selectedEmployee?.Lastname}</div>}
      open={open}
      onClose={onClose}
      width={700}
      styles={{body: {paddingTop: 0, paddingBottom: 12}}}
    >
      <Tabs
        items={items}
        size='small'
      />
    </Drawer>
  )
}

export default EmployeeDetail