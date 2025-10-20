import { Checkbox, Segmented } from 'antd'
import axios from 'axios'
import { MHighchart } from 'components';
import React, { useCallback, useEffect, useState } from 'react'

const options = {
  chart: {
    type: 'column'
  },
  xAxis: {
    type: 'category'
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count'
    },
    stackLabels: {
      enabled: true,
    }
  },
  plotOptions: {
    column: {
      groupPadding: 0.1,
      pointPadding: 0.1,
      centerInCategory: true,
      dataLabels: {
        enabled: true,
      }
    },
  },
  tooltip: {
    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
        '<td style="padding:0 5px"><b>{point.y}</b></td></tr>',
    footerFormat: '</table>',
    shared: true,
    useHTML: true
  },
}

const usageOptions = {
  chart: {
      type: 'column'
  },
  xAxis: {
      type: 'category'
  },
  legend: {
    enabled: false,
  },
  plotOptions: {
    column: {
      dataLabels: {
        enabled: true,
        format: '{point.y}%'
      }
    },
  },
  tooltip: {
      headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
          '<td style="padding:0 5px"><b>{point.y}%</b></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
  },
};

const items = [
  {
    value: 'Daily',
    label: 'Daily',
  },
  {
    value: 'Weekly',
    label: 'Weekly',
  },
  {
    value: 'Monthly',
    label: 'Monthly',
  },
]

const skippedIds = [1, 2031]
function CampUsage() {
  const [ data, setData ] = useState([])
  const [ type, setType ] = useState('Monthly')
  const [ usageUtil, setUsageUtil ] = useState({data: [], total: null, skippedTotal: null})
  const [ skippedData, setSkippedData ] = useState({data: [], total: 0})
  const [ isIncludedBigGer, setIsIncludedBigGer ] = useState(true)

  useEffect(() => {
    getWeekData(type)
  },[type])
    
  const getWeekData = useCallback((type) => {
    axios({
      method: 'get',
      url: `tas/dashboardsystemadmin/campusage/${type}`,
    }).then((res) => {
      let room = []
      let bed = []
      let occup = []
      let util = []
      let roomtotal = 0
      let bedtotal = 0
      let occuptotal = 0
      let utiltotal = 0
      let skippedTotal = 0
      let skippedItems = []
      const utilData = res.data.map((item) => item.Utilization)
      const minValue = Math.min.apply(null, utilData);
      const maxValue = Math.max.apply(null, utilData);
      res.data.map((item) => {
        roomtotal += item.RoomCount
        bedtotal += item.BedCount
        occuptotal += item.Occup
        utiltotal += item.Utilization || 0
        if(skippedIds.includes(item.CampId)){
          skippedItems.push(item)
        }else{
          skippedTotal += item.Utilization || 0
        }
        room.push({name: item.Camp, y: item.RoomCount})
        bed.push({name: item.Camp, y: item.BedCount})
        occup.push({name: item.Camp, y: item.Occup})
        if(minValue === (item.Utilization || 0)){
          util.push({name: item.Camp, y: item.Utilization || 0, color: 'red', id: item.CampId})
        }else if(maxValue === item.Utilization){
          util.push({name: item.Camp, y: item.Utilization || 0, color: 'green', id: item.CampId})
        }else{
          util.push({name: item.Camp, y: item.Utilization || 0, color: null, id: item.CampId})
        }
      })
      setUsageUtil({data: util, total: parseFloat(utiltotal / util.length).toFixed(1), skippedTotal: parseFloat(skippedTotal / (util.length - skippedItems.length)).toFixed(1)})
      setData([
        { name: 'Room', data: room, total: roomtotal},
        { name: 'Bed', data: bed, total: bedtotal },
        { name: 'Occupant', data: occup, total: occuptotal },
      ])
    })
  },[])

  const handleChange = useCallback((e) => {
    setType(e)
  },[])

  return (
    <div className='flex gap-5'>
      <div className='relative flex-1 rounded-ot overflow-hidden'>
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
            series: data,
            loading: true,
            subtitle: {
              text: `${data.map((item) => `${item.name}: <b>${Intl.NumberFormat().format(item.total)}</b> `)}`
            },
            title: {
              text: `${type} camp usage`
            }
          }}
        />
      </div>
      <div className='w-[40%] rounded-ot overflow-hidden relative'>
        <div className='absolute z-10 top-3 left-4 text-xs'>
          <Checkbox
            checked={isIncludedBigGer}
            onChange={(e) => setIsIncludedBigGer(e.target.checked)}
          >
            Include Big Ger (AVG)
          </Checkbox>
        </div>
        <MHighchart
          skippedData={skippedData}
          usageUtil={usageUtil}
          options={{
            ...usageOptions,
            series: [
              { name: 'Usage', data: usageUtil?.data},
            ],
            loading: true,
            title: {
              text: `Camp utilization`,
            },
            subtitle: {
              text: `${type} camp utilization`
            },
            yAxis: {
              min: 0,
              max: 100,
              title: {
                enabled: false
              },
              label: {
                format: '{value}%'
              },
              plotLines: [{
                color: 'red',
                type: 'spline',
                width: 2,
                value: isIncludedBigGer ? usageUtil?.total : usageUtil.skippedTotal,
                zIndex: 10,
                label: {
                    text: `AVG: ${isIncludedBigGer ? usageUtil?.total : usageUtil?.skippedTotal}%`,
                    align: 'right',
                    x: -20
                }
              }]
            },
          }}
        />
      </div>
    </div>
  )
}

export default CampUsage