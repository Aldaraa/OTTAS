import { DatePicker } from 'antd'
import axios from 'axios'
import { MHighchart } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useMemo, useState } from 'react'
import { TbCookieMan } from 'react-icons/tb'


const options = {
  chart: {
      type: 'pie',
      height: 250,
      style: {
        padding:0,
      }
  },
  title: {
    text: '',
    align: 'center',
    verticalAlign: 'bottom',
    margin: 0,
    floating: true,
    style: {
      marginTop: 0
    }
  },
  tooltip: {
    headerFormat: '',
    pointFormat: '<span style="color:{point.color}">\u25CF</span> <b> ' +
    '{point.name}</b><br/>' +
    'Percent: <b>{point.percentage:.0f}%</b><br/>' +
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

function Visitor() {
  const [ data, setData ] = useState({series: [], drilldown: []})
  const [ currentDate, setCurrentDate ] = useState(dayjs())

  useEffect(() => {
    getData()
  },[currentDate])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboardaccomadmin/nonsiteinfo?currentDate=${currentDate.format('YYYY-MM-DD')}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    })
  }

  const customData = useMemo(() => {
    let newData = data?.Employees?.map((item) => ({
      name: item.PeopleType,
      y: item.OnSiteEmployee
    })) || []
    return {data: newData}
  },[data])
  return (
    <div className='relative'>
      <div className='absolute top-3 left-3 z-10'>
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
          series: [
            {
              name: 'Percentage',
              colorByPoint: true,
              data: customData.data
            }
          ],
          subtitle: {
            // text: `${currentDate.startOf('isoWeek').format('MMM DD')} - ${currentDate.endOf('isoWeek').format('MMM DD')}`,
            text: `Visitor & Nons`,
            verticalAlign: 'bottom',
            floating: true,
          },
        }}
      />
    </div>
    // <div>
    //   <div className='px-3 py-2 flex justify-between border-b'>
    //     {/* <div className='font-semibold'>No room Workers</div> */}
    //     <div className='top-3 z-10'>
    //       <DatePicker.WeekPicker
    //         size='small'
    //         showWeek
    //         value={currentDate}
    //         onChange={(date) => setCurrentDate(date)}
    //       />
    //     </div>
    //   </div>
    //   <div className='relative'>
    //     <MHighchart
    //       options={{
    //         ...options,
    //         series: [
    //           {
    //             name: 'Percentage',
    //             colorByPoint: true,
    //             data: customData.data
    //           }
    //         ],
    //         subtitle: {
    //           // text: `${currentDate.startOf('isoWeek').format('MMM DD')} - ${currentDate.endOf('isoWeek').format('MMM DD')}`,
    //           text: `Visitor & Nons`,
    //           verticalAlign: 'top',
    //           // floating: true,
    //         },
    //       }}
    //     />
    //   </div>
    // </div>
  )
}

export default Visitor