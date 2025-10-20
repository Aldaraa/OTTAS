import { DatePicker, Popover, Segmented } from 'antd';
import React, { useState } from 'react'
import dayjs from 'dayjs'
import { BsAirplaneFill } from 'react-icons/bs';
import { CalendarOutlined, BarsOutlined, RightOutlined, LeftOutlined } from '@ant-design/icons';
import { Button } from 'components';
import hexToHSL from 'utils/hexToHSL';
import { twMerge } from 'tailwind-merge';
import { FaBus, FaCarSide } from 'react-icons/fa';

function CustomMultiCalendar({data, listData, onChange, currentDate, containerClass='', ...restProps}) {
  const [ viewMode, setViewMode ] = useState('calendar')
  const [ curDate, setCurDate ] = useState(currentDate ? currentDate : dayjs())

  const pickCellBg = (day) => {
    /// current month
    if(dayjs(day).format('YYYY-MM') === dayjs(curDate).format('YYYY-MM')){
      return 'bg-blue-50 hover:scale-125'
    }
    else if(dayjs(day).format('YYYY-MM') === dayjs(curDate).add(1, 'M').format('YYYY-MM')){
      return 'bg-orange-50 hover:scale-125'
    }
    /// other month
    else{
      return 'border-none opacity-70'
    }
  }

  const isToday = (day) => {
    if(dayjs(day).format('YYYY-MM-DD') === dayjs().format('YYYY-MM-DD')){
      return 'text-[#07f] font-extrabold'
    }
  }

  const isDisabledDays = (day) => {
    if((dayjs(day).format('YYYY-MM') !== dayjs(currentDate).format('YYYY-MM')) && (dayjs(day).format('YYYY-MM') !== dayjs(currentDate).add(1, 'month').format('YYYY-MM'))){
      return 'bg-gray-400 bg-opacity-50'
    }
  }

  const renderTransportIcon = (event) => {
    if(event){
      if(event.TransportMode === 'Airplane'){
        return(
          <BsAirplaneFill 
            id="airplane" 
            size={14} 
            color={event?.Direction === 'IN' ? '1a66ff' : '#555'} 
            style={{transform: event?.Direction === 'IN' ? 'rotate(135deg)' : 'rotate(45deg)'}}
          />
        ) 
      }else if(event.TransportMode === 'Bus'){
        return <FaBus 
          id="bus"
          size={14}
          color={event?.Direction === 'IN' ? '1a66ff' : '#555'}
          className={event?.Direction === 'OUT' && '-scale-x-100'}
        />
      }else if(event.TransportMode === 'Drive'){
        return <FaCarSide 
          id="drive"
          size={16}
          color={event?.Direction === 'IN' ? '1a66ff' : '#555'}
          className={event?.Direction === 'OUT' && '-scale-x-100'}
        /> 
      }
    }
  }

  const getDate = () => {
    var calendar = []

    const startDate = dayjs(currentDate).startOf('month').startOf('isoWeek');
    const endDate = dayjs(currentDate).endOf('month').endOf('isoWeek');
    
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
        <tr className='text-center' key={`custom-calendar-r${i}`}>
          {
            week.map((day, idx) => {
              let currentEvent = data?.find((item) => dayjs(item.EventDate).format('YYYY-MM-DD') === dayjs(day).format('YYYY-MM-DD'))
              let currentFlightEvent = listData?.find((item) => dayjs(item.EventDate).format('YYYY-MM-DD') === dayjs(day).format('YYYY-MM-DD'))
              return(
                <td key={`cell-r${i}-${idx}`}>
                  <div className={twMerge(`cursor-default px-1 py-1 w-full flex-1 flex justify-between items-center h-[32px] group hover:border border-gray-600 transition-all`,pickCellBg(day), isDisabledDays(day))}>
                    <div className='flex flex-col mt-1'>
                      <div className={`leading-none relative ${isToday(day)}`}>
                        <div className={`absolute z-0  ${isToday(day) ? 'animate-ping' : ''}`}>{dayjs(day).format('DD')}</div>
                        <span>{dayjs(day).format('DD')}</span>
                      </div>
                      <div className='text-[9px] text-black opacity-0 group-hover:opacity-100 transition-all'>{dayjs(day).format('MMM')}</div>
                    </div>
                    <Popover content={<div className='text-xs'>{currentFlightEvent?.Description}</div>} trigger='click'>
                      <button type='button'>
                        {renderTransportIcon(currentFlightEvent)}
                      </button>
                    </Popover>
                    <div>
                      <div className='flex gap-1'>
                        <div className='flex items-center justify-center' style={{background: currentEvent?.Color, width: '20px', height: '20px', borderRadius: '5px'}}>
                          <div className='text-[11px] drop-shadow-[1px_0px_10px_rgb(0,0,0)] font-normal' style={{color: hexToHSL(currentEvent?.Color) > 60 ? 'black' : 'white'}}>
                            {currentEvent?.ShiftCode}
                          </div>
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

  const handleChangePrevDate = (date) => {
    setCurDate(dayjs(date))
    onChange(date)
  }

  const handleChangeLastDate = (date) => {
    setCurDate(dayjs(date).subtract(1, 'month'))
    onChange(dayjs(date).subtract(1, 'month'))
  }

  const handleSubMonth = () => {
    setCurDate(dayjs(curDate).subtract(2, 'month'))
    onChange(dayjs(curDate).subtract(2, 'month'))
  }

  const handleAddMonth = () => {
    setCurDate(dayjs(curDate).add(2, 'month'))
    onChange(dayjs(curDate).add(2, 'month'))
  }

  return (
    <div id='calendar' className={twMerge(`w-full col-span-12 bg-white rounded-ot shadow-md overflow-hidden pb-4`, containerClass)}>  
      <div className='px-2 py-2 flex gap-4 justify-between'>
        <div className='flex items-center gap-2'>
          <Button onClick={handleSubMonth} icon={<LeftOutlined/>}></Button>
          <DatePicker allowClear={false} defaultValue={curDate} value={curDate} onChange={handleChangePrevDate} picker="month" size='small'/>
          <DatePicker disabled allowClear={false} defaultValue={curDate} value={dayjs(curDate).add(1, 'month')} onChange={handleChangeLastDate} picker="month" size='small'/>
          <Button onClick={handleAddMonth} icon={<RightOutlined/>}></Button>
        </div>
        {
          listData && 
          <Segmented
            value={viewMode}
            onChange={(e) => setViewMode(e)}
            options={[
              {
                icon: <div><CalendarOutlined /></div>,
                value: 'calendar',
              },
              {
                icon: <div><BarsOutlined/></div>,
                value: 'list',
              },
            ]}
          >
          </Segmented>
        }
      </div>
      {/* <div className='h-[248px] px-2'> */}
      <div className='h-[198px] px-2'>
        {
          viewMode === 'calendar' ? 
          <table className='w-full table-fixed'>
            <thead>
              <tr>
                <th>Mon</th>
                <th>Tue</th>
                <th>Wed</th>
                <th>Thu</th>
                <th>Fri</th>
                <th>Sat</th>
                <th>Sun</th>
                <th>Mon</th>
                <th>Tue</th>
                <th>Wed</th>
                <th>Thu</th>
                <th>Fri</th>
                <th>Sat</th>
                <th>Sun</th>
              </tr>
            </thead>
            <tbody>
              {getDate()}
            </tbody>
          </table>
          :
          <div className='px-4 py-1 divide-y h-full overflow-y-auto flex flex-col'>
            { listData.length > 0 ?
              listData.map((item, i) => (
                <div className='flex items-center gap-10' key={`listData-${item.EventDate}-${i}`}>
                  <div className='py-1 flex items-center gap-2'>
                    {renderTransportIcon(item)}
                    <div>{dayjs(item.EventDate).format('YYYY-MM-DD ddd')}</div>
                  </div>
                  <div>{item.Description}</div>
                </div>
              ))
              :
              <div className='w-full flex justify-center items-center'>No Data</div>
            }
          </div>
        }
      </div>
    </div>
  )
}

export default CustomMultiCalendar