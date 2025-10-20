import { Segmented } from 'antd'
import { Tooltip } from 'components'
import React, { useMemo, useState } from 'react'
const colors = ['#FFF0AA', '#FFC69A', '#FFB3A7', '#D4D9F8', '#E1C4E8', '#FFC3DC', '#C0E0F9', '#B2E6E2', '#B3E8B4', '#B8D4AA', '#F9B8B8', '#D9F8B8', '#FFF0AA', '#FFC69A', '#FFB3A7', '#D4D9F8', '#E1C4E8', '#FFC3DC', '#C0E0F9', '#B2E6E2', '#B3E8B4', '#B8D4AA', '#F9B8B8', '#D9F8B8']
function TransportInfo({data}) {
  const [ direction, setDirection ] = useState('IN')

  const filteredData = useMemo(() => {
    // return data?.filter((item) => item.Direction === direction)
    return []
  },[data, direction])

  const total = useMemo(() => {
    let totalIN = 0
    let totalOUT = 0
    data?.map((item) => {
      totalIN += item.IN
      totalOUT += item.OUT
    })

    return { IN: totalIN, OUT: totalOUT }
  }, [data])

  return (
    <div className='grid grid-cols-3 xl:grid-cols-6 2xl:grid-cols-10 items-start gap-3 mt-2 mb-4'>
      <div className='border rounded-md shadow-option overflow-hidden cursor-default'>
        <div className={'text-center py-1 bg-primary font-bold text-white'}>TOTAL</div>
        <div className='flex divide-x'>
          <div className='px-1 basis-1/2 flex justify-center items-center gap-1'>
            <Tooltip title='IN Total passengers'>
              <div className='text-lg font-semibold leading-none text-blue-500 py-1'>{total.IN}</div> 
            </Tooltip>
          </div>
          <div className='px-1 basis-1/2 flex justify-center items-center gap-1'>
            <Tooltip title='OUT Total passengers'>
              <div className='text-lg font-semibold leading-none text-gray-500'>{total.OUT}</div>
            </Tooltip>
          </div>
        </div>
      </div>
      {
        data?.map((item, i) => (
          <div className='border rounded-md shadow-option overflow-hidden cursor-default' key={i}>
            <div className={'text-center py-1 font-medium'} style={{backgroundColor: colors[i]}}>{item.Code}</div>
            <div className='flex divide-x'>
              <div className='px-1 basis-1/2 flex justify-center items-center gap-1'>
                <Tooltip title='IN passengers'>
                  <div className='text-lg font-semibold leading-none text-blue-500 py-1'>{item.IN}</div> 
                </Tooltip>
              </div>
              <div className='px-1 basis-1/2  flex justify-center items-center gap-1'>
                <Tooltip title='OUT passengers'>
                  <div className='text-lg font-semibold leading-none text-gray-500'>{item.OUT}</div>
                </Tooltip>
              </div>
            </div>
          </div>
        ))
      }
    </div>
  )
}

export default TransportInfo