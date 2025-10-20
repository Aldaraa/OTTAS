import { Form, Table, Button, Modal } from 'components'
import { Popup } from 'devextreme-react'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { Select } from 'antd'
import axios from 'axios'
import { QuestionCircleOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'  
import dayjs from 'dayjs'
import { Link } from 'react-router-dom'

const title = 'Delegation'
const initValues = {
  Code: '',
  Number: '',
  Description: '',
  Active: 1
}

function DelegationConfig() {
  const [ data, setData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ employees, setEmployees ] = useState([])
  const [ showModal, setShowModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)

  const [ form ] = Form.useForm()
  const { action, state } = useContext(AuthContext)
  const dataGrid = useRef(null)

  useEffect(() => {
    action.changeMenuKey('/tas/costcode')
    getData()
    getEmployees()
  },[action]) // eslint-disable-line

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/requestdelegate'
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getEmployees = () => {
    axios({
      method: 'get',
      url: 'tas/requestgroupemployee/linemanageradminemployees',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.EmployeeId, label: `#${item.EmployeeId} ${item.FullName}`})
      })
      setEmployees(tmp)
    }).catch((err) => {

    })
  }

  const handleEditButton = (dataItem) => {
    setEditData(dataItem)
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const columns = [
    {
      label: 'From Date',
      name: 'StartDate',
      cellRender: ({value}) => (
        <div>{value ? dayjs(value).format('YYYY-MM-DD') : null}</div>
      )
    },
    {
      label: 'To Date',
      name: 'EndDate',
      cellRender: ({value}) => (
        <div>{value ? dayjs(value).format('YYYY-MM-DD') : null}</div>
      )
    },
    {
      label: '#',
      name: 'fromEmployeeId',
      dataType: 'string',
      width: 80,
      cellRender: (e) => (
        <div className='flex items-center text-blue-500 hover:underline'>
          <Link to={`/tas/people/search/${e.value}`} target='_blank'>
            {e.value}
          </Link>
        </div>
      )
    },
    {
      label: 'Delegate From',
      name: 'fromEmployeeFullname'
    },
    {
      label: '#',
      name: 'toEmployeeId',
      dataType: 'string',
      width: 80,
      cellRender: (e) => (
        <div className='flex items-center text-blue-500 hover:underline'>
          <Link to={`/tas/people/search/${e.value}`} target='_blank'>
            {e.value}
          </Link>
        </div>
      )
    },
    {
      label: 'Delegate To',
      name: 'toEmployeeFullname',
      alignment: 'left',
    },
    {
      label: '',
      name: 'action',
      width: '170px',
      cellRender: (e) => (
        <div className='flex justify-end gap-4'>
          {
            !(state.userInfo.Role === 'DepartmentAdmin' || state.userInfo.Role === 'DepartmentManager') ? 
            <>
              <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
              <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Remove</button>
            </>
            : 
            null
          }
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    if(editData){
      axios({
        method: 'put',
        url:'tas/requestdelegate',
        data: {
          ...values,
          StartDate: dayjs(values.StartDate).format('YYYY-MM-DD'),
          EndDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
          Active: values.Active ? 1 : 0,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        form.resetFields()
        setEditData(null)
        form.setFieldsValue(initValues)
        setShowModal(false)
      }).catch((err) => {
        
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/requestdelegate',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
          StartDate: dayjs(values.StartDate).format('YYYY-MM-DD'),
          EndDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
        }
      }).then((res) => {
        getData()
        form.resetFields()
        form.setFieldsValue(initValues)
        setShowModal(false)
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/requestdelegate/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = async (values) => {
    let keys = Object.keys(values)
    let searchvalues = [] 
    keys.map((key) => {
      if(values[key]){
        if(key.includes('Date')){
          searchvalues.push(`${key}=${dayjs(values[key]).format('YYYY-MM-DD')}`)
        }else{
          searchvalues.push(`${key}=${values[key]}`)
        }
      }
    })
    let params = ''
    searchvalues.map((item, i) => {
      if(i === 0){
        params += `?${item}`
      }else{
        params += `&${item}`
      }
    })
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdelegate${params}`
    }).then((res) => {
      setData(res.data)
    }).then(() => {
      setSearchLoading(false)
    })
  }

  const fields = [
    {
      type: 'component',
      component: 
      <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.toEmployeeId !== curValues.toEmployeeId}>
        {({getFieldValue}) => {
          return(
            <Form.Item 
              name='fromEmployeeId' 
              label='Delegate From' 
              rules={[{required: true, message: 'Delegate From is required'}]}
              className='col-span-12 mb-2'
            >
              <Select
                showSearch
                allowClear
                disabled={(state.userInfo.Role === 'DepartmentAdmin' || state.userInfo.Role === 'DepartmentManager') && !editData}
                options={employees.filter((item) => item.value !== getFieldValue('toEmployeeId'))}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: 
      <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromEmployeeId !== curValues.fromEmployeeId}>
        {({getFieldValue}) => {
          return(
            <Form.Item 
              name='toEmployeeId' 
              label='Delegate To' 
              rules={[{required: true, message: 'Delegate To is required'}]}
              className='col-span-12 mb-2'
            >
              <Select
                showSearch
                allowClear
                options={employees.filter((item) => item.value !== getFieldValue('fromEmployeeId'))}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-12 mb-2',
      type: 'date',
      rules: [{required: true, message: 'Start Date is required'}],
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-12 mb-2',
      type: 'date',
      rules: [{required: true, message: 'End Date is required'}],
    },
  ]

  const searchFields = [
    {
      label: 'Delegate From',
      name: 'fromEmployeeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: employees,
      }
    },
    {
      label: 'Delegate To',
      name: 'toEmployeeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: employees,
      }
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-6 mb-2',
      type: 'date',
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-6 mb-2',
      type: 'date',
    },
  ]

  const handleAdd = () => {
    setEditData(null)
    if((state.userInfo.Role === 'DepartmentAdmin' || state.userInfo.Role === 'DepartmentManager')){
      form.setFieldValue('fromEmployeeId', state.userInfo.Id)
    }
    setShowModal(true)
  }

  return (
    <div>
      <div className='rounded-ot bg-white py-2 px-3 mb-5 shadow-md'>
        <div className='text-lg font-bold mb-3'>{title}</div>
        <div className='w-[750px]'>
          <Form 
            fields={searchFields} 
            size='small' 
            className='grid grid-cols-12 gap-x-5' 
            onFinish={handleSearch}
            labelCol={{flex: '95px'}}
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
        ref={dataGrid}
        data={data}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        title={<div className='flex justify-between py-2 px-1 border-b'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{data.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <Button onClick={handleAdd}>Add</Button>
        </div>}
      />
        <Modal open={showModal} onCancel={() => setShowModal(false)} title={editData ? `Edit ${title}` : `Add ${title}`}>
        <Form 
          form={form}
          fields={fields}
          editData={editData} 
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button
              type='primary' 
              onClick={() => form.submit()} 
              loading={actionLoading} 
              icon={<SaveOutlined/>}
            >
              Save
            </Button>
            <Button onClick={() => setShowModal(false)} disabled={actionLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height='auto'
        width='auto'
      >
        <div className='flex items-center gap-3'>
          <QuestionCircleOutlined style={{fontSize: '24px', color: '#F64E60'}}/>
          <div>Are you sure you want to remove this record?</div>
        </div>
        <div className='flex gap-5 mt-3 justify-center'>
          <Button type='danger' onClick={handleDelete}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default DelegationConfig