import { ActionForm, Form, Table, CustomSegmented, Button, Modal } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { SaveFilled, LeftOutlined, SaveOutlined, OrderedListOutlined, PlusOutlined } from '@ant-design/icons'
import { useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'

const title = 'Roster Master'

function RosterMaster() {
  const [ rosterDetail, setRosterDetail ] = useState(null)
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ shift, setShift ] = useState([])
  const [ rosterGroup, setRosterGroup ] = useState([])
  const [ isReorder, setIsReorder ] = useState(false)

  const {rosterId} =  useParams()
  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const navigate = useNavigate()
  const { action, state } = useContext(AuthContext)
  
  useEffect(() => {
    action.changeMenuKey('/tas/roster')
    getData()
    getRosterDetail()
    getShiftData()
    getRosterGroup()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/rosterdetail/${rosterId}`
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

  const getRosterDetail = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/roster/${rosterId}`
    }).then((res) => {
      setRosterDetail(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }
  
  const getShiftData = () => {
    axios({
      method: 'get',
      url: `tas/shift?Active=1`
    }).then((res) => {
      let tmp = []
      res.data.map((item, i) => {
        tmp.push({ value: item.Id, label: `${item.Code} - ${item.Description}`})
      })
      setShift(tmp)
    }).catch((err) => {
  
    })
  }

  const getRosterGroup = () => {
    axios({
      method: 'get',
      url: 'tas/rostergroup?Active=1'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({label: item.Description, value: item.Id}))
      setRosterGroup(tmp)
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
      label: 'Seq Number',
      name: 'SeqNumber',
      alignment: 'left',
      width: 90
    },
    {
      label: 'Nights',
      name: 'DaysOn',
      alignment: 'left',
      width: 60
   
    },
    {
      label: 'Shift Status',
      name: 'ShiftName',
    },
    {
      label: 'On Site',
      name: 'OnSite',
      width: '100px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.OnSite === 1 ? true : 0}/>
      )
    },
    {
      label: 'Active',
      name: 'Active',
      width: '70px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.OnSite === 1 ? true : 0}/>
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
        url:'tas/rosterdetail',
        data: {
          ...values,
          Id: editData.Id,
          rosterId: parseInt(rosterId),
        }
      }).then((res) => {
        getData()
        setShowModal(false)
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/rosterdetail',
        data: {
          ...values,
          rosterId: parseInt(rosterId)
        }
      }).then((res) => {
        getData()
        setShowModal(false)
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/rostergroup`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleEditSave = (values) => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: `tas/roster`,
      data: {
        ...values,
        id: rosterId
      }
    }).then(() => {
      getRosterDetail()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const searchFields = [
    {
      label: 'Name',
      name: 'Name',
      className: 'col-span-2 mb-2'
    },
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-3 mb-2'
    },
    {
      label: 'Roster Group',
      name: 'RosterGroupId',
      className: 'col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options: rosterGroup
      }
    },
    // {
    //   label: 'On Site',
    //   name: 'onSite',
    //   className: 'col-span-12 mb-2',
    //   type: 'check',
    // },
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

  const fields = [
    {
      label: 'Shift',
      name: 'ShiftId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: shift
      }
    },
    {
      label: 'Seq Number',
      name: 'SeqNumber',
      className: 'col-span-12 mb-2',
      type: 'number',
    },
    {
      label: 'Nights',
      name: 'DaysOn',
      className: 'col-span-12 mb-2',
      type: 'number',
    },
  ]

  const addFields = [
    {
      label: 'Shift',
      name: 'ShiftId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: shift
      }
    },
    {
      label: 'Seq Number',
      name: 'SeqNumber',
      className: 'col-span-12 mb-2',
      type: 'number',
    },
    {
      label: 'Nights',
      name: 'DaysOn',
      className: 'col-span-12 mb-2',
      type: 'number',
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 mb-2',
      type: 'check',
    },
  ]

  const onReorder = (e) => {
    const visibleRows = e.component.getVisibleRows();
    const newTasks = [...renderData];

    const toIndex = newTasks?.findIndex((item) => item.Id === visibleRows[e.toIndex].data.Id);
    const fromIndex = newTasks?.findIndex((item) => item.Id === e.itemData.Id);

    newTasks.splice(fromIndex, 1);
    newTasks.splice(toIndex, 0, e.itemData);

    setRenderData(newTasks)
  }

  const handleCancelReorder = () => {
    setRenderData(data)
    setIsReorder(false)
  }

  const handleSaveReorder = () => {
    let rosterDetailIds = []
    renderData.map((item) => {
      rosterDetailIds.push(item.Id)
    })
    axios({
      method: 'post',
      url: 'tas/rosterdetail/setseq',
      data: {
        rosterDetailIds: rosterDetailIds,
        rosterId: rosterId,
      }
    }).then((res) => {
      getData()
      setIsReorder(false)
    }).catch((err) => {

    })
  }

  return (
    <div>
      <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
        <Button className='mb-5' onClick={() => navigate(-1)} icon={<LeftOutlined/>}>Back</Button>
        <div className='text-lg font-bold mb-3'>Roster Detail</div>
          <Form 
            form={searchForm}
            fields={searchFields} 
            editData={rosterDetail}
            className='grid grid-cols-12 gap-x-8' 
            onFinish={handleEditSave}
            disabled={state.userInfo?.ReadonlyAccess}
          >
            <div className='flex gap-3 items-baseline col-span-1'>
              { !state.userInfo?.ReadonlyAccess &&
                <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveFilled/>}>Save</Button>
              }
            </div>

          </Form>
      </div>
      <Table
        title={<div className='flex justify-between items-center py-2 px-1'>
        <div className='text-lg font-bold'>{title}</div>
        {
          !state.userInfo?.ReadonlyAccess &&
          (
            isReorder ? 
            <div className='flex items-center gap-4'>
              <Button className='mb-2' type='primary' icon={<SaveOutlined />} onClick={handleSaveReorder}>Save Reorder</Button>
              <Button className='mb-2' onClick={handleCancelReorder}>Cancel</Button>
            </div>
            :
            <div className='flex gap-2 items-center'>
              <Button icon={<OrderedListOutlined />} onClick={() => setIsReorder(true)}>Change Seq</Button>
              <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add {title}</Button>
            </div>
          )
        }
      </div>}
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        tableClass='border-t max-h-[calc(100vh-282px)]'
        rowDragging={isReorder ? {
          onReorder:(e) => onReorder(e),
        } : false}
      />
      <Modal open={showModal} onCancel={() => setShowModal(false)} title={editData ? `Edit ${title}` : `Add ${title}`}>
        <Form 
          form={form}
          fields={editData ? fields : addFields}
          editData={editData ? editData : {SeqNumber: data.length+1}} 
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={() => setShowModal(false)} disabled={actionLoading}>Cancel</Button>
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

export default RosterMaster