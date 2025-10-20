import { MHighchart } from 'components';
import React from 'react'

const options = {
  title: {
    text: 'On Site employees',
  },
  chart:{
    type: 'column',
  },
  title: {
    text: 'Total employee by Division',
    align: 'left',
  },
  xAxis: {
    type: 'category'
  },
  yAxis: {
    allowDecimals: false,
    min: 0,
    stackLabels: {
      enabled: true,
    },
    title: {
      text: 'Count Employees'
    }
  },
  legend: {
    align: 'right',
    alignColumns: true,
    layout:'vertical',
    enabled: false,
  },
  // tooltip: {
  //   format: '<b>{key}</b><br/>{series.name}: {y}<br/> <span>employee</span>'
  // },
   tooltip: {
      headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
      pointFormat:
          '<td style="padding:0 5px"><b>{point.y}</b> <span>emlpoyee</span></td></tr>',
      footerFormat: '</table>',
      shared: true,
      useHTML: true
  },
  plotOptions: {
    column: {
      stacking: 'normal',
      dataLabels: {
        enabled: false,
      },
    }
  },
};

function Division({data}) {
  return (
    <div>
      <MHighchart
        options={{
          ...options,
          subtitle: {
            text: `Total: <b>${Intl.NumberFormat().format(data?.total)}</b>`,
            align: 'left',
          },
          series: [{
            name: 'Departments',
            data: data?.series || []
          }],
          drilldown: {
            activeAxisLabelStyle: {
              fontWeight: 400,
            },
            activeDataLabelStyle: {
              fontWeight: 400,
            },
            series: data?.drilldown
          }
        }}
      />
    </div>
  )
}

export default Division