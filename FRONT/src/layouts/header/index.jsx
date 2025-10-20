import { BellFilled, LockOutlined, MailOutlined, PhoneOutlined, QuestionCircleFilled, ReloadOutlined, SettingOutlined, UnlockOutlined, UserOutlined } from '@ant-design/icons';
import { Badge, Dropdown, notification } from 'antd';
import { AuthContext } from 'contexts';
import React, { useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { Link, NavLink, useLocation, useNavigate, useParams } from 'react-router-dom';
import logo from 'assets/icons/OT_logo.png';
import axios from 'axios';
import { Button, Tooltip } from 'components';
import MultiViewers from './MultiViewers';
// import useQuery from 'utils/useQuery';
import { Popup } from 'devextreme-react';

const getActionColor = (action) => {
  switch (action) {
    case 'Submitted':
      return 'text-orange-500';
    case 'Decline':
      return 'text-red-500';
    case 'Cancelled':
      return 'text-red-500';
    case 'Completed':
      return 'text-green-500';
    case 'Pending':
      return 'text-blue-500';
  }
}

function Header() {

  const { action, state } = useContext(AuthContext);
  const [ notifications, setNotifications ] = useState([]);
  const [ isOpen, setIsOpen ] = useState(false);
  const [ showImpersoniteUser, setShowImpersoniteUser ] = useState(false);
  const [ impersoniteUserData, setImpersoniteUserData ] = useState(null);
  const [ notificationNumber, setNotificationNumber ] = useState(0);
  const [ showPopup, setShowPopup ] = useState(false);

  const navigate = useNavigate();
  const { impersoniteId } = useParams();
  const location = useLocation();
  const [api, contextHolder] = notification.useNotification();
  
  useEffect(() => {
    if(impersoniteId){
      getImperoniteUserData()
    }
  },[impersoniteId])

  useEffect(() => {
    if(impersoniteId || location.search.includes('?impersonateuser=')){
      setShowImpersoniteUser(true)
    }else{
      setShowImpersoniteUser(false)
    }
  },[location])

  const getImperoniteUserData = useCallback(() => {
    axios({
      method: 'get',
      url: `auth/auth/impersonite/${impersoniteId}`,
    }).then((res) => {
      setImpersoniteUserData(res.data)
    }).catch(() => {

    })
  },[impersoniteId])
  
  const modules = useMemo(() => {
    if(state.userInfo){
      const availableMenus = state.userInfo?.Menu.filter((item) => item.Code === 'TAS' || item.Code === 'REQUEST' || item.Code === 'REPORT')
      const tas = state.userInfo?.Menu.find((item) => item.Route && item.Route.includes('/tas'))
      const req = state.userInfo?.Menu.find((item) => item.Route && item.Route.includes('/request'))
      const rep = state.userInfo?.Menu.find((item) => item.Route && item.Route.includes('/report'))
      return [
        {path: tas?.Route, label: 'TAS'},
        {path: req?.Route, label: 'REQUEST'},
        {path: rep?.Route, label: 'REPORT'},
      ].filter((item) => availableMenus?.find((menu) => menu.Code === item.label))
    }
    return []
  },[state.userInfo])

  // const getNotifications = useCallback(() => {
  //   axios({
  //     method: 'post',
  //     url: `tas/notification/all`,
  //     data: {
  //       pageIndex: 0,
  //       pageSize: 20,
  //     }
  //   }).then((res) => {
  //     setNotifications(res.data.data)
  //   }).catch((err) => {

  //   })
  // },[])

  // const sendNotifIndex = useCallback((index) => {
  //   axios({
  //     method: 'get',
  //     url: `tas/notification/detail/${index}`,
  //   }).then((res) => {
  //     api.destroy();
  //     setIsOpen(false)
  //     navigate(res.data.link)
  //   }).catch((err) => {

  //   })
  // }, [navigate, api])

  const handleClickImpersonate = useCallback(() => {
    setShowPopup(true)
  },[])

  const handleImpersonateLogout = useCallback(() => {
    setShowPopup(false)
    navigate('/request/task')
  }, [navigate])

  return (
    <>
      <header id='header' className='text-white relative z-10 px-[25px] flex justify-between'>
        <div className='flex items-center gap-4' >
          <div className='mr-4'>
            <img src={logo} width={60} loading='lazy'/>
          </div>
          {
            modules.map((module) => (
              <NavLink 
                key={module.path}
                to={module.path} 
                className="px-4 text-center"
                style={({isActive}) => isActive ? {color: '#ff7f00'} : {color: ''}}
              >
                {module.label}
              </NavLink>
            ))
          }
        </div>
        <div className='flex items-center gap-4'>
          <Popup
            visible={showPopup}
            showTitle={false}
            height={120}
            width={350}
          >
            <div>
              <div>Are you sure you want to logout from impersonate user ?</div>
              <div className='flex gap-5 mt-4 justify-center'>
                <Button type='success'onClick={handleImpersonateLogout}>Yes</Button>
                <Button onClick={() => setShowPopup(false)}>No</Button>
              </div>
            </div>
          </Popup>

          {/* ////////////////////////////    Multi Viewers    ///////////////////////////////// */}
          <MultiViewers/>
          {/* ////////////////////////////    Impersonite User    ///////////////////////////////// */}
          {
            showImpersoniteUser && impersoniteUserData ?
            <Tooltip title='Impersonite User'>
              <div className='flex flex-col cursor-pointer pl-5 border-r border-gray-600 pr-4' onClick={handleClickImpersonate}>
                <div className='text-xs font-semibold text-green-500'>{impersoniteUserData.Firstname}.{impersoniteUserData.Lastname[0]}</div>
                <div className='text-xs text-gray-300'>{impersoniteUserData.Role}</div>
              </div>
            </Tooltip>
            : null
          }

          {/* ////////////////////////////    Hot Reload Button    ///////////////////////////////// */}
          
          <Tooltip title='Hot Reload'>
            <button type='button' className='flex items-center gap-2' onClick={() => action.setReferLoading(true)}>
              <ReloadOutlined style={{fontSize: '20px'}}/>
            </button>
          </Tooltip>

          {/* ////////////////////////////    Notification    ///////////////////////////////// */}

          {/* <Dropdown 
            trigger={'click'} 
            open={isOpen} 
            onOpenChange={(e) => setIsOpen(e)} 
            dropdownRender={(menu) => (
              <div className='bg-white rounded-ot shadow-md w-[400px] overflow-hidden pt-1 relative max-h-[635px]'>
                <div className='max-h-[600px] divide-y overflow-auto'>
                  {
                    notifications?.map((item) => (
                      <div 
                        className={`flex flex-col gap-1 px-4 py-1 cursor-pointer hover:bg-blue-100 transition-all ${item.ViewStatus === 0 ? 'font-bold' : ''}`}
                        onClick={() => sendNotifIndex(item.NotifIndex)}
                      >
                        <div className='text-[12px]'>{item?.Description}</div>
                        <div className='w-full flex justify-between items-center'>
                          <div className='self-end text-[11px] text-gray-600'>{item.ChangeEmployee}</div>
                          <div className='self-end text-[11px] text-gray-400'>{item.RelativeTime}</div>
                        </div>
                      </div>
                    ))
                  }
                </div>
                <div className='bg-white'>
                  <Link to={`/tas/notifications`} onClick={() => setIsOpen(false)} className=' py-1 hover:underline decoration-blue-400 block text-center border-t'>
                    <span className='text-blue-400'>See all</span>
                  </Link>
                </div>
              </div>
            )}
          >
            <Badge count={notificationNumber} size='small'>
              <button onClick={getNotifications}>
                <BellFilled className='text-white text-lg' />
              </button>
            </Badge>
          </Dropdown> */}

          {/* ////////////////////////////    User Information    ///////////////////////////////// */}

          <Dropdown trigger={'click'} dropdownRender={(menu) => (
            <div className='bg-white rounded-ot shadow-md overflow-hidden pt-1'>
              <div className='flex gap-2 items-center px-4 py-1 cursor-default'>
                <UserOutlined style={{fontSize: '16px'}}/>
                <div>{state.userInfo?.Firstname} {state.userInfo?.Lastname}</div>
              </div>
              <div className='flex gap-2 items-center px-4 py-1 cursor-default'>
                <SettingOutlined style={{fontSize: '16px'}}/>
                <div>{state.userInfo.Role}</div>
              </div>
              <div className='flex gap-2 items-center px-4 py-1 cursor-default'>
                {state.userInfo?.ReadonlyAccess ? <LockOutlined style={{fontSize: '16px'}}/> : <UnlockOutlined style={{fontSize: '16px'}}/>}
                <div>{state.userInfo?.ReadonlyAccess ? 'Only read' : 'Full access'}</div>
              </div>
              <div className='flex gap-2 items-center px-4 py-1 cursor-default'>
                <PhoneOutlined rotate={90} style={{fontSize: '16px'}}/>
                <div>{state.userInfo.Mobile}</div>
              </div>
              <div className='flex gap-2 items-center px-4 py-1 cursor-default'>
                <MailOutlined style={{fontSize: '16px'}}/>
                <div>{state.userInfo.Email}</div>
              </div>
              <Link to={`/tas/people/search/${state.userInfo?.Id}`} className='py-1 hover:bg-gray-100 block text-center border-t'>
                <span className='text-blue-400'>more</span>
              </Link>
            </div>
          )}>
            <div className='flex flex-col cursor-pointer pl-5'>
              <div className='text-xs font-semibold'>{state.userInfo.Firstname}.{state.userInfo.Lastname[0]}</div>
              <div className='text-xs'>{state.userInfo.Role}</div>
            </div>
          </Dropdown>
          <Tooltip title='Help'>
            <a type='button' href='https://mgs-organization.gitbook.io/ot-tas' target='_blank' className='flex items-center gap-2'>
              <QuestionCircleFilled style={{fontSize: '20px'}}/>
            </a>
          </Tooltip>
        </div>
        {contextHolder}
      </header>
    </>
  )
}

export default Header