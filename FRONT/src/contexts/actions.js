import { message } from 'antd';
import ls from 'utils/ls';

const actions = (dispatch) => ({
  setLoading: (loading) => {
    dispatch({type: 'SET_LOADING', loading})
  },
  setReferLoading: (loading) => {
    dispatch({type: 'SET_REFER_LOADING', loading})
  },
  initToken: (token, userInfo) => {
    ls.set('userInfo', userInfo)
    ls.set('role', userInfo.Role)
    dispatch({type: 'INIT_TOKEN', token: token, userInfo: userInfo})
  },
  logout: () => {
    ls.remove('userInfo')
    dispatch({type: 'LOGOUT'})
  },
  changeMenuKey: async (key) => {
    ls.set('mkey', key)
    dispatch({type: 'CHANGE_MENU_KEY', key})
  },
  changeMainWidth: (width) => {
    dispatch({ type: 'CHANGE_MAIN_CONTAINER_WIDTH', mainWidth: width})
  },
  handleSidebarCollapse: async (isCollapsed) => {
    if(isCollapsed){
      document.documentElement.style.setProperty('--sidebar-width', '344px')
    }
    else{
      document.documentElement.style.setProperty('--sidebar-width', '84px')
    }
    dispatch({type: 'CHANGE_SIDEBAR_WIDTH', sidebarCollapsed: isCollapsed})
  },
  connectionSocket: (connection) => {
    dispatch({ type: 'CONNECTION_SOCKET', socket: connection });
  },
  saveUserProfileData: (data) => {
    dispatch({ type: 'USER_PROFILE_DATA', data: data });
  },
  saveReferData: (data) => {
    ls.set('referData', data)
    dispatch({ type: 'REFER_DATA', data: data });
  },
  setReferDataItem: (data) => {
    let prevData = ls.get('referData')
    ls.set('referData', {...prevData, ...data})
    dispatch({ type: 'SET_REFERDATA_ITEM', data: data });
  },
  changeLoadingStatusReferItem: (loading) => {
    dispatch({ type: 'CHANGE_LOADING_STATUS_REFERDATA_ITEM', loadingStatus: loading });
  },
  changedFlight: (e) => {
    dispatch({ type: 'CHANGED_FLIGHT', data: e});
  },
  changePageForbidden: (e) => {
    dispatch({ type: 'CHANGED_PAGE_FORBIDDEN', status: e});
  },
  onSuccess: (info) => {
    message.success(info)
  },
  changeStatusLoading:(boolean) => {
    dispatch({ type: 'CHANGE_LOADING_STATUS', loading: boolean });
  },
  setErrorStatus: (error) => {
    dispatch({ type: 'SET_ERROR_STATUS', error });
  },
  saveRoomSearchValues: (values) => {
    dispatch({ type: 'SAVE_ROOM_SEARCH_VALUES', values });
  },
  loadingsToFalse: () => {
    dispatch({ type: 'REFER_LOADINGS_TO_FALSE'});
  },
  onError: (info, duration) => {
    if(duration){
      message.error(info, duration)
    }else{
      message.error(info, 5)
    }
  },
  connectionMultiViewer: (connection) => {
    dispatch({ type: 'CONNECTION_MULTIVIEWER', connection});
  },
  setMultiViewers: (viewers) => {
    dispatch({ type: 'SET_MULTIVIEWERS', viewers});
  },
  setShowAgreement: (showAgreement) => {
    dispatch({ type: 'SHOW_AGREEMENT', showAgreement});
  },
  setFlightCalendarDate: (date) => {
    dispatch({ type: 'FLIGHT_CALENDAR_DATE_CHANGE', date});
  },
  setTravelOptions: (options) => {
    dispatch({ type: 'SET_TRAVEL_OPTIONS', options});
  },
})

export default actions
