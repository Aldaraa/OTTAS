import { Form, Button } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm, } from 'antd'
import { SearchOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import BookingInfoTable from './BookingInfoTable'

const title = 'Booking Information'

function ManageScheduleList() {
  const [ data, setData ] = useState([])
  const [ searchForm ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)
  const [ loading, setLoading ] = useState(false)

  useEffect(() => {
    action.changeMenuKey('/tas/bookinginfo')
  },[])

  const getData = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: `tas/transport/bookinginfo`,
      data: {
        ...values,
        StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : '',
        EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : '',
      }
    }).then((res) => {
      setData(res.data)
    }).finally(() => setLoading(false))
  }


  const searchFields = [
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-12 lg:col-span-6 row-span-12 lg:row-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true
      }
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-12 lg:col-span-6 row-span-12 lg:row-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true
      }
    },
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-12 lg:col-span-6 row-span-12 lg:row-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData.employers,
        allowClear: true,
        showSearch: true,
      } 
    },
    {
      label: 'Department',
      name: 'DepartmentId',
      className: 'col-span-12 lg:col-span-6 row-span-12 lg:row-span-6 mb-2',
      type: 'treeSelect',
      inputprops: {
        treeData: state.referData.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      } 
    },
    {
      label: 'SAP ID(s)',
      name: 'SAPIds',
      className: 'col-span-12 lg:col-span-6 row-span-12 lg:row-span-6 mb-2',
      type: 'textarea'
    },
    {
      label: 'Employee Id(s)',
      name: 'EmpIds',
      className: 'col-span-12 lg:col-span-6 row-span-12 lg:row-span-6 mb-2',
      type: 'textarea',
      inputprops: {
        showCount: true
      }
    },
    
  ]
  
  return (
    <>
      <div>
        <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
          <div className='text-lg font-bold mb-2'>{title}</div>
          <Form 
            form={searchForm} 
            fields={searchFields}
            className='grid grid-cols-12 gap-x-8 max-w-[800px]' 
            onFinish={getData}
            size='small'
            initValues={{
              SAPIds: '',
              EmpIds: '',
              StartDate: dayjs(),
              EndDate: dayjs().add(1, 'days'),
            }}
            labelCol={{flex: '100px'}}
          >
            <div className='flex gap-4 items-baseline col-span-12 justify-end mt-3'>
              <Button
                htmlType='submit'
                icon={<SearchOutlined/>}
                loading={loading}
              >
                Search
              </Button>
            </div>
          </Form>
        </div>
        <BookingInfoTable data={data}/>
      </div>
    </>
  )
}

export default ManageScheduleList