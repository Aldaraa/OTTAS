import React from 'react'
import { Tooltip } from 'antd'

const CustomTooltip = ({children, overlayInnerStyle, ...restProps}) => {
  return (
    <Tooltip 
      color='white'
      overlayInnerStyle={overlayInnerStyle ? {color: 'black', ...overlayInnerStyle} : {color: 'black', fontSize: '12px'}}
      {...restProps}
    >
      {children}
    </Tooltip>
  )
}

export default CustomTooltip