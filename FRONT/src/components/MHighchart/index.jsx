import React, { forwardRef } from 'react'
import Highcharts from 'highcharts'
import HighchartsReact from 'highcharts-react-official'
import drilldown from "highcharts/modules/drilldown.js";
import wordcloud from "highcharts/modules/wordcloud";
import exporting from "highcharts/modules/exporting";
import exportdata from "highcharts/modules/export-data";
import stock from "highcharts/modules/stock";
import grouped_categories from 'highcharts-grouped-categories';
 
drilldown(Highcharts)
exporting(Highcharts)
exportdata(Highcharts)
wordcloud(Highcharts)
grouped_categories(Highcharts)
stock(Highcharts)

const MHighchart = React.memo( forwardRef(({options, ...restprops}, ref) => {
  return (
    <HighchartsReact
      ref={ref}
      highcharts={Highcharts}
      options={{...options, credits: {enabled: false}}}
      {...restprops}
    />
  )
}), (prev, next) => JSON.stringify(prev.options) === JSON.stringify(next.options))

export default MHighchart