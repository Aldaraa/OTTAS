import React, { useCallback, useContext, useEffect, useMemo, useState } from 'react'
import axios from 'axios'
import { Input, Select, Form as AntForm } from 'antd'
import { Accordion, Button, DocumentTimeline, Form, ProfileCalendar, RequestDocumentHeader, Tooltip } from 'components'
import RescheduleExistingTravel from './RescheduleExistingTravel'
import RemoveExistingTravel from './RemoveExistingTravel'
import { useLoaderData, useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import Attachments from './Attachments'
import NewTravel from './NewTravel'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'
import checkPermission from 'utils/checkPermission'
import generateColor from 'utils/generateColor'
import useQuery from 'utils/useQuery'
import ProfileForm from './ProfileForm'

const popupText = {
  'Approve': 'Are you sure you want to approve this request?',
  'Cancel': 'Are you sure you want to cancel this request?',
  'Save': 'Are you sure you want to save this request?',
  'Decline': 'Are you sure you want to decline this request?',
  'Complete': 'Are you sure you want to complete this request?'
}

const actionButtonColor = {
  'Approve': 'success',
  'Cancel': 'danger',
  'Save': 'primary',
  'Decline': '',
  'Complete': 'success'
}

function SiteTravel() {
  const [ documentDetail, setDocumentDetail ] = useState(null)
  const [ groupAndMembers, setGroupAndMembers ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ isNotFound, setIsNotFound ] = useState(false)
  const [ actionType, setActionType ] = useState(null)
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ currentGroup, setCurrentGroup ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ routeTimeline, setRouteTimeline ] = useState([])
  const [ myInfo, setMyInfo ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ profileData, setProfileData ] = useState(null)
  const [ documentRoutes, setDocumentRoutes ] = useState(null)
  const [ availableSeat, setAvailableSeat ] = useState(null)

  const { data } = useLoaderData()
  const { state, action } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const [ newTravelForm ] = AntForm.useForm()
  const [ removeTravelForm ] = AntForm.useForm()
  const [ profileForm ] = AntForm.useForm()
  const navigate = useNavigate()
  const { documentId, documentTag } = useParams()
  const query = useQuery()
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
      .then(axios.spread((employeeData, history, myinfo, groupAndMembers, routes) => {
        action.saveUserProfileData(employeeData.data)
        setProfileData(employeeData.data)
        setRouteTimeline(history.data)
        setMyInfo(myinfo.data)
        setGroupAndMembers(groupAndMembers.data)
        setDocumentRoutes(routes.data)
        const curGroup = routes.data?.find((group) => group.CurrentPosition === 1);
        setCurrentGroup(curGroup)
      })).catch(() => {

      }).then(() => setLoading(false))
    }
  },[documentId])

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
      state.connectionMultiViewer.on('ErrorMessage', (res, users) => {
        console.log('error message', users, res);
      });
    }

    window.addEventListener("beforeunload", () => {
      leaveRoom()
    })

    return () => {
      state.connectionMultiViewer && leaveRoom()
    }
  },[state.connectionMultiViewer, state.userInfo])

  useEffect(() => {
    if(!((documentDetail?.CurrentStatus === 'Completed') || (documentDetail?.CurrentStatus === 'Cancelled'))){
      if(documentTag === 'remove' && documentRoutes && documentDetail && groupAndMembers){
        calculateNextGroup()
      }
      else if(documentTag !== 'remove' && documentRoutes && documentDetail && groupAndMembers && availableSeat){
        calculateNextGroup()
      }
    }
  },[documentRoutes, documentDetail, groupAndMembers, availableSeat])

  useEffect(() => {
    if((documentTag === 'reschedule') && documentDetail){
      axios({
        method: 'get',
        url: `tas/transportschedule/seatinfo/${documentDetail?.TravelData?.ReScheduleId}`
      }).then((res) => {
        setAvailableSeat(res.data)
      })
    }else if((documentTag === 'addtravel') && documentDetail){
      axios.all([
        `tas/transportschedule/seatinfo/${documentDetail?.TravelData?.inScheduleId}`,
        `tas/transportschedule/seatinfo/${documentDetail?.TravelData?.outScheduleId}`,
        ].map((endpoint) => axios.get(endpoint)))
      .then(axios.spread((inSeat, outSeat) => {
        setAvailableSeat({inSeat: inSeat.data, outSeat: outSeat.data})
      })).catch(() => {

      }).then(() => setLoading(false))
    }
  },[documentTag, documentDetail])

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
    state.connectionMultiViewer?.invoke(
      'LeaveScreen',
      `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`,
      screenName
    )
  }

  const getProfile = useCallback((employeeId) => {
    axios({
      method: 'get',
      url: `tas/employee/profile/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
      setProfileData(res.data)
    }).catch((err) => {
    })
  },[action])

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
  
  const calculateNextGroup = async () => {
    const travelData = documentDetail?.TravelData
    const curGroup = documentRoutes?.find((group) => group.CurrentPosition === 1);
    // setCurrentGroup(curGroup)

    const getNextGroup = (groupTag) => {
      const nextGroup = groupAndMembers?.find((group) => group?.GroupTag === groupTag)
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
    const handleAccommodation = () => getNextGroup('accomodation');
    const handleTravelSite = () => getNextGroup('travelsite');
    const handleDataApproval = () => getNextGroup('dataapproval');
    const handleCompletion = () => {
      setNextGroup({ GroupTag: 'complete' });
      form.setFieldValue('nextGroupName', null);
      form.setFieldValue('nextGroupId', null);
      form.setFieldValue('assignEmployeeOptions', []);
    };
    if(documentTag === 'reschedule'){
      const availableSt = availableSeat?.AvailableSeatCount || 0
      const airplaneModeCondition = ((documentDetail?.DaysAway >= 7) && (availableSt >= 3)) && (documentDetail?.TravelData?.ReScheduleTransportMode === 'Airplane')
      const otherModeCondition = (documentDetail?.DaysAway >= 2) && (documentDetail?.TravelData?.ReScheduleTransportMode !== 'Airplane')
      
      ///////// if current position is LINE MANAGER ///////////
      if(curGroup?.GroupTag === 'linemanager'){
        if(documentDetail.EmployeeActive === 1){
          ////////////  When it has flight schedule   ///////////
          if(documentDetail?.TravelData){
            let dateBetween = dayjs(documentDetail?.TravelData?.ExistingScheduleDate)-dayjs(documentDetail?.TravelData?.ReScheduleDate)
            const isOUTpostpone = (documentDetail?.TravelData?.ReScheduleDirection === 'OUT') && (dateBetween < 0)
            const isINforward = (documentDetail?.TravelData?.ReScheduleDirection === 'IN') && (dateBetween > 0)
            ///////////    When OUT direction schedule is postpone  /////////////
            if(isOUTpostpone || isINforward){
              handleAccommodation()
            }else{ 
              if(airplaneModeCondition || otherModeCondition){
                setNextGroup({GroupTag: 'complete'})
              }else{
                handleTravelSite()
              }
            }
          }
        }else{
          handleDataApproval()
        }
      }
  
      ///////// if current position is DATA APPROVAL ///////////
  
      else if(curGroup?.GroupTag === 'dataapproval'){
        handleAccommodation()
      }
  
      ///////// if current position is ACCOMMODATION TEAM ///////////
  
      else if(curGroup?.GroupTag === 'accomodation'){
        handleTravelSite()
      }
      
      ///////// if current position is TRAVEL TEAM ///////////
      
      else if(curGroup?.GroupTag === 'travelsite'){
        setNextGroup({GroupTag: 'complete'})
      }
      else if(curGroup?.GroupTag === 'Requester'){
        getNextGroup('linemanager');
      }
    }
    else if(documentTag === 'addtravel'){
      let in_to_out = false;
      if(travelData?.inScheduleDate && travelData?.outScheduleDate){
        if(dayjs(travelData?.outScheduleDate).diff(travelData?.inScheduleDate) > 0){
          in_to_out = true
        }
      }

      const isLongAway = documentDetail?.DaysAway >= 7;
      const isInboundAirplaneWithSeats = (availableSeat?.inSeat?.AvailableSeatCount >= 3) && (travelData?.INScheduleTransportMode === 'Airplane');
      const isOutboundAirplaneWithSeats = (availableSeat?.outSeat?.AvailableSeatCount >= 3) && (travelData?.OUTScheduleTransportMode === 'Airplane');
      const airplaneModeCondition = isLongAway && isInboundAirplaneWithSeats && isOutboundAirplaneWithSeats;


      const hasAirplaneMode = (travelData?.INScheduleTransportMode === 'Airplane') ||  (travelData?.OUTScheduleTransportMode === 'Airplane')
      const otherModeCondition = in_to_out && (documentDetail?.DaysAway >= 2) && ((travelData?.OUTScheduleTransportMode !== 'Airplane') || (travelData?.INScheduleDescription !== 'Airplane'))
      ///////// if current position is LINE MANAGER ///////////
      if(curGroup?.GroupTag === 'linemanager'){
        if(documentDetail.EmployeeActive === 1){
          if(hasAirplaneMode){
            if(airplaneModeCondition){
              if(!profileData?.RoomId){
                handleAccommodation()
              }else{
                handleCompletion()
              }
            }else{
              handleAccommodation()
            }
          }else if(otherModeCondition){
            if(!profileData?.RoomId){
              handleAccommodation()
            }else{
              handleCompletion()
            }
          }
          else{
            handleAccommodation()
          }
        }else{
          handleDataApproval()
        }
      }
  
      ///////// if current position is DATA APPROVAL ///////////
  
      else if(curGroup?.GroupTag === 'dataapproval'){
        if(hasAirplaneMode){
          if(airplaneModeCondition){
            if(!profileData?.RoomId){
              handleAccommodation()
            }else{
              handleCompletion()
            }
          }else{
            handleAccommodation()
          }
        }else if(otherModeCondition){
          if(!profileData?.RoomId){
            handleAccommodation()
          }else{
            handleCompletion()
          }
        }
        else{
          handleAccommodation()
        }
    
      }
  
      ///////// if current position is ACCOMMODATION TEAM ///////////
  
      else if(curGroup?.GroupTag === 'accomodation'){
        handleTravelSite()
      }
      
      ///////// if current position is TRAVEL TEAM ///////////
      
      else if(curGroup?.GroupTag === 'travelsite'){
        handleCompletion()
      }
      else if(curGroup?.GroupTag === 'Requester'){
        getNextGroup('linemanager')
      }
    }
    else {
      const otherModeCondition = (documentDetail?.DaysAway >= 2) && (documentDetail?.TravelData?.FirstScheduleDirection === 'IN') && (documentDetail?.TravelData?.LastScheduleDirection === 'OUT')
      ////////////////        REMOVE TRAVEL        /////////////////
      if(curGroup?.GroupTag === 'linemanager'){
        if(otherModeCondition){
          handleCompletion()
        }
        else if(documentDetail?.TravelData?.FirstScheduleDirection === 'OUT'){
          handleAccommodation()
        }else{
          handleTravelSite()
        }
      }else if(curGroup?.GroupTag === 'Requester'){
        getNextGroup('linemanager')
      }else if(curGroup?.GroupTag === 'accomodation'){
        handleTravelSite()
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
        url: `tas/requestsitetravel/${documentTag}/${actionType}/${documentId}`,
        data: {
          comment: values.comment,
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
      if(documentTag === 'remove'){
        const removeValues = removeTravelForm.getFieldsValue()
        axios({
          method: 'put',
          url: 'tas/requestsitetravel/removetravel',
          data: {
            "id": documentDetail?.TravelData.Id,
            "firstScheduleId": removeValues.firstTransport.flightId,
            "lastScheduleId": removeValues.lastTransport.flightId,
            "FirstScheduleNoShow": removeValues.firstTransport.NoShow,
            "LastScheduleNoShow": removeValues.lastTransport.NoShow,
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
        }).catch(() => {
    
        })
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

  const disableStatus = useMemo(() => {
    return checkPermission.getDisabledStatus({
      documentData: documentDetail,
      userData: myInfo,
      groupTag: currentGroup?.GroupTag,
    })
  },[documentDetail, myInfo, currentGroup])

  const hasActionPermission = useMemo(() => {
    return checkPermission.getActionPermission({
      documentData: documentDetail,
      userData: myInfo,
      groupTag: currentGroup?.GroupTag,
    })
  },[documentDetail, myInfo])

  const isRequester = useMemo(() => {
    if(myInfo && documentDetail){
      return (myInfo?.EmployeeId === documentDetail?.RequestUserId) && !(documentDetail?.CurrentStatus === 'Completed' || documentDetail?.CurrentStatus === 'Cancelled')
    }else{
      return false
    }
  },[myInfo, documentDetail])

  const renderTravelForm = () => {
    switch (documentTag) {
      case 'addtravel': return (
        <Accordion title='New Travel Request' className='border col-span-12 xl:col-span-7 2xl:col-span-6'>
          <div className='p-4'>
            <NewTravel
              form={newTravelForm} 
              documentDetail={documentDetail}
              refreshData={getDocumentDetail}
              disabled={disableStatus}
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
              disabled={disableStatus}
              currentGroup={currentGroup}
              travelData={documentDetail?.TravelData}
              hasActionPermission={hasActionPermission}
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
              disabled={disableStatus}
            />
          </div>
        </Accordion>
      ) 
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
      }
    }else{
      if((myInfo?.EmployeeId === documentDetail?.RequestUserId) && documentDetail?.CurrentStatus !== 'Completed'){
        return(
          <Button htmlType='button' type='danger' onClick={() => clickActionButtons('Cancel')} disabled={actionLoading}>
            Cancel Request
          </Button>
        ) 
      }
      return null
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

  const handleClickYesPopup = useCallback(() => {
    if(actionType === 'Approve'){
      form.submit()
    }else{
      handleSubmit()
    }
  },[actionType, form])

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
          {
            currentGroup?.GroupTag === 'dataapproval' && !disableStatus ?
            <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden mt-3' title='Profile'>
              <ProfileForm
                data={profileData}
                form={profileForm}
                currentGroup={currentGroup}
                className='shadow-none'
                refreshData={getProfile}
                disabled={disableStatus}
              />
            </Accordion>
            : null
          }
          <div className='grid grid-cols-12 gap-3 mt-3'>
            {renderTravelForm()}
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
                    (hasActionPermission || isRequester) ?
                    <div className='mt-2'>
                      Comments:
                      <Form.Item className='mb-0' name='comment'>
                        <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                      </Form.Item>
                    </div>
                    : null
                  }
                  <div className='flex mt-5 mb-2 gap-5'>
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
                                  rules={[{ required: true, message: 'Assign Employee is required' }]}
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
                  {
                    ((hasActionPermission && isRequester) || hasActionPermission) ?
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
                    : isRequester ?
                    <div className='flex justify-end gap-4'>
                      <div className='flex items-center gap-3 justify-end'>
                      <Button htmlType='button' type='danger' onClick={() => clickActionButtons('Cancel')} disabled={actionLoading}>
                        Cancel Request
                      </Button>
                      </div>
                    </div>
                    :
                    null
                  }
                </div>
              </Form>
            </Accordion>
            <Accordion defaultOpen={false} title='Attachments' className='border col-span-12'>
              <div className='p-4'>
                <Attachments 
                  showTitle={false}
                  disabled={disableStatus}
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
        </div>
      }
      <Popup visible={showPopup} showTitle={false} height={'auto'} width={350}>
          <div>{popupText[actionType]}</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type={actionButtonColor[actionType]} onClick={handleClickYesPopup} loading={actionLoading}>Yes</Button>
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
      </Popup>
    </div>
  )
}

export default SiteTravel