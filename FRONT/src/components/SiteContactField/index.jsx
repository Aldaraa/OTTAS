import { Select } from 'antd'
import axios from 'axios'
import { Form, SearchSelection } from 'components'
import React from 'react'

function SiteContactEmployeeField({ profileFields, form }) {

  const getManagers = (DepartmentId) => {
    axios({
      method: 'get',
      url: `tas/department/${DepartmentId}`,
    }).then((res) => {
      if(res.data.Managers?.length > 0){
        let managers = res.data.Managers?.map((item) => ({label: item.FullName, value: item.EmployeeId, ...item}))
        form.setFieldValue('SiteContanctOption', managers)
      }else{
        form.setFieldValue('SiteContanctOption', null)
      }
    }).catch((err) => {

    })
  }

  function fetchUserList(value) {
    return axios({
      method: 'post',
      url: 'tas/employee/searchshort',
      data: {
        model: {
          keyWord: value
        },
        pageIndex: 0,
        pageSize: 100
      }
    }).then((res) => 
      res.data.data.map((user) => ({
        label: `${user.Firstname} ${user.Lastname} /${user.Id}/`,
        value: user.Id,
      })),
    )
  }

  return (
    <>
      <Form.Item noStyle shouldUpdate={(prev, cur) => prev.DepartmentId !== cur.DepartmentId}>
        {({getFieldValue, setFieldValue}) => {
          let departmentId = getFieldValue('DepartmentId')
          if(departmentId){
            getManagers(departmentId)
          }
          return null
        }}
      </Form.Item>
      <Form.Item noStyle shouldUpdate={(prev, cur) => prev.SiteContanctOption !== cur.SiteContanctOption}>
        {({getFieldValue, setFieldValue}) => {
          let options = getFieldValue('SiteContanctOption')
          return (
            options ?
            <Form.Item 
              name='SiteContactEmployeeId' 
              label={profileFields['SiteContactEmployeeId']?.Label} 
              labelCol={{flex: '150px'}}
              rules={[{required: profileFields['SiteContactEmployeeId']?.FieldRequired, message: `${profileFields['SiteContactEmployeeId']?.Label} is required`}]}
              className='col-span-12 lg:col-span-6 mb-0'
            >
              <Select
                options={options}
                loading={false}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
            :
            <Form.Item 
              name='SiteContactEmployeeId' 
              label={profileFields['SiteContactEmployeeId']?.Label} 
              rules={[{required: profileFields['SiteContactEmployeeId']?.FieldRequired, message: `${profileFields['SiteContactEmployeeId']?.Label} is required`}]}
              className='col-span-12 lg:col-span-6 mb-0'
              labelCol={{flex: '150px'}}
            > 
              <SearchSelection
                fetchOptions={fetchUserList}
                showSearch={true}
                editName={['SiteContactEmployeeFirstname', 'SiteContactEmployeeLastname']}
                placeholder='Select user'
                allowClear={true}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    </>
  )
}

export default SiteContactEmployeeField