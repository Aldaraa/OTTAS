import { MHighchart } from 'components'
import React, { useEffect, useState } from 'react'
import axios from 'axios'
import { DatePicker } from 'antd'
import dayjs from 'dayjs'

const options = {
  chart: {
    type: 'pie'
  },
  title: {
    text: 'Employee registration',
    align: 'left',
    style: {fontSize: '16px'}
  },
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
function EmployeeStatus() {
  const [ currentDate, setCurrentDate ] = useState(dayjs().subtract(1, 'M'))
  const [ data, setData ] = useState([])

  useEffect(() => {
    getData()
  },[currentDate])

  
  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboarddataadmin/employeeregister?startDate=${currentDate.format('YYYY-MM-DD')}`
    }).then((res) => {
      setData(res.data.map((item) => ({name: item.Description, y: item.Cnt})))
    })
  }

  return (
    <div className='relative'>
      <div className='absolute top-3 right-12 z-10'>
        <div className='text-xs'>
          Start Date: <DatePicker allowClear={false} size='small' value={currentDate} onChange={(e) => setCurrentDate(e)}/>
        </div>
      </div>
      <MHighchart
        options={{
          ...options,
          series: [{
            enableMouseTracking: true,
            animation: {
              duration: 1000
            },
            colorByPoint: true,
            data: data
          }]
        }}
      />
    </div>
  )
}

export default EmployeeStatus