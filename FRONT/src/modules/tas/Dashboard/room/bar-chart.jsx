import React from 'react'
import { Column } from '@ant-design/plots'
import dayjs from 'dayjs';

function BarChart(props) {

  const config = {
    xField: 'date',
    yField: 'room',
    isStack: true,
    // label: {
    //   position: 'middle',

    // },
    yAxis:{
      grid: {
        line: {
          style: {
            strokeOpacity: 0,
          }
        }
      },
      // label: false,
    },
    xAxis:{
      grid: {
        line: {
          style: {
            strokeOpacity: 0,
          }
        }
      },
      // label: false
    },
    columnStyle: (obj) => {
      // return {
      //   radius: [10, 10, 0, 0],
      // }
    },

  };
  return (
    <Column 
      {...config} 
      {...props} 
    />
  )
}

export default BarChart