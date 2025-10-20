import { LoadingOutlined } from '@ant-design/icons'
import React, { useMemo } from 'react'
import { twMerge } from 'tailwind-merge'
// import DisabledContext from 'antd/es/config-provider/DisabledContext'

function TButton({children, loading=false, type, icon=null, disabled=false, className='', htmlType='button', iconClass='', iconPosition='left', ref, ...restProps}) {

  // const isDisabled = useMemo(() => {
  //   if(loading){
  //     return true
  //   }
  //   if(disabled){
  //     return true
  //   }
  //   if(DisabledContext){
  //     return true
  //   }
  //   return false
  // }, [loading, disabled])

  const renderButton = () => {
    switch (type) {
      case 'primary': return (
        <button ref={ref} className={twMerge(`primary-button py-1 px-[15px]`, className)} disabled={loading ? loading : disabled} {...restProps} type={htmlType}>
          <div className='flex items-center'>
            {
              iconPosition === 'left' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'mr-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon} 
                </div>
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/>
                </div>
              )
            }
            {children}
            {
              iconPosition === 'right' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'ml-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon}
                </div> 
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/> 
                </div>
              )
            }
          </div>
        </button>
      )
      case 'success': return (
        <button ref={ref} className={twMerge(`success-button py-1 px-[15px]`, className)} disabled={loading ? loading : disabled} {...restProps} type={htmlType}>
          <div className='flex items-center'>
            {
              iconPosition === 'left' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'mr-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon} 
                </div>  
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/> 
                </div>
              ) 
            }
            {children}
            {
              iconPosition === 'right' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'ml-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon} 
                </div>  
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/> 
                </div>
              ) 
            }
          </div>
        </button>
      )
      case 'danger': return (
        <button ref={ref} className={twMerge(`danger-button py-1 px-[15px]`, className)} disabled={loading ? loading : disabled} {...restProps} type={htmlType}>
          <div className='flex items-center'>
            {
              iconPosition === 'left' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'mr-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon} 
                </div>  
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/> 
                </div>
              ) 
            }
            {children}
            {
              iconPosition === 'right' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'ml-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon} 
                </div>  
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/> 
                </div>
              ) 
            }
          </div>
        </button>
      )
      default: return (
        <button ref={ref} className={twMerge(`default-button py-1 px-[15px]`, className)} disabled={loading ? loading : disabled} {...restProps} type={htmlType}>
          <div className='flex items-center'>
            {
              iconPosition === 'left' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'mr-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon} 
                </div>  
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/> 
                </div>
              )
            }
            {children}
            {
              iconPosition === 'right' &&
              (
                icon ?
                <div className={`flex overflow-hidden w-[14px] ${children ? 'ml-2' : ''}`}>
                  {loading ? <LoadingOutlined/> : icon} 
                </div>  
                :
                <div className={`flex overflow-hidden transition-all ${loading ? 'w-[14px] mr-2' : 'w-0'}`}>
                  <LoadingOutlined/> 
                </div>
              )
            }
          </div>
        </button>
      )
    }
  }
  return renderButton()
}

export default TButton