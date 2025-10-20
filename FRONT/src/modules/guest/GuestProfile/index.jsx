import axios from 'axios'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { Link, Outlet, useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts';
import { Drawer, Menu, Tabs } from 'antd';
import { PeopleSearch, Skeleton } from 'components';
import HeaderGuest from './HeaderGuest';
import './index.css'
import activeMale from 'assets/icons/male-active.png'
import male from 'assets/icons/male.png'
import activeFemale from 'assets/icons/female-active.png'
import female from 'assets/icons/female.png'
import { FileAddOutlined } from '@ant-design/icons';

function getItem({label, key, icon, children, type, disabled, name, routeName}) {
  return { key, icon, children, label, type, disabled };
}

function GuestProfile() {
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

  const menu = [
    getItem({
      name: 'My Request', 
      label: <Link to={`/guest/request`}><span className='submenu-title'>My Request</span></Link>,
      key: '/guest/request'
    }),
    getItem({
      name: 'Documents',
      routeName: 'Documents',
      label:<span className='font-medium'>Create Request</span>, 
      key: 'documents', 
      icon: <FileAddOutlined style={{fontSize: '16px'}} />, 
      type: 'group',
      children: [
        getItem({
          name: 'Non Site Travel', 
          routeName: 'NonSiteTravel', 
          label: <Link to={`/guest/nonsitetravel/${state.userInfo.Id}`}><span className='submenu-title'>Non Site Travel</span></Link>,
          key: '/guest/nonsitetravel'
        }),
        getItem({
          name: 'TAS Profile Changes', 
          routeName: 'TASProfileChanges', 
          label: <Link to={`/guest/samprofilechanges/${state.userInfo.Id}`}><span className='submenu-title'>TAS Profile Changes</span></Link>,
          key: '/guest/samprofilechanges'
        }),
        getItem({
          name: 'Site Travel',
          routeName: 'SiteTravel',
          label: <Link to={`/guest/sitetravel/${state.userInfo.Id}`}><span className='submenu-title'>Site Travel</span></Link>,
          key: '/guest/sitetravel'
        }),
        getItem({
          name: 'De-Mobilisation',
          routeName: 'De-Mobilisation',
          label: <Link to={`/guest/de-mobilisation/${state.userInfo.Id}`}><span className='submenu-title'>De-Mobilisation</span></Link>,
          key: '/guest/de-mobilisation'
        }),
      ]
    }),
  ]

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
    <div className='relative flex flex-col w-full z-50'>
      <HeaderGuest/>
      <div className='w-full flex items-start gap-3 p-3'>
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
            <div className=''>
              <div className='relative flex flex-col gap-4 w-[300px]'>
                <div className='w-full bg-white rounded-ot py-3 px-3 shadow-card self-start'>
                  <div className='flex flex-col items-center'>
                    <img src={profileImage} className='w-[70px] rounded-ot'/>
                    <div className='font-bold mt-2 text-primary'>{data?.Firstname} {data?.Lastname} ({data?.Id})</div>
                    <div className='text-gray-500 text-center text-xs'>{data?.EmployerName} &#x2014; {data?.PositionName}</div>
                  </div>
                  <div className='mt-4'>
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
                  {
                    state.userInfo.CreateRequest === 1 ?
                    <div className='w-full border-t mt-3'>
                      <Menu
                        id='tas'
                        mode="inline"
                        defaultOpenKeys={['people', 'documents']}
                        items={menu}
                        className={`transition-all duration-0`}
                      />
                    </div>
                    : null
                  }
                </div>
              </div>
            </div>
            <div className='flex-1 bg-white rounded-ot shadow-card px-4 py-3'>
              <Outlet/>
            </div>
          </>
        }
      </div>
      <Drawer
        title='Select employee'
        open={openDrawer} 
        width={1000}
        onClose={() => setOpenDrawer(false)}
      >
        <PeopleSearch
          onSelect={handleSelectEmployee}
        />
      </Drawer>
    </div>
  )
}

export default GuestProfile