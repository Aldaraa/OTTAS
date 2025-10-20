import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons';
import axios from 'axios';
import { Button, Form, Modal, Table } from 'components';
import { AuthContext } from 'contexts';
import { CheckBox, Popup, Tooltip } from 'devextreme-react';
import React, { useContext, useEffect, useState } from 'react'
import tableSearch from 'utils/TableSearch';

const title = 'Role'

const initValues = {
  User: '',
  Active: true,
}
function Administratort() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ showModal, setShowModal ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)

  const [ form ] = Form.useForm()
  const [ searchForm ] = Form.useForm()
  const { action, state } = useContext(AuthContext)

  useEffect(() => {
    action.changeMenuKey('/report/employer')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/employer'
    }).then((res) => {
      setData(res.data)
      if(searchForm.getFieldValue('Active') === 1){
        let tmp = res.data.filter((item) => item.Active === 1)
        setRenderData(tmp)
      }else if(searchForm.getFieldValue('Active') === 0){
        let tmp = res.data.filter((item) => item.Active === 0)
        setRenderData(tmp)
      }
      else{
        setRenderData(res.data)
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  } 

  const handleSubmit = (values) => {
    if(editData){
      axios({
        method: 'put',
        url:'tas/employer',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
        
      })
    }
    else{
      axios({
        method: 'post',
        url:'tas/employer',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
        
      })
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/employer`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const fields = [
    {
      label: 'Username',
      name: 'Username',
      className: 'col-span-12 mb-2',
      inputprops: {
      }
    },
    {
      label: 'Email',
      name: 'Email',
      className: 'col-span-12 mb-2',
      rules: [{type: 'email', message: `The input is not valid E-mail!`}],
    },
    {
      label: 'Password',
      name: 'Password',
      className: 'col-span-12 mb-2',
      type: 'password',
      inputprops: {

      }
    },
    {
      label: 'Confirm Password',
      name: 'ConfirmPassword',
      className: 'col-span-12 mb-2',
      type: 'password',
      inputprops: {

      }
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 mb-2',
      type: 'check',
    },
    {
      label: 'Role',
      name: 'Role',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: []
      }
    },
    {
      label: 'Start Page',
      name: 'StartPage',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: []
      }
    },
    {
      label: 'Force Change Password',
      name: 'ForceChangePassword',
      className: 'col-span-12 mb-2',
      type: 'check',
    },
    {
      label: 'Send Confirmation email',
      name: 'SendConfirmationEmail',
      className: 'col-span-12 mb-2',
      type: 'check',
    },
  ]

  const handleAddButton = () => {
    setEditData(null)
    form.resetFields()
    setShowModal(true)
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
      label: 'User',
      name: 'user',
    },
    {
      label: 'Role',
      name: 'Role',
      width: 100,
    },
    {
      label: 'Email',
      name: 'Email',
    },
    {
      label: 'Reports',
      name: 'Reports',
      alignment: 'left',
      width: 80
    },
    {
      label: 'Active',
      name: 'Active',
      width: '80px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Active === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '170px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          {
            e.data.Active === 1 ?
            e.data.EmployeeCount > 0 ? 
            <div>
              <div id={`target${e.data.Id}`} className="dlt-button_lbl">Deactivate</div> 
                  <Tooltip
                    target={`#target${e.data.Id}`}
                    showEvent="mouseenter"
                    hideEvent="mouseleave"
                    hideOnOutsideClick={false}
                    position="top"
                  >
                    <div>Don't deactivate because there is an active Employee</div>
                </Tooltip> 
            </div>:
            <button type='button' disabled={e.data.EmployeeCount > 0} className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Deactivate</button>
            :
            <button type='button' className='scs-button' onClick={() => handleDeleteButton(e.data)}>Reactivate</button>
          }
        </div>
      )
    },
  ]

  const searchFields = [
    {
      label: 'User',
      name: 'User',
      className: 'col-span-4 mb-2',
      inputprops: {
        placeholder: 'User search...'
      }
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
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

  const handleCancel = () => {
    setShowModal(false)
  }

  return (
    <div>
      <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
        <div className='text-lg font-bold mb-3'>{title}</div>
        <Form 
          form={searchForm} 
          fields={searchFields}
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSearch}
          noLayoutConfig={true}
          initValues={initValues}
        >
          <div className='flex gap-4 col-span-2 items-baseline'>
            <Button htmlType='submit' className='flex items-center' loading={loading} icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        tableClass='border-t'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{renderData.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add {title}</Button>
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
            <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to {editData?.Active === 1 ? 'deactivate' : 'reactivate'} this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button 
            type={editData?.Active === 1 ? 'danger' : 'success'} 
            onClick={handleDelete}
            loading={actionLoading}
          >
            Yes
          </Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>
            No
          </Button>
        </div>
      </Popup>
    </div>
  )
}

export default Administratort