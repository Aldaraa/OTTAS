import React from 'react'
import { useNavigation } from 'react-router-dom'
// import { LoadingOT } from 'components'
import { LoadingOutlined } from '@ant-design/icons'

function GlobalLoadingIndicator() {
  const navigation = useNavigation()

  if(navigation.state === 'loading'){
    return (
      <div className='absolute inset-0 m-auto w-[100px] h-[100px] flex flex-col justify-center gap-1 items-center z-50 bg-gray-200 bg-opacity-25 backdrop-blur-[10px] rounded-ot shadow-card'>
        {/* <LoadingOT/> */}
        <LoadingOutlined style={{fontSize: 30, color: '#e57200'}}/>
        <span className='text-xs'>Please wait...</span>
      </div>
    )
  }
}

export default GlobalLoadingIndicator