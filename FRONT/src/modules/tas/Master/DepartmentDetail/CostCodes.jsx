import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import { Popup } from 'devextreme-react'
import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

const title = 'Cost Code'

function CostCodes({data, getData, departmentData}) {
  const [ costCodes, setCostCodes ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ headLoading, setHeadLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editData , setEditData ] = useState(null)
  const [ form ] = Form.useForm()

  useEffect(() => {
    setHeadLoading(true)
    axios({
      method: 'get',
      url: 'tas/costcode',
    }).then((res) => {
      let tmp = res.data.map((item) => ({...item, value: item.Id, label: `${item.Number} ${item.Description}`}))
      setCostCodes(tmp)
    }).catch((err) => {

    }).then(() => setHeadLoading(false))
  },[])

  const handleClickRemove = (row) => {
    setEditData(row)
    setShowPopup(true)
  }

  const columns = [
    {
      label: 'Cost Code',
      name: 'CostCode',
      alignment: 'left',
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
      label: 'Cost Code',
      name: 'costCodeId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: costCodes,
        loading: headLoading,
      }
    },
  ]

  const handleSubmitAdd = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/departmentcostcode',
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
      // url: `tas/departmentcostcode/${editData.Id}`,
      url: `tas/departmentcostcode`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
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
    </div>
  )
}

export default CostCodes