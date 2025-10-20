import { Button, Form, Modal, Tooltip } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useCallback, useContext, useEffect, useMemo, useState } from 'react'
import ChangeRoom from './ChangeRoom'
import { CheckOutlined, EllipsisOutlined, SearchOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Input } from 'antd'
import { FaHouseUser } from 'react-icons/fa'
import { twMerge } from 'tailwind-merge'

const JumpAnimation = React.memo(({children, run}) => {
  return(
    <div className={twMerge('origin-center', run ? 'animate-jump' : '')}>{children}</div>
  ) 
})

function ReScheduleRoomField({form, isEdit, initData}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ roomNotAvailable, setRoomNotAvailable ] = useState(false)
  const [ run, setRun ] = useState(false)
  const { state } = useContext(AuthContext)

  const isStart = useMemo(() => {
    if(initData){
      return dayjs(initData?.startDate).diff(initData?.endDate) > 0
    }
  },[initData])

  useEffect(() => {
    if(initData?.RoomId && initData?.ReScheduleId !== 0 && initData?.existingScheduleId !== 0){
      handleCheckRoom()
    }
  },[initData])

  // const handleCheckRoom = () => {
  //   if(state.userProfileData?.RoomId){
  //     axios({
  //       method: 'post',
  //       url: 'tas/room/findavailablebyroomid',
  //       data: {
  //         startDate: isStart ? dayjs(initData?.endDate).format('YYYY-MM-DD') : dayjs(initData?.startDate).format('YYYY-MM-DD'),
  //         endDate: isStart ? dayjs(initData?.startDate).format('YYYY-MM-DD') : dayjs(initData?.endDate).format('YYYY-MM-DD'),
  //         roomId: state.userProfileData?.RoomId,
  //         EmployeeId: state.userProfileData?.Id,
  //       }
  //     }).then((res) => {
  //       if(res.data.status){
  //         setRoomNotAvailable(false)
  //       }else{
  //         setRoomNotAvailable(true)
  //         runAnimation()
  //       }
  //     }).catch((err) => {
        
  //     })
  //   }
  // }
  const handleCheckRoom = () => {
    axios({
      method: 'post',
      url: 'tas/room/findavailablebyroomid',
      data: {
        startDate: isStart ? dayjs(initData?.endDate).format('YYYY-MM-DD') : dayjs(initData?.startDate).format('YYYY-MM-DD'),
        endDate: isStart ? dayjs(initData?.startDate).format('YYYY-MM-DD') : dayjs(initData?.endDate).format('YYYY-MM-DD'),
        roomId: initData?.RoomId,
        EmployeeId: state.userProfileData?.Id,
      }
    }).then((res) => {
      if(res.data.status){
        setRoomNotAvailable(false)
      }else{
        setRoomNotAvailable(true)
        runAnimation()
      }
    }).catch((err) => {
      
    })
  }

  const handleForceCheckRoom = () => {
    const firstDate = form.getFieldValue('startDate')
    const lastDate = form.getFieldValue('endDate')
    const currentRoom = form.getFieldValue('RoomId')
    if(initData?.RoomId){
      axios({
        method: 'post',
        url: 'tas/room/findavailablebyroomid',
        data: {
          startDate: isStart ? dayjs(lastDate).format('YYYY-MM-DD') : dayjs(firstDate).format('YYYY-MM-DD'),
          endDate: isStart ? dayjs(firstDate).format('YYYY-MM-DD') : dayjs(lastDate).format('YYYY-MM-DD'),
          roomId: currentRoom,
          EmployeeId: state.userProfileData?.Id,
        }
      }).then((res) => {
        if(res.data.status){
          setRoomNotAvailable(false)
        }else{
          setRoomNotAvailable(true)
          runAnimation()
        }
      }).catch((err) => {
        
      })
    }
  }

  const runAnimation = () => {
    setRun(true)
    setTimeout(() => {
      setRun(false)
    }, 700)
  }

  return (
    <>
      <Form.Item label='Room' className='col-span-12 mb-2'>
        <div className='flex gap-2 items-start'>
          <Form.Item
            name={'RoomId'}
            noStyle
          >
          </Form.Item>
          <Form.Item
            key={`form-item-RoomId`}
            name={'RoomNumber'}
            className='flex-1 mb-0'
            rules={[{required: true, message: 'Room is required'}]}
            validateStatus={roomNotAvailable ? 'error' : false}
            help={
              roomNotAvailable ?
              <JumpAnimation run={run}>Room is not available</JumpAnimation>
              : 
              false
            }
          >
            <Input readOnly/>
          </Form.Item>
          <Form.Item noStyle shouldUpdate={(pre, cur) => pre.RoomId !== cur.RoomId}>
            {({getFieldValue}) => {
              return(
                <>
                  <Button 
                    className='text-xs py-1 px-2'
                    type={'primary'}
                    onClick={() => setShowModal(true)}
                    disabled={!isEdit}
                    icon={<EllipsisOutlined />}
                  />
                  {
                    initData?.RoomId ? 
                    <Tooltip title='Check room'>
                      <Button 
                        className='text-xs py-1 px-2'
                        type={state.userProfileData?.RoomId !== getFieldValue('RoomId') ? 'default' : 'success'}
                        disabled={!getFieldValue('startDate') || !getFieldValue('endDate')}
                        icon={<FaHouseUser size={14}/>}
                        onClick={handleForceCheckRoom}
                      />
                    </Tooltip>
                    : null
                  }
                </>
              )
            }}
          </Form.Item>
        </div>
      </Form.Item>
      <Modal
        open={showModal} 
        onCancel={() => setShowModal(false)} 
        title={`Change Room /${dayjs(form.getFieldValue('startDate')).format('YYYY-MM-DD')} ${dayjs(form.getFieldValue('endDate')).format('YYYY-MM-DD')}/`}
        width={900}
        forceRender={true}
      >
        <ChangeRoom
          form={form} 
          closeModal={() => setShowModal(false)}
          startDate={isStart ? dayjs(initData?.endDate).format('YYYY-MM-DD') : dayjs(initData?.startDate).format('YYYY-MM-DD')}
          endDate={isStart ? dayjs(initData?.startDate).format('YYYY-MM-DD') : dayjs(initData?.endDate).format('YYYY-MM-DD')}
          handleSelect={(e) => {
            form.setFieldValue('RoomId', e.RoomId)
            form.setFieldValue('RoomNumber', e.roomNumber)
          }}
        />
      </Modal>
    </>
  )
}

export default ReScheduleRoomField