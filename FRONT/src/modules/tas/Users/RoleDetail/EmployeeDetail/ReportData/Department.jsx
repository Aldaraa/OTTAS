import { PlusOutlined, QuestionCircleOutlined } from '@ant-design/icons'
import { Empty } from 'antd'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'
import React, { useCallback, useContext, useEffect, useState } from 'react'

function Department({employeeData, refreshData}) {
  const [ departmentData, setDepartmentData ] = useState([])
  const [ showModal, setShowModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ selectedData, setSelectedData ] = useState(null)

  const { state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    if(employeeData){
      getData()
    }
  },[employeeData])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/sysroleemployeereportdepartment/${employeeData?.EmployeeId}`
    }).then((res) => {
      setDepartmentData(res.data)
    })
  }

  const fields = [
    {
      label: 'Department',
      name: 'DepartmentId',
      className: 'col-span-12 mb-0',
      rules: [{required: true, message: 'Department is required'}],
      type: 'treeSelect',
      inputprops: {
        treeData: state.referData.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      }
    },
  ]

  const handleSubmitDepartment = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: `tas/sysroleemployeereportdepartment`,
      data: {
        ...values,
        EmployeeId: employeeData?.EmployeeId
      }
    }).then(() => {
      getData()
      refreshData()
      handleCancel()
    }).catch(() => {

    }).finally(() => setActionLoading(false))
  }

  const handleCancel = () => {
    setShowModal(false)
  }

  const handleDeleteButton = (row) => {
    setSelectedData(row)
    setShowPopup(true)
  }

  const handleDelete = useCallback(() => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/sysroleemployeereportdepartment/${selectedData.Id}`,
    }).then(() => {
      getData()
      refreshData()
      setShowPopup(false)
    }).catch(() => {

    }).finally(() => setActionLoading(false))
  }, [selectedData])

  return (
    <div>
      <div className='flex justify-between items-center border-b pb-4'>
        <div className='text-base font-medium'>Department <span className='ml-2 text-gray-400'>{departmentData.length}</span></div>
        <Button className='text-xs' icon={<PlusOutlined/>} onClick={() => setShowModal(true)}>Add</Button>
      </div>
      <div className='flex flex-col divide-y max-h-[calc(calc(100vh-240px)/2)] overflow-y-auto border-b'>
        {
          departmentData.length > 0 ?
          departmentData.map((item, i) => (
            <div className='flex justify-between py-2 text-xs items-center group relative hover:bg-sky-100' key={`department-list-item-${i}`}>
              <div>{item.Name}</div>
              <button
                className='dlt-button text-xs py-1 hidden group-hover:block absolute right-2'
                onClick={() => handleDeleteButton(item)}
              >
                Remove
              </button>
            </div>
          ))
          : 
          <Empty image={Empty.PRESENTED_IMAGE_SIMPLE}/>
        }
      </div>
      <Modal title='Add Department' open={showModal} onCancel={handleCancel}>
        <Form
          form={form}
          fields={fields}
          className='gap-4'
          onFinish={handleSubmitDepartment}
        >
          <div className='col-span-12 flex justify-end gap-4'>
            <Button type='success' onClick={form.submit} loading={actionLoading}>Save</Button>
            <Button onClick={handleCancel}>Cancel</Button>
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
          <div>Are you sure you want to remove <span className='font-medium'>{selectedData?.Name}</span> ?</div>
        </div>
        <div className='flex gap-5 mt-3 justify-center'>
          <Button type='danger' onClick={handleDelete}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default Department