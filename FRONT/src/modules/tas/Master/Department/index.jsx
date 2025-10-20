import { ActionForm, CustomSegmented, Form, TreeTable, Button, Modal, AuditTable, DepartmentTooltip } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm, Segmented, message } from 'antd'
import axios from 'axios'
import { SearchOutlined, PlusOutlined, LoadingOutlined, TableOutlined, PartitionOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'
import { Link } from 'react-router-dom'

const title = 'Department'

const getDepartmentLevelName = (level) => {
  switch (level) {
    case 0: return 'Subdepartment'
    case 1: return 'OPS responbility'
    case 2: return 'General department'
    case 3: return 'OTUP Grouping'
    case 4: return 'Division'
  }
}

function Department() {
  const [ data, setData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ currentMode, setCurrentMode ] = useState('search')
  const [ showModal, setShowModal ] = useState(false)
  const [ parentData, setParentData ] = useState(null)
  const [ showAudit, setShowAudit ] = useState(false)
  const [ record, setRecord ] = useState(null)

  const [ form ] = AntForm.useForm()
  const [ addForm ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)
  const treeRef = useRef(null)

  useEffect(() => {
    action.changeMenuKey('/tas/department')
    getData()
  },[])

  const getData = () => {
    treeRef.current?.instance.beginCustomLoading();
    axios({
      method: 'get',
      url: 'tas/department'
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => treeRef.current?.instance.endCustomLoading())
  }

  const getReferDataDepartment = () => {
    action.changeLoadingStatusReferItem({ departments: true })
    axios({
      method: 'get',
      url: 'tas/department?Active=1',
    }).then((res) => {
      action.setReferDataItem({
        departments: res.data
      })
    }).catch((err) => {
  
    }).then(() => action.changeLoadingStatusReferItem({ departments: false }))
  }
  
  const handleAddButton = (rowData, row) => {
    setEditData(null)
    setParentData({...rowData, level: row?.row?.level})
    setShowModal(true)
  }

  const handleEditButton = (rowData, row) => {
    setParentData(null)
    setEditData({...rowData, level: row?.row?.level})
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const handleRowAudit = (row) => {
    setRecord(row)
    setShowAudit(true)
  } 

  const columns = [
    {
      label: 'Department Name',
      name: 'Name',
      width: 'auto',
      cellRender: (e) => (
        <DepartmentTooltip trigger='click' id={e.data.Id} showStatus={false}>
          <span className='text-blue-500 hover:text-blue-400 transition-all cursor-pointer'>{e.value}</span>
        </DepartmentTooltip>
      )
    },
    {
      label: 'Level',
      name: 'level',
      cellRender: (e) => (
        <span>{getDepartmentLevelName(e.row?.level)}</span>
      )
    },
    {
      label: 'Cost Code',
      name: 'CostCodeDescr', 
    },
    {
      label: 'Admin',
      name: 'Admins', 
    },
    {
      label: 'Approval',
      name: 'Managers',
    },
    {
      label: 'Supervisers',
      name: 'Supervisers',
    },
    {
      label: '# Resource',
      name: 'EmployeeCount'
    },
    {
      label: 'Active',
      name: 'Active',
      width: '90px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Active === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '395px',
      cellRender: (e) => (
        <div className='flex gap-4 justify-end'>
          <button type='button' className='edit-button' onClick={() => handleRowAudit(e.data)}>View Audit</button>
          { e.row.level < 5 ?
            <button
              type='button' 
              className='edit-button flex items-center px-2' 
              onClick={() => handleAddButton(e.data, e)}
            >
              <PlusOutlined/>
            </button>
            : null
          }
          {
            e.data.ParentDepartmentId ?
            <button 
              type='button' 
              className='edit-button flex items-center gap-2' 
              onClick={() => handleEditButton(e.data, e)}
            >
              Edit
            </button>
            : null

          }
          <Link to={`${e.data.Id}`} target='_blank'>
            <button 
              type='button' 
              className='edit-button flex items-center gap-2' 
            >
              View
            </button>
          </Link>
          {
            e.data.Active === 1 ?
            <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Deactivate</button>
            :
            <button type='button' className='scs-button' onClick={() => handleDeleteButton(e.data)}>Reactivate</button>
          }
        </div>
      )
    },
  ]

  const handleDelete = () => {
    axios({
      method: 'delete',
      url: `tas/department`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      getReferDataDepartment()
      setShowPopup(false)
    }).catch((err) => {

    })
  }

  const handleSearch = (values) => {
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/department?DepartmentName=${values.DepartmentName}&keyword=${values.keyword}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setSearchLoading(false))
  }

  const fields = [
    {
      label: 'Department Name',
      name: 'DepartmentName',
      className: 'col-span-3 mb-2',
      rules: [{required: currentMode === 'edit' ? true : false, message: 'Department Name is required'}]
    },
    {
      label: 'Related Employees',
      name: 'keyword',
      className: 'col-span-3 mb-2',
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: currentMode === 'edit' ? false : true,
      }
    },
  ]

  const addfields = [
    {
      label: 'Parent Department',
      name: 'ParentDepartmentId',
      className: 'col-span-12 mb-2',
      type: 'treeSelect',
      inputprops: {
        treeData: state.referData?.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      }
    },
    {
      label: 'Department Name',
      name: 'Name',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Department Name is required'}]
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: { 
      }
    },
  ]

  const handleSubmitAdd = (values) => {
    if(editData){
      axios({
        method: 'put',
        url:'tas/department',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
          Id: editData?.Id
        }
      }).then((res) => {
        getData()
        getReferDataDepartment()
        setShowModal(false)
        setCurrentMode('search')
        form.resetFields()
        setEditData(null)
      }).catch((err) => {
  
      })
    }else{
      
      axios({
        method: 'post',
        url:'tas/department',
        data: {
          ...values,
          ParentDepartmentId: parentData?.Id,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        getData()
        getReferDataDepartment()
        setShowModal(false)
        addForm.resetFields()
        setParentData(null)
      }).catch((err) => {
  
      })
    }
  }

  const handleTableAudit = () => {
    setRecord(null)
    setShowAudit(true)
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <Form 
          form={form} 
          fields={fields}
          className='grid grid-cols-12 lg:gap-x-8' 
          onFinish={handleSearch}
          noLayoutConfig={true}
        >
          <div className='flex gap-4 items-baseline col-span-2'>
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
      <TreeTable
        ref={treeRef}
        title={<div className='flex items-center justify-end gap-3 bg-white py-2 px-3 rounded-t-ot'>
          <Button onClick={handleTableAudit}>Audit</Button>
        </div>}
        dataSource={data}
        itemsExpr={'ChildDepartments'}
        columns={columns}
        className='bg-white p-2 pt-0 shadow-md border-t h-[calc(100vh-250px)]'
        dataStructure='tree'
        autoExpandAll={true}
        paging={{enabled: false}}
        pager={{enabled: false}}
      />
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to {editData?.Active === 1 ? 'deactivate' : 'reactivate'} this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={editData?.Active === 1 ? 'danger' : 'success'} onClick={handleDelete}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
      <Modal 
        title={<div>
          {
            parentData ? 
            <div className='text-gray-500 text-sm'>({getDepartmentLevelName(parentData?.level)})</div>
            :
            <div className='text-gray-500 text-sm'>({getDepartmentLevelName(editData?.level)})</div>
          }
            <div className='mr-5 whitespace-pre-wrap'>{editData ? 'Edit' : 'Add'} department on {editData ? editData?.Name : parentData?.Name}</div>
          </div>
        } 
        open={showModal} 
        onCancel={() => setShowModal(false)} 
        destroyOnClose={true}
      >
        <Form 
          form={addForm}
          fields={addfields}
          editData={editData ? editData : {ParentDepartmentId: parentData?.Id}}
          onFinish={handleSubmitAdd}  
          size='small'
        />
        <div className='flex gap-5 justify-end'>
          <Button type={'primary'} onClick={() => addForm.submit()}>Save</Button>
          <Button onClick={() => setShowModal(false)}>
            Cancel
          </Button>
        </div>
      </Modal>
      <AuditTable
        title='Department Audit' 
        tablename={'Department'} 
        open={showAudit} 
        onClose={() => setShowAudit(false)}
        record={record}
        recordName={record ? <span className='text-gray-500 ml-2'>/{record.Name}/</span> : ''}
      />
    </div>
    
  )
}

export default Department