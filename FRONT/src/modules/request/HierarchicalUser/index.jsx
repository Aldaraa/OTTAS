import React, { useContext, useEffect, useState } from 'react'
import { Form, Table, Button, Modal, PeopleSearch } from 'components';
import axios from 'axios';
import { Drawer, Input, Select } from 'antd';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from 'contexts';
import { Popup } from 'devextreme-react';
import { SearchOutlined } from '@ant-design/icons';
import tableSearch from 'utils/TableSearch';

function HierarchicalUser({...restProps}) {
  const [ showChoosingModal, setShowChoosingModal ] = useState(false)
  const [ selectedPerson, setSelectedPerson ] = useState(null)
  const [ selectedLineManager, setSelectedLineManager ] = useState(null)
  const [ linemanagers, setLineManagers ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ openPeopleSearch, setOpenPeopleSearch ] = useState(false)
  const [ data, setData ] = useState([])
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ renderData, setRenderData ] = useState([])


  const navigate = useNavigate()
  const { action } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    action.changeMenuKey('/request/hierarchicaluser')
    getLineManagersData()
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: '/tas/requestlinemanageremployee',
    }).then((res) => {
      setData(res.data)
      setRenderData(res.data)
    }).catch((err) => {
      
    }).then(() => setLoading(false))
  }

  const getLineManagersData = () => {
    axios({
      method: 'get',
      url: `/tas/requestgroupemployee/linemanageremployees`,
    }).then((res) => {
      let tmp = []
      res.data.Employees?.map((emp) => {
        tmp.push({value: emp.EmployeeId, label: emp.FullName})
      })
      setLineManagers(tmp)
    })
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const columns = [
    {
      label: '#',
      name: 'EmployeeId',
      dataType: 'string',
      width: '80px',
      cellRender: (e) => (
        <div className='flex items-center text-blue-500 hover:underline'>
          <Link to={`/tas/people/search/${e.value}`} target='_blank'>
            {e.value}
          </Link>
        </div>
      )
    },
    {
      label: 'Employee Name',
      name: 'EmployeeFullName',
      dataType: 'string',
    },
    {
      label: '#',
      name: 'LineManagerEmployeeId',
      dataType: 'string',
      width: '100px',
      cellRender: (e) => (
        <div className='flex items-center text-blue-500 hover:underline'>
          <Link to={`/tas/people/search/${e.value}`} target='_blank'>
            {e.value}
          </Link>
        </div>
      )
    },
    {
      label: 'Line Manager Name',
      name: 'LineManagerFullName',
      dataType: 'string',
    },
    {
      label: '',
      name: 'action',
      dataType: 'string',
      width: 100,
      cellRender: (e) => (
        <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
      )
    },
  ]

  const handleCancel = () => {
    setSelectedPerson(null)
    setShowChoosingModal(false)
  }

  const handleSaveChangingLineManager = () => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: '/tas/requestlinemanageremployee',
      data: {
        employeeId: selectedPerson.Id,
        lineManagerEmployeeId: selectedLineManager,
      }
    }).then((res) => {
      getData()
      setSelectedPerson(null)
      setSelectedLineManager(null)
      setShowChoosingModal(false)
      setOpenPeopleSearch(false)
    }).catch((err) => {
      
    }).then(() => setActionLoading(false))
  }

  const handleSelect = (data) => {
    setSelectedPerson(data)
    setShowChoosingModal(true)
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/requestlinemanageremployee/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const searchFields = [
    {
      label: 'Employee Name',
      name: 'EmployeeFullName',
      className: 'col-span-6 mb-0 ',
      inputprops: {
        className: 'w-[200px]'
      }
    },
    {
      label: 'Line Manager Name',
      name: 'LineManagerFullName',
      className: 'col-span-6 mb-0',
      inputprops: {
        className: 'w-[200px]'
      }
    },
  ]

  const handleSearch = (values) => {
    setLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setLoading(false)
    })
  }

  return (
    <>
      <div className='rounded-ot bg-white px-3 py-2 mb-5'>
        <div className='text-lg font-bold mb-2'>Hierarchical User</div>
        <Form 
          form={form} 
          fields={searchFields} 
          className='flex gap-x-5' 
          onFinish={handleSearch}
          wrapperCol={{flex: 1}}
          noLayoutConfig={true}
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
        columns={columns}
        data={renderData}
        keyExpr={'Id'}
        pager={renderData.length > 20}
        loading={loading}
        containerClass='shadow-none px-0 border rounded-ot overflow-hidden'
        title={<div className='flex justify-end items-center border-b py-1 px-2'>
          <Button onClick={() => setOpenPeopleSearch(true)}>Add</Button>
        </div>}
      />
      <Drawer open={openPeopleSearch} width={1000} onClose={() => setOpenPeopleSearch(false)}>
        <PeopleSearch
          onSelect={handleSelect} 
          onRowDblClick={(e) => navigate(`/tas/people/search/${e.data.Id}`)}
          defaultColumns={['Id', 'Active', 'Lastname', 'Firstname', 'DepartmentName', 'EmployerName', 'RosterName']}
        />
      </Drawer>
      <Modal open={showChoosingModal} title={`Line Manager on ${selectedPerson?.Firstname} ${selectedPerson?.Lastname}`}>
        <label>Selected Person:</label>
        <Input value={`${selectedPerson?.Firstname} ${selectedPerson?.Lastname}`} disabled className='mb-4'/>
        <label>Line Manager:</label>
        <Select 
          options={linemanagers} 
          onChange={(e) => setSelectedLineManager(e)} 
          placeholder='Select Line Manager' 
          className='w-full'
          allowClear
          showSearch
          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
        />
        <div className='flex justify-end gap-4 mt-4'>
          <Button 
            type={'primary'}
            disabled={!selectedLineManager}
            onClick={handleSaveChangingLineManager}
            loading={actionLoading}
          >
            Save
          </Button>
          <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
        </div>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </>
  )
}

export default HierarchicalUser