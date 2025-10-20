import { MHighchart } from 'components'
import React from 'react'
import Highcharts from 'highcharts/highstock';
import exporting from "highcharts/modules/exporting";
import exportdata from "highcharts/modules/export-data";
exporting(Highcharts)
exportdata(Highcharts)

const options = {
  chart: {
    // type: 'column',
    alignTicks: false
  },
  title: {
    text: 'Resource type'
  },
  rangeSelector: {
    selected: 1,
    buttons: [
      {
        type: 'month',
        count: 1,
        text: 'Day',
        title: 'View 1 month',
        dataGrouping: {
          forced: true,
          units: [['day', [1]]]
        }
      }, 
      {
        type: 'month',
        count: 5,
        text: 'Week',
        title: 'View 1 year',
        dataGrouping: {
          forced: true,
          units: [['week', [1]]]
        }
      },
      {
        type: 'year',
        count: 1,
        text: 'Year',
        title: 'View 1 year',
        dataGrouping: {
          forced: true,
          units: [['month', [1]]]
        }
      },
    ]
  },
  // subtitle: {
  //   text: 'Resize the frame or click buttons to change appearance'
  // },
  legend: {
    align: 'right',
    verticalAlign: 'middle',
    layout: 'vertical'
  },
  legend: {
    enabled: true,
  },
  plotOptions: {
    series: { 
      showInNavigator: true
    }
  },
  series: [{
    type: 'column',
    name: 'Gender',
    data: [38, 51, 34]
  }],
}

function PeopleType({data}) {
  return (
    <div>
      <MHighchart 
        highcharts={Highcharts}
        options={{
          ...options,
          series: data || []
        }}
        constructorType={'stockChart'}
      />
    </div>
  )
}

export default PeopleType