import React, { forwardRef, useContext, useEffect, useRef } from 'react'
import { TreeList, Column, Selection, Pager, Paging, LoadPanel } from 'devextreme-react/tree-list'
import { AuthContext } from 'contexts'

const TreeTable = forwardRef(({
  dataSource,
  itemsExpr,
  columns=[],
  selection,
  title,
  containerClass='',
  loading,
  paging,
  tableCalss='',
  ...restProps
}, ref) => {
  const { state } = useContext(AuthContext)
  const dataGrid = useRef(null)
  const Ref = ref ? ref : dataGrid
  
  useEffect(() => {
    if(loading){
      Ref.current.instance.beginCustomLoading();
    }
    else{
      Ref.current.instance.endCustomLoading();
    }
  },[loading])

  const getNodeByLevel = (node, level) => {  
    if(!node.parent) {  
        return;  
    }  


    if (node.parent.level === level || node.parent.level === undefined) { // Remove the "|| node.parent.level === undefined" part after release  
        return node.parent;  
    } else {  
        return getNodeByLevel(node.parent, level);  
    }  
  }

  const onCellPrepared = (e) => {
    if(e.rowType === "data" && e.columnIndex === 0) {  
      var currentNode = e.row.node,  
          $emptySpaceElements = e.cellElement.querySelectorAll(".dx-treelist-empty-space"),  
          children = currentNode.parent.children,  
          isLasChildren = children[children.length - 1].key === currentNode.key;  
  
      for(var i = 0; i < $emptySpaceElements.length; i++) {  
          var node = getNodeByLevel(currentNode, i-1);  
  
          if(node && node.children.length > 1 && currentNode.parent.key !== node.children[node.children.length - 1].key) {  
              $emptySpaceElements[i].classList.add("dx-line");  
          }  
  
          if(i === ($emptySpaceElements.length - 1)) {  
            $emptySpaceElements[i].classList.add("dx-line");
            $emptySpaceElements[i].classList.add("dx-line-middle");
            if (isLasChildren) $emptySpaceElements[i].classList.toggle("dx-line-last");
          }  
      }  
    }  
  }

  return (
    <div className={containerClass}>
      {title}
      <TreeList 
        ref={Ref}
        dataSource={dataSource}
        itemsExpr={itemsExpr}
        showColumnLines={false} 
        showRowLines={false}
        rowAlternationEnabled
        onCellPrepared={onCellPrepared}
        className={tableCalss}
        pager={{
          visible: true, 
          allowedPageSizes: [5, 40, 80], 
          showInfo: true, 
          showPageSizeSelector: true,
          infoText: (currentPage, totalPage, totalDataCount ) => `${totalDataCount} items in ${totalPage} pages`,
        }}
        {...restProps}
      >
        {loading && <LoadPanel/>}
        {
          paging?.enabled &&
          <Paging defaultPageSize={5}/>
        }
        {
          selection &&
          <Selection {...selection}/>
        }
        {
          columns.map((col, i) => {
            if(col.name === 'action'){
              if(!state.userInfo?.ReadonlyAccess){
                return(
                  <Column 
                    {...col}
                    caption={col.label} 
                    dataField={col.name} 
                    key={`column-${i}`} 
                    headerCellRender={col.headerRender ? col.headerRender : false}
                  />
                )
              }

            }
            else{
              return(
                <Column 
                  {...col}
                  caption={col.label} 
                  dataField={col.name} 
                  key={`column-${i}`} 
                  headerCellRender={col.headerRender ? col.headerRender : false}
                />
              )
            }
          })
        }
      </TreeList>
    </div>
  )
})

export default TreeTable