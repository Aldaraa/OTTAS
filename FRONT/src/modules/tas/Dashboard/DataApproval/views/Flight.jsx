import axios from 'axios'
import { MHighchart } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import { TbCookieMan } from 'react-icons/tb'

const options = {
  chart: {
    type: 'column'
  },
  title: {
    align: 'left',
    text: 'Transport'
  },
  // subtitle: {
  //   align: 'left',
  //   text: 'Click the columns to view versions. Source: <a href="http://statcounter.com" target="_blank">statcounter.com</a>'
  // },
  // accessibility: {
  //   announceNewData: {
  //     enabled: true
  //   }
  // },
  xAxis: {
    type: 'category'
  },
  yAxis: {
    title: {
      text: 'Employees'
    }

  },
  legend: {
    enabled: true
  },
  plotOptions: {
    series: {
      borderWidth: 0,
      distance: 20,
      dataLabels: {
        enabled: true,
        format: '{point.y}'
      }
    }
  },

  tooltip: {
    headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
    pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b> <br/>'
  },
}

function Flight() {
  const [ data, setData ] = useState({series: [], drilldown: []})

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: 'tas/dashboarddataadmin/transportdata'
    }).then((res) => {
      const series = res.data.TransportData.map((item) => {
        return {
          name: item.Type,
          data: item.DateData.map((row) => ({
            name: row.Datekey,
            y: row.Cnt,
            drilldown: row.Key
          }))
        }
      })
      const drilldown = res.data.Details.map((item) => ({
        name: item.key.split('-')[0],
        id: item.key,
        data: item.Data.map((row) => ([row.Name, row.Cnt]))
      }))
      setData({series, drilldown})
    }).catch((err) => {

    })
  }
  return (
    <MHighchart
      options={{
        ...options,
        series: data.series,
        drilldown: {
          activeAxisLabelStyle: {
            fontWeight: 400,
          },
          activeDataLabelStyle: {
            fontWeight: 400,
          },
          series: data.drilldown
        }
      }}
    />
  )
}

export default Flight