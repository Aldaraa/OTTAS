import axios from 'axios'
import { Table } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'

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

const ExistingTransport =  ({firstDate, lastDate}) => {
  const [ loading, setLoading ] = useState(false)
  const [ data, setData ] = useState([])

  const { empId } = useParams()

  useEffect(() => {
    setLoading(true)
    axios({
      method: 'get',
      url: `/tas/transport/employee/existingtransport/${empId}/${dayjs(firstDate).format('YYYY-MM-DD')}/${dayjs(lastDate).format('YYYY-MM-DD')}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  },[firstDate, lastDate])

   return (
     <div className='py-4 '>
       <Table
         data={data}
         columns={columns}
         allowColumnReordering={false}
         loading={loading}
         containerClass='shadow-none border border-gray-300'
         keyExpr='Id'
         pager={data.length > 20}
       />
     </div>
   )
 }

export default ExistingTransport