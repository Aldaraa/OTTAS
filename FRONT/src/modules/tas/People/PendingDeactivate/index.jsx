import React, { useEffect, useRef, useState } from 'react'
import { MemoTable, Button, Timer } from 'components';
import { Popup } from 'devextreme-react';
import axios from 'axios';
import { Tag } from 'antd';
import { Link } from 'react-router-dom';
import dayjs from 'dayjs';

function PendingDeactivate() {
  const [ loading, setLoading ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ rejectLoading, setRejectLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ data, setData ] = useState([])

  const dataGrid = useRef(null)

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/statuschangesemployeerequest/deactive'
    }).then((res) => setData(res.data))
    .catch((err) => {})
    .then(() => setLoading(false))
  }

  const columns = [
    {
      label: 'Fullname',
      name: 'FullName',
      dataType: 'string',
      cellRender:(e) => (
        <div className='flex items-center' >
          <Link to={`/tas/people/search/${e.row.data.EmployeeId}`}>
            <span className='text-blue-400 hover:underline'>{e.value}</span>
          </Link>
        </div>
      )
    },
    {
      label: 'Termination Type',
      name: 'TerminationTypeName',
      dataType: 'string',
    },
    {
      label: 'Comment',
      name: 'Comment',
      dataType: 'string',
    },
    {
      label: 'Requester',
      name: 'CreatedEmployeeName',
      dataType: 'string',
    },
    {
      label: 'CreatedDate',
      name: 'CreatedDate',
      dataType: 'string',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'EventDate',
      name: 'EventDate',
      dataType: 'string',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Deactive Date',
      name: 'ExectDate',
      cellRender: (e) => (
        <Tag color='blue'>
          <Timer 
            eventDate={e.row.data.EventDate}
            format='D[d] h:mm:ss'
          />
        </Tag>
      )
    },
    {
      label: '',
      name: 'action',
      width: '90px',
      alignment: 'center',
      cellRender: (e) => (
        <button type='button' className='dlt-button' onClick={() => handleRejectButton(e.row.data)}>Reject</button>
      )
    },
  ]

  const handleRejectButton = (data) => {
    setEditData(data)
    setShowPopup(true)
  }

  const handleReject = () => {
    setRejectLoading(true)
    axios({
      method: 'delete',
      url: `tas/statuschangesemployeerequest/${editData.Id}`,
    }).then((res) => {
      getData()
      setShowPopup(false)
    }).catch((err) => {
    }).then(() => setRejectLoading(false))
  }

  return (
    <div>
      <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
        <div className='flex items-center justify-between'>
          <div className='text-lg font-bold'>Pending Deactivate</div>
        </div>
      </div>
      <MemoTable
        tableRef={dataGrid}
        data={data}
        columns={columns}
        loading={loading}
        keyExpr="Id"
        showRowLines={true}
        pager={data.length > 20}
      />
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to reject this request?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleReject} loading={rejectLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
    </div>
  )
}

export default PendingDeactivate