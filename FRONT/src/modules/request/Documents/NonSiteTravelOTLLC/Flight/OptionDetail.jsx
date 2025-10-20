import { LoadingOutlined } from '@ant-design/icons'
import { Badge, Drawer, Tag } from 'antd'
import axios from 'axios'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import { twMerge } from 'tailwind-merge'
import style from './style.module.css'
import { Tooltip } from 'components'
const duration = require('dayjs/plugin/duration')
dayjs.extend(duration)

const Duration = ({duration}) => {
  const hour = Math.floor(duration / 60)
  const minute = duration % 60
  return `${hour ? `${hour}h` : ''} ${minute ? `${minute}m` : ''}`
}

const Time = ({flight, direction, date, time}) => {
  const isSameDate = dayjs(flight.ArrivalDate).format('YYYY-MM-DD') === dayjs(flight.TransportDate).format('YYYY-MM-DD')
  return(
    <Tooltip title={`Timezome: ${direction === 'arr' ? flight.ArrivalTimeZone : flight.DepartureTimeZone}`}>
      <div className='flex flex-col items-center w-[45px] mr-[52px] cursor-default'>
        {
          !isSameDate ?
          <div className='text-xs text-blue-400'>
            {dayjs(date).format('MMM DD')}
          </div> : null
        }
        <div className='font-semibold leading-none'>{time.split(':')[0]}:{time.split(':')[1]}</div>
      </div>
    </Tooltip>
  )
}

function OptionDetail({data, open, onClose}) {
  const [ detailData, setDetailData ] = useState(null)
  const [ loading, setLoading ] = useState(true)

  useEffect(() => {
    if(data){
      getDetail()
    }
  },[data])

  const getDetail = () => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/requestnonsiteticketconfig/extractoption',
      data: {
        optionData: data?.OptionData
      }
    }).then((res) => {
      const timeDuration = dayjs.duration(res.data.TotalMinute, 'minutes')
      const hours = Math.floor(timeDuration.asHours());
      const minutes = String(timeDuration.minutes()).padStart(2, '0');

      setDetailData({
        ...res.data,
        TotalDuration: `${hours}h ${minutes}m`
      })
    }).catch((err) => {

    }).finally(() => setLoading(false))
  }
  const minuteToMillSec = (minute) => {
    return minute*60*60
  }

  return (
    <Drawer
      open={open}
      onClose={onClose}
      width={600}
      title={<div className='flex justify-between items-center'>
        <div className='flex flex-col gap-1'>
          <div>{Intl.NumberFormat().format(data?.Cost)} ₮</div>
          <Tag className='text-xs font-normal' color={detailData?.BookIngType === 'RT' ? 'blue' : 'purple'}>
            <span className='uppercase leading-none'>{detailData?.BookIngType === 'RT' ? 'Round Trip' : 'One way'}</span>
          </Tag>
        </div>
        <div className='flex flex-col gap-4'>
          <div className='text-gray-400 font-normal'>{detailData?.TotalDuration}</div>
        </div>
      </div>}
    >
      <div>
        {
          loading ? <LoadingOutlined/>
          :
          <>
            <div className='flex flex-col border-y'>
              {
                detailData?.FlightData.map((flight, i) => (
                  <Badge.Ribbon text={<span className='text-xs'>{flight.TicketStatus}</span>}>
                    <div className={`pl-16 flex flex-col gap-2 text-sm border-x py-4 relative hover:bg-blue-100 transition-all ${i%2 === 0 ? 'bg-gray-100' : ''}`} key={i}> 
                      <div className='absolute inset-y-0 left-2 flex items-center'>
                        {
                          flight.TransportDate ? 
                          <div className='p-1 rounded bg-blue-500 text-white w-[40px] flex flex-col items-center leading-none'>
                            <div className='text-xs'>{dayjs(flight.TransportDate).format('MMM')}</div>
                            <div>{dayjs(flight.TransportDate).format('DD')}</div>
                          </div>
                          : null
                        }
                      </div>
                      <div className='flex gap-3 items-end'>
                        <Time flight={flight} date={flight.TransportDate} time={flight.ETD} direction={'dep'}/>
                        <div className='font-semibold leading-none'>{flight.FromAirportCode} {flight.FromAirportName}</div>
                      </div>
                      <div className='flex gap-3 items-end leading-none py-2'>
                        <div className={twMerge('w-[55px] -ml-[5px] mr-[47px] h-4 text-xs text-secondary2 flex justify-center duration',style.duration)}>
                          <Duration duration={flight.TravelDurationMinutes}/>
                        </div>
                        <div className='flex gap-2 text-xs leading-none'>
                          <span className='text-secondary2'>{flight.AirlineName}</span>
                          <span className='text-secondary2'>{flight.AirlineCode}{flight.TransportNumber}</span>
                          <span className='text-secondary2'>{flight.ClassOfSeat}</span>
                        </div>
                      </div>
                      <div className='flex gap-3 items-end leading-none'>
                        <Time flight={flight} date={flight.ArrivalDate} time={flight.ETA} direction={'arr'}/>
                        <div className='font-semibold leading-none'>{flight.ToAirportCode} {flight.ToAirportName}</div>
                      </div>
                    </div>
                  </Badge.Ribbon>
                ))
              }
            </div>
            <div className='mt-5'>
              <div className='font-semibold py-2 border-b'>Ticket Rule</div>
              <div className='flex flex-wrap gap-10 pt-4'>
                {
                  detailData?.TicketRules.map((rule) => (
                    rule.AirlineCode &&  
                    <div className='flex flex-col gap-3'>
                      <div>{rule.AirlineCode}</div>
                      <div>
                        <div className='font-semibold'>Baggage</div>
                        {/* <div>Carry-on Baggage: <span className='font-semibold'>1 × 23kg</span></div> */}
                        <div>Carry-on Baggage: <span className='font-semibold'>{rule.CarryOnAllowance}</span></div>
                        <div>Checked Baggage: <span className='font-semibold'>{rule.LuggageAllowance}</span></div>
                      </div>
                      <div>
                        <div className='font-semibold'>Fare Rules</div>
                        <div>Refund: <span className='font-semibold'>{rule.RefundCost}</span></div>
                        <div>Change: <span className='font-semibold'>{rule.Changes}</span></div>
                        <div>No show: <span className='font-semibold'>{rule.NoShowCost}</span></div>
                      </div>
                    </div>
                  ))
                }
              </div>
            </div>
          </>
        }
      </div>
    </Drawer>
  )
}

export default OptionDetail