import { MHighchart } from 'components'
import dayjs from 'dayjs';
import React, { useMemo } from 'react'

const options = {
  chart: {
    type: 'column',
  },
  xAxis: {
    crosshair: true,
    type: 'category',
    gridLineWidth: 1,
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count of Employees'
    },
  },
  tooltip: {
    // valueSuffix: ' (1000 MT)'
  },
  plotOptions: {
    series: {
      centerInCategory: true,
      pointPadding: 0.1,
      borderWidth: 0,
      groupPadding: 0.1,
      dataLabels: [
        {
          enabled: true,
          y: 0,
          allowOverlap: true,
        },
      ]
    }
  },
}
function PersonByWeek({startDate, picker, data}) {

  
  const subtitle = useMemo(() => {

  },[picker])
  return (
    <MHighchart 
      className='border'
      options={{
        ...options,
        subtitle: {
          text: `${startDate.format('YYYY MMM DD')} - ${startDate.format('YYYY MMM DD')}}`,
          align: 'left'
        },
        title: {
          text: `Person by Roster ${picker.charAt(0).toUpperCase() + picker.slice(1)}`,
          align: 'left'
        },
        series: {
          name: 'Roster Groups',
          data: data?.series,
        },
        drilldown: {
          series: data?.drilldown,
        }
      }}
    />
  )
}

export default PersonByWeek