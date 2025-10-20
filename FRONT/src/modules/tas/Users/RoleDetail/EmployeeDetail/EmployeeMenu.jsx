import React, { useCallback, useContext, useEffect, useState } from 'react'
import { Button, Skeleton, TreeTable } from 'components'
import { AuthContext } from 'contexts'
import axios from 'axios'

const menuColumns = [
  {
    label: 'Name',
    name: 'Name',
    allowSorting: false
  },
]

function EmployeeMenu({employeeData, refreshData}) {
  const [ menus, setMenus ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])

  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(employeeData){
      getEmployeeMenu()
    }
  },[employeeData])

  const getEmployeeMenu = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/sysroleemployeemenu/getmenu/${employeeData?.EmployeeId}`,
    }).then((res) => {
      setMenus(res.data,)
      let tmp = []
      res.data.map((item) => {
        if(item.Permission){
          tmp.push(item.Id)
        }
      })
      setSelectedRowKeys(tmp)
    }).catch((err) => {

    }).finally(() => setLoading(false))
  }

  const handleSavePermission = () => {
    setActionLoading(true)
    let tmp = []
    selectedRowKeys.map((id) => {
      tmp.push({menuId: id, permission: 1})
    })  
    axios({
      method: 'post',
      url: 'tas/sysroleemployeemenu',
      data: {
        employeeId: employeeData?.EmployeeId,
        menuPermissions: [...tmp]
      }
    }).then((res) => {
      getEmployeeMenu()
      refreshData()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSelect = useCallback((e) => {
    const selectedData = e.component.getSelectedRowsData('leavesOnly');
    let lastRowKeys = selectedData.map((item) => item.Id)
    setSelectedRowKeys(lastRowKeys)
  }, [])

  const preparing = useCallback((e) => {
    if(state.userInfo?.ReadonlyAccess ){
      e.editorOptions.disabled = true;
    }
  }, [])

  return (
    <div className='mt-3'>
      {
        loading ? 
        <Skeleton className='h-[200px]'/>
        :
        <div className='relative'>
          <div className='absolute right-0'>{selectedRowKeys.length} menu access</div>
          <TreeTable
            dataSource={menus}
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
            pager={false}
            style={{maxHeight: 'calc(100vh - 175px)'}}
            autoExpandAll={true}
            scrolling={{mode: 'standard'}}
          />
          {
            !state.userInfo?.ReadonlyAccess &&
            <div className='flex justify-end mt-4'>
              <Button type='primary' className='text-xs' onClick={handleSavePermission} loading={actionLoading}>Save</Button>
            </div>
          }
        </div>
      }
    </div>
  )
}

export default EmployeeMenu