import React, { forwardRef, useContext, useEffect, useRef } from 'react'
import DataGrid, {
  Column,
  HeaderFilter,
  Grouping,
  GroupPanel,
  Paging,
  Pager,
  SearchPanel,
  Editing,
  Selection,
  MasterDetail,
  RowDragging,
  LoadPanel,
} from 'devextreme-react/data-grid';
import { AuthContext } from 'contexts';
import { twMerge } from 'tailwind-merge';

const pageSizes = [20, 50, 100]

const   CalendarTable = React.memo(forwardRef(({
    data, 
    columns=[], 
    allowColumnReordering=false,
    containerClass='', 
    tableClass='',
    isHeaderFilter, 
    isGrouping, 
    isSearch, 
    loading, 
    visible=true, 
    title, 
    keyExpr,  
    selection, 
    defaultSelectedRowKeys,
    selectedRowKeys,
    edit,
    onSaving,
    pager = true,
    defaultPageSize,
    renderDetail,
    remoteOperations=false,
    rowAlternationEnabled=true,
    rowDragging,
    ...restProps
  }, ref) => {
  const dataGrid = useRef(null)
  const Ref = ref ? ref: dataGrid
  
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(loading){
      Ref.current.instance.beginCustomLoading();
    }
    else{
      Ref.current.instance.endCustomLoading();
    }
  },[loading])
  
  return (
    <div className={twMerge(`px-2 rounded-ot bg-white shadow-md`, containerClass)}>
      {title}
      <DataGrid
        id='calendar-table'
        ref={Ref}
        dataSource={data}
        allowColumnReordering={allowColumnReordering}
        rowAlternationEnabled={rowAlternationEnabled}
        showColumnLines={false}
        showRowLines={false}
        className={twMerge(`w-full`, tableClass)}
        visible={visible}
        focusedRowEnabled={true}
        keyExpr={keyExpr}
        remoteOperations={remoteOperations}
        onSaving={onSaving} 
        errorRowEnabled={false}
        hoverStateEnabled={true}
        loadPanel={{enabled: false, shadingColor: 'transparent', shading: false, showPane:false, text: null }}
        {...restProps}
      >
        { isSearch && <SearchPanel visible={true} highlightCaseSensitive={true}/> }
        { isGrouping && <GroupPanel visible={true}/> }
        { isGrouping && <Grouping autoExpandAll={true}/> }
        { isHeaderFilter && <HeaderFilter visible={true}/>}
        { selection && <Selection {...selection}/> }
        { edit && 
          <Editing 
            {...edit}
          /> 
        }
        {
          rowDragging &&
          <RowDragging
            allowReordering={true}
            {...rowDragging}
          />
        }
        { pager ?
          remoteOperations ?
          <Pager 
            visible={true}
            showPageSizeSelector={true}
            showNavigationButtons={true} 
            showInfo={true}
            infoText={(currentPage, totalPage, totalDataCount ) => `${totalDataCount} items in ${totalPage} pages`}
          />
          :
          <Pager 
            visible={true}
            allowedPageSizes={pager.pageSize ? pager.pageSize : pageSizes} 
            showPageSizeSelector={true}
            showNavigationButtons={true} 
            showInfo={true}
            infoText={(currentPage, totalPage, totalDataCount ) => `${totalDataCount} items in ${totalPage} pages`}
            {...pager}
          />
          :
          null
        }
        <Paging defaultPageSize={defaultPageSize ? defaultPageSize : 20} />
        {
        renderDetail &&
        <MasterDetail enabled={true} render={renderDetail}/>
        }
        {
          columns.map((col, i) => {
            if(col.name === 'action'){
              if(!state.userInfo?.ReadonlyAccess){
                return(
                  <Column
                    key={`column-${i}-${col.name}`}
                    caption={col.headerCellRender ? null : col.label} 
                    dataField={col.name} 
                  >
                  </Column>
                )
              }
            }
            else if(col.child){
              return(
                <Column caption={col.caption} {...col}>
                  {
                    col.child.map((item, idx) => {
                      return(
                        <Column
                          key={idx}
                          caption={item.headerCellRender ? null : item.label} 
                          dataField={item.name} 
                        />
                      )
                    })
                  }
                </Column>
              )
            }
            else{
              return(
                <Column
                  {...col}
                  dataField={col.name} 
                  caption={col.headerCellRender ? null : col.label}
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
      </DataGrid>
    </div>
  )
}))

export default CalendarTable