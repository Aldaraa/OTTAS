import { SearchOutlined } from '@ant-design/icons'
import { Tag } from 'antd'
import axios from 'axios'
import { Button, Form, Table } from 'components'
import dayjs from 'dayjs'
import lowerCase from 'lodash/lowerCase'
import React, { useEffect, useState } from 'react'

function ActiveTransportSchedule() {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ location, setLocation ] = useState([])
  const [ transportMode, setTransportMode ] = useState([])
  const [ form ] = Form.useForm()

  useEffect(() => {
    getData(form.getFieldsValue())
    axios({
      method: 'get',
      url: 'tas/location?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.Id, label: `${item.Code} - ${item.Description}`, ...item})
      })
      setLocation(tmp)
    }).catch((err) => {

    })
    axios({
      method: 'get',
      url: 'tas/transportmode?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        if(lowerCase(item.Code) !== 'drive'){
          tmp.push({value: item.Id, label: item.Code, ...item})
        }
      })
      setTransportMode(tmp)
    }).catch((err) => {

    })
  },[])

  const getData = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: `/tas/transportschedule/monthtransportschedule`,
      data: {
        ...values,
        ScheduleDate: values.ScheduleDate ? dayjs(values.ScheduleDate).format('YYYY-MM-DD') : null,
      }
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const columns = [
    {
      label: 'Travel Date',
      name: 'EventDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Code',
      name: 'Code',
      alignment: 'left',
    },
    {
      label: 'Direction',
      name: 'Direction',
      alignment: 'left',
      cellRender: (e) => (
        e.value &&
        <Tag color={e.value === 'IN' ? 'green' : e.value === 'OUT' ? 'blue' : 'magenta'} className='text-center text-[10px] font-semibold'>{e.value}</Tag>
      )
    },
    {
      label: 'Description',
      name: 'Description',
      alignment: 'left',
    },
    {
      label: 'Seats #',
      name: 'Seats',
      alignment: 'left',
    },
    // {
    //   label: 'Confirmed',
    //   name: 'Confirmed',
    //   alignment: 'left',
    // },
    {
      label: 'Available Seats',
      name: 'availableseats',
      alignment: 'left',
      cellRender: (e) => {
        const availableSeats = e.data.Seats - e.data.Confirmed
        return <div className={`${availableSeats <= 0 ? 'text-red-500 font-bold' : 'text-green-600 font-bold'}`}>{availableSeats}</div>
      }
    },
  ]

  const searchFields = [
    {
      label: 'Port Of Departure',
      name: 'fromLocationId',
      className: 'col-span-12 lg:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        options: location
      }
    },
    {
      label: 'Port Of Arrive',
      name: 'toLocationId',
      className: 'col-span-12 lg:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        options: location
      }
    },
    {
      label: 'Transport Code',
      name: 'transportCode',
      className: 'col-span-12 lg:col-span-4 mb-2',
    },
    {
      label: 'Schedule Date',
      name: 'ScheduleDate',
      className: 'col-span-12 lg:col-span-4 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true
      }
    },
    {
      label: 'Mode',
      name: 'TransportModeId',
      className: 'col-span-12 lg:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        options: transportMode
      }
    },
  ]

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-5 shadow-md'>
        <div className='text-lg font-bold mb-3'>Active Transport</div>
        <Form
          form={form}
          fields={searchFields}
          className='grid grid-cols-12 gap-x-5'
          onFinish={getData}
          size='small'
        >
          <div className='flex gap-4 justify-end col-span-12'>
            <Button htmlType='submit' loading={loading} icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
       <Table
        data={data}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        containerClass='shadow-none border border-gray-300 mt-5'
        keyExpr='Id'
        pager={data.length > 20}
        tableClass='max-h-[calc(100vh-300px)]'
      />
    </div>
  )
}

export default ActiveTransportSchedule