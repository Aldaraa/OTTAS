import React from 'react'
import { Column } from '@ant-design/plots'
import dayjs from 'dayjs';

function MiniBarChart(props) {

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
    },
    // columnStyle: (obj) => {
    //   return {
    //     radius: [10, 10, 10, 10],
    //     lineWidth: 4,
    //   }
    // },
    style: {
      radiusTopLeft: 10,
      radiusTopRight: 10,
      maxWidth: 20,
    },

  };
  return (
    <Column 
      columnWidthRatio={0.3}
      {...config} 
      {...props}
    />
  )
}

export default MiniBarChart