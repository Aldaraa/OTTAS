import { Table } from 'components'
import React, { useEffect, useState } from 'react'
import dayjs from 'dayjs'
import axios from 'axios'
import { useParams } from 'react-router-dom'
import ls from 'utils/ls'

function AccountHistory() {
  const [ loading, setLoadng ] = useState(true)
  const [ data, setData ] = useState([])
  const { employeeId } = useParams()

  useEffect(() => {
    ls.set('pp_rt', 'history')
    getData()
  },[])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employee/accounthistory/${employeeId}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).finally(() => setLoadng(false))
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
      label: 'Termination Type',
      name: 'TerminationTypeName',
    },
    {
      label: 'Comment',
      name: 'Comment',
    },
    {
      label: 'Action',
      name: 'Action',
    },
  ]

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      <div className='text-lg font-bold mb-3'>Account history</div>
      <Table
        data={data}
        columns={column}
        loading={loading}
        allowColumnReordering={false}
        containerClass='shadow-none border'
        keyExpr='EventDate'
        pager={data.length > 20}
      />
    </div>
  )
}

export default AccountHistory