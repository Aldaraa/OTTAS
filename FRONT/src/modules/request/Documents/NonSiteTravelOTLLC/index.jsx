import axios from 'axios'
import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { Input, Select } from 'antd';
import Flight from './Flight'
import Attachments from './Attachments'
import Accommodation from './Accommodation'
import { Accordion, Button, DocumentTimeline, Form, ProfileCalendar, RequestDocumentHeader, Tooltip } from 'components'
import { InfoCircleOutlined, StarFilled } from '@ant-design/icons'
import { Popup } from 'devextreme-react'
import { AuthContext } from 'contexts';
import checkPermission from 'utils/checkPermission';
import generateColor from 'utils/generateColor';
import useQuery from 'utils/useQuery';
import TravelInfo from './TravelInfo';

function NonSiteTravel() {
  const [ data, setData ] = useState(null)
  const [ documentDetail, setDocumentDetail ] = useState(null)
  const [ flightData, setFlightData ] = useState([])
  const [ accommodationData, setAccommodationData ] = useState([])
  const [ isInited, setIsInited ] = useState(false)
  const [ actionType, setActionType ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ isNotFound, setIsNotFound ] = useState(false)
  const [ groupAndMembers, setGroupAndMembers ] = useState([])
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ currentGroup, setCurrentGroup ] = useState(null)
  const [ routeTimeline, setRouteTimeline ] = useState([])
  const [ userInfo, setUserInfo ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ profileData, setProfileData ] = useState(null)
  const [ isForeign, setIsForeign ] = useState(false)
  
  const navigate = useNavigate()
  const { state, action } = useContext(AuthContext)
  const { documentId } =  useParams()
  const query = useQuery()
  const [ form ] = Form.useForm()
  const [ flightInfoForm ] = Form.useForm()
  const flightRef = useRef(null)
  const screenName = `requestDocumentDetail_${documentId}`

  const isDisabled = useMemo(() => {
    return checkPermission.getDisabledStatus({
      documentData: documentDetail, 
      userData: userInfo, 
      groupTag: currentGroup?.GroupTag,
      userGroupIds: userInfo?.ApprovalGroupIds,
    })
  },[documentDetail, userInfo, currentGroup]) 

  const hasUpdatePermission = useMemo(() => {
    return checkPermission.checkUpdateItinerary({
      groupAndMembers: groupAndMembers,
      userData: userInfo
    })
  },[groupAndMembers, userInfo]) 

  

  // window.onbeforeunload = function () {
  //   return "Do you really want to close?";
  // };

  // window.onbeforeunload = function () {
  //   return leaveRoom();
  // };

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
        `tas/requestnonsitetravel/${documentId}`,
        `tas/requestdocumenthistory/${documentId}`,
        `tas/requestdocument/myinfo`,
      ].map((endpoint) => axios.get(endpoint))).then(axios.spread((document, history, myinfo, traveloptions) => {
        if(document.data){
          getProfile(document.data.EmployeeId)
          setDocumentDetail(document.data)
        }
        setAccommodationData(document.data.Accommodations)
        setFlightData(document.data.FlightInfo)
        setRouteTimeline(history.data)
        setUserInfo(myinfo.data)

      })).catch(() => {

      }).then(() => setIsInited(true))
    }
  },[documentId])
  
  useEffect(() => {
    if(groupAndMembers){
      getDocRoutes()
    }
  },[groupAndMembers])
  
  useEffect(() => {
    if(documentDetail){
      getData(documentDetail.EmployeeId)
      getDocumentMembers()
    }
  },[documentDetail])

  useEffect(() => {
    if(actionType){
      if(actionType === 'Approve' || actionType === 'Complete' || actionType === 'Decline'){
        handleSubmit()
      }else{
        setShowPopup(true)
      }
    }
  },[actionType])

  const joinRoom = () => {
    try {
      state.connectionMultiViewer?.invoke(
        'JoinScreen', 
        `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`,
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
      form.setFieldsValue({
        ...res.data,
        PassportExpiry: res.data.PassportExpiry ? dayjs(res.data.PassportExpiry) : null,
      })
      setProfileData(res.data)
      const userNationality = state.referData.nationalities.find((item) => item.Id === res.data.NationalityId)
      if(userNationality && userNationality.Code !== 'MN'){
        setIsForeign(true)
      }
    }).catch((err) => {
    })
  }

  const getDocumentDetail = () => {
    axios({
      method: 'get',
      url: `tas/requestnonsitetravel/${documentId}`
    }).then((res) => {
      if(res.data.Id){
        setDocumentDetail(res.data)
        setAccommodationData(res.data.Accommodations)
        setFlightData(res.data.FlightInfo)
      }
      else{
        setIsNotFound(true)
      }
    }).catch((err) => {

    })
  }

  const getRouteHistory = () => {
    axios({
      method: 'get',
      url: `tas/requestdocumenthistory/${documentId}`,
    }).then((res) => {
      setRouteTimeline(res.data)
    }).catch((err) => {

    })
  }

  const getData = (employeeId) => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    })
  }
  
  const getDocRoutes = () => {
    axios({
      method: 'get',
      url: `tas/requestgroupconfig/documentroute/${documentId}`
    }).then((res) => {
      calculateNextGroup(res.data)
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

  const calculateNextGroup = (documentRoutes) => {
    let currentGroup = documentRoutes.find((group) => group.CurrentPosition === 1);
    setCurrentGroup(currentGroup)
    ///////// if current position is TRAVEL TEAM FLIGHT ///////////

    if(currentGroup?.GroupTag === 'travelflight' && currentGroup.OrderIndex === 1){
      let nextGroup = documentRoutes.find((group) => group.GroupTag === 'requester')
      if(nextGroup){
        form.setFieldValue('nextGroupName', `${nextGroup?.GroupName} - ${documentDetail?.RequesterFullName}`)
        form.setFieldValue('nextGroupId', nextGroup?.Id)
        form.setFieldValue('assignedEmployeeId', documentDetail.RequestUserId)
        setNextGroup(nextGroup)
      }
    }

    ///////// if current position is REQUESTER ///////////
    else if(currentGroup?.GroupTag === 'requester' && currentGroup.OrderIndex === 2){
      // if(documentDetail.DaysAway >= 7){
        // if()
        let nextGroup = documentRoutes.find((group) => group.GroupTag === 'linemanager')
        let nextGroupMembers = groupAndMembers.find((group) => group.GroupTag === 'linemanager')
        if(nextGroup){
          let tmp = []
          nextGroupMembers.groupMembers.map((item) => {
            tmp.push({value: item.employeeId, label: item.fullName})
          })
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.Id)
          form.setFieldValue('assignEmployeeOptions', tmp)
          setNextGroup(nextGroup)
        }
      // }
    }

    ///////// if current position is LINE MANAGER ///////////

    else if(currentGroup?.GroupTag === 'linemanager'){
      ////////////  When it has flight schedule   ///////////
      if(documentDetail?.FlightInfo?.FlightData?.length > 0){
        let nextGroup = documentRoutes.find((group) => group.GroupTag === 'travelflight' && group.OrderIndex === 4)
        if(nextGroup){
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.Id)
          setNextGroup(nextGroup)
        }
      }
      //////////////   When it has accommodation    //////////////
      else if(documentDetail?.Accommodations.length > 0){
        let nextGroup = documentRoutes.find((group) => group.GroupTag === 'travelhotel')
        if(nextGroup){
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.Id)
          setNextGroup(nextGroup)
        }
      }
    }
    
    ///////// if current position is TRAVEL TEAM FLIGHT ///////////
    
    else if(currentGroup?.GroupTag === 'travelflight' && currentGroup.OrderIndex === 4){
      if(documentDetail?.Accommodations.length > 0){
        let nextGroup = documentRoutes.find((group) => group.GroupTag === 'travelhotel')
        if(nextGroup){
          form.setFieldValue('nextGroupName', nextGroup?.GroupName)
          form.setFieldValue('nextGroupId', nextGroup?.Id)
          setNextGroup(nextGroup)
        }
      }else{
        setNextGroup({GroupTag: 'complete'})
      }
    }

    ///////// if current position is TRAVEL HOTEL ///////////

    else if(currentGroup?.GroupTag === 'travelhotel'){
      setNextGroup({GroupTag: 'complete'})
    }
    else if(currentGroup?.GroupTag === 'Requester' && currentGroup.OrderIndex === 0){
      let nextGroup = documentRoutes.find((group) => group.GroupTag === 'travelflight')
      let nextGroupMembers = groupAndMembers.find((group) => group.GroupTag === 'travelflight')
      if(nextGroup){
        let tmp = []
        nextGroupMembers.groupMembers.map((item) => {
          tmp.push({value: item.employeeId, label: item.fullName})
        })
        form.setFieldValue('nextGroupName', nextGroup?.GroupName)
        form.setFieldValue('nextGroupId', nextGroup?.Id)
        form.setFieldValue('assignEmployeeOptions', tmp)
        setNextGroup(nextGroup)
      }
    }
  }

  const renderCurrentPositionStar = (row) => {
    if(row.value === 1 && row.data.Id === 'travelflight'){
      if(routeTimeline.length === 1 && row.rowIndex === 1){
        return <StarFilled style={{color: '#e57200'}}/>
      }
      else if(routeTimeline.length > 1 && row.rowIndex !== 1){
        return <StarFilled style={{color: '#e57200'}}/>
      }
    }else if(row.value === 1){
      return <StarFilled style={{color: '#e57200'}}/>
    }
  }

  const handleSubmit = () => {
    const values = form.getFieldsValue()
    setActionLoading(true)
    if(actionType === 'Approve'){
      axios({
        method: 'put',
        url: 'tas/requestdocument/approve',
        data: {
          id: documentId,
          assignEmployeeId: form.getFieldValue('assignedEmployeeId'),
          comment: values.comment,
          nextGroupId: values.nextGroupId,
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
          nextGroupId: documentDetail.NextGroupId,
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
    else if(actionType === 'Save'){
      axios({
        method: 'put',
        url: 'tas/requestdocument/save',
        data: {
          id: documentId,
          assignEmployeeId: form.getFieldValue('assignedEmployeeId'),
          nextGroupId: documentDetail.NextGroupId,
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
      axios({
        method: 'put',
        url: `tas/requestnonsitetravel/complete/${documentId}`,
        data: {
          documentId: documentId,
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

  const onClosePopup = useCallback(() => {
    setActionType(null)
    setShowPopup(false)
  },[])

  const renderDialogText = (type) => {
    switch (type) {
      case 'Approve':
        return (<>
          <div>Are you sure you want to approve this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='success'onClick={handleSubmit} loading={actionLoading}>Yes</Button>
            <Button onClick={onClosePopup} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Cancel':
        return (<>
          <div>Are you sure you want to cancel this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='danger'onClick={handleSubmit} loading={actionLoading}>Yes</Button>
            <Button onClick={onClosePopup} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Save':
        return (<>
          <div>Are you sure you want to save this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='primary'onClick={handleSubmit} loading={actionLoading}>Yes</Button>
            <Button onClick={onClosePopup} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Decline':
        return (<>
          <div>Are you sure you want to decline this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button onClick={handleSubmit} loading={actionLoading}>Yes</Button>
            <Button onClick={onClosePopup} disabled={actionLoading}>No</Button>
          </div>
        </>)
      case 'Complete': return (<div>
          <div>Are you sure you want to complete this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='success' onClick={handleSubmit} loading={actionLoading}>Yes</Button> 
            <Button onClick={onClosePopup} disabled={actionLoading}>No</Button>
          </div>
        </div>)
    }
  }

  const clickActionButtons = (type) => {
    if(type === 'Cancel'){
      setActionType(type)
    }else{
      form.validateFields().then((res) => {
        if(type === 'Complete' && documentDetail?.FlightInfo?.FlightData?.length > 0){
          flightInfoForm.validateFields().then(() => {
            setActionType(type);
          }).catch((err) => {
            flightInfoForm.scrollToField('RequestTravelPurposeId')
          })
        }else{
          setActionType(type);
        }
      }).catch((err) => {
        // console.log('>>> err', err);
      })
    }
  }

  const handleWaitingAgent = () => {
    const values = form.getFieldsValue()
    setActionLoading(true)
    axios({
      method: 'put',
      url: `tas/requestnonsitetravel/waitingagent/${documentId}?comment=${values.comment}`
    }).then((res) => {
      navigate('/request/task')
    }).finally(() => setActionLoading(false))
  }

  const isDisabledBTR = useMemo(() => {
    let status = false
    if(documentDetail?.FlightInfo?.FlightData.length > 0){
      status = state.travelOptions.length === 0
    }
    return status
  },[documentDetail, state.travelOptions])

  const isDisabledSubmit = useMemo(() => {
    let status = false
    if(documentDetail?.FlightInfo?.FlightData.length > 0){
      status = documentDetail?.IssuedoptionId ? false : true
    }
    return status
  },[documentDetail, state.travelOptions])

  const renderButtons = (groupTag, declineDisabled) => {
    if(groupTag){
      if(groupTag !== 'complete'){
        return(
          <>
            {
              currentGroup?.GroupTag === 'Requester' && currentGroup?.OrderIndex === 0 ? 
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
                {
                  (currentGroup?.GroupTag === 'travelflight' && currentGroup?.OrderIndex === 1) ?
                  <>
                    <Button
                      htmlType='button'
                      disabled={documentDetail?.CurrentStatus === 'Waiting Agent' || actionLoading}
                      type='primary'
                      onClick={() => handleWaitingAgent()}
                      loading={actionLoading && !actionType}
                    >
                      Wait for Agent
                    </Button>
                    <Button
                      htmlType='button'
                      type='success'
                      onClick={() => clickActionButtons('Approve')}
                      disabled={actionLoading || isDisabledBTR}
                      loading={actionLoading && actionType === 'Approve'}
                    >
                      Back to Requester
                    </Button>
                  </>
                  :
                  currentGroup?.GroupTag === 'requester' && currentGroup?.OrderIndex === 2 ? 
                  <Button
                    htmlType='button'
                    type='primary'
                    onClick={() => clickActionButtons('Approve')}
                    disabled={isDisabledSubmit}
                    loading={actionLoading && actionType === 'Approve'}
                  >
                    Submit
                  </Button>
                  :
                  <Button
                    htmlType='button'
                    type='success'
                    onClick={() => clickActionButtons('Approve')}
                    disabled={actionLoading}
                    loading={actionLoading && actionType === 'Approve'}
                  >
                    Approve
                  </Button>
                }
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
  }

  const refreshFlightData = () => {
    getDocumentDetail()
    getRouteHistory()
  }

  const hasActionPermission = useMemo(() => {
    return checkPermission.getActionPermission({
      documentData: documentDetail, 
      userData: userInfo, 
      groupTag:currentGroup?.GroupTag,
      userGroupIds: userInfo?.ApprovalGroupIds,
    })
  },[ documentDetail, userInfo, currentGroup ])

  return (
    <div>
      {
        !isInited ? 
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
          <Form 
            className='grid grid-cols-12 gap-4 relative' 
            form={form} 
            editData={{...data, PassportExpiry: data?.PassportExpiry ? dayjs(data?.PassportExpiry) : null}}
            labelWrap={true}
            disabled={isDisabled}
            onFinish={() => setShowPopup(true)}
          >
            <RequestDocumentHeader profileData={profileData} documentDetail={documentDetail}/>
            <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden' title='Travel Calendar'>
              <div className='p-4'>
                <ProfileCalendar profile={profileData}/>
              </div>
            </Accordion>
            <Accordion title='Traveller Information' className='shadow-md border col-span-6'>
              <div className='flex gap-x-8 gap-y-8 p-4 text-xs flex-wrap'>
                <div className='flex flex-col gap-3'>
                  <div className='flex gap-4'>
                    <div className='w-[110px] text-secondary leading-snug'>Travel Purpose :</div>
                    <div className='flex-1'>{documentDetail?.FlightInfo?.RequestTravelPurposeDescription ?? <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-3'>
                    <div className='w-[110px] text-secondary leading-snug'>Passport Name :</div>
                    <div className='flex-1'>{profileData?.PassportName ? profileData?.PassportName : <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-3'>
                    <div className='w-[110px] text-secondary leading-snug'>Passport Number :</div>
                    <div className='flex-1'>{profileData?.PassportNumber ? profileData?.PassportNumber : <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-4'>
                    <div className='w-[110px] text-secondary leading-snug'>Passport Expiry :</div>
                    <div className='flex-1'>{profileData?.PassportExpiry ? dayjs(profileData?.PassportExpiry).format('YYYY-MM-DD') : <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-4'>
                    <div className='w-[110px] text-secondary leading-snug'>Pick Up Address:</div>
                    <div className='flex-1'>{profileData?.PickUpAddress}</div>
                  </div>
                </div>
                <div className='flex flex-col gap-3'>
                  {
                    isForeign ??
                    <div className='flex gap-3'>
                      <div className='w-[110px] text-secondary leading-snug'>Hometown :</div>
                      <div className='flex-1'>{profileData?.Hometown ?? <span className='text-gray-400'>Not registered</span>}</div>
                    </div>
                  }
                  <div className='flex gap-3'>
                    <div className='w-[150px] text-secondary leading-snug'>Frequent Flyer :</div>
                    <div className='flex-1 '>{profileData?.FrequentFlyer ?? <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-3'>
                    <div className='w-[150px] text-secondary leading-snug'>Emergency Contact Name :</div>
                    <div className='flex-1'>{profileData?.EmergencyContactName ?? <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-3'>
                    <div className='w-[150px] text-secondary leading-snug'>
                      <Tooltip title='Number for this Travel'>
                        <InfoCircleOutlined className='text-sm text-gray-700 cursor-pointer'/>
                      </Tooltip> 
                      <span className='ml-2'>Emergency Contact :</span>
                    </div>
                    <div className='flex-1'>{profileData?.EmergencyContactMobile ?? <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-3'>
                    <div className='w-[150px] text-secondary leading-snug'>Email :</div>
                    <div className='flex-1'>{profileData?.Email ?? <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                  <div className='flex gap-3'>
                    <div className='w-[150px] text-secondary leading-snug'>Hometown:</div>
                    <div className='flex-1'>{profileData?.Hometown ?? <span className='text-gray-400'>Not registered</span>}</div>
                  </div>
                </div>
              </div>
            </Accordion>
            <Accordion className='col-span-6 shadow-md rounded-ot overflow-hidden' title='Travel Info'>
              <TravelInfo
                form={flightInfoForm}
                documentDetail={documentDetail}
                getData={refreshFlightData}
                flightData={flightData}
              />
            </Accordion>
            <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden' title='Travel Detail'>
              <div className='p-4 flex flex-col gap-5'>
                <Flight
                  ref={flightRef}
                  disabled={isDisabled}
                  documentDetail={documentDetail}
                  data={flightData}
                  getData={refreshFlightData}
                  hasUpdatePermission={hasUpdatePermission}
                  currentGroup={currentGroup}
                  userInfo={userInfo}
                  groupAndMembers={groupAndMembers}
                  parentForm={form}
                  hasActionPermission={hasActionPermission}
                />
                <Accommodation
                  disabled={isDisabled}
                  getData={getDocumentDetail}
                  data={accommodationData}
                  currentGroup={currentGroup}
                  documentDetail={documentDetail}
                  userInfo={userInfo}
                  groupAndMembers={groupAndMembers}
                />
              </div>
            </Accordion>
            <Accordion defaultOpen={true} title='Attachments' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
              <Attachments
                disabled={isDisabled}
                documentDetail={documentDetail}
                hasUpdatePermission={hasUpdatePermission}
              />
            </Accordion>
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
                      <div>
                        {
                          nextGroup?.GroupTag === 'Requester' ?
                          <div>
                            Send to: <span className='font-bold'>{documentDetail?.RequesterFullName}</span>
                          </div>
                          :
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
                                    className='w-[150px]'
                                    filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                                    showSearch
                                  />
                                </Form.Item>
                              )
                            }}
                          </Form.Item>
                        }
                      </div>
                    }
                  </div>
                  { currentGroup?.GroupTag !== 'Completed' &&
                    <div className='flex items-center gap-3 justify-end'>
                      <Form.Item noStyle shouldUpdate={(pre, cur) => pre.comment !== cur.comment}>
                        {({getFieldValue}) => {
                          const declineDisabled = !Boolean(getFieldValue('comment'))
                          return renderButtons(nextGroup?.GroupTag, declineDisabled)
                        }}
                      </Form.Item>
                    </div>
                  }
                </div>
              </Accordion>
              :
              userInfo?.EmployeeId === documentDetail?.RequestUserId && documentDetail?.CurrentStatus !== 'Completed' ?
              <Accordion title='Request Document' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
                <div className='flex flex-col gap-4 p-4'>
                  <div className='flex items-center gap-3 justify-end'>
                    <Form.Item noStyle shouldUpdate={(pre, cur) => pre.comment !== cur.comment}>
                      {({getFieldValue}) => {
                        return(
                          <Button
                            htmlType='button'
                            type='danger'
                            onClick={() => clickActionButtons('Cancel')}
                            disabled={actionLoading}
                          >
                            Cancel Request
                          </Button>
                        )
                      }}
                    </Form.Item>
                  </div>
                </div>
              </Accordion>
              : null
            }
            <Accordion defaultOpen={!isDisabled} title='Document History & Comments, Document Route' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
              <div className='grid grid-cols-12 items-start gap-4 p-4 pl-10'>
                <DocumentTimeline
                  data={routeTimeline}
                />
              </div>
            </Accordion>
          </Form>
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

export default NonSiteTravel