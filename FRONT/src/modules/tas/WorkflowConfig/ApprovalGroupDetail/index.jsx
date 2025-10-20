import { Form, Table, Button, Modal } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'  
import { Link, useParams } from 'react-router-dom'

function ApprovalGroupDetail() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ groupEmployees, setGroupEmployees ] = useState([])
  const [ employees, setEmployees ] = useState([])
  const [ managers, setManagers ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ showPrimaryPopup, setShowPrimaryPopup ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  const { groupId } = useParams()

  useEffect(() => {
    action.changeMenuKey('/tas/approvalgroups')
    getEmployeeList()
    getManagerList()
  },[])
  
  useEffect(() => {
    getData()
  },[groupId])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/requestgroupemployee/groupemployees/${groupId}`
    }).then((res) => {
      setData(res.data)
      setGroupEmployees(res.data.Employees)
      setRenderData(res.data.Employees)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getEmployeeList = () => {
    axios({
      method: 'get',
      url: `tas/requestgroupemployee/activeemployees`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.EmployeeId, label: `${item.FullName} - ${item.RoleName}`})
      })
      setEmployees(tmp)
    }).catch((err) => {

    })
  }

  const getManagerList = () => {
    axios({
      method: 'get',
      url: `tas/department/managers`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.EmployeeId, label: `${item.FullName} - ${item.EmployeeId}`, ...item})
      })
      setManagers(tmp)
    }).catch((err) => {

    })
  }
  
  const handleAddButton = () => {
    setEditData(null)
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const handleChangePrimaryButton  = (dataItem) => {
    setEditData(dataItem)
    setShowPrimaryPopup(true)
  }


  const columns = [
    {
      label: 'Person #',
      name: 'EmployeeId',
      width: 80,
      alignment: 'left'
    },
    {
      label: 'SAPID#',
      name: 'SAPID',
      width: 80,
      alignment: 'left'
    },
    {
      label: 'Fullname',
      name: 'FullName',
      cellRender: (e) => (
        <div className='text-blue-400 hover:underline'><Link to={`/tas/people/search/${e.data.EmployeeId}`}>{e.value}</Link></div>
      )
    },
    {
      label: 'AD Account',
      name: 'ADAccount',
    },
    {
      label: 'PrimaryContact',
      name: 'PrimaryContact',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      // width: '290px',
      width: '220px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleChangePrimaryButton(e.data)}>Change Primary</button>
          <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url:'tas/requestgroupemployee/groupemployees',
      data: {
        ...values,
        groupId: groupId,
      }
    }).then((res) => {
      getData()
      form.resetFields()
      setShowModal(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleDelete = () => {
    setActionLoading(true)
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
  }

  const handleChangePrimary = () => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: `tas/requestgroupemployee/setprimary`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPrimaryPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = async (values) => {
    setSearchLoading(true)
    tableSearch(values, groupEmployees).then((res) => {
      setRenderData(res)
      setSearchLoading(false)
    })
  }

  const fields = [
    {
      label: 'Employee',
      name: 'EmployeeId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Employee is required'}],
      inputprops: {
        options: data?.GroupTag === 'linemanager' ? managers : employees,
        popupMatchSelectWidth: false
        // options: employees.filter((emp) => !groupEmployees.some((item) => emp.EmployeeId === item.Id)),
        // fieldNames: {value: 'EmployeeId', label: 'FullName'}
      } 
    },
  ]

  const searchFields = [
    {
      label: 'FullName',
      name: 'FullName',
      className: 'mb-0',
      inputprops: {
        style: {width: '250px'}
      }
    },
  ]

  const handleCancel = () => {
    setShowModal(false)
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>Members of {data.Name}</div>
        <Form 
          form={searchForm} 
          fields={searchFields} 
          className='flex items-center gap-x-5'
          onFinish={handleSearch}
          labelCol={{flex: '60px'}}
        >
          <div className='flex gap-4 justify-end col-span-12'>
            <Button 
              htmlType='submit' 
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
        loading={loading}
        keyExpr='Id'
        tableClass='border-t max-h-[calc(100vh-240px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{renderData.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add Member</Button>
        </div>}
      />
      <Modal open={showModal} onCancel={() => setShowModal(false)} title='Add Member'>
        <Form
          form={form}
          fields={fields}
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={'auto'}
        width={'auto'}
      >
        <div>Are you sure you want to delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
      <Popup
        visible={showPrimaryPopup}
        showTitle={false}
        height={'auto'}
        width={'auto'}
      >
        <div>Are you sure you want to change Primary status this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'success'} onClick={handleChangePrimary} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPrimaryPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default ApprovalGroupDetail