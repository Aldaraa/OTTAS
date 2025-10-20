import { ConfigProvider, DatePicker } from 'antd'
import axios from 'axios'
import { MHighchart } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'

const options = {
  chart: {
    type: 'pie',
    marginTop: 50,
  },
  title: {
    text: 'Profile changes',
    align: 'left',
    style: {fontSize: '16px'}
  },
  legend: {
    align: 'right',
    alignColumns: true,
    layout: 'vertical'
  },
  plotOptions: {
    series: {
      borderRadius: 5,
      showInLegend: false,
      dataLabels: [
        {
          enabled: true,
          distance: 10,
          format: '{point.name}',
          style: {
            fontWeight: 400,
          },
          padding: 0,
        }, 
        {
          enabled: true,
          distance: '-30%',
          filter: {
            property: 'percentage',
            operator: '>',
            value: 0
          },
          format: '{point.percentage:.1f}%',
          style: {
            fontWeight: '400',
            fontSize: '12px',
            textOutline: 'none'
          }
        }
      ]
    }
  },
  legend: {
    enabled: true,
  },
  tooltip: {
    // headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
    headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
    pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b><br/>'
  },
}

const disabledDate = (current, { from }) => {
  if (from) {
    return Math.abs(current.diff(from, 'days')) >= 7;
  }
  return false;
};

function ProfileChanges() {
  const [ dateRange, setDateRange ] = useState([dayjs().subtract(1, 'w'), dayjs()])
  const [ data, setData ] = useState({series: [], drilldown: []})
  useEffect(() => {
    getData()
  },[dateRange])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/dashboarddataadmin/profilechangedata?startDate=${dateRange[0].format('YYYY-MM-DD')}&endDate=${dateRange[1].format('YYYY-MM-DD')}`
    }).then((res) => {
      const series = res.data.EditFields.map((item) => ({
        ...item, name: item.ColumnName, y: item.Cnt
      }))
      setData({series})
    })
  }
  return (
    <div className='relative'>
      <div className='absolute top-3 right-11 z-10'>
        <ConfigProvider>
          <DatePicker.RangePicker
            allowClear={false}
            size='small'
            value={dateRange}
            disabledDate={disabledDate}
            onChange={(e) => setDateRange(e)}
          />
        </ConfigProvider>
      </div>
      <MHighchart
        options={{
          ...options,
          series: {
            name: 'Changes',
            data: data.series,
          },
        }}
      />
    </div>
  )
}

export default ProfileChanges