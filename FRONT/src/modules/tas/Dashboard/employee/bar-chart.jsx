import React, { useState } from 'react'
import { Column } from '@ant-design/plots'
import dayjs from 'dayjs';
import NumberFormat from 'utils/numberformat';

function BarChart({selectedLegend, ...restprops}) {
  const [ selected, setSelected ] = useState(null)
  const config = {
    isStack: true,
    xField: 'date',
    yField: 'value',
    seriesField: 'type',
    label: {
      content: (obj) => {
        if(obj.value){
          return NumberFormat(obj.value) 
        }
      },
      onclick: (e) => {
        
      },
      position: 'middle',
      layout: [
        {
          type: 'interval-adjust-position',
        },
        {
          type: 'interval-hide-overlap',
        },
        {
          type: 'adjust-color',
        },
      ],
    },
    yAxis: {
    },
    height: 200,
    columnStyle: (d) => {
      if(d.date === dayjs().format('MM-DD')){
        if(d.type === 'On Site Employees'){
          return {
            strokeOpacity:1,
            stroke: '#fb8c1e',
            lineWidth: 2,
            radius: [10,10,0,0]
          }
        }else{
          return {
            strokeOpacity:1,
            stroke: '#fb8c1e',
            lineWidth: 2,
          }
        }
      }else{
        if(d.type === 'On Site Employees' && d.value){
          return {
            lineWidth: 2,
            radius: d.type === 'On Site Employees' ? [10,10,0,0] : []
          }
        }else{
          return {
            lineWidth: 2,
            // radius: [10,10,0,0]
          }
        }
      }
    }
  };
  return (
    <Column 
      columnWidthRatio={0.3}
      {...config} 
      {...restprops} 
      color={(e) => e.type === 'On Site Employees' ? '#63DAAB' : '#7BCEEE'}
      legend={{
        selected: selectedLegend === 'on' ? {
          'Off Site Employees': false,
          'On Site Employees': true,
        }
        :
        selectedLegend === 'off' ?
        {
          'Off Site Employees': true,
          'On Site Employees': false,
        }
        :
        {
          'Off Site Employees': true,
          'On Site Employees': true,
        }
      }}
    />
  )
}

export default BarChart