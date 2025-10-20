import { Form, Table, Modal, Button, PeopleSearch } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm, Tag } from 'antd'
import axios from 'axios'
import { CloseOutlined, SaveOutlined, LeftOutlined } from '@ant-design/icons'
import { Link, useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import BulkReplace from './BulkReplace'
import { Popup } from 'devextreme-react'

function SeatBlockDetail() {
  const [ data, setData ] = useState([])
  const [ recorder, setRecorder ] = useState([])
  const [ transportType, setTransportType ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showReplaceModal, setShowReplaceModal ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ isEditing, setIsEditing  ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)

  const [ form ] = AntForm.useForm()
  const {eventId} = useParams()
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
      url: `tas/visitevent/${eventId}`
    }).then((res) => {
      setData(res.data)
      setRecorder(res.data.Employees)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getTransportType = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/cluster/activetransports/${eventId}`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.Id, label: `${item.Code} - ${item.Description}`, ...item})
      })
      setTransportType(tmp)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }


  const handleUndoReplacement = () => {
    axios({
      method: 'post',
      url: `tas/visitevent/replaceprofileundo`,
      data: {
        employeeId: editData.EmployeeId,
        eventId: eventId,
      }
    }).then((res) => {
      getData()
    }).catch((err) => {

    })
  }

  const handleUndoReplacementBtn = (row) => {
    setEditData(row)
    setShowPopup(true)
  }



  const columns = [
    {
      label: 'Employee',
      name: 'Firstname',
      alignment: 'left',
      cellRender: (e) => (
        e.data.Id ? 
        <Link to={`/tas/people/search/${e.data.EmployeeId}`}>
          <span className='text-blue-500'>{e.value} {e.data.Lastname}</span>
        </Link>
        :
        <span>{e.value} {e.data.Lastname}</span>
      )
    },
    {
      label: 'In Event Date',
      name: 'InEventDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'In',
      name: 'InDescr',
      alignment: 'left',
    },
    {
      label: 'In Status',
      name: 'InStatus',
      alignment: 'left',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'green' : 'orange'}>{e.value}</Tag>
      )
    },
    {
      label: 'Out Event Date',
      name: 'OutEventDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Out',
      name: 'OutDescr',
    },
    {
      label: 'Out Status',
      name: 'OutStatus',
      alignment: 'left',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'green' : 'orange'}>{e.value}</Tag>
      )
    },
    {
      label: '',
      name: 'action',
      cellRender: (e) => (
        <div>
          {
            e.data.Active === 2 ?
            <button className='edit-button' onClick={() => handleReplaceBtn(e.data)}>Replace Profile</button>
            :
            <button className='edit-button' onClick={() => handleUndoReplacementBtn(e.data)}>Undo Replacement</button>
          }
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url:'tas/clusterdetail',
      data: {
        ...values,
        eventId: eventId
      }
    }).then((res) => {
      getData()
      setShowModal(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }
  
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

  const handleReplaceBtn = (data) => {
    setEditData(data)
    setShowReplaceModal(true)
  }

  const handleReplace = (data) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: `tas/visitevent/replaceprofile`,
      data: {
        "oldEmployeeId": editData.EmployeeId,
        "newEmployeeId": data.Id,
        "eventId": eventId
      },
    }).then((res) => {
      // if(res.data && res.data?.length > 0){
      //   setReplaceResponse(res.data)
      //   setShowResponse(true)
      // }else{
      // }
      setShowReplaceModal(false)
      getData()
    }).finally(() => {
      setActionLoading(false)
    })
  }

  const handleSelect = (e) => {
    setSelectedRowKeys(e.selectedRowKeys)
    setCurrentSelectedData(e.selectedRowsData.map((item) => ({...item, Fullname: `${item.Firstname} ${item.Lastname}`})))
  }

  const handleAddSelection = () => {
    if(selectedData.length === 0){
      setSelectedData(currentSelectedData)
      form.setFieldValue('rosters', currentSelectedData)
      setCurrentSelectedData([])
    }
    else{
      let tmp = [...selectedData]
      currentSelectedData.map((item) => {
        if(!tmp?.find(el => el.Id === item.Id)){
          if(tmp.length < 50){
            tmp.push(item)
          }
        }
      })
      setSelectedData(tmp)
      setCurrentSelectedData([])
    }
  }

  const handleReturn = () => {
    if(selectedData.length !== 50){
      handleAddSelection()
    }
    setIsEditing(true)
  }

  const handleChangeData = (data) => {
    setSelectedRowKeys(data.map((item) => (item.Id)))
    setSelectedData(data)
  }

  const handleCanceResponse = () => {

  }

  return (
    <>
      {
        isEditing ?
        <BulkReplace 
          data={selectedData} 
          handleChangeData={handleChangeData}
          changeIsEditing={setIsEditing}
          className={`${isEditing ? 'block' : 'hidden' }`}
          refreshData={getData}
        />
        :
        <div>
          <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
            <div className='flex mb-4'>
              <Button onClick={() => navigate(-1)} icon={<LeftOutlined/>}>Back</Button>
            </div>
            <div className='text-lg font-bold mb-3'>{data.Name}</div>
            <div className='w-1/3'>
              <div className='flex'>
                <div className='w-[100px]'>Start Date:</div> <div>{dayjs(data.StartDate).format('YYYY-MM-DD')}</div>
              </div>
              <div className='flex'>
                <div className='w-[100px]'>End Date:</div> <div>{dayjs(data.EndDate).format('YYYY-MM-DD')}</div>
              </div>
            </div>
          </div>
          <Table
            data={recorder}
            columns={columns}
            isGrouping={false}
            allowColumnReordering={false}
            loading={loading}
            keyExpr='Id'
            pager={recorder >  20}
            selection={{mode: 'multiple', recursive: false, showCheckBoxesMode: 'always', selectAllMode: 'page'}}
            selectedRowKeys={selectedRowKeys}
            onSelectionChanged={handleSelect}
            title={!state.userInfo?.ReadonlyAccess ? <div className='flex justify-between items-center py-2 gap-3 border-b'>
            <div className='ml-2'><span className='font-bold'>{selectedData.length}</span> people selected {selectedData.length === 50 && <span className='text-red-400'>(full)</span>}</div>
            <div className='flex items-center gap-3'>
              <Button onClick={handleAddSelection} disabled={selectedData.length === 50 || selectedRowKeys.length === 0}>Add Selection</Button>
              <Button onClick={handleReturn} disabled={selectedData.length === 0 && selectedRowKeys.length === 0}>Add Selection & Replace</Button>
            </div>
          </div> : ''}
          //   title={<div className='flex justify-between py-2 px-1'>
          //     <span className=' text-gray-400'><span className='text-black font-bold'>{recorder.length}</span> employees</span>
          // </div>}
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
              className='grid grid-cols-12 gap-x-8 mt-5' 
              onFinish={handleSubmit}
            />
            <div className='flex gap-4 justify-end'>
              <Button onClick={() => form.submit()} type={'primary'} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
              <Button onClick={handleCloseModal} disablde={actionLoading}>Cancel</Button>
            </div>
          </Modal>
    
          <Modal
            open={showReplaceModal}
            title={`Replace Employee (${editData?.Firstname} ${editData?.Lastname})`}
            closeIcon={<div className='flex items-center justify-center'><CloseOutlined/></div>}
            onCancel={() => setShowReplaceModal(false)}
            width={1000}
            destroyOnClose
          >
            <PeopleSearch onSelect={(data) => handleReplace(data)} onRowDblClick={(e) => navigate(`/tas/people/search/${e.data.Id}`)}/>
          </Modal>
          <Popup
            visible={showPopup}
            showTitle={false}
            height={120}
            width={350}
          >
            <div>Are you sure you want to undo replacement on this record?</div>
            <div className='flex gap-5 mt-4 justify-center'>
              <Button type={editData?.Active === 1 ? 'primary' : 'success'} onClick={handleUndoReplacement} loading={actionLoading}>Yes</Button>
              <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
            </div>
          </Popup>
          {/* <Modal title='Skipped Employees' open={showResponse} onCancel={() => handleCanceResponse()}>
            <Table
              data={replaceResponse}
              columns={[
                {
                  label: 'Fullname',
                  name: 'Firstname',
                  alignment: 'left',
                  cellRender: (e) => (
                    <div>{e.value} {e.data.Lastname}</div>
                  )
                },
                {
                  label: '',
                  name: 'Reason',
                  alignment: 'left',
                  cellRender: (e) => (
                    <div className='text-red-400'>{e.value}</div>
                  )
                },
              ]}
            />
          </Modal> */}
        </div>
      }
    </>
  )
}

export default SeatBlockDetail