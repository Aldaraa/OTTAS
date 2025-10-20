import { Button, Table, TreeTable } from 'components'
import React, { useEffect, useMemo, useState } from 'react'
import { Tabs } from 'antd'
import axios from 'axios'
import { useParams } from 'react-router-dom'
import { LoadingOutlined } from '@ant-design/icons'
import { Popup } from 'devextreme-react'
import ls from 'utils/ls'

function RoleCenter() {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ removeType, setRemoveType ] = useState(null)
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const { employeeId } = useParams()

  useEffect(() => {
    ls.set('pp_rt', 'rolecenter')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/sysrole/roleinfo/${employeeId}`,
    }).then((res) => {
      setData(res.data)
      let tmp = []
      if(res.data.Menu.length > 0){
        res.data.Menu.map((item) => {
          if(item.Permission){
            tmp.push(item.Id)
          }
        })
      }
      setSelectedRowKeys(tmp)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const menuColumns = [
    {
      label: 'Name',
      name: 'Name',
    },
  ]
  const systemRoleColumn = [
    {
      label: 'Role Name',
      name: 'RoleName',
    },
    {
      label: '',
      name: 'action',
      width: '100px',
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleRemoveBtn(e.row.data, 'system')}>Remove</button>
      )
    },
  ]

  const departmentAdminColumn = [
    {
      label: 'Department Name',
      name: 'DepartmentName',
    },
    {
      label: '',
      name: 'action',
      width: '100px',
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleRemoveBtn(e.row.data, 'admin')}>Remove</button>
      )
    },
  ]

  const departmentManagerColumn = [
    {
      label: 'Department Name',
      name: 'DepartmentName',
    },
    {
      label: '',
      name: 'action',
      width: '100px',
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleRemoveBtn(e.row.data, 'manager')}>Remove</button>
      )
    },
  ]

  const departmentSupervisorColumn = [
    {
      label: 'Department Name',
      name: 'DepartmentName',
    },
    {
      label: '',
      name: 'action',
      width: '100px',
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleRemoveBtn(e.row.data, 'supervisor')}>Remove</button>
      )
    },
  ]

  const groupInfoColumn = [
    {
      label: 'Group Name',
      name: 'GroupName',
    },
    {
      label: '',
      name: 'action',
      width: '100px',
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleRemoveBtn(e.row.data, 'group')}>Remove</button>
      )
    },
  ]

  const items = useMemo(() => {
    let tmp = []
    if(data?.Id !== 0){
      tmp.push({
        key: 0,
        label: 'System role',
        children: <Table
          data={[data]}
          columns={systemRoleColumn}
          allowColumnReordering={false}
          containerClass='shadow-none border mt-5'
          keyExpr='EventDate'
          pager={{showPageSizeSelector: [data]?.length > 100}}
          style={{maxHeight: 'calc(100vh - 340px)'}}
        />
      })
    }
    if(data?.DepartmentAdminInfo.length > 0){
      tmp.push({
        key: 1,
        label: `Department Admin (${data?.DepartmentAdminInfo.length})`,
        children: <Table
          data={data?.DepartmentAdminInfo}
          columns={departmentAdminColumn}
          allowColumnReordering={false}
          containerClass='shadow-none border mt-5'
          keyExpr='EventDate'
          pager={{showPageSizeSelector: data?.DepartmentAdminInfo?.length > 100}}
          style={{maxHeight: 'calc(100vh - 340px)'}}
        />
      })
    }
    if(data?.DepartmentManagerInfo?.length > 0){
      tmp.push({
        key: 2,
        label: `Department Manager (${data?.DepartmentManagerInfo?.length})`,
        children: <Table
          data={data?.DepartmentManagerInfo}
          columns={departmentManagerColumn}
          allowColumnReordering={false}
          containerClass='shadow-none border mt-5 max-w-[600px]'
          keyExpr='Id'
          pager={{showPageSizeSelector: data?.DepartmentManagerInfo?.length > 100}}
          style={{maxHeight: 'calc(100vh - 340px)'}}
        />
      })
    }
    if(data?.DepartmentSupervisorInfo?.length > 0){
      tmp.push({
        key: 3,
        label: `Department Supervisor (${data?.DepartmentSupervisorInfo?.length})`,
        children: <Table
          data={data?.DepartmentSupervisorInfo}
          columns={departmentSupervisorColumn}
          allowColumnReordering={false}
          containerClass='shadow-none border mt-5'
          pager={{showPageSizeSelector: data?.DepartmentSupervisorInfo?.length > 100}}
          keyExpr='Id'
          style={{maxHeight: 'calc(100vh - 340px)'}}
        />
      })
    }
    if(data?.GroupInfo.length > 0){
      tmp.push({
        key: 4,
        label: `Group Info (${data?.GroupInfo.length})`,
        children: <Table
          data={data?.GroupInfo}
          columns={groupInfoColumn}
          allowColumnReordering={false}
          containerClass='shadow-none border mt-5'
          pager={{showPageSizeSelector: data?.GroupInfo?.length > 100}}
          keyExpr='Id'
          style={{maxHeight: 'calc(100vh - 340px)'}}
        />
      })
    }
    if(data?.Menu?.length > 0){
      tmp.push({
        key: 5,
        label: `Menu (${data?.Menu?.length})`,
        children: <TreeTable
          selectedRowKeys={selectedRowKeys}
          autoExpandAll={true}
          dataSource={data?.Menu}
          pager={false}
          columns={menuColumns}
          allowColumnReordering={false}
          selection={{mode: 'multiple', recursive: true, showCheckBoxesMode: 'always', allowSelectAll: true, selectAllMode: 'page'}}
          containerClass='shadow-none border mt-5'
          keyExpr="Id"
          parentIdExpr="Head_ID"
          style={{maxHeight: 'calc(100vh - 340px)'}}
        />
      })
    }

    return tmp
  },[data])

  const handleRemoveBtn = (row, type) => {
    setEditData(row)
    setRemoveType(type)
    setShowPopup(true)
  }

  const handleRemove = () => {
    setActionLoading(true)
    if(removeType === 'group'){
      axios({
        method: 'delete',
        url: `tas/requestgroupemployee/groupemployees`,
        data: {
          id: editData.Id
        }
      }).then(() => {
        getData()
        setShowPopup(false)
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }else if(removeType === 'system'){
      axios({
        method: 'delete',
        url: `tas/sysrole/removeuser/${editData.Id}`,
      }).then(() => {
        getData()
        setShowPopup(false)
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }else{
      axios({
        method: 'delete',
        url: `tas/department/${removeType}/${editData.Id}`,
      }).then(() => {
        getData()
        setShowPopup(false)
      }).catch(() => {
  
      }).then(() => setActionLoading(false))
    }
  }

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      {
        loading ? 
        <LoadingOutlined style={{fontSize: 26}}/>
        :
        items.length > 0 ?
        <Tabs
          items={items}
          type='card'
        />
        : 'No Roles'
      }
      <Popup
        visible={showPopup}
        showTitle={false}
        height={130}
        width={350}
      >
        <div>Are you sure you want to remove from <span className='font-semibold text-primary'>{editData?.DepartmentName} {editData?.GroupName}</span> role?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleRemove} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default RoleCenter