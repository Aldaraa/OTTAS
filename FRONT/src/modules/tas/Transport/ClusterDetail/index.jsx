import { Form, Table, Modal, Button } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { CloseOutlined, PlusOutlined, SaveOutlined, LeftOutlined, OrderedListOutlined } from '@ant-design/icons'
import { useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'

const title = 'Cluster Detail'

function ClusterDetail() {
  const [ data, setData ] = useState(null)
  const [ recorder, setRecorder ] = useState([])
  const [ transportType, setTransportType ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ isReorder, setIsReorder ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)

  const [ searchForm ] = AntForm.useForm()
  const [ form ] = AntForm.useForm()
  const {clusterId} = useParams()
  const { state } = useContext(AuthContext)
  const navigate = useNavigate()

  useEffect(() => {
    getData()
    getTransportType()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/cluster/${clusterId}`
    }).then((res) => {
      setData(res.data)
      setRecorder(res.data?.data || [])
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getTransportType = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/cluster/activetransports/${clusterId}`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.Id, label: `#${item.Id} • ${item.Code} • ${item.Description}`, ...item})
      })
      setTransportType(tmp)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }
  
  const handleClickAdd = () => {
    setShowModal(true)
  }

  const handleDeleteBtn = (row) => {
    setEditData(row)
    setShowPopup(true)
  }

  const columns = [
    {
      label: '# Seq',
      name: 'SeqNumber',
      width: '130px',
      alignment: 'left',
      allowEditing: false,
    },
    {
      label: 'Transport #',
      name: 'ActiveTransportId',
      allowEditing: false,
      alignment: 'left',
    },
    {
      label: 'Transportation Type',
      name: 'ActiveTransportDescription',
      allowEditing: false,
    },
    {
      label: 'Transport Code',
      name: 'ActiveTransportCode',
      allowEditing: false,
    },
    {
      label: '',
      name: 'action',
      width: 80,
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleDeleteBtn(e.data)}>Delete</button>
      )
    },
  ]

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url:'tas/clusterdetail',
      data: {
        ...values,
        clusterId: clusterId
      }
    }).then((res) => {
      getData()
      getTransportType()
      setShowModal(false)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSearch = (values) => {
    // tableSearch(values, data).then((res) => {
    //   setRenderData(res)
    // })
  }

  const fields = [
    {
      label: 'Code',
      name: 'Code',
      className: 'col-span-6 mb-2'
    },
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-6 mb-2'
    },
    {
      label: 'Direction',
      name: 'Direction',
      className: 'col-span-6 mb-2'
    },
    {
      label: 'Day',
      name: 'DayNum',
      className: 'col-span-6 mb-2'
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 mb-2',
      type: 'check',
      hide: true,
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]
  
  const addRecordFields = [
    {
      label: 'Transportation Type',
      name: 'activeTransportId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: transportType
      }
    },
    {
      label: 'Sequence Number',
      name: 'seqNumber',
      className: 'col-span-12 mb-2',
      type: 'number',
      inputprops: {
        min: 0,
      }
    },
  ]

  const handleCloseModal = () => {
    form.resetFields()
    setShowModal(false)
  }

  const onReorder = (e) => {
    const visibleRows = e.component.getVisibleRows();
    const newTasks = [...recorder];

    const toIndex = newTasks?.findIndex((item) => item.Id === visibleRows[e.toIndex].data.Id);
    const fromIndex = newTasks?.findIndex((item) => item.Id === e.itemData.Id);

    newTasks.splice(fromIndex, 1);
    newTasks.splice(toIndex, 0, e.itemData);

    setRecorder(newTasks)
  }

  const handleCancelReorder = () => {
    setRecorder(data.data)
    setIsReorder(false)
  }

  const handleSaveReorder = () => {
    let clusterDetailIds = []
    recorder.map((item) => {
      clusterDetailIds.push(item.Id)
    })
    axios({
      method: 'post',
      url: 'tas/clusterdetail/reorder',
      data: {
        clusterDetailIds: clusterDetailIds,
        clusterId: clusterId,
      }
    }).then((res) => {
      getData()
      setIsReorder(false)
    }).catch((err) => {

    })
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/clusterdetail/${editData?.Id}`,
    }).then(() => {
      getData()
      getTransportType()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <div className='max-w-[800px]'>
          <Form 
            form={searchForm} 
            fields={fields} 
            editData={data}
            className='grid grid-cols-12 gap-x-8' 
            size='small'
            onFinish={handleSearch}
            disabled={true}
          />
          <div className='flex gap-4 justify-end'>
            <Button onClick={() => navigate(-1)} icon={<LeftOutlined/>}>Back</Button>
          </div>
        </div>
      </div>
      <Table
        data={recorder}
        columns={columns}
        isGrouping={false}
        allowColumnReordering={false}
        loading={loading}
        containerClass={ !state.userInfo?.ReadonlyAccess && 'pt-2'}
        tableClass={ !state.userInfo?.ReadonlyAccess && 'border-t'}
        keyExpr='Id'
        title={ !state.userInfo?.ReadonlyAccess ? <div className='flex justify-between items-center'>
            <Button className='mb-2' icon={<PlusOutlined/>} onClick={handleClickAdd}>Add Record</Button>
            {
              isReorder ? 
              <div className='flex items-center gap-4'>
                <Button className='mb-2' type='primary' icon={<SaveOutlined />} onClick={handleSaveReorder}>Save Reorder</Button>
                <Button className='mb-2' onClick={handleCancelReorder}>Cancel</Button>
              </div>
              :
              <Button className='mb-2' icon={<OrderedListOutlined />} onClick={() => setIsReorder(true)}>Change Seq</Button>
            }
          </div>
          : null
        }
        rowDragging={isReorder ? {
          onReorder:(e) => onReorder(e),
        } : false}
      />
      <Modal
        open={showModal}
        title='Add Record'
        closeIcon={<div className='flex items-center justify-center'><CloseOutlined/></div>}
        onCancel={handleCloseModal}
        width={600}
        destroyOnClose
      >
        <Form 
          form={form}
          fields={addRecordFields} 
          editData={{seqNumber: recorder.length+1}}
          className='grid grid-cols-12 gap-x-8 mt-5' 
          onFinish={handleSubmit}
        />
        <div className='flex gap-4 justify-end'>
          <Button onClick={handleCloseModal} disablde={loading}>Cancel</Button>
          <Button onClick={() => form.submit()} type={'primary'} loading={loading} icon={<SaveOutlined/>}>Save</Button>
        </div>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to `delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>
            No
          </Button>
        </div>
      </Popup>
    </div>
  )
}

export default ClusterDetail