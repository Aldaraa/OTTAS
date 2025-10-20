import React, { useMemo } from 'react';
import { MHighchart } from 'components';

const options = {
  chart: {
    type: 'column',
  },
  title: {
    text: 'Total Request'
  },
  subtitle: {
      text: 'By Month',
      align: 'center',
  },
  // colors: [ "#2caffe", "#544fc5", "#00e272", "#fe6a35", "#6b8abc", "#d568fb", "#2ee0ca", "#fa4b42", "#feb56a", "#91e8e1" ],
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
          // pointPadding: 0.2,
          // borderWidth: 0
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
  let oldData = prevData.find((item) => item.name === current.documentType)
  // if(!oldData && current.count > 0){
  if(!oldData){
    if(current.value > 0){
      prevData.push({name: current.documentType, data: [{name: current.day, y: current.value}]})
    }else{ 

    }
    // prevData.push({name: current.documentType, data: [{name: current.day, y: current.count}], color: colors[result.length]})
  }else{
    prevData.map((item) => item.name === current.documentType ? (current.value > 0 ? item.data.push({name: current.day, y: current.value}) : item) : item)
    // prevData.map((item) => item.name === current.documentType ? item.data.push({name: current.day, y: current.value}) : item)
  }
  return prevData
}

function TotalReqByMonth({data=[]}) {

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

export default TotalReqByMonth
