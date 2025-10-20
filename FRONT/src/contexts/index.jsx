import React from 'react';
import actions from './actions';
import reducer from './reducer'
import ls from 'utils/ls';
import { message, notification } from 'antd';

const AuthContext = React.createContext()

const initialState = {
  loading: true,
  inited: false,
  referLoading: false,
  userInfo: null,
  token: null,
  error: null,
  tabIndexSetting: 0,
  sidebarWidth: '250px',
  sidebarCollapsed: false,
  mainContainerWidth: null,
  theme: null,
  socket: null,
  userProfileData: null,
  pageForbidden: false,
  socket: null,
  flightCalendarDate: null,
  referData: {
    departments: null,
    profileFields: {}
  },
  travelOptions: [],
  loadings: {},
  customLoading: false,
  ChangedFlight: 0,
  menuKey: null,
  roomSearchValues: null,
  roomSearchResult: null,
  connectionMultiViewer: null,
  multiViewers: [],
  showAgreement: false,
}

const AuthContextProvider = ({children}) => {
  const [ state, dispatch ] = React.useReducer(reducer, initialState)
  const action = React.useMemo(() => actions(dispatch), [])
  const [ api, contextHolder] = notification.useNotification();

  React.useLayoutEffect(() => {
    dispatch({ 
      type: 'SET_SOCKET_MESSAGE_API', 
      api: api
    });
    const bootstrapAsync = async () => {
      try {
        // var userInfo = ls.get('userInfo');
        var menuKey = ls.get('mkey');

        // dispatch({ 
        //   type: 'RESTORE_TOKEN', 
        //   token: userInfo ? userInfo.token : null, 
        //   userInfo: userInfo ? userInfo : null,
        // });
        if(menuKey){
          dispatch({ type: 'CHANGE_MENU_KEY', key: menuKey });
        }
        // action.setLoading(false)
      } catch(e) {
      }
    }

    bootstrapAsync();
  }, [])

  return (
    <AuthContext.Provider value={{state, action}}>
      {children}
      {contextHolder}
    </AuthContext.Provider>
  );
}

export {
  AuthContextProvider,
  AuthContext
}
