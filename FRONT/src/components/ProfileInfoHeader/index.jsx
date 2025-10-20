import React, { useContext, useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { 
  FaRegBuilding,
  FaUserAlt,
  FaBriefcase,
  FaUsers,
  FaLocationArrow,
  FaHouseUser,
  FaPhone,
  FaPassport
} from 'react-icons/fa'
import { MdPayment, MdPhone } from 'react-icons/md'
import { ImProfile } from 'react-icons/im'
import { BsFillHousesFill } from "react-icons/bs";
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'
import { DepartmentTooltip, Tooltip } from 'components'
import { Badge } from 'antd'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'

function ProfileInfoHeader({data}) {
  const { state } = useContext(AuthContext)
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
    <div className='grid grid-cols-12 gap-5 relative flex-1'>
      {data ? 
      <div className={`rounded-ot border p-4 flex justify-between col-span-12 ${data?.Active === 1 ? 'bg-white' : 'bg-[#d9d9d9]'}`}>
        <div className='flex gap-5'>
          <Link className={`${data?.Active === 1 ? '' : 'bg-gray-100'} rounded-ot overflow-hidden h-[60px] w-[60px] flex items-center justify-center`} to={`/tas/people/search/${data?.Id}`}>
            <img src={profileImage}/>
          </Link>
          <div className='flex-1'>
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
            <div className='flex mt-2'>
              <div className='text-[#718096] text-xs flex flex-col gap-1 border-r pr-4'>
                <Tooltip Tooltip title='Nationality' className='flex gap-3 items-center cursor-default'>
                  <div><FaPassport size={15}/></div>
                  <div>{data?.NationalityName ? data?.NationalityName : <span className='text-gray-400'>Not registered</span>}</div>
                </Tooltip>
                <Tooltip Tooltip title={`${data.EmployerCurrentStatus ? 'Temporary' : 'Permanent'} Employer name`}>
                  <div className='flex gap-3 items-start cursor-default'>
                    <div><FaRegBuilding size={15}/></div>
                    <div>
                      <Badge className='mr-1' status={data.EmployerCurrentStatus ? 'warning' : 'success'}></Badge>
                      {data?.EmployerName ? data?.EmployerName : <span className='text-gray-400'>Not registered</span>}
                    </div>
                  </div>
                </Tooltip>
                <DepartmentTooltip data={data} id={data?.DepartmentId}>
                  <div className='flex items-start gap-3 cursor-default'>
                    <div><FaUsers size={15}/></div>
                    <div>
                      <Badge className='mr-1' status={data.DepartmentCurrentStatus ? 'warning' : 'success'}></Badge>
                      {data?.DepartmentName ? data?.DepartmentName : <span className='text-gray-400'>Not registered</span>}
                    </div>
                  </div>
                </DepartmentTooltip>
                <Tooltip title={<div><span className='text-secondary2'>Cost Code: </span>{data.CostCodeDescription ? data.CostCodeDescription : null}</div>}>
                  <div className='flex items-start gap-3 cursor-default'>
                    <div><MdPayment size={15}/></div>
                    <div>
                      <Badge className='mr-1' status={data.CostCodeCurrentStatus ? 'warning' : 'success'}></Badge>
                      {data?.CostCodeNumber ? data?.CostCodeNumber : <span className='text-gray-400'>Not registered</span>}
                    </div>
                  </div>
                </Tooltip>
              </div>
              <div className='text-[#718096] text-xs flex flex-col gap-1 px-4 border-r'>
                <Tooltip title={`${data.PositionCurrentStatus ? 'Temporary' : 'Permanent'} Position name`} className='flex items-start gap-3 cursor-default'>
                    <div><FaBriefcase size={15}/></div>
                    <div>
                      <Badge className='mr-1' status={data.PositionCurrentStatus ? 'warning' : 'success'}></Badge>
                      {data?.PositionName ? data?.PositionName : <span className='text-gray-400'>Not registered</span>}
                    </div>
                </Tooltip>
                <Tooltip title='SAP ID #' className='flex items-center gap-3 cursor-default'>
                  <div><ImProfile size={15}/></div>
                  {data?.SAPID ? data?.SAPID : <span className='text-gray-400'>Not registered</span>}
                </Tooltip>
                <Tooltip title='Resource Type' className='flex items-center gap-3 cursor-default'>
                  <div><FaUserAlt size={14}/></div>
                  <div>{data?.PeopleTypeName ? data?.PeopleTypeName : <span className='text-gray-400'>Not registered</span>}</div>
                </Tooltip>
                <Tooltip title='Mobile' className='flex items-center gap-3 cursor-default'>
                    <div><MdPhone size={15}/></div>
                    <div>{data?.PersonalMobile ? data?.PersonalMobile : <span className='text-gray-400'>Not registered</span>}</div>
                </Tooltip>
              </div>
              <div className='text-[#718096] text-xs flex flex-col gap-1 pl-4'>
                <Tooltip title='Location' className='flex items-start gap-3 cursor-default'>
                  <div><FaLocationArrow size={15}/></div>
                  <div>{data?.LocationName ? data?.LocationName : <span className='text-gray-400'>Not registered</span>}</div>
                </Tooltip>
                <Tooltip title='Camp & Room Type' className='flex items-start gap-3 cursor-default'>
                  <div><BsFillHousesFill size={15}/></div>
                  <div>{data?.CampRoomType}</div>
                </Tooltip>
                <Tooltip title='Own Room' className='flex items-start gap-3 cursor-default'>
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
            </div>
          </div>
        </div>
      </div>
      : 
      <div></div>
      }
    </div>
  )
}

export default ProfileInfoHeader