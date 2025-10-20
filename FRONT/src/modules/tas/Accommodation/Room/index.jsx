import { Form, Table, Button, Modal, AuditTable } from 'components'
import { CheckBox, Popup, Tooltip } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { Link, useNavigate } from 'react-router-dom'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'

const title = 'Room'

function Room() {
  const [ data, setData ] = useState([])
  const [ roomTypes, setRoomTypes ] = useState([])
  const [ camps, setCamps ] = useState([])
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
  const { action, state } = useContext(AuthContext)

  const navigate = useNavigate()

  useEffect(() => {
    getData()
    getRoomTypes()
    getCamps()

    return () => {
      axios.CancelToken.source().cancel()
    } 
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/room'
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

  const getReferDataRoom = () => {
    action.changeLoadingStatusReferItem({ rooms: false })
    axios({
      method: 'get',
      url: 'tas/room?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({value: item.Id, label: `${item.Number} - ${item.RoomTypeName}`, ...item}))
      action.setReferDataItem({
        rooms: tmp
      })
    }).catch((err) => {
  
    }).then(() => action.changeLoadingStatusReferItem({ rooms: false }))
  }
  
  const getRoomTypes = () => {
    axios({
      method: 'get',
      url: 'tas/roomtype?Active=1'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({label: item.Description, value: item.Id}))
      setRoomTypes(tmp)
    }).catch((err) => {
  
    })
  }

  const getCamps = () => {
    axios({
      method: 'get',
      url: 'tas/camp?Active=1'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({label: item.Description, value: item.Id}))
      setCamps(tmp)
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

  const handleRowAudit = (row) => {
    setRecord(row)
    setShowAudit(true)
  }

  const columns = [
    {
      label: 'Room Number',
      name: 'Number',
      width: '150px'
    },
    {
      label: 'Camp Name',
      name: 'CampName'
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      width: '80px',
      alignment: 'left'
    },
    {
      label: 'Room Type',
      name: 'RoomTypeName',
      alignment: 'left'
    },
    {
      label: 'Resource #',
      name: 'EmployeeCount',
      alignment: 'left'
    },
    // {
    //   label: 'No Room',
    //   name: 'VirtualRoom',
    //   width: '100px',
    //   alignment: 'center',
    //   cellRender: (e) => (
    //     <CheckBox disabled iconSize={18} value={e.data.VirtualRoom === 1 ? true : 0}/>
    //   )
    // },
    {
      label: 'Private',
      name: 'Private',
      width: '60px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Private === 1 ? true : 0}/>
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
      name: '',
      width: '80px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <Link to={`/tas/room/${e.data.Id}`}>
            <button type='button' className='edit-button'>Beds</button>
          </Link>
        </div>
      )
    },
    {
      label: '',
      name: 'action',
      width: '265px',
      cellRender: (e) => (
        <div className='flex gap-3'>
          <button type='button' className='edit-button' onClick={() => handleRowAudit(e.data)}>View Audit</button>
          {
            state.referData?.noRoomId?.Id !== e.data.Id &&
            <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          }
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
                    <div>Don't deactivate because there is an active Bed Count</div>
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
        url:'tas/room',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        getReferDataRoom()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/room',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        getData()
        getReferDataRoom()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/room`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      getReferDataRoom()
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
      label: 'Room Number',
      name: 'Number',
      rules: [{required: true, message: 'Room Number is required'}],
      className: 'col-span-12 mb-2',
      inputprops: {
        maxLength: 30
      } 
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      rules: [{required: true, message: 'Bed Count is required'}],
      className: 'col-span-12 mb-2',
      type: 'number',
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      rules: [{required: true, message: 'Room Type is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: roomTypes
      }
    },
    {
      label: 'Camp',
      name: 'CampId',
      rules: [{required: true, message: 'Camp is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: camps
      }
    },
    {
      label: 'Private',
      name: 'Private',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
  ]

  const addFields = [
    {
      label: 'Room Number',
      name: 'Number',
      rules: [{required: true, message: 'Room Number is required'}],
      className: 'col-span-12 mb-2',
      inputprops: {
        maxLength: 30
      } 
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      rules: [{required: true, message: 'Bed Count is required'}],
      className: 'col-span-12 mb-2',
      type: 'number',
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      rules: [{required: true, message: 'Room Type is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: roomTypes
      }
    },
    {
      label: 'Camp',
      name: 'CampId',
      rules: [{required: true, message: 'Camp is required'}],
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: camps
      }
    },
    {
      label: 'Private',
      name: 'Private',
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
      // hide: true,
      inputprops: {
      }
    },
  ]

  const searchfields = [
    {
      label: 'Room Number',
      name: 'Number',
      className: 'col-span-12 lg:col-span-3 2xl:col-span-2 mb-2'
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      className: 'col-span-12 lg:col-span-3 2xl:col-span-2 mb-2',
      type: 'number',
      inputprops: {
        className: 'w-full'
      }
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      className: 'col-span-12 lg:col-span-3 2xl:col-span-2 mb-2',
      type: 'select',
      inputprops: {
        options: roomTypes
      }
    },
    {
      label: 'Camp',
      name: 'CampId',
      className: 'col-span-12 lg:col-span-3 2xl:col-span-2 mb-2',
      type: 'select',
      inputprops: {
        options: camps
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
    {
      label: 'Private',
      name: 'Private',
      className: 'col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
    {
      label: 'No Room',
      name: 'VirtualRoom',
      className: 'col-span-12 lg:col-span-2 2xl:col-span-1 mb-2',
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
          className='grid grid-cols-12 gap-x-8 ' 
          onFinish={handleSearch}
          noLayoutConfig={true}
          size='small'
          initValues={{
            Active: 1,
            Private: null,
            VirtualRoom: null,
          }}
        >
          <div className='col-span-1 flex items-baseline'>
            <Button htmlType='submit' loading={loading} icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        onRowDblClick={(e) => navigate(`/tas/room/${e.data.Id}`)}
        tableClass='border-t max-h-[calc(100vh-270px)]'
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
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>
            No
          </Button>
        </div>
      </Popup>
      <AuditTable
        title='Room Audit' 
        tablename={'Room'} 
        open={showAudit} 
        onClose={() => setShowAudit(false)}
        record={record}
        recordName={record ? <span className='text-gray-500 ml-2'>/{record.Code} {record.Description}/</span> : ''}
      />
    </div>
  )
}

export default Room