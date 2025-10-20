import { MHighchart } from 'components'
import React, { useMemo } from 'react'
const options = {
  chart: {
    type: 'column'
  },
  title: {
    text: 'Document Type Distribution over Days Away Count'
  },
  xAxis: {
    type: 'category',
    title: {
      text: 'Days Away'
    }
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count of Request'
    }
  },
  legend: {
    align: 'right',
    verticalAlign: 'bottom',
  },
  tooltip: {
    headerFormat: '<b>{point.name}</b><br/>',
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

const grouping = (result, current) => {
  let prevData = [...result]
  let oldData = prevData.find((item) => item.name === current.DocumentType)
  if(!oldData){
    if(current.Count > 0){
      prevData.push({name: current.DocumentType, data: [{name: current.DaysAway, y: current.Count}]})
    }
  }else{
    prevData.map((item) => item.name === current.DocumentType ? (current.Count > 0 ? item.data.push({name: current.DaysAway, y: current.Count}) : item) : item)
  }
  return prevData
}

function DaysAway({data=[]}) {

  const series = useMemo(() => {
    let returnValue = []
    const sorted = data.sort((a, b) => a.DaysAway - b.DaysAway)
    if(data?.length > 0){
      returnValue = sorted.reduce(grouping, [])
    }
    return {data: returnValue,
      categories: sorted.map((item) => item.DaysAway)
    }
  },[data])

  return (
    <div>
      <MHighchart
        options={{
          ...options,
          series: series?.data,
          // xAxis: {
          //   categories: series?.categories
          // }
        }}
      />
    </div>
  )
}

export default DaysAway