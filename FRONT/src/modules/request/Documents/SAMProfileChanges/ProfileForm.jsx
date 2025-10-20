import { CloseCircleTwoTone, SaveFilled } from '@ant-design/icons'
import { Tabs, Form as AntForm, Input, DatePicker, Select, Row, Col, notification } from 'antd'
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


function ProfileForm({data, form, documentData, className, refreshData, disabled, getProfile, profileFields=[]}) {
  // const [ data, setData ] = useState(null)
  const [ currentKey, setCurrentKey ] = useState(0)
  const [ loading, setLoading ] = useState(true)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ error, setError ] =  useState(null)
  const [ isEdit, setIsEdit ] = useState(false)

  // const [form] = AntForm.useForm()
  const { state } = useContext(AuthContext)
  const [api, contextHolder] = notification.useNotification();

  useEffect(() => {
    if(documentData?.DocumentTag !== 'temp'){
      if(data && form){
        let initValues = {
          ...data,
          Dob: data?.Dob ? dayjs(data?.Dob) : null,
          CommenceDate: data?.CommenceDate ? dayjs(data?.CommenceDate) : null,
          CompletionDate: data?.CompletionDate ? dayjs(data?.CompletionDate) : null,
          NextRosterDate: data?.NextRosterDate ? dayjs(data?.NextRosterDate) : null,
          PassportExpiry: data?.PassportExpiry ? dayjs(data?.PassportExpiry) : null,
        }
        form?.setFieldsValue(initValues)
        initValues.GroupData?.map((group) => {
          form?.setFieldValue(group.GroupMasterId, group.GroupDetailId)
        })
      }
    }
  },[data, form])

  useEffect(() => {
    if(documentData?.DocumentTag === 'temp' && state.userProfileData){
      form?.setFieldsValue(state?.userProfileData)
      personalFields.map((item) => {
        if(item.type === 'date'){
          if(state?.userProfileData[item.name]){
            form?.setFieldValue(item.name, dayjs(state?.userProfileData[item.name]))
          }
        }
      })
      onSiteFields.map((item) => {
        if(item.type === 'date'){
          if(state?.userProfileData[item.name]){
            form?.setFieldValue(item.name, dayjs(state?.userProfileData[item.name]))
          }
        }
      })
      citizenshipFieldList.map((item) => {
        if(item.type === 'date'){
          if(state?.userProfileData[item.name]){
            form?.setFieldValue(item.name, dayjs(state?.userProfileData[item.name]))
          }
        }
      })
      state?.userProfileData.GroupData?.map((group) => {
        form?.setFieldValue(group.GroupMasterId, group.GroupDetailId)
      })
    }
  },[documentData, state.userProfileData])

  const handleCheckAccount = () => {
    axios({
      method: 'post',
      url: `tas/employee/checkadaccount`,
      data: {
        employeeId: data?.Id,
        adAccount: form?.getFieldValue('ADAccount')
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
      label: profileFields['NationalityId']?.Label,
      name: 'NationalityId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['NationalityId']?.FieldRequired, message: `${profileFields['NationalityId']?.Label} is required`}],
      inputprops: {
        options: state.referData.nationalities,
      }
    },
    {
      label: profileFields['Email']?.Label,
      name: 'Email',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [
        {required: profileFields['Email']?.FieldRequired, message: `${profileFields['Email']?.Label} is required`},
        {type: 'email', message: `The input is not valid E-mail!`}
      ],
    },
    // {
    //   type: 'component',
    //   component:  <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NationalityId !== curValues.NationalityId }>
    //     {({getFieldValue}) => {
    //       let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
    //       return( selectedNationality?.Code === 'MN' ?
    //         <AntForm.Item 
    //           label={profileFields['NRN']?.Label}
    //           name='NRN'
    //           className='col-span-12 lg:col-span-6 mb-0'
    //           rules={[
    //             {required: profileFields['NRN']?.FieldRequired, message: `${profileFields['NRN']?.Label} is required`},
    //             {pattern: cyrillic, message: `Please use only cyrillic characters`}
    //           ]}
    //         >
    //           <Input maxLength={10} onInput={(e) => e.target.value = e.target.value.toUpperCase()}/>
    //         </AntForm.Item>
    //         :
    //         <div className='col-span-12 lg:col-span-6 mb-0'></div>
    //       )
    //     }}
    //   </AntForm.Item>
    // },
    {
      label: profileFields['Gender']?.Label,
      name: 'Gender',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['Gender']?.FieldRequired, message: `${profileFields['Gender']?.Label} is required`}],
      inputprops: {
        options: [{value: 1, label: 'Male'}, {value: 0, label: 'Female'}],
      }
    },
    {
      label: profileFields['Dob']?.Label,
      name: 'Dob',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'date',
      rules: [{required: profileFields['Dob']?.FieldRequired, message: `${profileFields['Dob']?.Label} is required`}],
    },
    {
      label: profileFields['Firstname']?.Label,
      name: 'Firstname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['Firstname']?.FieldRequired, message: `${profileFields['Firstname']?.Label} is required`}],
    },
    {
      label: profileFields['Lastname']?.Label,
      name: 'Lastname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['Lastname']?.FieldRequired, message: `${profileFields['Lastname']?.Label} is required`}],
    },
    {
      label: profileFields['MFirstname']?.Label,
      name: 'MFirstname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['MFirstname']?.FieldRequired, message: `${profileFields['MFirstname']?.Label} is required`}],
    },
    {
      label: profileFields['MLastname']?.Label,
      name: 'MLastname',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['MLastname']?.FieldRequired, message: `${profileFields['MLastname']?.Label} is required`}],
    },
    {
      label: profileFields['CommenceDate']?.Label,
      name: 'CommenceDate',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'date',
      rules: [{required: profileFields['CommenceDate']?.FieldRequired, message: `${profileFields['CommenceDate']?.Label} is required`}],
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
      label: profileFields['DepartmentId']?.Label,
      name: 'DepartmentId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['DepartmentId']?.FieldRequired, message: `${profileFields['DepartmentId']?.Label} is required`}],
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
      label: profileFields['CostCodeId']?.Label,
      name: 'CostCodeId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['CostCodeId']?.FieldRequired, message: `${profileFields['CostCodeId']?.Label} is required`}],
      inputprops: {
        options: state.referData.costCodes,
      }
    },
    {
      label: profileFields['StateId']?.Label,
      name: 'StateId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['StateId']?.FieldRequired, message: `${profileFields['StateId']?.Label} is required`}],
      inputprops: {
        options: state.referData.states,
      }
    },
    {
      type: 'component',
      component: <AntForm.Item className='col-span-12 lg:col-span-6 mb-0' label='AD Account' key={'component-ADAccount'}>
        <Row gutter={8}>
          <Col span={12}>
            <AntForm.Item 
              name="ADAccount"
              className='mb-0'
              // rules={[{required: 'ADAccount', message: 'ADAccount is required'}]}
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

  const onSiteFields = [
    {
      label: profileFields['EmployerId']?.Label,
      name: 'EmployerId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['EmployerId']?.FieldRequired, message: `${profileFields['EmployerId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.employers,
      }
    },
    {
      label: profileFields['PeopleTypeId']?.Label,
      name: 'PeopleTypeId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['PeopleTypeId']?.FieldRequired, message: `${profileFields['PeopleTypeId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.peopleTypes,
      }
    },
    {
      label: profileFields['SAPID']?.Label,
      name: 'SAPID',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['SAPID']?.FieldRequired, message: `${profileFields['SAPID']?.Label} is required`}],
      inputprops: {
        maxLength: 8,
      }
    },
    {
      label: profileFields['ContractNumber']?.Label,
      name: 'ContractNumber',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['ContractNumber']?.FieldRequired, message: `${profileFields['ContractNumber']?.Label} is required`}],
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
      label: profileFields['PositionId']?.Label,
      name: 'PositionId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['PositionId']?.FieldRequired, message: `${profileFields['PositionId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.positions,
      }
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
    },
    {
      label: profileFields['HotelCheck']?.Label,
      name: 'HotelCheck',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
      rules: [{required: profileFields['HotelCheck']?.FieldRequired, message: `${profileFields['HotelCheck']?.Label} is required`}],
    },
    {
      label: profileFields['RosterId']?.Label,
      name: 'RosterId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['RosterId']?.FieldRequired, message: `${profileFields['RosterId']?.Label} is required`}],
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
    // {
    //   label: profileFields['RoomId']?.Label,
    //   name: 'RoomId',
    //   className: 'col-span-12 lg:col-span-6 mb-0',
    //   type: 'select',
    //   rules: [{required: profileFields['RoomId']?.FieldRequired, message: `${profileFields['RoomId']?.Label} is required`}],
    //   inputprops: {
    //     options: state.referData?.rooms,
    //   }
    // },
    {
      label: profileFields['LocationId']?.Label,
      name: 'LocationId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['LocationId']?.FieldRequired, message: `${profileFields['LocationId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.locations.filter((item) => item.onSite !== 1),
      }
    },
    {
      label: profileFields['FlightGroupMasterId']?.Label,
      name: 'FlightGroupMasterId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['FlightGroupMasterId']?.FieldRequired, message: `${profileFields['FlightGroupMasterId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.transportGroups,
      }
    },
    {
      label: profileFields['SiteContactEmployeeId']?.Label,
      name: 'SiteContactEmployeeId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'searchSelect',
      rules: [{required: profileFields['SiteContactEmployeeId']?.FieldRequired, message: `${profileFields['SiteContactEmployeeId']?.Label} is required`}],
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
      label: profileFields['EmergencyContactName']?.Label,
      name: 'EmergencyContactName',
      className: 'col-span-6 mb-0',
      rules: [{required: profileFields['EmergencyContactName']?.FieldRequired, message: `${profileFields['EmergencyContactName']?.Label} is required`}],
    },
    {
      label: profileFields['EmergencyContactMobile']?.Label,
      name: 'EmergencyContactMobile',
      className: 'col-span-6 mb-0',
      rules: [
        {required: profileFields['EmergencyContactMobile']?.FieldRequired, message: `${profileFields['EmergencyContactMobile']?.Label} is required`}, 
        { pattern: internationalPhone, message: 'Invalid phone number'}
      ],
      inputprops: {
        // prefix: '+976',
        className: 'w-full',
        maxLength: 15,
      }
    },
    {
      label: profileFields['Mobile']?.Label,
      name: 'Mobile',
      className: 'col-span-6 mb-0',
      type: 'number',
      rules: [
        {required: profileFields['Mobile']?.FieldRequired, message: `${profileFields['Mobile']?.Label} is required`}, 
        {pattern: phoneNumber, message: `${profileFields['Mobile']?.Label} must be 8 digit`}
      ],
      inputprops: {
        prefix: '+976',
        className: 'w-full',
        maxLength: 8
      }
    },
    {
      label: profileFields['PersonalMobile']?.Label,
      name: 'PersonalMobile',
      className: 'col-span-6 mb-0',
      type: 'number',
      rules: [
        {required: profileFields['PersonalMobile']?.FieldRequired, message: `${profileFields['PersonalMobile']?.Label} is required`},
        {pattern: phoneNumber, message: `${profileFields['PersonalMobile']?.Label} must be 8 digit`}
      ],
      inputprops: {
        prefix: '+976',
        className: 'w-full',
        maxLength: 8
      }
    },
    {
      label: profileFields['PickUpAddress']?.Label,
      name: 'PickUpAddress',
      className: 'col-span-6 mb-0',
      type: 'textarea',
      rules: [
        {required: profileFields['PickUpAddress']?.FieldRequired, message: `${profileFields['PickUpAddress']?.Label} is required`}, 
      ],
    },
    {
      label: profileFields['FrequentFlyer']?.Label,
      name: 'FrequentFlyer',
      className: 'col-span-6 mb-0',
      rules: [
        {required: profileFields['FrequentFlyer']?.FieldRequired, message: `${profileFields['FrequentFlyer']?.Label} is required`}, 
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
              <AntForm.Item 
                name={item.Id}
                label={item.Description}
                className='col-span-12 lg:col-span-6 mb-0'
                key={`g-fields-${i}`}
                rules={[{required: item.Required, message: `${item.Description} is required`}]}
              >
                <Select
                  showSearch
                  allowClear
                  filterOption={(input, option) => (option?.children ?? '').toLowerCase().includes(input.toLowerCase())}
                >
                  {
                    item.details?.map((detail, idx) => (
                      <Select.Option value={detail.Id} key={idx}>{detail.Description}</Select.Option>
                    ))
                  }
                </Select>
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
        return [
    
        ]
      }
    }
    else {
      return [
     
      ]
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
                label={profileFields['PassportNumber']?.Label}
                name='PassportNumber'
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[{required: profileFields['PassportExpiry']?.FieldRequired, message: `${profileFields['PassportNumber']?.Label} is required`}]}
              >
                <Input />
              </AntForm.Item>
              <AntForm.Item 
                label={profileFields['PassportName']?.Label}
                name='PassportName'
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[
                  {required: profileFields['PassportExpiry']?.FieldRequired, message: `${profileFields['PassportName']?.Label} is required`},
                  { pattern: latin, message: 'The input is not valid name!'}
                ]}
              >
                <Input />
              </AntForm.Item>
              <AntForm.Item 
                label={profileFields['PassportExpiry']?.Label}
                name='PassportExpiry'
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[{required: profileFields['PassportExpiry']?.FieldRequired, message: `${profileFields['PassportExpiry']?.Label} is required`}]}
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
    // {
    //   name: 'NRN',
    // },
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
    if(documentData?.DocumentTag === 'temp'){
      let groupValues = []
      const formData = new FormData();
      Object.keys(values).map((item) => {
        if(!isNaN(item)){
          groupValues.push({GroupMasterId: parseInt(item), GroupDetailId: values[item] ? values[item] : ''})
        }
        if(item !== 'PassportRawImage' && item !== 'SiteContactEmployeeId'){
          if(item === 'Gender'){
            formData.append(item, values[item] ? 1 : 0)
          }
          else if(item === 'CompletionDate' || item === 'CommenceDate' || item === 'Dob'){
            formData.append(item, values[item] ? dayjs(values[item]).format('YYYY-MM-DD') : '')
          }
          else{
            formData.append(item, values[item] ? values[item] : '')
          }
        }
        if(item === 'SiteContactEmployeeId'){
          if(typeof values[item] === 'string'){
            formData.append(item, data?.SiteContactEmployeeId ? data?.SiteContactEmployeeId : '')
          }
          else{
            formData.append(item, values[item] ? values[item] : '')
          }
        }
      })
      formData.append('PassportRawImage', values.PassportRawImage?.file ? values.PassportRawImage?.file : '')
      formData.append('id', state?.userProfileData.Id)
      axios({
        method: 'patch',
        url: 'tas/employee',
        data: formData
      }).then( async (res) => {
        axios({
          method: 'post',
          url: `tas/groupmembers/${state?.userProfileData?.Id}`,
          data: groupValues
        }).then((res) => {
          setIsEdit(false)
          getProfile(state?.userProfileData.Id)
          refreshData()
        }).catch((err) => {
          setSubmitLoading(false)
        })
      }).catch((err) => {
        let errordata = JSON.parse(err.response.data.message)
        if(errordata && errordata.length > 0){
          api.error({
            message: `Data Duplicated`,
            duration: 0,
            description: <div>
              <table className='border-collapse border border-gray-200 text-xs mt-5'>
                <thead>
                  <tr className='text-[11px] font-medium'>
                    <th className='border border-gray-200'></th>
                    <th className='border border-gray-200 px-1'>ADAccount</th>
                    <th className='border border-gray-200 px-1'>SAP ID #</th>
                    <th className='border border-gray-200 px-1'>Mobile</th>
                    {/* <th className='border border-gray-200 px-1'>NRN</th> */}
                  </tr>
                </thead>
                <tbody>
                  {
                    errordata?.map((item) => (
                      <tr>
                        <td className='border border-gray-200 text-center'>
                          <Link to={`/tas/people/search/${item.EmployeeId}`}>
                            <span className='text-blue-500 hover:underline cursor-pointer'>{item.FullName} ({item.EmployeeId})</span>
                          </Link>
                        </td>
                        <td className='border border-gray-200 text-center'>
                          {typeof item.ADAccount === 'boolean' ? item.ADAccount ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                        </td>
                        <td className='border border-gray-200 text-center'>
                          {typeof item.SAPID === 'boolean' ? item.SAPID ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                        </td>
                        <td className='border border-gray-200 text-center'>
                          {typeof item.Mobile === 'boolean' ? item.Mobile ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                        </td>
                        {/* <td className='border border-gray-200 text-center'>
                          {typeof item.NRN === 'boolean' ? item.NRN ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                        </td> */}
                      </tr>
                    ))
                  }
                </tbody>
              </table>
            </div>
          });
        }
      }).then(() => setSubmitLoading(false))
    }else{
      let groupValues = []
      let objData = {};
      Object.keys(values).map((item) => {
        if(!isNaN(item)){
          groupValues.push({GroupMasterId: parseInt(item), GroupDetailId: values[item] ? values[item] : ''})
        }
        if((item !== 'PassportImage') && (item !== 'SiteContactEmployeeId')){
          if(item === 'Gender'){
            objData[item] = values[item]
          }else if(item === 'CompletionDate' || item === 'CommenceDate' || item === 'Dob'){
            objData[item] = values[item] ? dayjs(values[item]).format('YYYY-MM-DD') : ''
          }else{
            objData[item] = values[item] ? values[item] : ''
          }
        }
        else if(item === 'SiteContactEmployeeId'){
          if(typeof values[item] === 'string'){
            objData[item] = data?.SiteContactEmployeeId ? data?.SiteContactEmployeeId : ''
          }
          else{
            objData[item] = values[item] ? values[item] : ''
          }
        }
      })
      axios({
        method: 'put',
        url: 'tas/requestdocumentprofilechange',
        data: {
          ...objData
        },
      }).then( async (res) => {
        axios({
          method: 'post',
          url: `tas/groupmembers/${state?.userProfileData?.Id}`,
          data: groupValues
        }).then((res) => {
          setIsEdit(false)
          refreshData()
        }).catch((err) => {
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
        }
      }).then(() => setSubmitLoading(false))
    }
  }

  const onFailed = (e) => {
    if(e){
      setError(e.errorFields)
    }
  }

  const renderErrors = (fields) => {
    let errorCount = 0;
    if(error){
      error.map((item, i) => {
        if(fields?.find((el) => el.name === item['name'][0])){
          errorCount++;
        }
      })
    }
    return errorCount > 0 && <ErrorTabElement>{errorCount}</ErrorTabElement>
  }

  const items = [
    {
      label: <div className='font-normal'>Personal {renderErrors([...personalFields])}</div>,
      key: 0,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-0'}>
        <FormItemRender key={'t-item-1c'} fields={personalFields} form={form} editData={data}/>
      </div>,
    },
    {
      label: <div>On Site {renderErrors(onSiteFields)}</div>,
      key: 1,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-1'}>
        <FormItemRender key={'t-item-2c'} fields={onSiteFields} form={form} editData={data}/>
      </div>,
    },
    {
      label: <div>Off Site {renderErrors([...offSiteFields, {name: 'PersonalMobile'}])}</div>,
      key: 2,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-2'}>
        <FormItemRender key={'t-item-3c'} fields={offSiteFields} form={form} editData={data}/>
      </div>,
    },
    {
      label: <div>Group {renderErrors(groupValidationFields)}</div>,
      key: 3,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-3'}>
        <FormItemRender key={'t-item-4c'} fields={groupsFields} form={form} editData={data}/>
      </div>,
    },
    {
      label: <div>Citizenship {renderErrors(validationCitizenshipFields())}</div>,
      key: 4,
      forceRender: true,
      children: <div className='grid grid-cols-12 gap-x-5 gap-y-2 mt-5' key={'t-item-4'}>
        <FormItemRender key={'t-item-5c'} fields={citizenshipFields} form={form} editData={data}/>
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
            labelWrap
            onFinish={handleSubmit}
            className='bg-white rounded-lg'
            onFinishFailed={onFailed}
            disabled={!isEdit}
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