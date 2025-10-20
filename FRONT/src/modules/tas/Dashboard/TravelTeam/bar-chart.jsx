import React from 'react'
import { Column } from '@ant-design/plots'

function BarChart(props) {

  const config = {
    xField: 'date',
    yField: 'value',
    isGroup: true,
    isStack: true,
    seriesField: 'name',
    groupField: 'group',
    label: {
      position: 'bottom',
      content: (obj) => {
        return obj.value ? obj.value : null
      },
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
    // columnStyle: (obj, i, d) => {
    //   // radius: () => {
    //   //   return [10, 10, 0, 0]
    //   // },
    //   return {
    //     radius: [10, 10, 0, 0]
    //   }
    // },
  };
  return (
    <Column {...config} {...props}/>
  )
}

export default BarChart