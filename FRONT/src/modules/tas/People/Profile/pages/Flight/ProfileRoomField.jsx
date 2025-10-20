import { Button, Form } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect, useState } from 'react'
import axios from 'axios'
import { Select } from 'antd'
import { twMerge } from 'tailwind-merge'
import { Popup } from 'devextreme-react'

const JumpAnimation = React.memo(({children, run}) => {
  return(
    <div className={twMerge('origin-center', run ? 'animate-jump' : '')}>{children}</div>
  ) 
})

function ProfileRoomField({form, startDate=null, endDate=null, ...restProps}) {
  const [ roomNotAvailable, setRoomNotAvailable ] = useState(false)
  const [ showRoomPopup, setShowRoomPopup ] = useState(false)
  const [ run, setRun ] = useState(false)
  const { state } = useContext(AuthContext)

  useEffect(() => {
    const currentRoom = form?.getFieldValue('RoomId')
    if(currentRoom && state.userProfileData){
      handleCheckRoom()
    }
  },[startDate, endDate, state.userProfileData, form])

  const handleCheckRoom = () => {
    const currentRoom = form.getFieldValue('RoomId')
    if(startDate && endDate && currentRoom){
      axios({
        method: 'post',
        url: 'tas/room/findavailablebyroomid',
        data: {
          startDate: startDate,
          endDate: endDate,
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

  const handleCheckOwnRoom = (e) => {
      setShowRoomPopup(true)
  }

  const handleApproveNoRoom = () => {
    setShowRoomPopup(false)
    setRoomNotAvailable(false)
    form.setFieldValue('RoomId', state.referData?.noRoomId?.Id)
  }

  return (
    <>
      <Form.Item label='Room' className='col-span-12 mb-2'>
        <div className='flex gap-2 items-start'>
          <Form.Item
            key={`form-item-RoomId`}
            name={'RoomId'}
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
            <Select disabled options={restProps?.inputprops?.options}/>
          </Form.Item>
          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.RoomId !== cur.RoomId}>
            {({getFieldValue}) => {
              const currentRoom = getFieldValue('RoomId')
              return roomNotAvailable || !currentRoom ?
              <Button 
                htmlType='button' 
                onClick={(e) => handleCheckOwnRoom(e)}
                disabled={!startDate}
              >
                Set No Room
              </Button>
              : null
            }}
          </Form.Item>
        </div>
      </Form.Item>
      <Popup
        visible={showRoomPopup}
        showTitle={false}
        height={135}
        width={400}
      >
        <div className='font-bold'>
          Room is unavailable on {startDate} - {endDate}
        </div>
        <div className='mt-4 text-center'>Do you wish to book No Room?</div>
        <div className='flex gap-5 mt-3 justify-center'>
          <Button type='primary' onClick={handleApproveNoRoom}>Yes</Button>
          <Button onClick={() => setShowRoomPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
    </>
  )
}

export default ProfileRoomField