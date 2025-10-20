import axios from 'axios'
import React, { useCallback, useContext, useEffect, useState } from 'react'
import { Outlet, useLoaderData, useLocation, useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts';
import { Button, FlightList, VerticalCalendar } from 'components'
import generateColor from 'utils/generateColor'
import ProfileHeader from './ProfileHeader'
import useShortcut from 'hooks/useShortcut'
import { Popup } from 'devextreme-react'
import ls from 'utils/ls';

function Profile() {
  const previousRoute = ls.get('pp_rt')
  
  const loadedData = useLoaderData()
  const [ loading, setLoading ] = useState(true)
  const [ isNotFound, setIsNotFound ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const navigate = useNavigate()
  const { employeeId } =  useParams()
  const { state, action } =  useContext(AuthContext)
  const screenName = `employeeProfile_${employeeId}`
  const location = useLocation()

  const handleShorcut = useCallback(() => {
    if(state?.userInfo?.Role === 'SystemAdmin'){
      setShowPopup(true)
    }
  },[state])

  useShortcut('safeMode', handleShorcut)

  // window.onbeforeunload = function () {
  //   return "Do you really want to close?";
  // };

  // window.onbeforeunload = function () {
  //   return leaveRoom();
  // };

  useEffect(() => {
    console.log('location', location, previousRoute);
    if(typeof previousRoute === 'string' && !location.pathname.includes(previousRoute)){
      navigate(previousRoute)
    }
    action.changeMenuKey('/tas/people/search')
  },[])

  // useEffect( () => {
  //   if(state.userInfo){
      
      
  //     // Subscribe to the group
  //     const handleMessage = (users) => {
  //       console.log('users', users);
  //       let multiViewers = users.map((item) => ({
  //         name: item.userName.split('_')[0],
  //         role: item.userName.split('_')[1],
  //         color: generateColor()
  //       }))
  //       action.setMultiViewers(multiViewers.filter(user => user.name !== `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}`))
  //       // setMessages((prevMessages) => [...prevMessages, message]);
  //     };
  //     startSignalRConnection(handleMessage);
      
  //     subscribeToGroup(`${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`, screenName, handleMessage);
  
  //     // Clean up on unmount
  //     return () => {
  //       unsubscribeFromGroup(`${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`, screenName, handleMessage);
  //       stopSignalRConnection();
  //     };
  //   }
  // }, [state.userInfo]);
  
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

    return () => state.connectionMultiViewer && leaveRoom()
  },[state.connectionMultiViewer, state.userInfo])

  useEffect(() => {
    const initData = async () => {
      try {
        if (loadedData?.data) {
          await action.saveUserProfileData(loadedData.data);
        } else {
          setIsNotFound(true);
        }
      } catch (error) {
        console.error('Error initializing data:', error);
      } finally {
        setLoading(false);
      }
    };
  
    initData();
  }, [loadedData, action]); 
  

  useEffect(() => {
    if(state.ChangedFlight !== 0){
      getData()
    }
  },[state.ChangedFlight])

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

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
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
        (
          isNotFound ?
          <div className='bg-white rounded-ot px-4 py-3 min-h-[300px] flex flex-col justify-center items-center'>
            <div className='text-xl'>Not Found Employee</div>
            <Button type='primary' className='mt-5' onClick={() => navigate('/tas/people/search')}>Back to List</Button>
          </div>
          :
          <>
            <div className='grid grid-cols-12 gap-5 relative'>
              <ProfileHeader data={state.userProfileData}/>
              <div className='w-full col-span-12'>  
                <div className='flex items-start gap-5'>
                  <VerticalCalendar employeeId={state.userProfileData.Id}/>
                  <FlightList employeeId={state.userProfileData.Id} profileData={state.userProfileData}/>
                </div>
              </div>
              <Outlet/>
              <Popup
                visible={showPopup}
                showTitle={false}
                height={'auto'}
                width={350}
              >
                <div>
                  <div className='text-center'>Do you want to enable <b>Safe Mode</b>? This will restrict certain functions for security?</div>
                  <div className='flex gap-5 mt-3 justify-center'>
                    <Button type={'success'} onClick={() => navigate(`/tas/people/search/sm/${employeeId}`)}>Yes</Button>
                    <Button onClick={() => setShowPopup(false)}>No</Button>
                  </div>
                </div>
              </Popup>
            </div>
          </>
        )
      }
    </div>
  )
}

export default Profile