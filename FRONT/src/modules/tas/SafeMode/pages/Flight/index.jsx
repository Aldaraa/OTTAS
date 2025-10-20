import axios from 'axios'
import { Button, ExternalTransport, Modal, Table } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm, DatePicker, Tag } from 'antd'
import { PlusOutlined, SearchOutlined } from '@ant-design/icons'
import { useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import RescheduleForm from './RescheduleForm'
import COLORS from 'constants/colors'
import { Popup } from 'devextreme-react'
import AddTransport from './AddTransport'

function Flight() {
  const [ data, setData ] = useState([])
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ selectedDate, setSelectedDate ] = useState(dayjs().startOf('month'))
  const [ loading, setLoading ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ rescheduleData, setRescheduleData ] = useState(null)
  const [ showModal, setShowModal ] = useState(false)
  const [ showCRUDModal, setShowCRUDModal ] = useState(false)
  const [ showExternalModal, setShowExternalModal ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)

  const { state, action } = useContext(AuthContext)
  const [ reScheduleForm ] = AntForm.useForm()
  const { employeeId } = useParams()

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/transport/all/${employeeId}/${dayjs(selectedDate).format('YYYY-MM-DD')}`,
    }).then((res) => {
      setData(res.data.map((item) => ({...item, label: dayjs(item.EventDate).format('YYYY-MM-DD')})))
    }).catch((err) => {

    }).then(() => setSearchLoading(false))
  }

  const getProfileData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
    }).catch((err) => {

    })
  }

  const handleSearch = () => {
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/transport/all/${employeeId}/${dayjs(selectedDate).format('YYYY-MM-DD')}`,
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setSearchLoading(false))
  }

  const handleRemoveButton = (dataItem, e) => {
    setEditData({...dataItem, rowIndex: e.rowIndex})
    setShowPopup(true)
  }

  const handleClickReschedule = (rowData) => {
    action.setFlightCalendarDate(dayjs(rowData.EventDate).format('YYYY-MM-DD'))
    setRescheduleData(rowData)
    setShowModal(true)
  }

  const column = [
    {
      label: 'Date',
      name: 'EventDate',
      cellRender: (e) => (
        <div>{dayjs(e.data.EventDate).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Transport Mode',
      name: 'TransportMode',
    },
    {
      label: 'Code',
      name: 'Code',
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Direction',
      name: 'Direction',
      cellRender: ({value}) => (
        <Tag className='text-xs' color={COLORS.Directions[value]?.tagColor}>{value}</Tag>
      )
    },
    {
      label: 'Status',
      name: 'Status',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'success' : 'orange'} className='text-xs'>{e.value}</Tag>
      )
    },
    {
      label: '',
      name: 'action',
      width: '150px',
      alignment: 'left',
      cellRender: (e) => (
        // dayjs(e.data.EventDate).startOf('D').diff(dayjs().startOf('D')) >= 0 ?
        <div className='flex items-center gap-4'>
          <button type='button' className='edit-button' onClick={() => handleClickReschedule(e.data)}>Edit</button>
          <button type='button' className='dlt-button' onClick={() => handleRemoveButton(e.data, e)}>Delete</button>
        </div>
        // : null
      )
    },
  ]

  const refreshData = () => {
    getData()
    getProfileData()
  }

  const cancelCRUD = () => {
    setShowCRUDModal(false)
  }

  const handleAddTravelButton = () => {
    setShowCRUDModal(true)
  }

  const handleRemove = () => {
    setLoading(true)
    axios({
      method: 'delete',
      url: `tas/safemode/transport/${editData.Id}`,
    }).then((res) => {
      getData()
      getProfileData()
      setShowPopup(false)
    }).catch((err) => {

    }).finally(() => {
      setLoading(false)
    })
  }

  return (
    <div className='bg-white rounded-ot px-3 pb-0 col-span-12 shadow-md'>
      <div className='flex justify-between items-center py-2'>
        <div className='text-lg font-bold'>Existing Transports</div>
        <div className='flex gap-5'>
          <DatePicker 
            value={selectedDate}
            onChange={(e) => setSelectedDate(e)}
          />
          <Button 
            onClick={handleSearch} 
            icon={<SearchOutlined/>}
            disabled={!selectedDate}
          >
            Search
          </Button>
          {
            !state.userInfo?.ReadonlyAccess ?
            <>
              <Button 
                onClick={handleAddTravelButton}  
                icon={<PlusOutlined/>}
              >
                Add
              </Button>
            </>
            : null
          }
        </div>
      </div>
      <Table
        data={data}
        columns={column}
        allowColumnReordering={false}
        loading={searchLoading}
        containerClass='shadow-none border-t rounded-none'
        tableClass='max-h-[calc(100vh-630px)]'
        keyExpr='EventDate'
        pager={{showPageSizeSelector: data?.length > 100}}
      />
      <Modal
        title={'Add Booking'}
        open={showCRUDModal}
        onCancel={cancelCRUD}
        width={800}
        destroyOnClose
      >
        <AddTransport
          onCancel={cancelCRUD}
          refresh={refreshData}
          employeeId={employeeId}
        />
      </Modal>
      <Modal 
        title='Edit Transport'
        open={showModal}
        onCancel={() => {setShowModal(false); reScheduleForm.resetFields()}}
        width={800}
        destroyOnClose
        zIndex={1100}
      >
        <RescheduleForm
          data={rescheduleData} 
          handleShowModal={setShowModal}
          refreshData={refreshData}
          reScheduleForm={reScheduleForm}
        />
      </Modal>
      <Modal width={800} title='Add External Transport' open={showExternalModal} onCancel={() =>setShowExternalModal(false)}>
        <ExternalTransport
          employeeId={employeeId}
          refreshData={refreshData}
          onCancel={() => setShowExternalModal(false)}
        />
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={'auto'}
        width={350}
      >
        <div>
          <div>Are you sure you want to remove this transport?</div>
          <div className='flex gap-5 mt-3 justify-center'>
            <Button type={'danger'} onClick={handleRemove} loading={loading}>Yes</Button>
            <Button onClick={() => setShowPopup(false)} disabled={loading}>No</Button>
          </div>
        </div>
      </Popup>
    </div>
  )
}

export default Flight