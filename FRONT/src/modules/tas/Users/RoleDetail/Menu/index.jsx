import React, { useContext, useEffect, useState } from 'react'
import { Button, Skeleton, TreeTable } from 'components'
import { useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import axios from 'axios'

const menuColumns = [
  {
    label: 'Name',
    name: 'Name',
  },
]

function Menu() {
  const [ menus, setMenus ] = useState([])
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ loading, setLoading ] = useState(true)
  const [ actionLoading, setActionLoading ] = useState(false)
  const { state } = useContext(AuthContext)
  const {roleId} =  useParams()

  useEffect(() => {
    getMenus()
  },[])

  const getMenus = () => {
    axios({
      method: 'get',
      url: `tas/sysrolemenu/getmenu/${roleId}`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        if(item.Permission){
          tmp.push(item.Id)
        }
      })
      setSelectedRowKeys(tmp)
      setMenus(res.data)
    }).catch((err) => {

    }).finally(() => {
      setLoading(false)
    })
  }

  const handleSelect = (e) => {
    const selectedData = e.component.getSelectedRowsData('leavesOnly');
    let lastRowKeys = selectedData.map((item) => item.Id)
    setSelectedRowKeys(lastRowKeys)
  }

  const rowPreparing = (e) => {
    // const checkBox = e.cellElement.querySelector('.dx-select-checkbox')
    // checkBox.style.display = "none";
  }

  const preparing = (e) => {
    if(state.userInfo?.ReadonlyAccess ){
      e.editorOptions.disabled = true;
    }
  }

  const handleSavePermission = () => {
    setActionLoading(true)
    let tmp = []
    selectedRowKeys.map((id) => {
      tmp.push({menuId: id, permission: 1})
    })  
    axios({
      method: 'post',
      url: 'tas/sysrolemenu',
      data: {
        roleId: roleId,
        menuPermissions: [...tmp]
      }
    }).then((res) => {
      getMenus()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  return (
    <div className='pt-4'>
      {
        loading ?
          <Skeleton className='h-[200px]'/> 
        :
        <>
          <TreeTable
            dataSource={menus}
            id="employees"
            className='border rounded-ot overflow-hidden'
            keyExpr="Id"
            parentIdExpr="Head_ID"
            columns={menuColumns}
            showRowLines={true}
            selection={{mode: 'multiple', recursive: true, showCheckBoxesMode: 'always', allowSelectAll: true, selectAllMode: 'page'}}
            rowAlternationEnabled={false}
            selectedRowKeys={selectedRowKeys}
            onSelectionChanged={handleSelect}
            onEditorPreparing={preparing}
            onCellPrepared={rowPreparing}
            autoExpandAll={true}
            pager={false}
            tableClass='max-h-[calc(100vh-150px)]'
            style={{maxHeight: 'calc(100vh - 270px)'}}
          />
          {
            !state.userInfo?.ReadonlyAccess &&
            <div className='flex justify-end my-4'>
              <Button type='primary' onClick={handleSavePermission} loading={actionLoading}>Save</Button>
            </div>
          }
        </>
      }
    </div>
  )
}

export default Menu