import { CloseCircleTwoTone, FileTextOutlined, SaveFilled, UploadOutlined } from '@ant-design/icons'
import { Tabs, Form as AntForm, Input, DatePicker, Select, Row, Col, Button as AntButton, notification, InputNumber, Space, Upload } from 'antd'
import { ErrorTabElement, FormItemRender, Button, Skeleton, Form, GroupLog } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import weekday from "dayjs/plugin/weekday"
import localeData from "dayjs/plugin/localeData"
import axios from 'axios'
import { Link, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import SiteContactEmployeeField from 'components/SiteContactField'
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


function MoreData() {
  const [ currentKey, setCurrentKey ] = useState('0')
  const [ loading, setLoading ] = useState(true)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ error, setError ] =  useState(null)
  const [ isEdit, setIsEdit ] = useState(false)
  const [ profileFields, setProfileFields ] = useState([])
  const [ fieldLoading, setFieldLoading ] = useState(true)
  // const [ managers, setManagers ] = useState([])

  const { employeeId } = useParams()
  const { state, action } = useContext(AuthContext)
  const [api, contextHolder] = notification.useNotification();
  const [ form ] = AntForm.useForm()

  useEffect(() => {
    getProfileFields()
  },[])

  useEffect(() => {
    form.resetFields()
    setIsEdit(false)
  },[employeeId])

  // useEffect(() => {
  //   if(data?.DepartmentId){
  //     getDepartmentManagers()
  //   }
  // },[data?.DepartmentId])

  useEffect(() => {
    if(state.userProfileData){
      initFormData(state.userProfileData)
    }
  },[state.userProfileData])

  const initFormData = (data) => {
    form.setFieldsValue({
      ...data,
      Dob: data.Dob ? dayjs(data.Dob) : null,
      CommenceDate: data.CommenceDate ? dayjs(data.CommenceDate) : null,
      CompletionDate: data.CompletionDate ? dayjs(data.CompletionDate) : null,
      NextRosterDate: data.NextRosterDate ? dayjs(data.NextRosterDate) : null,
      PassportExpiry: data.PassportExpiry ? dayjs(data.PassportExpiry) : null,
    })

    data.GroupData?.map((group) => {
      form.setFieldValue(group.GroupMasterId, group.GroupDetailId)
    })
    setLoading(false)
  }

  const initialValues = useMemo(() => {
    if(state.userProfileData){
      return {
        ...state.userProfileData,
        Dob: state.userProfileData.Dob ? dayjs(state.userProfileData.Dob) : null,
        CommenceDate: state.userProfileData.CommenceDate ? dayjs(state.userProfileData.CommenceDate) : null,
        CompletionDate: state.userProfileData.CompletionDate ? dayjs(state.userProfileData.CompletionDate) : null,
        NextRosterDate: state.userProfileData.NextRosterDate ? dayjs(state.userProfileData.NextRosterDate) : null,
        PassportExpiry: state.userProfileData.PassportExpiry ? dayjs(state.userProfileData.PassportExpiry) : null,
      }
    }
  },[state.userProfileData])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getProfileFields = () => {
    axios({
      method: 'get',
      url: `tas/profilefield`,
    }).then((res) => {
      let tmp = {}
      res.data.map((item) => {
        tmp[item.ColumnName] = item
      })
      setProfileFields(tmp)
    }).catch(() => {

    }).then(() => setFieldLoading(false))
  }

  // const getDepartmentManagers = () => {
  //   axios({
  //     method: 'get',
  //     url: `tas/department/${data?.DepartmentId}`,
  //   }).then((res) => {
  //     setManagers(res.data.Managers?.map((item) => ({label: item.FullName, value: item.EmployeeId, ...item})))
  //   }).catch((err) => {

  //   })
  // }

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

  const validateDoB = (rule, value, callback) => {
    if (value) {
      if (dayjs().diff(value, 'year', true) >= 18) {
        callback();
      } else {
        callback(`Must be at least 18 years old`);
      }
    } else {
      callback();
    }
  };

  const personalFields = [
    {
      label: profileFields['NationalityId']?.Label,
      name: 'NationalityId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      labelCol: {flex: '150px'},
      rules: [{required: profileFields['NationalityId']?.FieldRequired, message: `${profileFields['NationalityId']?.Label} is required`}],
      inputprops: {
        options: state.referData.nationalities,
      }
    },
    {
      type: 'component',
      name: 'Gender',
      component:  <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NRN !== curValues.NRN || prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue, setFieldValue}) => {
          // const nrn = getFieldValue('NRN');
          // let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
          // if(selectedNationality?.Code === 'MN' && (nrn?.length === 10)){
          //   const gender = mnRegister.getGender(nrn)
          //   setFieldValue('Gender', gender)
          // }
          return(
            <AntForm.Item 
              label={profileFields['Gender']?.Label}
              name='Gender'
              labelCol={{flex: '150px'}}
              className='col-span-12 lg:col-span-6 mb-0'
              rules={[{required: profileFields['Gender']?.FieldRequired, message: `${profileFields['Gender']?.Label} is required`}]}
            >
              <Select options={[{value: 1, label: 'Male'}, {value: 0, label: 'Female'}]} />
            </AntForm.Item>
          )
        }}
      </AntForm.Item>
    },
    {
      type: 'component',
      name: 'NRN',
      component:  <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue}) => {
          let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
          return( selectedNationality?.Code === 'MN' &&
            <AntForm.Item 
              label={profileFields['NRN']?.Label}
              name='NRN'
              className='col-span-12 lg:col-span-6 mb-0'
              labelCol={{flex: '150px'}}
              rules={
                [
                  {required: profileFields['NRN']?.FieldRequired, message: `${profileFields['NRN']?.Label} is required`},
                  {pattern: cyrillic, message: 'Please use only cyrillic characters'},
              ]
              }
            >
              <Input maxLength={10} onInput={(e) => e.target.value = e.target.value.toUpperCase()}/>
            </AntForm.Item>
          )
        }}
      </AntForm.Item>
    },
    {
      type: 'component',
      name: 'Dob',
      component:  <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NRN !== curValues.NRN || prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue, setFieldValue}) => {
          let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
          if(selectedNationality?.Code === 'MN' && (getFieldValue('NRN')?.length === 10) && !getFieldValue('Dob')){
            setFieldValue('Dob', dayjs(mnRegister.getDob(getFieldValue('NRN')))) 
          }
          return(
            <AntForm.Item 
              label={profileFields['Dob']?.Label}
              name='Dob'
              labelCol={{flex: '150px'}}
              className='col-span-12 lg:col-span-6 mb-0'
              // rules={[{required: profileFields['Dob']?.FieldRequired, message: `${profileFields['Dob']?.Label} is required`}, { validator: validateDoB}]}
              rules={[{required: profileFields['Dob']?.FieldRequired, message: `${profileFields['Dob']?.Label} is required`}]}
            >
              <DatePicker />
            </AntForm.Item>
          )
        }}
      </AntForm.Item>
    },
    {
      label: profileFields['Firstname']?.Label,
      name: 'Firstname',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['Firstname']?.FieldRequired, message: `${profileFields['Firstname']?.Label} is required`}],
    },
    {
      label: profileFields['Lastname']?.Label,
      name: 'Lastname',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['Lastname']?.FieldRequired, message: `${profileFields['Lastname']?.Label} is required`}],
    },
    {
      label: profileFields['MFirstname']?.Label,
      name: 'MFirstname',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['MFirstname']?.FieldRequired, message: `${profileFields['MFirstname']?.Label} is required`}],
    },
    {
      label: profileFields['MLastname']?.Label,
      name: 'MLastname',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['MLastname']?.FieldRequired, message: `${profileFields['MLastname']?.Label} is required`}],
    },
    {
      label: profileFields['CommenceDate']?.Label,
      name: 'CommenceDate',
      labelCol: {flex: '150px'},
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
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['CompletionDate']?.FieldRequired, message: `${profileFields['CompletionDate']?.Label} is required`}],
      type: 'date',
      inputprops: {
        format: 'M/D/YYYY'
      }
    },
    {
      label: profileFields['DepartmentId']?.Label,
      name: 'DepartmentId',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['DepartmentId']?.FieldRequired, message: `${profileFields['DepartmentId']?.Label} is required`}],
      type: 'treeSelect',
      inputprops: {
        treeData: state.referData.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        filterTreeNode: (input, node) => (node?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
        allowClear: true,
        showSearch: true,
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.DepartmentId !== cur.DepartmentId}>
        {({getFieldValue, setFieldValue}) => {
          let selectedDepartment = state.referData.departments?.find((department) => department.Id === getFieldValue('DepartmentId'))
          if(selectedDepartment && selectedDepartment.CostCodeId){
            setFieldValue('CostCodeId', selectedDepartment.CostCodeId)
          }
          return (
            <Form.Item 
              name='CostCodeId' 
              label={profileFields['CostCodeId']?.Label} 
              labelCol={{flex: '150px'}}
              rules={[{required: profileFields['CostCodeId']?.FieldRequired, message: `${profileFields['CostCodeId']?.Label} is required`}]}
              className='col-span-12 lg:col-span-6 mb-0'
            >
              <Select
                options={state.referData.costCodes}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      label: profileFields['StateId']?.Label,
      name: 'StateId',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['StateId']?.FieldRequired, message: `${profileFields['StateId']?.Label} is required`}],
      inputprops: {
        options: state.referData.states,
      }
    },
    {
      label: profileFields['Email']?.Label,
      name: 'Email',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [
        {required: profileFields['Email']?.FieldRequired, message: `${profileFields['Email']?.Label} is required`},
        {type: 'email', message: `The input is not valid E-mail!`},
      ],
    },
    {
      type: 'component',
      component: <AntForm.Item  labelCol={{flex: '150px'}} className='col-span-12 lg:col-span-6 mb-0' label={'ADAccount'} key={'component-ADAccount'}>
        <Row gutter={8}>
          <Col span={12}>
            <AntForm.Item 
              name="ADAccount"
              className='mb-0'
              // rules={[{required: profileFields['ADAccount']?.FieldRequired, message: 'ADAccount is required'}]}
            >
              <Input />
            </AntForm.Item>
          </Col>
          <Col span={12}>
            <AntButton htmlType='button' onClick={handleCheckAccount}>Account Check</AntButton>
          </Col>
        </Row>
      </AntForm.Item>
    },
    {
      label: 'Camp',
      name: 'CampId',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      // rules: [{required: profileFields['CampId']?.FieldRequired, message: `${profileFields['CampId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.camps,
      }
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      depindex: 'CampId',
      inputprops: {
        optionsurl: 'tas/roomtype?active=1&campId=',
        optionvalue: 'Id', 
        optionlabel: 'Description',
      }
    },
    state.userInfo?.Role === 'SystemAdmin' &&
    {
      label: 'Create Request',
      name: 'CreateRequest',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
    },
  ]

  // function fetchUserList(value) {
  //   return axios({
  //     method: 'post',
  //     url: 'tas/employee/searchshort',
  //     data: {
  //       model: {
  //         keyWord: value
  //       },
  //       pageIndex: 0,
  //       pageSize: 100
  //     }
  //   }).then((res) => 
  //     res.data.data.map((user) => ({
  //       label: `${user.Firstname} ${user.Lastname} /${user.Id}/`,
  //       value: user.Id,
  //     })),
  //   )
  // }

  const onSiteFields = [
    {
      label: profileFields['EmployerId']?.Label,
      name: 'EmployerId',
      labelCol: {flex: '150px'},
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
      labelCol: {flex: '150px'},
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
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['SAPID']?.FieldRequired, message: `${profileFields['SAPID']?.Label} is required`}],
      inputprops: {
        maxLength: 8,
      }
    },
    {
      label: profileFields['ContractNumber']?.Label,
      name: 'ContractNumber',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [{required: profileFields['ContractNumber']?.FieldRequired, message: `${profileFields['ContractNumber']?.Label} is required`}],
    },
    {
      label: 'Person #',
      name: 'Id',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      inputprops: {
        disabled: true,
      }
    },
    {
      label: profileFields['PositionId']?.Label,
      name: 'PositionId',
      labelCol: {flex: '150px'},
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
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
    },
    {
      label: profileFields['HotelCheck']?.Label,
      name: 'HotelCheck',
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
      rules: [{required: profileFields['HotelCheck']?.FieldRequired, message: `${profileFields['HotelCheck']?.Label} is required`}],
    },
    {
      label: profileFields['RosterId']?.Label,
      name: 'RosterId',
      labelCol: {flex: '150px'},
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
      labelCol: {flex: '150px'},
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
      labelCol: {flex: '150px'},
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
      labelCol: {flex: '150px'},
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['FlightGroupMasterId']?.FieldRequired, message: `${profileFields['FlightGroupMasterId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.transportGroups,
      }
    },
    // {
    //   label: profileFields['SiteContactEmployeeId']?.Label,
    //   name: 'SiteContactEmployeeId',
    //   className: 'col-span-12 lg:col-span-6 mb-0',
    //   type: managers.length > 0 ? 'select' : 'searchSelect',
    //   rules: [{required: profileFields['SiteContactEmployeeId']?.FieldRequired, message: `${profileFields['SiteContactEmployeeId']?.Label} is required`}],
    //   inputprops: {
    //     options: managers,
    //     fetchOptions: fetchUserList,
    //     showSearch: true,
    //     editName: ['SiteContactEmployeeFirstname', 'SiteContactEmployeeLastname'],
    //     placeholder: 'Select user',
    //     allowClear: true,
    //   }
    // },
    {
      type: 'component',
      component: <SiteContactEmployeeField profileFields={profileFields} form={form}/>
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
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.PeopleTypeId !== cur.PeopleTypeId}>
        {({getFieldValue}) => {
          const PeopleTypeId = getFieldValue('PeopleTypeId')
          const isVisitor = state.referData.peopleTypes?.find((item) => item.value === PeopleTypeId)?.Code === 'Visitor' 
          return(
            <Form.Item
              className='col-span-6 mb-0'
              name='PersonalMobile'
              label={profileFields['PersonalMobile']?.Label}
              rules={[
                {required: isVisitor ? false : profileFields['PersonalMobile']?.FieldRequired, message: `${profileFields['PersonalMobile']?.Label} is required`},
                {pattern: phoneNumber, message: `${profileFields['PersonalMobile']?.Label} must be 8 digit`}
              ]}
            >
              <InputNumber controls={false} prefix='+976' className='w-full' maxLength={8}/>
            </Form.Item>
          )
        }}
      </Form.Item>,
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
              <AntForm.Item key={i} label={<div>{item.Required ? <span className='text-red-500'>* </span> : null}{item.Description}</div>} labelCol={{flex: '130px'}} className='col-span-12 lg:col-span-7 mb-0'>
                <Row gutter={8}>
                  <Col span={12}>
                    <AntForm.Item
                      name={item.Id}
                      className='mb-0 col-span-12 lg:col-span-7'
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
                  </Col>
                  {
                    item.CreateLog ? 
                    <Col span={12}>
                      <GroupLog groupId={item.Id} employeeId={employeeId} name={item.Description}/>
                    </Col>
                    : null
                  }
                </Row>
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
          {
            label: 'National Registration Number',
            name: 'NRN',
            rules: [{required: true, message: 'National registration number is required'}],
          },
        ]
      }
    }
    else {
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
        {
          label: 'National Registration Number',
          name: 'NRN',
          rules: [{required: true, message: 'National registration number is required'}],
        },
      ]
    }
  } 

  const citizenshipFields = [
    {
      type: 'component',
      component: <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue}) => {
          let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
          return selectedNationality?.Code !== 'MN' &&
            <>
              <AntForm.Item 
                label={profileFields['PassportNumber']?.Label}
                name='PassportNumber'
                labelCol={{flex: '150px'}}
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[{required: profileFields['PassportExpiry']?.FieldRequired, message: `${profileFields['PassportNumber']?.Label} is required`}]}
              >
                <Input />
              </AntForm.Item>
              <AntForm.Item 
                label={profileFields['PassportName']?.Label}
                name='PassportName'
                labelCol={{flex: '150px'}}
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[
                  {required: profileFields['PassportExpiry']?.FieldRequired, message: `${profileFields['PassportName']?.Label} is required`},
                  { pattern: latin, message: 'The input is not valid name!'}
                ]}
              >
                <Input />
              </AntForm.Item>
              <AntForm.Item 
                label={profileFields['Hometown']?.Label}
                name='Hometown'
                labelCol={{flex: '150px'}}
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[{required: profileFields['Hometown']?.FieldRequired, message: `${profileFields['Hometown']?.Label} is required`}]}
              >
                <Input.TextArea maxLength={50} />
              </AntForm.Item>
              <AntForm.Item 
                label={profileFields['PassportExpiry']?.Label}
                name='PassportExpiry'
                labelCol={{flex: '150px'}}
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
      labelCol: {flex: '150px'},
      type: 'image',
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
        else if(item === 'CommenceDate' || item === 'CompletionDate' || item === 'Dob' || item === 'NextRosterDate' || item === 'PassportExpiry'){
          formData.append(item, values[item] ? dayjs(values[item]).format('YYYY-MM-DD') : '')
        }
        // else if(item === 'CompletionDate'){
        //   formData.append(item, values[item] ? dayjs(values[item]).format('YYYY-MM-DD') : '')
        // }
        else{
          formData.append(item, values[item] ? values[item] : '')
        }
      }
      else if(item === 'SiteContactEmployeeId'){
        if(typeof values[item] === 'string'){
          formData.append(item, values[item] || '')
        }
        else{
          formData.append(item, values[item] || '')
        }
      }
    })
    formData.append('PassportRawImage', values.PassportRawImage?.file ? values.PassportRawImage?.file : '')
    formData.append('id', state.userProfileData.Id)
    if(state.userInfo?.Role === 'DataApproval' && state.userProfileData.Active === 0){
      formData.set('Active', 1)
    }
    formData.append('id', state.userProfileData.Id)
    axios({
      method: 'patch',
      url: 'tas/employee',
      data: formData
    }).then( async (res) => {
      axios({
        method: 'post',
        url: `tas/groupmembers/${employeeId}`,
        data: groupValues
      }).then((res) => {
        setIsEdit(false)
        getData()
        setSubmitLoading(false)
      }).catch((err) => {
        setSubmitLoading(false)
      })
    }).catch((err) => {
      setSubmitLoading(false)
      if(err.response.data.message[0] === '['){
        let errordata = JSON.parse(err.response.data.message)
        if(errordata?.length > 0){
          api.error({
            message: `Data Duplicated`,
            duration: 10,
            description: <div>
              <table className='border-collapse border border-gray-200 text-xs mt-5'>
                <thead>
                  <tr className='text-[11px] font-medium'>
                    <th className='border border-gray-200'></th>
                    <th className='border border-gray-200 px-1'>ADAccount</th>
                    <th className='border border-gray-200 px-1'>SAP ID #</th>
                    <th className='border border-gray-200 px-1'>Mobile</th>
                    <th className='border border-gray-200 px-1'>NRN</th>
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
                        <td className='border border-gray-200 text-center'>
                          {typeof item.NRN === 'boolean' ? item.NRN ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                        </td>
                      </tr>
                    ))
                  }
                </tbody>
              </table>
            </div>
          });
        }
      }else if(err.response.data.message){
        api.error({
          message: 'Operation Failed',
          duration: 5,
          description: err.response.data.message
        })
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
      label: <div className='font-normal'>Personal {renderErrors([...personalFields, {name: 'NRN'}])}</div>,
      key: '0',
      forceRender: true,
      children: <div className='py-4 grid grid-cols-12 gap-x-10 gap-y-2' key={'t-item-0'}>
        <FormItemRender key={'t-item-1c'} fields={personalFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
    {
      label: <div>On Site {renderErrors(onSiteFields)}</div>,
      key: '1',
      forceRender: true,
      children: <div className='py-4 grid grid-cols-12 gap-x-10 gap-y-2' key={'t-item-1'}>
        <FormItemRender key={'t-item-2c'} fields={onSiteFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
    {
      label: <div>Off Site {renderErrors([...offSiteFields, {name: 'PersonalMobile'}])}</div>,
      key: '2',
      forceRender: true,
      children: <div className='py-4 grid grid-cols-12 gap-x-10 gap-y-2' key={'t-item-2'}>
        <FormItemRender key={'t-item-3c'} fields={offSiteFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
    {
      label: <div>Group {renderErrors(groupValidationFields)}</div>,
      key: '3',
      forceRender: true,
      children: <div className='py-4 grid grid-cols-12 gap-x-10 gap-y-2' key={'t-item-3'}>
        <FormItemRender key={'t-item-4c'} fields={groupsFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
    {
      label: <div>Citizenship {renderErrors(validationCitizenshipFields())}</div>,
      key: '4',
      forceRender: true,
      children: <div className='py-4 grid grid-cols-12 gap-x-10 gap-y-2' key={'t-item-4'}>
        <FormItemRender key={'t-item-5c'} fields={citizenshipFields} form={form} editData={state.userProfileData}/>
      </div>,
    },
  ]

  const handleCancel = () => {
    setIsEdit(false)
    initFormData(state.userProfileData)
  }

  return (
    <div className='col-span-12'>
      <div className='bg-white rounded-ot p-4 col-span-12 shadow-md mb-1'>
        {
          fieldLoading ?
          <Skeleton className='h-[400px]'/>
          :
          <>
            <AntForm
              form={form}
              {...formItemLayout}
              labelAlign='left'
              onFinish={handleSubmit}
              className='bg-white rounded-lg'
              onFinishFailed={onFailed}
              disabled={!isEdit}
              initialValues={initialValues}
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
                  <Button type='primary' loading={submitLoading} onClick={() => form.submit()} icon={<SaveFilled/>}>Save</Button>
                  <Button onClick={handleCancel} disabled={submitLoading}>Cancel</Button>
                </div>
                :
                <Button onClick={() => setIsEdit(true)}>Edit</Button>
                )
                :
                null    
              }
            </div>
          </>
        }
      </div>
      {contextHolder}
    </div>
  )
}

export default MoreData