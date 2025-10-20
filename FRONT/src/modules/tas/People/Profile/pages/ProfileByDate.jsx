import { Button, Form, Table } from 'components'
import React, { useEffect, useState } from 'react'
import dayjs from 'dayjs'
import { DatePicker } from 'antd'
import axios from 'axios'
import { useParams } from 'react-router-dom'
import ls from 'utils/ls'

function ProfileByDate() {
  const [ data, setData ] = useState([])
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ form ] = Form.useForm()
  const { employeeId } = useParams()

  useEffect(() => {
    ls.set('pp_rt', 'profilebydate')
    getData()
  },[])

  const getData = () => {
    let values = form.getFieldsValue()
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/employeestatus/profilebydate/${employeeId}/${dayjs(values.StartDate).format('YYYY-MM-DD')}/${dayjs(values.EndDate).format('YYYY-MM-DD')}`,
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setSearchLoading(false))
  }

  const column = [
    {
      label: 'Date',
      name: 'EventDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
      )
    },
    {
      label: 'Cost Code',
      name: 'CostCode',
    },
    {
      label: 'Department',
      name: 'Department',
    },
    {
      label: 'Employer',
      name: 'Employer',
    },
    {
      label: 'Position',
      name: 'Position',
    },
    {
      label: 'Room',
      name: 'RoomNumber',
      dataType: 'string',
    },
    // {
    //   label: 'Commute Base / Transport',
    //   name: 'Location',
    // },
  ]

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      <div className='flex justify-between mb-3'>
        <div className='text-lg font-bold'>Profile By Date</div>
        <Form 
          className='flex items-center gap-2' 
          form={form} 
          onFinish={getData}
          initValues={{StartDate: dayjs().subtract(6, 'months'), EndDate: dayjs()}}
        >
          <Form.Item name='StartDate' className='mb-0'>
            <DatePicker placeholder='Start Date'/>
          </Form.Item>
          -
          <Form.Item name='EndDate' className='mb-0'>
            <DatePicker placeholder='End Date'/>
          </Form.Item>
          <Button htmlType='button' disabled={searchLoading} onClick={() => form.submit()}>Search</Button>
        </Form>
      </div>
      <Table
        data={data}
        columns={column}
        allowColumnReordering={false}
        containerClass='shadow-none border'
        keyExpr='EventDate'
        pager={data.length > 20}
        style={{maxHeight: 'calc(100vh - 340px)'}}
        loading={searchLoading}
      />
    </div>
  )
}

export default ProfileByDate