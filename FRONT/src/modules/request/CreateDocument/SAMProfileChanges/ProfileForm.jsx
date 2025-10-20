import { CloseCircleTwoTone, SaveFilled } from '@ant-design/icons'
import { Tabs, Form as AntForm, Input, DatePicker, Select, Row, Col, Button as AntButton, notification, message, InputNumber } from 'antd'
import { ErrorTabElement, FormItemRender, Button, Table } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import weekday from "dayjs/plugin/weekday"
import localeData from "dayjs/plugin/localeData"
import axios from 'axios'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import mnRegister from 'utils/mnRegister'

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


function ProfileForm({form}) {
  const [ currentKey, setCurrentKey ] = useState(0)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ error, setError ] =  useState(null)
  const [ isEdit, setIsEdit ] = useState(false)
  const [ employees, setEmployees ] = useState([])
  const [ isInitedData, setIsInitedData ] = useState(0)
  const [ initData, setInitData ] = useState(null)

  // const [form] = AntForm.useForm()
  const { employeeId } = useParams()
  const { state, action } = useContext(AuthContext)
  const [api, contextHolder] = notification.useNotification();

  const navigate = useNavigate()
  const { empId } = useParams()

  useEffect(() => {
    form.resetFields()
    setIsEdit(false)
  },[])

  useEffect(() => {
    if(state.userProfileData){
      let initValues = {
        ...state.userProfileData,
        Dob: state.userProfileData?.Dob ? dayjs(state.userProfileData?.Dob) : null,
        CommenceDate: state.userProfileData?.CommenceDate ? dayjs(state.userProfileData?.CommenceDate) : null,
        CompletionDate: state.userProfileData?.CompletionDate ? dayjs(state.userProfileData?.CompletionDate) : null,
        NextRosterDate: state.userProfileData?.NextRosterDate ? dayjs(state.userProfileData?.NextRosterDate) : null,
        PassportExpiry: state.userProfileData?.PassportExpiry ? dayjs(state.userProfileData?.PassportExpiry) : null,
      }
      initValues.GroupData?.map((group) => {
        initValues[group.GroupMasterId] = group.GroupDetailId
      })
      setInitData(initValues)
      form.setFieldsValue(initValues)
    }
  },[state.userProfileData])

  const handleCheckAccount = () => {
    axios({
      method: 'post',
      url: `tas/employee/checkadaccount`,
      data: {
        employeeId: employeeId,
        adAccount: form.getFieldValue('ADAccount')
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

    })
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
      label: state.referData?.profileFields['Email']?.Label,
      name: 'Email',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [
        {required: state.referData?.profileFields['Email']?.FieldRequired, message: `${state.referData?.profileFields['Email']?.Label} is required`},
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
    //           label={state.referData?.profileFields['NRN']?.Label}
    //           name='NRN'
    //           className='col-span-12 lg:col-span-6 mb-0'
    //           rules={[
    //             {required: state.referData?.profileFields['NRN']?.FieldRequired, message: `${state.referData?.profileFields['NRN']?.Label} is required`},
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
      type: 'component',
      component: <AntForm.Item 
        label={state.referData?.profileFields['Dob']?.Label}
        name='Dob'
        className='col-span-12 lg:col-span-6 mb-0'
        rules={[{required: state.referData?.profileFields['Dob']?.FieldRequired, message: `${state.referData?.profileFields['Dob']?.Label} is required`}]}
      >
        <DatePicker />
      </AntForm.Item>
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
      type: 'component',
      component: <AntForm.Item className='col-span-12 lg:col-span-6 mb-0' label='AD Account' key={'component-ADAccount'}>
        <Row gutter={8}>
          <Col span={12}>
            <AntForm.Item 
              name="ADAccount"
              className='mb-0'
              // rules={[{required: state.referData?.profileFields['ADAccount']?.FieldRequired, message: 'ADAccount is required'}]}
            >
              <Input />
            </AntForm.Item>
          </Col>
          <Col span={12}>
            <AntButton htmlType='button' disabled={!isEdit} onClick={handleCheckAccount}>Account Check</AntButton>
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
    }).then((res) => {
       return res.data.data.map((user) => ({
          label: `${user.Firstname} ${user.Lastname} /${user.Mobile}/`,
          value: user.Id,
        }))
        
      }
    )
  }

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
        disabled: !isEdit || state.userProfileData?.PeopleTypeId ? true : false
      }
    },
    {
      label: state.referData?.profileFields['SAPID']?.Label,
      name: 'SAPID',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: state.referData?.profileFields['SAPID']?.FieldRequired, message: `${state.referData?.profileFields['SAPID']?.Label} is required`}],
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
      label: 'Active',
      name: 'Active',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
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
    // {
    //   label: state.referData?.profileFields['RoomId']?.Label,
    //   name: 'RoomId',
    //   className: 'col-span-12 lg:col-span-6 mb-0',
    //   type: 'select',
    //   rules: [{required: state.referData?.profileFields['RoomId']?.FieldRequired, message: `${state.referData?.profileFields['RoomId']?.Label} is required`}],
    //   inputprops: {
    //     options: state.referData?.rooms,
    //   }
    // },
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

  const groupsFields = [
    {
      type: 'component',
      component: <>
          {
            state.referData?.fieldsOfGroups?.map((item, i) => (
              <AntForm.Item name={item.Id} label={item.Description} className='col-span-12 lg:col-span-6 mb-0' key={`g-fields-${i}`}>
                <Select
                  showSearch
                  allowClear
                  options={item.details}
                  filterOption={(input, option) => (option?.Description ?? '').toLowerCase().includes(input.toLowerCase())}
                  fieldNames={{value: 'Id', label: 'Description'}}
                />
              </AntForm.Item>
            ))
          }
        </>
    },
  ]

  const validationCitizenshipFields = () => {
    if(form.getFieldValue('NationalityId')){
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
            rules: [
              {required: true, message: 'Name on password is required'},
              { pattern: latin, message: 'The input is not valid name!'}
            ],
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
          // {
          //   label: 'National Registration Number',
          //   name: 'NRN',
          //   rules: [{required: true, message: 'National registration number is required'}],
          // },
        ]
      }
    }
    else {
      return [
        // {
        //   label: 'Passport Number',
        //   name: 'PassportNumber',
        //   rules: [{required: true, message: 'Passport Number is required'}],
        // },
        // {
        //   label: 'Name On Passport',
        //   name: 'PassportName',
        //   rules: [{required: true, message: 'Name on password is required'}],
        // },
        // {
        //   label: 'Passport Expiry Date',
        //   name: 'PassportExpiry',
        //   rules: [{required: true, message: 'Password expiry date is required'}],
        // },
        // {
        //   label: 'National Registration Number',
        //   name: 'NRN',
        //   rules: [{required: true, message: 'National registration number is required'}],
        // },
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
                  {required: state.referData?.profileFields['PassportExpiry']?.FieldRequired, message: `${state.referData?.profileFields['PassportName']?.Label} is required`},
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
    // {
    //   label: 'Passport Image',
    //   name: 'PassportImage',
    //   className: 'col-span-12 lg:col-span-7 mb-0',
    //   type: 'image',
    // },
  ]

  const citizenshipFieldList = [
    {
      name: 'PassportNumber',
    },
    {
      name: 'PassportName',
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

  const handleSubmit = (values) => {
    setError(null)
    setIsEdit(false)
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
      children: <div className='pt-4 grid grid-cols-12 gap-x-5 gap-y-2' key={'t-item-0'}>
        <FormItemRender 
          key={'t-item-1c'} 
          fields={personalFields} 
          form={form} 
          // editData={initData}
          // isInitedData={isInitedData}
          handleInitStatus={setIsInitedData}
        />
      </div>,
    },
    {
      label: <div>On Site {renderErrors(onSiteFields)}</div>,
      key: 1,
      forceRender: true,
      children: <div className='pt-4 grid grid-cols-12 gap-x-5 gap-y-2' key={'t-item-1'}>
        <FormItemRender 
          key={'t-item-2c'} 
          fields={onSiteFields} 
          form={form} 
          // editData={initData} 
          // isInitedData={isInitedData}
          handleInitStatus={setIsInitedData}
        />
      </div>,
    },
    {
      label: <div>Off Site {renderErrors([...offSiteFields, {name: 'PersonalMobile'}])}</div>,
      key: 2,
      forceRender: true,
      children: <div className='pt-4 grid grid-cols-12 gap-x-5 gap-y-2' key={'t-item-2'}>
        <FormItemRender 
          key={'t-item-3c'} 
          fields={offSiteFields} 
          form={form} 
          // editData={initData}
          // isInitedData={isInitedData}
          handleInitStatus={setIsInitedData}
        />
      </div>,
    },
    // {
    //   label: <div>Group</div>,
    //   key: 3,
    //   forceRender: true,
    //   children: <div className='pt-4 grid grid-cols-12 gap-x-5 gap-y-2' key={'t-item-3'}>
    //     <FormItemRender 
    //       key={'t-item-4c'} 
    //       fields={groupsFields} 
    //       form={form}
    //       // editData={initData}
    //       // isInitedData={isInitedData}
    //       handleInitStatus={setIsInitedData}
    //     />
    //   </div>,
    // },
    {
      label: <div>Citizenship {renderErrors(validationCitizenshipFields())}</div>,
      key: 4,
      forceRender: true,
      children: <div className='pt-4 grid grid-cols-12 gap-x-5 gap-y-2' key={'t-item-4'}>
        <FormItemRender 
          key={'t-item-5c'} 
          fields={citizenshipFields} 
          form={form} 
          // editData={initData}
          // isInitedData={isInitedData}
          handleInitStatus={setIsInitedData}
        />
      </div>,
    },
  ]

  const handleCancel = () => {
    setIsEdit(false)
    form.setFieldsValue(initData)
  }

  return (
    <>
      {
        state.customLoading ?
        <div className='animate-skeleton h-[400px] flex rounded-ot shadow-md col-span-12'></div>
        :
        <div className='bg-white rounded-ot p-4 col-span-12 mb-1'>
          <AntForm 
            form={form} 
            {...formItemLayout} 
            labelAlign='left'
            onFinish={handleSubmit}
            className='bg-white rounded-lg'
            onFinishFailed={onFailed}
            disabled={!isEdit}
            size='middle'
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
              !state.userInfo?.ReadonlyAccess ?
              (isEdit ?
              <div className='flex items-center gap-3'>
                <Button 
                  type='primary' 
                  loading={submitLoading} 
                  onClick={() => form.submit()} 
                  // icon={<SaveFilled/>}
                >
                  Done
                </Button>
                <Button 
                  onClick={handleCancel} 
                  disabled={submitLoading}
                >
                  Cancel
                </Button>
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