import { Tooltip, Form, Table, Button } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm, Tag } from 'antd'
import axios from 'axios'
import { SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'  
import { Link, useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { twMerge } from 'tailwind-merge'

const title = 'Impersonate User'

const dayColor = (day) => {
  let color = ''
  if(day < 0){
    color = 'bg-orange-500 text-white'
  }else if(day === 0 || day === 1){
    color = 'bg-red-500 text-white'
  }else if(day === 2 || day === 3){
    color = 'bg-yellow-500 text-white'
  }
  return color
}

function ImpersonateUserTasks() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ currentMode, setCurrentMode ] = useState('search')

  const [ form ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  const { impersoniteId } = useParams()
  const navigate = useNavigate()

  useEffect(() => {
    action.changeMenuKey('/request/impersonateuser')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocument/documentlistinpersonate/${impersoniteId}`
    }).then((res) => {
      setData(res.data)
      // // if(form.getFieldValue('Active')){
      //   let tmp = res.data.filter((item) => item.Active === 1)
      //   setRenderData(tmp)
      // }else{
        setRenderData(res.data)
      // }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const toLink = (e) => {
    if(e.data.DocumentType === 'Non Site Travel'){
      return `/request/task/nonsitetravel/${e.data.Id}?impersonateuser=${impersoniteId}`
    }
    else if(e.data.DocumentType === 'Profile Changes'){
      return `/request/task/profilechanges/${e.data.Id}?impersonateuser=${impersoniteId}`
    }
    else if(e.data.DocumentType === 'De Mobilisation'){
      return `/request/task/de-mobilisation/${e.data.Id}?impersonateuser=${impersoniteId}`
    }
    else if(e.data.DocumentType === 'Site Travel'){
      if(e.data.DocumentTag === 'ADD'){
        return `/request/task/sitetravel/addtravel/${e.data.Id}?impersonateuser=${impersoniteId}`
      }
      else if(e.data.DocumentTag === "RESCHEDULE"){
        return `/request/task/sitetravel/reschedule/${e.data.Id}?impersonateuser=${impersoniteId}`
      }
      else if(e.data.DocumentTag === "REMOVE"){
        return `/request/task/sitetravel/remove/${e.data.Id}?impersonateuser=${impersoniteId}`
      }
    }
  }

  const columns = [
    {
      label: '',
      name: 'DaysAway',
      width: '45px',
      alignment: 'left',
      cellRender: ({value}) => (
        <Tooltip title='Days Away'>
          <div className={twMerge('py-[1px] px-[3px] rounded inline-block text-xs min-w-[30px] text-center', dayColor(value))}>
            {value}
          </div>
        </Tooltip>
      )
    },
    {
      label: '#',
      name: 'Id',
      dataType: 'string',
      width: '65px',
      // alignment: 'center',
      cellRender: (e) => (
        <div className='flex items-center text-blue-500 hover:underline'>
          <Link to={toLink(e)} target='_blank'>
            {e.value}
          </Link>
        </div>
      )
    },
    {
      label: 'CurrentStatus',
      name: 'CurrentStatusGroup',
      alignment: 'left',
      // width: '110px'
    },
    {
      label: 'Document',
      name: 'DocumentType',
      alignment: 'left',
      width: '120px',
    },
    // {
    //   label: 'Description',
    //   name: 'Description',
    //   alignment: 'left',
    // },
    {
      label: 'Employer',
      name: 'EmployerName',
      alignment: 'left',
    },
    {
      label: 'Requester',
      name: 'RequesterFullName',
      alignment: 'left',
    },
    {
      label: 'Assigned To',
      name: 'AssignedEmployeeFullName',
      alignment: 'left',
    },
    {
      label: 'Date Requested',
      name: 'RequestedDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
      )
    },
    {
      label: 'Subject',
      name: 'EmployeeFullName',
      alignment: 'left',
    },
    {
      label: 'Updated',
      name: 'UpdateInfo',
      alignment: 'left',
    },
    {
      label: '',
      name: '',
      width: '90px',
      alignment: 'center',
      showInColumnChooser: false,
      cellRender: (e) => (
        <div className='flex items-center'>
          <Link to={toLink(e)}>
            <button type='button' className='edit-button'>View</button>
          </Link>
        </div>
      )
    },
  ]

  const handleSearch = async (values) => {
    setSearchLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setSearchLoading(false)
    })
  }

  const searchFields = [
    {
      label: 'First Name',
      name: 'FirstName',
      className: 'col-span-12 mb-2',
      inputprops: {
      }
    },
    {
      label: 'Last Name',
      name: 'Lastname',
      className: 'col-span-12 mb-2',
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

  return (
    <div>
      <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
        <div className='text-lg font-bold mb-3'>{title}</div>
        <div className='w-[400px]'>
          <Form
            form={form}
            fields={searchFields}
            size='small'
            className='grid grid-cols-12 gap-x-8'
            onFinish={handleSearch}
            wrapperCol={{flex: 1}}
            labelCol={{flex: '100px'}}
          >
            <div className='flex gap-4 justify-end col-span-12'>
              <Button 
                htmlType='submit' 
                // onClick={(e) => e.stopPropagation()} 
                className='flex items-center' 
                loading={searchLoading} 
                icon={<SearchOutlined/>}
              >
                Search
              </Button>
            </div>
          </Form>
        </div>
      </div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        pager={renderData.length > 20}
        onRowDblClick={(e) => navigate(toLink(e))}
        tableClass='max-h-[calc(100vh-325px)]'
        
      />
    </div>
  )
}

export default ImpersonateUserTasks