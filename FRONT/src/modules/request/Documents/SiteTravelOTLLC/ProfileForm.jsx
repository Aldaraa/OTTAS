import { CloseCircleTwoTone, SaveFilled } from '@ant-design/icons'
import { Tabs, Form as AntForm, Input, DatePicker, Select, Row, Col, notification, InputNumber } from 'antd'
import { ErrorTabElement, FormItemRender, Button, Table } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import weekday from "dayjs/plugin/weekday"
import localeData from "dayjs/plugin/localeData"
import axios from 'axios'
import { Link } from 'react-router-dom'
import { AuthContext } from 'contexts'

dayjs.extend(weekday)
dayjs.extend(localeData)

const cyrillic = new RegExp(/^[А-ЯҮӨ]{2}\d{8}$/)
const phoneNumber = new RegExp(/^[0-9]{8}$/)
const latin = new RegExp(/^[A-Za-z -]+$/g)
const internationalPhone = new RegExp(/^[0-9 +]+$/)

const formItemLayout = {
  labelCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 12,
    },
    md: {
      span: 8,
    },
    lg: {
      span: 12,
    },
    xl: {
      span: 8,
    },
    xxl: {
      span: 6,
    },
  },
  wrapperCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 16,
    },
  },
};


function ProfileForm({className, refreshData, currentGroup, disabled}) {
  const [ currentKey, setCurrentKey ] = useState(0)
  const [ loading, setLoading ] = useState(true)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ error, setError ] =  useState(null)
  const [ isEdit, setIsEdit ] = useState(false)

  const [ form ] = AntForm.useForm()
  const { state } = useContext(AuthContext)
  const [api, contextHolder] = notification.useNotification();

  useEffect(() => {
    if(state.userProfileData && form){
      form?.setFieldsValue(state.userProfileData)
      personalFields.map((item) => {
        if(item.type === 'date'){
          if(state.userProfileData[item.name]){
            form?.setFieldValue(item.name, dayjs(state.userProfileData[item.name]))
          }
        }
      })
      onSiteFields.map((item) => {
        if(item.type === 'date'){
          if(state.userProfileData[item.name]){
            form?.setFieldValue(item.name, dayjs(state.userProfileData[item.name]))
          }
        }
      })
      citizenshipFieldList.map((item) => {
        if(item.type === 'date'){
          if(state.userProfileData[item.name]){
            form?.setFieldValue(item.name, dayjs(state.userProfileData[item.name]))
          }
        }
      })
      state.userProfileData.GroupData?.map((group) => {
        form?.setFieldValue(group.GroupMasterId, group.GroupDetailId)
      })
    }
  },[state.userProfileData, form])

  const handleCheckAccount = () => {
    axios({
      method: 'post',
      url: `tas/employee/checkadaccount`,
      data: {
        employeeId: state.userProfileData?.Id,
        adAccount: form?.getFieldValue('ADAccount'),
      }
    }).then((res) => {
      if(res.data.AdAccountValidationStatus){
        api.success({
          message: res.data.AdAccountValidationFailedReason,
          duration: 4,
          btn: false,
          closeIcon: false,
          description: <div>
            {/* {} */}
            {/* <ul className='list-disc'> */}
              {/* {
                res.response.data?.data?.map((error) => (
                  <li>{error}</li>
                ))
              } */}
            {/* </ul> */}
          </div>
        });
      }else{
        api.error({
          message: res.data.AdAccountValidationFailedReason,
          duration: 4,
          btn: false,
          closeIcon: false,
          description: <div>
            {
              res.data.EmployeeId &&
              <Link to={`/tas/people/search/${res.data.EmployeeId}`} className='text-blue-500 hover:underline'>
                {res.data.Firstname} {res.data.Lastname}
              </Link>
            }
          </div>
        });
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const personalFields = [
    {
      label: state.referData?.profileFields['NationalityId']?.Label,
      name: 'NationalityId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['NationalityId']?.FieldRequired, message: `${state.referData?.profileFields['NationalityId']?.Label} is required`}],
      inputprops: {
        options: state.referData.nationalities,
      }
    },
    {
      type: 'component',
      component:  <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue}) => {
          let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
          return( selectedNationality?.Code === 'MN' ?
            <AntForm.Item 
              label={state.referData?.profileFields['NRN']?.Label}
              name='NRN'
              className='col-span-12 lg:col-span-6 mb-0'
              rules={[
                {required: state.referData?.profileFields['NRN']?.FieldRequired, message: `${state.referData?.profileFields['NRN']?.Label} is required`},
                {pattern: cyrillic, message: `Please use only cyrillic characters`}
              ]}
            >
              <Input maxLength={10} onInput={(e) => e.target.value = e.target.value.toUpperCase()}/>
            </AntForm.Item>
            :
            <div className='col-span-12 lg:col-span-6 mb-0'></div>
          )
        }}
      </AntForm.Item>
    },
    {
      label: state.referData?.profileFields['Gender']?.Label,
      name: 'Gender',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['Gender']?.FieldRequired, message: `${state.referData?.profileFields['Gender']?.Label} is required`}],
      inputprops: {
        options: [{value: 1, label: 'Male'}, {value: 0, label: 'Female'}],
      }
    },
    {
      label: state.referData?.profileFields['Dob']?.Label,
      name: 'Dob',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'date',
      rules: [{required: state.referData?.profileFields['Dob']?.FieldRequired, message: `${state.referData?.profileFields['Dob']?.Label} is required`}],
    },
    {
      label: state.referData?.profileFields['Firstname']?.Label,
      name: 'Firstname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['Firstname']?.FieldRequired, message: `${state.referData?.profileFields['Firstname']?.Label} is required`}],
    },
    {
      label: state.referData?.profileFields['Lastname']?.Label,
      name: 'Lastname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['Lastname']?.FieldRequired, message: `${state.referData?.profileFields['Lastname']?.Label} is required`}],
    },
    {
      label: state.referData?.profileFields['MFirstname']?.Label,
      name: 'MFirstname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['MFirstname']?.FieldRequired, message: `${state.referData?.profileFields['MFirstname']?.Label} is required`}],
    },
    {
      label: state.referData?.profileFields['MLastname']?.Label,
      name: 'MLastname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['MLastname']?.FieldRequired, message: `${state.referData?.profileFields['MLastname']?.Label} is required`}],
    },
    {
      label: state.referData?.profileFields['CommenceDate']?.Label,
      name: 'CommenceDate',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'date',
      rules: [{required: state.referData?.profileFields['CommenceDate']?.FieldRequired, message: `${state.referData?.profileFields['CommenceDate']?.Label} is required`}],
      inputprops: {
        format: 'M/D/YYYY'
      }
    },
    {
      label: 'Completion Date',
      name: 'CompletionDate',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'date',
      inputprops: {
        format: 'M/D/YYYY'
      }
    },
    {
      label: state.referData?.profileFields['DepartmentId']?.Label,
      name: 'DepartmentId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['DepartmentId']?.FieldRequired, message: `${state.referData?.profileFields['DepartmentId']?.Label} is required`}],
      type: 'treeSelect',
      inputprops: {
        treeData: state.referData.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      }
    },
    {
      label: state.referData?.profileFields['CostCodeId']?.Label,
      name: 'CostCodeId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['CostCodeId']?.FieldRequired, message: `${state.referData?.profileFields['CostCodeId']?.Label} is required`}],
      inputprops: {
        options: state.referData.costCodes,
      }
    },
    {
      label: state.referData?.profileFields['StateId']?.Label,
      name: 'StateId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['StateId']?.FieldRequired, message: `${state.referData?.profileFields['StateId']?.Label} is required`}],
      inputprops: {
        options: state.referData.states,
      }
    },
    {
      label: state.referData?.profileFields['Email']?.Label,
      name: 'Email',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [
        {required: state.referData?.profileFields['Email']?.FieldRequired, message: `${state.referData?.profileFields['Email']?.Label} is required`},
        {type: 'email', message: `The input is not valid E-mail!`},
      ],
    },
    {
      type: 'component',
      component: <AntForm.Item className='col-span-12 lg:col-span-6 mb-0' label='AD Account' key={'component-ADAccount'}>
        <Row gutter={8}>
          <Col span={12}>
            <AntForm.Item 
              name="ADAccount"
              className='mb-0'
            >
              <Input />
            </AntForm.Item>
          </Col>
          <Col span={12}>
            <Button htmlType='button' disabled={!isEdit} onClick={handleCheckAccount}>Account Check</Button>
          </Col>
        </Row>
      </AntForm.Item>
    },
  ]

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
        label: `${user.Firstname} ${user.Lastname} /${user.Mobile}/`,
        value: user.Id,
      })),
    )
  }

  const isDisabledPType = useMemo(() => {
    if(state.userInfo?.Role === 'DataApproval'){
      return true
    }
    return !isEdit || state.userProfileData?.PeopleTypeId ? true : false
  },[state.userInfo, isEdit, state.userProfileData])

  const onSiteFields = [
    {
      label: state.referData?.profileFields['EmployerId']?.Label,
      name: 'EmployerId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['EmployerId']?.FieldRequired, message: `${state.referData?.profileFields['EmployerId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.employers,
      }
    },
    {
      label: state.referData?.profileFields['PeopleTypeId']?.Label,
      name: 'PeopleTypeId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['PeopleTypeId']?.FieldRequired, message: `${state.referData?.profileFields['PeopleTypeId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.peopleTypes,
        disabled: isDisabledPType
      }
    },
    {
      label: state.referData?.profileFields['SAPID']?.Label,
      name: 'SAPID',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['SAPID']?.FieldRequired, message: `${state.referData?.profileFields['SAPID']?.Label} is required`}],
      inputprops: {
        maxLength: 8,
      }
    },
    {
      label: state.referData?.profileFields['ContractNumber']?.Label,
      name: 'ContractNumber',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['ContractNumber']?.FieldRequired, message: `${state.referData?.profileFields['ContractNumber']?.Label} is required`}],
    },
    {
      label: 'Person #',
      name: 'Id',
      className: 'col-span-12 lg:col-span-6 mb-0',
      inputprops: {
        disabled: true,
      }
    },
    {
      label: state.referData?.profileFields['PositionId']?.Label,
      name: 'PositionId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['PositionId']?.FieldRequired, message: `${state.referData?.profileFields['PositionId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.positions,
      }
    },
    {
      label: state.referData?.profileFields['HotelCheck']?.Label,
      name: 'HotelCheck',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
      rules: [{required: state.referData?.profileFields['HotelCheck']?.FieldRequired, message: `${state.referData?.profileFields['HotelCheck']?.Label} is required`}],
    },
    {
      label: state.referData?.profileFields['RosterId']?.Label,
      name: 'RosterId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['RosterId']?.FieldRequired, message: `${state.referData?.profileFields['RosterId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.rosters,
      }
    },
    {
      label: 'Next Roster Start',
      name: 'NextRosterDate',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'date',
    },
    {
      label: state.referData?.profileFields['RoomId']?.Label,
      name: 'RoomId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['RoomId']?.FieldRequired, message: `${state.referData?.profileFields['RoomId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.rooms,
      }
    },
    {
      label: state.referData?.profileFields['LocationId']?.Label,
      name: 'LocationId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['LocationId']?.FieldRequired, message: `${state.referData?.profileFields['LocationId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.locations.filter((item) => item.onSite !== 1),
      }
    },
    {
      label: state.referData?.profileFields['FlightGroupMasterId']?.Label,
      name: 'FlightGroupMasterId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: state.referData?.profileFields['FlightGroupMasterId']?.FieldRequired, message: `${state.referData?.profileFields['FlightGroupMasterId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.transportGroups,
      }
    },
    {
      label: state.referData?.profileFields['SiteContactEmployeeId']?.Label,
      name: 'SiteContactEmployeeId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'searchSelect',
      rules: [{required: state.referData?.profileFields['SiteContactEmployeeId']?.FieldRequired, message: `${state.referData?.profileFields['SiteContactEmployeeId']?.Label} is required`}],
      inputprops: {
        fetchOptions: fetchUserList,
        showSearch: true,
        editName: ['SiteContactEmployeeFirstname', 'SiteContactEmployeeLastname'],
        placeholder: 'Select user',
        allowClear: true,
      }
    },
  ]

  const offSiteFields = [
    {
      label: state.referData?.profileFields['EmergencyContactName']?.Label,
      name: 'EmergencyContactName',
      className: 'col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['EmergencyContactName']?.FieldRequired, message: `${state.referData?.profileFields['EmergencyContactName']?.Label} is required`}],
    },
    {
      label: state.referData?.profileFields['EmergencyContactMobile']?.Label,
      name: 'EmergencyContactMobile',
      className: 'col-span-6 mb-0',
      rules: [
        {required: state.referData?.profileFields['EmergencyContactMobile']?.FieldRequired, message: `${state.referData?.profileFields['EmergencyContactMobile']?.Label} is required`}, 
        { pattern: internationalPhone, message: 'Invalid phone number'}
      ],
      inputprops: {
        // prefix: '+976',
        className: 'w-full',
        maxLength: 15,
      }
    },
    {
      label: state.referData?.profileFields['Mobile']?.Label,
      name: 'Mobile',
      className: 'col-span-6 mb-0',
      type: 'number',
      rules: [
        {required: state.referData?.profileFields['Mobile']?.FieldRequired, message: `${state.referData?.profileFields['Mobile']?.Label} is required`}, 
        {pattern: phoneNumber, message: `${state.referData?.profileFields['Mobile']?.Label} must be 8 digit`}
      ],
      inputprops: {
        prefix: '+976',
        className: 'w-full',
        maxLength: 8
      }
    },
    {
      type: 'component',
      component: <AntForm.Item noStyle shouldUpdate={(prev, cur) => prev.PeopleTypeId !== cur.PeopleTypeId}>
        {({getFieldValue}) => {
          const PeopleTypeId = getFieldValue('PeopleTypeId')
          const isVisitor = state.referData.peopleTypes?.find((item) => item.value === PeopleTypeId)?.Code === 'Visitor' 
          return(
            <AntForm.Item
              className='col-span-6 mb-0'
              name='PersonalMobile'
              label={state.referData?.profileFields['PersonalMobile']?.Label}
              rules={[
                {required: isVisitor ? false : state.referData?.profileFields['PersonalMobile']?.FieldRequired, message: `${state.referData?.profileFields['PersonalMobile']?.Label} is required`},
                {pattern: phoneNumber, message: `${state.referData?.profileFields['PersonalMobile']?.Label} must be 8 digit`}
              ]}
            >
              <InputNumber controls={false} prefix='+976' className='w-full' maxLength={8}/>
            </AntForm.Item>
          )
        }}
      </AntForm.Item>,
    },
    // {
    //   label: state.referData?.profileFields['PersonalMobile']?.Label,
    //   name: 'PersonalMobile',
    //   className: 'col-span-6 mb-0',
    //   type: 'number',
    //   rules: [
    //     {required: state.referData?.profileFields['PersonalMobile']?.FieldRequired, message: `${state.referData?.profileFields['PersonalMobile']?.Label} is required`},
    //     {pattern: phoneNumber, message: `${state.referData?.profileFields['PersonalMobile']?.Label} must be 8 digit`}
    //   ],
    //   inputprops: {
    //     prefix: '+976',
    //     className: 'w-full',
    //     maxLength: 8
    //   }
    // },
    {
      label: state.referData?.profileFields['PickUpAddress']?.Label,
      name: 'PickUpAddress',
      className: 'col-span-6 mb-0',
      type: 'textarea',
      rules: [
        {required: state.referData?.profileFields['PickUpAddress']?.FieldRequired, message: `${state.referData?.profileFields['PickUpAddress']?.Label} is required`}, 
      ],
    },
    {
      label: state.referData?.profileFields['FrequentFlyer']?.Label,
      name: 'FrequentFlyer',
      className: 'col-span-6 mb-0',
      rules: [
        {required: state.referData?.profileFields['FrequentFlyer']?.FieldRequired, message: `${state.referData?.profileFields['FrequentFlyer']?.Label} is required`}, 
      ],
    },
  ]

  const groupValidationFields = useMemo(() => {
    return state.referData?.fieldsOfGroups.map((item) => ({name: item.Id, label: item.Description}))
  },[state.referData?.fieldsOfGroups])

  const groupsFields = [
    {
      type: 'component',
      component: <>
          {
            state.referData?.fieldsOfGroups?.map((item, i) => (
              <AntForm.Item name={item.Id} label={item.Description} rules={[{required: item.Required, message: `${item.Description} is required`}]} className='col-span-12 lg:col-span-7 mb-0' key={`g-fields-${i}`}>
                <Select
                  fieldNames={{value: 'Id', label: 'Description'}}
                  filterOption={(input, option) => (option?.Description ?? '').toLowerCase().includes(input.toLowerCase())}
                  showSearch
                  allowClear
                  options={item.details}
                />
              </AntForm.Item>
            ))
          }
        </>
    },
  ]

  const validationCitizenshipFields = () => {
    if(form?.getFieldValue('NationalityId')){
      let selectedNationality = state.referData?.nationalities?.find((item) => item.value === form.getFieldValue('NationalityId'))
      if(selectedNationality?.Code !== 'MN'){
        return [
          {
            label: 'Passport Number',
            name: 'PassportNumber',
            rules: [{required: true, message: 'Passport Number is required'}],
          },
          {
            label: 'Name On Passport',
            name: 'PassportName',
            rules: [{required: true, message: 'Name on password is required'}],
          },
          {
            label: 'Passport Expiry Date',
            name: 'PassportExpiry',
            rules: [{required: true, message: 'Password expiry date is required'}],
          },
        ]
      }
      else{
        return []
      }
    }
    else {
      return []
    }
  } 

  const citizenshipFields = [
    {
      type: 'component',
      component: <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue}) => {
          let selectedNationality = state.referData?.nationalities?.find((item) => item.value === form.getFieldValue('NationalityId'))
          return selectedNationality?.Code !== 'MN' &&
            <>
              <AntForm.Item 
                label={state.referData?.profileFields['PassportNumber']?.Label}
                name='PassportNumber'
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[{required: state.referData?.profileFields['PassportExpiry']?.FieldRequired, message: `${state.referData?.profileFields['PassportNumber']?.Label} is required`}]}
              >
                <Input />
              </AntForm.Item>
              <AntForm.Item 
                label={state.referData?.profileFields['PassportName']?.Label}
                name='PassportName'
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[
                  { required: state.referData?.profileFields['PassportExpiry']?.FieldRequired, message: `${state.referData?.profileFields['PassportName']?.Label} is required`},
                  { pattern: latin, message: 'The input is not valid name!'}
                ]}
              >
                <Input />
              </AntForm.Item>
              <AntForm.Item 
                label={state.referData?.profileFields['PassportExpiry']?.Label}
                name='PassportExpiry'
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[{required: state.referData?.profileFields['PassportExpiry']?.FieldRequired, message: `${state.referData?.profileFields['PassportExpiry']?.Label} is required`}]}
              >
                <DatePicker />
              </AntForm.Item>
            </>
        }}
      </AntForm.Item>
    },
    {
      label: 'Passport Image',
      name: 'PassportRawImage',
      className: 'col-span-12 lg:col-span-7 mb-0',
      type: 'image',
    },
  ]

  const citizenshipFieldList = [
    {
      name: 'PassportNumber',
      type: 'image',
    },
    {
      name: 'PassportName',
      type: 'image',
    },
    {
      name: 'PassportExpiry',
      type: 'date',
    },
    {
      name: 'PassportImage',
      type: 'image',
    },
  ]

  const errorColumns = [
    {
      label: '',
      name: 'FullName',
      alignment:'left',
      wrap: true,
      cellRender: (e) => (
        <div className='flex items-center'>
          <Link to={`/tas/people/search/${e.data?.EmployeeId}`}>
            <button type='button' className='text-blue-500 hover:underline whitespace-normal'>
              {e.value} ({e.data?.EmployeeId})
            </button>
          </Link>
        </div>
      )
    },
    {
      label: 'ADAccount',
      name: 'ADAccount',
      alignment:'left',
      // width: '50px',
      cellRender: (e) => (
        <div>
          {typeof e.value === 'boolean' ? e.value ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
        </div>
      )
    },
    {
      label: 'SAP ID',
      name: 'SAPID',
      alignment:'center',
      width: '60px',
      cellRender: (e) => (
        <div>
          {typeof e.value === 'boolean' ? e.value ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
        </div>
      )
    },
    {
      label: 'Mobile',
      name: 'Mobile',
      alignment:'center',
      width: '60px',
      cellRender: (e) => (
        <div>
          {typeof e.value === 'boolean' ? e.value ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setError(null)
    setSubmitLoading(true)
    let groupValues = []
    const formData = new FormData();
    Object.keys(values).map((item) => {
      if(!isNaN(item)){
        groupValues.push({GroupMasterId: parseInt(item), GroupDetailId: values[item] ? values[item] : ''})
      }
      else if(item !== 'PassportRawImage' && item !== 'SiteContactEmployeeId'){
        if(item === 'Gender'){
          formData.append(item, values[item] ? 1 : 0)
        }
        else if(item === 'CommenceDate'){
          formData.append(item, values[item] ? dayjs(values[item]).format('YYYY-MM-DD') : '')
        }
        else if(item === 'CompletionDate'){
          formData.append(item, values[item] ? dayjs(values[item]).format('YYYY-MM-DD') : '')
        }
        else{
          formData.append(item, values[item] ? values[item] : '')
        }
      }
      else if(item === 'SiteContactEmployeeId'){
        if(typeof values[item] === 'string'){
          formData.append(item, state.userProfileData?.SiteContactEmployeeId ? state.userProfileData?.SiteContactEmployeeId : '')
        }
        else{
          formData.append(item, values[item] ? values[item] : '')
        }
      }
    })
    formData.append('PassportRawImage', values.PassportRawImage?.file ? values.PassportRawImage?.file : '')
    formData.append('id', state.userProfileData?.Id)
    axios({
      method: 'patch',
      url: 'tas/employee',
      data: formData,
    }).then( async (res) => {
      axios({
        method: 'post',
        url: `tas/groupmembers/${state.userProfileData?.Id}`,
        data: groupValues
      }).then((res) => {
        //////      Approve Document       ///////
        setIsEdit(false)
        refreshData(state.userProfileData?.Id)
      }).catch((err) => {
      }).then(() => {
        setSubmitLoading(false)
      })
    }).catch((err) => {
      let errordata = JSON.parse(err.response.data.message)
      if(errordata && errordata.length > 0){
        api.error({
          message: 'Validation Warning',
          duration: 0,
          description: <div>
            <Table data={errordata} columns={errorColumns} containerClass='shadow-none border' pager={false}/>
          </div>
        });
        setSubmitLoading(false)
      }
    })
  }

  const onFailed = (e) => {
    if(e){
      setError(e.errorFields)
    }
  }

  const renderErrors = (fields) => {
    let errorCount = 0;
    if(error){
      error.map((item) => {
        if(fields?.find((el) => el.name === item['name'][0])){
          errorCount++;
        }
      })
    }
    return errorCount > 0 && <ErrorTabElement>{errorCount}</ErrorTabElement>
  }

  const items = [
    {
      label: <div className='font-normal'>Personal {renderErrors([...personalFields, {name: 'NRN'}])}</div>,
      key: 0,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-0'}>
        <FormItemRender key={'t-item-1c'} fields={personalFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
    {
      label: <div>On Site {renderErrors(onSiteFields)}</div>,
      key: 1,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-1'}>
        <FormItemRender key={'t-item-2c'} fields={onSiteFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
    {
      label: <div>Off Site {renderErrors([...offSiteFields, {name: 'PersonalMobile'}])}</div>,
      key: 2,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-2'}>
        <FormItemRender key={'t-item-3c'} fields={offSiteFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
    state.userInfo?.Role === 'DataApproval' ?
    {
      label: <div>Group {renderErrors(groupValidationFields)}</div>,
      key: 3,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-3'}>
        <FormItemRender key={'t-item-4c'} fields={groupsFields} form={form} editData={state.userProfileData}/>
      </div>,
    } : null,
    {
      label: <div>Citizenship {renderErrors(validationCitizenshipFields())}</div>,
      key: 4,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-4'}>
        <FormItemRender key={'t-item-5c'} fields={citizenshipFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
  ]

  return (
    <>
      {
        state.customLoading ?
        <div className='animate-skeleton h-[400px] flex rounded-ot shadow-md col-span-12'></div>
        :
        <div className={`bg-white rounded-ot p-4 col-span-12 shadow-md mb-1 ${className}`}>
          <AntForm 
            form={form} 
            {...formItemLayout} 
            labelAlign='left'
            onFinish={handleSubmit}
            className='bg-white rounded-lg'
            onFinishFailed={onFailed}
            disabled={!isEdit}
            scrollToFirstError={true}
          >
            <Tabs
              defaultActiveKey="1"
              type="card"
              size={'middle'}
              items={items}
              activeKey={currentKey}
              onTabClick={(e) => setCurrentKey(e)}
            />
          </AntForm>
          <div className='mt-3 flex justify-end'>
            {
              !disabled ?
              (isEdit ?
              <div className='flex items-center gap-3'>
                <Button type='primary' loading={submitLoading} onClick={() => form.submit()} icon={<SaveFilled/>}>Save</Button>
                <Button onClick={() => setIsEdit(false)} disabled={submitLoading}>Cancel</Button>
              </div>
              :
              <Button onClick={() => setIsEdit(true)}>Edit</Button>
              )
              :
              null    
            }
          </div>
        </div>
      }
      {contextHolder}
    </>
  )
}

export default ProfileForm