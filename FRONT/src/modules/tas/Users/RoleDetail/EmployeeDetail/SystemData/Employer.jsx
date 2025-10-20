import { PlusOutlined } from '@ant-design/icons'
import { Empty } from 'antd'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'
import React, { useCallback, useContext, useEffect, useState } from 'react'

const title = 'Employer'

function Employer({employeeData}) {
  const [ data, setData ] = useState([])
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editData , setEditData ] = useState(null)
  const [ form ] = Form.useForm()
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(employeeData){
      getData()
    }
  },[employeeData])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employeradmin/${employeeData?.EmployeeId}`,
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    })
  }

  const handleClickRemove = useCallback((row) => {
    setEditData(row)
    setShowPopup(true)
  },[])

  const fields = [
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-12 mb-0',
      type: 'select',
      rules: [{required: true, message: 'Employer is required'}],
      inputprops: {
        options: state.referData.employers,
        allowClear: true,
        showSearch: true,
        popupMatchSelectWidth: false,
      }
    },
  ]

  const handleSubmitAdd = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/employeradmin',
      data: {
        ...values,
        employeeId: employeeData?.EmployeeId
      }
    }).then(() => {
      getData()
      setShowModal(false)
    }).catch(() => {

    }).then(() => setActionLoading(false))
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/employeradmin/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch(() => {

    }).then(() => setActionLoading(false))
  }

  return (
    <div className='mt-3'>
      <div className='flex justify-between items-center border-b pb-4'>
        <div className='text-base font-medium'>Empoyers <span className='ml-2 text-gray-400'>{data.length}</span></div>
        <Button className='text-xs' icon={<PlusOutlined/>} onClick={() => setShowModal(true)}>Add</Button>
      </div>
      <div className='flex flex-col divide-y max-h-[calc(calc(100vh-240px)/2)] overflow-y-auto border-b'>
        {
          data.length > 0 ?
          data.map((item, i) => (
            <div className='flex justify-between py-2 text-xs items-center group relative hover:bg-sky-100' key={`manager-list-item-${i}`}>
              <div>{item.Name}</div>
              <button
                className='dlt-button text-xs py-1 hidden group-hover:block absolute right-2'
                onClick={() => handleClickRemove(item)}
              >
                Remove
              </button>
            </div>
          ))
          : 
          <Empty image={Empty.PRESENTED_IMAGE_SIMPLE}/>
        }
      </div>
      <Modal 
        title={`Add ${title}`} 
        open={showModal} 
        onCancel={() => setShowModal(false)}
        destroyOnClose={true}
      >
        <Form
          form={form}
          fields={fields}
          onFinish={handleSubmitAdd}
        />
        <div className='col-span-12 flex justify-end gap-4 mt-4'>
          <Button type={'success'} onClick={() => form.submit()} loading={actionLoading}>Save</Button>
          <Button onClick={() => setShowModal(false)}>Cancel</Button>
        </div>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to remove this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
    </div>
  )
}

export default Employer