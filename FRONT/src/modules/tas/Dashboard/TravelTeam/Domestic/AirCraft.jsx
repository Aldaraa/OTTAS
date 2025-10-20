import { MHighchart } from 'components';
import React, { useCallback, useEffect, useMemo, useState } from 'react'

const options = {
  chart: {
    type: 'column'
  },
  title: {
    text: 'Air Craft',
    align: 'left'
  },
  legend: {
    align: 'right',
    alignColumns: true,
    layout:'vertical'
  },
  xAxis: {
    type: 'category',
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count of Passenger'
    }
  },
  tooltip: {
    valueSuffix: ' (passenger)'
  },
  plotOptions: {
    column: {
      stacking: 'normal',
      groupPadding: 0.001,
      pointPadding: 0.05,
      centerInCategory: true,
      dataLabels: {
        enabled: true,
      },
      relativeXValue: true,
      treshold: 1,
    },
  },
}

const daysOfWeek = [ 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday' ]
const modes = ['Airplane', 'Bus', 'Drive']

function AirCraft({data, date, picker}) {
  const [ mainData, setMainData ] = useState(null)
  const customizeData = (oldData, item) => {
    let newData = [...oldData]
    let inData = []
    let outData = []

    daysOfWeek.map((day) => {
      let dayData = item.WeekINInfo.find((item) => item.Dayname === day)
      inData.push(dayData ? dayData.Count : null)
    })
    daysOfWeek.map((day) => {
      let dayData = item.WeekOUTInfo.find((item) => item.Dayname === day)
      outData.push(dayData ? dayData.Count : null)
    })
    newData.push({name: `${item.Code}/IN`, data: inData, stack: item.Code})
    newData.push({name: `${item.Code}/OUT`, data: outData, stack: item.Code})
    
    return newData
  }

  useEffect(() => {
    if(data){
      let tmp = {}
      modes.map((mode) => {
        tmp[mode] = generateData(data[mode])
      })
      setMainData(tmp)
    }
  },[data])

  const generateData = useCallback((data) => {
    let names = []
    data?.map((item) => {
      if(names.length !== 0){
        if(!names.find((name) => item.Code === name)){
          names.push(item.Code)
        }
      }else{
        names.push(item.Code)
      }
    })
    let newData = []
    names.map((name) => {
      const filtered = data?.filter((item) => item.Code === name)
      let inItem = { name: `${name}/IN`, data: [], stack: name}
      let outItem = { name: `${name}/OUT`, data: [], stack: name}
      daysOfWeek.map((day) => {
        let in_day_data = null
        let out_day_data = null
        filtered.map((row, index) => {
          let dayOUTData = row.WeekOUTInfo.find((item) => item.Dayname === day)
          let dayINData = row.WeekINInfo.find((item) => item.Dayname === day)
          if(dayOUTData){
            out_day_data = dayOUTData.Count
          } 
          if(dayINData){
            in_day_data = dayINData.Count
          } 
        })
        inItem.data.push(in_day_data) 
        outItem.data.push(out_day_data) 
      })
      newData.push(inItem)
      newData.push(outItem)
    })
    return newData
  },[data])

  const fixedData = useMemo(() => {
    return mainData ? mainData[picker] : []
  },[mainData, picker])
  return (
    <MHighchart
      options={{
        ...options,
        xAxis: {
          categories: daysOfWeek,
        },
        series: fixedData,
        subtitle: {
          text: `${date.startOf('isoWeek').format('YYYY MMM DD')} — ${date.endOf('isoWeek').format('YYYY MMM DD')}`,
          align: 'left'
        },
      }}
    />
  )
}

export default AirCraft