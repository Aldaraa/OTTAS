import { MHighchart } from 'components'
import React, { useEffect, useMemo, useState } from 'react'
import Highcharts from 'highcharts/highstock';
import exporting from "highcharts/modules/exporting";
import exportdata from "highcharts/modules/export-data";
import axios from 'axios';
import dayjs from 'dayjs';
import { DatePicker } from 'antd';
exporting(Highcharts)
exportdata(Highcharts)

const options = {
  chart: {
    type: 'column'
  },
  title: {
    text: 'Location Data',
    align: 'center'
  },
  xAxis: {
    type: 'category'
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count of Employees'
    },
    stackLabels: {
      enabled: true
    }
  },
  // legend: {
  //   align: 'left',
  //   verticalAlign: 'top',
  // },
  tooltip: {
    headerFormat: '<b>{point.x}</b><br/>',
    pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
  },
  plotOptions: {
    column: {
      stacking: 'normal',
      dataLabels: {
        enabled: true
      }
    }
  },
}

function Location() {
  const [ data, setData ] = useState(null)
  const [ date, setDate ] = useState(dayjs())

  useEffect(() => {
    getData()
  },[date])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboardsystemadmin/locationdata/${`${date.startOf('isoWeek').format('YYYY-MM-DD')}`}`,
    }).then((res) => {
      setData(res.data)
    })
  }

  const chartData = useMemo(() => {
    let drilldownSeries = []
    let InData = []
    data?.InData.forEach((item) => {
      InData.push({name: dayjs(item.EventDate).format('MMM DD'), y: item.Count, drilldown: item.LocationData?.length > 0 ? item.Drilldown : false})
      let locData = []
      item.LocationData.map((loc) => {
        locData.push({name: loc.Location, y: loc.Count, drilldown: loc.StateData?.length > 0 ? loc.Drilldown : false})
        if(loc.StateData?.length > 0){
          drilldownSeries.push({name: 'State', id: loc.Drilldown, data: loc.StateData.map((state) => ({name: state.StateDescr, y: state.Count}))})
        }
      })
      drilldownSeries.push({
        name: `Location`,
        id: item.Drilldown,
        data: locData
      })
    })

    let OutData = []
    data?.OutData.forEach((item) => {
      OutData.push({name: dayjs(item.EventDate).format('MMM DD'), y: item.Count, drilldown: item.Drilldown})
      let locData = []
      item.LocationData.map((loc) => {
        locData.push({name: loc.Location, y: loc.Count, drilldown: loc.StateData?.length > 0 ? loc.Drilldown : false})
        if(loc.StateData?.length > 0){
          drilldownSeries.push({name: 'State', id: loc.Drilldown, data: loc.StateData.map((state) => ({name: state.StateDescr, y: state.Count}))})
        }
      })
      drilldownSeries.push({
        name: `Location`,
        id: item.Drilldown,
        data: locData
      })
    })
    
    return {
      series: [{name: 'IN', data: InData}, {name: 'OUT', data: OutData}],
      drilldown: {
        series: drilldownSeries
      }
    }
  },[data])
  
  return (
    <div className='relative'>
      <div className='absolute left-0 top-0 pt-2 pl-4 z-10'>
        <DatePicker.WeekPicker size='small' value={date} onChange={(e) => setDate(e)}/>
      </div>
      <MHighchart 
        options={{
          ...options,
          ...chartData,
        }}
      />
    </div>
  )
}

export default Location