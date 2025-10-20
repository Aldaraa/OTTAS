import { Form, Table, Button, Modal } from 'components'
import { CheckBox, Popup, Tooltip } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import tableSearch from 'utils/TableSearch'
import { Link, useNavigate } from 'react-router-dom'
import { AuthContext } from 'contexts'

const title = 'Group Master'

function GroupMaster() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const navigate = useNavigate()
  const { action } = useContext(AuthContext)
  
  useEffect(() => {
    action.changeMenuKey('/tas/groupmaster')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/groupmaster'
    }).then((res) => {
      setData(res.data)
      if(searchForm.getFieldValue('Active') === 1){
        let tmp = res.data.filter((item) => item.Active === 1)
        setRenderData(tmp)
      }else if(searchForm.getFieldValue('Active') === 0){
        let tmp = res.data.filter((item) => item.Active === 0)
        setRenderData(tmp)
      }else{
        setRenderData(res.data)
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }
  
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
      label: 'Description',
      name: 'Description'
    },
    {
      label: 'Order',
      name: 'OrderBy',
      alignment: 'left',
    },
    {
      label: '# Detail Count',
      name: 'DetailCount',
      alignment: 'left',
    },
    {
      label: 'Log',
      name: 'CreateLog',
      width: '120px',
      alignment: 'center',
      cellRender: ({value}) => (
        <CheckBox disabled iconSize={18} value={value === 1 ? true : 0}/>
      )
    },
    {
      label: 'Show on Profile',
      name: 'ShowOnProfile',
      width: '120px',
      alignment: 'center',
      cellRender: ({value}) => (
        <CheckBox disabled iconSize={18} value={value === 1 ? true : 0}/>
      )
    },
    {
      label: 'Required',
      name: 'Required',
      width: '120px',
      alignment: 'center',
      cellRender: ({value}) => (
        <CheckBox disabled iconSize={18} value={value === 1 ? true : 0}/>
      )
    },
    {
      label: 'Active',
      name: 'Active',
      width: '90px',
      alignment: 'center',
      cellRender: ({value}) => (
        <CheckBox disabled iconSize={18} value={value === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: '',
      width: '80px',
      cellRender: (e) => (
        <div className='flex items-center'>
          <Link to={`/tas/groupmaster/${e.data.Id}`}>
            <button type='button' className='edit-button'>View</button>
          </Link>
        </div>
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
          e.data.DetailCount > 0 ? 
            <div>
              <div id={`target${e.data.Id}`} className="dlt-button_lbl">Deactivate</div> 
                  <Tooltip
                    target={`#target${e.data.Id}`}
                    showEvent="mouseenter"
                    hideEvent="mouseleave"
                    hideOnOutsideClick={false}
                    position="top"
                  >
                    <div>Don't deactivate because there is an active Detail Count</div>
                </Tooltip> 
            </div>:
          <button type='button' disabled={e.data.DetailCount > 0} className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Deactivate</button>
          :
          <button type='button' className='scs-button' onClick={() => handleDeleteButton(e.data)}>Reactivate</button>
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
        url:'tas/groupmaster',
        data: {
          ...values,
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/groupmaster',
        data: {
          ...values,
          Active: values.Active ? 1 : 0,
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/groupmaster`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = (values) => {
    setLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setLoading(false)
    })
  }

  const fields = [
    {
      label: 'Description',
      name: 'Description',
      rules: [{required: true, message: 'Description is required'}],
      className: 'col-span-12 mb-2'
    },
    {
      label: 'Order',
      name: 'OrderBy',
      rules: [{required: true, message: 'Order is required'}],
      type: 'number',
      className: 'col-span-12 mb-2'
    },
    {
      label: 'Show on Profile',
      name: 'ShowOnProfile',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Create Log',
      name: 'CreateLog',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Required',
      name: 'Required',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
  ]

  const addFields = [
    {
      label: 'Description',
      name: 'Description',
      rules: [{required: true, message: 'Description is required'}],
      className: 'col-span-12 mb-2'
    },
    {
      label: 'Order',
      name: 'OrderBy',
      rules: [{required: true, message: 'Order is required'}],
      type: 'number',
      className: 'col-span-12 mb-2'
    },
    {
      label: 'Show on Profile',
      name: 'ShowOnProfile',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Create Log',
      name: 'CreateLog',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Required',
      name: 'Required',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
  ]

  const searchFields = [
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-3 mb-2'
    },
    {
      label: 'Order',
      name: 'OrderBy',
      type: 'number',
      className: 'col-span-2 mb-2',
      inputprops: {
        className : 'w-full'
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
    {
      label: 'Show on Profile',
      name: 'ShowOnProfile',
      className: 'col-span-2 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

  const handleCancel = () => {
    setShowModal(false)
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <Form 
          form={searchForm} 
          fields={searchFields} 
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSearch}
          noLayoutConfig={true}
        >
          <div className='flex gap-4 items-baseline col-span-1'>
            <Button htmlType='submit' className='flex items-baseline' loading={loading} icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        onRowDblClick={(e) => navigate(`/tas/groupmaster/${e.data.Id}`)}
        tableClass='border-t max-h-[calc(100vh-250px)]'
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
          fields={editData ? fields : addFields}
          editData={editData} 
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
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
          <Button type={editData?.Active === 1 ? 'danger' : 'success'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default GroupMaster