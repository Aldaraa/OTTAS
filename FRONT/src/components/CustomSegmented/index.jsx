import React, { useContext } from 'react'
import { PlusOutlined, SearchOutlined } from '@ant-design/icons'
import { Segmented } from 'antd'
import { AuthContext } from 'contexts'
import { twMerge } from 'tailwind-merge'

function CustomSegmented({value, onChange, renderData, notDisabled, options, containerClass='', ref, ...restProps}) {
  const { state } = useContext(AuthContext)

  const getSegmentOptions = () => {
    if(!state.userInfo?.ReadonlyAccess){
      return [
        {
          label: (
            <div className='flex items-center gap-2 justify-center'>
              <SearchOutlined/> Search
            </div>
          ),
          value: 'search', 
          key: 'search',
        },
        {
          label: (
            <div className='flex items-center gap-2 justify-center'>
              <PlusOutlined/> Add
            </div>
          ),
          value: 'add', 
          key: 'add',
          disabled: notDisabled ? false : renderData.length > 0
        },
      ]
    }else{
      return [
        {
          label: (
            <div className='flex items-center gap-2 justify-center'>
              <SearchOutlined/> Search
            </div>
          ),
          value: 'search', 
          key: 'search',
        },
      ]
    }
  }

  return (
    <div className={twMerge(`mb-2`, !state.userInfo?.ReadonlyAccess ? 'w-[300px]' : 'w-[150px]', containerClass)}>
      <Segmented
        value={value}
        block
        ref={ref}
        options={options ? options : getSegmentOptions()}
        onChange={onChange}
        className='shadow-md rounded-ot'
        {...restProps}
      />
    </div>
  )
}

export default CustomSegmented