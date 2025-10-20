import { CloseCircleTwoTone, SaveFilled } from '@ant-design/icons'
import { Tabs, Form as AntForm, DatePicker, Input, notification, Select, Row, Col } from 'antd'
import { ErrorTabElement, Button, FormItemRender, Skeleton } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import axios from 'axios'
import { Link, useNavigate } from 'react-router-dom'
import { AuthContext } from 'contexts'
import mnRegister from 'utils/mnRegister'
import dayjs from 'dayjs'

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
    name: 'PassportRawImage',
    type: 'image',
  },
]
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

const cyrillic = new RegExp(/^[А-ЯҮӨ]{2}\d{8}$/)
const phoneNumber = new RegExp(/^[0-9]{8}$/)
const latin = new RegExp(/^[A-Za-z -]+$/g)
const internationalPhone = new RegExp(/^[0-9 +]+$/)

function AddPerson({getData, changeMode, navigateUrl}) {
  const [ currentKey, setCurrentKey ] = useState(0)
  const [ loading, setLoading ] = useState(false)
  const [ profileFields, setProfileFields ] = useState([])
  const [ fieldLoading, setFieldLoading ] = useState(false)
  const [ error, setError ] = useState(null);
  const navigate = useNavigate();
    
  const [form] = AntForm.useForm()
  const [api, contextHolder] = notification.useNotification();
  const { state } = useContext(AuthContext)

  useEffect(() => {
    getProfileFields()
  },[])

  const getProfileFields = () => {
    setFieldLoading(true)
    axios({
      method: 'get',
      url: `tas/profilefield`,
    }).then((res) => {
      let tmp = {}
      res.data.map((item) => {
        tmp[item.ColumnName] = item
      })
      setProfileFields(tmp)
      form.setFieldValue('HotelCheck', 1)
    }).catch(() => {

    }).then(() => setFieldLoading(false))
  }

  const handleCheckAccount = () => {
    axios({
      method: 'post',
      url: `tas/employee/checkadaccount`,
      data: {
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
      type: 'component',
      component:  <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NRN !== curValues.NRN || prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue, setFieldValue}) => {
          let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
          if(selectedNationality?.Code === 'MN' && (getFieldValue('NRN')?.length === 10)){
            setFieldValue('Gender', mnRegister.getGender(getFieldValue('NRN'))) 
          }
          return(
            <AntForm.Item 
              label={profileFields['Gender']?.Label}
              name='Gender'
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
      component:  <AntForm.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.NationalityId !== curValues.NationalityId }>
        {({getFieldValue}) => {
          let selectedNationality = state.referData.nationalities.find((item) => item.value === getFieldValue('NationalityId'))
          return( selectedNationality?.Code === 'MN' &&
            <AntForm.Item 
              label={profileFields['NRN']?.Label}
              name='NRN'
              className='col-span-12 lg:col-span-6 mb-0'
              rules={[
                {required: profileFields['NRN']?.FieldRequired, message: `${profileFields['NRN']?.Label} is required`},
                {pattern: cyrillic, message: `Please use only cyrillic characters`}
              ]}
            >
              <Input maxLength={10} onInput={(e) => e.target.value = e.target.value.toUpperCase()}/>
            </AntForm.Item>
          )
        }}
      </AntForm.Item>
    },
    {
      type: 'component',
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
              className='col-span-12 lg:col-span-6 mb-0'
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
      rules: [{required: false, message: `Completion Date is required`}],
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
      label: profileFields['Email']?.Label,
      name: 'Email',
      className: 'col-span-12 lg:col-span-6 mb-0',
      rules: [
        {required: profileFields['Email']?.FieldRequired, message: `${profileFields['Email']?.Label} is required`},
        {type: 'email', message: `The input is not valid E-mail!`}
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
              // rules={[{required: profileFields['ADAccount']?.FieldRequired, message: 'ADAccount is required'}]}
            >
              <Input />
            </AntForm.Item>
          </Col>
          <Col span={12}>
            <Button onClick={handleCheckAccount}>Account Check</Button>
          </Col>
        </Row>
      </AntForm.Item>
    },
    {
      label: 'Camp',
      name: 'CampId',
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
      className: 'col-span-12 lg:col-span-6 mb-0',
      type: 'check',
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
      className: 'col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['EmployerId']?.FieldRequired, message: `${profileFields['EmployerId']?.Label} is required`}],
      inputprops: {
        options: state.referData.employers,
      }
    },
    {
      label: profileFields['PeopleTypeId']?.Label,
      name: 'PeopleTypeId',
      className: 'col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['PeopleTypeId']?.FieldRequired, message: `${profileFields['PeopleTypeId']?.Label} type is required`}],
      inputprops: {
        options: state.referData.peopleTypes,
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
      className: 'col-span-6 mb-0',
      rules: [{required: profileFields['ContractNumber']?.FieldRequired, message: `${profileFields['ContractNumber']?.Label} ID is required`}],
    },
    {
      label: 'PersonId',
      name: 'PersonId',
      className: 'col-span-6 mb-0',
      inputprops: {
        disabled: true,
      }
    },
    {
      label: profileFields['PositionId']?.Label,
      name: 'PositionId',
      className: 'col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['PositionId']?.FieldRequired, message: `${profileFields['PositionId']?.Label} is required`}],
      inputprops: {
        options: state.referData.positions,
      }
    },
    // {
    //   label: 'Active',
    //   name: 'Active',
    //   className: 'col-span-6 mb-0',
    //   type: 'check',
    // },
    {
      label: profileFields['RosterId']?.Label,
      name: 'RosterId',
      className: 'col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['RosterId']?.FieldRequired, message: `${profileFields['RosterId']?.Label} is required`}],
      inputprops: {
        options: state.referData.rosters,
      }
    },
    // {
    //   label: 'Next Roster Start',
    //   name: 'NextRosterDate',
    //   className: 'col-span-6 mb-0',
    //   type: 'date',
    // },
    {
      label: profileFields['HotelCheck']?.Label,
      name: 'HotelCheck',
      className: 'col-span-6 mb-0',
      type: 'check',
      rules: [{required: profileFields['HotelCheck']?.FieldRequired, message: `${profileFields['HotelCheck']?.Label} is required`}],
    },
    // {
    //   label: profileFields['RoomId']?.Label,
    //   name: 'RoomId',
    //   className: 'col-span-6 mb-0',
    //   type: 'select',
    //   rules: [{required: profileFields['RoomId']?.FieldRequired, message: `${profileFields['RoomId']?.Label} is required`}],
    //   inputprops: {
    //     options: state.referData.rooms,
    //   }
    // },
    {
      label: profileFields['LocationId']?.Label,
      name: 'LocationId',
      className: 'col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['LocationId']?.FieldRequired, message: `${profileFields['LocationId']?.Label} is required`}],
      inputprops: {
        options: state.referData?.locations.filter((item) => item.onSite !== 1),
      }
    },
    {
      label: profileFields['FlightGroupMasterId']?.Label,
      name: 'FlightGroupMasterId',
      className: 'col-span-6 mb-0',
      type: 'select',
      rules: [{required: profileFields['FlightGroupMasterId']?.FieldRequired, message: `${profileFields['FlightGroupMasterId']?.Label} is required`}],
      inputprops: {
        options: state.referData.transportGroups,
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
                rules={[{required: profileFields['PassportNumber']?.FieldRequired, message: `${profileFields['PassportNumber']?.Label} is required`}]}
              >
                <Input />
              </AntForm.Item>
              <AntForm.Item 
                label={profileFields['PassportName']?.Label}
                name='PassportName'
                className='col-span-12 lg:col-span-7 mb-0'
                rules={[
                  {required: profileFields['PassportName']?.FieldRequired, message: `${profileFields['PassportName']?.Label} is required`},
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

  const handleSubmit = (values) => {
    setLoading(true)
    setError(null)
    const formData = new FormData();
    Object.keys(values).map((key) => {
      if(key !== 'PassportRawImage'){
        if(key === 'Gender'){
          formData.append(key, values[key] ? 1 : 0)
        }
        else formData.append(key, values[key] ? values[key] : '')
      }
    })
    formData.set('PassportRawImage', values.PassportRawImage?.file ? values.PassportRawImage?.file : '')
    formData.set('Active', 0)
    axios({
      method: 'post',
      url: 'tas/employee/request',
      data: formData
    }).then( (res) => {
      navigate(`${res.data}`)
    }).catch((err) => {
      let errordata = JSON.parse(err.response.data.message)
      if(errordata && errordata.length > 0){
        api.error({
          message: `Validation Warning`,
          duration: 0,
          description: <div>
            <table className='border-collapse border border-slate-500 text-xs mt-5'>
              <thead>
                <tr className='text-[11px] font-medium'>
                  <th className='border border-slate-600'></th>
                  <th className='border border-slate-600 px-1'>ADAccount</th>
                  <th className='border border-slate-600 px-1'>SAP ID #</th>
                  <th className='border border-slate-600 px-1'>Mobile</th>
                  <th className='border border-slate-600 px-1'>NRN</th>
                </tr>
              </thead>
              <tbody>
                {
                  errordata?.map((item) => (
                    <tr>
                      <td className='border border-slate-600 text-center'>
                        <Link to={`/tas/people/search/${item.EmployeeId}`}>
                          <span className='text-blue-500 hover:underline cursor-pointer'>{item.FullName} ({item.EmployeeId})</span>
                        </Link>
                      </td>
                      <td className='border border-slate-600 text-center'>
                        {typeof item.ADAccount === 'boolean' ? item.ADAccount ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                      </td>
                      <td className='border border-slate-600 text-center'>
                        {typeof item.SAPID === 'boolean' ? item.SAPID ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                      </td>
                      <td className='border border-slate-600 text-center'>
                        {typeof item.Mobile === 'boolean' ? item.Mobile ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
                      </td>
                      <td className='border border-slate-600 text-center'>
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
    }).then(() => setLoading(false))
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
      label: <div>Personal {renderErrors([...personalFields, {name: 'NRN'}, {name: 'Gender'}])}</div>,
      key: 0,
      forceRender: true,
      children: <div className='px-4 grid grid-cols-12 gap-x-5 gap-y-2 pt-4' key={'t-item-0'}>
        <FormItemRender key={'t-item-1c'} fields={personalFields} form={form}/>
      </div>,
    },
    {
      label: <div>On Site {renderErrors(onSiteFields)}</div>,
      key: 1,
      forceRender: true,
      children: <div className='px-4 grid grid-cols-12 gap-x-5 gap-y-2 pt-4' key={'t-item-1'}>
        <FormItemRender key={'t-item-2c'} fields={onSiteFields} form={form}/>
      </div>,
    },
    {
      label: <div>Off Site {renderErrors([...offSiteFields, {name: 'PersonalMobile'}])}</div>,
      key: 2,
      forceRender: true,
      children: <div className='px-4 grid grid-cols-12 gap-x-5 gap-y-2 pt-4' key={'t-item-2'}>
        <FormItemRender key={'t-item-3c'} fields={offSiteFields} form={form}/>
      </div>,
    },
    {
      label: <div>Citizenship {renderErrors(citizenshipFieldList)}</div>,
      key: 4,
      forceRender: true,
      children: <div className='px-4 grid grid-cols-12 gap-x-5 gap-y-2 pt-4' key={'t-item-4'}>
        <FormItemRender key={'t-item-5c'} fields={citizenshipFields} form={form}/>
      </div>,
    },
  ]

  return (
    <>
    {
      fieldLoading ? 
      <Skeleton className={'h-[400px]'}/>
      :
      <AntForm
        {...formItemLayout} 
        form={form} 
        labelAlign='left'
        onFinish={handleSubmit}
        className='bg-white rounded-lg'
        onFinishFailed={onFailed}
      >
        <Tabs
          defaultActiveKey="1"
          type="card"
          size={'middle'}
          items={items}
          activeKey={currentKey}
          onTabClick={(e) => setCurrentKey(e)}
        />
        <AntForm className='px-4 pb-5 flex justify-end'>
          <Button htmlType='button' className='mt-5 mr-2' type={'primary'} onClick={() => form.submit()} loading={loading} icon={<SaveFilled/>}>
            Save
          </Button>
        </AntForm>
      </AntForm>
    }
      {contextHolder}
    </>
  )
}

export default AddPerson