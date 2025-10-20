import React, { useContext, useEffect, useMemo, useRef, useState } from 'react'
import axios from 'axios'
import { Input, Select, Radio, Space, Steps, Form as AntForm, notification, Checkbox } from 'antd'
import { InfoCircleOutlined, LeftOutlined, StarFilled } from '@ant-design/icons'
import { Accordion, Button, DuplicatedDocument, Form, Skeleton, Table, Tooltip } from 'components'
import RescheduleExistingTravel from './RescheduleExistingTravel'
import RemoveExistingTravel from './RemoveExistingTravel'
import { useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import Attachments from './Attachments'
import NewTravel from './NewTravel'
import { AuthContext } from 'contexts'
import EmployeeHeader from '../EmployeeHeader'
import ProfileForm from '../ProfileForm'
import ProfileUpdateError from 'modules/request/Documents/SiteTravelOTLLC/ProfileUpdateError'
import { twMerge } from 'tailwind-merge'

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
      <div>{e.rowIndex === 0 ? <StarFilled style={{color: '#e57200'}}/> : ''}</div>
    )
  },
]

const contentStyle = {
  marginTop: 16,
};

const items = (steps) => {
  return steps.map((item) => ({
    key: item.title,
    title: item.title,
  }))
};

function CreateSiteTravel() {
  const [ profileData, setProfileData ] = useState(null)
  const [ docRoute, setDocRoute ] = useState(null)
  const [ changesType, setChangesType ] = useState(null)
  const [current, setCurrent] = useState(0);
  const [ showAlertModal, setShowAlertModal ] = useState(false)
  const [ duplicatedData, setDuplicatedData ] = useState([])
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ pendingInfo, setPendingInfo ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  
  const [ siteTravelData, setSiteTravelData ] = useState({
    documentData: {},
    flightData: {},
    files: []
  })

  const [ form ] = AntForm.useForm()
  const [ rescheduleForm ] = AntForm.useForm()
  const [ profileForm ] = AntForm.useForm()
  const [api, contextHolder] = notification.useNotification();
  const profileFormRef = useRef(null)
  
  const { action } = useContext(AuthContext)
  const navigate = useNavigate()
  const { empId } = useParams()


  useEffect(() => {
    getProfileData()
  },[])

  useEffect(() => {
    if(profileData){
      getDocumentMembers()
    }
  },[profileData])

  const getDocumentMembers = (e) => {
    axios({
      method: 'get',
      url: `/tas/requestgroupconfig/groupandmembers/bytype/Site Travel?empId=${empId}`,
    }).then((res) => {
      setPendingInfo(res.data)
      let firstGroup = res.data.groups[0]
      let routes = [...res.data.groups]
      routes.unshift({GroupName: 'Requester', id: 0})
      routes.push({GroupName: 'Completed', id: 99999})
      setDocRoute(routes)
      form.setFieldValue('files', [])
      let tmp = []
      firstGroup.groupMembers.map((item) => {
        tmp.push({value: item.employeeId, label: item.fullName})
      })
      form.setFieldValue('assignEmployeeOptions', tmp)
      form.setFieldValue('nextGroupName', firstGroup?.GroupName)
      form.setFieldValue('nextGroupId', firstGroup?.id)

      setNextGroup(firstGroup)
    }).finally(() => {
      setLoading(false)
    })
  } 

  const getProfileData = () => {
    axios({
      method: 'get',
      url: `tas/employee/profile/${empId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
      setProfileData(res.data)
    }).catch((err) => {

    })
  }

  const types = useMemo(() => ({
    1: <NewTravel form={form}/>,
    2: <RescheduleExistingTravel data={null} form={rescheduleForm} />,
    3: <RemoveExistingTravel form={form}/>,
  }), [form, rescheduleForm])

  const steps = useMemo(() => {
    return [
      {
        title: 'Choose Request Type',
        content: <div className='flex flex-col items-start gap-4 p-4'>
          <Radio.Group onChange={(e) => {setChangesType(e.target.value); setCurrent(prev => prev + 1);}}>
            <Space direction='vertical'>
              <Radio value={1}>New Travel Request</Radio>
              { profileData?.Active === 1 ?
                <Radio value={2}>Reschedule Existing Travel</Radio>
                : null
              }
              { profileData?.Active === 1 ?
                <Radio value={3}>Remove Existing Travel</Radio>
                : null
              } 
            </Space>
          </Radio.Group>
        </div>,
      },
      {
        title: 'Form',
        content: <div>
          {types[changesType]}
        </div>,
      },
    ]
  },[profileData, changesType, rescheduleForm, form]) ;

  const prev = () => {
    setCurrent((prev) => prev - 1);
  };

  const handleSubmit = (values) => {
    if(changesType === 1){
      if(profileData?.Active === 0){
        profileForm.submit()
      }else{
        submitAddTravel(values)
      }
    }else if(changesType === 2){
      rescheduleForm.validateFields().then(() => {
        submitReschedule(values, rescheduleForm.getFieldsValue())
      }).catch(() => {
        rescheduleForm.scrollToField('ReScheduleDescription')
      })
    }else if(changesType === 3){
      submitRemoveSchedule(values)
    }else if(changesType === 4){
      // externalForm.validateFields().then(() => {
      //   submitNewExternalTravel(values, externalForm.getFieldsValue())
      // }).catch((err) => {
      //   externalForm.scrollToField(['firstTransport', 'Date'])
      // })
    }
  }

  const addTravel = () => {
    const values = form.getFieldsValue()
    axios({
      method: 'post',
      url: 'tas/requestsitetravel/addtravel',
      data: {
        documentData: {
          comment: values.comment,
          employeeId: empId,
          action: 'Submitted',
          assignedEmployeeId: values.assignedEmployeeId,
          nextGroupId: values.nextGroupId,
        },
        flightData: {
          inScheduleId: values.firstTransport.Direction === 'IN' ? values.firstTransport.flightId : values.lastTransport.flightId,
          outScheduleId: values.firstTransport.Direction === 'OUT' ? values.firstTransport.flightId : values.lastTransport.flightId,
          inScheduleGoShow: values.firstTransport.Direction === 'IN' ? values.firstTransport.GoShow : values.lastTransport.GoShow,
          outScheduleGoShow: values.firstTransport.Direction === 'OUT' ? values.firstTransport.GoShow : values.lastTransport.GoShow,
          departmentId: values.DepartmentId,
          shiftId: values.ShiftId,
          employerId: values.EmployerId,
          positionId: values.PositionId,
          costcodeId: values.CostCodeId,
          Reason: values.Reason,
        },
        files: values.files,
        skipMailNotification: values.skipMailNotification ? true : false,
      }
    }).then((res) => {
      navigate('/request/task')
    }).catch((err) => {
      setActionLoading(false)
    })
  }

  const updateProfileData = () => {
    const profileValues = profileForm.getFieldsValue()
    let groupValues = []
    const formData = new FormData();
    Object.keys(profileValues).map((item) => {
      if(!isNaN(item)){
        groupValues.push({GroupMasterId: parseInt(item), GroupDetailId: profileValues[item] ? profileValues[item] : ''})
      }
      else if(item !== 'PassportRawImage' && item !== 'SiteContactEmployeeId'){
        if(item === 'Gender'){
          formData.append(item, profileValues[item] ? 1 : 0)
        }
        else if(item === 'CommenceDate'){
          formData.append(item, profileValues[item] ? dayjs(profileValues[item]).format('YYYY-MM-DD') : '')
        }
        else if(item === 'CompletionDate'){
          formData.append(item, profileValues[item] ? dayjs(profileValues[item]).format('YYYY-MM-DD') : '')
        }
        else{
          formData.append(item, profileValues[item] ? profileValues[item] : '')
        }
      }
      else if(item === 'SiteContactEmployeeId'){
        if(typeof profileValues[item] === 'string'){
          formData.append(item, profileValues?.SiteContactEmployeeId ? profileValues?.SiteContactEmployeeId : '')
        }
        else{
          formData.append(item, profileValues[item] ? profileValues[item] : '')
        }
      }
    })
    formData.append('PassportRawImage', profileValues.PassportRawImage?.file ? profileValues.PassportRawImage?.file : '')
    formData.append('id', empId)
    axios({
      method: 'patch',
      url: 'tas/employee',
      data: formData,
    }).then( async (res) => {
      if(groupValues?.length > 0){
        axios({
          method: 'post',
          url: `tas/groupmembers/${empId}`,
          data: groupValues
        }).then((res) => {
          addTravel()
        }).catch((err) => {
          setActionLoading(false)
        })
      }else{
        addTravel()
      }
    }).catch((err) => {
      setActionLoading(false)
      let errordata = JSON.parse(err.response.data.message)
      if(errordata && errordata.length > 0){
        api.error({
          message: 'Validation Warning',
          duration: 0,
          description: <div>
            <ProfileUpdateError errordata={errordata}/>
          </div>
        });
      }
    })
  }

  const submitAddTravel = () => {
    const values = form.getFieldsValue()
    setActionLoading(true)
    axios({
      method: 'post',
      url: `tas/requestsitetravel/addtravel/checkduplicate`,
      data: {
        employeeId: empId,
        inScheduleId: values.firstTransport.Direction === 'IN' ? values.firstTransport.flightId : values.lastTransport.flightId,
        outScheduleId: values.firstTransport.Direction === 'OUT' ? values.firstTransport.flightId : values.lastTransport.flightId,
      }
    }).then((res) => {
      setDuplicatedData(res.data)
      if(res.data.length === 0){
        if(changesType === 1 && profileData?.Active === 0){
          //////      Update profile data       ///////
          updateProfileData(values)
        }else{
          addTravel()
        }
      }else{
        setActionLoading(false)
        setShowAlertModal(true)
      }
    }).catch(() => {
      setActionLoading(false)
    })
  }

  const submitReschedule = (values) => {
      setActionLoading(true)
      let flightData = {
        existingScheduleId: rescheduleForm.getFieldValue('existingScheduleId'),
        ExistingScheduleIdNoShow: rescheduleForm.getFieldValue('ExistingScheduleIdNoShow'),
        reScheduleId: rescheduleForm.getFieldValue('ReScheduleId'),
        ReScheduleGoShow: rescheduleForm.getFieldValue('ReScheduleGoShow'),
        shiftId: rescheduleForm.getFieldValue('ShiftId'),
        Reason: rescheduleForm.getFieldValue('Reason'),
      }
      axios({
        method: 'post',
        url: `tas/requestsitetravel/reschedule/checkduplicate`,
        data: {
          existingScheduleId: flightData.existingScheduleId,
          reScheduleId: flightData.reScheduleId,
          employeeId: empId,
        }
      }).then((res) => {
        setDuplicatedData(res.data)
        if(res.data.length === 0){
          axios({
            method: 'post',
            url: 'tas/requestsitetravel/reschedule',
            data: {
              documentData: {
                comment: values.comment,
                employeeId: empId,
                action: 'Submitted',
                assignedEmployeeId: values.assignedEmployeeId,
                nextGroupId: values.nextGroupId, 
              },
              flightData: flightData,
              files: values.files,
              skipMailNotification: values.skipMailNotification ? true : false,
            }
          }).then((res) => {
            navigate('/request/task')
          }).catch((err) => {
      
          })
        }else{
          setShowAlertModal(true)
          setActionLoading(false)
        }
      }).catch(() => {
        setActionLoading(false)
      })
    // }
  }

  const submitRemoveSchedule = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: `tas/requestsitetravel/remove/checkduplicate`,
      data: {
        employeeId: empId,
        firstScheduleId: values?.firstTransport?.firstScheduleId,
        lastScheduleId: values?.lastTransport?.lastScheduleId,
        Reason: values?.Reason,
      }
    }).then((res) => {
      setDuplicatedData(res.data)
      if(res.data.length === 0){
        axios({
          method: 'post',
          url: 'tas/requestsitetravel/removetravel',
          data: {
            documentData: {
              comment: values.comment,
              employeeId: empId,
              action: 'Submitted',
              assignedEmployeeId: values.assignedEmployeeId,
              nextGroupId: values.nextGroupId, 
            },
            flightData: {
              firstScheduleId: values?.firstTransport?.firstScheduleId,
              lastScheduleId: values?.lastTransport?.lastScheduleId,
              FirstScheduleNoShow: values?.firstTransport?.NoShow,
              LastScheduleNoShow: values?.lastTransport?.NoShow,
              shiftId: values.ShiftId,
              CostCodeId: values.CostCodeId,
              Reason: values.Reason,
            },
            files: values.files,
            skipMailNotification: values.skipMailNotification ? true : false,
          }
        }).then((res) => {
          navigate('/request/task')
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
  }

  const handleDraft = () => {
    axios({
      method: 'post',
      url: 'tas/requestdocument/sitetravel',
      data: {
        ...siteTravelData,
        travelData: {
          ...siteTravelData.travelData,
          employeeId: empId,
          action: 'Saved',
        },
      }
    }).then((res) => {
      navigate('/request/task')
    }).catch((err) => {

    })
  }

  return (
    <div className='px-6 py-5 rounded-ot shadow-card bg-white'>
      <Button
        className='mb-3' 
        onClick={() => navigate('/request/sitetravel')}
        icon={<LeftOutlined />}
      >
        Back
      </Button>
      <div className={twMerge('grid grid-cols-1 gap-4', loading ? 'grid' : 'hidden')}>
        <Skeleton className='h-[200px]'/>
        <Skeleton className='h-[200px]'/>
        <Skeleton className='h-[300px]'/>
        <Skeleton className='h-[200px]'/>
      </div>
      <div className={loading ? 'hidden' : 'block'}>
        <Accordion title='Profile' className='border'>
          <div className='p-3'>
            <EmployeeHeader profileData={profileData} pendingInfo={pendingInfo}/>
          </div>
        </Accordion>
      
        <div className='flex flex-col gap-4 mt-4'>
          <Form
            className='grid grid-cols-12 gap-y-5'
            size='small'
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
            }}
            onFinish={handleSubmit}
          >
            {
              changesType === 1 && profileData?.Active === 0 ?
              <Accordion className='col-span-12 border rounded-ot overflow-hidden mt-3' title='Profile'>
                <div className='flex flex-col gap-5' ref={profileFormRef}>
                  <ProfileForm
                    data={profileData}
                    form={profileForm}
                    containerRef={profileFormRef}
                    onSubmit={submitAddTravel}
                    className='shadow-none border-none'
                  />
                </div>
              </Accordion>
              : null
            }
            <Accordion title='Travel' className='border col-span-12'>
              <div className='p-4'>
                <Steps items={items(steps)} current={current} size='small'/>
                <div style={contentStyle}>{steps[current].content}</div>
                <div className='mt-6 flex justify-between items-center'>
                  {current > 0 && (
                    <Button style={{ margin: '0 8px'}} onClick={prev}>
                      Previous
                    </Button>
                  )}
                </div>
              </div>
            </Accordion>
            <Form.Item className='col-span-12 mb-0' name='files'>
              <Attachments
                // getMaster={getDocumentInfo}
                mainForm={form}
              />
            </Form.Item>
            <Accordion title='Request Document' className='border col-span-12'>
              <div className='flex flex-col gap-4 p-4'>
                <div>
                  Comments:
                  <Form.Item className='m-0' name='comment'>
                    <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                  </Form.Item>
                </div>
                <div className='flex gap-10'>
                  <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.nextGroupName !== curValues.nextGroupName}>
                    {({getFieldValue}) => {
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
                              size='small' 
                              options={getFieldValue('assignEmployeeOptions')}
                              className='w-[200px]'
                              popupMatchSelectWidth={false}
                              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                              showSearch
                            />
                          </Form.Item>
                        )
                      }}
                    </Form.Item>
                  }
                  <Form.Item name='skipMailNotification' valuePropName='checked' className='w-[300px] mb-0'>
                    <Checkbox>
                      <span className='text-xs'>Skip Mail Notifcation</span>
                      <Tooltip title='If checked, email notifications to the assigned Line Manager will not be sent for this request.'><InfoCircleOutlined className='text-gray-400 ml-2'/></Tooltip>
                    </Checkbox>
                  </Form.Item>
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
              </div>
            </Accordion>
          </Form>
        </div>
        <div className='flex justify-end mt-4 gap-4'>
          <Button type='primary' onClick={() => form.submit()} loading={actionLoading}>Submit</Button>
          <Button onClick={handleDraft} disabled={actionLoading}>Draft</Button>
          <Button disabled={actionLoading}>Cancel</Button>
        </div>
        <DuplicatedDocument
          data={duplicatedData}
          open={showAlertModal}
          onCancel={() => setShowAlertModal(false)}
        />
        {contextHolder}
      </div>
    </div>
  )
}

export default CreateSiteTravel