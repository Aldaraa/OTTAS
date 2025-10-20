import React, { useContext, useEffect, useState } from 'react'
import axios from 'axios'
import { Input, Select, Form as AntForm, DatePicker, } from 'antd'
// import { Accordion } from 'devextreme-react'
import { LeftOutlined, StarFilled } from '@ant-design/icons'
import { Accordion, Button, DuplicatedDocument, Form, Table } from 'components'
import { AuthContext } from 'contexts'
import { useLoaderData, useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import EmployeeHeader from '../EmployeeHeader'

function CreateDeMobilisation({handleChangeData, data, onSubmit}) {
  const [ types, setTypes ] = useState([])
  const [ classOfTravel, setClassOfTravel ] = useState([])
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ docRoute, setDocRoute ] = useState(null)
  const [ showAlertModal, setShowAlertModal ] = useState(false)
  const [ duplicatedData, setDuplicatedData ] = useState([])
  const [ pendingInfo, setPendingInfo ] = useState(null)
  const [ deMobilisationData, setDeMobilisationData ] = useState({
    deMobilisationData: {},
    requestData: {}
  })
  const [ nextGroup, setNextGroup ] = useState(null)

  const [ form ] = AntForm.useForm()
  const { state } = useContext(AuthContext)
  const { empId } = useParams()
  const navigate =  useNavigate()

  useEffect(() => {
    if(state.userProfileData){
      form.setFieldValue('employerId', state.userProfileData?.EmployerId)
    }
  },[state.userProfileData])
  
  useEffect(() => {
    axios({
      method: 'get',
      url: 'tas/requestdemobilisationtype?active=1'
    }).then((res) => {
      setTypes(res.data)
    }).catch((err) => {

    })
    getDocumentMembers()
  },[])

  const getDocumentMembers = (e) => {
    axios({
      method: 'get',
      url: `/tas/requestgroupconfig/groupandmembers/bytype/De Mobilisation?empId=${empId}`,
    }).then((res) => {
      setPendingInfo(res.data)
      let firstGroup = res.data.groups[0]
      let tmp = []
      firstGroup?.groupMembers.map((item) => {
        tmp.push({value: item.employeeId, label: item.fullName})
      })
      form.setFieldValue('assignEmployeeOptions', tmp)
      form.setFieldValue('files', [])
      form.setFieldValue('nextGroupName', firstGroup?.GroupName)
      form.setFieldValue('nextGroupId', firstGroup?.id)

      let routes = [...res.data.groups]
      routes.unshift({GroupName: 'Requester', id: 0})
      routes.push({GroupName: 'Completed', id: 99999})
      setDocRoute(routes)
      setNextGroup(firstGroup)
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
    setActionLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocument/checkduplicate/De Mobilisation/${empId}`
    }).then((res) => {
      setDuplicatedData(res.data)
      if(res.data.length === 0){
        axios({
          method: 'post',
          url: 'tas/requestdemobilisation',
          data: {
            deMobilisationData: {
              completionDate: values.completionDate ? dayjs(values.completionDate).format('YYYY-MM-DD') : null,
              employerId: values.employerId ? values.employerId : null,
              requestDeMobilisationTypeId: values.requestDeMobilisationTypeId ? values.requestDeMobilisationTypeId : null,
              comment: values.employerComment
            },
            requestData: {
              nextGroupId: values.nextGroupId,
              comment: values.comment,
              assignedEmployeeId: values.assignedEmployeeId ? values.assignedEmployeeId : null,
              employeeId: empId,
              action: 'Submitted',
            },
          }
        }).then((res) => {
          navigate('/request/task')
        }).catch((err) => {
  
        }).then(() => setActionLoading(false))
      }else{
        setShowAlertModal(true)
      }
    }).catch(() => {

    })
  }

  const handleDraft = () => {
    axios({
      method: 'post',
      url: 'tas/requestdocumentdemobilisation',
      data: {
        deMobilisationData: deMobilisationData.deMobilisationData,
        requestData: {
          ...deMobilisationData.requestData,
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
    <div>
      <Button
        className='mb-3' 
        onClick={() => navigate('/request/de-monilisation')} 
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
          initValues={{
            completionDate: null, 
            requestDeMobilisationTypeId: null,
            employerId: null,
            employerComment: '',
            assignedEmployeeId: null,
            comment: '',
            NextGroupName: '',
            NextGroupId: ''
          }}
          onFinish={handleSubmit}
        >
          <Accordion title='De-Mobilisation Details' className='border col-span-12'>
            <div className='flex flex-col gap-4 p-4'>
              <div className='flex'> 
                <div className='w-[180px]'><span className='text-red-400'>*</span>Date of Completion :</div> 
                <Form.Item 
                  name='completionDate' 
                  className='mb-0'
                  rules={[{required: true, message: 'Date of Completion is required'}]}
                >
                  <DatePicker />
                </Form.Item>
              </div>
              <div className='flex'> 
                <div className='w-[180px]'><span className='text-red-400'>*</span>De-Mobilisation Type :</div> 
                <Form.Item 
                  name='requestDeMobilisationTypeId' 
                  className='mb-0 w-[300px]'
                  rules={[{required: true, message: 'De-Mobilisation Type is required'}]}
                >
                  <Select 
                    options={types.map((item) => ({label: `${item.Code} ${item.Description}`, value: item.Id}))}
                    filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                    showSearch
                    allowClear
                  />
                </Form.Item>
              </div>
              <div className='flex'>
                <div className='w-[180px]'>
                  <span className='text-red-400'>*</span>Employer :
                </div> 
                <Form.Item 
                  name='employerId' 
                  className='mb-0 w-[400px]' 
                  rules={[{required: true, message: 'Employer is required'}]}
                >
                  <Select 
                    className='w-[500px]' 
                    options={state.referData?.employers}
                    filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                    showSearch
                    allowClear
                    // onChange={(e) =>  handleChangeData((prevData) => ({
                    //   ...prevData,
                    //   deMobilisationData: {
                    //     ...prevData.deMobilisationData,
                    //     positionId: e,
                    //   }
                    // }))} 
                  />
                </Form.Item>
              </div>
              <div className='flex'>
                <div className='w-[180px]'>
                  Comments
                </div> 
                <Form.Item name='employerComment' className='mb-0'>
                  <Input.TextArea size='small' className='w-[500px]' maxLength={300} showCount/>
                </Form.Item>
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
                {
                  nextGroup?.GroupTag === 'linemanager' &&
                  <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.assignEmployeeOptions !== curValues.assignEmployeeOptions}>
                    {({getFieldValue}) => {
                      return(
                        <Form.Item
                          name='assignedEmployeeId' 
                          label='Send to:' 
                          rules={[{required: true, message: 'Assign Employee is required'}]}
                          className='w-[300px]'
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
        {/* <Accordion title='Document History & Comments'>
          <div className='flex flex-col gap-4 p-3'>
          </div>
        </Accordion> */}
      </div>
      <div className='flex justify-end mt-4 gap-4'>
        <Button type='primary' loading={actionLoading} onClick={() => form.submit()}>Submit</Button>
        <Button onClick={handleDraft}>Draft</Button>
        <Button>Cancel</Button>
      </div>
      <DuplicatedDocument
        open={showAlertModal}
        onCancel={() => setShowAlertModal(false)}
        data={duplicatedData}
      />
    </div>
    
  )
}

export default CreateDeMobilisation