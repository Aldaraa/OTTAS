import { DatePicker, Space } from 'antd'
import { Button, Form, Modal, Table } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useState } from 'react'
import RoomSelectionOwner from './RoomSelectionOwner'
import axios from 'axios'
import { AuthContext } from 'contexts'
import { Link } from 'react-router-dom'

function Ownership({data, changeTab}) {
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ selectedRoom, setSelectedRoom ] = useState(null)
  const [ showModal, setShowModal ] = useState(false)
  const [ warningData, setWarningData ] = useState([])
  const [ openWarning, setOpenWarning ] = useState(false)


  const { action, state } = useContext(AuthContext)
  const [ form ] = Form.useForm()
  const startDate = Form.useWatch('StartDate', form)

  const ownershipFields = [
    {
      type: 'component',
      component: <Form.Item name={'StartDate'} className='mb-2 col-span-12' label='First Night'>
        <DatePicker/>
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.StartDate !== curValues.StartDate}>
        {
          ({getFieldValue, setFieldValue}) => {
            return(
              <Form.Item label='Room' className='mb-2 col-span-12'>
                <Space direction="horizontal">
                  <div className='w-[300px] border border-[#d9d9d9] bg-gray-50 text-gray-600 h-[30px] rounded-md p-1 px-2 flex items-center'>
                    {selectedRoom?.Descr}
                  </div>
                  <Button disabled={!getFieldValue('StartDate')} style={{width: 80}} onClick={() => setShowModal(true)}>Change</Button>
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
      url: 'tas/roomassignment/ownership',
      data: {
        employeeId: data.Id,
        roomId: selectedRoom.RoomId,
        startDate: dayjs(values.StartDate).format('YYYY-MM-DD'), 
      }
    }).then((res) => {
      if(res.data.length > 0){
        setWarningData(res.data)
        setOpenWarning(true)
      }else{
        action.onSuccess('Operation successful')
        form.resetFields()
        changeTab('roombooking')
        action.changedFlight(state.ChangedFlight + 1)
      }
    }).catch((err) => {
      
    }).then(() => setActionLoading(false))
  }

  const handleRoomSelect = (e) => {
    setShowModal(false)
    setSelectedRoom(e)
  }

  const detailColumns = [
    {
      label: '#', 
      name: 'EmployeeId', 
      alignment: 'left',
      width: 80
    },
    {
      label: 'Fullname', 
      name: 'FullName', 
      alignment: 'left',
      cellRender: (e) => (
        // <div>
        //   <Link to={`/tas/roomassign/${e.data.EmployeeId}/roombooking`} target='_blank'>
            <span className='text-[12px] px-1'>{e.data.Firstname} {e.data.Lastname}</span>
        //   </Link>
        // </div>
      )
    },
    {
      label: 'People Type', 
      name: 'PeopleTypeName', 
      alignment: 'left',
    },
    {
      label: 'Employer', 
      name: 'EmployerName', 
      alignment: 'left',
    },
    {
      label: 'Position', 
      name: 'PositionName', 
      alignment: 'left',
    },
  ]

  return (
    <>
      <Form
        className={`mt-4 max-w-[500px] gap-x-4`} 
        fields={ownershipFields}
        onFinish={handleSubmit}
        initValues={{StartDate: dayjs()}}
        form={form}
        labelCol={{flex: '110px'}}
        wrapperCol={{flex: 1}}
      >
        <Form.Item className='col-span-12 flex justify-end mt-2'>
          <Button htmlType='submit' type={'primary'} loading={actionLoading}>Process</Button>
        </Form.Item>
      </Form>
      <Modal open={showModal} onCancel={() => setShowModal(false)} title='Assign room' width={900}>
        <RoomSelectionOwner
          onSelect={handleRoomSelect} 
          fromDate={startDate} 
        />
      </Modal>
      <Modal
        title='Room occupancy full'
        open={openWarning}
        onCancel={() => setOpenWarning(false)}
        width={800}
      >
        <Table
          containerClass='shadow-none border'
          columns={[
            {
              label: 'Event Date', 
              name: 'EventDate', 
              alignment: 'left',
              cellRender: (e) => (
                <span className='text-[12px] px-1'>
                  {dayjs(e.value).format('YYYY-MM-DD')} <span className='ml-2'>({e.data?.Guests?.length})</span>
                </span>
              )
            },
          ]}
          data={warningData}
          allowColumnReordering={false}
          keyExpr='EventDate'
          focusedRowEnabled={false}
          tableClass='max-h-[600px]'
          showRowLines={true}
          pager={true}
          renderDetail={{
            enabled: true, 
            component: ({data}) => {
              return <Table
                containerClass='shadow-none border'
                columns={detailColumns}
                data={data?.data?.Guests}
                allowColumnReordering={false}
                keyExpr='EventDate'
                focusedRowEnabled={false}
                showRowLines={true}
                pager={data.length > 20}
              />
            }
          }}
        />
      </Modal>
    </>
  )
}

export default Ownership