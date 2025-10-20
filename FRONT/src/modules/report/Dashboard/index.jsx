import axios from 'axios'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import Report from './report'
import { AuthContext } from 'contexts'
import { Skeleton } from 'components'
import NumberFormat from 'utils/numberformat'
import { reportInstance } from 'utils/axios'

function Dashboard() {
  const [ loading, setLoading ] = useState(true)
  const [ employee, setEmployee ] = useState(null)
  const [ data, setData ] = useState(null)
  const [ employeeWeekData, setEmployeeWeekData ] = useState([])
  const [ reportTotalValues, setReportTotalValues ] =  useState([])
  const [ transport, setTransport ] = useState(null)
  const [ transportSeatWeek, setTransportSeatWeek ] = useState([])
  const [ room, setRoom ] = useState(null)
  const [ roomChartData, setRoomChartData ] = useState(null)

  const { action } = useContext(AuthContext)

  useEffect(() => {
    action.changeMenuKey('/report/dashboard')
    getData()
  },[])

  const getData = async () => {
    await reportInstance({
      method: 'get',
      url: 'tasreport/reporttemplate/dashboard'
    }).then((res) => {
      let tmp = []
      let ann = []
      let pieData = []
      res.data.ScheduleTypesData?.map((item, i) => {
        tmp.push({type: 'Active', value: item.ActiveCount, reportType: item.ScheduleType})
        tmp.push({type: 'Deactive', value: item.AllCount - item.ActiveCount < 0 ? 0 : item.AllCount - item.ActiveCount, reportType: item.ScheduleType})
        ann.push({
          type: 'text',
          content: `${NumberFormat(item.AllCount)}`,
          position: [i, item.AllCount + 0.5],
          style: {
            textAlign: 'center',
            fill: '#FF8F4E',
          },
          offsetY: -10,
        })
      })
      res.data.TemplateTypeData.map((item, i) => {
        const colors = ['#7BCEEE','#FF8F4E', '#63DAAB']
        pieData.push({value: item.AllCount, type: item.TemplateName, fillColor: colors[i]})
      })
      setData({...res.data, ScheduleTypesData: tmp, TemplateTypeData: pieData})
      setEmployee(res.data)
      setEmployeeWeekData(tmp)
      setReportTotalValues(ann)
    }).catch((err) => {

    })

    await axios({
      method: 'get',
      url: 'tas/dashboard/transport'
    }).then((res) => {
      let tmp = []
      res.data?.SeatWeek?.map((item, i) => {
        tmp.push({name: 'In Confirmed', value: item.InConfirmed, date: dayjs(item.date).format('MM-DD'), group: 'In'})
        tmp.push({name: 'In EmloyeeCount', value: item.InEmloyeeCount, date: dayjs(item.date).format('MM-DD'), group: 'In'})
        tmp.push({name: 'In OverBook', value: item.InOverBook, date: dayjs(item.date).format('MM-DD'), group: 'In'})
        tmp.push({name: 'In SeatBlock', value: item.InSeatBlock, date: dayjs(item.date).format('MM-DD'), group: 'In'})
        tmp.push({name: 'Out Confirmed', value: item.OutConfirmed, date: dayjs(item.date).format('MM-DD'), group: 'Out'})
        tmp.push({name: 'Out EmloyeeCount', value: item.OutEmloyeeCount, date: dayjs(item.date).format('MM-DD'), group: 'Out'})
        tmp.push({name: 'Out OverBook', value: item.OutOverBook, date: dayjs(item.date).format('MM-DD'), group: 'Out'})
        tmp.push({name: 'Out SeatBlock', value: item.OutSeatBlock, date: dayjs(item.date).format('MM-DD'), group: 'Out'})
      })
      setTransportSeatWeek(tmp)
      setTransport(res.data)
    }).catch((err) => {

    })

    await axios({
      method: 'get',
      url: 'tas/dashboard/room'
    }).then((res) => {
      let active = []
      let empty = []
      let virtual = []
      res.data.ActiveRoomWeek.map((item) => {
        active.push({date: dayjs(item.date).format('MM-DD'), room: item.ActiveRoomCount})
      })
      res.data.EmptyRoomWeek.map((item) => {
        empty.push({date: dayjs(item.date).format('MM-DD'), room: item.EmptyRoomCount})
      })
      res.data.VirtualRoomWeek.map((item) => {
        virtual.push({date: dayjs(item.date).format('MM-DD'), room: item.VirtualRoomEmployeeCount})
      })
      setRoomChartData({ActiveWeek: active, EmptyWeek: empty, VirtualWeek: virtual})
      setRoom(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  return (
    <div className='flex flex-col gap-5'>
      {
        loading ?
        <div className='flex flex-col gap-5'>
          <Skeleton className='h-[300px]'/>
          <Skeleton className='h-[300px]'/>
          <Skeleton className='h-[300px]'/>
        </div>
        :
        <div className='flex gap-5'>
          <div className='flex-1 grid grid-cols-12 gap-5 items-start'>
            <Report
              data={data}
              reportTotalValues={reportTotalValues}
            />
            
          </div>
          <div className='w-[300px] flex flex-col gap-5'>
          </div>
        </div>
      }
    </div>
  )
}

export default Dashboard