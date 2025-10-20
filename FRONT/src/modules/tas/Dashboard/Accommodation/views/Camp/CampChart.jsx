import { MHighchart } from 'components'
import React, { useMemo } from 'react'

const options = {
  chart: {
    type: 'column',
  },
  subtitle: {
      text: 'Room and Bed data',
      align: 'left'
  },
  yAxis: {
      min: 0,
      title: {
          text: 'Count'
      },
  },
  tooltip: {
    pointFormat: '<b>{point.y}</b> {series.name}<br/>'
  },
  plotOptions: {
    column: {
      pointPadding: 0.2,
      borderWidth: 0
    },
    series: {
      borderWidth: 0,
      dataLabels: {
        enabled: true,
      }
    }
  },
}

function CampChart({data}) {

  const customData = useMemo(() => {
    let categories = [];
    let bed = []
    let room = []
    let owner = []
    let onsite = []

    data?.RoomAndBed?.forEach((item) => {
      categories.push(item.RoomType)
      room.push(item.RoomQTY)
      bed.push(item.BedQTY)
      owner.push(item.Owner)
      onsite.push(item.OnSite)
    });
    return {
      categories,
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
        data: owner
      },
      {
        name: 'On Site',
        data: onsite
      },
    ]}
  },[data])

  return (
    <div className='flex-1'>
      <MHighchart
        options={{
          ...options,
          title: {
            text: `${data?.Camp} camp`,
            align: 'left'
          },
          xAxis: {
            categories: customData.categories,
            crosshair: true,
            accessibility: {
                description: 'Countries'
            }
          },
          series: customData.series || []
        }}
      />
    </div>
  )
}

export default CampChart