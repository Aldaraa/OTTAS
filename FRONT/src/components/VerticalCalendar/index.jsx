import { DatePicker, Popover } from 'antd';
import React, { useContext, useEffect, useState } from 'react'
import dayjs from 'dayjs'
import { BsAirplaneFill, BsDashLg, BsWindow } from 'react-icons/bs';
import { RightOutlined, LeftOutlined } from '@ant-design/icons';
import { Button, Tooltip } from 'components';
import hexToHSL from 'utils/hexToHSL';
import { twMerge } from 'tailwind-merge';
import { FaBus, FaCarSide } from 'react-icons/fa';
import isoWeek from 'dayjs/plugin/isoWeek'
import { AuthContext } from 'contexts';
import axios from 'axios';
import COLORS from 'constants/colors'
dayjs.extend(isoWeek)

const bgColor = {
  'thisMonth': 'bg-white border border-white hover:z-10 hover:shadow-option1',
  'notThisMonth': 'bg-gray-300 border border-gray-300 opacity-60'
}

function VerticalCalendar({employeeId, containerClass=''}) {
  const [ curDate, setCurDate ] = useState(dayjs())
  const [ isExpanded, setIsExpanded ] = useState(true)
  const [ employeeStatusDates, setEmployeeStatusDates ] = useState([])
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(state.flightCalendarDate){
      setCurDate(state.flightCalendarDate)
    }
  },[state.flightCalendarDate])

  useEffect(() => {
    if(state.userProfileData){
      getCalendarData(curDate)
    }
  },[state.userProfileData])

  const isToday = (day) => {
    if(dayjs(day).format('YYYY-MM-DD') === dayjs().format('YYYY-MM-DD')){
      return 'text-[#07f] font-extrabold'
    }
  }

  const renderTransportIcon = (event) => {
    if(event){
      if(event.TransportMode === 'Airplane'){
        return(
          <BsAirplaneFill 
            id="airplane" 
            size={14}
            color={COLORS.Directions[event?.Direction]?.color}
            style={{transform: COLORS.Directions[event?.Direction]?.rotate}}
          />
        ) 
      }else if(event.TransportMode === 'Bus'){
        return <FaBus 
          id="bus"
          size={14}
          color={COLORS.Directions[event?.Direction]?.color}
          className={event?.Direction === 'OUT' && '-scale-x-100'}
        />
      }else if(event.TransportMode === 'Drive'){
        return <FaCarSide
          id="drive"
          size={16}
          color={COLORS.Directions[event?.Direction]?.color}
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

  const getDate = () => {
    var calendar = []
    const startDate = dayjs(curDate).startOf('month').startOf('isoWeek');
    const endDate = dayjs(curDate).endOf('month').endOf('isoWeek');
    
    let day = startDate.subtract(1, 'day'); 
    
    while (day.isBefore(endDate, 'day')) {
      calendar.push(
        Array(7).fill(0).map(() => {
          day = day.add(1, 'day');
          return day.format('YYYY-MM-DD');
        })
      );
    }

    if(calendar.length > 0){
      return calendar.map((week, i) => (
        <tr id='calendar-row' className={twMerge('calendar-row text-center ease-in-out')} key={`custom-calendar-r${i}`}>
            {
              week.map((day, idx) => {
                const isSameMonth = dayjs(curDate).isSame(day, 'M')
                let currentEvent = employeeStatusDates?.find((item) => dayjs(item.EventDate).format('YYYY-MM-DD') === dayjs(day).format('YYYY-MM-DD'))
                return(
                  <td key={`cell-r${i}-${idx}`}>
                    <div 
                      className={twMerge(
                        `cursor-default px-1 py-1 flex-1 flex justify-between items-center h-[32px] relative group transition-all bg-red-50`,
                        isSameMonth ? bgColor.thisMonth : bgColor.notThisMonth
                      )}
                    >
                      <div className='flex flex-col mt-1'>
                        <div className={`leading-none text-[12px] relative ${isToday(day)}`}>
                          <div className={`absolute z-0 inset-0 ${isToday(day) ? 'animate-ping' : ''}`}>{dayjs(day).format('DD')}</div>
                          <span>{dayjs(day).format('DD')}</span>
                        </div>
                        <div className='text-[9px] text-gray-500'>{dayjs(day).format('MMM')}</div>
                      </div>
                      <Popover content={<div className='text-xs'>{currentEvent?.Schedule}</div>} trigger='click'>
                        <button type='button'>
                          {renderTransportIcon(currentEvent)}
                        </button>
                      </Popover>
                      <div>
                        <div className='flex gap-1'>
                          <div className='flex items-center justify-center' style={{background: currentEvent?.Color, width: '20px', height: '20px', borderRadius: '5px'}}>
                            {
                              currentEvent?.Color ?
                              <div className='text-[9px] font-semibold' style={{color: hexToHSL(currentEvent?.Color) > 60 ? 'black' : 'white'}}>
                                {currentEvent?.ShiftCode}
                              </div>
                              : 
                              <div className='text-[9px] font-semibold' style={{color: 'black'}}>
                                {currentEvent?.ShiftCode}
                              </div>
                            }
                          </div>
                        </div>
                      </div>
                    </div>
                  </td>
                )
              })
            }
        </tr>
      ))
    }
  }

  const getCalendarData = (date) => {
    axios({
      method: 'post',
      url: 'tas/employee/statusdates',
      data: {
        employeeId: employeeId,
        currentDate:  dayjs(date).startOf('M').format('YYYY-MM-DD'),
      }
    }).then((res) => {
      setEmployeeStatusDates(res.data.employeeStatusDates)
    }).catch((err) => {
      
    })
  }

  const handleClickUp = () => {
    const newDate = dayjs(curDate).subtract(1, 'month').format('YYYY-MM-DD')
    setCurDate(newDate)
    getCalendarData(newDate)
  }

  const handleClickDown = () => {
    const newDate = dayjs(curDate).add(1, 'month').format('YYYY-MM-DD')
    setCurDate(newDate)
    getCalendarData(newDate)
  }

  const handleChangeDate = (date) => {
    setCurDate(date)
    getCalendarData(date)
  }

  const handleClickExpand = (e) => {
    e.stopPropagation()
    setIsExpanded((prev) => !prev)
  }

  return (
    <div
      id='calendar' 
      className={twMerge(
        '2xl:flex-1 2xl:max-w-full max-w-[520px] overflow-hidden p-4 pt-3 bg-white rounded-ot shadow-md border transition-all relative z-10',
        isExpanded ? 'max-h-[300px]' : 'max-h-[50px]',
        containerClass
      )}
    >
      <div className='flex justify-between items-center mb-3'>
        <div className='flex gap-3 items-center text-xs text-gray-500'>
          <Button onClick={handleClickUp}>
            <LeftOutlined className='mx-2'/>
          </Button>
          <DatePicker
            size='small'
            picker='month'
            value={dayjs(curDate)}
            onChange={handleChangeDate}
            allowClear={false}
            format={'YYYY MMM'}
          />
          <Button onClick={handleClickDown}>
            <RightOutlined className='mx-2'/>
          </Button>
        </div>
        <div className='flex gap-3'>
          <Tooltip title={isExpanded ? 'Minimize' : 'Expand'}>
            <button className='hover:bg-black hover:bg-opacity-5 p-[5px] rounded-md' onClick={handleClickExpand}>
              {isExpanded ? <BsDashLg size={14}/> : <BsWindow size={14} />}
            </button>
          </Tooltip>
        </div>
      </div>
      <div className='overflow-hidden w-full'>
        <table className='w-full table-fixed'>
          <thead className='relative z-50 bg-white'>
            <tr>
              <th className='text-black rounded-tl'>Mon</th>
              <th className='text-black'>Tue</th>
              <th className='text-black'>Wed</th>
              <th className='text-black'>Thu</th>
              <th className='text-black'>Fri</th>
              <th className='text-primary2'>Sat</th>
              <th className='text-primary2 rounded-tr'>Sun</th>
            </tr>
          </thead>
          <tbody className='h-[70px] overflow-hidden'>
            {getDate(curDate)}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default VerticalCalendar