import React, { forwardRef, useContext, useEffect, useRef, useState } from 'react'
import DataGrid, {
  Column,
  HeaderFilter,
  Grouping,
  GroupPanel,
  Paging,
  Pager,
  SearchPanel,
  LoadPanel,
  Editing,
  Selection,
  Toolbar,
  Item,
  MasterDetail,
  RowDragging,
  Summary,
  GroupItem,
  FilterRow,
  RequiredRule,
  PatternRule
} from 'devextreme-react/data-grid';
import { AuthContext } from 'contexts';
import { twMerge } from 'tailwind-merge';
// import anim from 'assets/images/animate.webp'

const pageSizes = [100, 200, 500, 800, 1000]

const Table = forwardRef(({
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
    edit,
    onSaving,
    pager = true,
    defaultPageSize,
    renderDetail,
    remoteOperations=false,
    rowDragging,
    showRowLines=false,
    focusedRowEnabled=true,
    toolbar=[],
    autoExpandAllGroup=true,
    isGroupedCount=false,
    isFilterRow=false,
    columnChooser=null,
    // masterDetail,
    children,
    ...restProps
  }, ref) => {
  const dataGrid = useRef(null)
  const [ height, setHeight ] = useState(300)
  
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(loading){
      ref?.current.instance.beginCustomLoading();
    }
    else{
      ref?.current.instance.endCustomLoading();
    }
  },[loading])

  useEffect(() => {
    function handleResize() {
      setHeight(window.innerHeight);
    }

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);
  
  return (
    <div className={twMerge(`px-2 rounded-ot bg-white shadow-md`, containerClass)}>
      {title}
      <DataGrid
        ref={ref}
        dataSource={data}
        allowColumnReordering={allowColumnReordering}
        rowAlternationEnabled={true}
        showColumnLines={false}
        showRowLines={false}
        className={twMerge(`w-full`, tableClass)}
        visible={visible}
        focusedRowEnabled={focusedRowEnabled}
        keyExpr={keyExpr}
        hoverStateEnabled={true}
        remoteOperations={remoteOperations}
        columnAutoWidth={true}
        // remoteOperations={{grouping: isGrouping, paging: true, sorting: true, groupPaging: isGrouping}}
        onSaving={onSaving}
        errorRowEnabled={false}
        // columnResizingMode={true}
        allowColumnResizing={true}
        cacheEnabled={false}
        wordWrapEnabled={true}
        columnChooser={columnChooser}
        {...restProps}
      >
        <LoadPanel 
          enabled={true}
          // showIndicator={true}
          // indicatorSrc={anim}
        />
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
              columnChooser ? 
              <Item name="columnChooserButton" />
              : null
            }
            {
              restProps?.onExporting ? 
              <Item name="exportButton" />
              : null
            }
            {
              isSearch ? 
              <Item name="searchPanel" />
              : null
            }
          </Toolbar>
        }

        { isSearch && <SearchPanel visible={true} highlightCaseSensitive={true}/> }
        { 
          isGrouping && 
          (
            // remoteOperations ? 
            // <RemoteOperations groupPaging={true}/> 
            // : 
            <GroupPanel visible={true}/> 
          )
        }
        {
          isFilterRow &&
          <FilterRow
            visible={true}
            applyFilter={'auto'} />
        }
        { isGrouping && <Grouping autoExpandAll={autoExpandAllGroup} contextMenuEnabled={true}/> }
        { isHeaderFilter && <HeaderFilter visible={true}/>}
        { selection && <Selection {...selection} /> }
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
            displayMode={'full'}
            showInfo={true}
            allowedPageSizes={pager.pageSize ? pager.pageSize : pageSizes}
            infoText={(currentPage, totalPage, totalDataCount ) => `${totalDataCount} items in ${totalPage} pages`}
          />
          :
          <Pager 
            visible={true}
            allowedPageSizes={pager.pageSize ? pager.pageSize : pageSizes} 
            showPageSizeSelector={true}
            showNavigationButtons={true} 
            showInfo={true}
            displayMode={'full'}
            infoText={(currentPage, totalPage, totalDataCount ) => `${totalDataCount} items in ${totalPage} pages`}
            {...pager}
          />
          :
          null
        }
        <Paging defaultPageSize={defaultPageSize ? defaultPageSize : 100} />
        {
          renderDetail &&
          <MasterDetail {...renderDetail}/>
        }
        {
          columns.map((col, i) => {
            if(col){
              if(col.name === 'action'){
                if(!state.userInfo?.ReadonlyAccess){
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
                    {col.required ? <RequiredRule/> : null}
                    {col.pattern ? <PatternRule pattern={col.pattern?.pattern} message={col.pattern?.message}/> : null}
                  </Column>
                )
              }
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
        {children}
      </DataGrid>
    </div>
  )
})

export default Table