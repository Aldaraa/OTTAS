import axios from 'axios'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect, useState } from 'react'
import { redirect, useNavigate } from 'react-router-dom'
import OtLogo from 'assets/icons/OT_logo.png'
import ls from 'utils/ls'
import { NetworkError, Page500 } from '..'
import { notification } from 'antd'
import { LoadingOutlined } from '@ant-design/icons'

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
  'master',
]

function Unauthorized() {
  const [ percent, setPercent ] = useState(0)
  const [ noReferData, setNoReferData ] = useState(false)
  const { action, state } =  useContext(AuthContext)
  const navigate = useNavigate()
  const source = axios.CancelToken.source();
  const [ api, contextHolder ] = notification.useNotification()

  useEffect(() => {
    checkAuthorize()
  },[])
  
  const checkAuthorize = async () => {
    await axios({
      method: 'get',
      url: 'auth/auth/loginadservice',
      withCredentials: true,
    }).then( async (res) => {
      let userInfo = res.data
      action.initToken(res.data.token, userInfo)
      if(userInfo.Role === 'Guest'){
        navigate('/guest') 
      }else{
        if(!res.data.Agreement){
          action.setShowAgreement(true)
        }
        checkReferData()
      }
    })
  }

  const checkReferData = () => {
    const referData = ls.get('referData')
    if(referData){
      action.saveReferData(referData)
      navigate('/tas')
    }else{
      setNoReferData(true)
      getAll([
        axios.get('tas/GroupMaster/profiledata').then((res) => {
          return res.data
        }), 
        axios.get('tas/department?Active=1').then((res) => {
          return res.data
        }), 
        axios.get('tas/costcode?Active=1').then((res) => {
          return res.data.map((item) => ({
            value: item.Id, 
            label: `#${item.Number} ${item.Code}-${item.Description} `,
            content: () => <div className='flex gap-2'>
              <span className='text-gray-400'>#{item.Number}</span>
              <span>{item.Code} - {item.Description}</span>
            </div>,
            ...item
          }))
        }),
        axios.get('tas/employer?Active=1').then((res) => {
          return res.data.map((item) => ({
            value: item.Id, 
            label: `${item.Code} - ${item.Description}`, 
            ...item
          }))
        }),
        axios.get('tas/peopletype?Active=1').then((res) => {
          return res.data.map((item) => ({
            value: item.Id, 
            label: `${item.Code} - ${item.Description}`, 
            ...item
          }))
        }),
        axios.get('tas/position?Active=1').then((res) => {
          return res.data.map((item) => ({
            value: item.Id, 
            label: `${item.Code} - ${item.Description}`, 
            ...item
          }))
        }),
        axios.get('tas/roster?Active=1').then((res) => {
          return res.data.map((item) => ({
            value: item.Id, 
            label: `${item.Description}`, 
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
            label: `${item.Code} - ${item.Description}`, 
            ...item
          }))
        }),
        axios.get('tas/state?Active=1').then((res) => {
          return res.data.map((item) => ({
            value: item.Id, 
            label: `${item.Code} - ${item.Description}`, 
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
            label: `${item.Code} - ${item.Description}`, 
            ...item
          })) 
        }),
        axios.get('tas/flightgroupmaster?Active=1&fullCluster=1').then((res) => {
          return res.data.map((item) => ({
            value: item.Id, 
            label: `${item.Code} - ${item.Description}`, 
            ...item
          }))
        }),
        axios.get('tas/room/getvirtualroomid').then((res) => {
          return res.data.Id
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
      ], (p) => setPercent(p))
    }
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
        if('noRoomId' !== nameList[index]){
          action.changeLoadingStatusReferItem({ [nameList[index]]: false })
        }
      })
    }).then(() => {
      setTimeout(() => {
        action.setLoading(false)
        navigate('/tas')
      }, 500)
    });
  }

  const renderErrorPage = () => {
    if(state.error === 'ERR_NETWORK'){
      return(<NetworkError/>)
    }
    else if(state.error === "ERR_BAD_RESPONSE"){
      return(<Page500/>)
    }
  }

  return (
    <>
      {
        state.error ? renderErrorPage() :
        <div className='flex flex-col h-screen justify-center items-center bg-white z-40 relative'>
          <div className='text-xl'>
            <img src={OtLogo} width={200}/>
          </div>
          {
            noReferData &&
            <div className='flex flex-col items-center gap-2 mt-5'>
              <div>Loading: {parseInt(percent)}%</div>
              <div className='w-[150px] h-[5px] rounded-md bg-primary bg-opacity-25  relative overflow-hidden'>
                <div className={`absolute inset-y-0 bg-primary w-0 transition-all`} style={{width: `${parseInt(percent)}%`}}></div>
              </div>
            </div>
          }
          {/* <LoadingOutlined style={{fontSize:30, color: '#e57200'}} className='mt-4'/> */}
        </div>
      }
      {contextHolder}
    </>
  )
}

export default Unauthorized