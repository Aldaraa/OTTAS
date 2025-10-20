import React, { useContext, useEffect, useState } from 'react'
import { BellFilled, LockOutlined, MailOutlined, PhoneOutlined, QuestionCircleFilled, SettingOutlined, UnlockOutlined, UserOutlined } from '@ant-design/icons'
import { Badge, Dropdown } from 'antd'
import { AuthContext } from 'contexts'
import { Link, NavLink, useNavigate } from 'react-router-dom'
import logo from 'assets/icons/OT_logo.png'
import axios from 'axios'
import { Tooltip } from 'components'

function HeaderGuest() {
  const [ isOpen, setIsOpen ] = useState(false)
  const { state, action } =  useContext(AuthContext)
  const [ notification, setNotification ] = useState(null)

  useEffect(() => {
    if(state.socket && state.userInfo){
      joinRoom()
      state.socket.on('SendMessage', (res) => {
        setNotification(res)
      })
      state.socket.on('socketUpdate', res => {
        state.socket.send('newMessage', state.userInfo.Id);
      })
    }
  },[state.socket, state.userInfo])
  
  const joinRoom = async () => {
    if (state.socket._connectionStarted) {
      try {
        await state.socket.send('newMessage', state.userInfo.Id);
      }
      catch(e) {
      }
    }
  }

  const handleClickNotif = (index) => {
    axios({
      method: 'get',
      url: `tas/notification/detail/${index}`
    }).then((res) => {
      setIsOpen(false)
      // navigate(res.data.link)
    }).catch((err) => {

    })
  }

  return (
    <header className='header h-[50px] text-white relative z-10 px-[25px] flex justify-between bg-[#27223f]'>
      <div className='flex items-center' >
        <img src={logo} width={60}/>
      </div>
      <div className='flex items-center gap-8'>
        <Dropdown 
          trigger={'click'} 
          open={isOpen} 
          onOpenChange={(e) => setIsOpen(e)} 
          disabled={notification?.employeeNoticationShortcutDetails.length === 0}
          dropdownRender={(menu) => (
            <div className='bg-white rounded-ot shadow-md w-[400px] overflow-hidden pt-1 relative max-h-[635px]'>
              <div className='max-h-[400px] overflow-auto'>
                {notification?.employeeNoticationShortcutDetails?.length > 0 ?
                  notification?.employeeNoticationShortcutDetails.map((item) => (
                    <div 
                      className={`flex flex-col gap-1 items-center px-4 py-1 cursor-pointer hover:bg-blue-100 transition-all ${item.viewStatus === 0 ? 'font-bold' : ''}`}
                      onClick={() => handleClickNotif(item.notifIndex)}
                    >
                      <div className='text-[12px]'>{item?.description}</div>
                      <div className='w-full flex justify-between items-center'>
                        <div className='self-end text-[11px] text-gray-600'>{item.changeEmployee}</div>
                        <div className='self-end text-[11px] text-gray-400'>{item.relativeTime}</div>
                      </div>
                    </div>
                  ))
                  : 
                  <div className='px-4 py-1'>No notification</div>
                }
              </div>
              {
                notification?.employeeNoticationShortcutDetails.length > 0 &&
                <div className='bg-white'>
                  <Link to={`/tas/notifications`} onClick={() => setIsOpen(false)} className=' py-1 hover:underline decoration-blue-400 block text-center border-t'>
                    <span className='text-blue-400'>See all</span>
                  </Link>
                </div>
              }
            </div>
          )}
        >
          <Badge count={notification?.newNotificationCount} size='small' className='cursor-pointer'>
            <BellFilled style={{color: notification?.employeeNoticationShortcutDetails.length > 0 ? 'white' : 'lightgray', fontSize: '18px'}} />
          </Badge>
        </Dropdown>
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
          </div>
        )}>
          <div className='flex flex-col cursor-pointer'>
            <div className='text-xs font-semibold'>{state.userInfo.Firstname}.{state.userInfo.Lastname[0]}</div>
            <div className='text-xs'>{state.userInfo.Role}</div>
          </div>
        </Dropdown>
        <Tooltip title='Help'>
          <button type='button' className='flex items-center gap-2'>
            <QuestionCircleFilled style={{fontSize: '20px'}}/>
          </button>
        </Tooltip>
      </div>
    </header>
  )
}

export default HeaderGuest