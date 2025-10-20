import axios from 'axios'
import React, { useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { 
  FaRegBuilding,
  FaUserAlt,
  FaBriefcase,
  FaUsers,
  FaLocationArrow,
  FaHouseUser,
  FaPassport
} from 'react-icons/fa'
import { MdPayment, MdPhone } from 'react-icons/md'
import { ImProfile } from 'react-icons/im'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts';
import { Button, DepartmentTooltip, FlightList, Modal, Table, Tooltip, VerticalCalendar } from 'components'
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'
import { Badge, Tag } from 'antd'
import { BsCalendarCheck, BsFillHousesFill } from 'react-icons/bs'
import { LoadingOutlined, PrinterFilled } from '@ant-design/icons'
import { CheckBox } from 'devextreme-react'
import ReactToPrint from 'react-to-print'

const toLink = (e) => {
  if(e.data.DocumentType === 'Non Site Travel'){
    return `/request/task/nonsitetravel/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'Profile Changes'){
    return `/request/task/profilechanges/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'De Mobilisation'){
    return `/request/task/de-mobilisation/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'Site Travel'){
    if(e.data.DocumentTag === 'ADD'){
      return `/request/task/sitetravel/addtravel/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "RESCHEDULE"){
      return `/request/task/sitetravel/reschedule/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "REMOVE"){
     return `/request/task/sitetravel/remove/${e.data.Id}`
    }
  }
}

const nonSiteTravelCols = [
  {
    label: 'Travel Date',
    name: 'TravelDate',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd ')}</div>
    )
  },
  {
    label: 'Favor Time',
    name: 'FavorTime',
  },
  {
    label: 'Depart Location',
    name: 'DepartLocationName',
    alignment: 'left',
  },
  {
    label: 'Arrive Location',
    name: 'ArriveLocationName',
    alignment: 'left',
  },
  {
    label: 'Comment',
    name: 'Comment',
  },
  {
    label: 'ETD',
    name: 'ETD',
    alignment: 'left',
    cellRender: (e) => (
      <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    )
  },
]

const siteTravelCols = [
  {
    label: '',
    name: 'Direction',
  },
  {
    label: 'Date',
    name: 'EventDate',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
    )
  },
  {
    label: 'Description',
    name: 'Description',
  },
  {
    label: 'Status',
    name: 'Status',
    cellRender: (e) => (
      <Tag color={getTagColor[e.value]}>{e.value}</Tag>
    )
  },
]

const pendingRequestCols = [
  {
    label: 'Type',
    name: 'DocumentType',
  },
  {
    label: 'Date',
    name: 'RequestedDate',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
    )
  },
  {
    label: 'Description',
    name: 'Description',
  },
  {
    label: '#',
    name: 'Id',
    alignment: 'left',
    cellRender: (e) => (
      <div className='flex items-center text-blue-500 hover:underline'>
        <Link to={toLink(e)} target='_blank'>
          {e.value}
        </Link>
      </div>
    )
  },
  {
    label: 'Status',
    name: 'CurrentStatus',
    cellRender: (e) => (
      <Tag color={getTagColor[e.value]}>{e.value}</Tag>
    )
  },
]

const getTagColor = {
  'Confirmed': 'green',
  'Waiting': 'green',
  'Submitted': 'orange',
}

function EmployeeHeader({employeeId, profileData, pendingInfo}) {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  const [ showModal, setShowModal ] = useState(false)
  const [ existingBookings, setExistingBookings ] = useState(null)
  const [ bookingLoading, setBookingLoading ] = useState(false)

  const { state, action } =  useContext(AuthContext)
  const printRef = useRef(null)

  useEffect(() => {
    if(employeeId){
      getData()
    }else if(profileData){
      action.saveUserProfileData(profileData)
      setData(profileData)
      // setEmployeeStatusDates({
      //   employeeStatusDates: profileData.employeeStatusDates,
      //   employeeTransports: profileData.employeeTransports
      // })
      setLoading(false)
    }
  },[employeeId, profileData, action]) // eslint-disable-line react-hooks/exhaustive-deps
  

  useEffect(() => {
    if(state.ChangedFlight !== 0){
      getData()
    }
  },[state.ChangedFlight]) // eslint-disable-line react-hooks/exhaustive-deps

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
      setData(res.data)
      // setEmployeeStatusDates({
      //   employeeStatusDates: res.data.employeeStatusDates,
      //   employeeTransports: res.data.employeeTransports
      // })
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getExistingBookings = (employeeId) => {
    setBookingLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocument/existingbooking/${employeeId}`
    }).then((res) => {
      setExistingBookings(res.data)
    }).catch((err) => {

    }).then(() => setBookingLoading(false))
  }

  const handleClickCalendar = () => {
    setShowModal(true)
    getExistingBookings(employeeId)
  }

  const profileImage = useMemo(() => {
    if(data){
      if(data.Gender && data.Active){
        return activeMale
      }
      if(data.Gender && !data.Active){
        return male
      }
      if(!data.Gender && data.Active){
        return activeFemale
      }
      if(!data.Gender && !data.Active){
        return female
      }
    }
  },[data])
 

  return (
    <div>
      {
        loading ? 
        <>
          <div className='animate-skeleton h-[130px] flex rounded-ot mb-5 border'>
          </div>
          <div className='animate-skeleton h-[300px] flex rounded-ot border'></div>
        </>
        :
        // <div className='grid grid-cols-12 gap-5 divide-y-2'>
        data ?
        <div className='grid grid-cols-12 gap-5 relative'>
          {/* <div className={`rounded-ot flex justify-between col-span-12 border-gray-300`}> */}
          <div className={`rounded-ot p-4 flex justify-between col-span-12 border border-gray-300 sticky top-0 z-10 shadow-md bg-white`}>
            <div className='flex gap-5'>
              <Link 
                className='bg-gray-100 h-[80px] w-[80px] flex items-center justify-center rounded-ot border overflow-hidden cursor-pointer' 
                to={`/tas/people/search/${data?.Id}`}
              >
                <img alt='profile-img' src={profileImage}/>
              </Link>
              <div>
                <span className='text-start lg:text-lg font-bold flex gap-1 items-center'>
                  {data?.Firstname} {data?.Lastname} ({data?.Id})
                  {
                    data?.OnSiteStatus ? 
                    <Tooltip title='Today on Site'>
                      <span>
                        <div className='ml-2 h-3 w-3 bg-green-400 rounded-full relative z-0'>
                          <div className='absolute animate-ping inset-0 bg-green-400 rounded-full z-10'></div>
                        </div>
                      </span> 
                    </Tooltip>
                    : null
                  }
                </span>
                <div className='flex gap-5 divide-x mt-2'>
                  <div className='text-[#718096] text-xs flex flex-col gap-1'>
                    <Tooltip Tooltip title='Nationality'>
                      <div className='flex gap-3 items-center cursor-default'>
                        <div><FaPassport size={15}/></div>
                        <div>{data?.NationalityName ? data?.NationalityName : <span className='text-gray-400'>Not registered</span>}</div>
                      </div>
                    </Tooltip>
                    <Tooltip Tooltip title={`${data?.EmployerCurrentStatus ? 'Temporary' : 'Permanent'} Employer name`}>
                      <div className='flex gap-3 items-center cursor-default'>
                        <div><FaRegBuilding size={15}/></div>
                        <div>
                          <Badge className='mr-1' status={data.EmployerCurrentStatus ? 'warning' : 'success'}></Badge>
                          {data?.EmployerName ? data?.EmployerName : <span className='text-gray-400'>Not registered</span>}
                        </div>
                      </div>
                    </Tooltip>
                    <DepartmentTooltip data={data} id={data?.DepartmentId}>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><FaUsers size={15}/></div>
                        <div>
                          <Badge className='mr-1' status={data.DepartmentCurrentStatus ? 'warning' : 'success'}></Badge>
                          {data?.DepartmentName ? data?.DepartmentName : <span className='text-gray-400'>Not registered</span>}
                        </div>
                      </div>
                    </DepartmentTooltip>
                    <Tooltip title={`${data.CostCodeCurrentStatus ? 'Temporary' : 'Permanent'} Cost Code`} className='flex items-center gap-3 cursor-default'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><MdPayment size={15}/></div>
                        <div>
                          <Badge className='mr-1' status={data.CostCodeCurrentStatus ? 'warning' : 'success'}></Badge>
                          {data?.CostCodeName ? data?.CostCodeName : <span className='text-gray-400'>Not registered</span>}
                        </div>
                      </div>
                    </Tooltip>
                    <Tooltip title='Mobile' className='flex items-center gap-3 cursor-default'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><MdPhone size={15}/></div>
                        <div>
                          {data?.PersonalMobile ? data?.PersonalMobile : <span className='text-gray-400'>Not registered</span>}
                        </div>
                      </div>
                    </Tooltip>
                  </div>
                  <div className='pl-5 text-[#718096] text-xs flex flex-col gap-1'>
                    <Tooltip title={`${data.PositionCurrentStatus ? 'Temporary' : 'Permanent'} Position name`} className='flex items-center gap-3 cursor-default'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><FaBriefcase size={15}/></div>
                        <div>
                          <Badge className='mr-1' status={data.PositionCurrentStatus ? 'warning' : 'success'}></Badge>
                          {data?.PositionName ? data?.PositionName : <span className='text-gray-400'>Not registered</span>}
                        </div>
                      </div>
                    </Tooltip>
                    <Tooltip title='SAP ID #' className='flex items-center gap-3 cursor-default'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><ImProfile size={15}/></div>
                        <div>{data?.SAPID}</div>
                      </div>
                    </Tooltip>
                    <Tooltip title='Room number' className='flex items-center gap-3 cursor-default'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><FaHouseUser size={15}/></div>
                        <div>{data?.RoomNumber ? `${data?.RoomNumber}` : <span className='text-gray-400'>Not registered</span>}</div>
                      </div>
                    </Tooltip>
                    <Tooltip title='Camp & Room Type' className='flex items-start gap-3 cursor-default'>
                      <div><BsFillHousesFill size={15}/></div>
                      <div>{data?.CampRoomType}</div>
                    </Tooltip>
                    {
                      data?.CurrentRoomNumber ?
                      <Tooltip title='Current Room' className='flex items-center gap-3 cursor-default'>
                        <div className='flex items-center gap-3 cursor-default text-success'>
                          <div><FaHouseUser size={15}/></div>
                          <div>{data?.CurrentRoomNumber}</div>
                        </div>
                      </Tooltip>
                      : null
                    }
                  </div>
                  <div className='pl-5 text-[#718096] text-xs flex flex-col gap-1'>
                    <Tooltip title='Location' className='flex items-center gap-3 cursor-default'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><FaLocationArrow size={15}/></div>
                        <div>{data?.LocationName ? data?.LocationName : <span className='text-gray-400'>Not registered</span>}</div>
                      </div>
                    </Tooltip>
                    <Tooltip title='Resource Type' className='flex items-center gap-3 cursor-default'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><FaUserAlt size={14}/></div>
                        <div>{data?.PeopleTypeName ? data?.PeopleTypeName : <span className='text-gray-400'>Not registered</span>}</div>
                      </div>
                    </Tooltip>
                    <Tooltip title='Gender'>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div>{data?.Gender ? 'Male' : 'Female'}</div>
                      </div>
                    </Tooltip>
                  </div>
                </div>
              </div>
            </div>
            <div className='px-5 py-2'>
              <Badge count={pendingInfo?.PendingRequests}>
                <button type='button' onClick={handleClickCalendar}>
                  <BsCalendarCheck size={26}/>
                </button>
              </Badge>
            </div>
          </div>
          <div className='w-full col-span-12'>  
            <div className='flex items-start gap-5'>
              <VerticalCalendar employeeId={state.userInfo?.Id}/>
              <FlightList profileData={state.userProfileData} employeeId={state.userInfo?.Id}/>
            </div>
          </div>
        </div>
        : null
      }
      <Modal
        open={showModal}
        title='Existing Bookings'
        width={800}
        onCancel={() => setShowModal(false)}
        
      >
        {
          bookingLoading ? 
          <div className='p-8 flex justify-center items-center'>
            <LoadingOutlined style={{fontSize: 24}}/>
          </div>
          :
          <>
            <div ref={printRef} className='flex flex-col gap-5'>
              <div>
                <div><span className='text-gray-500 '>Name:</span> {existingBookings?.FullName} ({existingBookings?.SAPID})</div>
                <div><span className='text-gray-500'>Department:</span> {existingBookings?.Department}</div>
                <div><span className='text-gray-500'>Employer:</span> {existingBookings?.Employer}</div>
              </div>
              <Table
                data={existingBookings?.SiteTravel}
                columns={siteTravelCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                title={<div className='border-b py-1 font-bold'>Site Travel Requests</div>}
              />
              <Table
                data={existingBookings?.NonSiteTravel}
                columns={nonSiteTravelCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                title={<div className='border-b py-1 font-bold'>Non Site Travel Requests</div>}
              />
              <Table
                data={existingBookings?.PendingRequest}
                columns={pendingRequestCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                title={<div className='border-b py-1 font-bold'>Pending Requests</div>}
              />
              <Table
                data={existingBookings?.OtherRequest}
                columns={pendingRequestCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                tableClass={'max-h-full'}
                title={<div className='border-b py-1 font-bold'>Other Requests</div>}
              />
            </div>
            <div className='flex justify-end mt-5'>
              <ReactToPrint
                bodyClass='p-5'
                content={() => printRef.current}
                trigger={() => <Button type={'primary'} icon={<PrinterFilled/>}>Print</Button>}
              />
            </div>
          </>
        }
      </Modal>
    </div>
  )
}

export default EmployeeHeader