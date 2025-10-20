import React, { useContext, useLayoutEffect } from 'react';
import axios from "axios";
import { AuthContext, } from 'contexts';
import { notification, Space } from 'antd';
import { Button } from 'components';

const ignoreResponse = [
  '/position/getall',
  'tas/employee/searchshort',
  'tas/activetransport/getactivetransports',
  'tas/position/getall',
  'tas/employee/statusdates',
  'tas/Room/statusbetweendates',
  'tas/room/findavailable',
  'tas/employee/rosterexecutepreview',
  'tas/Room/datestatus',
  'tas/employeestatus/calendarroombookingassign',
  'tas/employee/checkadaccount',
  'tas/room/bulkuploadpreview',
  'tas/employee/bulkuploadpreview',
  'tas/costcode/bulkuploadpreview',
  'tas/position/bulkuploadpreview',
  'tas/employer/bulkuploadpreview',
  'tas/bulkroster/preview',
  'tas/room/dateprofile',
  'tas/transport/dateschedule',
  'tas/notification/all',
  'tas/room/findavailablebydates',
  'requestdocument/documentlist',
  'transportschedule/monthtransportschedule',
  'room/assignanalyze',
  'roomassignment/findavailablebydatesassignment',
  'tas/SysFile',
  'tas/room/analyze',
  'tas/room/statusbetweendates',
  'tas/room/search',
  'tas/room/detaildateprofile',
  'tas/room/findavailablebyroomid',
  'tas/transportschedule/manageschedule',
  'tas/room/statusbetweendates',
  'tas/transportschedule/busstopschedule',
  'tas/transport/searchreschedulepeople',
  'tas/roomassignment/ownership',
  'tas/room/ownerandlockdateprofile',
  'tas/room/dateprofile',
  'tas/employeestatus/roombookingbyroom',
  'tas/room/getmonthroomdata',
  'tas/requestnonsiteticketconfig/extractoption',
  'tas/requestsitetravel/addtravel/checkduplicate',
  'tas/requestsitetravel/reschedule/checkduplicate',
  'tas/requestsitetravel/remove/checkduplicate',
  'tas/SysFile/multi',
  'tas/transport/multiplebooking/preview',
  'tas/transportschedule/datedriveschedule',
  'tas/dashboardtransportadmin/transportgroup',
  'tas/dashboardtransportadmin/transportgroup/employees',
]

const isIgnoreResponse = (url) => {
  return ignoreResponse.some(iUrl => url.includes(iUrl))
}

const reportInstance = axios.create({
  baseURL: process.env.REACT_APP_REPORT_API_URL,
  withCredentials: true,
});

axios.defaults.baseURL = process.env.REACT_APP_MAIN_API_URL
axios.defaults.withCredentials = true
axios.defaults.headers = {
  "Access-Control-Allow-Origin": `*`,
  'Access-Control-Allow-Headers': 'Origin, X-Requested-With, Content-Type, Accept',
  'Content-Security-Policy': "default-src 'self'",
  'X-Content-Type-Options': 'nosniff',
  'X-Frame-Options': 'DENY',
  'X-XSS-Protection': '1; mode=block',
}

const source = axios.CancelToken.source()

const findDuplicates = (list) => {
  const countOccurrences = list?.reduce((acc, num) => {
    acc[num] = (acc[num] || 0) + 1;
    return acc;
  }, {});
  
  const duplicates = Object.entries(countOccurrences)
  .filter(([_, count]) => count > 1)
  .map(([num, count]) => ({ number: num, count }));

  return duplicates
}

const setCommonHeaders = (config) => {
  config.headers["Access-Control-Allow-Origin"] = `*`;
  config.headers['Access-Control-Allow-Headers'] = 'Origin, X-Requested-With, Content-Type, Accept';
  config.headers['Content-Security-Policy'] = "default-src 'self'";
  config.headers['X-Content-Type-Options'] = 'nosniff';
  config.headers['X-Frame-Options'] = 'DENY';
  config.headers['X-XSS-Protection'] = '1; mode=block';

  return config;
};


const extractErrorMessage = async (response) => {
  if (response.data instanceof Blob) {
    const stringData = await response.data.text();
    const responseData = JSON.parse(stringData);
    return responseData.message || 'Operation failed!';
  } else if (response.data) {
    if (response.data.value) {
      return Array.isArray(response.data.value) ? response.data.value[0].msg : response.data.value;
    } else {
      return response.data.message || 'Operation failed!';
    }
  }
  return 'Operation failed!';
};

const AxiosComponent = (props) => {
  const { action } = useContext(AuthContext)
  const [api, contextHolder] = notification.useNotification();

  useLayoutEffect(() => {
    reportInstance.interceptors.request.use( async (config) => {
      return config
    },(error) => {
      return Promise.reject(error)
    })
    ////////     REPORT INSTANECE    ////////////
    reportInstance.interceptors.response.use(
			function (response) {
				if(response.config.method === 'delete') {
					action.onSuccess('Operation successful')
				}
				else if(response.config.method === 'get') {
          if(response.config.url.includes('reportjob/test')){
            action.onSuccess('Operation successful')
          }
        }
				else if(response.config.method === 'post') {
          if(response.config.url !== 'reportjob/validatetime'){
            action.onSuccess('Operation successful')
          }
				}
				else if(response.config.method === 'put') {
					action.onSuccess('Operation successful')
				}
				else if(response.config.method === 'patch') {
					action.onSuccess('Operation successful')
				}
				return response;
			},
			async (error) => {
        if(error.response) {
          const errorMessage = await extractErrorMessage(error.response)
          if(error.response.status === 401){
            action.logout()
					}else if(error.response.status === 400){
            action.onError(errorMessage)
          }else{
            action.onError(errorMessage)
					}
				}
				return Promise.reject(error);
			}
		);

    ////////     TAS INSTANECE    ////////////

    axios.interceptors.request.use( async (config) => {
      if(config.data?.model?.SAPID && (config.url === "tas/employee/search" || config.url === "tas/employee/searchaccommodation")){
        let stringData = config.data?.model?.SAPID
        const numberList = stringData?.match(/\d+/g).map(Number)
        const duplicatedNumbers = findDuplicates(numberList)
        if(duplicatedNumbers.length > 0){
          api.warning({
            message: `Duplicated IDs (${duplicatedNumbers.length})`,
            duration: 0,
            description: <div className='grid text-xs max-h-[250px] overflow-auto'>{duplicatedNumbers.map((item, i) => <div key={`duplicated-item-${i}`}>{item.number} <b>({item.count})</b></div>)}</div>,
            btn: <Space><Button type='primary' onClick={() => api.destroy()}>Confirm</Button></Space>
          })
        }
      }
      return setCommonHeaders(config)
    },
    function (err) {
      return Promise.reject(err);
    });
  
    axios.interceptors.response.use(
      function (response) {
        if(response.config.method === 'post'){
          if(response.config.url.includes('/employee/search') || response.config.url === 'tas/employee/searchaccommodation'){
            if(response.data?.NotFoundSAPIDs?.length > 0){
              const sorted = response.data?.NotFoundSAPIDs?.sort((a, b) => a - b)
              api.info({
                message: `Not Found (${response.data?.NotFoundSAPIDs?.length})`,
                duration: 0,
                description: <div className='grid grid-cols-5 gap-1 text-xs max-h-[calc(100vh-300px)] overflow-auto'>{sorted.map((item, i) => <div key={`not-found-item-${i}`}>{item}</div>)}</div>,
                btn: <Space><Button type='primary' onClick={() => api.destroy()}>Confirm</Button></Space>
              })
            }
          }else if(!isIgnoreResponse(response.config.url)){
            action.onSuccess('Operation successful')
          }
        }
        else if(response.config.method === 'put'){
          action.onSuccess('Operation successful')
        }
        return response;
      },
      async (error) =>  {
        if(error.code === "ERR_NETWORK"){
          api.error({
            message: 'Network error',
            duration: 0,
            description: 'Please check the network'
          });
        }
        // else if(error.code === "ERR_BAD_RESPONSE"){
          // action.setErrorStatus(error.code)
        // }
        if(error.response) {
          if(error.response.status === 401){
            await source.cancel()
            await action.logout()
          }
          else if(error.response.code === 500){
            // action.setErrorStatus(error.response.code)
          }
          else if(error.response.status === 403){
            action.changePageForbidden(true)
          }
          else if(error.response.status === 400){
            if(error.response.config.method === 'post' && error.response.config.url === 'tas/sysrole/adduser'){
              api.error({
                message: 'Validation Warning',
                duration: 0,
                description: <div>
                  <ul className='list-disc'>
                    <li>{error.response.data.message}</li>
                  </ul>
                </div>
              });
            }
            else if(
              (error.response.config.method === 'post' && error.response.config.url === 'tas/employee') ||
              (error.response.config.method === 'patch' && error.response.config.url === 'tas/employee') || 
              (error.response.config.url === 'tas/employee/rosterexecute' && error.response.config.method === 'post') ||
              (error.response.config.method === 'delete' && error.response.config.url === 'tas/activetransport')
            ){
              // no message
            }
            else if((error.response.config.method === 'post' && error.response.config.url === 'tas/transport/addtravel')){
              if(error.response.data?.data){
                api.error({
                  message: 'Validation Warning',
                  duration: 0,
                  description: <div>
                    <ul className='list-disc'>
                      {
                        error.response.data?.data?.map((error) => (
                          <li>{error}</li>
                        ))
                      }
                    </ul>
                  </div>
                });
              }
              else{
                if(error.response.data){
                  if(error.response.data.message) {
                    if(Array.isArray(error.response.data.value)) {
                      api.error({
                        message: 'Warning',
                        duration: 5,
                        description: error.response.data.value[0].msg
                      });
                    }
                    else {
                      api.error({
                        message: 'Warning',
                        duration: 5,
                        description: error.response.data.message
                      });
                    }
                  }
                  else{
                    api.error({
                      message: 'Warning',
                      duration: 5,
                      description: error.response.data.title
                    });
                  }
                }
              }
            }
            else{
              if(error.response.data instanceof Blob){
                let stringData = await error.response.data.text()
                let responseData = JSON.parse(stringData)
                if(responseData.message){
                  action.onError(responseData.message)
                }else{
                  action.onError('Operation failed!')
                }
              }else if(error.response.data){
                if(error.response.data.data) {
                  api.error({
                    message: 'Validation error',
                    duration: 5,
                    description: <div>
                      <ul className='list-disc'>
                        {
                          error.response.data?.data?.map((error) => (
                            <li>{error}</li>
                          ))
                        }
                      </ul>
                    </div>
                  });
                }
                else{
                  if(error.response.data.message) {
                    if(Array.isArray(error.response.data.value)) {
                      api.error({
                        message: 'Warning',
                        duration: 5,
                        description: error.response.data.value[0].msg
                      });
                    }
                    else {
                      api.error({
                        message: 'Warning',
                        duration: 5,
                        description: error.response.data.message
                      });
                    }
                  }
                  else if(error.response.data.title){
                    api.error({
                      message: 'Warning',
                      duration: 5,
                      description: error.response.data.title
                    });
                  }else{
                    api.error({
                      message: 'Warning',
                      duration: 5,
                      description: error.response.data
                    });
                  }
                }
              }
            }
          }
          else if(error.response.data){
            if(error.response.data.message) {
              if(Array.isArray(error.response.data.value)) {
                api.error({
                  message: 'Warning',
                  duration: 5,
                  description: error.response.data.value[0].msg
                });
              }
              else {
                api.error({
                  message: 'Warning',
                  duration: 5,
                  description: error.response.data.message
                });
              }
            }
            else{
              api.error({
                message: 'Warning',
                duration: 5,
                description: error.response.data.title
              });
            }
          }
        }
        return Promise.reject(error);
      }
    );
  }, [])

  return (
    <>
      {contextHolder}
      {props.children}
    </>
  )
}
export { reportInstance }
export default AxiosComponent;
