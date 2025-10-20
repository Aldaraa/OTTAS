import axios from 'axios'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
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
import dayjs from 'dayjs'
import { AuthContext } from 'contexts';
import { DepartmentList, Tooltip, VerticalCalendar } from 'components'
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'
import { Badge } from 'antd'


function PeopleProfile({employeeId, profileData}) {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  const [ currentDate, setCurrentDate ] = useState(dayjs())
  const [ employeeStatusDates, setEmployeeStatusDates ] = useState(null)
  
  const navigate = useNavigate()
  const { state, action } =  useContext(AuthContext)

  useEffect(() => {
    if(employeeId){
      getData()
    }else if(profileData){
      action.saveUserProfileData(profileData)
      setData(profileData)
      setEmployeeStatusDates({
        employeeStatusDates: profileData.employeeStatusDates,
        employeeTransports: profileData.employeeTransports
      })
      setLoading(false)
    }
  },[employeeId, profileData])
  

  useEffect(() => {
    if(state.ChangedFlight !== 0){
      getData()
    }
  },[state.ChangedFlight])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
      setData(res.data)
      setEmployeeStatusDates({
        employeeStatusDates: res.data.employeeStatusDates,
        employeeTransports: res.data.employeeTransports
      })
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleChangeDate = (date) => {
    setCurrentDate(dayjs(date).format('YYYY-MM-DD'))
    axios({
      method: 'post',
      url: 'tas/employee/statusdates',
      data: {
        employeeId: data.Id,
        currentDate:  dayjs(date).startOf('month').format('YYYY-MM-DD'),
      }
    }).then((res) => {
      setEmployeeStatusDates(res.data)
    }).catch((err) => {
      
    })
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
        <div className='grid grid-cols-12 gap-5 relative'>
          <div className={`rounded-ot p-4 flex justify-between items-center border col-span-12 ${data?.Active === 1 ? 'bg-white' : 'bg-[#d9d9d9]'}`}>
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
                        <div>{data?.RoomNumber ? `${data?.RoomNumber}` : <span className='text-gray-400'>Not registered</span>}</div>
                      </div>
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
                    <DepartmentList data={data} id={data?.DepartmentId}/>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <VerticalCalendar containerClass='col-span-12 max-w-full' employeeId={employeeId}/>
        </div>
      }
    </div>
  )
}

export default PeopleProfile