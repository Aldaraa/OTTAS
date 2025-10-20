import { Segmented, Tabs } from 'antd'
import axios from 'axios'
import { MHighchart } from 'components';
import React, { useCallback, useEffect, useState } from 'react'

const options = {
  chart: {
      type: 'column'
  },
  title: {
      text: 'POB average'
  },
  xAxis: {
      type: 'category'
  },
  yAxis: {
      min: 0,
      title: {
          text: 'Number of Employees'
      },
      stackLabels: {
        enabled: true,
      }
  },
  legend: {
    align: 'right',
    alignColumns: true,
    layout:'vertical'
  },
  plotOptions: {
      column: {
          stacking: 'normal',
          dataLabels: {
              enabled: true,
              format: '{point.y}'
          }
      },
      series: {
          borderWidth: 0
      }
  },
  tooltip: {
      headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
          '<td style="padding:0 5px"><b>{point.y}</b> <span>emlpoyees</span></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
  },
};

function convertToS2(s1) {
  const roomTypes = {};
  if(s1){
    s1.forEach(entry => {
        const { Camp, RoomType, Count } = entry;

        if (!roomTypes[RoomType]) {
          roomTypes[RoomType] = [];
        }
        roomTypes[RoomType].push({
            name: Camp,
            y: Count,
        });
    });
  }

  const series = Object.keys(roomTypes).map(roomType => ({
      name: roomType,
      data: roomTypes[roomType]
  }));

  return {
      series,
  };
}

function POB() {
  const [ data, setData ] = useState([])
  const [ type, setType ] = useState('month')

  useEffect(() => {
    getWeekData()
  },[])

  const getWeekData = useCallback(() => {
    axios({
      method: 'get',
      url: 'tas/dashboardsystemadmin/camppob/weekly',
    }).then((res) => {
      const covertedData = convertToS2(res.data)
      setData(covertedData)
    })
  },[])

  const getWMonthData = useCallback(() => {
    axios({
      method: 'get',
      url: 'tas/dashboardsystemadmin/camppob/monthly',
    }).then((res) => {
      const covertedData = convertToS2(res.data)
      setData(covertedData)
    })
  },[])

  const items = [
    {
      value: 'week',
      label: 'Week',
    },
    {
      value: 'month',
      label: 'Month',
    },
  ]

  const handleChange = useCallback((e) => {
    setType(e)
    if(e==='week'){
      getWeekData()
    }else{
      getWMonthData()
    }
  },[])


  return (
    <div className='relative'>
      <Segmented
        className='absolute left-2 top-2 z-10'
        value={type}
        options={items}
        onChange={handleChange}
        size='small'
      />
      <MHighchart
        options={{
          ...options,
          series: data?.series || [],
        }}
      />
    </div>
  )
}

export default POB