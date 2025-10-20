import { ActionForm, Form, Table, CustomSegmented, Button, Modal } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { useNavigate, useParams } from 'react-router-dom'
import { LeftOutlined, PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'

const title = 'Bed'

function Bed() {
  const [ data, setData ] = useState([])
  const [ rooms, setRooms ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { roomId } = useParams()
  const navigate = useNavigate()

  useEffect(() => {
    getData()
    getRooms()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/bed?roomId=${roomId}`
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
  
  const getRooms = () => {
    axios({
      method: 'get',
      url: 'tas/room?Active=1'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({label: item.Number, value: item.Id}))
      setRooms(tmp)
    }).catch((err) => {
  
    })
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
      label: 'Description',
      name: 'Description',
      width: '150px',
    },
    {
      label: 'RoomNumber',
      name: 'RoomNumber',
      width: '150px',
    },
    {
      label: 'CampName',
      name: 'CampName',
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
        url:'tas/bed',
        data: {
          Description: values.Description,
          RoomId: roomId,
          Active: values.Active ? 1 : 0,
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
        url:'tas/bed',
        data: {
          Description: values.Description,
          RoomId: roomId,
          Active: values.Active ? 1 : 0,
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
      url: `tas/bed`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = (values) => {
    tableSearch(values, data).then((res) => {
      setRenderData(res)
    })
  }

  const fields = [
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-12 mb-2'
    },
  ]

  const addFields = [
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-12 mb-2'
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
      label: 'Description',
      name: 'Description',
      className: 'col-span-12 mb-2'
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

  const handleCancel = () => {
    setShowModal(false)
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <div className='w-1/3'>
          <Form form={searchForm} fields={searchFields} size='small' className='grid grid-cols-12 gap-x-8' onFinish={handleSearch}>

          </Form>
          <div className='flex gap-4 justify-end'>
            <Button onClick={() => searchForm.submit()} icon={<SearchOutlined/>}>Search</Button>
          </div>
        </div>
      </div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        tableClass='border-t max-h-[calc(100vh-260px)]'
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
    </div>
  )
}

export default Bed