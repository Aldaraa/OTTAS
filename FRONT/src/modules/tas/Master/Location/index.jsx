import { ActionForm, Form, Table, CustomSegmented, Button, Modal, AuditTable } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'

const title = 'Location';

function Location() {
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
    action.changeMenuKey('/tas/location')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/location'
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

  const getReferDataLocation = () => {
    action.changeLoadingStatusReferItem({ locations: true })
    axios({
      method: 'get',
      url: 'tas/location?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({value: item.Id, label: `${item.Description}`, ...item}))
      action.setReferDataItem({
        locations: tmp
      })
    }).catch((err) => {
  
    }).then(() => action.changeLoadingStatusReferItem({ locations: false }))
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
      label: 'On Site',
      name: 'onSite',
      width: '60px',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.onSite === 1 ? true : 0}/>
      )
    },
    {
      label: 'Active',
      name: 'Active',
      width: '90px',
      alignment:'center',
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
            <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Deactivate</button>
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
        url:'tas/location',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        getReferDataLocation()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/location',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        getData()
        getReferDataLocation()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/location`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      getReferDataLocation()
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
      rules: [{required: true, message: 'Code is required'}],
      className: 'col-span-12 mb-2',
      inputprops: {
        maxLength: 20    
      }
    },
    {
      label: 'Description',
      name: 'Description',
      rules: [{required: true, message: 'Description is required'}],
      className: 'col-span-12 mb-2'
    },
    {
      label: 'Site',
      name: 'onSite',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
  ]

  const addFields = [
    {
      label: 'Code',
      name: 'Code',
      rules: [{required: true, message: 'Code is required'}],
      className: 'col-span-12 mb-2',
      inputprops: {
        maxLength: 20    
      }
    },
    {
      label: 'Description',
      name: 'Description',
      rules: [{required: true, message: 'Description is required'}],
      className: 'col-span-12 mb-2'
    },
    {
      label: 'Site',
      name: 'onSite',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
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

  const searchfields = [
    {
      label: 'Code',
      name: 'Code',
      className: 'col-span-3 mb-2',
      inputprops: {  
      }
    },
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-3 mb-2'
    },
    {
      label: 'Site',
      name: 'onSite',
      className: 'col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-1 mb-2',
      type: 'check',
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
          fields={searchfields} 
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSearch}
          noLayoutConfig={true}
        >
          <div className='flex gap-4 items-baseline col-span-1'>
            <Button 
              htmlType='submit' 
              className='flex items-center' 
              loading={loading} 
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
        pager={{showPageSizeSelector: false, showNavigationButtons: false}}
        tableClass='border-t max-h-[calc(100vh-282px)]'
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
        title='Location Audit' 
        tablename={'Location'} 
        open={showAudit} 
        onClose={() => setShowAudit(false)}
        record={record}
        recordName={record ? <span className='text-gray-500 ml-2'>/{record.Code} {record.Description}/</span> : ''}
      />
    </div>
  )
}

export default Location