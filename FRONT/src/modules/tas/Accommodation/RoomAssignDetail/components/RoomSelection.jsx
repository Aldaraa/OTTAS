import { SearchOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, ExpandRowRoomDetail, Form, Table } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect, useState } from 'react'
import ExpandRow from './ExpandRow'
import dayjs from 'dayjs'

function RoomSelection({onSelect, startDate, endDate}) {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)

  const { state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    if(startDate){
      form.setFieldValue('StartDate', startDate)
    }
    if(endDate){
      form.setFieldValue('EndDate', endDate)
    }
  },[startDate, endDate])

  const findAvailableRoom = (values) => {
    setLoading(true)
    setSearchLoading(true)
    axios({
      method: 'post',
      url: 'tas/roomassignment/findavailablebydatesassignment',
      data: {
        ...values,
        EmployeeId: state.userProfileData?.Id,
        Private: values.Private,
        StartDate: dayjs(values.StartDate).format('YYYY-MM-DD'),
        EndDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
      }
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => {setLoading(false); setSearchLoading(false)})
  }

  const fields = [
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-6 mb-2',
      type: 'date',
      inputprops: {
        disabled: true
      }
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-6 mb-2',
      type: 'date',
      inputprops: {
        disabled: true
      }
    },
    {
      label: 'Camp',
      name: 'CampId',
      className: 'col-span-6 mb-2',
      type: 'select',
      // rules: [{required: true, message: 'Camp is required'}],
      inputprops: {
        options: state.referData?.camps,
      }
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
      className: 'col-span-6 mb-2'
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      depindex: 'CampId',
      // rules: [{required: true, message: 'Room Type is required'}],
      inputprops: {
        optionsurl: 'tas/roomtype?active=1&campId=',
        loading: state.customLoading,
        optionvalue: 'Id', 
        optionlabel: 'Description',
      }
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      className: 'col-span-6 mb-2',
      type: 'number',
      inputprops: {
        min: 0
      }
    },
    {
      label: 'Private',
      name: 'Private',
      className: 'col-span-12 xl:col-span-6 2xl:col-span-2 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      } 
    },
  ]

  const columns = [
    {
      label: 'Room Number',
      name: 'roomNumber',
      width: '150px'
    },
    {
      label: 'Bed #',
      name: 'BedCount',
      alignment: 'left',
      width: 100,
    },
    {
      label: 'Owner #',
      name: 'RoomOwners',
      alignment: 'left',
      width: 100,
    },
    {
      label: 'OnSite',
      name: 'Employees',
      alignment: 'left',
    },
    {
      label: 'Owner In Date',
      name: 'OwnerInDate',
      cellRender: ({value}) => (
        <div>{value ? dayjs(value).format('YYYY-MM-DD HH:mm') : null}</div>
      )
    },
    {
      label: '',
      name: 'action',
      width: '90px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => onSelect(e.data)}>Select</button>
        </div>
      )
    },
  ]

  return (
    <div>
      <Form 
        form={form}
        fields={fields} 
        size='small' 
        className={'border rounded-ot p-4 gap-x-8 mb-5'}
        initValues={{Private: null}}
        onFinish={findAvailableRoom}
      >
        <div className='col-span-12 flex justify-end'>
          <Button htmlType='submit' icon={<SearchOutlined/>} loading={searchLoading}>Search</Button>
        </div>
      </Form>
      <Table
        containerClass='shadow-none border'
        columns={columns}
        data={data}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='RoomId'
        focusedRowEnabled={false}
        tableClass='max-h-[500px]'
        showRowLines={true}
        pager={true}
        renderDetail={{
          enabled: true, 
          component: (data) => {
            return (
              <ExpandRowRoomDetail
                propData={data}
                startDate={startDate}
                endDate={endDate}
              />
            )
          }
        }}
      />
    </div>
  )
}

export default RoomSelection