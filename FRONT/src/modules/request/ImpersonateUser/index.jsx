import { Form, Table, Button } from 'components'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'  
import { Link, useNavigate } from 'react-router-dom'

const title = 'Impersonate User'

function ImpersonateUser() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ loading, setLoading ] = useState(false)

  const [ form ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  const dataGrid = useRef(null)

  useEffect(() => {
    action.changeMenuKey('/request/impersonateuser')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/requestgroupemployee'
    }).then((res) => {
      setData(res.data)
      setRenderData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }


  const columns = [
    {
      label: '#',
      name: 'EmployeeId',
      dataType: 'string',
      width: 80,
    },
    {
      label: 'Fullname',
      name: 'FullName',
    },
    {
      label: 'Email',
      name: 'Email',
    },
    {
      label: 'Task',
      name: 'TaskCount',
    },
    {
      label: '',
      name: 'action',
      width: '170px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <Link to={`/request/impersonateuser/${e.data.EmployeeId}`}>
            <button type='button' className='edit-button'>Impersonate & Tasks</button>
          </Link>
        </div>
      )
    },
  ]

  const handleSearch = (values) => {
    setLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setLoading(false)
    })
  }

  const searchFields = [
    {
      label: 'Full Name',
      name: 'FullName',
      className: 'col-span-12 mb-2',
      inputprops: {
        className: 'w-[250px]'
      }
    },
  ]

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-5 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <Form 
          form={form} 
          fields={searchFields} 
          className='flex gap-x-5 w-[600px]' 
          onFinish={handleSearch}
          noLayoutConfig={true}
          wrapperCol={{flex: 1}}
        >
          <div>
            <Button 
              htmlType='submit'
              className='flex items-center' 
              icon={<SearchOutlined/>}
            >
              Search
            </Button>
          </div>
        </Form>
      </div>
      <Table
        ref={dataGrid}
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        tableClass='max-h-[calc(100vh-210px)]'
      />
    </div>
  )
}

export default ImpersonateUser