import { DatePicker, Tag } from 'antd'
import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useCallback, useContext, useEffect, useState } from 'react'
import COLORS from 'constants/colors'

function SelectTRFieldForRemoved({form, onSelect}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ selectedDate, setSelectedDate ] = useState([null, null])
  const [ flights, setFlights ] = useState([])
  const [ searchLoading, setSearchLoading ] = useState(false)
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(selectedDate && state.userProfileData){
      setSearchLoading(true)
      axios({
        method: 'get',
        url: `tas/transport?employeeId=${state.userProfileData?.Id}${selectedDate[0] ? `&startDate=${dayjs(selectedDate[0])?.format('YYYY-MM-DD')}` : ''}${selectedDate[1] ?  `&endDate=${dayjs(selectedDate[1]).format('YYYY-MM-DD')}` : ''}`,
      }).then((res) => {
        setFlights(res.data.filter((item) => item))
      }).catch((err) => {
    
      }).then(() => setSearchLoading(false))
    }
  },[selectedDate, state.userProfileData])

  const column = [
    {
      label: 'Date',
      name: 'EventDate',
      cellRender: (e) => (
        <div>{dayjs(e.data.EventDate).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Transport Mode',
      name: 'TransportMode',
    },
    {
      label: 'Code',
      name: 'Code',
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Direction',
      name: 'Direction',
      cellRender: ({value}) => (
        <Tag color={COLORS.Directions[value]?.tagColor}>{value}</Tag>
      )
    },
    {
      label: 'Status',
      name: 'Status',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'success' : 'orange'} className='text-xs'>{e.value}</Tag>
      )
    },
    {
      label: 'Reschedule',
      name: '',
      alignment: 'center',
      cellRender: (e) => (
        e.data.Direction !== 'EXTERNAL' ?
        <button type='button' onClick={() => handleClickReschedule(e.data)} className='edit-button'>
          Select
        </button>
        : null
      )
    },
  ]

  const handleClickReschedule = useCallback((rowData) => {
    if(onSelect){
      onSelect(rowData)
    }
    // form.setFieldValue('startDate', dayjs(rowData.EventDate))
    // form.setFieldValue('ExistingScheduleDirection', rowData.Direction)
    // form.setFieldValue('ExistingScheduleDescription', `${rowData.Code} ${rowData.Description}`)
    // form.setFieldValue('existingScheduleId', rowData.ScheduleId)
    setShowModal(false)
  }, [form])

  const handleSelectTr = () => {
    setShowModal(true)
  }

  return (
    <>
      <Form.Item className='col-span-1 mb-0'>
        <Button
          className='text-xs py-[3px]'
          type={'primary'}
          onClick={handleSelectTr}
        >...</Button>
      </Form.Item>
      <Modal
        width={800}
        open={showModal}
        onCancel={() => setShowModal(false)}
        title='Select existing transport'
      >
        <div className='flex flex-col border rounded-ot'>
          <div className='flex px-2 py-2 justify-between items-center leading-none'>
            <DatePicker.RangePicker
              disabled={false}
              value={selectedDate}
              onChange={(e) => setSelectedDate(e)}
            />
          </div>
          <Table
            data={flights}
            columns={column}
            allowColumnReordering={false}
            loading={searchLoading}
            pager={flights.length > 20}
            containerClass='shadow-none border-t rounded-t-none'
            keyExpr='EventDate'
          />
        </div>
      </Modal>
    </>
  )
}

export default SelectTRFieldForRemoved