import { SaveOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import React, { useEffect, useState } from 'react'

function ReAssignLM({selectedRow, open, onCancel, reload}) {
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ loading, setLoading ] = useState(false)
  const [ lineManagers, setLineManagers ] = useState([])
  const [ form ] = Form.useForm()

  useEffect(() => {
    if(selectedRow?.EmployeeId){
      fetchLineManagers()
    }
  },[selectedRow?.EmployeeId])

  const fetchLineManagers = () => {
    setLoading(true)
    axios.get(`tas/requestgroupconfig/employee/linemanagers/${selectedRow?.EmployeeId}`).then((res) => {
      setLineManagers(res.data)
    }).catch(() => {

    }).finally(() => {
      setLoading(false)
    })
  }


  const handleSubmit = (values) => {
    setActionLoading(true)
    axios.put('tas/requestdocument/changelinemanager', {
      id: selectedRow?.Id,
      ...values
    }).then(() => {
      reload()
      onCancel()
    }).catch(() => {

    }).finally(() => {
      setActionLoading(false)
    })
  }

  const fields = [
    {
      name: 'newAssignEmployeeId',
      label: 'Assign To',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Please select a Employee'}],
      type: 'select',
      inputprops: {
        options: lineManagers,
        fieldNames: {value: 'id', label: 'fullName'},
        loading: loading,
        disabled: loading,
      }
    }
  ]
  return (
    <Modal open={open} onCancel={onCancel} title='Reassign Line Manager' >
      <div className='flex gap-2 mb-4'>
        <div className='text-secondary2 mr-2'>Current Line Manager:</div>
        <div className=''>{selectedRow?.AssignedEmployeeFullName}</div>
      </div>

      <Form
        form={form}
        fields={fields}
        labelCol={{flex: '100px'}}
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
          <Button onClick={onCancel} disabled={actionLoading}>Cancel</Button>
        </div>
      </Form>
    </Modal>
  )
}

export default ReAssignLM