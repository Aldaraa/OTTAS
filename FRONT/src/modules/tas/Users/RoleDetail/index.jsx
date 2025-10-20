import { Form, Table, Button, Modal, PeopleSearch, Tooltip } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useCallback, useContext, useEffect, useState } from 'react'
import { Tabs } from 'antd'
import axios from 'axios'
import { DeleteOutlined, EyeOutlined, PlusOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { Link, useLoaderData, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import RoleReportPermission from './report'
import Menu from './Menu'
import EmployeeDetail from './EmployeeDetail'
import dayjs from 'dayjs'

function RoleDetail() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ showStatusPopup, setShowStatusPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ openDrawer, setOpenDrawer ] = useState(false)
  const [ selectedEmployee, setSelectedEmployee ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showRemovePopup, setShowRemovePopup ] = useState(false)

  const { loadedData } = useLoaderData()
  const {roleId} =  useParams()

  const [ form ] = Form.useForm()
  const { action, state } = useContext(AuthContext)

  useEffect(() => {
    action.changeMenuKey('/tas/roles')
  },[])

  useEffect(() => {
    if(loadedData){
      setData(loadedData)
      setRenderData(loadedData.RoleUsers)
    }
  },[loadedData])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/sysrole/${roleId}`
    }).then((res) => {
      setData(res.data)
      setRenderData(res.data.RoleUsers)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const handleSelectEmployee = (dataItem) => {
    setSelectedEmployee(dataItem)
    setOpenDrawer(true)
  }

  const handleClickRemoveCatchBtn = useCallback((rowData) => {
    setEditData(rowData)
    setShowRemovePopup(true)
  },[])

  const columns = [
    {
      label: 'Person #',
      name: 'EmployeeId',
      width: 80,
      alignment: 'left'
    },
    {
      label: 'Fullname',
      name: 'Name',
      cellRender:(e) => (
        <div className='text-blue-400 hover:underline'>
          <Link to={`/tas/people/search/${e.data.EmployeeId}`}>{e.data.Firstname} {e.data.Lastname}</Link>
        </div>
      )
    },
    {
      label: 'ADAccount',
      name: 'AdAccount'
    },
    {
      label: 'Last Login Date',
      name: 'LastLoginDate',
      alignment: 'left',
      cellRender: (e) => (
        <span>{dayjs(e.value).format('YYYY-MM-DD HH:mm')}</span>
      )
    },
    {
      label: 'Custom Menu',
      name: 'HasMenu',
      alignment: 'center',
      width: '100px',
      cellRender: (e) => (
        <div>
          {
            e.value ? 
            <Tooltip title='Employee has set up personalized access permissions for menus.'>
              <CheckBox disabled iconSize={18} value={e.value}/>
            </Tooltip>
            : '-'
          }
        </div>
      )
    },
    {
      label: 'Custom Report',
      name: 'HasReport',
      alignment: 'center',
      width: '100px',
      cellRender: (e) => (
        <div>
          {
            e.value ? 
            <Tooltip title='Employee has set up personalized access permissions for menus.'>
              <CheckBox disabled iconSize={18} value={e.value}/>
            </Tooltip>
            : '-'
          }
        </div>
      )
    },
    {
      label: 'Custom Report Data',
      name: 'HasReportData',
      alignment: 'center',
      width: '100px',
      cellRender: (e) => (
        <div>
          {
            e.value ? 
            <Tooltip title='Employee has set up personalized access permissions for menus.'>
              <CheckBox disabled iconSize={18} value={e.value}/>
            </Tooltip>
            : '-'
          }
        </div>
      )
    },
    {
      label: '',
      name: 'action',
      width: '280px',
      alignment: 'center',
      cellRender: (e) => (
        <div className='flex gap-4 items-center'>
          <button type='button' className='edit-button flex items-center gap-1' onClick={() => handleSelectEmployee(e.data)}><EyeOutlined />View</button>
          <Tooltip title='Clear Cache'>
            <button 
              type='button'
              className='dlt-button flex items-center gap-1'
              onClick={() => handleClickRemoveCatchBtn(e.data)}
            >
              <DeleteOutlined />Cache
            </button>
          </Tooltip>
          <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Remove</button>
        </div>
      )
    },
  ]

  const handleRemoveCache = useCallback(() => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `auth/auth/userremovecache?EmployeeId=${editData?.EmployeeId}`,
    }).then((res) => {
      setShowRemovePopup(false)
      getData()
    }).catch((err) => {

    }).finally(() => {
      setActionLoading(false)
    })
  },[editData])

  const handleAddSubmit = (data) => {
    setLoading(true)
    axios({
      method: 'post',
      url:'tas/sysrole/adduser',
      data: {
        employeeId: data.Id,
        roleId: roleId
      }
    }).then((res) => {
      form.resetFields()
      getData()
      setShowModal(false)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/sysrole/removeuser/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const changeStatus = () => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: `tas/sysrole/accesspermission`,
      data: {
        id: editData.Id,
        readOnlyAccess: editData.ReadonlyAccess ? 0 : 1
      }
    }).then(() => {
      getData()
      setShowStatusPopup(false)
    }).catch((err) => {

    }).then(() => {
      setActionLoading(false)
    })
  }

  const handleSearch = (values) => {
    setSearchLoading(true)
    tableSearch(values, data?.RoleUsers).then((res) => {
      setRenderData(res)
      setSearchLoading(false)
    }).catch(() => setSearchLoading(false))
  }

  const fields = [
    {
      label: "Firstname",
      name: 'Firstname',
      className: 'col-span-4 xl:col-span-3  mb-2',
    },
    {
      label: "Lastname",
      name: 'Lastname',
      className: 'col-span-4 xl:col-span-3 mb-2',
    },
    {
      label: "AD Account",
      name: 'ADAccount',
      className: 'col-span-4 xl:col-span-3 mb-2',
    },
  ]

  const items = [
    {
      key: '1',
      label: `Members`,
      children: <div className='py-4 '>
        <div className='rounded-ot px-1 mb-3'>
          <h4>Search member</h4>
          <Form 
            form={form} 
            fields={fields} 
            className='grid grid-cols-12 gap-x-8 mt-2' 
            onFinish={handleSearch}
            noLayoutConfig={true}
          >
            <div className='col-span-12 xl:col-span-1 flex justify-end xl:justify-start xl:items-baseline'>
              <Button 
                htmlType={'submit'}
                className='flex items-center' 
                loading={searchLoading} 
                icon={<SearchOutlined/>}
              >
                Search
              </Button>
            </div>
          </Form>
        </div>
        <Table
          data={renderData}
          columns={columns}
          allowColumnReordering={false}
          loading={false}
          containerClass='shadow-none pb-0 pt-0 border overflow-hidden'
          tableClass='max-h-[calc(100vh-355px)]'
          keyExpr='Id'
          title={
            !state.userInfo?.ReadonlyAccess &&
            <div className='flex justify-end py-2 px-3 border-b'>
              <Button 
                onClick={(e) => setShowModal(true)} 
                className='flex items-center' 
                loading={searchLoading}
                icon={<PlusOutlined/>}
              >
                Add Member
              </Button>
            </div>
          }
        />
        <Modal 
          width={1200} 
          open={showModal} 
          onCancel={() => setShowModal(false)} 
          title='People Search please select a person' 
          forceRender={true}
        >
          <PeopleSearch
            showTitle={false}
            onSelect={handleAddSubmit}
            tableClass='max-h-[500px]'
          />
        </Modal>
      </div>
    },
    {
      key: '2',
      label: `Menu`,
      children: <Menu/>
    },
    {
      key: '3',
      label: `Report`,
      children: <div className='py-4'>
        <RoleReportPermission/>
      </div>
    },
  ];

  return (
    <div>
      <div className='rounded-ot bg-white shadow-md px-3'>
        <div className='text-lg font-bold py-3'>{data?.Name}</div>
        <Tabs items={items} type='card' size='small'/>
      </div>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to remove this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type='danger' onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
      {/* <Popup
        visible={showStatusPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to {editData?.ReadonlyAccess ? <span className='text-green-500 font-bold'>change full access</span> : <span className='text-gray-400 font-bold'>read only access</span>}?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type='primary' onClick={changeStatus}>Yes</Button>
          <Button onClick={() => setShowStatusPopup(false)}>No</Button>
        </div>
      </Popup> */}
      <Popup
        visible={showRemovePopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to remove cache?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type='primary' onClick={handleRemoveCache} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowRemovePopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
      <EmployeeDetail
        selectedEmployee={selectedEmployee}
        open={openDrawer}
        onClose={() => setOpenDrawer(false)}
        roleData={data}
        refreshData={getData}
      />
    </div>
  )
}

export default RoleDetail