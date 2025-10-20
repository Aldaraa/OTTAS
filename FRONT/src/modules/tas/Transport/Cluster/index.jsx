import { ActionForm, Form, Table, CustomSegmented, Button, Modal } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm, Tag } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { Link, useNavigate } from 'react-router-dom'
import { AuthContext } from 'contexts'

const title = 'Cluster'

function Cluster() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  const dataref = useRef(null)
  const navigate = useNavigate()
  
  useEffect(() => {
    action.changeMenuKey('/tas/cluster')
    getData()
  },[])

  const getData = () => {
    // setLoading(true)
    dataref.current?.instance?.beginCustomLoading()
    axios({
      method: 'get',
      url: 'tas/cluster'
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

    }).then(() => dataref.current?.instance?.endCustomLoading())
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
      label: 'Code',
      name: 'Code',
    },
    {
      label: 'Description',
      name: 'Description'
    },
    {
      label: 'Week day',
      name: 'DayNum'
    },
    {
      label: 'Direction',
      name: 'Direction',
      cellRender: (e) => (
        <Tag color={e.value === 'IN' ? 'green' : 'blue'} className='w-[42px] text-center text-[10px] font-semibold'>{e.value}</Tag>
      )
    },
    {
      label: 'Detail Count',
      name: 'DetailCount',
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
      name: '',
      width: '80px',
      cellRender: (e) => (
        <div className='flex items-center'>
          <Link to={`/tas/cluster/${e.data.Id}`}>
            <button type='button' className='edit-button'>View</button>
          </Link>
        </div>

      )
    },
    {
      label: '',
      name: 'action',
      width: '170px',
      alignment: 'center',
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
        url:'tas/cluster',
        data: {
          ...values,
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
        url:'tas/cluster',
        data: {
          ...values,
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
      url: `tas/cluster`,
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
      label: 'Week day',
      name: 'DayNum',
      rules: [{required: true, message: 'Week day is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: [
          {value: 'Monday', label: 'Monday'},
          {value: 'Tuesday', label: 'Tuesday'},
          {value: 'Wednesday', label: 'Wednesday'},
          {value: 'Thursday', label: 'Thursday'},
          {value: 'Friday', label: 'Friday'},
          {value: 'Saturday', label: 'Saturday'},
          {value: 'Sunday', label: 'Sunday'},
        ]
      }
    },
    {
      label: 'Direction',
      name: 'Direction',
      rules: [{required: true, message: 'Direction is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: [
          {value: 'IN', label: 'IN'},
          {value: 'OUT', label: 'OUT'},
        ]
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
      label: 'Week day',
      name: 'DayNum',
      rules: [{required: true, message: 'Week day is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: [
          {value: 'Monday', label: 'Monday'},
          {value: 'Tuesday', label: 'Tuesday'},
          {value: 'Wednesday', label: 'Wednesday'},
          {value: 'Thursday', label: 'Thursday'},
          {value: 'Friday', label: 'Friday'},
          {value: 'Saturday', label: 'Saturday'},
          {value: 'Sunday', label: 'Sunday'},
        ]
      }
    },
    {
      label: 'Direction',
      name: 'Direction',
      rules: [{required: true, message: 'Direction is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: [
          {value: 'IN', label: 'IN'},
          {value: 'OUT', label: 'OUT'},
        ]
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
          initValues={{
            Active: 1
          }}
        >
          <div className='flex gap-4 items-baseline col-span-1'>
            <Button htmlType='submit' loading={loading} icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Table
        ref={dataref}
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        keyExpr='Id'
        onRowDblClick={(e) => navigate(`/tas/cluster/${e.data.Id}`)}
        tableClass='border-t max-h-[calc(100vh-245px)]'
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
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>
            No
          </Button>
        </div>
      </Popup>
    </div>
  )
}

export default Cluster