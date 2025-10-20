import { CalendarOutlined } from '@ant-design/icons';
import { Calendar, Checkbox, ConfigProvider, DatePicker, Dropdown, Form, Input, InputNumber, Select, TreeSelect } from 'antd';
import { Button, Tooltip } from 'components';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import React, { useContext } from 'react'
import { reportInstance } from 'utils/axios';

const { SHOW_CHILD } = TreeSelect;

function ReportFields(field, parentFieldName, form, templateDetail) {
  const { state } = useContext(AuthContext)

  const getDate = (variable, days, name) => {
    reportInstance({
      method: 'get',
      url: `tasreport/reporttemplate/datesimulate/${variable}/${days}`
    }).then((res) => {
      form.setFieldValue(name, dayjs(res.data).format('YYYY-MM-DD'))
    })
  }

  switch (field?.Component) {
    case 'CAMP_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.camps]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'DEPARTMENT_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <TreeSelect
            treeData={[...state.referData.reportDepartments]}
            fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
            filterTreeNode={(input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            placeholder='Default all'
            multiple
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'EMPLOYER_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.reportEmployers]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'COSTCODE_DROPDOWB':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.costCodes]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'POSITION_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.positions]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'RESOURCETYPE_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.peopleTypes]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'STATE_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.states]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'NATIONALITY_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.nationalities]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'ROSTER_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.rosters]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'TRANSPORTMODE_DROPDOWN':  
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.transportModes]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    // case 'ACTIVETRANSPORT_DROPDOWN':  
    //   return(
    //     <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
    //       <Select 
    //         options={[...state.referData?.transportModes]}
    //         filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
    //         popupMatchSelectWidth={false}
    //         mode='multiple'
    //         placeholder='Default all'
    //         allowClear
    //         showSearch
    //       />
    //     </Form.Item>
    //   )
    case 'LOCATION_DROPDOWN_SINGLE_SELECT':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.locations]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'LOCATION_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.locations]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'ROOMTYPE_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.roomTypes]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'GROUP_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <TreeSelect
            treeData={[...state.referData?.fieldsOfGroups.map((item) => ({...item, selectable: false}))]}
            fieldNames={{label: 'Description', value: 'Id', children: 'details'}}
            filterTreeNode={(input, option) => (option?.Description ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            placeholder='Default all'
            // showCheckedStrategy='SHOW_CHILD'
            treeCheckable={true}
            multiple
            allowClear
            showSearch
          />
        </Form.Item>
      )
    // case 'GROUP_DETAIL_DROPDOWN':
    //   return(
    //     <Form.Item noStyle shouldUpdate={(prev, cur) => prev.parameters?.GroupMasterId !== cur.parameters?.GroupMasterId}>
    //       {
    //         ({getFieldValue, getFieldsValue, setFieldValue}) => {
    //           let options = state.referData?.fieldsOfGroups.find((item) => item.Id === getFieldValue(['parameters', 'GroupMasterId']))
    //           return(
    //             <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
    //               <Select 
    //                 options={options ? options.details : []}
    //                 loading={state.loadings.fieldsOfGroups}
    //                 filterOption={(input, option) => (option?.Description ?? '').toLowerCase().includes(input.toLowerCase())}
    //                 popupMatchSelectWidth={false}
    //                 fieldNames={{label: 'Description', value: 'Id'}}
    //                 mode='multiple'
    //                 placeholder='Default all'
    //                 allowClear
    //                 showSearch
    //               />
    //             </Form.Item>
    //           )
    //         }
    //       }
    //     </Form.Item>
    //   )
    case 'DOCUMENTTYPE_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.master?.documentTypes]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'REQUESTGROUP_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[...state.referData?.approvalGroups]}
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            mode='multiple'
            placeholder='Default all'
            allowClear
            showSearch
          />
        </Form.Item>
      )
    case 'TAS_BOOLEAN_DROPDOWN':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <Select 
            options={[{value: 'Yes', label: 'Yes'}, {value: 'No', label: 'No'}]}
            popupMatchSelectWidth={false}
            placeholder='Default all'
            allowClear
          />
        </Form.Item>
      )
    case 'TAS_DYNAMIC_DATE':
      return(
        <div className='flex items-center gap-2'>
          <div className='flex'>
            <Form.Item noStyle shouldUpdate={(pre, cur) => pre[parentFieldName, field.FielName, 'FieldValue'] !== cur[parentFieldName, field.FielName, 'FieldValue'] || pre[parentFieldName, field.FielName, 'Days'] !== cur[parentFieldName, field.FielName, 'Days']}>
              {({getFieldValue}) => {
                let variable = getFieldValue([parentFieldName, field.FielName, 'FieldValue'])
                let days = getFieldValue([parentFieldName, field.FielName, 'Days'])
                if(variable && typeof days === 'number'){
                  getDate(variable, days, [parentFieldName, field.FielName, 'Date'])
                }
                return null
              }}
            </Form.Item>
            <Form.Item name={[parentFieldName, field.FielName, 'FieldValue']} className='my-1'>
              <Select
                options={state.referData?.dateTypes}
                popupMatchSelectWidth={false}
                allowClear
                style={{width: 179}}
              />
            </Form.Item>
            <Form.Item className='my-1'>
              <Dropdown
                trigger={'click'}
                menu={{items: []}}
                dropdownRender={(menu) => {
                  return <div className='w-[300px] border rounded-[10px] overflow-hidden'>
                    <Calendar fullscreen={false} mode='month' onChange={(e) => form.setFieldValue([parentFieldName, field.FielName, 'FieldValue'], e.format('YYYY-MM-DD')) }/>
                  </div>
                }}
              >
                <Button className='py-2 px-2' icon={<CalendarOutlined/>}></Button>
              </Dropdown>
            </Form.Item>
          </div>
           <Form.Item name={[parentFieldName, field.FielName, 'Days']} className='my-1'>
            <InputNumber 
              max={1000}
              min={-1000}
            />
          </Form.Item>
          <Tooltip title="Simulate a date parameter based on today's date.">
            <Form.Item name={[parentFieldName, field.FielName, 'Date']} className='my-1'>
              <Input disabled={true} className='cursor-default text-gray-400'/>
            </Form.Item>
          </Tooltip>
        </div>
      )
    case 'TAS_STATIC_DATE':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} className='my-1'>
          <DatePicker/>
        </Form.Item>
      )
    case 'GENDER_CHECK':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} valuePropName="checked" className='my-1'>
          <Checkbox/>
        </Form.Item>
      )
    case 'ACTIVE_CHECK':
      return(
        <Form.Item name={[parentFieldName, field.FielName]} valuePropName="checked" className='my-1'>
          <Checkbox/>
        </Form.Item>
      )
  }
}

export default ReportFields