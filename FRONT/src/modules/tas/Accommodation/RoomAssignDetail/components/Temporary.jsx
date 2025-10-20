import { Space } from 'antd'
import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import RoomSelection from './RoomSelection'
import dayjs from 'dayjs'

function Temporary({data, changeTab}) {
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ temporaryDates, setTemporaryDates ] = useState(null)
  const [ selectedRoom, setSelectedRoom ] = useState(null)
  const [ showModal, setShowModal ] = useState(false)
  const [ showBed, setShowBed ] = useState(false)
  const [ bedInfo, setBedInfo ] = useState([])
  
  const [ form ] = Form.useForm()
  const navigate = useNavigate()

  const temporaryFields = [
    {
      label: 'First Night',
      name: 'StartDate',
      className: 'col-span-12 mb-2',
      type: 'date',
      inputprops: {
      }
    },
    {
      label: 'Last Night',
      name: 'EndDate',
      className: 'col-span-12 mb-2',
      type: 'date',
      inputprops: {
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.StartDate !== curValues.StartDate || prevValues.EndDate !== curValues.EndDate}>
        {
          ({getFieldValue, setFieldValue}) => {
            return(
              <Form.Item label='Room' name={'RoomId'} className='mb-2 col-span-12'>
                <Space direction="horizontal">
                  <div className='w-[300px] border border-[#d9d9d9] bg-gray-50 text-gray-600 h-[30px] rounded-md p-1 px-2 flex items-center'>
                    {selectedRoom?.Descr}
                  </div>
                  <Button disabled={!getFieldValue('StartDate') || !getFieldValue('EndDate')} style={{width: 80}} onClick={handleChangeRoom}>Change</Button>
                </Space>
              </Form.Item>
            )
          }
        }
      </Form.Item>
    }
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/roomassignment/temporary',
      data: {
        ...values,
        EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
        StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : null,
        employeeId: data.Id,
        roomId: selectedRoom?.RoomId,
      }
    }).then((res) => {
      if(res.data.BedInfo.length > 0){
        setBedInfo(res.data.BedInfo)
        setShowBed(true)
      }else{
        form.resetFields()
        changeTab('roombooking')
      }
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleChangeRoom = () => {  
    setTemporaryDates({StartDate: form.getFieldValue('StartDate'), EndDate: form.getFieldValue('EndDate')})
    setShowModal(true)
  }

  const handleRoomSelect = (e) => {
    setShowModal(false)
    setSelectedRoom(e)
  }

  const bedCol = [
    {
      label: 'Date',
      name: 'EventDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
    {
      label: 'Bed Number',
      name: 'BedDescr',
    },
  ]

  return (
    <>
      <Form
        className={`mt-4 max-w-[500px] gap-x-4`}
        fields={temporaryFields}
        onFinish={handleSubmit}
        form={form}
        labelCol={{flex: '110px'}}
        wrapperCol={{flex: 1}}
      >
        <Form.Item className='col-span-12 flex justify-end mt-2'>
          <Button htmlType='submit' type={'primary'} loading={actionLoading}>Process</Button>
        </Form.Item>
      </Form>
      <Modal open={showModal} onCancel={() => setShowModal(false)} title='Assign room' width={900}>
        <RoomSelection
          onSelect={handleRoomSelect} 
          startDate={temporaryDates?.StartDate}
          endDate={temporaryDates?.EndDate} 
        />
      </Modal>
      <Modal open={showBed} onCancel={() => setShowBed(false)}>
        <Table
          data={bedInfo}
          columns={bedCol}
          title={<div className='border-b text-sm font-bold py-2'>Booked Data</div>}
        />
      </Modal>
    </>
  )
}

export default Temporary