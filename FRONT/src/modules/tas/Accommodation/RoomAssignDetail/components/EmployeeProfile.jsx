import axios from 'axios'
import React, { useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { Link, NavLink, useParams } from 'react-router-dom'
import { 
  FaRegBuilding,
  FaUserAlt,
  FaBriefcase,
  FaLocationArrow,
  FaHouseUser,
  FaPassport
} from 'react-icons/fa'
import { MdPayment, MdPhone } from 'react-icons/md'
import { ImProfile } from 'react-icons/im'
import { AuthContext } from 'contexts';
import { DepartmentList, FlightList, Tooltip, VerticalCalendar } from 'components'
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'
import { Badge, Dropdown } from 'antd'
import { MoreOutlined } from '@ant-design/icons'
import { ReactComponent as Icon1} from 'assets/icons/accommodation.svg'
import { BsFillHousesFill } from 'react-icons/bs'
import { twMerge } from 'tailwind-merge'
import dayjs from 'dayjs'

function EmployeeProfile({profileData}) {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  const { state, action } =  useContext(AuthContext)
  const { employeeId } = useParams()

  useEffect(() => {
    if(profileData){
      action.saveUserProfileData(profileData)
      setData(profileData)
      setLoading(false)
    }
  },[profileData]) // eslint-disable-line
  

  useEffect(() => {
    if(state.ChangedFlight !== 0){
      getData()
    }
  },[state.ChangedFlight]) // eslint-disable-line

  const getData = useCallback(() => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  },[employeeId, action])

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


  return (
    <div className='mb-4'>
      {
        loading ? 
        <>
          <div className='animate-skeleton h-[130px] flex rounded-ot mb-5 border'>
          </div>
          <div className='animate-skeleton h-[300px] flex rounded-ot border'></div>
        </>
        :
        <div className='grid grid-cols-12 gap-5 relative'>
          <div className={twMerge(`rounded-ot p-4 flex justify-between col-span-12 border border-gray-300 sticky top-0 z-[100] shadow-md`, data?.Active === 1 ? 'bg-white' : 'bg-[#d9d9d9]')}>
            <div className='flex gap-5'>
              <div className='bg-gray-100 h-[80px] w-[80px] flex items-center justify-center rounded-ot border overflow-hidden'>
                <img alt='profile-img' src={profileImage}/>
              </div>
              <div>
                <span className='text-start lg:text-lg font-bold flex gap-1 items-center'>
                  {data?.Firstname} {data?.Lastname} ({data?.Id})
                  {
                    data?.OnSiteStatus === 1 ? 
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
                    <Tooltip title='Nationality'>
                      <div className='flex gap-3 items-center cursor-default'>
                        <div><FaPassport size={15}/></div>
                        <div>{data?.NationalityName ? data?.NationalityName : <span className='text-gray-400'>Not registered</span>}</div>
                      </div>
                    </Tooltip>
                    <Tooltip Tooltip title={`${data.EmployerCurrentStatus ? 'Temporary' : 'Permanent'} Employer name`}>
                      <div className='flex gap-3 items-center cursor-default'>
                        <div><FaRegBuilding size={15}/></div>
                        <div>
                          <Badge className='mr-1' status={data.EmployerCurrentStatus ? 'warning' : 'success'}></Badge>
                          {data?.EmployerName ? data?.EmployerName : <span className='text-gray-400'>Not registered</span>}
                        </div>
                      </div>
                    </Tooltip>
                    {/* <DepartmentTooltip data={data} id={data?.DepartmentId}>
                      <div className='flex items-center gap-3 cursor-default'>
                        <div><FaUsers size={15}/></div>
                        <div>
                          <Badge className='mr-1' status={data.DepartmentCurrentStatus ? 'warning' : 'success'}></Badge>
                          {data?.DepartmentName ? data?.DepartmentName : <span className='text-gray-400'>Not registered</span>}
                        </div>
                      </div>
                    </DepartmentTooltip> */}
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
                  <div className='pl-5 text-[#718096] text-xs flex flex-col gap-1'>
                    <Tooltip title={`${data.PositionCurrentStatus ? 'Temporary' : 'Permanent'} Position name`}>
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
                    <Tooltip title='Camp & Room Type' className='flex items-start gap-3 cursor-default'>
                      <div><BsFillHousesFill size={15}/></div>
                      <div>{data?.CampRoomType}</div>
                    </Tooltip>
                    <Tooltip title='Room number' className='flex items-center gap-3 cursor-default'>
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
                  </div>
                  <div className='pl-4 text-[#718096] text-xs flex flex-col gap-1'>
                    <DepartmentList toLink='/tas/roomassign/' data={data} id={data?.DepartmentId}/>
                  </div>
                </div>
              </div>
            </div>
            
            <div className='grid grid-cols-2 items-center gap-3 text-[10px] leading-none'>
              <NavLink 
                to={`roombooking`} 
                className={({isActive}) => `p-1 border rounded-[12px] hover:shadow-card transition-all ${isActive ? 'border-[#00CBA6] shadow-card' : ''}`}
              >
                <div className='h-[50px] w-[50px] text-[#374151] rounded-[10px] flex flex-col justify-center items-center gap-1 text-center'>
                  <Icon1/>
                  Room Booking
                </div>
              </NavLink>
              <NavLink 
                to={`information`} 
                className={({isActive}) => `p-1 border rounded-[12px] hover:shadow-card transition-all ${isActive ? 'border-[#0058cb] shadow-card' : ''}`}
              >
                <div className='h-[50px] w-[50px] text-[#374151] rounded-[10px] flex flex-col justify-center items-center gap-1 text-center'>
                  <ImProfile size={18}/>
                  Profile Information
                </div>
              </NavLink>
              <Dropdown
                trigger={'hover'} 
                placement='bottomRight'
                dropdownRender={(menu) => (
                  <div className='bg-white rounded-ot shadow-md p-1 mt-1'>
                    <Link to={'audit'}>
                      <div className='py-1 px-3 hover:bg-black hover:bg-opacity-[0.04] rounded-ot'>
                        Profile Audit
                      </div>
                    </Link>
                    <Link to={'profilebydate'}>
                      <div className='py-1 px-3 hover:bg-black hover:bg-opacity-[0.04] rounded-ot'>
                        Profile by date
                      </div>
                    </Link>
                  </div>
                )}
              >
                <button 
                  type='button'
                  className={`p-1 border rounded-[12px] hover:shadow-card transition-all`}
                >
                  <div className='h-[50px] w-[50px] text-[10px] text-[#374151] rounded-[10px] flex flex-col justify-center items-center gap-1'>
                    <MoreOutlined style={{fontSize: 20}}/>
                    Others
                  </div>
                </button>
              </Dropdown>
            </div>
          </div>
          <div className='w-full col-span-12'>  
            <div className='flex items-start gap-5'>
              <VerticalCalendar employeeId={state.userProfileData.Id}/>
              <FlightList profileData={state.userProfileData} employeeId={state.userProfileData.Id}/>
            </div>
          </div>
        </div>
      }
    </div>
  )
}

export default EmployeeProfile