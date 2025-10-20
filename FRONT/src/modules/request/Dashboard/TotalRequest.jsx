import React, { useMemo } from 'react';
import { MHighchart } from 'components';

const options = {
  chart: {
    type: 'column',
  },
  title: {
    text: 'Total Request'
  },
  colors: [ "#2caffe", "#544fc5", "#00e272", "#fe6a35", "#6b8abc", "#d568fb", "#2ee0ca", "#fa4b42", "#feb56a", "#91e8e1" ],
  yAxis: {
      min: 0,
      title: {
          text: 'Count'
      }
  },
  xAxis: {
    type: 'category',
  },
  tooltip: {
    pointFormat: '<b>{point.y}</b> {series.name}<br/>'
  },
  plotOptions: {
      column: {
          pointPadding: 0.2,
          borderWidth: 0,
          centerInCategory: true,
      },
      series: {
        borderWidth: 0,
        dataLabels: {
            enabled: true,
            format: '{point.y}'
        }
    }
  },
}

const colors = ["#fe6a35", "#2caffe", "#fa4b42", "#00e272", "gray"]
const grouping = (result, current) => {
  let prevData = [...result]
  let oldData = prevData.find((item) => item.name === current.CurrentStatus)
  // if(!oldData && current.count > 0){
  if(!oldData){
    if(current.count > 0){
      prevData.push({name: current.CurrentStatus, data: [{name: current.documentType, y: current.count}], color: colors[result.length]})
    }
  }else{
    prevData.map((item) => item.name === current.CurrentStatus ? (current.count > 0 ? item.data.push({name: current.documentType, y: current.count}) : item) : item)
    // prevData.map((item) => item.name === current.CurrentStatus ? item.data.push({name: current.documentType, y: current.count}) : item)
  }
  return prevData
}

function TotalRequest({data=[]}) {

  const series = useMemo(() => {
    let returnData = []
    if(data.length > 0){
      returnData = data.reduce(grouping, [])
    }
    return returnData
  },[data])

  return (
    <div>
      <MHighchart
        options={{
          ...options, 
          series: series
        }}
      />
    </div>
  )
}

export default TotalRequest
