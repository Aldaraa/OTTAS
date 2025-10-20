import React from 'react'
import BarChart from './bar-chart'

function RoomSection({room, chartData}) {
  return (
    <div className='bg-white rounded-ot p-5 col-span-12'>
      <div className='mb-5 text-2xl font-bold'>Rooms</div>
      <div className=' grid grid-cols-12 gap-4'>
        <div className='col-span-12 grid grid-cols-12 gap-4'>
          <div className='col-span-4 border rounded-ot p-3 '>
            <div className='font-medium'>Today Active Bed</div>
            <div className='grid grid-cols-12 gap-4'>
              <div className='col-span-4'>
                <div className='font-semibold text-xl'>{room?.TodayActiveBed}</div> 
              </div>
              <div className='col-span-8'>
                <BarChart 
                  height={100} 
                  autoFit={true}
                  data={chartData?.ActiveWeek ? chartData?.ActiveWeek : []}
                  color='#63daab' 
                />
              </div>
            </div>
          </div>
          <div className='col-span-4 border rounded-ot p-3 '>
            <div className='font-medium'>Today Empty Room</div>
            <div className='grid grid-cols-12 gap-4'>
              <div className='col-span-4'>
                <div className='font-semibold text-xl'>{room?.TodayEmptyRoom}</div> 
              </div>
              <div className='col-span-8'>
                <BarChart 
                  autoFit={true}
                  height={100} 
                  data={chartData?.EmptyWeek ? chartData?.EmptyWeek : []} 
                  color='#74cbed'
                />
              </div>
            </div>
          </div>
          <div className='col-span-4 border rounded-ot p-3 '>
            <div className='font-medium'>Today No Room</div>
            <div className='grid grid-cols-12 gap-4'>
              <div className='col-span-4'>
                <div className='font-semibold text-xl'>{room?.TodayVirtualRoomEmloyees}</div> 
              </div>
              <div className='col-span-8'>
                <BarChart 
                  height={100} 
                  data={chartData?.VirtualWeek ? chartData?.VirtualWeek : []} 
                  color='#7767f9'
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default RoomSection