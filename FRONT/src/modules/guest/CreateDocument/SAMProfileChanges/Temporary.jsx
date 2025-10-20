import { Checkbox, DatePicker, Select, Skeleton, TreeSelect } from 'antd'
import { Button, Form } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect } from 'react'

function Temporary({form, disabled}) {
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(state.userProfileData){
      form.setFieldsValue(state.userProfileData)
    }
  },[state.userProfileData])
  return (
    <Form form={form} className='max-w-[600px]' disabled={disabled}>
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
          filterTreeNode={(input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase())}
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
    </Form>
  )
}

export default Temporary