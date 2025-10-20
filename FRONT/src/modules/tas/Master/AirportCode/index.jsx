import { Form, Table, Button, Modal } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'  

const title = 'Airport Code'

const fields = [
  {
    label: 'Code',
    name: 'Code',
    className: 'col-span-12 mb-2',
    rules: [{required: true, message: 'Code is required'}], 
    inputprops: {
      maxLength: 10
    }
  },
  {
    label: 'Description',
    name: 'Description',
    className: 'col-span-12 mb-2',
    rules: [{required: true, message: 'Description is required'}], 
  },
  {
    label: 'Country',
    name: 'Country',
    className: 'col-span-12 mb-2',
    rules: [{required: true, message: 'Country is required'}], 
  },
]

const searchFields = [
  {
    label: 'Code',
    name: 'Code',
    className: 'col-span-2 mb-2',
    inputprops: {
    }
  },
  {
    label: 'Country',
    name: 'Country',
    className: 'col-span-2 mb-2',
  },
  {
    label: 'Description',
    name: 'Description',
    className: 'col-span-3 mb-2',
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

function AirportCode() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  const dataGrid = useRef(null)

  useEffect(() => {
    action.changeMenuKey('/tas/airportcode')
    getData()
  },[])

  const getData = () => {
    dataGrid.current?.instance.beginCustomLoading()
    axios({
      method: 'get',
      url: 'tas/requestairport'
    }).then((res) => {
      setData(res.data)
      setRenderData(res.data)
    }).catch((err) => {

    }).then(() => dataGrid.current?.instance.endCustomLoading())
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

  const columns = [
    {
      label: 'Country',
      name: 'Country',
    },
    {
      label: 'Description',
      name: 'Description'
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
      width: '170px',
      cellRender: (e) => (
        <div className='flex gap-4'>
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
        url:'tas/requestairport',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
        
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/requestairport',
        data: {
          ...values,
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/requestairport`,
      data: {
        Id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = async (values) => {
    setSearchLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setSearchLoading(false)
    })
  }

  const handleCancel = () => {
    setShowModal(false)
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
          <div className='flex gap-4 col-span-3 items-baseline'>
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
        ref={dataGrid}
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        keyExpr='Id'
        tableClass='border-t max-h-[calc(100vh-250px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{renderData.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add {title}</Button>
        </div>}
      />
      <Modal open={showModal} onCancel={() => setShowModal(false)} title={editData ? `Edit ${title}` : `Add ${title}`}>
        <Form 
          form={form}
          fields={fields}
          editData={editData} 
          onFinish={handleSubmit}
          loading={loading}
        >
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button type='primary' onClick={() => form.submit()} loading={loading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={handleCancel}>Cancel</Button>
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
    </div>
  )
}

export default AirportCode