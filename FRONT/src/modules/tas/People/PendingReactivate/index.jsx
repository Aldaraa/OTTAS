import React, { useContext, useEffect, useRef, useState } from 'react'
import { Table, Button } from 'components';
import { Popup } from 'devextreme-react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';

function PendingReactivate() {
  const [ loading, setLoading ] = useState(false)
  const [ editData, setEditData ] = useState([])
  const [ showPopup, setShowPopup ] = useState(false)
  const [ rejectLoading, setRejectLoading ] = useState(false)
  const [ data, setData ] = useState([])

  const { state } = useContext(AuthContext)
  const navigate = useNavigate()
  const dataGrid = useRef(null)

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/statuschangesemployeerequest/reactive'
    }).then((res) => setData(res.data))
    .catch((err) => {})
    .then(() => setLoading(false))
  }

  const columns = [
    {
      label: 'Person #',
      name: 'Id',
      dataType: 'string',
      width: '70px',
    },
    {
      label: 'FullName',
      name: 'FullName',
    },
    {
      label: 'EventDate',
      name: 'EventDate',
      cellRender: (row) => (
        <div>{dayjs(row.value).format('YYYY-MM-DD HH:mm')}</div>
      )
    },
    {
      label: 'CreatedEmployeeName',
      name: 'CreatedEmployeeName',
    },
    {
      label: 'CreatedDate',
      name: 'CreatedDate',
    },
    {
      label: '',
      name: '',
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
          <div className='text-lg font-bold'>Pending Reactivate</div>
        </div>
      </div>
      <Table
        ref={dataGrid}
        data={data}
        columns={columns}
        loading={loading}
        id="Id"
        className={`overflow-hidden ${!state.userInfo?.ReadonlyAccess && 'border-t'}`}
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

export default PendingReactivate