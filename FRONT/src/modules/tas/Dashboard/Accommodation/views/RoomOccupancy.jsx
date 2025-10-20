import { Tabs } from 'antd';
import { MHighchart } from 'components'
import dayjs from 'dayjs';
import React, { useEffect, useMemo, useState } from 'react'

const options = {
  chart: {
      type: 'column'
  },
  title: {
      text: 'Total Occupants by Bed Type'
  },
  subtitle: {
    text: dayjs().format('YYYY MMM DD')
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
      enabled: true
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

const optionsAll = {
  chart: {
    type: 'column'
  },
  title: {
    text: 'Total Employees by Resource type',
    align: 'center'
  },
  subtitle: {
    text: dayjs().format('YYYY MMM DD'),
    align: 'center'
  },
  plotOptions: {
    column: {
      pointPadding: 0.1,
      groupPadding: 0.1,
      borderWidth: 0,
      dataLabels: {
        enabled: true,
      }
    }
  },
  legend: {
    enabled: true
  },
  tooltip: {
    headerFormat: '<span style="font-size: 15px">' +
      '{series.chart.options.countries.(point.key).name}' +
      '</span><br/>',
    pointFormat: '<span style="color:{point.color}">\u25CF</span> ' +
      '{series.name}: <b>{point.y}</b><br/>'
  },
  xAxis: {
    type: 'category',
  },
  yAxis: [{
    title: {
      text: 'Employees'
    },
  }],
}

function convertToS2(s1) {
  const roomTypes = {};
  if(s1){
    s1.forEach(entry => {
      const { OccupantType, RoomType, TotalOccupants } = entry;

      if (!roomTypes[RoomType]) {
        roomTypes[RoomType] = [];
      }
      roomTypes[RoomType].push({
        name: OccupantType,
        y: TotalOccupants,
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

const AllChart = ({data}) => {
  const [ series, setSeries ] = useState([])

  useEffect(() => {
    if(data){
      setSeries(data.series)
    }
  },[data])

  return(
    <MHighchart
      options={{
        ...optionsAll,
        // xAxis: data?.categories,
        series: series,
      }}
    />
  )
}

function RoomOccupancy({data}) {
  const convertedData = useMemo(() => {
    return convertToS2(data?.ByBedCountData)
  },[data])
  
  const fixData = (data) => {
    const array = {}
    data?.reduce((result ,item) => {
      const currentItem = {...item, Code: item.Code || 'Unknown'}
      if(!result){
        return {
          name: currentItem.Code,
          data: [{name: dayjs(currentItem.EventDate).format('YYYY-MM-DD'), y: currentItem.Count}]
        }
      }
      if(currentItem.Code === result.name){
        return {
          ...result,
          data: [
            ...result.data, 
            {name: dayjs(currentItem.EventDate).format('YYYY-MM-DD'), y: currentItem.Count}
          ]
        }
      }else{
        if(result.name){
          array[result.name] = {
            ...result,
            data: result.data.sort((a, b) => dayjs(a.name).diff(dayjs(b.name)))}
          return {
            name: currentItem.Code,
            data: [{name: dayjs(currentItem.EventDate).format('YYYY-MM-DD'), y: currentItem.Count}]
          }
        }
      }
    }, {})
    return array
  }

  const allData = useMemo(() => {
    const fixedData = fixData(data?.AllData)
    let keys = Object.keys(fixedData)
    const finallyData = keys.map((key) => fixedData[key])
    return {
      series: finallyData
    }
  },[data])

  const items = [
    {
      label: 'Occupants by Bed Type',
      key: 1,
      children: <MHighchart
        options={{
          ...options,
          series: convertedData.series,
        }}
      />
    },
    {
      label: 'Total Employees on Site',
      key: 2,
      children: <AllChart data={allData}/>
    },
  ]

  return (
    <div>
      <Tabs
        size='small'
        className='px-2'
        tabPosition='bottom'
        items={items}
      />
    </div>
  )
}

export default RoomOccupancy