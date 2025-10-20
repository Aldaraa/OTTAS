import { Tabs } from 'antd'
import axios from 'axios'
import React, { useCallback, useEffect, useState } from 'react'
import { useLoaderData, useParams } from 'react-router-dom'
import Admins from './Admins'
import Managers from './Managers'
import Supervisors from './Supervisors'
import CostCodes from './CostCodes'
import GroupConfig from './GroupConfig'

function DepartmentDetail() {
  const data = useLoaderData()
  const [ departmentData, setDepartmentData ] = useState(data)
  const [ groupData, setGroupData ] = useState([])
  const { departmentId } = useParams()

  useEffect(() => {
    getGroupData()
  },[])

  const getData = () => {
    axios.get(`tas/department/${departmentId}`).then((res) => {
      setDepartmentData(res.data)
    }).catch((err) => {

    })
  }

  const getGroupData = useCallback(() => {
    axios({
      method: 'get',
      url: `tas/DepartmentGroupConfig/${departmentId}`,
    }).then((res) => {
      setGroupData(res.data)
    }).catch((err) => {

    })
  },[departmentId])

  const items = [
    {
      key: '1',
      label: `Admins (${departmentData?.Admins.length})`,
      children: <Admins departmentData={departmentData} data={departmentData?.Admins} getData={getData}/>
    },
    {
      key: '2',
      label: `Managers (${departmentData?.Managers.length})`,
      children: <Managers departmentData={departmentData} data={departmentData?.Managers} getData={getData}/>
    },
    {
      key: '3',
      label: `Supervisors (${departmentData?.Supervisors.length})`,
      children: <Supervisors departmentData={departmentData} data={departmentData?.Supervisors} getData={getData}/>
    },
    {
      key: '4',
      label: `Cost Code (${departmentData?.CostCodes.length})`,
      children: <CostCodes departmentData={departmentData} data={departmentData?.CostCodes} getData={getData}/>
    },
    {
      key: '5',
      label: `Group Config (${groupData?.length})`,
      children: <GroupConfig departmentData={departmentData} data={groupData} getData={getGroupData}/>
    },
  ]

  return (
    <div>
      <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
        <div className='text-lg font-bold'>{departmentData?.Name}</div>
      </div>
      <div className='rounded-ot bg-white px-5 mb-5 shadow-md'>
        <Tabs
          items={items}
        />
      </div>
    </div>
  )
}

export default DepartmentDetail