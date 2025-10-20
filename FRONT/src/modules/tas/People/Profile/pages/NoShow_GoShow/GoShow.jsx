import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import React, { useCallback, useEffect, useRef, useState } from 'react'
import { useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { Popup } from 'devextreme-react'
import { PlusOutlined } from '@ant-design/icons'

function GoShow({selectedDate, fields=[]}) {
  const [ data, setData ] = useState([])
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editItem, setEditItem ] = useState(null)
  const { employeeId } = useParams()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(selectedDate){
      getData()
    }
  },[selectedDate])

  const getData = () => {
    dataGrid.current.instance.beginCustomLoading()
    axios({
      method: 'get',
      url: `tas/transport/goshow/${employeeId}/${dayjs(selectedDate).format('YYYY')}`,
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => dataGrid.current.instance.endCustomLoading())
  }

  const column = [
    {
      label: 'Date',
      name: 'EventDateTime',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Direction',
      name: 'Direction',
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Reason',
      name: 'Reason',
    },
    {
      label: '',
      name: 'action',
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleClickDelete(e.row.data)}>Delete</button>
      )
    }
  ]

  const handleSubmit = useCallback((values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/transport/nogoshow',
      data: {
        ...values,
        EventDate: values.EventDate ? dayjs(values.EventDate).format('YYYY-MM-DD') : null,
        EmployeeId: employeeId,
        NoShow: false,
      }
    }).then(() => {
      getData()
      handleClickCancel()
    }).catch(() => {

    }).finally(() => {
      setActionLoading(false)
    }) 
  },[employeeId])

  const handleDelete = useCallback(() => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/transport/goshow/${editItem.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch(() => {

    }).finally(() => {
      setActionLoading(false)
    }) 
  },[editItem])

  const handleClickAdd = useCallback(() => {
    setShowModal(true)
  },[])

  const handleClickCancel = useCallback(() => {
    setShowModal(false)
  },[])

  const handleClickDelete = useCallback((row) => {
    setEditItem(row)
    setShowPopup(true)
  },[])

  return (
    <>
      <div className='flex justify-end py-1'>
        <Button onClick={handleClickAdd} icon={<PlusOutlined/>}>Add</Button>
      </div>
      <Table
        ref={dataGrid}
        data={data}
        columns={column}
        allowColumnReordering={false}
        containerClass='shadow-none border'
        keyExpr='EventDate'
        pager={data.length > 20}
      />
      <Modal title={'Add No Show'} open={showModal} onCancel={handleClickCancel}>
        <Form onFinish={handleSubmit} fields={fields} labelCol={{flex: '90px'}}>
          <div className='col-span-12 flex justify-end gap-4'>
            <Form.Item className='mb-0'>
              <Button type='primary' htmlType='submit' loading={actionLoading}>Save</Button>
            </Form.Item>
            <Form.Item className='mb-0'>
              <Button onClick={handleClickCancel}>Cancel</Button>
            </Form.Item>
          </div>
        </Form>
      </Modal>
      <Popup visible={showPopup} showTitle={false} height={110} width={350}>
        <div>Are you sure you want to delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button htmlType='button' type='danger'onClick={handleDelete}>Yes</Button>
          <Button htmlType='button' onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </>
  )
}

export default GoShow