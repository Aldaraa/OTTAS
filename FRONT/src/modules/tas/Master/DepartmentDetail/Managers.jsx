import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

const title = 'Manager'

function Managers({data, getData, departmentData}) {
  const [ managers, setManagers ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ headLoading, setHeadLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editData , setEditData ] = useState(null)
  const [ showPrimaryPopup, setShowPrimaryPopup ] = useState(false)
  const [ form ] = Form.useForm()

  useEffect(() => {
    setHeadLoading(true)
    axios({
      method: 'get',
      url: 'tas/department/managers',
    }).then((res) => {
      let tmp = res.data.map((item) => ({...item, value: item.EmployeeId, label: item.FullName}))
      setManagers(tmp)
    }).catch((err) => {

    }).then(() => setHeadLoading(false))
  },[])

  const handleClickRemove = useCallback((row) => {
    setEditData(row)
    setShowPopup(true)
  },[])

  const handleSetPrimary = useCallback((boolean, row) => {
    setEditData(row)
    setShowPrimaryPopup(boolean)
  },[])

  const columns = [
    {
      label: 'Person #',
      name: 'EmployeeId',
      alignment: 'left',
      width: 80,
    },
    {
      label: 'SAPID #',
      name: 'SAPID',
      alignment: 'left',
      width: 80,
    },
    {
      label: 'Fullname',
      name: 'FullName',
      cellRender: (e) => (
        <div className='text-blue-400 hover:underline'>
          <Link to={`/tas/people/search/${e.data.EmployeeId}`}>{e.value}</Link>
        </div>
      )
    },
    {
      label: 'AD Account',
      name: 'ADAccount',
    },
    {
      label: 'Primary',
      name: 'Main',
      alignment: 'left',
      cellRender: (row, r) => (
        <CheckBox iconSize={18} value={row.value === 1 ? true : 0} onValueChange={(e) => handleSetPrimary(e, row.data)}/>
      ),
    },
    {
      label: '',
      name: 'action',
      width: 93,
      cellRender: (e) => (
        <Button className='dlt-button' onClick={() => handleClickRemove(e.data)}>Remove</Button>
      )
      
    }
  ]

  const fields = [
    {
      label: 'Manager',
      name: 'departmentManagerId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: managers,
        loading: headLoading,
      }
    },
  ]

  const handleSubmitAdd = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/department/manager',
      data: {
        ...values,
        departmentId: departmentData?.Id
      }
    }).then(() => {
      getData()
      setShowModal(false)
    }).catch(() => {

    }).then(() => setLoading(false))
  }

  const handleDelete = () => {
    setLoading(true)
    axios({
      method: 'delete',
      url: `tas/department/manager/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch(() => {

    }).then(() => setLoading(false))
  }

  const setPrimary = () => {
    setLoading(true)
    axios({
      method: 'put',
      url: `tas/department/setmainmanager?Id=${editData.Id}`,
    }).then(() => {
      getData()
      setShowPrimaryPopup(false)
    }).catch(() => {

    }).then(() => setLoading(false))
  }

  return (
    <div className='max-w-[800px]'>
      <Table
        data={data}
        columns={columns}
        containerClass='px-0 shadow-none rounded-none border-none'
        title={<div className='border-b py-1 flex justify-between'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{data?.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <Button className='text-xs' onClick={() =>  setShowModal(true)}>Add</Button>
        </div>}
      />
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
          size='small'
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
        height={110}
        width={350}
      >
        <div>Are you sure you want to remove this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={loading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
      <Popup
        visible={showPrimaryPopup}
        showTitle={false}
        height={120}
        width={350}
      >
        <div className='text-center'>Are you sure you want to assign <span className='font-bold'>{editData?.FullName}</span> the main manager?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'success'} onClick={setPrimary} loading={loading}>Yes</Button>
          <Button onClick={() => setShowPrimaryPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
    </div>
  )
}

export default Managers