  const reducer = (prevState, action) => {
    switch (action.type) {
      case 'SET_LOADING':
        return {
          ...prevState,
          loading: action.loading
        }
      case 'SET_REFER_LOADING':
        return {
          ...prevState,
          referLoading: action.loading
        }
      case 'RESTORE_TOKEN':
        return {
          ...prevState,
          token: action.token,
          userInfo: action.userInfo,
        }
      case 'INIT_TOKEN':
        return {
          ...prevState,
          token: action.token,
          userInfo: action.userInfo,
        }
      case 'LOGOUT':
        return {
          ...prevState,
          token: null,
          userInfo: null,
        }
      case 'CHANGE_MENU_KEY':
        return {
          ...prevState,
          menuKey: action.key,
        }
      case 'CHANGE_SIDEBAR_WIDTH':
        return {
          ...prevState,
          sidebarWidth: action.sidebarWidth,
          sidebarCollapsed: action.sidebarCollapsed
        }
      case 'CHANGE_MAIN_CONTAINER_WIDTH':
        return {
          ...prevState,
          mainContainerWidth: action.mainWidth,
        }
      case 'CHANGE_THEME':
        return {
          ...prevState,
          theme: action.status
        };    
      case 'CHANGED_FLIGHT':
        return {
          ...prevState,
          ChangedFlight: action.data
        };    
      case 'CHANGE_LOADING_STATUS':
        return {
          ...prevState,
          customLoading: action.loading
        };    
      case 'CONNECTION_SOCKET':
        return {
          ...prevState,
          socket: action.socket
        }
      case 'USER_PROFILE_DATA':
        return {
          ...prevState,
          userProfileData: action.data
        }
      case 'REFER_DATA':
        return {
          ...prevState,
          referData: {...action.data},
        }
      case 'SET_REFERDATA_ITEM': 
        return {
          ...prevState,
          referData: {
            ...prevState.referData,
            ...action.data
          },
        }
      case 'CHANGE_LOADING_STATUS_REFERDATA_ITEM': 
        return {
          ...prevState,
          loadings: {
            ...action.loadingStatus
          },
        }
      case 'CHANGED_PAGE_FORBIDDEN': 
        return {
          ...prevState,
          pageForbidden: action.status,
        }
      case 'CONNECTION_MULTIVIEWER': 
        return {
          ...prevState,
          connectionMultiViewer: action.connection,
        }
      case 'SET_ERROR_STATUS': 
        return {
          ...prevState,
          error: action.error,
        }
      case 'SAVE_ROOM_SEARCH_VALUES': 
        return {
          ...prevState,
          roomSearchValues: action.values,
        }
      case 'REFER_LOADINGS_TO_FALSE': 
        return {
          ...prevState,
        }
      case 'SET_MULTIVIEWERS': 
        return {
          ...prevState,
          multiViewers: action.viewers,
        }
      case 'SET_SOCKET_MESSAGE_API': 
        return {
          ...prevState,
          socketMessageApi: action.api,
        }
      case 'SHOW_AGREEMENT':
        return {
          ...prevState,
          showAgreement: action.showAgreement
        };  
      case 'FLIGHT_CALENDAR_DATE_CHANGE':
        return {
          ...prevState,
          flightCalendarDate: action.date
        };  
      case 'SET_TRAVEL_OPTIONS':
        return {
          ...prevState,
          travelOptions: action.options
        };  
      default:
        return prevState;
    }
  }

  export default reducer
