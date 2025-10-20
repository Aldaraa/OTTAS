import axios from 'axios'
import { MHighchart } from 'components'
import React, { useEffect, useState } from 'react'
import Highcharts from 'highcharts'
import dayjs from 'dayjs'

const options = {
  chart: {
    type: 'column',
  },
  title: {
    text: 'On Site Visitors',
    align: 'left',
    style: { fontSize: '16px'}
  },
  xAxis: {
    type: 'category',
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count trophies'
    },
    stackLabels: {
      enabled: true
    }
  },
  tooltip: {
    pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
  },
  // legend: {
  //   layout: 'vertical',
  //   // itemStyle: {
  //   //   textOverflow: 
  //   // }
  //   // itemWidth: 100
  // },
  // accessibility: {
  //   enabled: false,
  // },
  plotOptions: {
    column: {
      stacking: 'normal',
      dataLabels: {
          enabled: true
      }
    }
  },
  series: [
    {
      name: 'BPL',
      data: [
        { name: '2024-05-05', y: 3 },
        { name: '2024-05-06', y: 5 },
        { name: '2024-05-07', y: 1 },
        { name: '2024-05-08', y: 13 }
      ]
    }, {
      name: 'FA Cup',
      data: [
        { name: '2024-05-05', y: 14 },
        { name: '2024-05-06', y: 8 },
        { name: '2024-05-07', y: 8 },
        { name: '2024-05-08', y: 12 }
      ]
    }, {
      name: 'CL',
      data: [
        { name: '2024-05-05', y: 0 },
        { name: '2024-05-06', y: 2 },
        { name: '2024-05-07', y: 6 },
        { name: '2024-05-08', y: 3 }
      ]
    }
  ]
}

const grouping = (result, c) => {
  let newArray = [...result]
  let oldData = newArray.find((item) => item.name === c.DepartmentName)
  if(!oldData){
    newArray.push({name: c.DepartmentName, data: [{name: dayjs(c.EventDate).format('YYYY-MM-DD'), y: c.Cnt}]})
  }
  else{
    newArray.map((item) => item.name === c.DepartmentName ? (c.Cnt > 0 ? item.data.push({ name: dayjs(c.EventDate).format('YYYY-MM-DD'), y: c.Cnt }) : item) : item)
  }
  return newArray
}

function OnSiteVisitor() {
  const [ data, setData ] = useState({series: [], drilldown: []})
  const [ startDate, setStartDate ] = useState(dayjs().subtract(1, 'w'))

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboarddataadmin/seatblockonsiteemployees?startDate=${startDate.format('YYYY-MM-DD')}`
    }).then((res) => {
      setData(res.data.sort((a, b) => dayjs(a.EventDate).format('YYYYMMDD') - dayjs(b.EventDate).format('YYYYMMDD')).reduce(grouping, []))
    })
  }
  return (
    <MHighchart
      options={{
        ...options,
        series: data
      }}
    />
  )
}

export default OnSiteVisitor