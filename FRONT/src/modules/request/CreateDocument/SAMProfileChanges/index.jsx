import React, { useCallback, useContext, useEffect, useState } from 'react'
import axios from 'axios'
import { Input, Form as AntForm, Steps, Radio, Space } from 'antd'
import { LeftOutlined, StarFilled } from '@ant-design/icons'
import { Accordion, Button, DuplicatedDocument, Form, Table } from 'components'
import ProfileForm from './ProfileForm'
import { useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import EmployeeHeader from '../EmployeeHeader'
import Temporary from './Temporary'
import { AuthContext } from 'contexts'

function CreateSAMProfileChanges({ onSubmit}) {
  const [ docRoute, setDocRoute ] = useState(null)
  const [ duplicatedData, setDuplicatedData ] = useState([])
  const [ showAlertModal, setShowAlertModal ] = useState(false)
  const [ groupAndMembers, setGroupAndMembers ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ pendingInfo, setPendingInfo ] = useState(null)
  const [ changesType, setChangesType ] = useState(null)
  const [ current, setCurrent ] = useState(0);

  const { action, state } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const [ profileForm ] = AntForm.useForm()
  const [ temporaryForm ] = AntForm.useForm()
  
  const navigate = useNavigate()
  const { empId } = useParams()

  useEffect(() => {
    getDocumentMembers()
    getData()
  },[])

  const getDocumentMembers = (e) => {
    axios({
      method: 'get',
      url: `/tas/requestgroupconfig/groupandmembers/bytype/Profile Changes?empId=${empId}`,
    }).then((res) => {
      let firstGroup = res.data.groups[0]
      setPendingInfo(res.data)
      setGroupAndMembers(firstGroup)
      let tmp = []
      firstGroup.groupMembers.map((item) => {
        tmp.push({value: item.employeeId, label: item.fullName})
      })
      form.setFieldValue('assignEmployeeOptions', tmp)
      form.setFieldValue('nextGroupName', firstGroup?.GroupName)
      form.setFieldValue('nextGroupId', firstGroup?.id)

      let routes = res.data.groups
      routes.unshift({GroupName: 'Requester', id: 0})
      routes.push({GroupName: 'Completed', id: 99999})
      setDocRoute(routes)

    })
  } 

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employee/profile/${empId}`
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
        <div>{e.rowIndex == 0 ? <StarFilled style={{color: '#e57200'}}/> : ''}</div>
      )
    },
  ]

  const handleSubmitPermanant = useCallback((values) => {
    const profileValues = profileForm.getFieldsValue()
    axios({
      method: 'post',
      url: 'tas/requestdocumentprofilechange',
      data: {
        employee: {
          ...profileValues,
          Dob: profileValues?.Dob ? dayjs(profileValues?.Dob).format('YYYY-MM-DD') : null,
          CommenceDate: profileValues?.CommenceDate ? dayjs(profileValues?.CommenceDate).format('YYYY-MM-DD') : null,
          CompletionDate: profileValues?.CompletionDate ? dayjs(profileValues?.CompletionDate).format('YYYY-MM-DD') : null,
          NextRosterDate: profileValues?.NextRosterDate ? dayjs(profileValues?.NextRosterDate).format('YYYY-MM-DD') : null,
          PassportExpiry: profileValues?.PassportExpiry ? dayjs(profileValues?.PassportExpiry).format('YYYY-MM-DD') : null,
          EmployeeId: empId,
        },
        changeRequestData: {
          ...values,
          action: 'Submitted',
        },
      }
    }).then((res) => {
      navigate('/request/task')
    }).catch((err) => {

    }).then(() => {
      setActionLoading(false)
    })
  },[profileForm, empId])

  const handleSubmitTemporary = useCallback((values) => {
    const tempValues = temporaryForm.getFieldsValue()
    axios({
      method: 'post',
      url: 'tas/requestdocumentprofilechange/temp',
      data: {
        employee: {
          ...temporaryForm.getFieldsValue(),
          StartDate: tempValues.StartDate ? dayjs(tempValues.StartDate).format('YYYY-MM-DD') : null,
          EndDate: tempValues.EndDate ? dayjs(tempValues.EndDate).format('YYYY-MM-DD') : null,
          EmployeeId: empId,
        },
        changeRequestData: {
          ...values,
          action: 'Submitted',
        },
      }
    }).then((res) => {
      navigate('/request/task')
    }).catch((err) => {

    }).then(() => {
      setActionLoading(false)
    })
  },[temporaryForm, empId])

  
  const handleSubmit = useCallback((values) => {
    setActionLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocument/checkduplicate/samprofilechanges/${empId}`
    }).then((res) => {
      setDuplicatedData(res.data)
      if(res.data.length === 0){
        if(changesType === 1){
          handleSubmitPermanant(values)
        }else if(changesType === 2){
          handleSubmitTemporary(values)
        }
      }else{
        setShowAlertModal(true)
        setActionLoading(false)
      }
    }).catch(() => {
      
    })
  },[changesType, empId])

  const handleDraft = () => {
    // axios({
    //   method: 'post',
    //   url: 'tas/requestdocumentprofilechange',
    //   data: {
    //     employee: profileChangesForm.getFieldsValue(),
    //     changeRequestData: samProfileChanges,
    //     employeeId: empId,
    //     action: 'Saved',
    //   }
    // }).then((res) => {
    //   navigate('/request/task')
    // }).catch((err) => {

    // })
  }

  const steps = [
    {
      title: 'Choose Request Type',
      content: <div className='flex flex-col items-start gap-4 p-4'>
        <Radio.Group onChange={(e) => {setChangesType(e.target.value); setCurrent(current + 1);}}>
          <Space direction='vertical'>
            <Radio value={1}>Permanent Change</Radio>
            <Space className='ml-6'>&#8226; Add a new site travel request</Space>
            <Radio disabled={!state.userProfileData?.Active} value={2}>Temporary Change</Radio>
            <Space className='ml-6'>&#8226; Change the details of an existing travel request</Space>
          </Space>
        </Radio.Group>
      </div>,
    },
    {
      title: 'Form',
      content: <div>
        {
          changesType ? 
          <>
            <div className={changesType === 1 ? 'block' : 'hidden'}>
              <ProfileForm form={profileForm}/>
            </div>
            <div className={changesType === 2 ? 'block' : 'hidden'}>
              <Temporary form={temporaryForm}/>
            </div>
          </>
          :
          null
        }
      </div>,
    },
  ];

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

  return (
    <div className='px-6 py-5 rounded-ot shadow-card bg-white'>
      <Button
        className='mb-3' 
        onClick={() => navigate('/request/tasprofilechanges')} 
        icon={<LeftOutlined />}
      >
        Back
      </Button>
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
          initValues={{comment: '', assignedEmployeeId: null}}
          onFinish={handleSubmit}
        >
          <Accordion title='Detail' className='border col-span-12'>
            <div className='p-4'>
              <Steps items={items} current={current} size='small'/>
              <div style={contentStyle}>{steps[current]?.content}</div>
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
        <Button type='primary' htmlType='button' onClick={() => form.submit()} loading={actionLoading}>Submit</Button>
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

export default CreateSAMProfileChanges