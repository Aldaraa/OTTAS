import axios from 'axios'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Input, Form as AntForm, } from 'antd';
import { Accordion, Button, DocumentTimeline, Form, ProfileCalendar, RequestDocumentHeader, Table, Tooltip } from 'components'
import { Popup } from 'devextreme-react'
import ProfileForm from './ProfileForm'
import { AuthContext } from 'contexts';
import checkPermission from 'utils/checkPermission';
import generateColor from 'utils/generateColor';
import useQuery from 'utils/useQuery';
import ChangesLog from './ChangesLog';
import Temporary from './Temporary';
import dayjs from 'dayjs';

function SAMProfileChanges() {
  const [ documentDetail, setDocumentDetail ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  const [ actionType, setActionType ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ groupAndMembers, setGroupAndMembers ] = useState([])
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ docRoute, setDocRoute ] = useState([])
  const [ currentGroup, setCurrentGroup ] = useState(null)
  const [ isNotFound, setIsNotFound ] = useState(false)
  const [ routeTimeline, setRouteTimeline ] = useState([])
  const [ userInfo, setUserInfo ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ profileData, setProfileData ] = useState(null)
  const [ temporaryData, setTemporaryData ] = useState(null)
  const [ declineDisabled, setDeclineDisabled ] = useState(true)
  const [ profileFields, setProfileFields ] = useState([])

  const [ form ] = AntForm.useForm()
  const [ profileForm ] = AntForm.useForm()
  const [ temporaryForm ] = AntForm.useForm()
  const { documentId } =  useParams()
  const query =  useQuery()
  const { state, action } = useContext(AuthContext)
  const screenName = `requestDocumentDetail_${documentId}`

  const navigate = useNavigate()

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
    if(documentId){
      action.changeMenuKey('/request/task')
      axios.all([
        `tas/requestdocumentprofilechange/${documentId}`,
        `tas/requestdocumenthistory/${documentId}`,
        `tas/requestdocument/myinfo`,
        `tas/requestdocumentprofilechange/temp/${documentId}`,
        `tas/profilefield`,
      ].map((endpoint) => axios.get(endpoint))).then(axios.spread((document, history, myinfo, temporaryData, proFields) => {
        if(document.data.Id){
          setDocumentDetail(document.data)
          getProfile(document.data.EmployeeId)
        }
        else{
          setIsNotFound(true)
        }
        let tmp = {}
        proFields.data.map((item) => {
          tmp[item.ColumnName] = item
        })
        setProfileFields(tmp)
        setRouteTimeline(history.data)
        setUserInfo(myinfo.data)
        setTemporaryData(temporaryData.data)
        temporaryForm.setFieldsValue({
          ...temporaryData.data,
          StartDate: temporaryData.data.StartDate ? dayjs(temporaryData.data.StartDate) : null,
          EndDate: temporaryData.data.EndDate ? dayjs(temporaryData.data.EndDate) : null,
        })
      })).catch(() => {

      }).then(() => setLoading(false))
    }
  },[documentId])

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
        `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}-${state.userInfo?.Role}`,
        // `${state.userInfo?.Role}`,
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
      `${state.userInfo?.Firstname} ${state.userInfo?.Role}`,
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

  const getProfileChangesData = () => {
    axios({
      method: 'get',
      url: `tas/requestdocumentprofilechange/${documentId}`
    }).then((res) => {
      setDocumentDetail(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
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
      setDocRoute(res.data)
      calculateNextGroup(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const calculateNextGroup = (documentRoutes) => {
    let currentGroup = documentRoutes.find((group) => group.CurrentPosition === 1);
    setCurrentGroup(currentGroup)
    ///////// if current position is DATA APPROVAL ///////////
    if(currentGroup?.GroupTag === 'dataapproval'){
      setNextGroup({GroupTag: 'complete'})
    }
    
    ///////// if current position is REQUESTER ///////////
    else if(currentGroup?.GroupTag === 'Requester'){
      let currentGroupIndex = documentRoutes.findIndex((group) => group?.CurrentPosition === 1)
      let nextGroupData = documentRoutes[currentGroupIndex+1]
      let nextGroup = groupAndMembers.find((item) => item.id === nextGroupData?.GroupId)
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

  const handleSubmit = (values) => {
    setActionLoading(true)
    let tempData = null
    if(documentDetail?.DocumentTag === 'temp'){
      tempData = {
        ...temporaryForm.getFieldsValue()
      }
    }

    if(actionType === 'Approve'){
      axios({
        method: 'put',
        url: 'tas/requestdocument/approve',
        data: {
          id: documentId,
          nextGroupId: values.nextGroupId,
          assignEmployeeId: values.assignedEmployeeId,
          comment: values.comment,
          impersonateUserId: query.get('impersonateuser'),
        }
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
    else if(actionType === 'Cancel'){
      axios({
        method: 'put',
        url: 'tas/requestdocument/cancel',
        data: {
          id: documentId,
          comment: values.comment,
          impersonateUserId: query.get('impersonateuser'),
        }
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
    else if(actionType === 'Decline'){
      axios({
        method: 'put',
        url: 'tas/requestdocument/decline',
        data: {
          id: documentId,
          comment: values.comment,
          impersonateUserId: query.get('impersonateuser'),
        }
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
    else if(actionType === 'Complete'){
      if(documentDetail?.DocumentTag === 'temp'){
        axios({
          method: 'put',
          url: `tas/requestdocumentprofilechange/complete/temp/${documentId}`,
          data: {
            id: documentId,
            comment: values.comment,
            impersonateUserId: query.get('impersonateuser'),
          }
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
          url: `tas/requestdocumentprofilechange/complete/${documentId}`,
          data: {
            id: documentId,
            comment: values.comment,
            impersonateUserId: query.get('impersonateuser'),
          }
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
            <Button type='danger'onClick={() => form.submit()} loading={actionLoading}>Yes</Button>
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Decline':
        return (<>
          <div>Are you sure you want to decline this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button onClick={() => form.submit()} loading={actionLoading}>Yes</Button>
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

  const clickActionButtons = (type) => {
    setActionType(type);
    if(type === 'Approve' || type === 'Complete' || type === 'Decline'){
      form.submit()
    }else{
      setShowPopup(true)
    }
  }

  const renderButtons = (groupTag, declineDisabled) => {
    if(groupTag){
      if(groupTag === 'complete'){
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
      }else{
        return( 
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
        )
      }
    }else{
      return null
    }
  }

  const hasActionPermission = useMemo(() => {
    return checkPermission.getActionPermission({
      documentData: documentDetail, 
      userData: userInfo, 
      groupTag:currentGroup?.GroupTag,
      userGroupIds: userInfo?.ApprovalGroupIds,
    })
  },[documentDetail, userInfo, currentGroup])

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
        (
          isNotFound ? 
          <div className='bg-white rounded-ot px-4 py-3 min-h-[300px] flex flex-col justify-center items-center'>
            <div className='text-xl'>Not Found Document</div>
            <Button htmlType='button' type='primary' className='mt-5' onClick={() => navigate('/request/task')}>Back to List</Button>
          </div>
          :
          <div className='grid grid-cols-12 gap-y-5 relative gap-x-3'>
            <RequestDocumentHeader documentDetail={documentDetail} profileData={profileData}/>
            <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden' title='Travel Calendar'>
              <div className='p-4'>
                <ProfileCalendar profile={profileData}/>
              </div>
            </Accordion>
            {
              documentDetail?.DocumentTag === 'temp' ?
              <>
                <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden' title='Temporary'>
                  <div className='p-4'>
                    <Temporary
                      form={temporaryForm}
                      temporaryData={temporaryData}
                      // refreshData={}
                      disabled={checkPermission.getDisabledStatus({
                        documentData: documentDetail, 
                        userData: userInfo, 
                        groupTag: currentGroup?.GroupTag
                      })}
                    />
                  </div>
                </Accordion>
                <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden' title='Detail'>
                  <div className='p-4 flex flex-col gap-5'>
                    <ProfileForm
                      data={documentDetail?.employee}
                      documentData={documentDetail}
                      form={profileForm}
                      currentGroup={currentGroup}
                      className='shadow-none border'
                      refreshData={getProfileChangesData}
                      getProfile={getProfile}
                      profileFields={profileFields}
                      disabled={checkPermission.getDisabledStatus({
                        documentData: documentDetail, 
                        userData: userInfo, 
                        groupTag: currentGroup?.GroupTag
                      })}
                    />
                  </div>
                </Accordion>
              </>
              :
              <>
                <>
                  <Accordion className='col-span-12 flex-1 shadow-md rounded-ot overflow-hidden' title='Detail'>
                    <div className='pt-4 flex flex-col gap-5'>
                      <ProfileForm 
                        data={documentDetail?.employee} 
                        documentData={documentDetail}
                        form={profileForm}
                        currentGroup={currentGroup}
                        className='shadow-none pt-0'
                        refreshData={getProfileChangesData}
                        profileFields={profileFields}
                        disabled={checkPermission.getDisabledStatus({
                          documentData: documentDetail, 
                          userData: userInfo, 
                          groupTag: currentGroup?.GroupTag
                        })}
                      />
                    </div>
                  </Accordion>
                  <Accordion className='col-span-12 w-full shadow-md rounded-ot overflow-hidden' title='Changes'>
                    <ChangesLog data={documentDetail?.ChangeInfo}/>
                  </Accordion>
                </>
              </>
            }
            <Form  
              form={form} 
              labelWrap={true}
              className='grid col-span-12'
              onFinish={handleSubmit}
            >
              {
                hasActionPermission ?
                <Accordion title='Request Document' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
                  <div className='flex flex-col gap-4 p-4'>
                    <div>
                      Comments:
                      <Form.Item className='m-0' name='comment'>
                        <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                      </Form.Item>
                    </div>
                    <div className='flex gap-10'>
                      {
                        nextGroup?.GroupTag !== 'complete' &&
                        <>
                          <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.nextGroupName !== curValues.nextGroupName}>
                            {({getFieldValue}) => {
                              return(
                                <Form.Item name='nextGroupId' className='mb-0'>
                                  Next Approval: <span className='font-bold'>{getFieldValue('nextGroupName')}</span>
                                </Form.Item>
                              )
                            }}
                          </Form.Item>
                        </>
                      }
                    </div>
                    <div className='flex items-center gap-3 justify-end'>
                      <Form.Item noStyle shouldUpdate={(pre, cur) => pre.comment !== cur.comment}>
                        {({getFieldValue}) => {
                          const declineDisabled = !Boolean(getFieldValue('comment'))
                          return renderButtons(nextGroup?.GroupTag, declineDisabled)
                        }}
                      </Form.Item>
                    </div>
                  </div>
                </Accordion>
                : 
                // userInfo?.EmployeeId === documentDetail?.RequestUserId && documentDetail?.CurrentStatus === 'Submitted' ?
                userInfo?.EmployeeId === documentDetail?.RequestUserId && documentDetail?.CurrentStatus !== 'Completed' ?
                <Accordion title='Request Document' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
                  <div className='flex flex-col gap-4 p-4'>
                    <div>
                      Comments:
                      <Form.Item className='m-0' name='comment'>
                        <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                      </Form.Item>
                    </div>
                    <div className='flex gap-10'>
                      {
                        nextGroup?.GroupTag !== 'complete' && 
                        <>
                          <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.nextGroupName !== curValues.nextGroupName}>
                            {({getFieldValue}) => {
                              return(
                                <Form.Item name='nextGroupId' className='mb-0'>
                                  Next Approval: <span className='font-bold'>{getFieldValue('nextGroupName')}</span>
                                </Form.Item>
                              )
                            }}
                          </Form.Item>
                        </>
                      }
                    </div>
                    <div className='flex items-center gap-3 justify-end'>
                      <Button
                        htmlType='button'
                        type='danger'
                        onClick={() => clickActionButtons('Cancel')}
                        disabled={actionLoading}
                      >
                        Cancel Request
                      </Button>
                    </div>
                  </div>
                </Accordion>
                : null
              }
            </Form>
            <Accordion title='Document History & Comments, Document Route' defaultOpen={false} className='col-span-12 shadow-md rounded-ot overflow-hidden'>
              <div className='grid grid-cols-12 items-start gap-4 p-4'>
                <DocumentTimeline
                  data={routeTimeline}
                />
              </div>
            </Accordion>
          </div>
        )
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

export default SAMProfileChanges