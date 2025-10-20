import { InfoCircleOutlined, LoadingOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Tooltip } from 'components'
import React, { useCallback, useState } from 'react'

function SeatTooltip({id}) {
  const [ open, setOpen ] = useState(false)
  const [ loading, setLoading ] = useState(false)
  const [ data, setData ] = useState(null)
  const onOpenChange = useCallback ((isOpen) => {
    if(isOpen){
      setLoading(true)
      axios.get(`tas/transportschedule/seatinfo/${id}`).then((res) => {
        setData(res.data)
        setOpen(true)
      }).catch(() => {

      }).finally(() => setLoading(false))
    }else{
      setOpen(false)
    }
  }, [id])

  return (
    <Tooltip
      open={open}
      trigger='click'
      onOpenChange={onOpenChange}
      title={<div>
          <div className='flex gap-2'>
            <span className='text-gray-500'>Seats:</span> {data?.Seats}
          </div>
          {/* <div className='flex gap-2'>
            <span className='text-gray-500'>Booked:</span> {data?.BookedCount}
          </div> */}
          <div className='flex gap-2'>
            <span className='text-gray-500'>Available Seats:</span>
            <span>{data?.AvailableSeatCount}</span>
          </div>
        </div>
      }
    >
      {
        loading ?
        <LoadingOutlined className='px-[11px] -mx-[11px]'/>
        :
        <InfoCircleOutlined disabled={false} className='px-[11px] -mx-[11px]'/>
      }
    </Tooltip>
  )
}

export default SeatTooltip