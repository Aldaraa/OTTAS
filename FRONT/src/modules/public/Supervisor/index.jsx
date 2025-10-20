import axios from 'axios'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts';
import { Drawer, Tabs } from 'antd';
import { CustomMultiCalendar, FlightList, PeopleSearch, Skeleton, VerticalCalendar } from 'components';
import HeaderGuest from './HeaderGuest';
import './index.css'
import ActiveTransportSchedule from './ActiveTransport'
import TeamBookings from './TeamBooking'
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'

function Supervisor() {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  const [ currentDate, setCurrentDate ] = useState(dayjs())
  const [ employeeStatusDates, setEmployeeStatusDates ] = useState(null)
  const [ tabValue, setTabValue ] = useState('transportlist')
  const [ openDrawer, setOpenDrawer ] = useState(false)
  const [ selectedEmployee, setSelectedEmployee ] = useState(null)

  const navigate = useNavigate()
  const { employeeId } =  useParams()
  const { state, action } =  useContext(AuthContext)

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${state.userInfo.Id}`
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
        employeeId: employeeId,
        currentDate:  dayjs(date).startOf('month').format('YYYY-MM-DD'),
      }
    }).then((res) => {
      setEmployeeStatusDates(res.data)
    }).catch((err) => {
      
    })

  }

  const handleSelectEmployee = (e) => {
    setSelectedEmployee(e)
    setOpenDrawer(true)
  }

  const items = [
    // {
    //   label: 'Schedule',
    //   key: 1,
    //   children: <CustomMultiCalendar
    //     containerClass='shadow-none border border-gray-300 rounded-ot' 
    //     data={employeeStatusDates?.employeeStatusDates} 
    //     listData={employeeStatusDates?.employeeTransports}
    //     currentDate={currentDate}
    //     onChange={handleChangeDate}
    //   />
    // },
    {
      label: 'Schedule',
      key: 1,
      children: <div className='flex gap-5'>
          <VerticalCalendar containerClass='col-span-12 max-w-full shadow-none' employeeId={employeeId}/>
          {/* <FlightList profileData={state.userProfileData} employeeId={state.userInfo?.Id}/> */}
        </div>
    },
    {
      label: 'Existing Transport',
      key: 2,
      children: <ActiveTransportSchedule onChangeView={(e) => setTabValue(e)}/>
    },
    {
      label: 'Team members',
      key: 3,
      children: <div>
        <PeopleSearch
          onSelect={handleSelectEmployee}
          onRowDblClick={(e) => navigate(`/tas/people/search/${e.data.Id}`)}
          containerClass='mt-4'
          tableClass='max-h-[calc(100vh-350px)]'
          actionText='View'
        />
      </div>
    },

  ]

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
    <div className='relative flex flex-col w-full z-50'>
      <HeaderGuest/>
      <div className='w-full flex gap-3 p-3'>
        {
          loading ? 
          <>
            <Skeleton className='w-[300px] h-[300px]'>

            </Skeleton>
            <Skeleton className='flex-1 h-[300px]'>

            </Skeleton>
          </>
          :
          <>
            <div className='flex flex-col gap-4 w-[300px] bg-white rounded-ot py-3 px-3 shadow-card self-start'>
              <div className='flex flex-col items-center'>
                <img src={profileImage} className='w-[60px]'/>
                <div className='font-bold mt-2 text-primary'>{data?.Firstname} {data?.Lastname} ({data?.Id})</div>
                <div className='text-gray-500 text-center'>{data?.EmployerName} &#x2014; {data?.PositionName}</div>
              </div>
              <div>
                <div className='font-semibold mb-2'>Detail information</div>
                <table className='text-xs'>
                  <tbody>
                    <tr>
                      <td className='text-gray-400 w-[100px]'>SAPID:</td>
                      <td>{data?.SAPID}</td>
                    </tr>
                    <tr>
                      <td className='text-gray-400'>Position:</td>
                      <td>{data?.PositionName}</td>
                    </tr>
                    <tr>
                      <td className='text-gray-400'>Department:</td>
                      <td className='flex-1'>{data?.DepartmentName}</td>
                    </tr>
                    <tr>
                      <td className='text-gray-400'>AD Account:</td>
                      <td className='flex-1'>{data?.ADAccount}</td>
                    </tr>
                    <tr>
                      <td className='text-gray-400'>Employer:</td>
                      <td className='flex-1'>{data?.EmployerName}</td>
                    </tr>
                    <tr>
                      <td className='text-gray-400'>Phone Number:</td>
                      <td className='flex-1'>{data?.PersonalMobile}</td>
                    </tr>
                    <tr>
                      <td className='text-gray-400'>People Type:</td>
                      <td>{data?.PeopleTypeName}</td>
                    </tr>
                    <tr>
                      <td className='text-gray-400'>Room Number:</td>
                      <td>{state.userData?.RoomNumber ? state.userData?.RoomNumber : <span className='text-gray-400'>Not Registered</span>}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <div className='flex-1 bg-white rounded-ot shadow-card px-4 py-3'>
              <Tabs
                items={items}
                type='card'
              />
            </div>
          </>
        }
      </div>
      <Drawer
        title='Transport and Room booking'
        open={openDrawer} 
        width={1000}
        footer={false}
        onClose={() => setOpenDrawer(false)}
      >
        <TeamBookings empId={selectedEmployee?.Id}/>
      </Drawer>
    </div>
  )
}

export default Supervisor