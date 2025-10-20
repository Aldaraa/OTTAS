import { LoadingOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Tooltip } from 'components'
import dayjs from 'dayjs'
import React, { useCallback, useState } from 'react'
import { FaCarSide } from 'react-icons/fa'
import { twMerge } from 'tailwind-merge'

function ButtonDrive({disabled, className='', name, onRecieve, searchValues, mode='morning'}) {
  const [ loading, setLoading ] = useState(false)
  
  const getData = useCallback(() => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/transportschedule/datedriveschedule',
      data: {
        ...searchValues,
        eventDate: dayjs(searchValues?.eventDate).format('YYYY-MM-DD'),
        morning: mode !== 'evening',
      }
    }).then((res) => {
      onRecieve(res.data, name)
    }).catch((err) => {

    }).finally(() => setLoading(false))
  },[searchValues])


  return (
    mode === 'morning' ?
    <Tooltip title={`Set ${mode} drive`}>
      <button
        className={twMerge('text-xs px-4 rounded-md py-[3px]' , disabled ? 'bg-[#FFE4B2]' : 'bg-[#FFD27F] hover:bg-[#FFE5B4]', className)}
        onClick={getData}
        disabled={disabled}
        type='button'
      >
        { 
          loading ?
          <LoadingOutlined style={{fontSize: '16px'}}/> :
          <FaCarSide className={twMerge('text-lg text-[#5A3E00]', disabled ? 'text-[#A8A8A8]' : 'text-[#5A3E00]')} />
        }
      </button>
    </Tooltip>
    : 
    <Tooltip title={`Set ${mode} drive`}>
      <button
        className={twMerge('text-xs px-4 rounded-md py-[3px]',  disabled ? 'bg-[#4A5B6A]' : 'bg-[#2D3E50] hover:bg-[#3A5068]', className)}
        onClick={getData}
        disabled={disabled}
        type='button'
      >
        { 
          loading ?
          <LoadingOutlined style={{fontSize: '16px', color: '#FCEAC7'}}/> :
          <FaCarSide className={twMerge('text-lg',  disabled ? 'text-[#B8B8B8]' : 'text-[#FCEAC7]')} />
        }
      </button>
    </Tooltip>
  )
}

export default ButtonDrive