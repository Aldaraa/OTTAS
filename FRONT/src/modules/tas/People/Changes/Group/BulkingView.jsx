import { LeftOutlined } from '@ant-design/icons'
import { Form, Select } from 'antd'
import axios from 'axios'
import { Button, Table } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

const flattenTreeData = (array) => array.flatMap((props) => [
  { ...props, expanded:  props.ChildDepartments.length > 0 ? true : false },
  ...flattenTreeData(props.ChildDepartments || [])
]);

function BulkingView({data=[], handleChangeData, changeIsEditing, className='', onRemove}) {
  const [ loading, setLoading ] = useState(false)

  const { state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    form.setFieldsValue({rosters: data})
  },[data])

  const handleSubmit = (values) => {
    setLoading(true)
    const empIds = data?.map((item) => item.Id)
    axios({
      method: 'put',
      url: 'tas/employee/changedata/group',
      data: {
        empIds: empIds,
        groupMasterId: values.groupMasterId,
        groupDetailId: values.groupDetailId,
      },
    }).then((res) => {
      clearDatas()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const clearDatas = () => {
    handleChangeData([])
    changeIsEditing(false)
  }

  const handleBack = () => {
    changeIsEditing(false)
  }

  const handleRemoveItem = (event, row, index) => {
    event.stopPropagation()
    onRemove(row, index)
  }

  const columns = [
    {
      label: '#',
      name: 'index',
      width: 30,
      cellRender: ({rowIndex}) =>(
        <div>{rowIndex+1}</div>
      )
    },
    {
      label: 'Fullname',
      name: 'Fullname',
      cellRender: ({data}) =>(
        <div>{data.Firstname} {data.Lastname}</div>
      )
    },
    {
      label: 'Roster',
      name: 'RosterName',
    },
    {
      label: 'Cost Code',
      name: 'CostCodeName',
    },
    {
      label: 'Department',
      name: 'DepartmentName',
    },
    {
      label: 'Room Type',
      name: 'RoomTypeName',
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
    },
    {
      label: '',
      name: 'remove',
      width: 100,
      cellRender: (e) => (
        <button type='button' className='dlt-button text-xs' onClick={(event) => handleRemoveItem(event, e.data, e.rowIndex)}>Remove</button>
      )
    },
  ]

  return (
    <div className={`rounded-ot bg-white p-5 mb-5 shadow-md ${className}`}>
      <Button className='mb-3' onClick={handleBack} icon={<LeftOutlined/>}>Back</Button>
      <Table
        columns={columns}
        data={data}
        containerClass='shadow-none border'
        pager={false}
      />
      <div className='mt-2 ml-4'><span className='font-bold'>{data.length}</span> people selected</div>
      <Form 
        form={form} 
        size='small' 
        onFinish={handleSubmit} 
        initialValues={{
          rosters: [], 
        }}
      >
        <div className='col-span-12 flex gap-5 mt-4'>
          <Form.Item name={'groupMasterId'} label='Group'>
            <Select 
              options={state.referData?.fieldsOfGroups}
              style={{width: 180}}
              fieldNames={{label: 'Description', value: 'Id'}}
            />
          </Form.Item>
          <Form.Item shouldUpdate={(prev, next) => prev.groupMasterId !== next.groupMasterId}>
            {
              ({getFieldValue, setFieldValue}) => {
                let selectedGroup = state.referData?.fieldsOfGroups?.find((item) => item.Id === getFieldValue('groupMasterId'))
                setFieldValue('groupDetailId', null)
                return(
                  <Form.Item name={'groupDetailId'} label='Sub group'>
                    <Select 
                      options={selectedGroup ? selectedGroup?.details : []}
                      fieldNames={{label: 'Description', value: 'Id'}}
                      style={{width: 180}}
                    />
                  </Form.Item>
                )
              }
            }
          </Form.Item>
          <Form.Item shouldUpdate={(prev, next) => prev.groupMasterId !== next.groupMasterId || prev.groupDetailId !== next.groupDetailId}>
            {
              ({getFieldValue}) => (
                <Form.Item>
                  <Button 
                    type='primary' 
                    disabled={
                      !getFieldValue('groupMasterId') ||
                      !getFieldValue('groupDetailId')
                    }
                    onClick={() => form.submit()}
                    loading={loading}
                    className='py-0'
                  >
                    Process
                  </Button>
                </Form.Item>
              )
            }
          </Form.Item>
        </div>
      </Form>
    </div>
  )
}

export default BulkingView