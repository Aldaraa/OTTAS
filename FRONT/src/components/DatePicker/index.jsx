import React from 'react'
import { DatePicker as AntDatePiker } from 'antd'

function DatePicker(props) {
  return (
    <AntDatePiker {...props} format={["YYYY-MM-DD", "YYYY-MMM-DD", "YYYY MMM DD", "YYYYMMDD", "MM/DD/YYYY", "M/D/YYYY", "YYYY/MM/DD", "DD/MM/YYYY", "DD-MM-YYYY", "DD.MM.YYYY", "MMMM DD, YYYY", "DD MMMM YYYY", "MMM DD, YYYY", "DD-MMM-YYYY"]}/>
  )
}

export default DatePicker