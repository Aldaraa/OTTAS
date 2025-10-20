import { DatePicker } from 'antd'
import axios from 'axios'
import { MHighchart } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'


const options = {
  chart: {
    type: 'pie',
    height: 250,
  },
  tooltip: {
      headerFormat: '',
      pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> ' +
      '{point.name}</b><br/>' +
      'Percent: <b>{point.percentage:.1f}%</b><br/>' +
      'Count: <b>{point.y}</b><br/>'
  },
  plotOptions: {
    pie: {
      allowPointSelect: true,
      cursor: 'pointer',
      dataLabels: {
        enabled: true,
        format: '<b>{point.name}</b><br>{point.y}',
        distance: -50,
        filter: {
            property: 'percentage',
            operator: '>',
            value: 4
        }
      }
    }
  },
}

function NoRoom() {
  const [ data, setData ] = useState(null)
  const [ currentDate, setCurrentDate ] = useState(dayjs())

  useEffect(() => {
    getData()
  },[currentDate])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboardaccomadmin/noroom?currentDate=${currentDate.format('YYYY-MM-DD')}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    })
  }

  return (
    <div>
      <div className='px-3 py-2 text-center font-semibold border-b'>No room Workers</div>
      <div className='relative grid grid-cols-1'>
        <div className='absolute left-3 top-3 z-10'>
          <DatePicker.WeekPicker
            size='small'
            showWeek
            value={currentDate}
            onChange={(date) => setCurrentDate(date)}
          />
        </div>
        <MHighchart
          options={{
            ...options,
            subtitle: {
              text: 'People Type',
              verticalAlign: 'bottom',
              floating: true,
            },
            title: {
              text: ''
            },
            colors: ['#47F9E3', '#D568FB'],
            series: [
              {
                colorByPoint: true,
                data: data?.PeopleType.map((item) => ({name: item.PeopleType, y: item.OnSiteEmployees})) || []
              }
            ],
          }}
        />
        <MHighchart
          options={{
            ...options,
            subtitle: {
              text: 'Room Type',
              verticalAlign: 'bottom',
              floating: true,
            },
            title: {
              text: ''
            },
            colors: ['#FE6A35', '#0FE273'],
            series: [
              {
                name: 'Percentage',
                colorByPoint: true,
                data: data?.RoomType.map((item) => ({name: item.RoomType, y: item.OnSiteEmployees})) || []
              }
            ],
          }}
        />
        <MHighchart
          options={{
            ...options,
            title: {
              text: ''
            },
            subtitle: {
              text: 'Gender',
              verticalAlign: 'bottom',
              floating: true,
            },
            series: [
              {
                name: 'Percentage',
                colorByPoint: true,
                data: data?.Gender.map((item) => ({name: item.Gender, y: item.OnSiteEmployees})) || []
              }
            ],
          }}
        />
      </div>
    </div>
    // <div className='basis-3/4 bg-white rounded-ot overflow-hidden'>
    //   <div className='px-3 py-2 flex justify-between border-b'>
    //     <div className='font-semibold'>No room Workers</div>
    //     <div className='top-3 z-10'>
    //       <DatePicker.WeekPicker
    //         size='small'
    //         showWeek
    //         value={currentDate}
    //         onChange={(date) => setCurrentDate(date)}
    //       />
    //     </div>
    //   </div>
    //   <div className='grid grid-cols-3 gap-4'>
    //     <div className='basis-1/3'>
    //       <MHighchart
    //         options={{
    //           ...options,
    //           subtitle: {
    //             text: 'People Type',
    //             verticalAlign: 'top',
    //             // floating: true,
    //           },
    //           title: {
    //             text: ''
    //           },
    //           colors: ['#47F9E3', '#D568FB'],
    //           series: [
    //             {
    //               colorByPoint: true,
    //               data: data?.PeopleType.map((item) => ({name: item.PeopleType, y: item.OnSiteEmployees})) || []
    //             }
    //           ],
    //         }}
    //       />
    //     </div>
    //     <div className='basis-1/3'>
    //       <MHighchart
    //         options={{
    //           ...options,
    //           subtitle: {
    //             text: 'Room Type',
    //             verticalAlign: 'top',
    //             // floating: true,
    //           },
    //           title: {
    //             text: ''
    //           },
    //           colors: ['#FE6A35', '#0FE273'],
    //           series: [
    //             {
    //               name: 'Percentage',
    //               colorByPoint: true,
    //               data: data?.RoomType.map((item) => ({name: item.RoomType, y: item.OnSiteEmployees})) || []
    //             }
    //           ],
    //         }}
    //       />
    //     </div>
    //     <MHighchart
    //       options={{
    //         ...options,
    //         title: {
    //           text: ''
    //         },
    //         subtitle: {
    //           text: 'Gender',
    //           verticalAlign: 'top',
    //           // floating: true,
    //         },
    //         series: [
    //           {
    //             name: 'Percentage',
    //             colorByPoint: true,
    //             data: data?.Gender.map((item) => ({name: item.Gender, y: item.OnSiteEmployees})) || []
    //           }
    //         ],
    //       }}
    //     />
    //   </div>
    // </div>
  )
}

export default NoRoom