import { Form, Table, Button, Modal, AuditTable } from 'components'
import { CheckBox, Popup, Tooltip } from 'devextreme-react'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'
import isArray from 'lodash/isArray'
import DataSource from 'devextreme/data/data_source'

const title = 'Position'

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
  {
    label: 'Active',
    name: 'Active',
    className: 'col-span-12 mb-2',
    type: 'check',
  },
]

const searchFields = [
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
    label: 'Active',
    name: 'Active',
    className: 'col-span-1 mb-2',
    type: 'check',
    inputprops: {
      indeterminatewith: true,
    }
  },
]

function Position() {
  const [ totalResults, setTotalResults ] = useState(0)
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showAudit, setShowAudit ] = useState(false)
  const [ record, setRecord ] = useState(null)
  const [ store, setStore ] = useState(new DataSource({
    key: 'Id',
    load: (loadOptions) => {
      dataGrid.current?.instance.beginCustomLoading();
      let params = '';
      if(loadOptions.sort?.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      return axios({
        method: 'post',
        url: `tas/position/getall${params}`,
        data: (loadOptions.filter && !isArray(loadOptions?.filter)) ? 
          {
            ...loadOptions.filter,
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take,
          } :
          {
            code: '',
            description: '',
            Active: 1,
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          },
        }).then((res) => {
          setTotalResults(res.data.totalcount)
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => dataGrid.current?.instance.endCustomLoading())
    }
  }))

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  const dataGrid = useRef(null)

  useEffect(() => {
    action.changeMenuKey('/tas/position')
  },[])

  const getReferDataPosition = () => {
    action.changeLoadingStatusReferItem({ positions: true })
    axios({
      method: 'get',
      url: 'tas/position?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({value: item.Id, label: `${item.Code} - ${item.Description}`, ...item}))
      action.setReferDataItem({
        positions: tmp
      })
    }).catch((err) => {
  
    }).then(() => action.changeLoadingStatusReferItem({ positions: false }))
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
    //   width: '100px'
    // },
    {
      label: 'Description',
      name: 'Description'
    },
    {
      label: '# Resource',
      name: 'EmployeeCount',
      alignment: 'left'
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
                    position="top"
                  >
                    <div>Don't deactivate because there is an active Employee</div>
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
        url:'tas/position',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        handleCancel()
        getReferDataPosition()
        store.reload()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/position',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        handleCancel()
        store.reload()
        getReferDataPosition()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/position`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      setShowPopup(false)
      store.reload()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = (values) => {
    dataGrid.current?.instance.filter(values)
  }

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
          <div className='flex gap-4 items-baseline col-span-2'>
            <Button htmlType='submit' icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Table
        ref={dataGrid}
        data={store}
        columns={columns}
        allowColumnReordering={false}
        cacheEnabled={false}
        remoteOperations={true}
        tableClass='border-t max-h-[calc(100vh-250px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{totalResults}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <div className='flex items-center gap-3'>
            <Button onClick={handleTableAudit}>Audit</Button>
            <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add {title}</Button>
          </div>
        </div>}
      />
      <Modal open={showModal} onCancel={handleCancel} title={editData ? `Edit ${title}` : `Add ${title}`}>
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
          <Button 
            type={editData?.Active === 1 ? 'danger' : 'success'} 
            onClick={handleDelete}
            loading={actionLoading}
          >
            Yes
          </Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>
            No
          </Button>
        </div>
      </Popup>
      <AuditTable
        title='Position Audit' 
        tablename={'Position'} 
        open={showAudit} 
        onClose={() => setShowAudit(false)}
        record={record}
        recordName={record ? <span className='text-gray-500 ml-2'>/{record.Code} {record.Description}/</span> : ''}
      />
    </div>
  )
}

export default Position