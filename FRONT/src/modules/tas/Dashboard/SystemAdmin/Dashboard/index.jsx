import axios from 'axios'
import React, { useEffect, useState } from 'react'
import Stat from './Stat';
import POB from './POB';
import TotalEmployee from './TotalEmployee';
import Division from './Division';
import CampUsage from './CampUsage';
import Location from './Location';
import { Skeleton } from 'components';

function SystemAdminDashboard() {
  const [ statData, setStatData ] = useState([])
  const [ totalEmployee, setTotalEmployee ] = useState([])
  const [ departmentData, setDepartmentData ] = useState(null)
  const [ totalEmpCount, setTotalEmpCount ] = useState(0)
  const [ loading, setLoading ] = useState(false)


  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios.all([
      `tas/dashboardsystemadmin/statdata`,
      'tas/dashboardsystemadmin/employeecount',
      ].map((endpoint) => axios.get(endpoint)))
    .then(axios.spread((stat, totalEmpData) => {
      let totalEmpNumber = 0
      let totalEmp = []
      totalEmpData.data.PeopleTypeEmployees.forEach((item) => { 
        totalEmpNumber += item.Count
        totalEmp.push({...item, name: item.Description, y: item.Count})
      })
      let depData = []
      let depDrill = []
      let totalDepEmp = 0
      totalEmpData.data.DepartmentEmployees.forEach((item) => {
        totalDepEmp += item.Count || 0
        depData.push({name: item.Description, y: item.Count, drilldown: item.Id})
        depDrill.push({
          name: item.Description,
          id: item.Id,
          data: item.PeopleTypeData?.map((resource) => ([resource.Description, resource.Count]))
        })
      })
      setStatData(stat.data)
      setTotalEmployee(totalEmp)
      setDepartmentData({series: depData, drilldown: depDrill, total: totalDepEmp})
      setTotalEmpCount(totalEmpNumber)
    })).catch(() => {

    }).then(() => setLoading(false))
  }

  return (
    <>
    {
      loading ?
      <>
        <div className='grid md:grid-cols-8 xl:grid-cols-7 2xl:grid-cols-8 gap-3'>
          {
            Array(8).fill(1).map((e, i) => (
              <Skeleton key={i} className='h-[80px] rounded-lg'/>
            ))
          }
        </div>
        <div className='grid grid-cols-12 gap-5'>
          <div className='col-span-6 rounded-ot overflow-hidden'>
            <Skeleton className='h-[400px] rounded-lg'/>
          </div>
          <div className='col-span-6 rounded-ot overflow-hidden'>
            <Skeleton className='h-[400px] rounded-lg'/>
          </div>
        </div>
        </>
        :
        <>
          <div className='grid grid-cols-12 gap-5'>
            <div className='col-span-12 rounded-ot overflow-hidden'>
              <Stat data={statData}/>
            </div>
            <div className='col-span-6 rounded-ot overflow-hidden'>
              <TotalEmployee data={totalEmployee} total={totalEmpCount}/>
            </div>
            <div className='col-span-6 rounded-ot overflow-hidden'>
              <Division data={departmentData} />
            </div>
            <div className='col-span-12 overflow-hidden'>
              <CampUsage />
            </div>
            <div className='col-span-12 rounded-ot overflow-hidden'>
              <Location/>
            </div>
            {/* <div className='col-span-12 rounded-ot overflow-hidden'>
              <POB />
            </div> */}
          </div>
        </>
    }
    </>
  )
}

export default SystemAdminDashboard