import { Statistic } from 'antd';
import React, { useMemo } from 'react'
import CountUp from 'react-countup';
const colors = ['#FFF0AA', '#FFC69A', '#FFB3A7', '#D4D9F8', '#E1C4E8', '#FFC3DC', '#C0E0F9', '#B2E6E2', '#B3E8B4', '#B8D4AA', '#F9B8B8', '#D9F8B8']
const brightColors = ['#f7cc08', '#FFC69A', '#FFB3A7', '#D4D9F8', '#E1C4E8', '#FFC3DC', '#C0E0F9', '#B2E6E2', '#B3E8B4', '#B8D4AA', '#F9B8B8', '#D9F8B8']

const formatter = (value) => <CountUp end={value} separator="," />;

function Metric({data=[]}) {
  const totalData = useMemo(() => {
    let totalRoom = 0
    let totalBed = 0
    data.forEach((item) => {
      totalRoom += item.RoomQTY
      totalBed += item.BedQTY
    })
    return { totalRoom, totalBed }
  },[data])
  return (
    <div className='grid md:grid-cols-7 xl:grid-cols-7 3xl:grid-cols-6 gap-3'>
      <div key={`metric`} className={`rounded-lg overflow-hidden h-[80px] flex items-center justify-between bg-white transition-all duration-300`} >
        <div className='leading-none basis-1/3 h-full flex justify-center items-center bg-primary'>
          <div className='text-white'>
            <label className='text-xs leading-none'>Total</label>
            <div className='text-3xl leading-none'>Σ =</div>
          </div>
        </div>
        <div className='leading-none flex h-full justify-evenly items-center basis-2/3 gap-5 bg-white'>
          <Statistic
            title={<label className='text-xs text-secondary2'>Room</label>}
            value={totalData.totalRoom}
            formatter={formatter} 
            valueStyle={{lineHeight: 1, marginTop: -4}}
          />
          <Statistic 
            title={<label className='text-xs text-secondary2'>Bed</label>}
            value={totalData.totalBed}
            formatter={formatter} 
            valueStyle={{lineHeight: 1, marginTop: -4}}
          />
        </div>
      </div>
      {
        data.map((item, i) => (
          <div key={`metric-${i}`} className={`rounded-lg overflow-hidden h-[80px] flex items-center justify-between bg-white transition-all duration-300 `} >
            <div className='leading-none basis-1/3 h-full flex justify-center items-center' style={{backgroundColor: colors[i]}}>
              <div className=' text-md leading-none p-2 rounded shadow-card bg-white'>{item.Camp}</div>
            </div>
            <div className='leading-none flex justify-evenly items-center basis-2/3 gap-3'>
              <Statistic
                title={<label className='text-xs text-secondary2'>Room</label>}
                value={item.RoomQTY}
                formatter={formatter} 
                valueStyle={{lineHeight: 1, marginTop: -4}}
              />
              <Statistic 
                title={<label className='text-xs text-secondary2'>Bed</label>}
                value={item.BedQTY}
                formatter={formatter} 
                valueStyle={{lineHeight: 1, marginTop: -4}}
              />
            </div>
          </div>
        ))
      }
    </div>
  )
}

export default Metric