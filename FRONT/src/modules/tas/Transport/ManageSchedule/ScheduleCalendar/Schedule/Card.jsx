import React, { memo } from 'react'
import { BsAirplaneFill } from 'react-icons/bs'
import { FaBus, FaCarSide } from 'react-icons/fa'
import COLORS from 'constants/colors'
import Progress from './Progress'
import { twMerge } from 'tailwind-merge'
import { Tooltip } from 'components'

const renderTransportIcon = (event) => {
  if(event){
    if(event.TransportMode === 'Airplane'){
      return(
        <Tooltip title={event?.Direction}>
          <BsAirplaneFill
            id="airplane" 
            size={14} 
            color={COLORS.Directions[event?.Direction].color}
            style={{transform: COLORS.Directions[event?.Direction].rotate}}
          />
        </Tooltip>
      ) 
    }else if(event.TransportMode === 'Bus'){
      return(
        <Tooltip title={event?.Direction}>
          <FaBus
            id="bus"
            size={14}
            color={COLORS.Directions[event?.Direction].color}
            className={event?.Direction === 'OUT' && '-scale-x-100'}
          />
        </Tooltip>
      ) 
    }else if(event.TransportMode === 'Drive'){
      return(
        <Tooltip>
          <FaCarSide
            id="drive"
            size={16}
            color={COLORS.Directions[event?.Direction].color}
            className={event?.Direction === 'OUT' && '-scale-x-100'}
          /> 
        </Tooltip>
      ) 
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

const cardBg = {
  "IN": "bg-[#CAEBC9] hover:bg-[#97e495]",
  "OUT": "bg-white hover:bg-gray-200",
  "EXTERNAL": "bg-purple-100 hover:bg-purple-200",
}

const bookingColor = {
  "IN": "border-green-500",
  "OUT": "border-gray-300",
  "EXTERNAL": "border-purple-300",
}

const Card = memo(({cellData, onClick}) => {
  return(
    <div onClick={() => onClick(cellData)} className={twMerge('rounded border overflow-hidden transition-all cursor-pointer', cardBg[cellData.Direction] )}>
      <div className='flex justify-between text-[11px] leading-none px-2 pt-2 text-gray-500'>
        <div>{cellData.Carrier}</div>
        <div>{cellData.Code}</div>
      </div>
      <div className='flex justify-between items-center px-2 my-1'>
        <div className='flex gap-3 items-center'>
          {renderTransportIcon({TransportMode: cellData.TransportMode, Direction: cellData.Direction})}
          <div className=' font-medium'>{cellData.FromLocationCode} - {cellData.ToLocationCode}</div>
        </div>
        <div className='text-xs'>{cellData.ETD.substr(0, 2) + ':' + cellData.ETD.substr(2)} - {cellData.ETA.substr(0, 2) + ':' + cellData.ETA.substr(2)}</div>
      </div>
      <div className={twMerge('border-t text-center py-1 text-[11px] leading-none', bookingColor[cellData.Direction])}>
        <div>{cellData.Booking || 0}/{cellData.Seats || 0}</div>
      </div>
      <Progress percent={((cellData.Booking || 0) / (cellData.Seats || 0) * 100) || 0}/>
    </div>
  ) 
})

export default Card