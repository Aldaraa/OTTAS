import { MHighchart } from 'components'
import dayjs from 'dayjs'
import groupBy from 'lodash/groupBy'
import React, { useMemo } from 'react'

const options = {
  chart: {
    type: 'column'
  },
  title: {
    text: 'No Show Go Show',
    align: 'left'
  },
  xAxis: {
    crosshair: true,
    title: {
      text: 'Weeks and Direction'
    },
    type: 'category'
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count of Request #'
    }
  },
  legend: {
    enabled: true,
  },
  plotOptions: {
    column: {
      pointPadding: 0.2,
      borderWidth: 0,
      dataLabels: {
        enabled: true
      }
    }
  },
}

function NoShowGoShow({data}) {

  const customData = useMemo(() => {
    const groupedData = groupBy(data, ({Type}) => Type)
    const convertedData = Object.keys(groupedData).map((key) => ({
      name: key,
      data: groupedData[key].map((item) => ({
        ...item,
        name: `${dayjs().isoWeek(item.WeekNumber).format('wo')}`,
        y: item.Count 
      }))
    }))
    return convertedData
  },[data])

  return (
    <div>
      <MHighchart
        options={{
          ...options,
          series: customData
        }}
      />
    </div>
  )
}

export default NoShowGoShow