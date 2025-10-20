import React, { useContext, useEffect, useMemo, useState } from 'react'
import axios from 'axios'
import { Input, Select, Form as AntForm } from 'antd'
import { Accordion, Button, DocumentTimeline, Form, ProfileCalendar, RequestDocumentHeader } from 'components'
import RescheduleExistingTravel from './RescheduleExistingTravel'
import RemoveExistingTravel from './RemoveExistingTravel'
import { useLoaderData, useNavigate, useParams } from 'react-router-dom'
import Attachments from './Attachments'
import NewTravel from './NewTravel'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'
import checkPermission from 'utils/checkPermission'
import generateColor from 'utils/generateColor'

function SiteTravel() {
  const [ documentDetail, setDocumentDetail ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ actionType, setActionType ] = useState(null)
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ currentGroup, setCurrentGroup ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ routeTimeline, setRouteTimeline ] = useState([])
  const [ userInfo, setUserInfo ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ profileData, setProfileData ] = useState(null)

  const { data } = useLoaderData()
  const { state, action } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const [ newTravelForm ] = AntForm.useForm()
  const [ removeTravelForm ] = AntForm.useForm()
  const navigate = useNavigate()
  const { documentId, documentTag } = useParams()
  const screenName = `requestDocumentDetail_${documentId}`

  useEffect(() => {
    if(data.Id){
      setDocumentDetail(data)
    }
  },[data])

  useEffect(() => {
    if(documentId && data.Id){
      setLoading(true)
      action.changeMenuKey('/request/task')
      axios.all([
        `tas/employee/profile/${data.EmployeeId}`,
        `tas/requestdocumenthistory/${documentId}`,
        `tas/requestdocument/myinfo`,
        `/tas/requestgroupconfig/groupandmembers/${documentId}`,
        `/tas/requestgroupconfig/documentroute/${documentId}`,
        ].map((endpoint) => axios.get(endpoint)))
      .then(axios.spread((employeeData, history, myinfo, groupMem, docRoute) => {
        action.saveUserProfileData(employeeData.data)
        setProfileData(employeeData.data)
        setRouteTimeline(history.data)
        setUserInfo(myinfo.data)
        calculateNextGroup(docRoute.data, groupMem.data)
      })).catch(() => {

      }).then(() => setLoading(false))
    }
  },[documentId, data])

  useEffect(() => {
    if(state.connectionMultiViewer && state.userInfo){
      joinRoom()
      state.connectionMultiViewer.on('UsersInScreen', (res, users) => {
        let multiViewers = users.map((item) => ({
          name: item.userName?.split('_')[0],
          role: item.userName?.split('_')[1],
          color: generateColor()
        }))
        action.setMultiViewers(multiViewers.filter(user => user.name !== `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}`))
      });
    }

    return () => {
      state.connectionMultiViewer && leaveRoom()
    }
  },[state.connectionMultiViewer, state.userInfo])

  const joinRoom = () => {
    const userName = `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`
    try {
      state.connectionMultiViewer?.invoke( 'JoinScreen', userName, screenName )
    }
    catch(e) {
    }
  }

  const leaveRoom = () => {
    action.setMultiViewers([])
    const userName = `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`
    state.connectionMultiViewer?.invoke( 'LeaveScreen', userName, screenName )
  }

  const getDocumentDetail = () => {
    axios({
      method: 'get',
      url: `tas/requestsitetravel/${documentTag}/${documentId}`,
    }).then( async (res) => {
      if(res.data.Id){
        setDocumentDetail(res.data)
      }
    }).catch((err) => {

    })
  }
  
  const calculateNextGroup = (documentRoutes, groupMembers) => {
    const curGroup = documentRoutes.find((group) => group.CurrentPosition === 1);
    setCurrentGroup(curGroup)
    if(documentTag === 'reschedule'){
      if(curGroup?.GroupTag === 'Requester'){
        const nextGroup = groupMembers.find((group) => group?.GroupTag === 'linemanager')
        if(nextGroup){
          let tmp = []
          nextGroup.groupMembers.map((item) => {
            tmp.push({value: item.employeeId, label: item.fullName})
          })
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.id)
          form.setFieldValue('assignEmployeeOptions', tmp)
        }
        setNextGroup(nextGroup);
      }
    }
    else if(documentTag === 'addtravel'){
      ///////// if current position is TRAVEL TEAM ///////////
     if(curGroup?.GroupTag === 'Requester'){
        const nextGroup = groupMembers.find((group) => group?.GroupTag === 'linemanager')
        if(nextGroup){
          let tmp = []
          nextGroup.groupMembers.map((item) => {
            tmp.push({value: item.employeeId, label: item.fullName})
          })
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.id)
          form.setFieldValue('assignEmployeeOptions', tmp)
        }
        setNextGroup(nextGroup);
      }
    }
    else {
      if(curGroup?.GroupTag === 'Requester'){
        const nextGroup = groupMembers.find((group) => group?.GroupTag === 'linemanager')
        if(nextGroup){
          let tmp = []
          nextGroup.groupMembers.map((item) => {
            tmp.push({value: item.employeeId, label: item.fullName})
          })
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.id)
          form.setFieldValue('assignEmployeeOptions', tmp)
        }
        setNextGroup(nextGroup);
      }
    }
  }

  const handleSubmit = () => {
    const values = form.getFieldsValue()
    setActionLoading(true)
    if(documentTag === 'remove'){
      if(actionType === 'Cancel'){
        axios({
          method: 'put',
          url: `tas/requestdocument/${actionType}`,
          data: {
            nextGroupId: values.nextGroupId,
            comment: values.comment,
            assignEmployeeId: values.assignedEmployeeId,
            id: documentId,
            impersonateUserId: null,
          },
        }).then((res) => {
          setShowPopup(false)
          navigate('/guest/request')
        }).catch((err) => {
    
        }).then(() => setActionLoading(false))
      }else{
        const removeValues = removeTravelForm.getFieldsValue()
        axios({
          method: 'put',
          url: 'tas/requestsitetravel/removetravel',
          data: {
            "id": documentDetail?.TravelData.Id,
            "firstScheduleId": removeValues.firstTransport.flightId,
            "lastScheduleId": removeValues.lastTransport.flightId,
            "shiftId": removeValues.ShiftId,
            "RoomId": removeValues.Room?.RoomId ? removeValues.Room?.RoomId : null,
            "CostCodeId": removeValues.CostCodeId,
          }
        }).then((res) => {
          axios({
            method: 'put',
            url: `tas/requestdocument/${actionType}`,
            data: {
              nextGroupId: values.nextGroupId,
              comment: values.comment,
              assignEmployeeId: values.assignedEmployeeId,
              id: documentId,
              impersonateUserId: null,
            },
          }).then((res) => {
            setShowPopup(false)
            navigate('/guest/request')
          }).catch((err) => {
      
          }).then(() => setActionLoading(false))
        }).catch(() => {
    
        })
      }
    }else{
      axios({
        method: 'put',
        url: `tas/requestdocument/${actionType}`,
        data: {
          nextGroupId: values.nextGroupId,
          comment: values.comment,
          assignEmployeeId: values.assignedEmployeeId,
          id: documentId,
          impersonateUserId: null,
        },
      }).then((res) => {
        setShowPopup(false)
        navigate('/guest/request')
      }).catch((err) => {

      }).then(() => setActionLoading(false))
    }
  }

  const disabledStatus = useMemo(() => {
    const isDisabled = (documentDetail?.CurrentStatus === 'Completed' || documentDetail?.CurrentStatus === 'Cancelled')
    return isDisabled || checkPermission.getDisabledStatus({
      documentData: documentDetail,
      userData: userInfo,
      groupTag: currentGroup?.GroupTag,
    })
  },[documentDetail, userInfo, currentGroup])

  const renderTravelForm = () => {
    switch (documentTag) {
      case 'addtravel': return (
        <Accordion title='New Travel Request' className='border col-span-12 xl:col-span-7 2xl:col-span-6'>
          <div className='p-4'>
            <NewTravel
              form={newTravelForm} 
              documentDetail={documentDetail}
              refreshData={getDocumentDetail}
              disabled={disabledStatus}
              currentGroup={currentGroup}
              travelData={documentDetail?.TravelData}
            />
          </div>
        </Accordion>
      )
      case 'reschedule': return (
        <Accordion title='Reschedule Existing Travel' className='border col-span-12 xl:col-span-7 2xl:col-span-6'>
          <div className='p-4'>
            <RescheduleExistingTravel
              documentDetail={documentDetail}
              data={documentDetail?.TravelData}
              refreshData={getDocumentDetail}
              disabled={disabledStatus}
              currentGroup={currentGroup}
              travelData={documentDetail?.TravelData}
            />
          </div>
        </Accordion>
      )
      case 'remove': return(
        <Accordion title='Remove Existing Travel' className='border col-span-12 xl:col-span-7 2xl:col-span-6'>
          <div className='p-4'>
            <RemoveExistingTravel
              form={removeTravelForm}
              editData={documentDetail?.TravelData}
              currentGroup={currentGroup}
              disabled={disabledStatus}
            />
          </div>
        </Accordion>
      ) 
    }
  }

  const renderButtons = (groupTag) => {
    if(!groupTag){
      return null
    }
    else if(groupTag !== 'complete'){
      return(
        <>
         {
            currentGroup.GroupTag === 'Requester' ?
            <>
              <Button
                htmlType='button'
                type='primary'
                onClick={() => clickActionButtons('Approve')}
                disabled={actionLoading}
                loading={actionLoading && actionType === 'Approve'}
              >
                Submit
              </Button>
              <Button
                htmlType='button'
                type='danger'
                onClick={() => clickActionButtons('Cancel')}
                disabled={actionLoading}
              >
                Cancel Request
              </Button>
            </>
            :
            null
          }
        </>
      )
    }
  }

  const clickActionButtons = (type) => {
    setActionType(type);
    if(type === 'Approve'){
      form.submit()
    }else{
      setShowPopup(true)
    }
  }

  const renderDialogText = (type) => {
    switch (type) {
      case 'Approve':
        return (<>
          <div>Are you sure you want to approve this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='success'onClick={() => form.submit()} loading={actionLoading}>Yes</Button>
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Cancel':
        return (<>
          <div>Are you sure you want to cancel this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='danger'onClick={handleSubmit} loading={actionLoading}>Yes</Button>
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
        </>)
    }
  }

  if(!data.Id){
    return(
      <div className='bg-white rounded-ot px-4 py-3 min-h-[300px] flex flex-col justify-center items-center'>
        <div className='text-xl'>Not Found Document</div>
        <Button htmlType='button' type='primary' className='mt-5' onClick={() => navigate('/request/task')}>Back to List</Button>
      </div>
    )
  }

  return (
    <div>
      {
        loading ? 
        <>
          <div className='animate-skeleton h-[230px] flex rounded-ot mb-5 shadow-md'>
          </div>
          <div className='animate-skeleton h-[400px] flex rounded-ot shadow-md'></div>
        </>
        :
        <div className='relative'>
          <RequestDocumentHeader profileData={profileData} documentDetail={documentDetail}/>
          <Accordion title='Travel Calendar' className='border mt-3'>
            <div className='p-3'>
              <ProfileCalendar profile={profileData}/>
            </div>
          </Accordion>
          <div className='grid grid-cols-12 gap-3 mt-3'>
            { renderTravelForm() }
            <Accordion title='Request Document' className='border col-span-12 xl:col-span-5 2xl:col-span-6'>
              <Form
                className='grid grid-cols-12 ' 
                form={form}
                onFinish={handleSubmit}
              >
                <div className='flex flex-col gap-1 px-4 pb-2 pt-4 col-span-12'>
                  <div className='border border-[#ffe58f] bg-[#fffbe6] rounded-md px-3 py-2'>
                    <div className='font-medium leading-none mb-1'>Reason</div>
                    <div>{documentDetail?.TravelData?.Reason}</div>
                  </div>
                  {
                    checkPermission.getActionPermission({
                      documentData: documentDetail,
                      userData: userInfo, 
                      groupTag: currentGroup?.GroupTag,
                      userGroupIds: userInfo?.ApprovalGroupIds
                    }) ?
                    <>
                      <div className='mt-2'>
                        Comments:
                        <Form.Item className='mb-0' name='comment'>
                          <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                        </Form.Item>
                      </div>
                      <div className='flex mt-2 mb-2 gap-5'>
                        {
                          nextGroup &&
                          <>
                            <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.nextGroupName !== curValues.nextGroupName}>
                              {({getFieldValue}) => {
                                return(
                                  <Form.Item name='nextGroupId' className='mb-0 leading-none'>
                                    Next Approval: <span className='font-bold'>{getFieldValue('nextGroupName')}</span>
                                  </Form.Item>
                                )
                              }}
                            </Form.Item>
                            <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.assignEmployeeOptions !== curValues.assignEmployeeOptions}>
                              {({getFieldValue}) => {
                                return(
                                  <Form.Item
                                    name='assignedEmployeeId' 
                                    label='Send to:' 
                                    rules={[{required: true, message: 'Assign Employee is required'}]}
                                    className='w-[300px] mb-0'
                                    labelCol={{flex: '70px'}}
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
                          </>
                        }
                      </div>
                      <div className='flex justify-end gap-4'>
                        <div className='flex items-center gap-3 justify-end'>
                          {renderButtons(nextGroup?.GroupTag)}
                        </div>
                      </div>
                    </>
                    :
                    (userInfo?.EmployeeId === documentDetail?.RequestUserId) && !((documentDetail?.CurrentStatus === 'Completed') || (documentDetail?.CurrentStatus === 'Cancelled')) ?
                    <>
                      <div className='mt-2'>
                        Comments:
                        <Form.Item className='mb-0' name='comment'>
                          <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                        </Form.Item>
                      </div>
                      <div className='flex justify-end mt-3'>
                        <Button
                          htmlType='button'
                          type='danger'
                          onClick={() => clickActionButtons('Cancel')}
                          disabled={actionLoading}
                        >
                          Cancel Request
                        </Button>
                      </div>
                    </>
                    : null
                  }
                </div>
              </Form>
            </Accordion>
            <Accordion defaultOpen={false} title='Attachments' className='border col-span-12'>
              <div className='p-4'>
                <Attachments 
                  showTitle={false}
                  disabled={disabledStatus}
                />
              </div>
            </Accordion>
            <Accordion defaultOpen={false} title='Document History & Comments, Document Route' className='border col-span-12 rounded-ot overflow-hidden'>
              <div className='grid grid-cols-12 items-start gap-4 p-4'>
                <DocumentTimeline
                  data={routeTimeline}
                />
              </div>
            </Accordion>
          </div>
        </div>
      }
      <Popup
        visible={showPopup}
        showTitle={false}
        height={120}
        width={350}
      >
        <div>
          {renderDialogText(actionType)}
        </div>
      </Popup>
    </div>
  )
}

export default SiteTravel