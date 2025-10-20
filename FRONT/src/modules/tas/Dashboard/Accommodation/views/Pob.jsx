import { DatePicker } from 'antd'
import axios from 'axios'
import { MHighchart } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'

const options = {
  chart: {
    type: 'spline'
  },
  title: {
    text: 'POB and Average Capacity',
    align: 'center',
  },
  xAxis: {
    type: 'datetime',
    dateTimeLabelFormats: {
      day: '%b %e %a'
    }
  },
  tooltip: {
    headerFormat: '<b>{point.x:%b %e %a}</b><br>',
    pointFormat: '{point.y:.0f} employees'
  },
  legend: {
    enabled: false,
  },
  plotOptions: {
    spline: {
      dataLabels: {
        enabled: true,
        zIndex: 11
      },
    },
  },
}

function Pob() {
  const [ data, setData ] = useState({PobDates: []})
  const [ currentDate, setCurrentDate ] = useState(dayjs())

  useEffect(() => {
    const getData = () => {
      axios({
        method: 'get',
        url: `tas/dashboardaccomadmin/pob?currentDate=${currentDate.format('YYYY-MM-DD')}`
      }).then((res) => {
        let sorted = res.data.PobDates.sort((a, b) => dayjs(a.EventDate).diff(dayjs(b.EventDate))) || []
  
        setData({
          ...res.data,
          // PobDates: sorted.map((item) => ({name: dayjs(item.EventDate).format('MMM DD DDD'), y: item.OnSiteEmployees}))
          PobDates: sorted.map((item) => ([new Date(item.EventDate).getTime(), item.OnSiteEmployees]))
        })
      }).catch((err) => {
  
      })
    }
    getData()
  },[currentDate])

  return (
    <div className='pt-2 relative'>
      <div className='absolute left-3 top-3 z-10'>
        <DatePicker.WeekPicker
          showWeek
          value={currentDate}
          onChange={(date) => setCurrentDate(date)}
        />
      </div>
      <MHighchart options={{
        ...options,
        subtitle: {
          text: `${currentDate.startOf('isoWeek').format('MMM DD')} - ${currentDate.endOf('isoWeek').format('MMM DD')}`
        },
        series: [
          {
            name: 'Pob',
            data: data.PobDates || []
          }
        ],
        yAxis: {
          title: {
            text: 'Number of Employees',
          },
          plotLines: [{
            color: 'red',
            width: 2,
            value: data?.POB || 0,
            zIndex: 10,
            label: {
                text: `POB: ${data?.POB}`,
                align: 'right',
                x: -20
            }
          }]
        }
      }}/>
    </div>
  )
}

export default Pob