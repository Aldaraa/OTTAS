import { Form, Table, Button, Modal } from 'components'
import { CheckBox } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { SaveOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'

const title = 'Profile Field'

function ProfileField() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)

  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)

  useEffect(() => {
    action.changeMenuKey('/tas/profilefield')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/profilefield'
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
  
  const handleEditButton = (dataItem) => {
    setEditData(dataItem)
    setShowModal(true)
  }

  const columns = [
    {
      label: 'Label',
      name: 'Label',
      sort: 'asc',
    },
    {
      label: 'Column Name',
      name: 'ColumnName',
    },
    {
      label: 'Required',
      name: 'FieldRequired',
      alignment: 'left',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    // {
    //   label: 'Visible',
    //   name: 'FieldVisible',
    //   alignment: 'left',
    //   cellRender: (e) => (
    //     <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    //   )
    // },
    // {
    //   label: 'Read Only',
    //   name: 'FieldReadOnly',
    //   alignment: 'left',
    //   cellRender: (e) => (
    //     <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    //   )
    // },
    // {
    //   label: 'Request Required',
    //   name: 'RequestRequired',
    //   alignment: 'left',
    //   cellRender: (e) => (
    //     <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    //   )
    // },
    // {
    //   label: 'Request Visible',
    //   name: 'RequestVisible',
    //   alignment: 'left',
    //   cellRender: (e) => (
    //     <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    //   )
    // },
    {
      label: '',
      name: 'action',
      width: 80,
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'put',
      url:'tas/profilefield',
      data: {
        ...values,
        FieldVisible: 0,
        FieldReadOnly: 0,
        RequestRequired: 0,
        RequestVisible: 0,
        Id: editData.Id
      }
    }).then((res) => {
      getData()
      handleCancel()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const fields = [
    {
      label: 'Label',
      name: 'Label',
      rules: [{required: true, message: 'Code is required'}],
      className: 'col-span-12 mb-2',
    },
    {
      label: 'Column Name',
      name: 'ColumnName',
      rules: [{required: true, message: 'Code is required'}],
      className: 'col-span-12 mb-2',
      inputprops: {
        disabled: true
      }
    },
    {
      label: 'Required',
      name: 'FieldRequired',
      className: 'col-span-12 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    // {
    //   label: 'Visible',
    //   name: 'FieldVisible',
    //   className: 'col-span-12 mb-2',
    //   type: 'check',
    //   inputprops: {
    //   }
    // },
    // {
    //   label: 'Read Only',
    //   name: 'FieldReadOnly',
    //   className: 'col-span-12 mb-2',
    //   type: 'check',
    //   inputprops: {
    //   }
    // },
    // {
    //   label: 'Request Required',
    //   name: 'RequestRequired',
    //   className: 'col-span-12 mb-2',
    //   type: 'check',
    //   inputprops: {
    //   }
    // },
    // {
    //   label: 'Request Visible',
    //   name: 'RequestVisible',
    //   className: 'col-span-12 mb-2',
    //   type: 'check',
    //   inputprops: {
    //   }
    // },
  ]

  const handleCancel = () => {
    setShowModal(false)
  }

  return (
    <div>
      <Table
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        pager={false}
        tableClass='border-t max-h-[calc(100vh-135px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{renderData.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
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
            <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
    </div>
  )
}

export default ProfileField