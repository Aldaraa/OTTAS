import { MHighchart } from 'components'
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
    text: 'Booking by Hotel',
    align: 'left'
  },
  tooltip: {
    enabled: false,
    // pointFormat: '{point.name}: <b>{point.y}</b><br/>' + 
    //   'Percent: <b>{point.percentage:.1f}%</b>'
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
          format: '{point.percentage:.0f}%',
          style: {
              fontSize: '14px',
              textOutline: 'none',
              opacity: 0.9
          },
          filter: {
              operator: '>',
              property: 'percentage',
              value: 10
          }
      }]
    }
  },
  series: [{
    colorByPoint: true,
    data: [{
      name: 'Customer Support',
      y: 21.3
    }, {
      name: 'Development',
      y: 18.7
    }, {
      name: 'Sales',
      y: 20.2
    }, {
      name: 'Marketing',
      y: 14.2
    }, {
      name: 'Other',
      y: 25.6
    }]
  }]
}

function Booking({startDate, endDate, data}) {

  const customData = useMemo(() => {
    return {
      subtitle: `${startDate?.format('YYYY MMM DD')} - ${endDate?.format('MMM DD')}`,
      data: data?.map((item) => ({ name: item.Hotel, y: item.Count })) || []
    }
  },[data])

  return (
    <MHighchart 
      options={{
        ...options,
        subtitle: {
          text: customData?.subtitle,
          align: 'left'
        },
        series: [{
          colorByPoint: true,
          data: customData?.data || []
        }]
      }}
    />
  )
}

export default Booking