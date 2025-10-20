import { MHighchart } from 'components';
import React, { useMemo } from 'react'

function RoomType({data}) {

  const options = useMemo(() => {
    let categories = [];
    let bed = []
    let room = []
    let owner = []
    let onsite = []
    data?.forEach((item) => {
      categories.push(item.RoomType)
      room.push(item.TotalRoomQTY)
      bed.push(item.TotalBedQTY)
      owner.push(item.Owned)
      onsite.push(item.OnSite)
    });
    return {
      chart: {
          type: 'column',
          // height: '100%'
          // height: (9 / 16 * 100) + '%'
      },
      title: {
          text: `Bed Count`,
          align: 'left'
      },
      subtitle: {
          text: 'Room and Bed data',
          align: 'left'
      },
      xAxis: {
          categories,
          crosshair: true,
          accessibility: {
              description: 'Countries'
          }
      },
      yAxis: {
          min: 0,
          crosshair: true,
          title: false
      },
      tooltip: {
        pointFormat: '<b>{point.y}</b> {series.name}<br/>'
      },
      plotOptions: {
          column: {
            pointWidth: 15,
            pointPadding: 0.1,
            groupPadding: 0.1
          },
          series: {
            dataLabels: {
                enabled: true,
                format: '{point.y}'
            }
        }
      },
      series: [
        {
          name: 'Room',
          data: room
        },
        {
          name: 'Bed',
          data: bed
        },
        {
          name: 'Owner',
          data: owner        },
        {
          name: 'On Site',
          data: onsite
        },
      ]
    }
  },[data])

  return (
    <div className='h-full'>
      <MHighchart options={{...options}}/>
    </div>
  )
}

export default RoomType