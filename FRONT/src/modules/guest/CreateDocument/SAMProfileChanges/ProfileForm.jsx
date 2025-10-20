import { SaveFilled } from '@ant-design/icons'
import { Tabs, Form as AntForm, Input, DatePicker, Select, Row, Col, Button as AntButton, notification } from 'antd'
import { ErrorTabElement, FormItemRender, Button } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import weekday from "dayjs/plugin/weekday"
import localeData from "dayjs/plugin/localeData"
import axios from 'axios'
import { Link, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'

dayjs.extend(weekday)
dayjs.extend(localeData)

const cyrillic = new RegExp(/^[А-ЯҮӨ]{2}\d{8}$/)
const phoneNumber = new RegExp(/^[0-9]{8}$/)
const internationalPhone = new RegExp(/^[0-9 +]+$/)
const latin = new RegExp(/^[A-Za-z -]+$/g)

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
  const [ loading, setLoading ] = useState(true)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ error, setError ] =  useState(null)
  const [ isEdit, setIsEdit ] = useState(false)
  const [ isInitedData, setIsInitedData ] = useState(0)
  const [ initData, setInitData ] = useState(null)
  const [ profileFields, setProfileFields ] = useState([])
  const [ fieldLoading, setFieldLoading ] = useState(true)

  const { employeeId } = useParams()
  const { state } = useContext(AuthContext)
  const [api, contextHolder] = notification.useNotification();


  useEffect(() => {
    form.resetFields()
    setIsEdit(false)
    getProfileFields()
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
      type: 'component',
      component: <AntForm.Item 
        label={profileFields['Dob']?.Label}
        name='Dob'
        className='col-span-12 lg:col-span-6 mb-0'
        rules={[{required: profileFields['Dob']?.FieldRequired, message: `${profileFields['Dob']?.Label} is required`}]}
      >
        <DatePicker />
      </AntForm.Item>
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
              // rules={[{required: profileFields['ADAccount']?.FieldRequired, message: 'ADAccount is required'}]}
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
    {
      label: profileFields['RoomId']?.Label,
      name: 'RoomId',
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['RoomId']?.FieldRequired, message: `${profileFields['RoomId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.rooms,
      }
    },
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
        { required: profileFields['EmergencyContactMobile']?.FieldRequired, message: `${profileFields['EmergencyContactMobile']?.Label} is required` },
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
                  { pattern: latin, message: 'The input is not valid name!' }
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
    {
      label: <div>Group</div>,
      key: 3,
      forceRender: true,
      children: <div className='pt-4 grid grid-cols-12 gap-x-5 gap-y-2' key={'t-item-3'}>
        <FormItemRender 
          key={'t-item-4c'} 
          fields={groupsFields} 
          form={form}
          // editData={initData}
          // isInitedData={isInitedData}
          handleInitStatus={setIsInitedData}
        />
      </div>,
    },
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

  return (
    <>
      {
        state.customLoading ?
        <div className='animate-skeleton h-[400px] flex rounded-ot shadow-md col-span-12'></div>
        :
        <div className='bg-white rounded-ot p-4 col-span-12 shadow-md mb-1'>
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
              isEdit ?
              <div className='flex items-center gap-3'>
                <Button 
                  type='primary' 
                  loading={submitLoading} 
                  onClick={() => form.submit()} 
                  icon={<SaveFilled/>}
                >
                  Save
                </Button>
                <Button 
                  onClick={() => setIsEdit(false)} 
                  disabled={submitLoading}
                >
                  Cancel
                </Button>
              </div>
              :
              <Button onClick={() => setIsEdit(true)}>Edit</Button>
            }
          </div>
        </div>
      }
      {contextHolder}
    </>
  )
}

export default ProfileForm