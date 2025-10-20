import React, { useEffect, useState } from 'react'
import { MHighchart } from 'components';
import axios from 'axios';
import dayjs from 'dayjs';

const options = {
  title: {
    text: 'My chart',
  },
  chart:{
    type: 'column',
  },
  title: {
    text: 'Off site employees',
    align: 'left',
    style: {fontSize: '16px'}
  },
  xAxis: {
    type: 'category'
  },
  yAxis: {
    allowDecimals: false,
    min: 0,
    title: {
      text: 'Employee count'
    }
  },
  legend: {
    align: 'right',
    alignColumns: true,
    layout:'vertical'
  },
  tooltip: {
    format: '<b>{key}</b><br/>{series.name}: {y}<br/>' + 'Total: {point.stackTotal}'
  },
  plotOptions: {
    series: {
      stacking: 'normal',
      dataLabels: {
        enabled: true,
      },
    }
  },
}

function OffSite() {
  const [ data, setData ] = useState({series: [], drilldown: []})

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: 'tas/dashboarddataadmin/onsiteemployees'
    }).then((res) => {
      const series = res.data.Departments.map((item) => {
        let newItem = {}
        newItem.name = item.Name
        newItem.data = item.DateData.map((row) => ({y: row.Cnt, name: dayjs(row.Date).format('YYYY-MM-DD'), drilldown: row.ChildKey}))
        return newItem
      })
      const drilldown = res.data.PeopleTypes.map((item) => {
        let newItem = {}
        newItem.id = item.ParentKey
        newItem.name = item.Name
        newItem.data = item.DateData.map((row) => ([row.PeopleTypeName ? row.PeopleTypeName : '', row.Cnt]))
        return newItem
      })
      setData({series, drilldown})
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

export default OffSite