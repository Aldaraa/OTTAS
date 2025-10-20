import React, { useContext, useEffect, useState } from 'react'
import axios from 'axios'
import { Input, Select, Radio, Space, Steps, Form as AntForm, notification } from 'antd'
import { LeftOutlined, StarFilled } from '@ant-design/icons'
import { Accordion, Button, DuplicatedDocument, Form, Table } from 'components'
import RescheduleExistingTravel from './RescheduleExistingTravel'
import RemoveExistingTravel from './RemoveExistingTravel'
import { Link, useNavigate, useParams } from 'react-router-dom'
import Attachments from './Attachments'
import NewTravel from './NewTravel'
import { AuthContext } from 'contexts'
import EmployeeHeader from '../EmployeeHeader'

function CreateSiteTravel() {
  const [ docRoute, setDocRoute ] = useState(null)
  const [ groupAndMembers, setGroupAndMembers ] = useState(null)
  const [ changesType, setChangesType ] = useState(null)
  const [current, setCurrent] = useState(0);
  const [ showAlertModal, setShowAlertModal ] = useState(false)
  const [ duplicatedData, setDuplicatedData ] = useState([])
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ pendingInfo, setPendingInfo ] = useState(null)
  
  const [ siteTravelData, setSiteTravelData ] = useState({
    documentData: {},
    flightData: {},
    files: []
  })

  const [ form ] = AntForm.useForm()
  const [ addForm ] = AntForm.useForm()
  const [ rescheduleForm ] = AntForm.useForm()
  const [ removeForm ] = AntForm.useForm()
  const [api, contextHolder] = notification.useNotification();
  
  const { action, state } = useContext(AuthContext)
  const navigate = useNavigate()
  const { empId } = useParams()


  useEffect(() => {
    getDocumentMembers()
  },[])

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
      setGroupAndMembers(firstGroup)
      let tmp = []
      firstGroup.groupMembers.map((item) => {
        tmp.push({value: item.employeeId, label: item.fullName})
      })
      form.setFieldValue('assignEmployeeOptions', tmp)
      form.setFieldValue('nextGroupName', firstGroup?.GroupName)
      form.setFieldValue('nextGroupId', firstGroup?.id)

      setNextGroup(firstGroup)
    })
  } 

  const getProfileData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${empId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
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
        <div>{e.rowIndex === 0 ? <StarFilled style={{color: '#e57200'}}/> : ''}</div>
      )
    },
  ]

  const steps = [
    {
      title: 'Choose Request Type',
      content: <div className='flex flex-col items-start gap-4 p-4'>
        <Radio.Group onChange={(e) => {setChangesType(e.target.value); setCurrent(current + 1);}}>
          <Space direction='vertical'>
            <Radio value={1}>New Travel Request</Radio>
            <Space className='ml-6'>&#8226; Add a new site travel request</Space>
            <Radio value={2}>Reschedule Existing Travel</Radio>
            <Space className='ml-6'>&#8226; Change the details of an existing travel request</Space>
            <Radio value={3}>Remove Existing Travel</Radio>
            <Space className='ml-6'>&#8226; Remove existing travel request</Space>
          </Space>
        </Radio.Group>
      </div>,
    },
    {
      title: 'Form',
      content: <div>
        {
          changesType === 1 ?
          <NewTravel form={form}/> 
          :
          changesType === 2 ?
          <RescheduleExistingTravel
            data={null} 
            form={rescheduleForm}
          />
          :
          changesType === 3 ?
          <RemoveExistingTravel 
            form={form}
          />
          :
          null
        }
        
      </div>,
    },
  ];

  const next = () => {
    setCurrent(current + 1);
  };
  const prev = () => {
    setCurrent(current - 1);
  };

  const items = steps.map((item) => ({
    key: item.title,
    title: item.title,
  }));

  const contentStyle = {
    marginTop: 16,
  };

  const handleSubmit = (values) => {
    if(changesType === 1){
      submitAddTravel(values, addForm.getFieldsValue())
    }else if(changesType === 2){
      submitReschedule(values, rescheduleForm.getFieldsValue())
    }else if(changesType === 3){
      submitRemoveSchedule(values, form.getFieldsValue())
    }
  }

  const submitAddTravel = (values) => {
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
              departmentId: values.DepartmentId,
              shiftId: values.ShiftId,
              employerId: values.EmployerId,
              positionId: values.PositionId,
              costcodeId: values.CostCodeId,
              Reason: values.Reason,
            },
            files: values.files,
          }
        }).then((res) => {
          navigate('/guest/request')
        }).catch((err) => {
    
        }).then(() => setActionLoading(false))
      }else{
        setShowAlertModal(true)
        setActionLoading(false)
      }
    }).catch(() => {
      setActionLoading(false)
    })
  }

  const submitReschedule = (values) => {
    setActionLoading(true)
    let flightData = {
      existingScheduleId: rescheduleForm.getFieldValue('existingScheduleId'),
      reScheduleId: rescheduleForm.getFieldValue('ReScheduleId'),
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
          }
        }).then((res) => {
          navigate('/guest/request')
        }).catch((err) => {
    
        }).then(() => setActionLoading(false))
      }else{
        setActionLoading(false)
        setShowAlertModal(true)
      }
    }).catch(() => {
      setActionLoading(false)
    })
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
              shiftId: values.ShiftId,
              CostCodeId: values.CostCodeId,
              Reason: values.Reason,
              noShow: values.noShow ? 1 : 0,
            },
            files: values.files,
          }
        }).then((res) => {
          navigate('/guest/request')
        }).catch((err) => {
    
        }).then(() => setActionLoading(false))
      }else{
        setActionLoading(false)
        setShowAlertModal(true)
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
      navigate('/guest/request')
    }).catch((err) => {

    })
  }

  return (
    <div className=''>
      <Link to={'/guest'}>
        <Button icon={<LeftOutlined/>}>Back</Button>
      </Link>
      <Accordion title='Profile' className='border'>
        <div className='p-3'>
          <EmployeeHeader employeeId={empId} pendingInfo={pendingInfo}/>
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
          <Accordion title='Travel' className='border col-span-12'>
            <div className='p-4'>
              <Steps items={items} current={current} size='small'/>
              <div style={contentStyle}>{steps[current].content}</div>
              <div className='mt-6 flex justify-between items-center'>
                {current > 0 && (
                  <Button
                  style={{
                    margin: '0 8px',
                  }}
                  onClick={() => prev()}
                  >
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
                  <Input.TextArea autoSize={{minRows: 3, maxRows: 5}}/>
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
            </div>
          </Accordion>
        </Form>
      </div>
      <div className='flex justify-end mt-4 gap-4'>
        <Button type='primary' onClick={() => form.submit()} loading={actionLoading}>Submit</Button>
        <Button disabled={actionLoading}>Cancel</Button>
      </div>
      <DuplicatedDocument
        data={duplicatedData}
        open={showAlertModal}
        onCancel={() => setShowAlertModal(false)}
      />
      {contextHolder}
    </div>
  )
}

export default CreateSiteTravel