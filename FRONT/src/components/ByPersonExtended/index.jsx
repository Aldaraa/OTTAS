import { Table, Button, Modal, PeopleSearch, RoomFindAvailable, Form } from 'components'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { DatePicker } from 'antd'
import axios from 'axios'
import { ReloadOutlined, SearchOutlined, UserOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { Link, useLocation, useNavigate, useParams, useSearchParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'
import useQuery from 'utils/useQuery'

function ByPersonExtended({className='', empData, changeTab}) {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ fromDate, setFromDate ] = useState(dayjs().subtract(30,'days'))
  const [ toDate, setToDate ] = useState(dayjs().add(30,'days'))
  const [ selectedPerson, setSelectedPerson ] = useState(empData ? empData : null)
  const [ selectedRoom, setSelectedRoom ] = useState(null)
  const [ toChangeRoom, setToChangeRoom ] = useState(null)
  const [ questionModal, setQuestionModal ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)

  const { employeeId } = useParams()
  const { state } = useContext(AuthContext)
  let query = useQuery()
  const location = useLocation()
  const navigate = useNavigate()

  useEffect(() => {
    if(query.get('startDate')){
      setFromDate(dayjs(query.get('startDate')))
      setToDate(dayjs(query.get('endDate')))
      setLoading(true)
      setSearchLoading(true)
      axios({
        method: 'get',
        url: `tas/employeestatus/employeebooking?employeeId=${employeeId}&startDate=${query.get("startDate")}&endDate=${query.get("endDate")}`,
      }).then((res) => {
        setData(res.data)
      }).catch((err) => {
  
      }).then(() => {setLoading(false); setSearchLoading(false)})
    }
  },[query])

  useEffect(() => {
    handleSearch()
  },[])

  const handleSearch = () => {
    setLoading(true)
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/employeestatus/employeebooking?employeeId=${employeeId}&startDate=${dayjs(fromDate).format('YYYY-MM-DD')}&endDate=${dayjs(toDate).format('YYYY-MM-DD')}`,
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => {setLoading(false); setSearchLoading(false)})
  }

  const handleClickChange = (data) => {
    setSelectedRoom(data)
    setShowModal(true)
  }

  const columns = [
    {
      label: 'Owner', 
      name: 'RoomOwner', 
      alignment: 'center', 
      width: '55px',
      cellRender: (e) => (
        <span className='text-[12px] px-1'>
          {
            e.value ? 
            <i className="dx-icon-home text-green-500"></i> 
            :
            <i className="dx-icon-home text-gray-400 text-[14px]"></i>
          }
        </span>
      )
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
    },
    {
      label: 'Type',
      name: 'RoomType',
    },
    {
      label: 'Camp',
      name: 'Camp',
      // width: '80px',
      alignment: 'left'
    },
    {
      label: 'Date In',
      name: 'DateIn',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Last Night',
      name: 'LastNight',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Days',
      name: 'Days',
      alignment: 'left',
    },
    {
      label: '',
      name: 'action',
      width: '220px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <Link to={`/tas/roomcalendar/${e.data.RoomId}/${dayjs(e.data.DateIn).startOf('month').format('YYYY-MM-DD')}?employeeId=${employeeId}`}>
            <button type='button' className='edit-button'>Room Calendar</button>
          </Link>
        </div>
      )
    },
  ]

  const selectPerson = (data) => {
    setSelectedPerson(data)
    setShowPopup(false)
  }

  const selectRoom = (data) => {
    setToChangeRoom(data)
    setQuestionModal(true)
  }

  const handleSubmit = () => {
    setSubmitLoading(true)
    axios({
      method: 'post',
      url: 'tas/employeestatus/changeroombydate',
      data: {
        employeeId: selectedPerson?.Id,
        startDate: dayjs(selectedRoom?.DateIn).format('YYYY-MM-DD'),
        endDate: dayjs(selectedRoom?.LastNight).format('YYYY-MM-DD'),
        roomId: toChangeRoom?.RoomId 
      }
    }).then((res) => {
      setQuestionModal(false)
      setShowModal(false)
      handleSearch()
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
  }

  return (
    <div className={`rounded-ot bg-white py-3 px-3 mb-5 border mt-4 ${className}`}>
      <div className='text-lg font-bold mb-3'>View Room Booking by Person</div>
      <div className='flex gap-5 mb-5'>
        <div className='flex items-center'>
            <span className='mr-2'>From:</span>
            <DatePicker value={fromDate} onChange={(date) => setFromDate(date)}/>
        </div>
        <div className='flex items-center'>
            <span className='mr-2'>To:</span>
            <DatePicker value={toDate} onChange={(date) => setToDate(date)}/>
        </div>
        <Button 
          onClick={handleSearch} 
          loading={searchLoading} 
          icon={<SearchOutlined/>}
        >
          Search
        </Button>
      </div>
      <Table
        data={data}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        containerClass={`shadow-none pl-0 pr-0 border-t`}
        keyExpr='Id'
        pager={data.length > 20}
      />
      <Modal 
        width={1200} 
        open={showPopup} 
        onCancel={() => setShowPopup(false)} 
        title='People Search please select a person' 
        forceRender={true}
      >
        <PeopleSearch onSelect={selectPerson} onRowDblClick={(e) => navigate(`/tas/people/search/${e.data.Id}`)}/>
      </Modal>
      <Modal open={showModal} onCancel={() => setShowModal(false)} title='Change Room' width={900}>
        <RoomFindAvailable 
          onSelect={(e) => selectRoom(e)} 
          fromDate={dayjs(selectedRoom?.DateIn).format('YYYY-MM-DD')} 
          toDate={dayjs(selectedRoom?.LastNight).format('YYYY-MM-DD')}
        />
      </Modal>
      <Popup
        visible={questionModal}
        showTitle={false}
        height={'auto'}
        width={470}
      >
        <div>
          Are you sure to change the room assignment for <b>#{selectedPerson?.Id} {selectedPerson?.Firstname}</b> <br/> from Room <b>{selectedRoom?.RoomNumber}</b> to Room <span className='text-success font-bold'>{toChangeRoom?.roomNumber}</span>, <br/> starting from <b>{dayjs(selectedRoom?.DateIn).format('YYYY-MM-DD')}</b> and ending on <b>{dayjs(selectedRoom?.LastNight).format('YYYY-MM-DD')}</b> ?
        </div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'success'} onClick={handleSubmit} loading={submitLoading}>Yes</Button>
          <Button onClick={() => setQuestionModal(false)} disabled={submitLoading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default ByPersonExtended