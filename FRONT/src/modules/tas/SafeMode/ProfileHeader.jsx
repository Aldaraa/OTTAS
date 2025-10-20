import { DepartmentTooltip, Tooltip } from 'components'
import React, { useMemo } from 'react'
import { Link, NavLink, useParams } from 'react-router-dom'
import { 
  FaRegBuilding,
  FaBriefcase,
  FaUsers,
  FaUserAlt,
  FaHouseUser,
  FaLocationArrow,
  FaPassport
} from 'react-icons/fa'
import { MdPayment, MdPhone } from 'react-icons/md'
import { ImProfile } from 'react-icons/im'
import { ReactComponent as Vector} from 'assets/icons/airplane.svg'
import { ReactComponent as Icon1} from 'assets/icons/accommodation.svg'
import { Badge } from 'antd';
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'
import { BsFillHousesFill } from 'react-icons/bs'
import style from './style.module.css'
import { SlLogout } from 'react-icons/sl'

function ProfileHeader({data}) {
  const { employeeId } =  useParams()

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
    <div className='col-span-12'>
      <Badge.Ribbon text='Safe mode' color='red' className='z-[200]'>
        <div className={`rounded-ot p-4 flex justify-between shadow-md sticky top-0 z-[100] bg-red-100`}>
          <div className='absolute right-0 bottom-2 mr-4 text-xs'>
            <div className={style.card}>
              You are in Safe Mode. Some features may be restricted. Please proceed with caution.
            </div>
          </div>
          <div className='flex gap-5'>
            <Link className={`${data?.Active === 1 ? '' : 'bg-gray-100'} h-[80px] w-[80px] flex items-center justify-center rounded-ot border overflow-hidden`} to={`/tas/people/search/${employeeId}`}>
              <img src={profileImage}/>
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
              <div className='flex gap-5 divide-x mt-2 text-gray-600'>
                {/* <div className='text-[#718096] text-xs flex flex-col gap-1'> */}
                <div className='text-xs flex flex-col gap-1'>
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
                <div className='pl-5 text-xs flex flex-col gap-1'>
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
                  <Tooltip title='Own Room' className='flex items-center gap-3 cursor-default'>
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
                <div className='pl-5 text-xs flex flex-col gap-1'>
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
          <div className='flex items-center gap-5 text-black text-[10px] leading-none'>
            <NavLink 
              to={`/tas/people/search/sm/${employeeId}`}
              replace={true}
              state={data}
              className={({isActive}) => `p-1 border rounded-[12px] bg-white hover:shadow-card transition-all ${isActive ? 'border-[#5939B2] shadow-card' : 'border-gray-400'}`}
              end
            >
              <div className='h-[60px] w-[60px] text-[#374151] rounded-[10px] flex flex-col justify-center items-center gap-1 text-center'>
                <Vector/>
                Flight
              </div>
            </NavLink>
            <NavLink 
              to={`/tas/people/search/sm/${employeeId}/roombooking`}
              replace={true}
              className={({isActive}) => `p-1 border rounded-[12px] bg-white hover:shadow-card transition-all ${isActive ? 'border-[#00CBA6] shadow-card' : 'border-gray-400'}`}
              end
            >
              <div className='h-[60px] w-[60px] text-[#374151] rounded-[10px] flex flex-col justify-center items-center gap-1 text-center'>
                <Icon1/>
                Room Booking
              </div>
            </NavLink>
            <NavLink 
              to={`/tas/people/search/${employeeId}/roombooking`}
              replace={true}
              className={({isActive}) => `p-1 border border-gray-400 bg-white rounded-[12px] hover:shadow-card transition-all`}
              end
            >
              <div className='h-[60px] w-[60px] text-red-500 rounded-[10px] flex flex-col justify-center items-center gap-1 text-center'>
                <SlLogout size={18} className='mr-1'/>
                Exit
              </div>
            </NavLink>
          </div>
        </div>
      </Badge.Ribbon>
    </div>
  )
}

export default ProfileHeader