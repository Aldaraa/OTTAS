import React from 'react'
import { Tooltip } from 'components'
import CountUp from 'react-countup';
import { Statistic } from 'antd';

const colors = ['#FFF0AA', '#FFC69A', '#FFB3A7', '#D4D9F8', '#E1C4E8', '#FFC3DC', '#C0E0F9', '#B2E6E2', '#B3E8B4', '#B8D4AA', '#F9B8B8', '#D9F8B8']
const formatter = (value) => <CountUp end={value} separator="," />;

function Stat({data}) {
  return (
    <div className='grid md:grid-cols-8 xl:grid-cols-7 2xl:grid-cols-8 gap-3'>
      {
        data.map((item, i) => (
          <Tooltip title={item.Description}>
            <div key={`metric-${i}`} className={`rounded-lg overflow-hidden h-[80px] flex items-center justify-between bg-white transition-all duration-300 hover:shadow-lg`} >
              <div className='leading-none basis-1/3 h-full flex justify-center items-center ' style={{backgroundColor: colors[i]}}>
                <div className='text-md text-center leading-none p-2 rounded shadow-card bg-white w-[60px]'>{item.Code}</div>
              </div>
              <div className='leading-none flex justify-evenly items-center basis-2/3 gap-6'>
                <Statistic 
                  title={<label className='text-xs text-secondary2'>Resource</label>}
                  value={item.Count}
                  formatter={formatter} 
                  valueStyle={{lineHeight: 1, marginTop: -4}}
                />
              </div>
            </div>
          </Tooltip>
        ))
      }
    </div>
  )
}

export default Stat