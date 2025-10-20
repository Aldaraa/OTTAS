import { InfoCircleOutlined, SearchOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Table } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox } from 'devextreme-react'
import React, { useContext, useState } from 'react'
import ExpandRow from './ExpandRow'
import dayjs from 'dayjs'

function RoomSelection({onSelect, fromDate}) {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)

  const { state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  const findAvailableRoom = (values) => {
    setLoading(true)
    setSearchLoading(true)
    axios({
      method: 'post',
      url: 'tas/room/roomassignavialable',
      data: {
        ...values,
        Private: values.Private,
        startDate: dayjs(fromDate).format('YYYY-MM-DD'),
      }
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => {setLoading(false); setSearchLoading(false)})
  }

  const fields = [
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
    },
    {
      label: 'Owner #',
      name: 'OwnerCount',
      alignment: 'left',
    },
    {
      label: 'OnSite',
      name: 'Employees',
      alignment: 'left',
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
        onFinish={findAvailableRoom}
        initValues={{Private: null}}
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
        tableClass='max-h-[calc(100vh-280px)]'
        showRowLines={true}
        pager={data.length > 20}
        renderDetail={{
          enabled: true, 
          component: (data) => {
            return (
              <ExpandRow
                propData={data}
                startDate={fromDate} 
              />
            )
          }
        }}
      />
    </div>
  )
}

export default RoomSelection  