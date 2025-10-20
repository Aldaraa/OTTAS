import { DatePicker, Form as AntForm, Tabs } from 'antd'
import axios from 'axios'
import { PeopleProfile } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import ExistingTransport from './ExistingTransport'
import RoomOccupancy from './RoomOccupancy'

function TeamBookings({empId}) {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ firstDate, setFirstDate ] = useState(dayjs())
  const [ lastDate, setLastDate ] = useState(dayjs().add(1, 'month'))

  const [ form ] = AntForm.useForm()
  // const { empId } = useParams()
  const { action } = useContext(AuthContext)
  const navigate = useNavigate()

  useEffect(() => {
    getData()
  },[firstDate, lastDate])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `/tas/transport/employee/existingtransport/${empId}/${dayjs(firstDate).format('YYYY-MM-DD')}/${dayjs(lastDate).format('YYYY-MM-DD')}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
    
  }

  const columns = [
    {
      label: 'Travel Date',
      name: 'TravelDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Direction',
      name: 'Direction',
      alignment: 'left',
    },
    {
      label: 'Dep',
      name: 'fromLocationCode',
      alignment: 'left',
      width: 50,
    },
    {
      label: 'ETD',
      name: 'ETD',
      alignment: 'left',
      width: 50,
    },
    {
      label: 'Arr',
      name: 'toLocationCode',
      alignment: 'left',
      width: 50,
    },
    {
      label: 'ETA',
      name: 'ETA',
      alignment: 'left',
      width: 50,
    },
    {
      label: 'Code',
      name: 'comment',
      alignment: 'left',
    },
    {
      label: 'Camp',
      name: 'comment',
      alignment: 'left',
    },
    {
      label: 'Status',
      name: 'status',
      alignment: 'left',
    },
    {
      label: 'Booking Ref#',
      name: 'comment',
      alignment: 'left',
    },
    {
      label: 'Request',
      name: 'comment',
      alignment: 'left',
    },
  ]

  const searchFields = [
    {
      label: 'From Date',
      name: 'startDate',
      className: 'col-span-6 mb-2',
      type: 'date',
      inputprops: {
        className: 'w-full',
      }
    },
    {
      label: 'To Date',
      name: 'endDate',
      className: 'col-span-6 mb-2',
      type: 'date',
      inputprops: {
        className: 'w-full',
      }
    },
  ]

  const items = [
    {
      key: '1',
      label: `Existing Transport`,
      children: <ExistingTransport firstDate={firstDate} lastDate={lastDate} empId={empId}/>
    },
    {
      key: '2',
      label: `Room Occupancy Enquiry`,
      children: <RoomOccupancy firstDate={firstDate} lastDate={lastDate} empId={empId}/>
    },
  ];
  

  return (
    <div className=''>
      <PeopleProfile employeeId={empId} />
      <Tabs 
        type="card"
        items={items}
        defaultActiveKey='1'
        className='mt-5'
        tabBarExtraContent={{
          right: <div className='flex gap-4 items-center py-2'>
            <div>
              <span className='mr-2'>From Date:</span>
              <DatePicker allowClear={false} value={firstDate} onChange={(e) => setFirstDate(dayjs(e))} size='small'/>
            </div>
            <div>
              <span className='mr-2'>To Date:</span>
              <DatePicker allowClear={false} value={lastDate} onChange={(e) => setLastDate(dayjs(e))} size='small'/>
            </div>
          </div>
        }}
      />
    </div>
  )
}

export default TeamBookings