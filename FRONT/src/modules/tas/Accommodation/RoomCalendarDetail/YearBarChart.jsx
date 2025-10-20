import React, { useEffect, useState } from 'react'
import { Column } from '@ant-design/plots';

const YearBarChart = React.memo(({bedCount, data}) => {
  const [ calculatedData, setCalculatedData ] = useState([])
  useEffect(() => {
    if(data){
      let foo = data.map((item) => ({...item, percent: Math.floor((item.AvgCount / bedCount) * 100)}))
      setCalculatedData(foo)
    }
  },[data])

  const config = {
    data: calculatedData,
    xField: 'Month',
    yField: 'percent',
    label: {
      position: 'middle',
      style: {
        fill: '#ffffff',
      },
      content: (data) => {
        return  `${data.percent} %`
      }
    },
    color: '#e57200',
    xAxis: {
      label: {
        autoHide: true,
        autoRotate: false,
      },
    },
    meta: {
      type: {
        alias: '类别',
      },
      sales: {
        alias: '销售额',
      },
    },
  };
  return (
    <Column height={'100%'} width={'100%'} yAxis={{maxLimit: 100}} {...config} />
  );
},(prevProps, curProps) => {
  if(JSON.stringify(prevProps.data) !== JSON.stringify(curProps.data)){
    return false
  }
  return true
})

export default YearBarChart