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
    text: 'Total employee by Resource type',
    align: 'left'
  },
  xAxis: {
    crosshair: true,
    // title: {
      // text: 'Resource Type'
    // },
    type: 'category'
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count Employees'
    }
  },
  legend: {
    enabled: false,
    // align: 'right',
    // alignColumns: true,
    // layout:'vertical'
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

function TotalEmployee({data=[], total=0}) {
  return (
    <MHighchart
      options={{
        ...options,
        series: [{name: 'Employees', data: data}],
        subtitle: {
          align: 'left',
          text: `Total: <b>${Intl.NumberFormat().format(total)}</b>`,
      },
      }}
    />
  )
}

export default TotalEmployee