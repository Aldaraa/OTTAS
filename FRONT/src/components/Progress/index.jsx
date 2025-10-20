import React from 'react'
import { Progress as AntProgress, ConfigProvider } from 'antd'
const Progress = (props) => {
  return (
    <ConfigProvider
      theme={{
        components: {
          Progress: {
            defaultColor: '#e57200',
            remainingColor: 'rgba(229, 114, 0, 0.25)',
            colorSuccess: '#1ea82d',
            colorText: "#ffffff",
          }
        }
      }}
    >
      <AntProgress {...props}/>
    </ConfigProvider>
  )
}

export default Progress