import { Form, Table, Button, Modal, AuditTable } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useCallback, useContext, useEffect, useState } from 'react'
import { Form as AntForm, Select } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'

const {Option} = Select;

const title = 'Shift Status'

function ShiftStatus() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ colors, setColors ] = useState([])
  const [ showAudit, setShowAudit ] = useState(false)
  const [ record, setRecord ] = useState(null)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  
  useEffect(() => {
    action.changeMenuKey('/tas/shiftstatus')
    getData()
    getColors()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/shift'
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

  const getReferDataShift = () => {
    action.changeLoadingStatusReferItem({ roomStatuses: true })
    axios({
      method: 'get',
      url: 'tas/shift?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({value: item.Id, label: `${item.Code} - ${item.Description}`, ...item}))
      action.setReferDataItem({
        roomStatuses: tmp
      })
    }).catch((err) => {
  
    }).then(() => action.changeLoadingStatusReferItem({ roomStatuses: false }))
  }

  const getColors = () => {
    axios({
      method: 'get',
      url: 'tas/color'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.Id, label: <div><span className='mr-2 h-[15px] w-[40px]' style={{background: item.Code}}></span>{item.Description}</div>})
      })
      setColors(res.data)
    })
  }
  
  const handleAddButton = () => {
    setEditData(null)
    form.resetFields()
    setShowModal(true)
  }
  
  const handleEditButton = useCallback((dataItem) => {
    setEditData(dataItem)
    setShowModal(true)
  },[])

  const handleDeleteButton = useCallback((dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  },[])

  const handleRowAudit = useCallback((row) => {
    setRecord(row)
    setShowAudit(true)
  },[])
  
  const columns = [
    {
      label: '',
      name: 'ColorCode',
      width: '50px',
      cellRender: (e) => (
        <div className={`w-[30px] h-[13px]`} style={{background: e.data.ColorCode}}></div>
      )
    },
    {
      label: 'Code',
      name: 'Code',
      width: '80px'
    },
    {
      label: 'Description',
      name: 'Description'
    },
    {
      label: 'OnSite',
      name: 'OnSite',
      width: '60px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.OnSite === 1 ? true : 0}/>
      )
    },
    {
      label: 'Default',
      name: 'isDefault',
      width: '60px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.isDefault === 1 ? true : 0}/>
      )
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
        url:'tas/shift',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        getReferDataShift()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/shift',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        getData()
        getReferDataShift()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/shift`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      getReferDataShift()
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

  const searchFields = [
    {
      label: 'Code',
      name: 'Code',
      className: 'col-span-6 lg:col-span-2 2xl:col-span-2 mb-2',
      inputprops: {
      }
    },
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-6 lg:col-span-3 2xl:col-span-3 mb-2'
    },
    {
      label: 'isDefault',
      name: 'isDefault',
      className: 'col-span-6 lg:col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-6 lg:col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
    {
      label: 'On Site',
      name: 'OnSite',
      className: 'col-span-6 lg:col-span-2 2xl:col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

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
      type: 'component',
      component: <Form.Item name='ColorId' className='col-span-12 mb-2' label='Color' rules={[{required: true, message: 'Color is required'}]}>
        <Select>
          {
            colors.map((item) => (
              <Option value={item.Id}>
                <div className='flex items-center gap-2'>
                  <div className='w-[40px] h-[13px]' style={{background: item.Code}}></div>
                  {item.Description}
                </div>
              </Option>
            ))
          }
        </Select>
      </Form.Item>
    },
    {
      label: 'On Site',
      name: 'OnSite',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Default',
      name: 'isDefault',
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
      type: 'component',
      component: <Form.Item name='ColorId' className='col-span-12 mb-2' label='Color' rules={[{required: true, message: 'Color is required'}]}>
        <Select>
          {
            colors.map((item) => (
              <Option value={item.Id}>
                <div className='flex items-center gap-2'>
                  <div className='w-[40px] h-[13px]' style={{background: item.Code}}></div>
                  {item.Description}
                </div>
              </Option>
            ))
          }
        </Select>
      </Form.Item>
    },
    {
      label: 'On Site',
      name: 'OnSite',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Default',
      name: 'isDefault',
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
          initValues={{
            isDefault: null,
            OnSite: null,
            Active: 1
          }}
        >
          <div className='flex gap-4 items-baseline col-span-6 lg:col-span-1'>
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
        pager={{showPageSizeSelector: false, showNavigationButtons: false}}
        tableClass='border-t max-h-[calc(100vh-250px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
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
        title='Shift Status Audit' 
        tablename={'Shift'} 
        open={showAudit} 
        onClose={() => setShowAudit(false)}
        record={record}
        recordName={record ? <span className='text-gray-500 ml-2'>/{record.Code} {record.Description}/</span> : ''}
      />
    </div>
  )
}

export default ShiftStatus