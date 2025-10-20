import { InputNumber } from 'antd';
import { Button } from 'components';
import { AuthContext } from 'contexts';
import DataGrid, { Column, GroupItem, Summary } from 'devextreme-react/data-grid';
import React, { forwardRef, useContext, useEffect, useMemo, useState } from 'react'
import { twMerge } from 'tailwind-merge';
import MemoTable from './MemoTable'

const pageSizeRegex = new RegExp(/^([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|1000)$/)

 const tablePagerPosition = (position) => {
  if(position === 'top'){
    return 'flex flex-col-reverse'
  }else{
    return 'flex flex-col'
  }
 }

const CustomTable = forwardRef(({data=[], containerClass='', pageIndex=0, pageSize, columns=[], keyExpr='',
  totalCount=0, onChangePageSize, onChangePageIndex, pagerPosition='bottom', tableClass='', isGrouping=false,
  loading, rowAlternationEnabled, pageSizeDisabled=false, id, isPagination=true, toolbar=[], title='', isSearch=false,
  ...restprops
}, ref) => {
  const [ pageLength, setPageLength ] = useState(0)

  useEffect(() => {
    if(data.length){
      setPageLength(parseInt(totalCount/pageSize) + ((totalCount%pageSize) > 0 ? 1 : 0))
    }
  },[data])

  const handleChangePageSize = (e) => {
    if(e.code === 'Enter' && e.target.value <= totalCount){
      onChangePageSize(e.target.value)
      onChangePageIndex(0)
    }
    else if(e.target.value >= totalCount){
      onChangePageSize(totalCount)
      onChangePageIndex(0)
    }
  }

  const renderPaging = useMemo(() => {
    if(pageLength > 0 && pageLength < 10){
      return (
        <div className='flex gap-2 items-center'>
          {
            Array(pageLength > 0 ? pageLength : 0).fill(0).map((item, index) => {
              return( (index) === pageIndex ?
                <Button type='primary' className='rounded-[10px] px-[10px] py-[5px] text-[12px]'>
                  <span>{index+1}</span>
                </Button>
                :
                <button onClick={() => onChangePageIndex(index)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
                  <span>{index+1}</span>
                </button>
              )
            })
          }
        </div>
      )
    }else if(pageLength >= 10){
      const pager = Array.from({ length: pageLength }, (_, i) => i + 1);
      if(pageIndex < 5){
        return (
          <div className='flex gap-2 items-center'>
            {
              pager.slice(0, 6).map((item, index) => {
                return( (item-1) === pageIndex ?
                  <Button type='primary' className='rounded-[10px] px-[10px] py-[5px] text-[12px]'>
                    <span>{item}</span>
                  </Button>
                  :
                  <button onClick={() => onChangePageIndex(index)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
                    <span>{item}</span>
                  </button>
                )
              })
            }
            <div className='self-end text-gray-300'>•••</div>
            <button onClick={() => onChangePageIndex(pager[pager.length-1]-1)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
              <span>{pager.length}</span>
            </button>
          </div>
        )
      }else if(pageIndex >= 5 && pageIndex < (pager.length-5)){
        return (
          <div className='flex gap-2 items-center'>
            <button onClick={() => onChangePageIndex(0)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
              <span>{pager[0]}</span>
            </button>
            <div className='text-gray-400 font-bold'>. . .</div>
            {
              pager.slice(pageIndex-2, pageIndex+3).map((item) => {
                return( (item-1) === pageIndex ?
                  <Button type='primary' className='rounded-[10px] px-[10px] py-[5px] text-[12px]'>
                    <span>{item}</span>
                  </Button>
                  :
                  <button onClick={() => onChangePageIndex(item-1)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
                    <span>{item}</span>
                  </button>
                )
              })
            }
            <div className='text-gray-400 font-bold'>. . .</div>
            <button onClick={() => onChangePageIndex(pager[pager.length-1]-1)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
              <span>{pager.length}</span>
            </button>
          </div>
        )
      }else if(pageIndex >= (pager.length-5)){
        return (
          <div className='flex gap-2 items-center'>
            <button onClick={() => onChangePageIndex(0)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
              <span>{pager[0]}</span>
            </button>
            <div className='self-end text-gray-300'>•••</div>
            {
              pager.slice(pager.length-6, pager.length).map((item) => {
                return( (item-1) === pageIndex ?
                  <Button type='primary' className='rounded-[10px] px-[10px] py-[5px] text-[12px]'>
                    <span>{item}</span>
                  </Button>
                  :
                  <button onClick={() => onChangePageIndex(item-1)} className='rounded-[10px] border border-white px-[10px] py-[5px] text-[12px]'>
                    <span>{item}</span>
                  </button>
                )
              })
            }
          </div>
        )
      }
    }
  },[pageLength, pageIndex])

  const changingPageSize = (value) => {
    if(pageSizeRegex.test(value)){
      return value
    }
  }

  return (
    <div className={twMerge(`px-2 bg-white rounded-ot shadow-md`, containerClass, tablePagerPosition(pagerPosition))}>
      {title}
      <MemoTable
        ref={ref}
        tableClass={tableClass}
        keyExpr={keyExpr}
        data={data}
        isGrouping={isGrouping ? {expandMode: 'rowClick', autoExpandAll: true} : false}
        pager={false}
        columns={columns}
        id={id}
        rowAlternationEnabled={rowAlternationEnabled}
        toolbar={toolbar}
        {...restprops}
      />
      {
        data?.length > 0 && isPagination &&
        <div className={twMerge('bg-white py-[9px] flex justify-between', pagerPosition === 'top' ? 'border-b' : 'border-t')}>
          <div className='flex gap-5 items-center'>
            <InputNumber
              size='small'
              controls={false}
              value={pageSize}
              max={totalCount}
              disabled={pageSizeDisabled}
              onPressEnter={handleChangePageSize}
            />
            <div className='text-[#858585] font-light'>{totalCount} items in {pageLength} pages</div>
          </div>
          {renderPaging}
        </div>
      }
    </div>
  )
}, (prev, cur) => {
  if(JSON.stringify(prev.data) !== JSON.stringify(cur.data)){
    return false
  }else if(JSON.stringify(prev.columns) !== JSON.stringify(cur.columns)){
    return false
  }
  return true
})

export default CustomTable