import React, { useContext, useEffect, useState } from 'react'
import axios from 'axios'
import { Input, Select, Form as AntForm, notification } from 'antd'
import { Accordion, Button, DocumentTimeline, Form, ProfileCalendar, RequestDocumentHeader, Tooltip } from 'components'
import RescheduleExistingTravel from './RescheduleExistingTravel'
import RemoveExistingTravel from './RemoveExistingTravel'
import { useLoaderData, useNavigate, useParams } from 'react-router-dom'
import Attachments from './Attachments'
import NewTravel from './NewTravel'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'
import checkPermission from 'utils/checkPermission'
import generateColor from 'utils/generateColor'
import useQuery from 'utils/useQuery'
import ProfileForm from './ProfileForm'

function ExternalTravel() {
  const [ documentDetail, setDocumentDetail ] = useState(null)
  const [ groupAndMembers, setGroupAndMembers ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ isNotFound, setIsNotFound ] = useState(false)
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
  const [ reScheduleForm ] = AntForm.useForm()
  const [ removeTravelForm ] = AntForm.useForm()
  const [ profileForm ] = AntForm.useForm()
  const navigate = useNavigate()
  const { empId, documentId, documentTag } = useParams()
  const query = useQuery()
  const screenName = `requestDocumentDetail_${documentId}`
  const [api, contextHolder] = notification.useNotification();
  
  // window.onbeforeunload = function () {
  //   return "Do you really want to close?";
  // };

  // window.onbeforeunload = function () {
  //   return leaveRoom();
  // };

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
        ].map((endpoint) => axios.get(endpoint)))
      .then(axios.spread((employeeData, history, myinfo) => {
        action.saveUserProfileData(employeeData.data)
        setProfileData(employeeData.data)
        setRouteTimeline(history.data)
        setUserInfo(myinfo.data)
      })).catch(() => {

      }).then(() => setLoading(false))
    }
  },[documentId])

  useEffect(() => {
    if(state.connectionMultiViewer && state.userInfo){
      joinRoom()
      state.connectionMultiViewer.on('UsersInScreen', (res, users) => {
        let multiViewers = users.map((item) => ({
          name: item.userName.split('_')[0],
          role: item.userName.split('_')[1],
          color: generateColor()
        }))
        action.setMultiViewers(multiViewers.filter(user => user.name !== `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}`))
      });
    }

    return () => {
      state.connectionMultiViewer && leaveRoom()
    }
  },[state.connectionMultiViewer, state.userInfo])

  useEffect(() => {
    if(documentDetail){
      getDocumentMembers()
    }
  },[documentDetail])

  useEffect(() => {
    if(groupAndMembers){
      getDocumentRoute()
    }
  },[groupAndMembers])

  const joinRoom = () => {
    try {
      state.connectionMultiViewer?.invoke(
        'JoinScreen', 
        `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`,
        screenName
      )
    }
    catch(e) {
    }
  }

  const leaveRoom = () => {
    action.setMultiViewers([])
    state.connectionMultiViewer?.invoke(
      'LeaveScreen',
      `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`,
      screenName
    )
  }

  const getProfile = (employeeId) => {
    axios({
      method: 'get',
      url: `tas/employee/profile/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
      setProfileData(res.data)
    }).catch((err) => {
    })
  }

  const getDocumentDetail = () => {
    axios({
      method: 'get',
      url: `tas/requestsitetravel/${documentTag}/${documentId}`,
    }).then( async (res) => {
      if(res.data.Id){
        setDocumentDetail(res.data)
      }
      else{
        setIsNotFound(true)
      }
    }).catch((err) => {

    })
  }

  const getDocumentMembers = (e) => {
    axios({
      method: 'get',
      url: `/tas/requestgroupconfig/groupandmembers/${documentId}`,
    }).then((res) => {
      setGroupAndMembers(res.data)
    })
  } 

  const getDocumentRoute = () => {
    axios({
      method: 'get',
      url: `/tas/requestgroupconfig/documentroute/${documentId}`,
    }).then((res) => {
      calculateNextGroup(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }
  
  const calculateNextGroup = (documentRoutes) => {
    if(documentTag === 'reschedule'){
      let currentGroup = documentRoutes.find((group) => group.CurrentPosition === 1);
      setCurrentGroup(currentGroup)
  
      ///////// if current position is LINE MANAGER ///////////
  
      if(currentGroup?.GroupTag === 'linemanager'){
        if(documentDetail.EmployeeActive === 1){
          let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'travelsite')
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
        }else{
          let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'dataapproval')
          if(nextGroup){
            let tmp = []
            nextGroup.groupMembers.map((item) => {
              tmp.push({value: item.employeeId, label: item.fullName})
            })
            form.setFieldValue('nextGroupName', nextGroup?.GroupName)
            form.setFieldValue('nextGroupId', nextGroup?.id)
            form.setFieldValue('assignEmployeeOptions', tmp)
            setNextGroup(nextGroup)
          }
        }
      }
  
      ///////// if current position is DATA APPROVAL ///////////
  
      else if(currentGroup?.GroupTag === 'dataapproval'){
        let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'travelsite')
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
      
      ///////// if current position is TRAVEL TEAM ///////////
      
      else if(currentGroup?.GroupTag === 'travelsite'){
        setNextGroup({GroupTag: 'complete'})
      }
      else if(currentGroup?.GroupTag === 'Requester'){
        if(documentDetail.DaysAway >= 7){
          setNextGroup(null);
        }else{
          let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'linemanager')
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

    /////////   ADD EXTERNAL TRAVEL     /////////
    else if(documentTag === 'addtravel'){
      let currentGroup = documentRoutes.find((group) => group.CurrentPosition === 1);
      setCurrentGroup(currentGroup)
  
      ///////// if current position is LINE MANAGER ///////////
  
      if(currentGroup?.GroupTag === 'linemanager'){
        if(documentDetail.EmployeeActive === 1){
          let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'travelsite')
          if(nextGroup){
            let tmp = []
            nextGroup.groupMembers.map((item) => {
              tmp.push({value: item.employeeId, label: item.fullName})
            })
            form.setFieldValue('nextGroupName', nextGroup?.GroupName)
            form.setFieldValue('nextGroupId', nextGroup?.id)
            form.setFieldValue('assignEmployeeOptions', tmp)
            setNextGroup(nextGroup);
          }
        }else{
          let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'dataapproval')
          if(nextGroup){
            let tmp = []
            nextGroup.groupMembers.map((item) => {
              tmp.push({value: item.employeeId, label: item.fullName})
            })
            form.setFieldValue('nextGroupName', nextGroup?.GroupName)
            form.setFieldValue('nextGroupId', nextGroup?.id)
            form.setFieldValue('assignEmployeeOptions', tmp)
            setNextGroup(nextGroup)
          }
        }
      }
  
      ///////// if current position is DATA APPROVAL ///////////
  
      else if(currentGroup?.GroupTag === 'dataapproval'){
        let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'travelsite')
        if(nextGroup){
          let tmp = []
          nextGroup.groupMembers.map((item) => {
            tmp.push({value: item.employeeId, label: item.fullName})
          })
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.id)
          form.setFieldValue('assignEmployeeOptions', tmp)
          setNextGroup(nextGroup);
        }
      }
      
      ///////// if current position is TRAVEL TEAM ///////////
      
      else if(currentGroup?.GroupTag === 'travelsite'){
        setNextGroup({GroupTag: 'complete'})
      }
      else if(currentGroup?.GroupTag === 'Requester'){
        // if(documentDetail.DaysAway >= 7){
        //   setNextGroup(null);
        // }else{
          let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'linemanager')
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
        // }
        // setNextGroup({GroupTag: 'complete'})
      }
    }
    else {
      /////////    REMOVE EXTERNAL TRAVEL     ///////////
      let currentGroup = documentRoutes.find((group) => group.CurrentPosition === 1);
      setCurrentGroup(currentGroup)

      if(currentGroup?.GroupTag === 'linemanager'){
        let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'travelsite')
        if(nextGroup){
          let tmp = []
          nextGroup.groupMembers.map((item) => {
            tmp.push({value: item.employeeId, label: item.fullName})
          })
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.id)
          form.setFieldValue('assignEmployeeOptions', tmp)
        }
        setNextGroup(nextGroup)
      }else if(currentGroup?.GroupTag === 'Requester'){
        let nextGroup = groupAndMembers.find((group) => group?.GroupTag === 'linemanager')
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
      else{
        setNextGroup({GroupTag: 'complete'})
      }
    }

  }

  const handleSubmit = () => {
    const values = form.getFieldsValue()
    setActionLoading(true)
    if(actionType === 'Complete'){
      axios({
        method: 'put',
        url: `tas/requestexternaltravel/${documentTag}/${actionType}/${documentId}`,
        data: {
          comment: values.comment,
          documentId: documentId,
          impersonateUserId: query.get('impersonateuser'),
        },
      }).then((res) => {
        setShowPopup(false)
        if(query.get('impersonateuser')){
          navigate(`/request/impersonateuser/${query.get('impersonateuser')}`)
        }else{
          navigate('/request/task')
        }
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }else{
      if(currentGroup?.GroupTag === 'dataapproval'){
        //////////       SEND PROFILE DATA       ///////////
        //////      Approve Document       ///////
        axios({
          method: 'put',
          url: `tas/requestdocument/${actionType}`,
          data: {
            nextGroupId: values.nextGroupId,
            comment: values.comment,
            assignEmployeeId: values.assignedEmployeeId,
            id: documentId,
            impersonateUserId: query.get('impersonateuser'),
          },
        }).then((res) => {
          setShowPopup(false)
          setActionLoading(false)
          if(query.get('impersonateuser')){
            navigate(`/request/impersonateuser/${query.get('impersonateuser')}`)
          }else{
            navigate('/request/task')
          }
        }).catch((err) => {
          setActionLoading(false)
        })
      }
      ////      Approve Document       ////
      else{
        if(documentTag === 'remove'){
          axios({
            method: 'put',
            url: `tas/requestdocument/${actionType}`,
            data: {
              nextGroupId: values.nextGroupId,
              comment: values.comment,
              assignEmployeeId: values.assignedEmployeeId,
              id: documentId,
              impersonateUserId: query.get('impersonateuser'),
            },
          }).then((res) => {
            setShowPopup(false)
            if(query.get('impersonateuser')){
              navigate(`/request/impersonateuser/${query.get('impersonateuser')}`)
            }else{
              navigate('/request/task')
            }
          }).catch((err) => {
      
          }).then(() => setActionLoading(false))
        }else{
          axios({
            method: 'put',
            url: `tas/requestdocument/${actionType}`,
            data: {
              nextGroupId: values.nextGroupId,
              comment: values.comment,
              assignEmployeeId: values.assignedEmployeeId,
              id: documentId,
              impersonateUserId: query.get('impersonateuser'),
            },
          }).then((res) => {
            setShowPopup(false)
            if(query.get('impersonateuser')){
              navigate(`/request/impersonateuser/${query.get('impersonateuser')}`)
            }else{
              navigate('/request/task')
            }
          }).catch((err) => {
      
          }).then(() => setActionLoading(false))
        }
        
      }
    }
  }

  const renderTravelForm = () => {
    switch (documentTag) {
      case 'addtravel': return (
        <Accordion title='New Travel Request' className='border col-span-12 xl:col-span-7 2xl:col-span-6'>
          <div className='p-4'>
            <NewTravel
              documentDetail={documentDetail}
              refreshData={getDocumentDetail}
              disabled={checkPermission.getDisabledStatus({
                documentData: documentDetail,
                userData: userInfo,
                groupTag: currentGroup?.GroupTag,
                userGroupIds: userInfo?.ApprovalGroupIds,
              })}
            />
          </div>
        </Accordion>
      )
      case 'reschedule': return (
        <Accordion title='Reschedule Existing Travel' className='border col-span-12 xl:col-span-7 2xl:col-span-6'>
          <div className='p-4'>
            <RescheduleExistingTravel 
              form={reScheduleForm} 
              documentDetail={documentDetail}
              data={documentDetail?.TravelData}
              refreshData={getDocumentDetail}
              disabled={checkPermission.getDisabledStatus({
                documentData: documentDetail, 
                userData: userInfo, 
                userGroupIds: userInfo?.ApprovalGroupIds,
                groupTag: currentGroup?.GroupTag,
              })}
              currentGroup={currentGroup}
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
              disabled={checkPermission.getDisabledStatus({
                documentData: documentDetail, 
                userData: userInfo, 
                groupTag: currentGroup?.GroupTag,
                userGroupIds: userInfo?.ApprovalGroupIds,
              })}
            />
          </div>
        </Accordion>
      ) 
    }
  }

  const renderButtons = (groupTag, declineDisabled) => {
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
            <>
              <Button
                htmlType='button'
                type='success'
                onClick={() => clickActionButtons('Approve')}
                disabled={actionLoading}
                loading={actionLoading && actionType === 'Approve'}
              >
                Approve
              </Button>
              <Button
                htmlType='button'
                type='danger'
                onClick={() => clickActionButtons('Cancel')}
                disabled={actionLoading}
              >
                Cancel Request
              </Button>
              <Tooltip title={declineDisabled ? 'Please fill comment field!' : ''}>
                <div>
                  <Button
                    htmlType='button'
                    onClick={() => clickActionButtons('Decline')}
                    disabled={declineDisabled || actionLoading}
                    loading={actionLoading && actionType === 'Decline'}
                  >
                    Decline
                  </Button>
                </div>
              </Tooltip>
            </>
          }
        </>
      )
    }else{
      return(
        <>
          <Button
            htmlType='button'
            type='success'
            onClick={() => clickActionButtons('Complete')}
            disabled={actionLoading}
            loading={actionLoading && actionType === 'Complete'}
          >
            Complete
          </Button>
          <Button
            htmlType='button'
            type='danger'
            onClick={() => clickActionButtons('Cancel')}
            disabled={actionLoading}
          >
            Cancel Request
          </Button>
          <Tooltip title={declineDisabled ? 'Please fill comment field!' : ''}>
            <div>
              <Button
                htmlType='button'
                onClick={() => clickActionButtons('Decline')}
                disabled={declineDisabled || actionLoading}
                loading={actionLoading && actionType === 'Decline'}
              >
                Decline
              </Button>
            </div>
          </Tooltip>
        </>
      )
    }
  }

  const clickActionButtons = (type) => {
    setActionType(type);
    if(type === 'Approve' || type === 'Complete' || type === 'Decline'){
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
      case 'Save':
        return (<>
          <div>Are you sure you want to save this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='primary'onClick={handleSubmit} loading={actionLoading}>Yes</Button>
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Decline':
        return (<>
          <div>Are you sure you want to decline this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button onClick={handleSubmit} loading={actionLoading}>Yes</Button >
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Complete': return (<div>
          <div>Are you sure you want to complete this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='success' onClick={() => form.submit()} loading={actionLoading}>Yes</Button> 
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
        </div>)
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

  const handleFinishMainForm = (name, { values, forms}) => {
    // console.log('asdasd', name, values, forms);
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
          <AntForm.Provider 
            onFormFinish={handleFinishMainForm}
          >
            {
              currentGroup?.GroupTag === 'dataapproval' && !checkPermission.getDisabledStatus({
                documentData: documentDetail,
                userData: userInfo,
                groupTag: currentGroup?.GroupTag
              }) ?
              <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden mt-3' title='Profile'>
                <div className='p-4 flex flex-col gap-5'>
                  <ProfileForm
                    data={profileData}
                    form={profileForm}
                    currentGroup={currentGroup}
                    className='shadow-none border'
                    refreshData={getProfile}
                    disabled={checkPermission.getDisabledStatus({
                      documentData: documentDetail,
                      userData: userInfo,
                      groupTag: currentGroup?.GroupTag
                    })}
                  />
                </div>
              </Accordion>
              : null
            }

            <div className='grid grid-cols-12 gap-3 mt-3'>
              {
                currentGroup?.GroupTag === 'dataapproval' ?
                null :
                renderTravelForm()
              }
              {
                checkPermission.getActionPermission({
                  documentData: documentDetail,
                  userData: userInfo, 
                  groupTag: currentGroup?.GroupTag,
                  userGroupIds: userInfo?.ApprovalGroupIds
                }) ?
                <Accordion title='Request Document' className='border col-span-12 xl:col-span-5 2xl:col-span-6'>
                  <Form
                    className='grid grid-cols-12 ' 
                    form={form}
                    onFinish={handleSubmit}
                  >
                    <div className='flex flex-col gap-1 px-4 pb-2 pt-4 col-span-12'>
                      {
                        documentDetail?.TravelData?.Reason ?
                        <div className='border border-[#ffe58f] bg-[#fffbe6] rounded-md px-3 py-2'>
                          <div className='font-medium leading-none mb-1'>Reason</div>
                          <div>{documentDetail.TravelData.Reason}</div>
                        </div>
                        : null
                      }
                      <div className='mt-2'>
                        Comments:
                        <Form.Item className='mb-0' name='comment'>
                          <Input.TextArea autoSize={{minRows: 3, maxRows: 5}}/>
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
                            {
                              nextGroup?.GroupTag === 'linemanager' &&
                              <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.assignEmployeeOptions !== curValues.assignEmployeeOptions}>
                                {({getFieldValue}) => {
                                  return(
                                    <Form.Item
                                      name='assignedEmployeeId' 
                                      label='Send to:' 
                                      rules={[
                                        {
                                          required: true,
                                          message: 'Assign Employee is required'
                                        }
                                      ]}
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
                          </>
                        }
                      </div>
                      <div className='flex justify-end gap-4'>
                        <div className='flex items-center gap-3 justify-end'>
                          <Form.Item noStyle shouldUpdate={(pre, cur) => pre.comment !== cur.comment}>
                            {({getFieldValue}) => {
                              const declineDisabled = !Boolean(getFieldValue('comment'))
                              return renderButtons(nextGroup?.GroupTag, declineDisabled)
                            }}
                          </Form.Item>
                        </div>
                      </div>
                    </div>
                  </Form>
                </Accordion>
                :
                (userInfo?.EmployeeId === documentDetail?.RequestUserId) && documentDetail?.CurrentStatus === 'Submitted' ?
                <Accordion title='Request Document' className='border col-span-12 xl:col-span-5 2xl:col-span-6'>
                  <Form
                    className='grid grid-cols-12 ' 
                    form={form}
                    initValues={{
                    }}
                    onFinish={handleSubmit}
                  >
                    <div className='flex flex-col gap-1 px-4 pt-4 pb-2 col-span-12'>
                      {
                        documentDetail?.TravelData?.Reason ?
                        <div className='border border-[#ffe58f] bg-[#fffbe6] rounded-xl p-3'>
                          <div className='font-medium'>Reason</div>
                          <div>{documentDetail.TravelData.Reason}</div>
                        </div>
                        : null
                      }
                      <div className='mt-2'>
                        Comments:
                        <Form.Item className='mb-0' name='comment'>
                          <Input.TextArea autoSize={{minRows: 3, maxRows: 5}}/>
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
                            {
                              nextGroup?.GroupTag === 'linemanager' &&
                              <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.assignEmployeeOptions !== curValues.assignEmployeeOptions}>
                                {({getFieldValue}) => {
                                  return(
                                    <Form.Item
                                      name='assignedEmployeeId' 
                                      label='Send to:' 
                                      rules={[
                                        {
                                          required: true,
                                          message: 'Assign Employee is required'
                                        }
                                      ]}
                                      className='w-[300px] mb-0'
                                    >
                                      <Select 
                                        options={getFieldValue('assignEmployeeOptions')}
                                        className='w-[200px]'
                                      />
                                    </Form.Item>
                                  )
                                }}
                              </Form.Item>
                            }
                          </>
                        }
                      </div>
                      <div className='flex justify-end gap-4'>
                        <div className='flex items-center gap-3 justify-end'>
                          <Form.Item noStyle shouldUpdate={(pre, cur) => pre.comment !== cur.comment}>
                            {({getFieldValue}) => {
                              const declineDisabled = !Boolean(getFieldValue('comment'))
                              return <Button
                                htmlType='button'
                                type='danger'
                                onClick={() => clickActionButtons('Cancel')}
                                disabled={actionLoading}
                              >
                                Cancel Request
                              </Button>
                            }}
                          </Form.Item>
                        </div>
                      </div>
                    </div>
                  </Form>
                </Accordion>
                : 
                documentDetail?.TravelData?.Reason ?
                <div className='col-span-12 xl:col-span-5 2xl:col-span-6 self-start'>
                  <div className='border border-[#ffe58f] bg-[#fffbe6] rounded-xl p-3'>
                    <div>Reason</div>
                    <div>{documentDetail.TravelData.Reason}</div>
                  </div>
                </div>
                : null
              }
              <Accordion defaultOpen={false} title='Attachments' className='border col-span-12'>
                <div className='p-4'>
                  <Attachments 
                    showTitle={false}
                    disabled={checkPermission.getDisabledStatus({
                      documentData: documentDetail, 
                      userData: userInfo, 
                      groupTag: currentGroup?.GroupTag,
                      userGroupIds: userInfo?.ApprovalGroupIds,
                    })}
                  />
                </div>
              </Accordion>
              <Accordion defaultOpen={false} title='Document History & Comments, Document Route' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
                <div className='grid grid-cols-12 items-start gap-4 p-4'>
                  <DocumentTimeline
                    data={routeTimeline}
                  />
                </div>
              </Accordion>
            </div>
          </AntForm.Provider>
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
      {contextHolder}
    </div>
  )
}

export default ExternalTravel