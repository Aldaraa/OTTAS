import React, { useContext, useEffect, useState } from 'react'
import { CheckOutlined, CloseCircleOutlined, SearchOutlined } from '@ant-design/icons'
import { Button, Form, Modal, SeatTooltip, Tooltip } from 'components'
import { Checkbox, DatePicker, Input, Select } from 'antd'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import axios from 'axios'
import ChangeRoom from './ChangeRoom'

function RemoveExistingTravel({form, editData, currentGroup, disabled}) {
  const { state } = useContext(AuthContext)
  const [ isCheckedRoom, setIsCheckedRoom ] = useState(false)
  const [ inited, setInited ] = useState(false)
  
  const [ showModal, setShowModal ] = useState(false)
  const [ selectedRoom, setSelectedRoom ] = useState(null)

  useEffect(() => {
    let initialvalues = {}
    if(editData && state.userProfileData){
        initialvalues = {
          ...editData,
          ShiftId: editData.shiftId,
          Room: {
            RoomId: editData.RoomId,
            roomNumber: editData?.RoomNumber,
          },
          firstTransport: {
            flightId: editData.FirstScheduleId,
            NoShow: editData.FirstScheduleNoShow,
            Direction: editData.FirstScheduleDirection,
            Date: editData.FirstScheduleDate ? dayjs(editData.FirstScheduleDate) : null,
            Description: editData.FirstScheduleDirection === 'IN' ? editData.FirstScheduleDescription : editData.FirstScheduleDescription,
          },
          lastTransport: {
            flightId: editData.LastScheduleId,
            NoShow: editData.LastScheduleNoShow,
            Direction: editData.LastScheduleDirection,
            Date: editData.LastScheduleDate ? dayjs(editData.LastScheduleDate) : null,
            Description: editData.LastScheduleDirection === 'IN' ? editData.LastScheduleDescription : editData.LastScheduleDescription,
          },
          CostCodeId: editData?.CostCodeId
      }
      form.setFieldsValue(initialvalues)
      setInited(true)
    }
  },[editData, state.userProfileData])

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='First Transport' className='col-span-12 mb-2' shouldUpdate={(prev, cur) => prev?.firstTransport?.Date !== cur?.firstTransport?.Date}>
        {({getFieldValue}) => {
          const firstDate = getFieldValue(['firstTransport', 'Date'])
          return(
            <div className='flex gap-2'>
              <Form.Item name={['firstTransport', 'Date']} className='mb-0 w-[115px]' noStyle={!firstDate}>
                { firstDate ? <DatePicker disabled className='w-full'/> : null}
              </Form.Item>
              <Form.Item name={['firstTransport', 'Direction']} className='mb-0 w-[65px]' noStyle={!firstDate}>
                { firstDate ? <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/> : null}
              </Form.Item>
              <Form.Item noStyle shouldUpdate={(pre, cur) => pre.firstTransport?.flightId !== cur.firstTransport?.flightId}>
                {({getFieldValue}) => {
                  const scheduleId = getFieldValue(['firstTransport', 'flightId']) 
                  return(
                    <Form.Item className='mb-0 flex-1' name={['firstTransport', 'Description']} noStyle={!firstDate} rules={[{required: true, message: 'Flight is required'}]}>
                      {firstDate ? <Input disabled addonAfter={<SeatTooltip id={scheduleId}/>}/> : null}
                    </Form.Item>
                  )
                }}
              </Form.Item>
              <Form.Item noStyle name={['firstTransport', 'flightId']}>
              </Form.Item>
              {
                !firstDate ? 
                <Tooltip title='Removed schedule'>
                  <Form.Item className='mb-0 w-full'>
                    <Input disabled status='error' prefix={<CloseCircleOutlined/>} value={editData?.FirstScheduleIdDescr} className='w-full'/>
                  </Form.Item>
                </Tooltip>
                : null
              }
              <Form.Item 
                className='mb-0' 
                name={['firstTransport', 'NoShow']}
                label=''
                valuePropName='checked'
                getValueFromEvent={(e) => e.target.checked ? 1 : 0}
              >
                <Checkbox>No Show</Checkbox>
              </Form.Item>
            </div>
          )
        }}
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='Last Transport' className='col-span-12 mb-2' shouldUpdate={(prev, cur) => prev.lastTransport?.Date !== cur.lastTransport?.Date}>
        {({getFieldValue}) => {
          const lastDate = getFieldValue(['lastTransport', 'Date'])
          return(
            <div className='flex gap-2'>
              <Form.Item name={['lastTransport', 'Date']} className='mb-0 w-[115px]' noStyle={!lastDate}>
                {lastDate ? <DatePicker disabled className='w-full'/> : null}
              </Form.Item>
              <Form.Item name={['lastTransport', 'Direction']} className='mb-0 w-[65px]' noStyle={!lastDate}>
                {lastDate ? <Select disabled options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]} className='w-full'/> : null}
              </Form.Item>
              <Form.Item noStyle shouldUpdate={(pre, cur) => pre.lastTransport?.flightId !== cur.lastTransport?.flightId}>
                {({getFieldValue}) => {
                  const scheduleId = getFieldValue(['lastTransport', 'flightId'])
                  return(
                    <Form.Item className='mb-0 flex-1' name={['lastTransport', 'Description']} rules={[{required: true, message: 'Flight is required'}]} noStyle={!lastDate}>
                      {lastDate ? <Input disabled addonAfter={<SeatTooltip id={scheduleId}/>}/> : null}
                    </Form.Item>
                  )
                }}
              </Form.Item>
              <Form.Item noStyle name={['lastTransport', 'flightId']}>
              </Form.Item>
              {
                !lastDate ? 
                <Tooltip title='Removed schedule'>
                  <Form.Item className='mb-0 w-full'>
                    <Input disabled status='error' prefix={<CloseCircleOutlined/>} value={editData?.LastScheduleIdDescr} className='w-full'/>
                  </Form.Item>
                </Tooltip>
                : null
              }
              <Form.Item 
                className='mb-0' 
                name={['lastTransport', 'NoShow']}
                label=''
                valuePropName='checked'
                getValueFromEvent={(e) => e.target.checked ? 1 : 0}
              >
                <Checkbox>No Show</Checkbox>
              </Form.Item>
            </div>
          )
        }}
      </Form.Item>
      </>
    },
    {
      label: 'Shift Status',
      name: 'ShiftId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Shift Status is required'}],
      inputprops: {
        style: {width: 'auto'},
        options: editData?.FirstScheduleDirection === 'IN' ? state.referData?.roomStatuses?.filter((row) => row.OnSite === 0) : state.referData?.roomStatuses?.filter((row) => row.OnSite === 1),
      }
    },
    {
      label: 'Cost Code',
      name: 'CostCodeId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Cost Code is required'}],
      inputprops: {
        style: {width: 'auto', minWidth: '200px'},
        options: state.referData?.costCodes,
      }
    },
  ]

  const handleCheckRoom = () => {
    axios({
      method: 'post',
      url: 'tas/room/findavailablebyroomid',
      data: {
        startDate: editData?.FirstScheduleDate,
        endDate: editData?.LastScheduleDate,
        roomId: state.userProfileData?.RoomId,
        EmployeeId: state.userProfileData?.Id,
      }
    }).then((res) => {
      if(res.data.status){
        form.setFieldValue(['Room', 'RoomId'], state.userProfileData?.RoomId)
        form.setFieldValue(['Room', 'roomNumber'], state.userProfileData?.RoomNumber)
      }else{
        form.setFieldValue(['Room', 'RoomId'], null)
        form.setFieldValue(['Room', 'roomNumber'], null)
      }
      setIsCheckedRoom(true)
    }).catch((err) => {
      
    })
  }

  const renderRoomChangeButton = () => {
    if(editData?.RoomId && !state.userProfileData?.RoomId){
      return(
        <button 
          className='border py-[5px] leading-none px-2 rounded-md bg-[#e57200] text-white hover:bg-[#ff9225] shadow-btn border-[#e57200] disabled:bg-[#ebb37a] disabled:border-[#ebb37a] hover:border-[#ff9225] transition-all' 
          type='button'
          onClick={() => setShowModal(true)}
          disabled={disabled}
        >
          <SearchOutlined/> Search
        </button>
      )
    }else{
      if(!state.userProfileData?.RoomId){
        return(
          <button 
            className='border py-[5px] leading-none px-2 rounded-md bg-[#e57200] text-white hover:bg-[#ff9225] shadow-btn border-[#e57200] disabled:bg-[#ebb37a] disabled:border-[#ebb37a] hover:border-[#ff9225] transition-all' 
            type='button'
            onClick={() => setShowModal(true)}
            disabled={disabled}
          >
            <SearchOutlined/> Search
          </button>
        )
      }else{
        if(isCheckedRoom && form.getFieldValue(['Room','RoomId']) && selectedRoom){
          return(
            <button 
              className='border py-[5px] leading-none px-2 rounded-md bg-[#e57200] text-white hover:bg-[#ff9225] shadow-btn border-[#e57200] disabled:bg-[#ebb37a] disabled:border-[#ebb37a] hover:border-[#ff9225] transition-all' 
              type='button'
              onClick={() => setShowModal(true)}
              disabled={disabled}
            >
              <SearchOutlined/> Search
            </button>
          )
        }else if(isCheckedRoom && form.getFieldValue(['Room','RoomId']) && !selectedRoom){
          return(
            <div className='flex items-center gap-2'>
              <Button 
                htmlType='button' 
                type={'success'} 
                disabled={true}
                icon={<CheckOutlined />}
                className='py-0'
              >
                Checked
              </Button>
              <button 
                className='border py-[5px] leading-none px-2 rounded-md bg-[#e57200] text-white hover:bg-[#ff9225] shadow-btn border-[#e57200] disabled:bg-[#ebb37a] disabled:border-[#ebb37a] hover:border-[#ff9225] transition-all' 
                type='button'
                onClick={() => setShowModal(true)}
                disabled={disabled}
              >
                <SearchOutlined/> Search
              </button>
            </div>
          )
        }else if(isCheckedRoom && !form.getFieldValue(['Room','RoomId']) && !selectedRoom){
          return(
            <button 
              className='border py-[5px] leading-none px-2 rounded-md bg-[#e57200] text-white hover:bg-[#ff9225] shadow-btn border-[#e57200] disabled:bg-[#ebb37a] disabled:border-[#ebb37a] hover:border-[#ff9225] transition-all' 
              type='button'
              onClick={() => setShowModal(true)}
              disabled={disabled}
            >
              <SearchOutlined/> Search
            </button>
          )
        }else{
          return(
            <Button 
              htmlType='button' 
              onClick={(e) => handleCheckRoom(e)}
              className='py-0'
              disabled={disabled}
            >
              Check
            </Button>
          )
        }
      }
    }
  }

  return (
    <div className='w-full max-w-[1200px]'>
      {console.log('valll', form.getFieldsValue())}
      {
        inited ?
        <Form
          form={form} 
          fields={fields} 
          size='small'
          labelAlign='right'
          labelCol={{flex: '120px'}}
          wrapperCol={{flex: 1}}
          disabled={disabled}
        >
          {
            editData?.FirstScheduleDirection === 'OUT' && currentGroup?.GroupTag === 'accomodation' ?
            <Form.Item label='Room' className='col-span-12 mb-0'>
              <div className='flex gap-2 items-start'>
                <Form.Item
                  key={`form-item-RoomId`}
                  name={['Room', 'roomNumber']}
                  className='mb-2'
                  rules={[{required: true, message: 'Room is required'}]}
                  validateStatus={isCheckedRoom && !form.getFieldValue(['Room','RoomId']) ? "error" : false}
                  help={isCheckedRoom && !form.getFieldValue(['Room','RoomId']) ? "Owner Room is unavailable" : false}
                >
                  <Input readOnly className=''/>
                </Form.Item>
                <Form.Item
                  key={`form-item-RoomI`}
                  noStyle
                  name={['Room', 'RoomId']}
                >
                  <Input className='hidden'/>
                </Form.Item>
                {renderRoomChangeButton()}
              </div>
            </Form.Item>
            : null
          }
          {/* {
            !editData?.FirstScheduleDate || !editData?.LastScheduleDate ? 
            <>
              <div className='col-span-12 flex justify-end gap-4'>
                <Form.Item>
                  <Button type='primary' onClick={handleSubmitRemove} icon={<SaveOutlined/>}>Save</Button>
                </Form.Item>
              </div>
            </>
            : null
          } */}
        </Form>
        : null
      }
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
          documentDetail={editData}
          handleSelect={(e) => setSelectedRoom(e)}
        />
      </Modal>
    </div>
  )
}

export default RemoveExistingTravel