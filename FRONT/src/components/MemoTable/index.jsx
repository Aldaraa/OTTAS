import { Table } from 'components'
import React from 'react'

const MemoTable = React.memo(({tableRef, data, columns, loading, keyExpr, showRowLines, pager, withStore, ...restProps}) => {
  return (
    <Table
      ref={tableRef}
      data={data}
      columns={columns}
      loading={loading}
      keyExpr={keyExpr}
      showRowLines={showRowLines}
      pager={pager}
      {...restProps}
    />
  )
}, (prevProps, currentProps) => {
  if(prevProps.withStore){
    if(JSON.stringify(prevProps.columns) !== JSON.stringify(currentProps.columns)){
      return false
    }
  }else{
    if(JSON.stringify(prevProps.data) !== JSON.stringify(currentProps.data)){
      return false
    }
  }
  return true
})

export default MemoTable