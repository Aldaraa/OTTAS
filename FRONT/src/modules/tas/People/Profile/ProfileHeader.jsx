import { DepartmentList, Modal, Table, Tooltip } from 'components'
import React, { useContext, useState, useEffect, useMemo } from 'react'
import { Link, NavLink, useLocation, useParams } from 'react-router-dom'
import { 
  FaRegBuilding,
  FaBriefcase,
  FaUserAlt,
  FaHouseUser,
  FaLocationArrow,
  FaPassport
} from 'react-icons/fa'
import { MdPayment, MdPhone } from 'react-icons/md'
import { ImProfile } from 'react-icons/im'
import { ReactComponent as Vector} from 'assets/icons/airplane.svg'
import { ReactComponent as Icon1} from 'assets/icons/accommodation.svg'
import { Badge, Dropdown, Tag } from 'antd';
import { VscCalendar } from 'react-icons/vsc'
import { LoadingOutlined, MoreOutlined } from '@ant-design/icons'
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'
import ls from 'utils/ls'
import { twMerge } from 'tailwind-merge'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import axios from 'axios'

const moreMenus = [
  {
    path: 'shiftvisual',
    label: 'Shift Visual',
    hasPermission: true,
  },
  {
    path: 'noshow',
    label: 'No Show & Go Show',
    hasPermission: true,
  },
  {
    path: 'history',
    label: 'Account History',
    hasPermission: false,
  },
  {
    path: 'audit',
    label: 'Profile Audit',
    hasPermission: false,
  },
  {
    path: 'profilebydate',
    label: 'Profile By Date',
    hasPermission: true,
  },
  {
    path: 'rolecenter',
    label: 'Role Center',
    hasPermission: true,
  },
]

const getTagColor = (status) => {
  switch (status) {
    case 'Confirmed': return 'green'
    case 'Waiting': return 'green'
    case 'Submitted': return 'orange'
  }
}
const siteTravelCols = [
  {
    label: '',
    name: 'Direction',
    width: 60,
  },
  {
    label: 'Date',
    name: 'EventDate',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
    )
  },
  {
    label: 'Description',
    name: 'Description',
  },
  {
    label: 'Status',
    name: 'Status',
    width: 100,
    cellRender: (e) => (
      <Tag color={getTagColor(e.value)}>{e.value}</Tag>
    )
  },
]

function ProfileHeader({data}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ existingBookings, setExistingBookings ] = useState(null)
  const [ loading, setLoading ] = useState(false)

  const { state } = useContext(AuthContext)
  const { employeeId } =  useParams()
  const { pathname } = useLocation()

  useEffect(() => {
    getExistingBookings()
  },[employeeId])


  const getExistingBookings = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocument/existingbooking/${employeeId}`
    }).then((res) => {
      setExistingBookings(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const profileImage = useMemo(() => {
    if(data){
      if(data?.Gender && data.Active){
        return activeMale
      }
      if(data?.Gender && !data.Active){
        return male
      }
      if(!data?.Gender && data.Active){
        return activeFemale
      }
      if(!data?.Gender && !data.Active){
        return female
      }
    }
  },[data])

  const menuOptions = useMemo(() => {
    if(data?.Active === 1){
      let tmp = [
        ...moreMenus, 
        {
        path: `/request/existingbookings/${data.Id}`,
        label: 'Existing Booking',
        hasPermission: true,
      }]

      return tmp
    }else{
      return moreMenus.filter((item) => !item.hasPermission)
    }
  },[data])

  return (
    <div className={`rounded-ot p-4 flex justify-between items-center col-span-12 shadow-md sticky top-0 z-[100] ${data?.Active === 1 ? 'bg-white' : 'bg-[#d9d9d9]'}`}>
      <div className='flex gap-5'>
        <Link className={`${data?.Active === 1 ? '' : 'bg-gray-100'} h-[80px] w-[80px] flex items-center justify-center rounded-ot border overflow-hidden`} to={`/tas/people/search/${employeeId}`} onClick={() => ls.set('pp_rt', '')}>
          <img className='object-cover' src={profileImage}/>
        </Link>
        <div>
          <span className='text-start lg:text-lg font-bold flex gap-1 items-center'>
            {data?.Firstname} {data?.Lastname} ({data?.Id})
            {
              data.OnSiteStatus ? 
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
          <div className='flex justify-start gap-4 divide-x mt-2'>
            <div className='text-[#718096] text-xs flex flex-col gap-1'>
              <Tooltip Tooltip title='Nationality'>
                <div className='flex gap-3 items-center cursor-default'>
                  <div><FaPassport size={15}/></div>
                  <div>{data?.NationalityName ? data?.NationalityName : <span className='text-gray-400'>Not registered</span>}</div>
                </div>
              </Tooltip>
              <Tooltip title={`${data.EmployerCurrentStatus ? 'Temporary' : 'Permanent'} Employer name`}>
                <div className='flex gap-3 items-center cursor-default'>
                  <div><FaRegBuilding size={15}/></div>
                  <div>
                    <Badge className='mr-1' status={data.EmployerCurrentStatus ? 'warning' : 'success'}></Badge>
                    {data?.EmployerName ? data?.EmployerName : <span className='text-gray-400'>Not registered</span>}
                  </div>
                </div>
              </Tooltip>
              <Tooltip title={<div><span className='text-secondary2'>Cost Code: </span>{data.CostCodeDescription ? data.CostCodeDescription : null}</div>}>
                <div className='flex items-center gap-3 cursor-default'>
                  <div><MdPayment size={15}/></div>
                  <div>
                    <Badge className='mr-1' status={data.CostCodeCurrentStatus ? 'warning' : 'success'}></Badge>
                    {data?.CostCodeNumber ? data?.CostCodeNumber : <span className='text-gray-400'>Not registered</span>}
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
            <div className='pl-4 text-[#718096] text-xs flex flex-col gap-1'>
              <Tooltip title={`${data.PositionCurrentStatus ? 'Temporary' : 'Permanent'} Position name`}>
                <div className='flex items-center gap-3 cursor-default'>
                  <div><FaBriefcase size={15}/></div>
                  <div className=''>
                    <Badge className='mr-1' status={data.PositionCurrentStatus ? 'warning' : 'success'}></Badge>
                    {data?.PositionName ? data?.PositionName : <span className='text-gray-400'>Not registered</span>}
                  </div>
                </div>
              </Tooltip>
              <Tooltip title='SAP ID #'>
                <div className='flex items-center gap-3 cursor-default'>
                  <div><ImProfile size={15}/></div>
                  <div>{data?.SAPID ? data?.SAPID : <span className='text-gray-400'>Not registered</span>}</div>
                </div>
              </Tooltip>
              <Tooltip title='Own Room'>
                <div className='flex items-center gap-3 cursor-default'>
                  <div><FaHouseUser size={15}/></div>
                  {
                    data?.RoomNumber ? 
                    (state.userInfo?.Role === 'AccomAdmin' || state.userInfo?.Role === 'SystemAdmin') ?
                    <Link target='_blank' to={`/tas/roomcalendar/${data.RoomId}/${dayjs().format('YYYY-MM-DD')}`}>{data?.RoomNumber}</Link> 
                    :
                    <div>{data?.RoomNumber}</div>
                    : 
                    <div className='text-gray-400'>Not registered</div>
                  }
                </div>
              </Tooltip>
              {
                data?.CurrentRoomNumber ?
                <Tooltip title='Current Room' className='flex items-center gap-3 cursor-default'>
                  <div className='flex items-center gap-3 cursor-default text-success'>
                    <div><FaHouseUser size={15}/></div>
                    {
                      (state.userInfo?.Role === 'AccomAdmin' || state.userInfo?.Role === 'SystemAdmin') ?
                      <Link target='_blank' to={`/tas/roomcalendar/${data.CurrentRoomId}/${dayjs().format('YYYY-MM-DD')}`}>{data?.CurrentRoomNumber}</Link> 
                      :
                      <div>{data?.CurrentRoomNumber}</div>
                    }
                  </div>
                </Tooltip>
                : null
              }
            </div>
            <div className='pl-4 text-[#718096] text-xs flex flex-col gap-1'>
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
            </div>
            <div className='pl-4 text-[#718096] text-xs flex flex-col gap-1'>
              <DepartmentList toLink='/tas/people/search/' data={data} id={data?.DepartmentId}/>
            </div>
          </div>
        </div>
      </div>
      <div className='grid grid-cols-2 items-center gap-3 text-[10px] leading-none'>
        {
          data?.Active === 1 &&
          <>
          <NavLink
            to={`/tas/people/search/${employeeId}/flight`}
            state={data}
            className={({isActive}) => twMerge(`p-1 border rounded-[12px] hover:shadow-card transition-all`, `${isActive ? 'border-[#5939B2]' : ''}`)}
          >
            <div className='h-[50px] w-[50px] flex flex-col justify-center gap-1 items-center text-center text-[#374151]'>
              <Vector/>
              Flight
            </div>
          </NavLink>
          <NavLink 
            to={`/tas/people/search/${employeeId}/roombooking`} 
            className={({isActive}) => `p-1 border rounded-[12px] hover:shadow-card transition-all ${isActive ? 'border-[#00CBA6] shadow-card' : ''}`}
          >
            <div className='h-[50px] w-[50px] text-[#374151] flex flex-col justify-center items-center gap-1 text-center'>
              <Icon1/>
              Room Booking
            </div>
          </NavLink>
          <NavLink
            to={`/tas/people/search/${employeeId}/rosterexecute`} 
            state={data}
            className={({isActive}) => `p-1 border rounded-[12px] hover:shadow-card transition-all ${isActive ? 'border-[#C59900] shadow-card' : ''}`}
          >
            <div className='h-[50px] w-[50px] text-[#374151] rounded-[10px] flex flex-col justify-center items-center gap-1 text-center'>
              <VscCalendar size={19}/>
              Roster Execute
            </div>
          </NavLink>
          </>
        }
        <Dropdown 
          trigger={'hover'} 
          placement='bottomRight'
          dropdownRender={(menu) => (
            <div className='bg-white rounded-ot shadow-md p-1 mt-1'>
              {
                menuOptions.map((item, index) => (
                  item.label === 'Existing Booking' ?
                  <button className='py-1 px-3 hover:bg-black hover:bg-opacity-[0.04] rounded-ot' onClick={() => setShowModal(true)}>{item.label}</button>
                  :
                  <Link to={item.path} key={index}>
                    <div className='py-1 px-3 hover:bg-black hover:bg-opacity-[0.04] rounded-ot'>{item.label}</div>
                  </Link>
                ))
              }
            </div>
          )}
        >
          <button 
            type='button'
            className={`p-1 border rounded-[12px] hover:shadow-card transition-all ${[`/tas/people/search/${employeeId}/shiftvisual`].includes(pathname) ? 'border-[#1E9500] shadow-card' : ''}`}
          >
            <div className='h-[50px] w-[50px] text-[10px] text-[#374151] rounded-[10px] flex flex-col justify-center items-center gap-1'>
              <MoreOutlined style={{fontSize: 20}}/>
              Others
            </div>
          </button>
        </Dropdown>
      </div>
      <Modal
        open={showModal}
        title='Existing Bookings'
        width={800}
        onCancel={() => setShowModal(false)}
        
      >
        {
          loading ? 
          <div className='p-8 flex justify-center items-center'>
            <LoadingOutlined style={{fontSize: 24}}/>
          </div>
          :
          <>
            <div className='flex flex-col gap-5'>
              <div>
                <div><span className='text-gray-500 '>Name:</span> {existingBookings?.FullName} ({existingBookings?.EmployeeId})</div>
                <div><span className='text-gray-500'>Department:</span> {existingBookings?.Department}</div>
                <div><span className='text-gray-500'>Employer:</span> {existingBookings?.Employer}</div>
              </div>
              <Table
                data={existingBookings?.SiteTravel}
                columns={siteTravelCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                title={<div className='border-b py-1 font-bold'>Site Travel</div>}
              />
            </div>
          </>
        }
      </Modal>
    </div>
  )
}

export default ProfileHeader