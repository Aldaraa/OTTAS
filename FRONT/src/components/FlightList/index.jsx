import { SwapRightOutlined } from '@ant-design/icons'
import { Empty } from 'antd'
import axios from 'axios'
import { Tooltip } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import { BsAirplaneFill, BsDashLg, BsWindow } from 'react-icons/bs'
import { FaBus, FaCarSide } from 'react-icons/fa'
import { twMerge } from 'tailwind-merge'
import COLORS from 'constants/colors'

const renderTransportIcon = (event) => {
  if(event){
    if(event.TransportMode === 'Airplane'){
      return(
        <BsAirplaneFill 
          id="airplane" 
          size={14} 
          color={COLORS.Directions[event?.Direction].color}
          style={{transform: COLORS.Directions[event?.Direction].rotate}}
        />
      ) 
    }else if(event.TransportMode === 'Bus'){
      return <FaBus
        id="bus"
        size={14}
        color={COLORS.Directions[event?.Direction].color}
        className={event?.Direction === 'OUT' && '-scale-x-100'}
      />
    }else if(event.TransportMode === 'Drive'){
      return <FaCarSide
        id="drive"
        size={16}
        color={COLORS.Directions[event?.Direction].color}
        className={event?.Direction === 'OUT' && '-scale-x-100'}
      /> 
    }else{
      if(event.TransportMode){
        let transportMode = event.TransportMode.split('/')
        let transportDirection = event.Direction.split('/')
        if(transportMode.length > 0){
          return <>
            {renderTransportIcon({...event, Direction: transportDirection[0], TransportMode: transportMode[0]})}
            {renderTransportIcon({...event, Direction: transportDirection[1], TransportMode: transportMode[1]})}
          </>
        }
      }
    }
  }
}

function FlightList({employeeId, showHeader=true, className='', height=194, profileData}) {
  const [ data, setData ] = useState([])
  const [ isExpanded, setIsExpanded ] = useState(true)
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(state.userProfileData){
      getData()
    }
  }, [state.userProfileData])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employee/profiletransport/${state.userProfileData.Id}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    })
  }

  const handleClickExpand = (e) => {
    e.stopPropagation()
    setIsExpanded((prev) => !prev)
  }

  return (
    <div className={twMerge('flex-1 px-4 bg-white rounded-ot shadow-md border transition-all relative z-10', className, isExpanded ? `${showHeader ? 'pb-4' : 'pb-0'}` : 'pb-0',)}>
      {
        showHeader ?
        <div className='py-3 flex justify-between items-center '>
          <div className='font-semibold'>Transport information</div>
          <Tooltip title={isExpanded ? 'Minimize' : 'Expand'}>
            <button className='hover:bg-black hover:bg-opacity-5 p-[5px] rounded-md' onClick={handleClickExpand}>
              {isExpanded ? <BsDashLg size={14}/> : <BsWindow size={14} />}
            </button>
          </Tooltip>
        </div>
        : null
      }
      <div className={twMerge('pt-1 divide-y overflow-y-auto flex flex-col transition-all', showHeader && 'border-t', !isExpanded && 'p-0 border-none')} style={{height: isExpanded ? height : 0}}>
        { data.length > 0 ?
          data.map((item, i) => {
            const isCurrentSession = dayjs(item.InEventDate).startOf() < dayjs().startOf() && dayjs().startOf() < dayjs(item.OutEventDate).startOf()
            let firstIsOut = false 
            if(dayjs(item.InEventDate).format('YYYY-MM-DD') === dayjs(item.InEventTime).format('YYYY-MM-DD')){
              firstIsOut = dayjs(item.InEventDateTime).diff(item.OutEventDateTime) > 0
            }
            return(
              firstIsOut ?
              <div className={twMerge('flex flex-row gap-x-4 items-center rounded', isCurrentSession ? 'bg-green-200' : '')} key={`listData-${i}`}>
                <div className='py-1 flex-1 flex items-center gap-3 text-xs justify-between pr-1'>
                  <div className='flex items-center gap-1'>
                    {renderTransportIcon({TransportMode: item.OutTransportMode, Direction: item.OutDirection})}
                    <Tooltip title={dayjs(item.OutEventDate).format('dddd')}>
                      <div className='cursor-default'>{item.OutEventDate ? dayjs(item.OutEventDate).format('YYYY-MM-DD') : null}</div>
                    </Tooltip>
                  </div>
                  <div>{item.OutTransportCode}</div>
                  <div>{item.OutDescription}</div>
                </div>
                <SwapRightOutlined  className='block'/>
                <div className='py-1 flex-1 flex items-center gap-3 text-xs justify-between '>
                  <div className='flex items-center gap-1'>
                    {renderTransportIcon({TransportMode: item.InTransportMode, Direction: item.InDirection})}
                    <Tooltip title={dayjs(item.InEventDate).format('dddd')}>
                      <div>{item.InEventDate ? dayjs(item.InEventDate).format('YYYY-MM-DD') : null}</div>
                    </Tooltip>
                  </div>
                  <div>{item.InTransportCode}</div>
                  <div>{item.InDescription}</div>
                </div>
              </div>
              :
              <div className={twMerge('flex flex-row gap-x-4 items-center rounded', isCurrentSession ? 'bg-green-200' : '')} key={`listData-${i}`}>
                <div className='py-1 flex-1 flex items-center gap-3 text-xs justify-between '>
                  <div className='flex items-center gap-1'>
                    {renderTransportIcon({TransportMode: item.InTransportMode, Direction: item.InDirection})}
                    <Tooltip title={dayjs(item.InEventDate).format('dddd')}>
                      <div>{item.InEventDate ? dayjs(item.InEventDate).format('YYYY-MM-DD') : null}</div>
                    </Tooltip>
                  </div>
                  <div>{item.InTransportCode}</div>
                  <div>{item.InDescription}</div>
                </div>
                <SwapRightOutlined  className='block'/>
                <div className='py-1 flex-1 flex items-center gap-3 text-xs justify-between pr-1'>
                  <div className='flex items-center gap-1'>
                    {renderTransportIcon({TransportMode: item.OutTransportMode, Direction: item.OutDirection})}
                    <Tooltip title={dayjs(item.OutEventDate).format('dddd')}>
                      <div className='cursor-default'>{item.OutEventDate ? dayjs(item.OutEventDate).format('YYYY-MM-DD') : null}</div>
                    </Tooltip>
                  </div>
                  <div>{item.OutTransportCode}</div>
                  <div>{item.OutDescription}</div>
                </div>
              </div>
            )
          })
          :
          <Empty image={Empty.PRESENTED_IMAGE_SIMPLE}/>
        }
      </div>
    </div>
  )
}

export default FlightList