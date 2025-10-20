import { AuthContext } from 'contexts';
import React, { lazy, useContext, useEffect, useState } from 'react';
import { Outlet, useLocation, useNavigate, useNavigation, useParams } from 'react-router-dom';
import { Button, GlobalLoadingIndicator, Modal, Suspense, Progress } from 'components';
import './index.css';
import axios from 'axios';
import { NetworkError, Page500, Supervisor } from 'modules/public';
import OtLogo from 'assets/icons/OT_logo.png';
import { Checkbox, message } from 'antd';
import ls from 'utils/ls';
import bg from 'assets/images/background-image.webp';
import { LoadingOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import purify from 'dompurify';
import y6 from 'assets/images/y6.webp';
import y5 from 'assets/images/y5.webp';
import y4 from 'assets/images/y4.webp';

const MainLayout = lazy(() => import('./MainLayout'));
const signalR = require('@microsoft/signalr');

const nameList = [ 
  'fieldsOfGroups', 
  'departments', 
  'costCodes',
  'employers',
  'peopleTypes',
  'positions',
  'rosters',
  'roomTypes',
  'locations',
  'nationalities',
  'states',
  'roomStatuses',
  'rooms',
  'camps',
  'transportGroups',
  'noRoomId',
  'transportModes',
  'master',
  'departmentsmini',
  'approvalGroups',
  'profileFields',
  'reportDepartments',
  'reportEmployers',
]


function Layout(props) {
  const { action, state } = useContext(AuthContext);
  const [ percent, setPercent ] = useState(0)
  const [ agreement, setAgreement ] = useState(null)
  const [ isAgree, setIsAgree ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const routeLocation = useLocation();
  const params = useParams();
  const navigate =  useNavigate();
  const navigation =  useNavigation();

  const cacheUserInfo = ls.get('role')


  const checkAuthorize = async () => {
    try {
      const { data: userInfo } = await axios.get('auth/auth/loginadservice', { withCredentials: true })
      if(cacheUserInfo !== userInfo.Role){
        ls.remove('el');
      }

      let callbackRoute = userInfo.Menu.find((item) => item.Route)?.Route;
      action.initToken(userInfo.token, {...userInfo, callbackRoute: callbackRoute});

      switch(userInfo?.Role){
        case 'Guest':
          navigate('/guest')
          if(userInfo?.CreateRequest === 1) checkReferData();
          break;
        case 'Supervisor':
          checkReferData();
          navigate('/supervisor');
          break;
        default:
          checkReferData();
          socketConnectionMultiViewer();
          getAgreementText();
          if(!userInfo.Agreement) action.setShowAgreement(true);
      }
      action.setLoading(false)
    } catch (error) {
      console.error('Authorization check failed', error);
    }
  }
  
  useEffect(() => {
    if(!state.userInfo){
      checkAuthorize()
    }
  },[state.userInfo])


  const checkReferData = () => {
    const referData = ls.get('referData')
    const lastUpdated = ls.get('lastUpdated')
    if(referData){
      if(!lastUpdated || (lastUpdated && dayjs(lastUpdated).add(1, 'h').diff(dayjs())) < 0){
        action.setReferLoading(true)
      }else{
        action.saveReferData(referData)
      }
    }else{
      action.setReferLoading(true)
    }
  }
  
  useEffect(() => {
    if(state.referLoading){
      getAllReferData()
    }
  },[state.referLoading])

  useEffect(() => {
    if(state.connectionMultiViewer && state.userInfo){
      joinRoom()
      state.connectionMultiViewer.on('Rolechanged', (res) => {
        if(res){
          action.logout()
          message.info(res)
        }
      });
    }

    return () => {
      state.connectionMultiViewer && leaveRoom()
    }
  },[state.connectionMultiViewer, state.userInfo])

  const joinRoom = () => {
    try {
      state.connectionMultiViewer?.invoke('JoinSystem', `${state.userInfo?.Id}`)
    }
    catch(e) {
    }
  }

  const leaveRoom = () => {
    action.setMultiViewers([])
    // state.connectionMultiViewer.stop()
    state.connectionMultiViewer?.invoke('LeaveSystem', `${state.userInfo?.Id}`)
  }

  const getAgreementText = () => {
    axios({
      method: 'get',
      url: 'tas/agreement'
    }).then((res) => {
      setAgreement(res.data.AgreementText)
    }).catch((err) => {

    })
  }

  const getAllReferData = () => {
    if(routeLocation?.pathname.includes('/tas/people/search/')){
      axios({
        method: 'get',
        url: `tas/employee/${params?.employeeId}?clearCache=true`,
      }).then((res) => {
        action.saveUserProfileData(res.data)
      })
    }else if(routeLocation?.pathname.includes('/tas/people/search') && !routeLocation?.pathname.includes('/tas/people/search/')){
      axios({
        method: 'get',
        url: `tas/employee/search?clearCache=true`,
      }).then((res) => {
      })
    }

    getAll([
      axios.get('tas/GroupMaster/profiledata').then((res) => {
        return res.data
      }), 
      axios.get('tas/department?Active=1').then((res) => {
        return res.data.map((item) => ({...item, selectable: false}))
      }),
      axios.get('tas/costcode?Active=1').then((res) => {
        return res.data.map((item) => ({
          ...item,
          value: item.Id, 
          label: `${item.Number} ${item.Description}`,
        }))
      }),
      axios.get('tas/employer?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: item.Description, 
          ...item
        }))
      }),
      axios.get('tas/peopletype?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Code}`, 
          ...item
        }))
      }),
      axios.get('tas/position?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Description}`, 
          ...item
        }))
      }),
      axios.get('tas/roster?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Name}`, 
          ...item
        }))
      }),
      axios.get('tas/roomtype?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Description}`, 
          ...item
        }))
      }),
      axios.get('tas/location?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Description}`, 
          ...item
        }))
      }),
      axios.get('tas/nationality?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Description}`, 
          ...item
        }))
      }),
      axios.get('tas/state?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Description}`, 
          ...item
        }))
      }),
      axios.get('tas/shift?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Code} - ${item.Description}`, 
          ...item
        }))
      }),
      axios.get('tas/room?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Number} - ${item.RoomTypeName}`, 
          ...item
        }))
      }),
      axios.get('tas/camp?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Description}`, 
          ...item
        })) 
      }),
      axios.get('tas/flightgroupmaster?Active=1&fullCluster=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: `${item.Description}`, 
          ...item
        }))
      }),
      axios.get('tas/room/getvirtualroomid').then((res) => {
        return res.data
      }),
      axios.get('tas/transportmode?Active=1').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: item.Code, 
          ...item
        }))
      }),
      axios.get('tas/requestdocument/master').then((res) => {
        return {
          documentTypes: res.data.RequestDocumentType?.map((item) => ({
            value: item,
            label: item,
          })),
          paymentConditions: res.data.PaymentCondition?.map((item) => ({
            value: item,
            label: item,
          })),
          searchCurrentSteps: res.data.DocumentSearchCurrentStep?.map((item) => ({
            value: item,
            label: item,
          })),
          favorTimes: res.data.RequestDocumentFavorTime?.map((item) => ({
            value: item,
            label: item,
          })),
        }
      }),
      axios.get('tas/department/minimum').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: item.Name, 
          ...item
        }))
      }),
      axios.get('tas/requestgroup').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: item.Description, 
          ...item
        }))
      }),
      axios.get('tas/profilefield').then((res) => {
        let tmp = {}
        res.data.map((item) => {
          tmp[item.ColumnName] = item
        })
        return tmp
      }),
      axios.get('tas/department/report').then((res) => {
        let tmp = res.data.map((item) => ({...item, selectable: false}))
        return tmp
      }),
      axios.get('tas/employer/report').then((res) => {
        return res.data.map((item) => ({
          value: item.Id, 
          label: item.Description, 
          ...item
        }))
      }),
    ], (p) => setPercent(p))
  }

  const getAll = (promises, progress_percent) => {
    let d = 0;
    progress_percent(0);
    for (const p of promises) {
      p.then(()=> {    
        d ++;
        progress_percent( (d * 100) / promises.length );
      });
    }
    return Promise.all(promises).then( async (res) => {
      await res.map((data, index) => {
        action.setReferDataItem({ [nameList[index]]: data })
        // if('noRoomId' !== nameList[index]){
        //   action.changeLoadingStatusReferItem({ [nameList[index]]: false })
        // }
      })
    }).then(() => {
      ls.set('lastUpdated', dayjs().format('YYYY/MM/DD HH:mm:ss'))
      setTimeout(() => {
        action.setLoading(false)
        action.setReferLoading(false)
        setPercent(0)
      }, 500)
    });
  }

  useEffect(() => {
    if(routeLocation.pathname.split('/')[1] === 'tas'){
      document.documentElement.style.setProperty('--pattern-color', "#1F3C58")
      document.documentElement.style.setProperty('--sidebar-pattern-url', `url(${y4})`)
    }
    else if(routeLocation.pathname.split('/')[1] === 'request'){
      document.documentElement.style.setProperty('--pattern-color', "#311F58")
      document.documentElement.style.setProperty('--sidebar-pattern-url', `url(${y5})`)
    }
    else if(routeLocation.pathname.split('/')[1] === 'report'){
      document.documentElement.style.setProperty('--pattern-color', "#1F583D")
      document.documentElement.style.setProperty('--sidebar-pattern-url', `url(${y6})`)
    }
    else{
      document.documentElement.style.setProperty('--pseudo-bg-image', "none")
    }
  },[routeLocation])

  const handleAgreement = () => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/agreement/agreementcheck',
      data: {
        check: isAgree ? 1 : 0,
      }
    }).then((res) => {
      action.setShowAgreement(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const socketConnectionMultiViewer = () => {
    const newConnect = new signalR.HubConnectionBuilder()
      .withUrl(`${process.env.REACT_APP_CDN_URL}/screenhub`, {
          skipNegotiation: true,
          transport: signalR.HttpTransportType.LongPolling || signalR.HttpTransportType.WebSockets,
        })
      .withAutomaticReconnect([0, 2000, 10000, 30000])
      .build()
    newConnect.start().then(() => {
      action.connectionMultiViewer(newConnect)
    }).catch((err) => {})
  }

  const renderErrorPage = () => {
    switch (state.error){
      case 'ERR_NETWORK': 
        return <NetworkError/>;
      case 'ERR_BAD_RESPONSE': 
        return <Page500/>;
      default: 
        return null;
    }
  }

  if(state.error){
    return renderErrorPage();
  }
  if(state.loading){
    return(
      <div className='flex flex-col h-screen justify-center items-center bg-white z-40 relative'>
        <div className='text-xl'>
          <img src={OtLogo} width={200}/>
        </div>
        <LoadingOutlined style={{fontSize:30, color: '#e57200'}} className='mt-4'/>
        {/* <LoadingOT className='w-[100px] h-[100px]' iconClass='w-[75px]'/> */}
      </div>
    )
  }

  if (state.userInfo) {
    return (
      <div className="relative">
        <GlobalLoadingIndicator/>
        {state.userInfo.Role === 'Guest' ? (
          <Outlet />
        ) : state.userInfo.Role === 'Supervisor' ? (
          <Supervisor />
        ) : (
          <div id="layout">
            {state.referLoading ? (
              <div className="absolute inset-0 flex flex-col h-screen justify-center items-center gap-5 transition-all bg-white duration-300 opacity-100 z-[9999999999]">
                <img src={bg} className="absolute inset-0 w-full h-full object-cover opacity-25" />
                <div className="relative z-[200]">
                  <img src={OtLogo} width={200} />
                  <div className="flex flex-col items-center gap-2 mt-5">
                    <Progress percent={parseInt(percent)} type='line' percentPosition={{ align: 'center'}}/>
                    {/* <div className="text-white drop-shadow-xl">Loading: {parseInt(percent)}%</div>
                    <div className="w-[150px] h-[5px] rounded-md bg-primary bg-opacity-25 relative drop-shadow-lg overflow-hidden">
                      <div className="absolute inset-y-0 bg-primary w-0 transition-all" style={{ width: `${parseInt(percent)}%` }}></div>
                    </div> */}
                  </div>
                </div>
              </div>
            ) : (
              <Suspense><MainLayout/></Suspense>
            )}
          </div>
        )}
        <Modal title="Privacy Policy" open={state.showAgreement} width={800} closable={false} isDraggable={false}>
          <div dangerouslySetInnerHTML={{ __html: purify.sanitize(agreement) }}></div>
          <div className="flex flex-col mt-4">
            <Checkbox onChange={(e) => setIsAgree(e.target.checked)}>Agree</Checkbox>
            {isAgree && (
              <Button className="self-center" type="primary" onClick={handleAgreement} loading={actionLoading}>
                OK
              </Button>
            )}
          </div>
        </Modal>
      </div>
    );
  }

  return null;
}

export default Layout