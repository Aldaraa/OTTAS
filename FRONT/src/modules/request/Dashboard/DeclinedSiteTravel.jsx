import { MHighchart } from 'components'
import React, { useEffect } from 'react'

const options = {
  accessibility: {
    screenReaderSection: {
      beforeChartFormat: '<h5>{chartTitle}</h5>' +
        '<div>{chartSubtitle}</div>' +
        '<div>{chartLongdesc}</div>' +
        '<div>{viewTableButton}</div>'
    }
  },
  title: {
    text: 'Declined Site Travel Requests',
    align: 'left',
  },
  // tooltip: {
  //   headerFormat: '<span style="font-size: 16px"><b>{point.key}</b>' + '</span><br>'
  // }
}

function DeclinedSiteTravel({data}) {

  return (
    <div>
      <MHighchart
        options={{
          ...options,
          series: [{
            type: 'wordcloud',
            name: 'Request',
            data: data?.map((item) => ({
              name: item.Comment,
              weight: item.Count,
            })),
          }]
        }}
      />
    </div>
  )
}

export default DeclinedSiteTravel