import React, { useEffect, useState } from 'react'
import { Camp, Metric, NoRoom, Pob, RoomOccupancy, RoomType, Visitor } from './views'
import axios from 'axios'
import { Skeleton } from 'components'

function AccommodationDashboard() {
  const [ loading, setLoading ] = useState(true)
  const [ campTableData, setCampTableData ] = useState({
    CampInfoCampBedRoomData: [],
    CampInfoCampData: [],
    CampInfoRoomTypeData: []
  })
  const [ usageData, setUsageData ] = useState({
    Camp: [],
    Senior: [],
    Gender: []
  })
  const [ nonSiteData, setNonSiteData ] = useState(null)
  const [ roomOccupantsData, setRoomOccupantsData ] = useState(null)
  
  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios.all([
      `tas/dashboardaccomadmin/campinfo`,
      `tas/dashboardaccomadmin/usageinfo`,
      `tas/dashboardaccomadmin/nonsiteinfo`,
      `tas/dashboardaccomadmin/occupants`,
      ].map((endpoint) => axios.get(endpoint)))
    .then(axios.spread((campTable, usage, nonsite, occupants) => {
      setCampTableData(campTable.data)
      setUsageData(usage.data)
      setNonSiteData(nonsite.data)
      setRoomOccupantsData(occupants.data)
    })).catch(() => {

    }).then(() => setLoading(false))
  }
  return (
    <>
      {
        loading ?
        <>
          <div className='grid md:grid-cols-7 xl:grid-cols-7 3xl:grid-cols-6 gap-3'>
            {
              Array(7).fill(1).map((item, i) => (
                <Skeleton key={i} className='h-[80px] rounded-lg'/>
              ))
            }
          </div>
          <div className='flex gap-4'>
            <div className='flex-1 grid grid-cols-12 gap-4 items-start'>
              <Skeleton className='h-[400px] col-span-6 rounded-lg'/>
              <Skeleton className='h-[400px] col-span-6 rounded-lg'/>
            </div>
            <div className='w-[200px] flex flex-col gap-4'>
              <Skeleton className='h-[120px] rounded-lg'/>
              <Skeleton className='h-[120px] rounded-lg'/>
              <Skeleton className='h-[120px] rounded-lg'/>
            </div>
          </div>
        </>
        :
        <div className='grid grid-cols-12 gap-4 items-start'>
          <div className='col-span-12'>
            <Metric data={campTableData.CampInfoCampData}/>
          </div>
          <div className='col-span-12 flex items-start gap-4' >
            <div className='flex-1 grid grid-cols-12 gap-4'>
              <div className='col-span-12 2xl:col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
                <Camp data={campTableData.CampInfoCampBedRoomData}/>
              </div>
              <div className='col-span-12 2xl:col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
                <RoomType data={campTableData.CampInfoRoomTypeData}/>
              </div>
              <div className='col-span-12 2xl:col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
                <Pob/>
              </div>
              <div className='col-span-12 2xl:col-span-6 bg-white rounded-ot overflow-hidden shadow-md'>
                <RoomOccupancy data={roomOccupantsData}/>
              </div>
            </div>
            <div className='w-[200px] flex flex-col gap-4'>
              <div className='bg-white rounded-ot overflow-hidden shadow-md'>
                <Visitor/>
              </div>
              <div className='bg-white rounded-ot overflow-hidden shadow-md'>
                <NoRoom/>
              </div>
            </div>
          </div>
        </div>
      } 
    </>
  )
}

export default AccommodationDashboard