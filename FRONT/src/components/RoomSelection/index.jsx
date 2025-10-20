import React, { useContext, useEffect, useState } from 'react'
import { Form, Select } from 'antd'
import { Button } from 'components'
import { CheckOutlined } from '@ant-design/icons';
import axios from 'axios';
import { AuthContext } from 'contexts';
import { Popup } from 'devextreme-react';

function RoomSelection({name, label, className='', form, startDate=null, endDate=null, isWithDuration, onCheck, addOnAfter, handleChangeRoom, userData, buttonSize, ...restProps}) {
  // const [ camps, setCamps ] = useState([])
  const [ showRoomPopup, setShowRoomPopup ] = useState(false)
  const [ roomNotAvailable, setRoomNotAvailable ] = useState(false)
  const [ run, setRun ] = useState(false)

  const { state } = useContext(AuthContext)

  // useEffect(() => {
    // getData()
  // },[])

  // const getData = () => {
  //   axios({
  //     method: 'get',
  //     url: 'tas/camp?Active=1',
  //   }).then((res) => {
  //     let tmp = [] 
  //     res.data.map((item) => {
  //       tmp.push({ value: item.Id, label: `${item.Code} - ${item.Description}`})
  //     })
  //     setCamps(tmp)
      
  //   }).catch((err) => {

  //   })
  // }

  const handleForceCheckRoom = () => {
    const roomId = form?.getFieldValue(name)
    if(roomId){
      axios({
        method: 'post',
        url: 'tas/room/findavailablebyroomid',
        data: {
          startDate: startDate,
          endDate: endDate,
          roomId: roomId,
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
    if(!userData?.RoomId){
      setShowRoomPopup(true)
    }
  }

  const handleApproveNoRoom = () => {
    setShowRoomPopup(false)
    form.setFieldValue(name, state.referData?.noRoomId?.Id)
  }

  return (
    <>
      <Form.Item
        key={`form-item-${name}-custom`}
        label={label}
        className={className}
      >
        <Form.Item
          key={'custom-field-1'}
          name={name}
          className='mb-0'
          rules={[
            {
              required: true,
            },
          ]}
          style={{
            display: 'inline-block',
            width: 'calc(50% - 8px)',
          }}
          >
            <Select
              disabled
              options={restProps?.inputprops?.options}
            />
        </Form.Item>
        <Form.Item
          key={'custom-field-2'}
          style={{
            display: 'inline-block',
            margin: '0 8px',
          }}
          shouldUpdate={true}
        > 
        </Form.Item>
        <Form.Item
          style={{display: 'inline-block',margin: '0'}} 
          shouldUpdate={(prevValues, curValues) => prevValues[name] !== curValues[name]}
        >
          {({setFieldValue, getFieldValue}) => {
            return(
              getFieldValue(name) ?
              <Button 
                htmlType='button' 
                type={'success'} 
                disabled={true}
                icon={<CheckOutlined />}
              >
                Checked
              </Button>
              :
              <Button 
                htmlType='button' 
                onClick={(e) => handleCheckOwnRoom(e)}
                disabled={!startDate}
              >
                Check
              </Button>
            )
          }}
        </Form.Item>
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

export default RoomSelection