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

function RoomField({form, isEdit, documentDetail}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ roomNotAvailable, setRoomNotAvailable ] = useState(false)
  const [ run, setRun ] = useState(false)
  const { state } = useContext(AuthContext)

  const firstTransport = Form.useWatch('firstTransport', form)
  const lastTransport = Form.useWatch('lastTransport', form)

  useEffect(() => {
    if(state.userProfileData?.RoomId){
      handleCheckRoom()
    }
  },[firstTransport, lastTransport, state.userProfileData])

  const handleCheckRoom = () => {
    const firstDate = form.getFieldValue(['firstTransport', 'Date'])
    const currentRoom = form.getFieldValue(['Room', 'RoomId'])
    const lastDate = form.getFieldValue(['lastTransport', 'Date'])
    if(firstDate && lastDate && currentRoom){
      axios({
        method: 'post',
        url: 'tas/room/findavailablebyroomid',
        data: {
          startDate: firstDate ? dayjs(firstDate).format('YYYY-MM-DD') : null,
          endDate: lastDate ? dayjs(lastDate).format('YYYY-MM-DD') : null,
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

  const handleForceCheckRoom = () => {
    const firstDate = form.getFieldValue(['firstTransport', 'Date'])
    const lastDate = form.getFieldValue(['lastTransport', 'Date'])
    const currentRoom = form.getFieldValue(['Room', 'RoomId'])
    if(firstDate && lastDate){
      axios({
        method: 'post',
        url: 'tas/room/findavailablebyroomid',
        data: {
          startDate: firstDate ? dayjs(firstDate).format('YYYY-MM-DD') : null,
          endDate: lastDate ? dayjs(lastDate).format('YYYY-MM-DD') : null,
          roomId: currentRoom,
          EmployeeId: state.userProfileData?.Id,
        }
      }).then((res) => {
        if(res.data.status){
          // form.setFieldValue(['Room', 'RoomId'], state.userProfileData?.RoomId)
          // form.setFieldValue(['Room', 'roomNumber'], state.userProfileData?.RoomNumber)
          setRoomNotAvailable(false)
        }else{
          // form.setFieldValue(['Room', 'RoomId'], state.referData?.noRoomId?.Id)
          // form.setFieldValue(['Room', 'roomNumber'], 'Virtual Room')
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
      <Form.Item label='Room' name='Room' className='col-span-12 mb-2'>
        <div className='flex gap-2 items-start'>
          <Form.Item
            key={`form-item-RoomId`}
            name={['Room', 'roomNumber']}
            className='flex-1 mb-0'
            rules={[{required: true, message: 'Room is required'}]}
            validateStatus={roomNotAvailable ? 'error' : false}
            help={
              roomNotAvailable ?
              <JumpAnimation run={run}>Owner Room is not available</JumpAnimation>
              : 
              false
            }
          >
            <Input readOnly/>
          </Form.Item>
          <Form.Item noStyle shouldUpdate={(pre, cur) => pre.Room?.RoomId !== cur.Room?.RoomId}>
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
                    state.userProfileData?.RoomId ? 
                    <Tooltip title='Check own room'>
                      <Button 
                        className='text-xs py-1 px-2'
                        type={state.userProfileData?.RoomId !== getFieldValue(['Room', 'RoomId']) ? 'default' : 'success'}
                        // disabled={state.userProfileData?.RoomId === getFieldValue(['Room', 'RoomId'])}
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
        title={`Change Room /${dayjs(form.getFieldValue(['firstTransport', 'Date'])).format('YYYY-MM-DD')} ${dayjs(form.getFieldValue(['lastTransport', 'Date'])).format('YYYY-MM-DD')}/`}
        width={900}
        forceRender={true}
      >
        <ChangeRoom 
          form={form}
          closeModal={() => setShowModal(false)}
          startDate={dayjs(form.getFieldValue(['firstTransport', 'Date'])).format('YYYY-MM-DD')}
          endDate={dayjs(form.getFieldValue(['lastTransport', 'Date'])).format('YYYY-MM-DD')}
          documentDetail={documentDetail}
          handleSelect={() => setRoomNotAvailable(false)}
        />
      </Modal>
    </>
  )
}

export default RoomField