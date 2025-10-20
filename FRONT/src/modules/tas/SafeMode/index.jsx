import axios from 'axios'
import React, { useContext, useEffect, useState } from 'react'
import { Outlet, useLoaderData, useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts';
import { Button, FlightList, VerticalCalendar } from 'components'
import generateColor from 'utils/generateColor'
import ProfileHeader from './ProfileHeader'

function Profile() {
  const loadedData = useLoaderData()
  const [ loading, setLoading ] = useState(true)
  const [ isNotFountd, setIsNotFound ] = useState(false)
  const [idleTime, setIdleTime] = useState(0);


  const navigate = useNavigate()
  const { employeeId } =  useParams()
  const { state, action } =  useContext(AuthContext)
  const screenName = `employeeProfile_${employeeId}`

  // window.onbeforeunload = function () {
  //   return "Do you really want to close?";
  // };

  // window.onbeforeunload = function () {
  //   return leaveRoom();
  // };

  useEffect(() => {
    action.changeMenuKey('/tas/people/searc')
  },[])

  const resetIdleTime = () => {
    setIdleTime(0);
  };

  useEffect(() => {
    let interval;
    interval = setInterval(() => {
        setIdleTime(prevIdleTime => prevIdleTime + 1);
    }, 1000);
    return () => {
        clearInterval(interval);
    };
  }, []);

useEffect(() => {
  // Exit safe mode if idle time exceeds 10 seconds
  if (idleTime >= 60) {
    navigate(`/tas/people/search/${employeeId}`, { replace: true })
  }
}, [idleTime]);


  useEffect(() => {
    // Add event listeners to reset idle time on any interaction
    window.addEventListener('mousemove', resetIdleTime);
    window.addEventListener('keypress', resetIdleTime);
    window.addEventListener('scroll', resetIdleTime);
    window.addEventListener('click', resetIdleTime);

    // Cleanup event listeners on component unmount
    return () => {
        window.removeEventListener('mousemove', resetIdleTime);
        window.removeEventListener('keypress', resetIdleTime);
        window.removeEventListener('scroll', resetIdleTime);
        window.removeEventListener('click', resetIdleTime);
    };
}, []);


  
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
    async function initData () {
      if(loadedData?.data){
        await action.saveUserProfileData(loadedData.data)
        await setLoading(false)
      }else{
        setIsNotFound(true)
        setLoading(false)
      }
    }
    initData()
    return () => {}
  },[loadedData])
  

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
          isNotFountd ?
          <div className='bg-white rounded-ot px-4 py-3 min-h-[300px] flex flex-col justify-center items-center'>
            <div className='text-xl'>Not Found Employee</div>
            <Button type='primary' className='mt-5' onClick={() => navigate('/tas/people/search')}>Back to List</Button>
          </div>
          :
          <div className='grid grid-cols-12 gap-5'>
            <ProfileHeader data={state.userProfileData}/>
            <div className='w-full col-span-12'>  
              <div className='flex items-start gap-5'>
                <VerticalCalendar employeeId={state.userProfileData.Id}/>
                <FlightList employeeId={state.userProfileData.Id} profileData={state.userProfileData}/>
              </div>
            </div>
            <div className='col-span-12 relative'>
              <Outlet/>
            </div>
          </div>
        )
      }
    </div>
  )
}

export default Profile