import { Tooltip } from 'components';
import { AuthContext } from 'contexts';
import React, { useContext, useState } from 'react'
import hexToHSL from 'utils/hexToHSL';

function MultiViewers() {
  const [ users, setUsers ] = useState([])
  const { state } = useContext(AuthContext)

  return (
    // users.length > 0 &&
    <div className='flex gap-2 border-r border-gray-400 pr-5'>
      {
        state.multiViewers.map((item, i) => (
          <Tooltip 
            key={i}
            title={<div className='flex flex-col items-center leading-none gap-2'>
                <div className='flex flex-col items-center'>
                  <div className='font-bold mb-1'>{item.name}</div>
                  <div>{item.role}</div>
                </div>
                <div className='text-gray-500'>Viewing This Data</div>
              </div>
            }
          >
            <div className='relative'>
              <div className='h-full w-full absolute inset-0 z-0 animate-ping bg-green-500 rounded-full'></div>
              <button className='w-7 h-7 border-2 border-green-500 rounded-full relative' style={{backgroundColor: `#${item.color}`}}>
                <div className='absolute inset-0 flex justify-center items-center' style={{color: hexToHSL(item?.color) > 60 ? 'black' : 'white'}}>
                  {item.name[0]}
                </div>
              </button>
            </div>
          </Tooltip>
        ))
      }
    </div>
  )
}

export default MultiViewers