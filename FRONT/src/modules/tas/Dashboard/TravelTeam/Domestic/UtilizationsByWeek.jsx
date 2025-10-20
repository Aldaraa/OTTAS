import { MHighchart } from 'components'
import dayjs from 'dayjs'
import React, { useMemo } from 'react'

const options = {
  chart: {
    type: 'pie',
    // style: {
    //   border: '1px solid #e5e7eb',
    //   borderRadius: '10px',
    // }
  },
  title: {
    text: 'Utilizatoin by Week',
    align: 'left'
  },
  tooltip: {
    headerFormat: '',
    pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> ' +
    '{point.name}</b><br/>' +
    'Passenger Count: <b>{point.y}</b><br/>' +
    'Percent: <b>{point.percentage:.1f} %</b><br/>'
  },
  accessibility: {
    point: {
      valueSuffix: '%'
    }
  },
  plotOptions: {
    series: {
      innerSize: '40%',
      borderRadius: 8,
      allowPointSelect: true,
      borderWidth: 2,
      cursor: 'pointer',
      dataLabels: [{
        enabled: true,
        format: '<b>{point.name}</b><br>{point.y}',
        distance: 20
      }, {
          enabled: true,
          distance: -40,
          format: '{point.percentage:.1f}%',
          style: {
              fontSize: '14px',
              textOutline: 'none',
              opacity: 0.9
          },
          filter: {
              operator: '>',
              property: 'percentage',
              value: 4
          }
      }]
    }
  },
}

function UtilizationsByWeek({data}) {

  const customData = useMemo(() => {
    return data?.map((item) => ({
      ...item, 
      name: item.Carrier,
      y: item.Passengers
    })) || []
    return []
  },[data])

  return (
    <div>
      <MHighchart
        options={{
          ...options,
          series: [{
            name: 'Percentage',
            colorByPoint: true,
            data: customData,
          }],
          // subtitle: {
          //   // text: `${currentDate.startOf('isoWeek').format('MMM DD')} - ${currentDate.endOf('isoWeek').format('MMM DD')}`,
          //   text: `Visitor & Nons`,
          // },
        }}
      />
    </div>
  )
}

export default UtilizationsByWeek