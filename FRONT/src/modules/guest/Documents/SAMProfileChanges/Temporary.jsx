import { Checkbox, DatePicker, Select, Skeleton, TreeSelect } from 'antd'
import axios from 'axios'
import { Button, Form } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useContext, useState } from 'react'

function Temporary({form, disabled, temporaryData}) {
  const { state } = useContext(AuthContext)
  const [ loading, setLoading ] = useState(false)

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: `tas/requestdocumentprofilechange/temp`,
      data: {
        ...values,
        StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : null,
        EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
        Id: temporaryData?.Id
      }
    }).then((res) => {

    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  return (
    <Form form={form} className='max-w-[600px]' onFinish={handleSubmit} disabled={disabled}>
      <Form.Item name={'EmployerId'} label='Employer' className='col-span-12 mb-2'>
        <Select 
          options={state.referData?.employers}
          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
          showSearch
          allowClear
        />
      </Form.Item>
      <Form.Item name={'DepartmentId'} label='Department' className='col-span-12 mb-2'>
        <TreeSelect 
          treeData={state.referData?.departments}
          fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
          onSelect={(val, node) => form.setFieldValue('CostCodeId', node.CostCodeId)}
          showSearch
          allowClear
        />
      </Form.Item>
      <Form.Item name={'CostCodeId'} label='Cost Code' className='col-span-12 mb-2'>
        <Select 
          options={state.referData?.costCodes}
          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
          showSearch
          allowClear
        />
      </Form.Item>
      <Form.Item name={'PositionId'} label='Position' className='col-span-12 mb-2'>
        <Select 
          options={state.referData?.positions}
          filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
          showSearch
          allowClear
        />
      </Form.Item>
      <Form.Item name={'StartDate'} label='Start Date' className='col-span-12 mb-2'>
        <DatePicker/>
      </Form.Item>
      <Form.Item name={'EndDate'} label='End Date' className='col-span-12 mb-2'>
        <DatePicker/>
      </Form.Item>
      {/* <Form.Item name={'Permanent'} label='Permanent' className='col-span-12 mb-2'>
        <Checkbox/>
      </Form.Item> */}
      {
        !disabled ?
        <Form.Item className='col-span-12 flex justify-end'>
          <Button type='primary' onClick={form.submit} loading={loading}>Save</Button>
        </Form.Item>
        : null
      }
    </Form>
  )
}

export default Temporary