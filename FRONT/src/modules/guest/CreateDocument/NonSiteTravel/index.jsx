import React, { useContext, useEffect, useState } from 'react'
import Flight from './Flight'
import axios from 'axios'
import Accommodation from './Accommodation'
import { DatePicker, Input, InputNumber, Select, Form as AntForm, TreeSelect, } from 'antd'
import { InfoCircleFilled, LeftOutlined, StarFilled } from '@ant-design/icons'
import Attachments from './Attachments'
import { Accordion, Button, DuplicatedDocument, Form, Table } from 'components'
import dayjs from 'dayjs'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import isArray from 'lodash/isArray'
import EmployeeHeader from '../EmployeeHeader'

const latin = new RegExp(/^[A-Za-z -]+$/g)
const internationalPhone = new RegExp(/^[0-9 +]+$/)

function CreateNonSiteTravel() {
  const [ travelPurpose, setTravelPurpose ] = useState([])
  const [ showAlertModal, setShowAlertModal ] = useState(false)
  const [ duplicatedData, setDuplicatedData ] = useState([])
  const [ docRoute, setDocRoute ] = useState([])
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ isRequiredFlightOrAcc, setIsRequiredFlightOrAcc ] = useState(false)
  const [ pendingInfo, setPendingInfo ] = useState(null)
  const [ isForeign, setIsForeign ] = useState(false)

  const { state, action } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const navigate = useNavigate()
  const { empId } = useParams()

  useEffect(() => {
    getTravelPurposeList()
    getDocumentMembers()
  },[])
  
  useEffect(() => {
    const userNationality = state.referData.nationalities.find((item) => item.Id === state.userProfileData?.NationalityId)
    if(userNationality && userNationality.Code !== 'MN'){
      setIsForeign(true)
    }
  },[])

  const getDocumentMembers = (e) => {
    axios({
      method: 'get',
      url: `/tas/requestgroupconfig/groupandmembers/bytype/Non Site Travel?empId=${empId}`,
    }).then((res) => {
      setPendingInfo(res.data)
      form.setFieldValue('nextGroupName', '')
      let firstGroup = res.data.groups[0]
      let routes = [...res.data.groups]
      routes.unshift({GroupName: 'Requester', id: 0})
      routes.push({GroupName: 'Completed', id: 99999})
      setDocRoute(routes)
      setNextGroup(firstGroup)

    })
  } 

  const getTravelPurposeList = () => {
    axios({
      method: 'get',
      url: 'tas/requesttravelpurpose?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({
          value: item.Id,
          label: item.Description
        })
      })
      setTravelPurpose(tmp)
    }).catch((err) => {

    })
  }

  const docRouteCols = [
    {
      label: 'Group',
      name: 'GroupName',
    },
    {
      label: 'Current Position',
      name: 'CurrentPosition',
      alignment: 'left',
      cellRender: (e) => (
        <div>{e.rowIndex == 0 ? <StarFilled style={{color: '#e57200'}}/> : ''}</div>
      )
    },
  ]

  const handleSubmit = (values) => {
    const flightData = form.getFieldValue('flightData')
    const firstFlight = isArray(flightData) && flightData.length > 0 ? flightData[0] : null
    const accommodationData = form.getFieldValue('accommodationData')
    const files = form.getFieldValue('files')
    if(flightData.length > 0 || accommodationData.length > 0){
      setIsRequiredFlightOrAcc(false)
      setActionLoading(true)
      axios({
        method: 'get',
        url: `tas/requestdocument/checkduplicate/non site travel/${empId}${firstFlight ? `?startdate=${dayjs(flightData.travelDate).format('YYYY-MM-DD')}` : ''}`
      }).then((res) => {
        setDuplicatedData(res.data)
        if(res.data.length === 0){
          axios({
            method: 'post',
            url: 'tas/requestnonsitetravel',
            data: {
              flightData: firstFlight ? flightData : [],
              accommodationData: accommodationData ? accommodationData.map((item) => ({...item, FirstNight: dayjs(item.FirstNight).format('YYYY-MM-DD'), LastNight: dayjs(item.LastNight).format('YYYY-MM-DD')})) : [],
              files: files ? files : [],
              requestInfo: {
                comment: values.comment,
              },
              travelData: {
                nextGroupId: values.nextGroupId,
                assignedEmployeeId: values.assignedEmployeeId,
                requestTravelPurposeId: values.requestTravelPurposeId,
                employeeId: empId,
                action: 'Submitted',
              }
            }
          }).then((res) => {
            axios({
              method: 'put',
              url: 'tas/requestnonsitetravel/employee',
              data: {
                "EmergencyContactName": values.EmergencyContactName,
                "EmergencyContactMobile": values.EmergencyContactMobile,
                "PassportExpiry": values.PassportExpiry,
                "EmployeeId": empId,
                "DocumentId": 0,
                "FrequentFlyer": values.FrequentFlyer,
                "PassportNumber": values.PassportNumber,
                "PassportName": values.PassportName,
                "Email": values.Email,
              }
            }).then((res) => {
              setActionLoading(false)
              navigate('/guest/request')
            }).catch((err) => {
    
            })
          }).catch((err) => {
            setActionLoading(false)
          })
        }else{
          setShowAlertModal(true)
          setActionLoading(false)
        }
      }).catch(() => {
        setActionLoading(false)
      })
    }else{
      setIsRequiredFlightOrAcc(true)
    }
  }

  return (
    <div>
      <Link to={'/guest'}>
        <Button icon={<LeftOutlined/>}>Back</Button>
      </Link>
      <Accordion title='Profile' className='border'>
        <div className='p-3'>
          <EmployeeHeader employeeId={empId} pendingInfo={pendingInfo}/>
        </div>
      </Accordion>
      {
        state.userProfileData ?
        <div className='flex flex-col gap-4 mt-4'>
          <Form 
            className='grid grid-cols-12 gap-y-5'
            form={form}
            scrollToFirstError={true}
            initValues={{
              EmergencyContactMobile: '',
              EmergencyContactName: '',
              FrequentFlyer: null,
              PassportExpiry: null,
              PassportName: '',
              PassportNumber: '',
              assignedEmployeeId: null,
              nextGroupId: null,
              nextGroupName: 'null',
              flightData: [],
              accommodationData: [],
              files: [],
              assignEmployeeOptions: [],
              DocumentRoute: [],
              ...state.userProfileData, PassportExpiry: state.userProfileData?.PassportExpiry ? dayjs(state.userProfileData?.PassportExpiry) : null
            }}
            // labelWrap
            onFinish={handleSubmit}
          >
            <Accordion title='Traveller Details' className='border col-span-12'>
              <div className='grid grid-cols-12 gap-y-0 gap-x-4 p-4'>
                <Form.Item 
                  name='requestTravelPurposeId' 
                  label='Travel Purpose' 
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'Travel Purpose'}]}
                >
                  <Select 
                    className='w-[200px]'
                    options={travelPurpose}
                  />
                </Form.Item>
                <Form.Item 
                  name='DepartmentId' 
                  label='Department' 
                  className='col-span-6 mb-2'
                >
                  <TreeSelect
                    treeData={[...state.referData?.departments]}
                    fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
                    disabled
                  />
                </Form.Item>
                <Form.Item 
                  name='EmployerId' 
                  label='Employer' 
                  className='col-span-6 mb-2'
                >
                  <Select 
                    className='w-[200px]'
                    options={state.referData.employers}
                    disabled
                  />
                </Form.Item>
                <Form.Item 
                  name='CostCodeId' 
                  label='Cost code' 
                  className='col-span-6 mb-2'
                >
                  <Select 
                    className='w-[200px]'
                    options={state.referData.costCodes}
                    disabled
                  />
                </Form.Item>
                {
                  isForeign ?
                  <Form.Item 
                    name='Hometown' 
                    label='Hometown' 
                    className='col-span-6 mb-2'
                  >
                    <Input.TextArea 
                      disabled
                    />
                  </Form.Item>
                  : null
                }
                <Form.Item 
                  name='PassportNumber' 
                  label='Passport Number' 
                  className='col-span-6 mb-2' 
                  rules={[{required: true, message: 'Passport Number is required'}]}
                  >
                  <Input/>
                </Form.Item>
                <Form.Item 
                  name='PassportName'
                  label='Passport Name'
                  className='col-span-6 mb-2'
                  rules={[
                    { required: true, message: 'Passport Name is required' },
                    { pattern: latin, message: 'The input is not valid name!' }
                  ]}
                  >
                  <Input/>
                </Form.Item>
                <Form.Item 
                  name='PassportExpiry'
                  label='Passport Expiry'
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'Passport Expiry is required'}]}
                  >
                  <DatePicker/>
                </Form.Item>
                <Form.Item 
                  name='FrequentFlyer' 
                  label='Frequent Flyer' 
                  className='col-span-6 mb-2'
                  // rules={[{required: true, message: 'Frequent Flyer is required'}]}
                  >
                  <InputNumber min={0} controls={false}/>
                </Form.Item>
                <Form.Item 
                  name='EmergencyContactName' 
                  label='Emergency Contact Name' 
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'Emergency Contact Name is required'}]}
                  >
                  <Input/>
                </Form.Item>
                <Form.Item 
                  name='EmergencyContactMobile' 
                  label='Emergency Contact (Number for this Travel)' 
                  className='col-span-6 mb-2'
                  rules={[
                    {required: true, message: 'Emergency Contact Mobile is required'},
                    { pattern: internationalPhone, message: 'Invalid phone number'}
                  ]}
                >
                  <Input
                    // prefix='+976'
                    className='w-full'
                    maxLength={15}
                    controls={false}
                  />
                </Form.Item>
                <Form.Item 
                  name='Email' 
                  label='E-Mail' 
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'E-Mail is required'}, {type: 'email', message: `The input is not valid E-mail!`}]}
                >
                  <Input className='w-full' maxLength={100}/>
                </Form.Item>
              </div>
            </Accordion>
            <Accordion title='Detail' className='border col-span-12'>
              <div className='flex flex-col gap-4 p-4'>
              <div className='w-full my-2 text-gray-400'>
                  <InfoCircleFilled className='mr-3'/>
                  Enter in the details for flights, select the travel class, for an international trip your password details must be entered. Click the Add new record link to enter in the details,
                </div>
                <Flight 
                  getMaster={getDocumentMembers} 
                  mainForm={form} 
                />
                <Accommodation 
                  getMaster={getDocumentMembers} 
                  mainForm={form} 
                />
                <Attachments 
                  getMaster={getDocumentMembers} 
                  mainForm={form} 
                />
              </div>
            </Accordion>
            <Accordion title='Request Document' className='border col-span-12'>
              <div className='flex flex-col gap-2 p-4'>
                <div>
                  Comments:
                  <Form.Item className='m-0' name='comment'>
                    <Input.TextArea autoSize={{minRows: 3, maxRows: 5}}/>
                  </Form.Item>
                </div>
                <div className='flex gap-10'>
                  <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.nextGroupName !== curValues.nextGroupName || prevValues.accommodationData !== curValues.accommodationData || prevValues.flightData !== curValues.flightData }>
                    {({getFieldValue, setFieldValue}) => {
                      // if(getFieldValue('flightData')?.length > 0){
                        let nextgroup = docRoute.find(route => route.GroupTag === 'travelflight')
                        setFieldValue('nextGroupId', nextgroup?.id)
                        setFieldValue('nextGroupName', nextgroup?.GroupName)
                        setNextGroup(nextgroup)
                      // } 
                      // else if(getFieldValue('accommodationData')?.length > 0){
                      //   let nextgroup = docRoute.find(route => route.GroupTag === 'linemanager')
                      //   let tmp = []
                      //   nextgroup.groupMembers.map((item) => {
                      //     tmp.push({value: item.employeeId, label: item.fullName})
                      //   })
                      //   form.setFieldValue('assignEmployeeOptions', tmp)
                      //   setFieldValue('nextGroupId', nextgroup.id)
                      //   setFieldValue('nextGroupName', nextgroup.GroupName)
                      //   setNextGroup(nextgroup)
                      // }
                      return(
                        <Form.Item name='nextGroupId' className='mb-0'>
                          Next Approval: <span className='font-bold'>{getFieldValue('nextGroupName')}</span>
                        </Form.Item>
                      )
                    }}
                  </Form.Item>
                  {
                    nextGroup?.GroupTag === 'linemanager' &&
                    <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.assignEmployeeOptions !== curValues.assignEmployeeOptions}>
                      {({getFieldValue}) => {
                        return(
                          <Form.Item
                            name='assignedEmployeeId' 
                            label='Send to:' 
                            rules={[{required: true, message: 'Assign Employee is required'}]}
                            className='w-[300px] mb-0'
                          >
                            <Select 
                              options={getFieldValue('assignEmployeeOptions')}
                              className='w-[200px]'
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                              showSearch
                            />
                          </Form.Item>
                        )
                      }}
                    </Form.Item>
                  }
                </div>
              </div>
            </Accordion>
            <Accordion title='Document Route' className='border col-span-12'>
              <div className='flex flex-col gap-4 p-4'>
                  <Table
                    data={docRoute}
                    columns={docRouteCols}
                    pager={false}
                    containerClass='shadow-none border border-gray-300'
                  />
                {/* <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.DocumentRoute !== curValues.DocumentRoute}>
                  {({getFieldValue}) => {
                    return(
                      <Table
                        data={getFieldValue('DocumentRoute')}
                        columns={docRouteCols}
                        pager={false}
                        containerClass='shadow-none border border-gray-300'
                      />
                    )
                  }}
                </Form.Item> */}
              </div>
            </Accordion>
          </Form>
        </div>
        :
        null        
      }
      <div className='flex justify-end items-center mt-4 gap-4'>
        {
          isRequiredFlightOrAcc ? 
            <span className='text-red-400'>Flight Data or Accommidation Data is empty</span> 
          : ''
        }
        <Button type='primary' onClick={() => form.submit()} loading={actionLoading}>Submit</Button>
        <Button disabled={actionLoading} onClick={() => navigate(-1)}>Cancel</Button>
      </div>
      <DuplicatedDocument
        open={showAlertModal}
        onCancel={() => setShowAlertModal(false)}
        data={duplicatedData}
      />
    </div>
  )
}

export default CreateNonSiteTravel