import dayjs from 'dayjs'
import React, { forwardRef, memo, useCallback, useMemo } from 'react'
import Header from './Header'
import Card from './Card'
import { twMerge } from 'tailwind-merge'
import { LoadingOutlined } from '@ant-design/icons'

const daysOfWeek = [ 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

const Filter = memo(({selectedDir, selectedMode, selectedAirline, timeGroup, children, item}) => {
  const isModeValid =  selectedMode === 'All' || item.TransportMode === selectedMode;
  const isDirValid = selectedDir === 'All' || item.Direction === selectedDir;
  const isTimeValid = timeGroup === 'All' || item.TimeGroup === timeGroup;
  const isAirlineValid = selectedAirline === 'All' || item.Carrier === selectedAirline;

  if (isDirValid && isModeValid && isTimeValid && (selectedMode !== 'Airplane' || isAirlineValid)) {
    return children;
  }
  
  return null;
});

const Schedule = memo(forwardRef(({date, data, onSelect, selectedDir, selectedMode, selectedAirline, timeGroup, loading, ...props}, ref) => {
  const datesOfWeekdays = useMemo(() => {
    let weekDates = []
    for(let i = 1; i <= 7; i++){
      weekDates.push(dayjs(date).isoWeekday(i).format('YYYY-MM-DD'))
    }
    return weekDates
  },[date])

  const filterByTime = useCallback((array) => {
    return {
      AM: array.filter((item) => item.TimeGroup === 'AM'),
      PM: array.filter((item) => item.TimeGroup === 'PM'),
    }
  },[])

  return (
    <div className='w-full relative h-[calc(100vh-200px)]'>
      {
        timeGroup === 'All' ?
        <div className='absolute left-0 top-[calc((100vh-240px)/2+34px)] rounded-l w-full border-t border-secondary2'></div>
        :
        <div className={twMerge('absolute -left-[8px] top-7 rounded-l h-[calc(100vh-240px)] w-2 bg-blue-300', timeGroup === 'AM' ? 'bg-blue-300' : 'bg-[#604cc3]')}></div>
      }
      <div className='flex gap-2 overflow-auto' id='schedule-container'>
        {datesOfWeekdays.map((date, i) => (
          <div className='rounded-t basis-full min-w-[210px]'>
            <Header date={date} dayOfWeek={daysOfWeek[i]}>
            </Header>
            {
              loading ? <div className='p-3 w-full flex justify-center'><LoadingOutlined style={{fontSize: 28}}/></div> :
              <>
                { data[date] ?
                <div className='flex flex-col gap-3'>
                  {
                    timeGroup === 'All' ?
                    <>
                      <div className='h-[calc((100vh-240px)/2)] border-b overflow-auto custom-scroll bg-[#f4f5f7] relative capture-div'>
                        <div className='flex flex-col gap-2 px-2'>
                          {
                            filterByTime(data[date])['AM']?.map((item) => (
                              <Filter
                                item={item}
                                selectedDir={selectedDir}
                                selectedMode={selectedMode}
                                selectedAirline={selectedAirline}
                                timeGroup={timeGroup}
                              >
                                <Card cellData={item} onClick={onSelect}/>
                              </Filter>
                            ))
                          }
                        </div>
                      </div>
                      <div className='h-[calc((100vh-240px)/2)] border-b overflow-auto custom-scroll bg-gray-100 capture-div'>
                        <div className='flex flex-col gap-2 px-2'>
                          {
                            filterByTime(data[date])['PM']?.map((item) => (
                              <Filter
                                item={item}
                                selectedDir={selectedDir}
                                selectedMode={selectedMode}
                                selectedAirline={selectedAirline}
                                timeGroup={timeGroup}
                              >
                                <Card cellData={item} onClick={onSelect}/>
                              </Filter>
                            ))
                          }
                        </div>
                      </div>
                    </>
                    : 
                    <div className='h-[calc(100vh-240px)] border-b overflow-auto custom-scroll bg-gray-100 relative capture-div'>
                      <div className='flex flex-col gap-2 px-2'>
                        {
                          filterByTime(data[date])[timeGroup]?.map((item) => (
                            <Filter
                              item={item}
                              selectedDir={selectedDir}
                              selectedMode={selectedMode}
                              selectedAirline={selectedAirline}
                              timeGroup={timeGroup}
                            >
                              <Card cellData={item} onClick={onSelect}/>
                            </Filter>
                          ))
                        }
                      </div>
                    </div>
                  }
                </div>
                  : null
                }
              </>
            }
          </div>
        ))}
      </div>
    </div>
  )
}), (prevProps, nextProps) => JSON.stringify(prevProps) === JSON.stringify(nextProps))

export default Schedule