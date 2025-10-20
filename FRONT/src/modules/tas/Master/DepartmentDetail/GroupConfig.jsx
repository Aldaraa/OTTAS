import { Select } from 'antd'
import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useCallback, useContext, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

const title = 'Admin'

function GroupConfig({data, getData, departmentData}) {
  const [ admins, setAdmins ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ headLoading, setHeadLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ showPrimaryPopup, setShowPrimaryPopup ] = useState(false)
  const [ editData , setEditData ] = useState(null)
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ form ] = Form.useForm()
  const { state } = useContext(AuthContext)

  const columns = [
    {
      label: 'Employer',
      name: 'EmployerName',
    },
    {
      label: 'Group Master',
      name: 'GroupMasterName',
      groupIndex: 0,
    },
    {
      label: 'Option',
      name: 'GroupDetailName',
    },
  ]

  const fields = [
    {
      label: 'Employer',
      name: 'EmployerIds',
      className: 'col-span-12 mb-3',
      type: 'select',
      inputprops: {
        options: state.referData.employers,
        loading: headLoading,
        mode: 'multiple',
      }
    },
    {
      label: 'Group',
      name: 'GroupMasterId',
      className: 'col-span-12 mb-3',
      type: 'select',
      inputprops: {
        options: state.referData.fieldsOfGroups,
        fieldNames: {value: 'Id', label: 'Description'},
        loading: headLoading,
        onChange: () => form.setFieldValue('GroupDetailId', null)
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, next) => prev.GroupMasterId !== next.GroupMasterId}>
        {({getFieldValue, setFieldValue}) => {
          const selectedGroupMasterId = getFieldValue('GroupMasterId')
          const options = state.referData.fieldsOfGroups.find((group) => group.Id === selectedGroupMasterId)
          return(
            <Form.Item name='GroupDetailId' label='Option' className='col-span-12 mb-3'>
              <Select options={options?.details || []} fieldNames={{value: 'Id', label: 'Description'}}/>
            </Form.Item>
          )
        }}
      </Form.Item>
    }
  ]

  const handleSubmitAdd = useCallback((values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/DepartmentGroupConfig',
      data: {
        ...values,
        departmentId: departmentData?.Id
      }
    }).then(() => {
      getData()
      setShowModal(false)
    }).catch(() => {

    }).then(() => setLoading(false))
  },[departmentData])

  const handleDelete = useCallback(() => {
    setLoading(true)
    axios({
      method: 'delete',
      url: `tas/DepartmentGroupConfig`,
      data: {
        ids: selectedRowKeys
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch(() => {

    }).then(() => setLoading(false))
  },[selectedRowKeys])

  const handleSelect = useCallback((e) => {
    setSelectedRowKeys(e.selectedRowKeys)
  },[])

  const handleClickRemove = useCallback(() => {
    setShowPopup(true)
  },[])

  return (
    <div>
      <Table
        keyExpr='Id'
        data={data}
        columns={columns}
        containerClass='px-0 shadow-none rounded-none border-none'
        selection={{mode: 'multiple', recursive: false, showCheckBoxesMode: 'always', selectAllMode: 'page'}}
        selectedRowKeys={selectedRowKeys}
        onSelectionChanged={handleSelect}
        isGrouping={true}
        title={<div className='border-b py-1 flex justify-between'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{data?.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <div className='flex items-center gap-3'>
            <Button className='text-xs' type='danger' onClick={handleClickRemove} disabled={selectedRowKeys.length === 0}>
              Remove
            </Button>
            <Button className='text-xs' onClick={() =>  setShowModal(true)}>
              Add
            </Button>
          </div>
        </div>}
      />
      <Modal 
        title={`Add Configuration`} 
        open={showModal} 
        onCancel={() => setShowModal(false)} 
        destroyOnClose={true}
        width={600}
      >
        <Form
          form={form}
          fields={fields}
          onFinish={handleSubmitAdd} 
          labelCol={{flex: '90px'}}
        />
        <div className='flex gap-5 justify-end'>
          <Button type={'primary'} onClick={() => form.submit()} loading={loading}>Save</Button>
          <Button onClick={() => setShowModal(false)}>
            Cancel
          </Button>
        </div>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={'auto'}
        width={390}
      >
        <div className='text-center'>Are you sure you want to remove this <span className='font-bold text-blue-500'>{selectedRowKeys.length}</span> record(s)?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={loading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
    </div>
  )
}

export default GroupConfig