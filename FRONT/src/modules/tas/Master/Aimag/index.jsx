import { Form, Table, Button, Modal, AuditTable } from 'components'
import { CheckBox, Popup, Tooltip } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'

const title = 'Aimag'

function Aimag() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showAudit, setShowAudit ] = useState(false)
  const [ record, setRecord ] = useState(null)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  
  useEffect(() => {
    action.changeMenuKey('/tas/aimag')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/state'
    }).then((res) => {
      setData(res.data)
      if(searchForm.getFieldValue('Active') === 1){
        let tmp = res.data.filter((item) => item.Active === 1)
        setRenderData(tmp)
      }else if(searchForm.getFieldValue('Active') === 0){
        let tmp = res.data.filter((item) => item.Active === 0)
        setRenderData(tmp)
      }else{
        setRenderData(res.data)
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getReferDataState = () => {
    action.changeLoadingStatusReferItem({ states: true })
    axios({
      method: 'get',
      url: 'tas/state?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({value: item.Id, label: `${item.Code} - ${item.Description}`, ...item}))
      action.setReferDataItem({
        states: tmp
      })
    }).catch((err) => {
  
    }).then(() => action.changeLoadingStatusReferItem({ states: false }))
  }
  
  const handleAddButton = () => {
    setEditData(null)
    form.resetFields()
    setShowModal(true)
  }
  
  const handleEditButton = (dataItem) => {
    setEditData(dataItem)
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
    // {
    //   label: 'Code',
    //   name: 'Code',
    //   width: '80px'
    // },
    {
      label: 'Description',
      name: 'Description'
    },
    {
      label: '# Resource',
      name: 'EmployeeCount',
      alignment: 'left',
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
      width: '265px',
      cellRender: (e) => (
        <div className='flex gap-3'>
          <button type='button' className='edit-button' onClick={() => handleRowAudit(e.data)}>View Audit</button>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          {
            e.data.Active === 1 ?
            e.data.EmployeeCount > 0 ? 
            <div>
              <div id={`target${e.data.Id}`} className="dlt-button_lbl">Deactivate</div> 
                  <Tooltip
                    target={`#target${e.data.Id}`}
                    showEvent="mouseenter"
                    hideEvent="mouseleave"
                    hideOnOutsideClick={false}
                  >
                    <div>Don't deactivate because there is an active Employee Count</div>
                </Tooltip> 
            </div>:
            <button type='button' disabled={e.data.EmployeeCount > 0} className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Deactivate</button>
            :
            <button type='button' className='scs-button' onClick={() => handleDeleteButton(e.data)}>Reactivate</button>
          }
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    if(editData){
      axios({
        method: 'put',
        url:'tas/state',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        getReferDataState()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/state',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        getData()
        getReferDataState()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/state`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      getReferDataState()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = (values) => {
    setLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setLoading(false)
    })
  }

  
  const fields = [
    {
      label: 'Code',
      name: 'Code',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Code is required'}],
      inputprops: {
        maxLength: 20
      }
    },
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Description is required'}],
    },
  ]

  const addFields = [
    {
      label: 'Code',
      name: 'Code',
      rules: [{required: true, message: 'Code is required'}],
      className: 'col-span-12 mb-2',
      inputprops: {
        maxLength: 10
      }
    },
    {
      label: 'Description',
      name: 'Description',
      rules: [{required: true, message: 'Description is required'}],
      className: 'col-span-12 mb-2'
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
  
  const searchFields = [
    {
      label: 'Code',
      name: 'Code',
      className: 'col-span-3 mb-2',
      inputprops: {
        maxLength: 20
      }
    },
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-4 mb-2'
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-1 mb-2',
      type: 'check',
      // hide: true,
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]


  const handleCancel = () => {
    setShowModal(false)
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
          form={searchForm} 
          fields={searchFields} 
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSearch}
          noLayoutConfig={true}
        >
          <div className='flex gap-4 items-baseline col-span-1'>
              <Button htmlType='submit' className='flex items-center' loading={loading} icon={<SearchOutlined/>}>Search</Button>
            </div>
        </Form>
      </div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        pager={false}
        tableClass='border-t max-h-[calc(100vh-250px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{renderData.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <div className='flex items-center gap-3'>
            <Button onClick={handleTableAudit}>Audit</Button>
            <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add {title}</Button>
          </div>
        </div>}
      />
      <Modal open={showModal} onCancel={() => setShowModal(false)} title={editData ? `Edit ${title}` : `Add ${title}`}>
        <Form 
          form={form}
          fields={editData ? fields : addFields}
          editData={editData}
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
        height={110}
        width={350}
      >
        <div>Are you sure you want to {editData?.Active === 1 ? 'deactivate' : 'reactivate'} this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={editData?.Active === 1 ? 'danger' : 'success'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
      <AuditTable
        title='Aimag Audit' 
        tablename={'State'} 
        open={showAudit} 
        onClose={() => setShowAudit(false)}
        record={record}
        recordName={record ? <span className='text-gray-500 ml-2'>/{record.Code} {record.Description}/</span> : ''}
      />
    </div>
  )
}

export default Aimag