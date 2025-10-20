import axios from 'axios'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts';
import { Input, Form as AntForm} from 'antd';
import { Accordion, Button, DocumentTimeline, Form, ProfileCalendar, RequestDocumentHeader, Table, Tooltip } from 'components'
import { SaveFilled, StarFilled } from '@ant-design/icons'
import { Popup } from 'devextreme-react'
import checkPermission from 'utils/checkPermission';
import generateColor from 'utils/generateColor';
import useQuery from 'utils/useQuery';

function DeMobilisation() {
  const [ documentDetail, setDocumentDetail ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  const [ actionType, setActionType ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ types, setTypes ] = useState([])
  const [ docRoutes, setDocRoutes ] = useState([])
  const [ routeTimeline, setRouteTimeline ] =  useState([])
  const [ groupAndMembers, setGroupAndMembers ] = useState([])
  const [ nextGroup, setNextGroup ] = useState(null)
  const [ currentGroup, setCurrentGroup ] = useState(null)
  const [ isEdit, setIsEdit ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ userInfo, setUserInfo ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ profileData, setProfileData ] = useState(null)

  const { documentId } =  useParams()
  const query =  useQuery()
  const { state, action } =  useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const [ demobform ] = AntForm.useForm()
  const screenName = `requestDocumentDetail_${documentId}`

  const navigate = useNavigate()

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
      setLoading(true)
      action.changeMenuKey('/request/task')
      axios.all([
        `tas/requestdemobilisation/${documentId}`,
        `tas/requestdocumenthistory/${documentId}`,
        'tas/requestdemobilisationtype?active=1',
        `tas/requestdocument/myinfo`,
        `tas/requestgroupconfig/documentroute/${documentId}`,
        `/tas/requestgroupconfig/groupandmembers/${documentId}`,
      ].map((endpoint) => axios.get(endpoint))).then(axios.spread((document, history, type, myinfo, documentRoute, groupandmembers) => {
        if(document.data){
          getProfile(document.data.EmployeeId)
          setDocumentDetail(document.data)
        }
        setRouteTimeline(history.data)
        setTypes(type.data)
        setUserInfo(myinfo.data)
        // setGroupAndMembers(groupandmembers.data)
        // setDocRoutes(documentRoute.data)
        calculateNextGroup(documentRoute.data, groupandmembers.data)
      })).catch(() => {
        // setLoading(false)
      }).finally(() => setLoading(false))
    }
  },[documentId])

  // useEffect(() => {
  //   if(documentDetail){
  //     getDocumentMembers()
  //   }
  // },[documentDetail])

  // useEffect(() => {
  //   if(groupAndMembers){
  //     getDocRoutes()
  //   }
  // },[groupAndMembers])

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
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdemobilisation/${documentId}`,
    }).then((res) => {
      setDocumentDetail(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  // const getDocRoutes = () => {
  //   axios({
  //     method: 'get',
  //     url: `tas/requestgroupconfig/documentroute/${documentId}`
  //   }).then((res) => {
  //     setDocRoutes(res.data)
  //     calculateNextGroup(res.data)
  //   }).catch((err) => {
  
  //   })
  // }

  // const getDocumentMembers = (e) => {
  //   axios({
  //     method: 'get',
  //     url: `/tas/requestgroupconfig/groupandmembers/${documentId}`,
  //   }).then((res) => {
  //     setGroupAndMembers(res.data)
  //   })
  // }

  const getRouteHistory = () => {
    axios({
      method: 'get',
      url: `tas/requestdocumenthistory/${documentId}`,
    }).then((res) => {
      setRouteTimeline(res.data)
    }).catch((err) => {

    })
  }

  const calculateNextGroup = (documentRoutes, groupAndMembers) => {
    let currentGroup = documentRoutes?.find((group) => group.CurrentPosition === 1);
    setCurrentGroup(currentGroup)
    ///////// if current position is REQUESTER ///////////
    if(currentGroup?.GroupTag === 'Requester'){
      let currentGroupIndex = documentRoutes?.findIndex((group) => group?.CurrentPosition === 1)
      let nextGroupData = documentRoutes[currentGroupIndex+1]
      let nextGroup = groupAndMembers?.find((item) => item.id === nextGroupData?.GroupId)
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
      ///////// if current position is DATA APPROVAL ///////////
      setNextGroup({GroupTag: 'complete'})
    }
  }

  // const docRouteCols = [
  //   {
  //     label: 'Group',
  //     name: 'GroupName',
  //   },
  //   {
  //     label: 'Current Position',
  //     name: 'CurrentPosition',
  //     alignment: 'left',
  //     cellRender: (e) => (
  //       <div>{e.value === 1 ? <StarFilled style={{color: '#e57200'}}/> : ''}</div>
  //     )
  //   },
  // ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    if(actionType === 'Approve'){
      axios({
        method: 'put',
        url: 'tas/requestdocument/approve',
        data: {
          id: documentId,
          assignEmployeeId: values.assignedEmployeeId,
          nextGroupId: values.nextGroupId,
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
    else if(actionType === 'Complete'){
      axios({
        method: 'put',
        url: `tas/requestdemobilisation/complete/${documentId}`,
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

  const fields = [
    {
      label: 'Date of Completion',
      name: 'CompletionDate',
      className: 'col-span-12 mb-0',
      type: 'date',
    },
    {
      label: 'De-Mobilisation Type',
      name: 'RequestDeMobilisationTypeId',
      className: 'col-span-12 mb-0',
      type: 'select',
      inputprops: {
        options: types.map((item) => ({label: `${item.Code} ${item.Description}`, value: item.Id}))
      }
    },
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-12 mb-0',
      type: 'select',
      inputprops: {
        options: state.referData?.employers,
      }
    },
    {
      label: 'Comments',
      name: 'comment',
      className: 'col-span-12 mb-0',
      type: 'textarea',
    },
  ]

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
      case 'Save':
        return (<>
          <div>Are you sure you want to save this request?</div>
          <div className='flex gap-5 mt-4 justify-center'>
            <Button type='primary'onClick={() => form.submit()} loading={actionLoading}>Yes</Button>
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
          <div>Are you sure you want to complete this document?</div>
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
      if(groupTag !== 'complete'){
        return(
          <>
            {
              currentGroup?.GroupTag === 'Requester' ? 
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
    }else{
      return null
    }
  }

  const handleSave = (values) => {
    setSubmitLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestdemobilisation',
      data: {
        ...values,
        id: documentDetail?.info?.Id,
      }
    }).then((res) => {
      setIsEdit(false)
      getDocumentDetail()
      getRouteHistory()
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
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
        <div className='grid grid-cols-12 gap-3 relative'>
          <RequestDocumentHeader profileData={profileData} documentDetail={documentDetail}/>
          <Accordion className='col-span-12 shadow-md rounded-ot overflow-hidden' title='Travel Calendar'>
            <div className='p-4'>
              <ProfileCalendar profile={profileData}/>
            </div>
          </Accordion>
          <Accordion className='col-span-6 shadow-md rounded-ot overflow-hidden' title='Detail'>
            <Form 
              editData={documentDetail?.info} 
              fields={fields} 
              form={demobform}
              className='grid grid-cols-12 gap-y-2 p-4'
              size='small'
              disabled={!isEdit}
              onFinish={handleSave}
            >
               <div className='col-span-12 mt-3 flex justify-end'>
                  {
                    hasActionPermission ?
                    (isEdit ?
                    <div className='flex items-center gap-3'>
                      <Button type='primary' loading={submitLoading} onClick={() => demobform.submit()} icon={<SaveFilled/>}>Save</Button>
                      <Button onClick={() => setIsEdit(false)} disabled={submitLoading}>Cancel</Button>
                    </div>
                    :
                    <Button onClick={() => setIsEdit(true)}>Edit</Button>
                    )
                    :
                    null    
                  }
                </div>
            </Form>
          </Accordion>
          {
            hasActionPermission ?
            <Form 
              className='grid grid-cols-12 gap-y-5 col-span-6'
              size='small'
              form={form}
              labelWrap={true}
              onFinish={handleSubmit}
            >
              <Accordion title='Request Document' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
                <div className='flex flex-col gap-4 p-4'>
                  <div>
                    Comments:
                    <Form.Item className='m-0' name='comment'>
                      <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                    </Form.Item>
                  </div>
                  {/* <div className='flex gap-10'>
                    {
                      nextGroup &&
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
                  </div> */}
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
            </Form>
            :
            // userInfo?.EmployeeId === documentDetail?.RequestUserId && documentDetail?.CurrentStatus === 'Submitted' ?
            userInfo?.EmployeeId === documentDetail?.RequestUserId && documentDetail?.CurrentStatus !== 'Completed' ?
            <Form 
              className='grid grid-cols-12 gap-y-5 col-span-6'
              size='small'
              form={form}
              labelWrap={true}
              onFinish={handleSubmit}
            >
              <Accordion title='Request Document' className='col-span-12 shadow-md rounded-ot overflow-hidden'>
                <div className='flex flex-col gap-4 p-4'>
                  <div>
                    Comments:
                    <Form.Item className='m-0' name='comment'>
                      <Input.TextArea autoSize={{minRows: 3, maxRows: 5}} maxLength={300} showCount/>
                    </Form.Item>
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
            </Form>
            :
            null
          }
          <Accordion title='Document History & Comments, Document Route' defaultOpen={false} className='col-span-12 shadow-md rounded-ot overflow-hidden'>
            <div className='grid grid-cols-12 items-start gap-4 p-4 pl-10'>
              <DocumentTimeline
                data={routeTimeline}
              />
              {/* <Table
                data={docRoutes}
                columns={docRouteCols}
                pager={false}
                containerClass='col-span-6 shadow-none border border-gray-300'
              /> */}
            </div>
          </Accordion>
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
      }
    </div>
  )
}

export default DeMobilisation