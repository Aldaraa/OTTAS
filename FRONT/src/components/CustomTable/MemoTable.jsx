import React, { forwardRef, useContext, useEffect } from 'react'
import { DataGrid, Toolbar } from 'devextreme-react';
import { Column, GroupItem, Summary, LoadPanel, Item, Button, SearchPanel } from 'devextreme-react/data-grid';
import { AuthContext } from 'contexts';
import { twMerge } from 'tailwind-merge';

const MemoTable = React.memo(forwardRef(({data=[], columns=[], keyExpr='',
  onChangePageSize, onChangePageIndex, pagerPosition='bottom', tableClass='', isGrouping=false,
  loading, rowAlternationEnabled=true, isGroupedCount=false, id, toolbar=[], isSearch=false,
  ...restprops}, ref) => 
{
  const { state } = useContext(AuthContext);
  
  return (
    <DataGrid
      ref={ref}
      id={id}
      keyExpr={keyExpr}
      dataSource={data}
      className={twMerge(tableClass)}
      grouping={isGrouping ? {expandMode: 'rowClick', autoExpandAll: true} : false}
      groupPanel={{visible: isGrouping}}
      pager={false}
      paging={false}
      allowColumnResizing={true}
      rowAlternationEnabled={rowAlternationEnabled}
      focusedRowEnabled
      columnAutoWidth={true}
      {...restprops}
    >
      { isSearch && <SearchPanel visible={true} highlightCaseSensitive={true}/> }
      {
        toolbar.length > 0 && 
        <Toolbar>
          {
            toolbar.map((item) => (
              <Item {...item}>
              </Item>
            ))
          }
          {
            isSearch ? 
            <Item name="searchPanel" />
            : null
          }
        </Toolbar>
      }
      {
        columns.map((col, i) => {
          if(col.name === 'action'){
            if(!state.userInfo?.ReadonlyAccess || state.userInfo?.CreateRequest === 1){
              return(
                <Column
                  {...col}
                  caption={col.headerCellRender ? null : col.label} 
                  dataField={col.name} 
                  key={`column-${i}-${col.name}`}
                  lookup={ col.haveLookup && {
                    dataSource: col.haveLookup.dataSource,
                    valueExpr: col.haveLookup.valueExpr,
                    displayExpr: col.haveLookup.displayExpr,
                  }}
                  editCellTemplate={col.editCellTemplate}
                >
                </Column>
              )
            }
          }
          else{
            return(
              <Column
                {...col}
                caption={col.headerCellRender ? null : col.label} 
                dataField={col.name} 
                key={`column-${i}-${col.name}`}
                lookup={ col.haveLookup && {
                  dataSource: col.haveLookup.dataSource,
                  valueExpr: col.haveLookup.valueExpr,
                  displayExpr: col.haveLookup.displayExpr,
              }}
                editCellTemplate={col.editCellTemplate}
              >
              </Column>
            )
          }
        }
        )
      }
      {
        isGrouping && isGroupedCount &&
        <Summary>
          <GroupItem
            column="Id"
            summaryType="count" />
        </Summary>
      }
    </DataGrid>
  )
}))

export default MemoTable