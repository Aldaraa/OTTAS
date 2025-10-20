import { MHighchart } from 'components'
import React, { useEffect, useMemo, useState } from 'react'
import axios from 'axios'

const options = {
  chart: {
    type: 'pie'
  },
  title: {
    text: 'Pack meal permanent access',
    align: 'left',
    style: {fontSize: '16px'}
  },
  // subtitle: {
  //   text: 'Custom animation of pie series',
  //   align: 'left'
  // },
  tooltip: {
    pointFormat: '<b>{point.percentage:.1f}%</b>'
  },
  accessibility: {
    point: {
      valueSuffix: '%'
    }
  },
  plotOptions: {
    pie: {
      allowPointSelect: true,
      borderWidth: 2,
      cursor: 'pointer',
      dataLabels: {
        enabled: true,
        format: '<b>{point.name}</b><br><span style="opacity: 0.5">{y}</span>',
        distance: 20
      },
      innerSize: '40%',
      borderRadius: 8,
      animation: {
        duration: 1000,
        easing: 'easeOutBounce'
      },
    },
  },
}

function PackMeal() {
  const [ data, setData ] = useState([])

  useEffect(() => {
    getData()
  },[])

  
  const getData = () => {
    axios({
      method: 'get',
      url: 'tas/dashboarddataadmin/packmeal'
    }).then((res) => {
      setData(res.data.map((item) => ({name: item.Description, y: item.Cnt})))
    })
  }

  return (
    <MHighchart
      options={{
        ...options,
        exporting: {
          showTable: false,
        },
        series: [{
          name: 'count',
          enableMouseTracking: true,
          animation: {
            duration: 1000
          },
          colorByPoint: true,
          data: data
        }]
      }}
    />
  )
}

export default PackMeal