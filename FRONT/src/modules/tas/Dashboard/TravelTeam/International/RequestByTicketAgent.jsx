import { MHighchart } from 'components'
import React, { useMemo } from 'react'

const options = {
  chart: {
    type: 'column'
  },
  title: {
    text: 'Request # By Ticket Agent',
    align: 'left'
  },
  xAxis: {
    crosshair: true,
    type: 'category',
    title: {
      text: 'Agent'
    }
  },
  yAxis: {
    min: 0,
    title: {
      text: 'Count of Request #'
    }
  },
  legend: {
    enabled: false,
  },
  plotOptions: {
    column: {
      pointPadding: 0.2,
      borderWidth: 0,
      dataLabels: {
        enabled: true
      }
    }
  },
}

function RequestByTicketAgent({startDate, endDate, data}) {

  const customData = useMemo(() => {
    return {
      subtitle: `${startDate?.format('YYYY MMM DD')} - ${endDate?.format('MMM DD')}`,
      data: data?.map((item) => ({ name: item.Agent, y: item.Count })) || []
    }
  },[data])

  return (
    <MHighchart
      options={{
        ...options,
        subtitle: {
          text: `${startDate?.format('YYYY MMM DD')} - ${endDate?.format('MMM DD')}`,
          align: 'left'
        },
        series: [{
          name: 'Request',
          data: customData?.data || []
        }],
      }}
    />
  )
}

export default RequestByTicketAgent