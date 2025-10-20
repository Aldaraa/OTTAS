import { TeamOutlined, SettingOutlined, DashboardOutlined, FormOutlined, DatabaseOutlined, SolutionOutlined, FileAddOutlined, LeftOutlined, FileExclamationOutlined, UserSwitchOutlined, UsergroupAddOutlined, CalendarOutlined } from '@ant-design/icons';
import { Menu, Tabs } from 'antd'
import React, { useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { Link, useLocation } from 'react-router-dom';
import {ReactComponent as Accommodation} from 'assets/icons/accommodation.svg';
import {ReactComponent as Airplane} from 'assets/icons/airplane.svg';
import { AuthContext } from 'contexts';
import axios from 'axios';
import { Modal, Tooltip } from 'components';
import dayjs from 'dayjs';
import VersionDetail from './VersionDetail';

function Sidebar() {
  const [ versionData, setVersionData ] = useState(null)
  const [ versionHistory, setVersionHistory ] = useState([])
  const [ open, setOpen ] = useState(false)
  const location =  useLocation()
  const { action, state } = useContext(AuthContext)
  const [ isCollapsed, setIsCollapsed ] = useState(false)
  const [ currentType, setCurrentType ] = useState(location.pathname.split('/')[1])

  useEffect(() => {
    getVersion()
    getVersionHistory()
  },[])

  useEffect(() => {
    setCurrentType(location.pathname.split('/')[1])
  },[location])

  const getVersion = () => {
    axios({
      method: 'get',
      url: 'tas/sysversion',
    }).then((res) => {
      setVersionData(res.data)
    }).catch((err) => {

    })
  }

  const getVersionHistory = () => {
    axios({
      method: 'get',
      url: `tas/sysversion/versionhistory`
    }).then((res) => {
      setVersionHistory(res.data)
    }).catch((err) => {
  
    })
  }

  const getItem = useCallback(({label, key, icon, children, type, disabled, name, routeName}) => {
    if(state.userInfo?.Menu.find((item) => item.Code === routeName)){
      return { key, icon, children, label, type, disabled };
    }else{
      return null
    }
  },[state.userInfo])

  const tasMenus = useMemo(() => [
    getItem({
      name: 'Dashboard', 
      routeName: 'DashboardTAS', 
      label: <Link to={'/tas'}><span className='font-medium'>Dashboard</span></Link>, 
      key: '/tas/dashboard', 
      icon: <DashboardOutlined style={{fontSize: '16px'}}/>,
    }),
    getItem({
      name: 'Accommodation',
      routeName: 'Accommodation',
      label:<span className='font-medium'>Accommodation</span>, 
      key: 'Accommodation', 
      icon: <Accommodation/>, 
      children: [
        getItem({
          name: 'By Room', 
          routeName: 'ByRoom', 
          label: <Link to='/tas/byroom'><span className='submenu-title'>By Room</span></Link>, 
          key: '/tas/byroom'
        }),
        getItem({
          name: 'Room Calendar', 
          routeName: 'RoomCalendar', 
          label: <Link to='/tas/roomcalendar'><span className='submenu-title'>Room Calendar</span></Link>, 
          key: '/tas/roomcalendar'
        }),
        getItem({
          name: 'Room Assign', 
          routeName: 'RoomAssign', 
          label: <Link to='/tas/roomassign'><span className='submenu-title'>Room Assign</span></Link>, 
          key: '/tas/roomassign'
        }),
        getItem({
          name: 'Room Audit', 
          routeName: 'RoomAudit', 
          label: <Link to='/tas/roomaudit'><span className='submenu-title'>Room Audit</span></Link>, 
          key: '/tas/roomaudit'
        }),
      ]
    }),
    getItem({
      name: 'People',
      routeName: 'People',
      label: <span className='font-medium'>People</span>, 
      key: 'people', 
      icon: <TeamOutlined style={{fontSize: '16px'}}/>, 
      children: [
        getItem({
          name: 'People search', 
          routeName: 'Peoplesearch', 
          label: <Link to='/tas/people/search'><span className='submenu-title'>People search</span></Link>,
          key: '/tas/people/search'
        }),
        getItem({
          name: 'Booking Info', 
          routeName: 'Peoplesearch', 
          label: <Link to='/tas/bookinginfo'><span className='submenu-title'>Booking Info</span></Link>,
          key: '/tas/bookinginfo'
        }),
        getItem({
          name: 'Changes', 
          routeName: 'Changes', 
          label: <span className='submenu-title'>Changes</span>, 
          key: '/tas/changes', 
          children: [
            getItem({
              name: 'Changes Employer', 
              routeName: 'EmployerChanges', 
              label: <Link to='/tas/changesemployer'><span className='submenu-title'>Employer</span></Link>, 
              key: '/tas/changesemployer'
            }),
            getItem({
              name: 'Changes Cost Code', 
              routeName: 'CostCodeChanges', 
              label: <Link to='/tas/changescostcode'><span className='submenu-title'>Cost Code</span></Link>, 
              key: '/tas/changescostcode'
            }),
            getItem({
              name: 'Changes Department', 
              routeName: 'DepartmentChanges', 
              label: <Link to='/tas/changesdepartment'><span className='submenu-title'>Department</span></Link>, 
              key: '/tas/changesdepartment'
            }),
            getItem({
              name: 'Changes Position', 
              routeName: 'Position', 
              label: <Link to='/tas/changesposition'><span className='submenu-title'>Position</span></Link>, 
              key: '/tas/changesposition'
            }),
            getItem({
              name: 'Changes Group', 
              routeName: 'Group', 
              label: <Link to='/tas/changesgroup'><span className='submenu-title'>Group</span></Link>, 
              key: '/tas/changesgroup'
            }),
            getItem({
              name: 'Changes Location', 
              routeName: 'LocationChanges', 
              label: <Link to='/tas/changeslocation'><span className='submenu-title'>Location</span></Link>,
              key: '/tas/changeslocation'
            }),
        ]}),
        getItem({
          name: 'Profile Deactivate', 
          routeName: 'ProfileDeactivate', 
          label: <Link to='/tas/profiledeactivate'><span className='submenu-title'>Profile Deactivate</span></Link>, 
          key: '/tas/profiledeactivate'
        }),
        getItem({
          name: 'Profile Reactivate', 
          routeName: 'ProfileReactivate', 
          label: <Link to='/tas/profilereactivate'><span className='submenu-title'>Profile Reactivate</span></Link>, 
          key: '/tas/profilereactivate'
        }),
      ]
    }),
    getItem({
      name: 'Transport',
      routeName: 'TransportTAS',
      label: <span className='font-medium'>Transport</span>, 
      key: 'transport', 
      icon: <Airplane />, 
      children: [
        getItem({
          name: 'Bulk Roster Execute', 
          routeName: 'BulkRosterExecute', 
          label: <Link to='/tas/bulkrosterexecute'><span className='submenu-title'>Bulk Roster Execute</span></Link>, 
          key: '/tas/bulkrosterexecute'
        }),
        getItem({
          name: 'Roster Booking', 
          routeName: 'RosterBooking', 
          label: <Link to='/tas/rosterbooking'><span className='submenu-title'>Roster Booking</span></Link>, 
          key: '/tas/rosterbooking'
        }),
        getItem({
          name: 'Manage Schedule', 
          routeName: 'ManageSchedule', 
          label: <Link to='/tas/manageschedule'><span className='submenu-title'>Manage Schedule</span></Link>, 
          key: '/tas/manageschedule'
        }),
        getItem({
          name: 'Active Transport', 
          routeName: 'ActiveTransport', 
          label: <Link to='/tas/activetransport'><span className='submenu-title'>Active Transport</span></Link>, 
          key: '/tas/activetransport'
        }),
        getItem({
          name: 'Cluster', 
          routeName: 'Cluster', 
          label: <Link to='/tas/cluster'><span className='submenu-title'>Cluster</span></Link>, 
          key: '/tas/cluster'
        }),
        getItem({
          name: 'Transport Groups', 
          routeName: 'TransportGroups', 
          label: <Link to='/tas/transportgroup'><span className='submenu-title'>Transport Groups</span></Link>, 
          key: '/tas/transportgroup'
        }),
        getItem({
          name: 'Seat Block', 
          routeName: 'SeatBlock', 
          label: <Link to='/tas/seatblock' ><span className='submenu-title'>Seat Block</span></Link>, 
          key: '/tas/seatblock'
        }),
        getItem({
          name: 'Multiple Booking', 
          routeName: 'MultipleBooking', 
          label: <Link to='/tas/multiplebooking'><span className='submenu-title'>Multiple Booking</span></Link>, 
          key: '/tas/multiplebooking'
        }),
        getItem({
          name: 'Reschedule Multiple', 
          routeName: 'RescheduleMultiple', 
          label: <Link to='/tas/reschedule'><span className='submenu-title'>Reschedule Multiple</span></Link>, 
          key: '/tas/reschedule'
        }),
        getItem({
          name: 'Transport Audit', 
          routeName: 'TransportAudit', 
          label: <Link to='/tas/transportaudit'><span className='submenu-title'>Transport Audit</span></Link>, 
          key: '/tas/transportaudit'
        }),
        getItem({
          name: 'Bus Stop Schdeule', 
          routeName: 'BusStopSchedule', 
          label: <Link to='/tas/busstop'><span className='submenu-title'>Bus Stop Schedule</span></Link>, 
          key: '/tas/busstop'
        }),
      ]
    }),
    getItem({
      name: 'Master',
      routeName: 'Master',
      label: <span className='font-medium'>Master</span>, 
      key: 'master', 
      icon: <DatabaseOutlined style={{fontSize:'16px'}}/>, 
      children: [
        getItem({
          name: 'Cost Code', 
          routeName: 'CostCodeMaster', 
          label: <Link to='/tas/costcode'><span className='submenu-title'>Cost Code</span></Link>, 
          key: '/tas/costcode'
        }),
        getItem({
          name: 'Employer', 
          routeName: 'Employer', 
          label: <Link to='/tas/employer'><span className='submenu-title'>Employer</span></Link>, 
          key: '/tas/employer'
        }),
        getItem({
          name: 'Department', 
          routeName: 'Department', 
          label: <Link to='/tas/department'><span className='submenu-title'>Department</span></Link>, 
          key: '/tas/department'
        }),
        getItem({
          name: 'Position', 
          routeName: 'PositionMaster', 
          label: <Link to='/tas/position'><span className='submenu-title'>Position</span></Link>, 
          key: '/tas/position'
        }),
        getItem({
          name: 'Group Master', 
          routeName: 'GroupMaster', 
          label: <Link to='/tas/groupmaster'><span className='submenu-title'>Group Master</span></Link>, 
          key: '/tas/groupmaster'
        }),
        getItem({
          name: 'People Type', 
          routeName: 'PeopleType', 
          label: <Link to='/tas/peopletype'><span className='submenu-title'>People Type</span></Link>,
          key: '/tas/peopletype'
        }),
        getItem({
          name: 'Location', 
          routeName: 'LocationMaster',
          label: <Link to='/tas/location'><span className='submenu-title'>Location</span></Link>,
          key: '/tas/location'
        }),
        // getItem({
        //   name: 'Termination Type', 
        //   routeName: 'TerminationType',
        //   label: <Link to='/tas/terminationtype'><span className='submenu-title'>Termination Type</span></Link>,
        //   key: '/tas/terminationtype'
        // }),
        getItem({
          name: 'Nationality', 
          routeName: 'Nationality', 
          label: <Link to='/tas/nationality'><span className='submenu-title'>Nationality</span></Link>,
          key: '/tas/nationality'
        }),
        getItem({
          name: 'Airport Code',
          routeName: 'AirportCode',
          label: <Link to='/tas/airportcode'><span className='submenu-title'>Airport Code</span></Link>,
          key: '/tas/airportcode'
        }),
        getItem({
          name: 'Travel Agent',
          routeName: 'TravelAgent',
          label: <Link to='/tas/travelagent'><span className='submenu-title'>Travel Agent</span></Link>,
          key: '/tas/travelagent'
        }),
        getItem({
          name: 'Travel Purpose',
          routeName: 'TravelPurpose',
          label: <Link to='/tas/travelpurpose'><span className='submenu-title'>Travel Purpose</span></Link>,
          key: '/tas/travelpurpose'
        }),
        getItem({
          name: 'Request Hotel',
          routeName: 'RequestHotel',
          label: <Link to='/tas/requesthotel'><span className='submenu-title'>Request Hotel</span></Link>,
          key: '/tas/requesthotel'
        }),
        getItem({
          name: 'De-Mobilisation Type',
          routeName: 'De-MobilisationType',
          label: <Link to='/tas/demobilisationtype'><span className='submenu-title'>De-Mobilisation Type</span></Link>,
          key: '/tas/demobilisationtype'
        }),
        getItem({
          name: 'Aimag', 
          routeName: 'Aimag', 
          label: <Link to='/tas/aimag'><span className='submenu-title'>Aimag</span></Link>,
          key: '/tas/aimag'
        }),
        getItem({
          name: 'Profile Field', 
          routeName: 'ProfileField',
          label: <Link to='/tas/profilefield'><span className='submenu-title'>Profile Field</span></Link>,
          key: '/tas/profilefield'
        }),
        getItem({
          name: 'Shift Status', 
          routeName: 'ShiftStatusMaster', 
          label: <span className='submenu-title'>Shift Status</span>, 
          key: '/tas/master/shiftstatus', 
          children: [
            getItem({
              name: 'Shift Status Color List', 
              routeName: 'ColorList', 
              label: <Link to='/tas/shiftstatuscolor'><span className='submenu-title'>Color List</span></Link>, 
              key: '/tas/shiftstatuscolor'
            }),
            getItem({
              name: 'Shift Status List', 
              routeName: 'ShiftStatusList',
              label: <Link to='/tas/shiftstatus'><span className='submenu-title'>Shift Status List</span></Link>,
              key: '/tas/shiftstatus'
            }),
        ]}),
        getItem({
          name: 'Roster', 
          routeName: 'Roster',
          label: <span className='submenu-title'>Roster</span>, 
          key: '/tas/master/roster', 
          children: [
            getItem({
              name: 'Roster Group', 
              routeName: 'RosterGroup',
              label: <Link to='/tas/rostergroup'><span className='submenu-title'>Roster Group</span></Link>,
              key: '/tas/rostergroup'
            }),
            getItem({
              name: 'List', 
              routeName: 'RosterList', 
              label: <Link to='/tas/roster'><span className='submenu-title'>Roster List</span></Link>, 
              key: '/tas/roster'
            }),
        ]}),
        getItem({
          name: 'Transport', 
          routeName: 'TransportMaster', 
          label: <span className='submenu-title'>Transport</span>, 
          key: '/tas/master/transport', 
          children: [
            getItem({
              name: 'Transport Mode', 
              routeName: 'TransportMode', 
              label: <Link to='/tas/transportmode'><span className='submenu-title'>Transport Mode</span></Link>, 
              key: '/tas/transportmode'
            }),
            getItem({
              name: 'Carrier', 
              routeName: 'Carrier', 
              label: <Link to='/tas/carrier'><span className='submenu-title'>Carrier</span></Link>, 
              key: '/tas/carrier'
            }),
        ]}),
        getItem({
          name: 'Room', 
          routeName: 'RoomMaster', 
          label: <span className='submenu-title'>Room</span>, 
          key: '/tas/master/room', 
          children: [
            getItem({
              name: 'Camp', 
              routeName: 'Camp', 
              label: <Link to='/tas/camp'><span className='submenu-title'>Camp</span></Link>, 
              key: '/tas/camp'
            }),
            getItem({
              name: 'Room Type Group', 
              routeName: 'RoomTypeGroup', 
              label: <Link to='/tas/roomtypegroup'><span className='submenu-title'>Room Type Group</span></Link>, 
              key: '/tas/roomtypegroup'
            }),
            getItem({
              name: 'Room',
              routeName: 'RoomRoom',
              label: <Link to='/tas/room'><span className='submenu-title'>Room</span></Link>,
              key: '/tas/room'
            }),
            getItem({
              name: 'Reactivate Room',
              routeName: 'Reactivate Room',
              label: <Link to='/tas/roomreactive'><span className='submenu-title'>Reactivate Room</span></Link>,
              key: '/tas/roomreactive', 
              disabled: true
            }),
            getItem({
              name: 'Deactivate Room', 
              routeName: 'Deactivate Room', 
              label: <Link to='/tas/roomdeactive'><span className='submenu-title'>Deactivate Room</span></Link>,
              key: '/tas/roomdeactive', 
              disabled: true
            }),
        ]}),
      ]
    }),
    getItem({
      name: 'Bulk',
      routeName: 'Bulk',
      label: <span className='font-medium'>Bulk</span>, 
      key: 'bulk', 
      icon: <FormOutlined style={{fontSize: '16px'}}/>, 
      children: [ 
        getItem({
          name: 'Bulk Profile', 
          routeName: 'Profile', 
          label: <Link to='/tas/bulkprofile'><span className='submenu-title'>Profile</span></Link>,
          key: '/tas/bulkprofile'
        }),
        getItem({
          name: 'Bulk Department', 
          routeName: 'DepartmentBulk', 
          label: <Link to='/tas/bulkdepartment'><span className='submenu-title'>Department</span></Link>,
          key: '/tas/bulkdepartment'
        }),
        getItem({
          name: 'Bulk Cost Code', 
          routeName: 'CostCodeBulk', 
          label: <Link to='/tas/bulkcostcode'><span className='submenu-title'>Cost Code</span></Link>,
          key: '/tas/bulkcostcode'
        }),
        getItem({
          name: 'Bulk Group', 
          routeName: 'GroupBulk', 
          label: <Link to='/tas/bulkgroup'><span className='submenu-title'>Group</span></Link>,
          key: '/tas/bulkgroup'
        }),
        getItem({
          name: 'Bulk Room', 
          routeName: 'RoomBulk', 
          label: <Link to='/tas/bulkroom'><span className='submenu-title'>Room</span></Link>,
          key: '/tas/bulkroom'
        }),
        getItem({
          name: 'Bulk Position', 
          routeName: 'PositionBulk', 
          label: <Link to='/tas/bulkposition'><span className='submenu-title'>Position</span></Link>,
          key: '/tas/bulkposition'
        }),
        getItem({
          name: 'Bulk Employer', 
          routeName: 'EmployerBulk', 
          label: <Link to='/tas/bulkemployer'><span className='submenu-title'>Employer</span></Link>,
          key: '/tas/bulkemployer'
        }),
        getItem({
          name: 'Bulk Shift Status', 
          routeName: 'ShiftStatusBulk', 
          label: <Link to='/tas/bulkshiftstatus'><span className='submenu-title'>Shift Status</span></Link>,
          key: '/tas/bulkshiftstatus'
        }),
      ]
    }),
    ///////////////////    USER      ////////////////////
    getItem({
      name: 'Configuration',
      routeName: 'Configuration',
      label: <span className='font-medium'>Configuration</span>, 
      key: 'configuration', 
      icon: <SettingOutlined style={{fontSize: '16px'}}/>, 
      children: [ 
        getItem({
          name: 'Roles', 
          routeName: 'Roles', 
          label: <Link to='/tas/roles'><span className='submenu-title'>Roles</span></Link>,
          key: '/tas/roles'
        }),
        getItem({
          name: 'Request Configuration', 
          routeName: 'RequestConfiguration', 
          label: <span className='submenu-title'>Request Configuration</span>, 
          key: '/tas/systemteam',
          // icon: <Accommodation/>, 
          children: [
            getItem({
              name: 'Approval Groups', 
              routeName: 'ApprovalGroups', 
              label: <Link to='/tas/approvalgroups' ><span className='submenu-title'>Approval Groups</span></Link>,
              key: '/tas/approvalgroups'
            }),
            getItem({
              name: 'Approval Configuration',
              routeName: 'ApprovalConfiguration',
              label: <Link to='/tas/approvalconfig'><span className='submenu-title'>Approval Configuration</span></Link>,
              key: '/tas/approvalconfig'
            }),
          ]
        }),
        getItem({
          name: 'SMTP Configuration', 
          routeName: 'SMTPConfiguration', 
          label: <Link to='/tas/smtpconfig'><span className='submenu-title'>SMTP Configuration</span></Link>,
          key: '/tas/smtpconfig'
        }),
        getItem({
          name: 'PrivacyPolicy', 
          routeName: 'PrivacyPolicy', 
          label: <Link to='/tas/privacypolicy'><span className='submenu-title'>PrivacyPolicy</span></Link>,
          key: '/tas/privacypolicy'
        }),
      ]
    }),
  ],[state.userInfo]);

  const requestMenus = useMemo(() => [
    getItem({
      name: 'Dashboard', 
      routeName: 'DashboardRequest', 
      label: <Link to={'/request'}><span className='font-medium'>Dashboard</span></Link>, 
      key: '/request/dashboard', 
      icon: <DashboardOutlined style={{fontSize: '16px'}}/>,
    }),
    getItem({
      name: 'Task', 
      routeName: 'Task', 
      label: <Link to={'/request/task'} ><span className='font-medium'>Task</span></Link>, 
      key: '/request/task', 
      icon: <SolutionOutlined style={{fontSize: '16px'}} />,
    }),
    getItem({
      name: 'Documents',
      routeName: 'Documents',
      label:<span className='font-medium'>Documents</span>, 
      key: 'documents', 
      icon: <FileAddOutlined style={{fontSize: '16px'}} />, 
      children: [
        getItem({
          name: 'Non Site Travel', 
          routeName: 'NonSiteTravel', 
          label: <Link to='/request/nonsitetravel'><span className='submenu-title'>Non Site Travel</span></Link>,
          key: '/request/nonsitetravel'
        }),
        getItem({
          name: 'TAS Profile Changes', 
          routeName: 'TASProfileChanges', 
          label: <Link to='/request/tasprofilechanges'><span className='submenu-title'>TAS Profile Changes</span></Link>,
          key: '/request/tasprofilechanges'
        }),
        getItem({
          name: 'Site Travel',
          routeName: 'SiteTravel',
          label: <Link to='/request/sitetravel'><span className='submenu-title'>Site Travel</span></Link>,
          key: '/request/sitetravel'
        }),
        getItem({
          name: 'De-Mobilisation',
          routeName: 'De-Mobilisation',
          label: <Link to='/request/de-mobilisation'><span className='submenu-title'>De-Mobilisation</span></Link>,
          key: '/request/de-mobilisation'
        }),
      ]
    }),
    getItem({
      name: 'Existing Bookings', 
      routeName: 'ExistingBookings', 
      label: <Link to={'/request/existingbookings'} ><span className='font-medium'>Existing Bookings</span></Link>, 
      key: '/request/existingbookings', 
      icon: <SolutionOutlined style={{fontSize: '16px'}} />,
    }),
    getItem({
      name: 'Impersonate User', 
      routeName: 'ImpersonateUser', 
      label: <Link to={'/request/impersonateuser'} ><span className='font-medium'>Impersonate User</span></Link>, 
      key: '/request/impersonateuser', 
      icon: <UserSwitchOutlined style={{fontSize: '16px'}}/>,
    }),
    getItem({
      name: 'Hierarchical User', 
      routeName: 'HierarchicalUser', 
      label: <Link to={'/request/hierarchicaluser'} ><span className='font-medium'>Hierarchical User</span></Link>, 
      key: '/request/hierarchicaluser', 
      icon: <UsergroupAddOutlined style={{fontSize: '16px'}} />,
    }),
    getItem({
      name: 'Cancel Request', 
      routeName: 'CancelRequest', 
      label: <Link to={'/request/cancelrequest'} ><span className='font-medium'>Cancel Request</span></Link>, 
      key: '/request/cancelrequest', 
      icon: <FileExclamationOutlined style={{fontSize: '16px'}}/>,
    }),
    getItem({
      name: 'Active Transport', 
      routeName: 'ActiveTransportRequest', 
      label: <Link to={'/request/activetransport'} ><span className='font-medium'>Active Transport</span></Link>, 
      key: '/request/activetransport', 
      icon: <Airplane />, 
    }),
    getItem({
      name: 'Delegation Configuration', 
      routeName: 'DelegationConfiguration', 
      label: <Link to={'/request/delegationconfig'} ><span className='font-medium'>Delegation Configuration</span></Link>, 
      key: '/request/delegationconfig', 
      icon: <Airplane />, 
    }),
    getItem({
      name: 'Shift Visual', 
      routeName: 'ShiftVisual', 
      label: <Link to={'/request/shiftvisual'} ><span className='font-medium'>Shift Visual</span></Link>, 
      key: '/request/shiftvisual', 
      icon: <Airplane />, 
    }),
  ],[state.userInfo]);

  const reportMenus = useMemo(() => [
    getItem({
      name: 'Dashboard', 
      routeName: 'DashboardReport', 
      label: <Link to={'/report'}><span className='font-medium'>Dashboard</span></Link>, 
      key: '/report/dashboard', 
      icon: <DashboardOutlined style={{fontSize: '16px'}}/>,
    }),
    getItem({
      name: 'Template', 
      routeName: 'Template', 
      label: <Link to={'/report/template'} ><span className='font-medium'>Template</span></Link>, 
      key: '/report/template', 
      icon: <SolutionOutlined style={{fontSize: '16px'}} />,
    }),
    getItem({
      name: 'Scheduler',
      routeName: 'Scheduler',
      label: <Link to={'/report/scheduler'} className='font-medium'>Scheduler</Link>, 
      key: '/report/scheduler', 
      icon: <CalendarOutlined style={{fontSize: '16px'}} />, 
    }),
  ],[state.userInfo])

  const vHistory = useMemo(() => {
    return versionHistory.map((item) => ({
      key: item.Id,
      label: `v${item.Version}`,
      children: <VersionDetail versionId={item.Id}/>
    }))
  },[versionHistory])

  const items = {
    "tas": tasMenus,
    "request": requestMenus,
    "report": reportMenus,
  }

  return (
    <div className='sidebar z-10 shadow-md'>
      <div className={`fixed z-20 bottom-[10px] flex items-center border-t bg-white rounded-b-ot px-2 ${isCollapsed ? 'w-[80px] justify-center' : 'w-[240px] justify-between'} h-[40px]`}>
        {
          !isCollapsed ?
          <>
            {
              versionData ?
              <div className='flex gap-1 text-gray-400'>
                <Tooltip title={<span>Released date: {dayjs(versionData?.ReleaseDate).format('YYYY-MM-DD')}</span>}>
                  <button className='text-xs hover:underline' onClick={() => setOpen(true)}>version {versionData?.Version}</button>
                </Tooltip>
              </div>
              : 
              <div></div>
            }
          </>
          : null
        }
        <button 
          type='button'
          className='relative z-40 cursor-pointer flex items-center p-2 rounded-full transition-all  hover:text-primary hover:bg-black hover:bg-opacity-[0.06]' 
          onClick={() => setIsCollapsed(!isCollapsed)}
        >
          <LeftOutlined rotate={isCollapsed ? 180 : 0} className='cursor-pointer'/>
        </button>
      </div>
      <Menu
        id='tas'
        mode="inline"
        defaultOpenKeys={['people', 'documents']}
        selectedKeys={[state.menuKey]}
        onSelect={(e) => action.changeMenuKey(e.key)}
        items={items[currentType]}
        className={`transition-all duration-0 pb-[38px] bg-transparent ${isCollapsed ? '' : 'w-[240px]'}`}
        inlineCollapsed={isCollapsed}
      />
      <Modal title={`Version History`} open={open} onCancel={() => setOpen(false)}>
        <Tabs
          items={vHistory}
        />
      </Modal>
    </div>
  )
}

export default Sidebar