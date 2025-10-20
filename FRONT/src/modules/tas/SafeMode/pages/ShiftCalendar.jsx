import { DatePicker } from 'antd';
import React, { useState } from 'react'
import dayjs from 'dayjs'
import { LeftOutlined, RightOutlined } from '@ant-design/icons';
import { Button } from 'components';
import { twMerge } from 'tailwind-merge';

const ShiftCalendar = React.memo(({data, onChange, currentDate, cellRender, extraComponent, selectedData, containerClass='', headerClass='', cellClass='', picker='month', isEdit, ...restProps}) => {
  const [ curDate, setCurDate ] = useState(currentDate ? currentDate : dayjs().startOf('M'))

  const pickCellBg = (day) => {
    if(dayjs(day).format('YYYY-MM-DD') === dayjs().format('YYYY-MM-DD')){
      return 'text-blue-500 font-bold'
    }
    else if(dayjs(day).format('YYYY-MM') !== dayjs(currentDate).format('YYYY-MM')){
      // return 'opacity-50'
    }
  }

  const renderDate = (date, index) => {
    return(
      <td key={`calendar-cell-${index}`}>
        <div className={twMerge(`p-1 w-full flex justify-between items-center`, cellClass, pickCellBg(date))}>
          {
            cellRender ? 
            cellRender(date) :
            <div className='flex flex-col justify-between h-full'>
              <div className='font-medium'>{dayjs(date).format('DD')}</div>
            </div>
          }
        </div>
      </td>
    )
  }

  const getDate = () => {
    var calendar = []
    const startDate = dayjs(currentDate).startOf('month').startOf('isoWeek');
    const endDate = dayjs(currentDate).endOf('month').endOf('isoWeek');
    
    let day = startDate.subtract(1, 'day');
    
    while (day.isBefore(endDate, 'day')) {
      calendar.push(Array(7).fill(0).map(() => {
        day = day.add(1, 'day');
        return day.format('YYYY-MM-DD');
      }));
    }

    if(calendar.length > 0){
      return calendar.map((week, i) => (
        <tr className='text-center' key={`calendar-row-${i}`}>
          {
            week.map((day, index) => renderDate(day, index))
          }
        </tr>
      ))
    }
  }

  const handleChange = (date) => {
    setCurDate(dayjs(date))
    if(onChange){
      onChange(date)
    }
  }

  const handleSubMonth = () => {
    setCurDate(dayjs(curDate).subtract(1, 'month'))
    onChange(dayjs(curDate).subtract(1, 'month'))
  }

  const handleAddMonth = () => {
    setCurDate(dayjs(curDate).add(1, 'month'))
    onChange(dayjs(curDate).add(1, 'month'))
  }

  return (
    <div 
      id='calendar' 
      className={twMerge(`w-full col-span-12 bg-white rounded-ot shadow-md overflow-hidden`, containerClass)}
    >  
      <div className={twMerge(`p-2 flex gap-4 justify-start`, headerClass)}>
        <Button onClick={handleSubMonth} icon={<LeftOutlined/>}></Button>
        <DatePicker allowClear={false} defaultValue={curDate} value={curDate} onChange={handleChange} picker={picker} size='small'/>
        <Button onClick={handleAddMonth} icon={<RightOutlined/>}></Button>
        { extraComponent && extraComponent }
      </div>
      <div className='min-h-[248px]'>
        <table className='w-full table-fixed'>
          <thead>
            <tr className='text-center'>
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
      </div>
    </div>
  )
}
, (prevProps, nextProps) => {
  if(JSON.stringify(prevProps.data) !== JSON.stringify(nextProps.data) || prevProps.isEdit !== nextProps.isEdit){
    return false
  }
  return true
}
)

export default ShiftCalendar