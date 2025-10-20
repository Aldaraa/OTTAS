import { Table } from 'components'
import React, { useEffect, useState } from 'react'
import { Button, DatePicker } from 'antd'
import axios from 'axios'
import { SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import dayjs from 'dayjs'

const { RangePicker } = DatePicker;

function TransportScheduleDetail() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ selectedDate, setSelectedDate ] = useState([false, false])

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/costcode'
    }).then((res) => {
      setData(res.data)
      setRenderData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSearch = (values) => {
    setSearchLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setSearchLoading(false)
    })
  }

  const columns = [
    {
      label: 'Camp',
      name: 'Camp',
      cellRender: (e) => (
        <div>{dayjs(e.data.EventDate).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Person #',
      name: 'Id',
      cellRender: (e) => (
        <div>{dayjs(e.data.EventDate).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Details',
      name: 'Details',
    },
    {
      label: 'Date In',
      name: 'Date in',
    },
    {
      label: 'Last Night',
      name: 'Last Night',
    },
    {
      label: 'Cost Code',
      name: 'CostCode',
    },
  ]

  return (
    <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
      <div className='text-lg font-bold mb-3'>Room Occupancy Enquiry</div>
      <div className='flex gap-5'>
        <RangePicker 
          value={selectedDate}
          onChange={(e) => setSelectedDate(e)}
        />
        <Button 
          onClick={handleSearch} 
          loading={searchLoading} 
          icon={<SearchOutlined/>}
        >
          Search
        </Button>
      </div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
         loading={loading}
        containerClass='shadow-none'
        keyExpr='Id'
      />
    </div>
  )
}

export default TransportScheduleDetail