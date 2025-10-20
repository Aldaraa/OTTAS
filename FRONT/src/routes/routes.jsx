import { lazy } from 'react';
import { Link, Navigate } from 'react-router-dom';
import { ErrorPage } from 'modules/public';
import { SafeProfile } from 'modules/tas';
import { SafeFlight, SafeRoomBooking } from 'modules/tas/SafeMode/pages';
import axios from 'axios';
import GuestProtectedRoute from './GuestProtectedRoute';
import { LocalError, ProtectedSuspense } from 'components';
import SafeModeProtect from './SafeModeProtect';
import ls from 'utils/ls';

const Page404 = lazy(() => import("modules/public/Page404"))
const Supervisor = lazy(() => import("modules/public/Supervisor"))
const Dashboard = lazy(() => import("modules/tas/Dashboard"))
const Notifications = lazy(() => import("modules/tas/Notifications"))
// Accommodation
const Bed = lazy(() => import("modules/tas/Accommodation/Bed"))
const ByRoom = lazy(() => import("modules/tas/Accommodation/ByRoom"))
const Camp = lazy(() => import("modules/tas/Accommodation/Camp"))
const Room = lazy(() => import("modules/tas/Accommodation/Room"))
const RoomAssign = lazy(() => import("modules/tas/Accommodation/RoomAssign"))
const RoomAssignDetail = lazy(() => import("modules/tas/Accommodation/RoomAssignDetail/RoomAssignDetail"))
const RoomAssignLayout = lazy(() => import("modules/tas/Accommodation/RoomAssignDetail"))
const RoomAudit = lazy(() => import("modules/tas/Accommodation/RoomAudit"))
const RoomCalendar = lazy(() => import("modules/tas/Accommodation/RoomCalendar"))
const RoomCalendarDetail = lazy(() => import("modules/tas/Accommodation/RoomCalendarDetail"))
const RoomTypeGroup = lazy(() => import("modules/tas/Accommodation/RoomTypeGroup"))
// Bulk
const BulkCostCode = lazy(() => import("modules/tas/BulkProcessing/BulkCostCode"))
const BulkDepartment = lazy(() => import("modules/tas/BulkProcessing/BulkDepartment"))
const BulkEmployer = lazy(() => import("modules/tas/BulkProcessing/BulkEmployer"))
const BulkPosition = lazy(() => import("modules/tas/BulkProcessing/BulkPosition"))
const BulkProfile = lazy(() => import("modules/tas/BulkProcessing/BulkProfile"))
const BulkRoom = lazy(() => import("modules/tas/BulkProcessing/BulkRoom"))
const BulkShiftStatus = lazy(() => import("modules/tas/BulkProcessing/BulkShiftStatus"))
// People
const BookingInfo = lazy(() => import("modules/tas/People/BookingInfo"))
const PendingDeactivate = lazy(() => import("modules/tas/People/PendingDeactivate"))
const PendingReactivate = lazy(() => import("modules/tas/People/PendingReactivate"))
const Profile = lazy(() => import("modules/tas/People/Profile"))
const ProfileDeactivate = lazy(() => import("modules/tas/People/ProfileDeactivate"))
const ProfileReactivate = lazy(() => import("modules/tas/People/ProfileReactivate"))
const Search = lazy(() => import("modules/tas/People/Search"))
// Employee Profile
const AccountHistory = lazy(() => import("modules/tas/People/Profile/pages/AccountHistory"))
const Audit = lazy(() => import("modules/tas/People/Profile/pages/Audit"))
const Flight = lazy(() => import("modules/tas/People/Profile/pages/Flight"))
const NoShowAndGoShow = lazy(() => import("modules/tas/People/Profile/pages/NoShow_GoShow"))
const ProfileByDate = lazy(() => import("modules/tas/People/Profile/pages/ProfileByDate"))
const RoleCenter = lazy(() => import("modules/tas/People/Profile/pages/RoleCenter"))
const RoomBooking = lazy(() => import("modules/tas/People/Profile/pages/RoomBooking"))
const RosterDetail = lazy(() => import("modules/tas/People/Profile/pages/RosterDetail"))
const ShiftVisual = lazy(() => import("modules/tas/People/Profile/pages/ShiftVisual"))
const MoreData = lazy(() => import("modules/tas/People/Profile/pages/moreData"))
// Changes
const ChangesCostCode = lazy(() => import("modules/tas/People/Changes/CostCode"))
const ChangesDepartment = lazy(() => import("modules/tas/People/Changes/Department"))
const ChangesEmployer = lazy(() => import("modules/tas/People/Changes/Employer"))
const ChangesGroup = lazy(() => import("modules/tas/People/Changes/Group"))
const ChangesLocation = lazy(() => import("modules/tas/People/Changes/Location"))
const ChangesPosition = lazy(() => import("modules/tas/People/Changes/Position"))
// Master
const Aimag = lazy(() => import("modules/tas/Master/Aimag"))
const AirportCode = lazy(() => import("modules/tas/Master/AirportCode"))
const CostCode = lazy(() => import("modules/tas/Master/CostCode"))
const DeMobilisationType = lazy(() => import("modules/tas/Master/DeMobilisationType"))
const Department = lazy(() => import("modules/tas/Master/Department"))
const DepartmentDetail = lazy(() => import("modules/tas/Master/DepartmentDetail"))
const Employer = lazy(() => import("modules/tas/Master/Employer"))
const GroupDetail = lazy(() => import("modules/tas/Master/GroupDetail"))
const GroupMaster = lazy(() => import("modules/tas/Master/GroupMaster"))
const Location = lazy(() => import("modules/tas/Master/Location"))
const Nationality = lazy(() => import("modules/tas/Master/Nationality"))
const PeopleType = lazy(() => import("modules/tas/Master/PeopleType"))
const Position = lazy(() => import("modules/tas/Master/Position"))
const ProfileField = lazy(() => import("modules/tas/Master/ProfileField"))
const RequestHotel = lazy(() => import("modules/tas/Master/RequestHotel"))
const Roster = lazy(() => import("modules/tas/Master/Roster"))
const RosterGroup = lazy(() => import("modules/tas/Master/RosterGroup"))
const RosterMaster = lazy(() => import("modules/tas/Master/RosterMaster"))
const ShiftStatus = lazy(() => import("modules/tas/Master/ShiftStatus"))
const ShiftStatusColor = lazy(() => import("modules/tas/Master/ShiftStatusColor"))
const TravelAgent = lazy(() => import("modules/tas/Master/TravelAgent"))
const TravelPurpose = lazy(() => import("modules/tas/Master/TravelPurpose"))
// Transport
const BulkRosterExecute = lazy(() => import("modules/tas/Transport/BulkRosterExecute"))
const BusTimetable = lazy(() => import("modules/tas/Transport/BusTimetable"))
const Carrier = lazy(() => import("modules/tas/Transport/Carrier"))
const Cluster = lazy(() => import("modules/tas/Transport/Cluster"))
const ClusterDetail = lazy(() => import("modules/tas/Transport/ClusterDetail"))
const ManageSchedule = lazy(() => import("modules/tas/Transport/ManageSchedule"))
const MultipleBooking = lazy(() => import("modules/tas/Transport/MultipleBooking"))
const RescheduleMultiple = lazy(() => import("modules/tas/Transport/RescheduleMultiple"))
const RosterBooking = lazy(() => import("modules/tas/Transport/RosterBooking"))
const SeatBlock = lazy(() => import("modules/tas/Transport/SeatBlock"))
const SeatBlockDetail = lazy(() => import("modules/tas/Transport/SeatBlock Detail"))
const TransportActive = lazy(() => import("modules/tas/Transport/TransportActive"))
const TransportAudit = lazy(() => import("modules/tas/Transport/TransportAudit"))
const TransportAuditDetail = lazy(() => import("modules/tas/Transport/TransportAuditDetail"))
const TransportGroup = lazy(() => import("modules/tas/Transport/TransportGroup"))
const TransportGroupDetail = lazy(() => import("modules/tas/Transport/TransportGroupDetail"))
const TransportMode = lazy(() => import("modules/tas/Transport/TransportMode"))
const TransportSchedule = lazy(() => import("modules/tas/Transport/TransportSchedule"))
// User
const PrivacyPolicy = lazy(() => import("modules/tas/Users/PrivacyPolicy"))
const RoleDetail = lazy(() => import("modules/tas/Users/RoleDetail"))
const Roles = lazy(() => import("modules/tas/Users/Roles"))
const SMTPConfig = lazy(() => import("modules/tas/Users/SMTP_Config"))
// Workflow
const ApprovalConfiguration = lazy(() => import("modules/tas/WorkflowConfig/ApprovalConfig"))
const ApprovalGroupDetail = lazy(() => import("modules/tas/WorkflowConfig/ApprovalGroupDetail"))
const ApprovalGroups = lazy(() => import("modules/tas/WorkflowConfig/ApprovalGroups"))
// Request module
const ActiveTransportSchedule = lazy(() => import("modules/request/ActiveTransportSchedule"))
const CancelRequest = lazy(() => import("modules/request/CancelRequest"))
const RequestDashboard = lazy(() => import("modules/request/Dashboard"))
const DelegationConfig = lazy(() => import("modules/request/DelegationConfig"))
const ExistingBooking = lazy(() => import("modules/request/ExistingBookings"))
const ExistingBookingDetail = lazy(() => import("modules/request/ExistingBookings/Detail"))
const HierarchicalUser = lazy(() => import("modules/request/HierarchicalUser"))
const ImpersonateUser = lazy(() => import("modules/request/ImpersonateUser"))
const ImpersonateUserTasks = lazy(() => import("modules/request/ImpersonateUser/ImpersonateUserTasks"))
const RequestShiftVisual = lazy(() => import("modules/request/RequestShiftVisual"))
const RequestShiftVisualDetail = lazy(() => import("modules/request/RequestShiftVisualDetail"))
const Task = lazy(() => import("modules/request/Task"))
// Create Document
const CreateDeMobilisation = lazy(() => import("modules/request/CreateDocument/De-Mobilisation"))
const CreateNonSiteTravel = lazy(() => import("modules/request/CreateDocument/NonSiteTravel"))
const CreateSAMProfileChanges = lazy(() => import("modules/request/CreateDocument/SAMProfileChanges"))
const CreateSiteTravel = lazy(() => import("modules/request/CreateDocument/SiteTravel"))
const PeopleListSiteTravel = lazy(() => import("modules/request/CreateDocument/SiteTravel/PeopleList"))
const PeopleList = lazy(() => import("modules/request/CreateDocument/indexPage"))
// Document
const DeMobilisation = lazy(() => import("modules/request/Documents/De-Mobilisation"))
const ExternalTravel = lazy(() => import("modules/request/Documents/ExternalTravel"))
const NonSiteTravel = lazy(() => import("modules/request/Documents/NonSiteTravelOTLLC"))
const SAMProfileChanges = lazy(() => import("modules/request/Documents/SAMProfileChanges"))
const SiteTravelOTLLC = lazy(() => import("modules/request/Documents/SiteTravelOTLLC"))
// Report module
const ReportDashboard = lazy(() => import("modules/report/Dashboard"))
const Scheduler = lazy(() => import("modules/report/Scheduler"))
const Template = lazy(() => import("modules/report/Template"))
// Guest module
const GuestHome = lazy(() => import("modules/guest/GuestHome"))
const GuestProfile = lazy(() => import("modules/guest/GuestProfile"))
const Requests = lazy(() => import("modules/guest/Requests"))
// Guest Create Document
const GuestCreateDeMobilisation = lazy(() => import("modules/guest/CreateDocument/De-Mobilisation"))
const GuestCreateNonSiteTravel = lazy(() => import("modules/guest/CreateDocument/NonSiteTravel"))
const GuestCreateSAMProfileChanges = lazy(() => import("modules/guest/CreateDocument/SAMProfileChanges"))
const GuestCreateSiteTravel = lazy(() => import("modules/guest/CreateDocument/SiteTravel"))
// Guest Create Document
const DeMobilisationGuest = lazy(() => import("modules/guest/Documents/De-Mobilisation"))
const NonSiteTravelGuest = lazy(() => import("modules/guest/Documents/NonSiteTravelOTLLC"))
const ProfileChangesGuest = lazy(() => import("modules/guest/Documents/SAMProfileChanges"))
const SiteTravelGuest = lazy(() => import("modules/guest/Documents/SiteTravelOTLLC"))

const routes = [
  {
    name: '',
    path: '/',
    element: <Navigate to='/tas'/>,
    errorElement: <ErrorPage/>,
  },
  {
    name: 'Guest',
    path: '/guest',
    errorElement: <ErrorPage/>,
    element: <GuestProfile/>,
    children: [
      {
        index: true,
        element: <GuestProtectedRoute><GuestHome/></GuestProtectedRoute>,
      },
      {
        name: 'Non site travel',
        path: '/guest/nonsitetravel/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/guest/nonsitetravel/${e.empId}`}>Create a request</Link>
        },
        element: <GuestProtectedRoute routeName='NonSiteTravel'><GuestCreateNonSiteTravel/></GuestProtectedRoute>,
      },
      {
        name: 'TAS Profile Changes Request Create',
        path: '/guest/samprofilechanges/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/guest/samprofilechanges/${e.empId}`}>Create a request</Link>
        },
        element: <GuestProtectedRoute routeName='TASProfileChanges'><GuestCreateSAMProfileChanges/></GuestProtectedRoute>,
      },
      {
        name: 'Site Travel Request Create',
        path: '/guest/sitetravel/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/guest/sitetravel/${e.empId}`}>Create a request</Link>
        },
        element: <GuestProtectedRoute routeName='SiteTravel'><GuestCreateSiteTravel/></GuestProtectedRoute>,
      },
      {
        name: 'De-Mobilisation Request Create',
        path: '/guest/de-mobilisation/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/guest/de-mobilisation/${e.empId}`}>Create a request</Link>
        },
        element: <GuestProtectedRoute routeName='De-Mobilisation'><GuestCreateDeMobilisation/></GuestProtectedRoute>,
      },
      {
        name: 'Task',
        path: '/guest/request',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/guest/request`}>Requests</Link>
        },
        children: [
          {
            index: true,
            element: <GuestProtectedRoute routeName='Request'><Requests/></GuestProtectedRoute>,
          },
          {
            name: 'Non Site Travel',
            path: '/guest/request/nonsitetravel/:documentId',
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/guest/request/nonsitetravel/${e.documentId}`}>Non Site Travel Request</Link>
            },
            element: <GuestProtectedRoute routeName='Task'><NonSiteTravelGuest/></GuestProtectedRoute>,
          },
          {
            name: 'Profile Changes',
            path: '/guest/request/profilechanges/:documentId',
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/guest/request/profilechanges/${e.documentId}`}>Profile Changes</Link>
            },
            element: <GuestProtectedRoute routeName='Task'><ProfileChangesGuest/></GuestProtectedRoute>,
          },
          {
            name: 'De-Mobilisation',
            path: '/guest/request/de-mobilisation/:documentId',
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/guest/request/de-mobilisation/${e.documentId}`}>De-Mobilisation</Link>
            },
            element: <GuestProtectedRoute routeName='Task'><DeMobilisationGuest/></GuestProtectedRoute>,
          },
          {
            name: 'Site Travel',
            path: '/guest/request/sitetravel/:documentTag/:documentId',
            errorElement: <LocalError listUrl={'/guest/request'}/>,
            loader: async ({params}) => {
              return axios.get(`tas/requestsitetravel/${params.documentTag}/${params.documentId}`).then((res) => {
                return {
                  data: res.data,
                  documentTag: params.documentTag,
                  documentId: params.documentId
                }
              }).catch((err) => {
                if(err.response.status === 499){
                  throw new Response("Not Found Request", { status: err.response.status })
                }else{
                  throw new Response("Something went wrong", { status: err.response.status })
                }
              })
            },
            handle: {
              crumb: (e) => e ? <Link to={`/guest/request/sitetravel/${e.documentId}`}>Site Travel</Link> : 'Error'
            },
            element: <GuestProtectedRoute routeName='Task'><SiteTravelGuest/></GuestProtectedRoute>,
          },
        ]
      },
    ]
  },
  {
    name: 'Guest',
    path: '/supervisor',
    errorElement: <ErrorPage/>,
    element: <ProtectedSuspense routeName='Supervisor'><Supervisor/></ProtectedSuspense>,
  },
  ///////////////////////////////////                TAS                 ///////////////////////
  {
    name: 'Tas',
    path: '/tas',
    errorElement: <ErrorPage/>,
    handle: {
      crumb: () => <Link to={'/tas'}>Home</Link>
    },
    children:[
      {
        index: true,
        element: <ProtectedSuspense routeName='DashboardTAS'><Dashboard/></ProtectedSuspense>,
      },
      {
        name: 'By Room',
        path: '/tas/byroom',
        handle: {
          crumb: () => <Link to={'/tas/byroom'}>By Room</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='ByRoom'><ByRoom/></ProtectedSuspense>,
          },
          {
            name: 'Employee Detail',
            path: '/tas/byroom/:employeeId',
            element: <ProtectedSuspense routeName='ByRoom'><RoomAssignLayout/></ProtectedSuspense>,
            loader: async ({params}) => {
              return axios.get( `tas/employee/${params.employeeId}`).then((res) => {
                return res.data
              })
            },
            handle: {
              crumb: (e) => <Link to={`/tas/byroom/${e.Id}`}>{e.Firstname} {e.Lastname}</Link>
            },
            children: [
              {
                index: true,
                element: <ProtectedSuspense routeName='ByRoom'><Navigate to={'roombooking'}/></ProtectedSuspense>,
              },
              // {
              //   index: true,
              //   element: <ProtectedSuspense routeName='RoomAssign'><RoomAssignDetail/></ProtectedSuspense>,
              //   caseSensitive: true,
              // },
              {
                name: 'Room Booking',
                path: '/tas/byroom/:employeeId/roombooking', 
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/byroom/${e.employeeId}/roombooking`}>Room Booking</Link>
                },
                element: <ProtectedSuspense routeName='ByRoom'><RoomAssignDetail/></ProtectedSuspense>
              },
              {
                name: 'Room Audit',
                path: '/tas/byroom/:employeeId/audit', 
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/byroom/${e.employeeId}/audit`}>Profile Audit</Link>
                },
                element: <ProtectedSuspense routeName='ByRoom'><Audit/></ProtectedSuspense>
              },
              {
                name: 'Profile By Date',
                path: '/tas/byroom/:employeeId/profilebydate',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/byroom/${e.employeeId}/profilebydate`}>Profile By Date</Link>
                },
                element: <ProtectedSuspense routeName='ByRoom'><ProfileByDate/></ProtectedSuspense>
              },
              {
                name: 'Profile Info',
                path: '/tas/byroom/:employeeId/information',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/byroom/${e.employeeId}/information`}>Profile Info</Link>
                },
                element: <ProtectedSuspense routeName='ByRoom'><MoreData/></ProtectedSuspense>
              },
            ]
          },
        ]
      },
      {
        name: 'Room Calendar',
        path: '/tas/roomcalendar',
        handle: {
          crumb: () => <Link to={'/tas/roomcalendar'}>Room Calendar</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='RoomCalendar'><RoomCalendar/></ProtectedSuspense>,
          },
          {
            name: 'Room Calendar Detail',
            path: '/tas/roomcalendar/:roomId/:startDate',
            element: <ProtectedSuspense routeName='RoomCalendar'><RoomCalendarDetail/></ProtectedSuspense>,
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/room/${e.roomId}/${e.startDate}`}>Room Calendar Detail</Link>
            }
          },
        ]
      },
      {
        name: 'Room Assign',
        path: '/tas/roomassign',
        handle: {
          crumb: () => <Link to={'/tas/roomassign'}>Room Assign</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='RoomAssign'><RoomAssign/></ProtectedSuspense>,
          },
          {
            name: 'Room Assign Detail',
            path: '/tas/roomassign/:employeeId',
            element: <ProtectedSuspense routeName='RoomAssign'><RoomAssignLayout/></ProtectedSuspense>,
            id: 'assignRoomLayout',
            loader: async ({params}) => {
              return axios.get( `tas/employee/${params.employeeId}`).then((res) => {
                return res.data
              })
            },
            handle: {
              crumb: (e) => <Link to={`/tas/roomassign/${e.Id}`}>{e.Firstname} {e.Lastname}</Link>
            },
            children: [
              {
                index: true,
                element: <ProtectedSuspense routeName='RoomAssign'><Navigate to={'roombooking'}/></ProtectedSuspense>,
              },
              {
                name: 'Room Booking',
                path: '/tas/roomassign/:employeeId/roombooking', 
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/roomassign/${e.employeeId}/roombooking`}>Room Booking</Link>
                },
                element: <ProtectedSuspense routeName='RoomAssign'><RoomAssignDetail/></ProtectedSuspense>
              },
              {
                name: 'Room Audit',
                path: '/tas/roomassign/:employeeId/audit', 
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/roomassign/${e.employeeId}/audit`}>Profile Audit</Link>
                },
                element: <ProtectedSuspense routeName='RoomAssign'><Audit/></ProtectedSuspense>
              },
              {
                name: 'Profile By Date',
                path: '/tas/roomassign/:employeeId/profilebydate',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/roomassign/${e.employeeId}/profilebydate`}>Profile By Date</Link>
                },
                element: <ProtectedSuspense routeName='RoomAssign'><ProfileByDate/></ProtectedSuspense>
              },
              {
                name: 'Profile Info',
                path: '/tas/roomassign/:employeeId/information',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/roomassign/${e.employeeId}/information`}>Profile Info</Link>
                },
                element: <ProtectedSuspense routeName='RoomAssign'><MoreData/></ProtectedSuspense>
              },
            ]
          },
        ]
      },
      {
        name: 'Room Audit',
        path: '/tas/roomaudit',
        handle: {
          crumb: () => <Link to={'/tas/roomaudit'}>Room Audit</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='RoomAudit'><RoomAudit /></ProtectedSuspense>,
          },
        ]
      },
      // {
      //   name: 'By Person',
      //   path: '/tas/byperson',
      //   element: <ProtectedSuspense routeName='By Person'><ByPerson/></ProtectedSuspense>,
      //   handle: {
      //     crumb: () => <Link to={'/tas/byperson'}>By Person</Link>
      //   }
      // },
    
      /////////////     People    ///////////
    
      {
        name: 'Search',
        path: '/tas/people/search',
        routeName: 'People',
        handle: {
          crumb: () => <Link to={'/tas/people/search'}>Search</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='Peoplesearch'><Search/></ProtectedSuspense>,
          },
          {
            name: 'Profile',
            path: '/tas/people/search/:employeeId',
            element: <ProtectedSuspense routeName='Peoplesearch'><Profile/></ProtectedSuspense>,
            errorElement: <LocalError listUrl={'/tas/people/search'}/>,
            loader: async ({params}) => {
              return axios(`tas/employee/${params.employeeId}`).then((res) => {
                console.log('get profile');
                return {
                  data: res.data,
                  employeeId: params.employeeId
                }
              }).catch((err) => {
                if(err.response.status === 499){
                  throw new Response("Not Found Employee", { status: err.response.status })
                }else{
                  throw new Response("Something went wrong", { status: err.response.status })
                }
              })
            },
            handle: {
              crumb: (e) => (
                e ? 
                <Link to={`/tas/people/search/${e.employeeId}`} onClick={() => ls.set('pp_rt', '')}>{e.data.Firstname} {e.data.Lastname}</Link>
                : <div>Not Found</div>
              )
            },
            children: [
              {
                index: true,
                element: <ProtectedSuspense routeName='Peoplesearch'><MoreData/></ProtectedSuspense>
              },
              {
                name: 'Flight',
                path: '/tas/people/search/:employeeId/flight', 
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/flight`}>Flight</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><Flight/></ProtectedSuspense>
              },
              {
                name: 'Room Booking',
                path: '/tas/people/search/:employeeId/roombooking',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/roombooking`}>Room Booking</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><RoomBooking/></ProtectedSuspense>
              },
              {
                name: 'Roster Detail',
                path: '/tas/people/search/:employeeId/rosterexecute',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/rosterexecute`}>Roster Execute</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><RosterDetail/></ProtectedSuspense>
              },
              {
                name: 'Shift Visual',
                path: '/tas/people/search/:employeeId/shiftvisual',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/shiftvisual`}>Shift Visual</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><ShiftVisual/></ProtectedSuspense>
              },
              {
                name: 'No Show',
                path: '/tas/people/search/:employeeId/noshow',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/noshow`}>No Show</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><NoShowAndGoShow/></ProtectedSuspense>
              },
              {
                name: 'Account history',
                path: '/tas/people/search/:employeeId/history',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/history`}>Account history</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><AccountHistory/></ProtectedSuspense>
              },
              {
                name: 'Profile Audit',
                path: '/tas/people/search/:employeeId/audit',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/history`}>Profile Audit</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><Audit/></ProtectedSuspense>
              },
              {
                name: 'Profile By Date',
                path: '/tas/people/search/:employeeId/profilebydate',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/profilebydate`}>Profile By Date</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><ProfileByDate/></ProtectedSuspense>
              },
              {
                name: 'Role Center',
                path: '/tas/people/search/:employeeId/rolecenter',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/${e.employeeId}/rolecenter`}>Role Center</Link>
                },
                element: <ProtectedSuspense routeName='Peoplesearch'><RoleCenter/></ProtectedSuspense>
              },
            ]
          },
          //////////////////  SAFE MODE   ////////////////////
          {
            name: 'Profile',
            path: '/tas/people/search/sm/:employeeId',
            element: <SafeModeProtect routeName='Peoplesearch'><SafeProfile/></SafeModeProtect>,
            errorElement: <LocalError listUrl={'/tas/people/search'}/>,
            loader: async ({params}) => {
              return axios(`tas/employee/${params.employeeId}`).then((res) => {
                return {
                  data: res.data,
                  employeeId: params.employeeId
                }
              }).catch((err) => {
                if(err.response.status === 499){
                  throw new Response("Not Found Employee", { status: err.response.status })
                }else{
                  throw new Response("Something went wrong", { status: err.response.status })
                }
              })
            },
            handle: {
              crumb: (e) => (
                e ? 
                <Link to={`/tas/people/search/sm/${e.employeeId}`}>{e.data.Firstname} {e.data.Lastname}</Link>
                : <div>Not Found</div>
              )
            },
            children: [
              {
                index: true,
                element: <SafeModeProtect routeName='Peoplesearch'><SafeFlight/></SafeModeProtect>
              },
              {
                name: 'Room Booking',
                path: '/tas/people/search/sm/:employeeId/roombooking',
                loader: async ({params}) => {
                  return params
                },
                handle: {
                  crumb: (e) => <Link to={`/tas/people/search/sm/${e.employeeId}/roombooking`}>Room Booking</Link>
                },
                element: <SafeModeProtect routeName='Peoplesearch'><SafeRoomBooking/></SafeModeProtect>
              },
            ]
          },
        ]
      },
      {
        routeName: 'Peoplesearch',
        path: '/tas/bookinginfo',
        // element: <ProtectedSuspense routeName='EmployerChanges'><ChangesEmployer/></ProtectedSuspense>,
        element: <ProtectedSuspense routeName='Peoplesearch'><BookingInfo/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bookinginfo'}>Booking Info</Link>
        }
      },
    
      ///// People -> Changes ////////
      {
        routeName: 'Changes Employer',
        path: '/tas/changesemployer',
        // element: <ProtectedSuspense routeName='EmployerChanges'><ChangesEmployer/></ProtectedSuspense>,
        element: <ProtectedSuspense routeName='EmployerChanges'><ChangesEmployer/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/changesemployer'}>Change Employer</Link>
        }
      },
      {
        routeName: 'Changes Cost Code',
        path: '/tas/changescostcode',
        element: <ProtectedSuspense routeName='CostCodeChanges'><ChangesCostCode/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/changescostcode'}>Change Cost Code</Link>
        }
      },
      {
        routeName: 'Changes Department', 
        path: '/tas/changesdepartment',
        element: <ProtectedSuspense routeName='DepartmentChanges'><ChangesDepartment/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/changesdepartment'}>Change Department</Link>
        }
      },
      {
        routeName: 'Changes Position', 
        path: '/tas/changesposition',
        element: <ProtectedSuspense routeName='Position'><ChangesPosition/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/changesposition'}>Change Position</Link>
        }
      },
      {
        routeName: 'Changes Group',
        path: '/tas/changesgroup',
        element: <ProtectedSuspense routeName='Group'><ChangesGroup/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/changesgroup'}>Change Group</Link>
        }
      },
      {
        routeName: 'Changes Location', 
        path: '/tas/changeslocation',
        element: <ProtectedSuspense routeName='LocationChanges'><ChangesLocation/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/changeslocation'}>Changes Location</Link>
        }
      },
      {
        name: 'Profile Deactivate', 
        path: '/tas/profiledeactivate',
        handle: {
          crumb: () => <Link to={'/tas/profiledeactivate'}>Profile Deactivate</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='ProfileDeactivate'><ProfileDeactivate/></ProtectedSuspense>,
          },
          {
            name: 'Pending Deactivate',
            path: '/tas/profiledeactivate/pendingdeactivate',
            element: <ProtectedSuspense routeName='ProfileDeactivate'><PendingDeactivate/></ProtectedSuspense>,
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: () => <Link to={'/tas/profiledeactivate/pendingdeactivate'}>Pending Deactivate</Link>
            }
          },
        ]
      },
      {
        name: 'Profile Reactivate',
        path: '/tas/profilereactivate',
        handle: {
          crumb: () => <Link to={'/tas/profilereactivate'}>Profile Reactivate</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='ProfileReactivate'><ProfileReactivate/></ProtectedSuspense>,
          },
          {
            name: 'Pending Reactivate',
            path: '/tas/profilereactivate/pendingreactivate',
            element: <ProtectedSuspense routeName='ProfileReactivate'><PendingReactivate/></ProtectedSuspense>,
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: () => <Link to={'/tas/profilereactivate/pendingreactivate'}>Pending Reactivate</Link>
            }
          },
        ]
      },
      
      //////////////////////////     Transport    /////////////////////////////
      
      {
        name: 'Bulk Roster Execute',
        path: '/tas/bulkrosterexecute',
        element: <ProtectedSuspense routeName='BulkRosterExecute'><BulkRosterExecute/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkrosterexecute'}>Bulk Roster Execute</Link>
        }
      },
      {
        routeName: 'Roster Booking',
        path: '/tas/rosterbooking',
        element: <ProtectedSuspense routeName='RosterBooking'><RosterBooking/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/rosterbooking'}>Roster Booking</Link>
        },
      },
      {
        routeName: 'Manage Schedule',
        path: '/tas/manageschedule',
        element: <ProtectedSuspense routeName='ManageSchedule'><ManageSchedule/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/manageschedule'}>Manage Schedule</Link>
        }
      },
      {
        name: 'Active Transport',
        path: '/tas/activetransport',
        handle: {
          crumb: () => <Link to={'/tas/activetransport'}>Active Transport</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='ActiveTransport'><TransportActive/></ProtectedSuspense>,
          },
          {
            name: 'Transport Schedule',
            path: '/tas/activetransport/:transportId',
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/activetransport/${e.transportId}`}>Transport Schedule</Link>
            },
            element: <ProtectedSuspense routeName='ActiveTransport'><TransportSchedule/></ProtectedSuspense>,
          },
        ]
      },
      {
        routeName: 'Cluster',
        path: '/tas/cluster',
        handle: {
          crumb: () => <Link to={'/tas/cluster'}>Cluster</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='Cluster'><Cluster/></ProtectedSuspense>,
          },
          {
            name: 'Cluster Detail',
            path: '/tas/cluster/:clusterId',
            element: <ProtectedSuspense routeName='Cluster'><ClusterDetail/></ProtectedSuspense>,
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/cluster/${e.clusterId}`}>Cluster Detail</Link>
            },
          }
        ]
      },
      {
        name: 'Transport Group',
        path: '/tas/transportgroup',
        handle: {
          crumb: () => <Link to={'/tas/transportgroup'}>Transport Group</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='TransportGroups'><TransportGroup/></ProtectedSuspense>,
          },
          {
            name: 'Transport Group Detail',
            path: '/tas/transportgroup/:groupId',
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/transportgroup/${e.groupId}`}>Transport Group Detail</Link>
            },
            element: <ProtectedSuspense routeName='TransportGroups'><TransportGroupDetail/></ProtectedSuspense>,
          },
        ]
      },
      {
        name: 'Seat Block',
        path: '/tas/seatblock',
        handle: {
          crumb: () => <Link to={'/tas/seatblock'}>Seat Block</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='SeatBlock'><SeatBlock/></ProtectedSuspense>,
          },
          {
            name: 'Seat Block Detail',
            path: '/tas/seatblock/:eventId',
            element: <ProtectedSuspense routeName='SeatBlock'><SeatBlockDetail/></ProtectedSuspense>,
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/seatblock/${e.eventId}`}>Seat Block Detail</Link>
            },
          }
        ]
      },
      {
        name: 'MultipleBooking',
        path: '/tas/multiplebooking',
        element: <ProtectedSuspense routeName='MultipleBooking'><MultipleBooking/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/multiplebooking'}>Multiple Booking</Link>
        }
      },
      {
        name: 'Reschedule Multiple',
        path: '/tas/reschedule',
        element: <ProtectedSuspense routeName='RescheduleMultiple'><RescheduleMultiple/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/reschedule'}>Reschedule Multiple</Link>
        }
      },
      {
        name: 'Bus Stop Schdeule',
        path: '/tas/busstop',
        element: <ProtectedSuspense routeName='BusStopSchedule'><BusTimetable/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/busstop'}>Bus Stop Schdeule</Link>
        }
      },
      {
        name: 'Transport Audit', 
        path: '/tas/transportaudit',
        handle: {
          crumb: () => <Link to={'/tas/transportaudit'}>Transport Audit</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='TransportAudit'><TransportAudit /></ProtectedSuspense>,
          },
          {
            name: 'Transport Audit Detail',
            path: '/tas/transportaudit/:empId',
            element: <ProtectedSuspense routeName='TransportAudit'><TransportAuditDetail/></ProtectedSuspense>,
            loader: async ({params}) => {
              return axios.get( `tas/employee/${params.empId}`).then((res) => {
                return res.data
              })
            },
            handle: {
              crumb: (e) => <Link to={`/tas/transportaudit/${e.Id}`}>{e.Firstname} {e.Lastname}</Link>
            },
          },
        ]
      },
    
      ///////          Master           ////////
    
      {
        name: 'Cost Code', 
        path: '/tas/costcode',
        element: <ProtectedSuspense routeName='CostCodeMaster'><CostCode/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/costcode'}>Cost Code</Link>
        }
      },
      {
        name: 'Employer', 
        path: '/tas/employer',
        element: <ProtectedSuspense routeName='Employer'><Employer/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/employer'}>Employer</Link>
        }
      },
      {
        routeName: 'Department', 
        path: '/tas/department',
        handle: {
          crumb: () => <Link to={'/tas/department'}>Department</Link>
        },
        children : [
          {
            index: true,
            element: <ProtectedSuspense routeName='Department'><Department/></ProtectedSuspense>,
          },
          {
            name: 'Department Detail',
            path: '/tas/department/:departmentId',
            element: <ProtectedSuspense routeName='Department'><DepartmentDetail/></ProtectedSuspense>,
            errorElement: <LocalError listUrl={'/tas/department'}/>,
            loader: async ({params}) => {
              return axios.get( `tas/department/${params.departmentId}`).then((res) => {
                return res.data
              }).catch((err) => {
                if(err.response.status === 499){
                  throw new Response("Not Found Department", { status: err.response.status })
                }else{
                  throw new Response("Something went wrong", { status: err.response.status })
                }
              })
            },
            handle: {
              crumb: (e) => e ? <Link to={`/tas/department/`}>Department Detail</Link> : <div>Error</div>
            },
          },
        ]
      },
      {
        name: 'Position', 
        path: '/tas/position',
        element: <ProtectedSuspense routeName='PositionMaster'><Position/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/position'}>Position</Link>
        }
      },
      {
        routeName: 'Group Master',
        path: '/tas/groupmaster',
        handle: {
          crumb: () => <Link to={'/tas/groupmaster'}>Group Master</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='GroupMaster'><GroupMaster/></ProtectedSuspense>,
          },
          {
            name: 'Group Master Detail',
            path: '/tas/groupmaster/:groupMasterId',
            element: <ProtectedSuspense routeName='GroupMaster'><GroupDetail/></ProtectedSuspense>,
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/groupmaster/${e.groupMasterId}`}>Group Master Detail</Link>
            }
          },
        ]
      },
      {
        routeName: 'People Type',
        path: '/tas/peopletype',
        element: <ProtectedSuspense routeName='PeopleType'><PeopleType/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/peopletype'}>PeopleType</Link>
        }
      },
      {
        name: 'Location',
        path: '/tas/location',
        element: <ProtectedSuspense routeName='LocationMaster'><Location/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/location'}>Location</Link>
        }
      },
      // {
      //   name: 'Termination Type',
      //   path: '/tas/terminationtype',
      //   element: <ProtectedSuspense routeName='TerminationType'><TerminationType/></ProtectedSuspense>,
      //   handle: {
      //     crumb: () => <Link to={'/tas/terminationtype'}>Termination Type</Link>
      //   }
      // },
      {
        name: 'Nationality',
        path: '/tas/nationality',
        element: <ProtectedSuspense routeName='Nationality'><Nationality/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/nationality'}>Nationality</Link>
        }
      },
      {
        name: 'Airport Code',
        path: '/tas/airportcode',
        element: <ProtectedSuspense routeName='AirportCode'><AirportCode/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/airportcode'}>Airport Code</Link>
        }
      },
      {
        name: 'Travel Agent',
        path: '/tas/travelagent',
        element: <ProtectedSuspense routeName='TravelAgent'><TravelAgent/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/travelagent'}>Travel Agent</Link>
        }
      },
      {
        routeName: 'Travel Purpose',
        path: '/tas/travelpurpose',
        element: <ProtectedSuspense routeName='TravelPurpose'><TravelPurpose/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/travelpurpose'}>Travel Purpose</Link>
        }
      },
      {
        routeName: 'Request Hotel',
        path: '/tas/requesthotel',
        element: <ProtectedSuspense routeName='RequestHotel'><RequestHotel/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/requesthotel'}>Request Hotel</Link>
        }
      },
      {
        routeName: 'De-Mobilisation Type',
        path: '/tas/demobilisationtype',
        element: <ProtectedSuspense routeName='De-MobilisationType'><DeMobilisationType/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/demobilisationtype'}>De-Mobilisation Type</Link>
        }
      },
      {
        name: 'Aimag',
        path: '/tas/aimag',
        element: <ProtectedSuspense routeName='Aimag'><Aimag/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/aimag'}>Aimag</Link>
        }
      },
      {
        routeName: 'Profile Field',
        path: '/tas/profilefield',
        element: <ProtectedSuspense routeName='ProfileField'><ProfileField/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/profilefield'}>Profile Field</Link>
        }
      },
      {
        routeName: 'Shift Status Color List',
        path: '/tas/shiftstatuscolor',
        element: <ProtectedSuspense routeName='ColorList'><ShiftStatusColor/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/shiftstatuscolor'}>Shift Status Color</Link>
        }
      },
      {
        routeName: 'Shift Status List',
        path: '/tas/shiftstatus',
        element: <ProtectedSuspense routeName='ShiftStatusList'><ShiftStatus/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/shiftstatus'}>Shift Status</Link>
        }
      },
      {
        routeName: 'Roster',
        path: '/tas/roster',
        handle: {
          crumb: () => <Link to={'/tas/roster'}>Roster</Link>
        },
        children: [
          {
            name: 'Roster List',
            index: true,
            element: <ProtectedSuspense routeName='RosterList'><Roster/></ProtectedSuspense>
          },
          {
            name: 'Roster Master',
            path: '/tas/roster/:rosterId',
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/roster/${e.rosterId}`}>Roster Master</Link>
            },
            element: <ProtectedSuspense routeName='RosterList'><RosterMaster/></ProtectedSuspense>
          }
        ]
      },
      {
        name: 'Transport Mode',
        path: '/tas/transportmode',
        element: <ProtectedSuspense routeName='TransportMode'><TransportMode/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/transportmode'}>Transport Mode</Link>
        }
      },
      {
        name: 'Carrier',
        path: '/tas/carrier',
        element: <ProtectedSuspense routeName='Carrier'><Carrier/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/carrier'}>Carrier</Link>
        }
      },
      {
        name: 'Camp',
        path: '/tas/camp',
        element: <ProtectedSuspense routeName='Camp'><Camp/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/camp'}>Camp</Link>
        }
      },
      {
        name: 'Room Type Group',
        path: '/tas/roomtypegroup',
        element: <ProtectedSuspense routeName='RoomTypeGroup'><RoomTypeGroup/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/roomtypegroup'}>Room Type Group</Link>
        }
      },
      {
        name: 'Room',
        path: '/tas/room',
        handle: {
          crumb: () => <Link to={'/tas/room'}>Room</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='RoomRoom'><Room/></ProtectedSuspense>,
          },
          {
            name: 'Bed',
            path: '/tas/room/:roomId',
            element: <ProtectedSuspense routeName='RoomRoom'><Bed/></ProtectedSuspense>,
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/room/${e.roomId}`}>Bed</Link>
            }
          },
        ]
      },
      {
        name: 'Roster Group',
        path: '/tas/rostergroup',
        element: <ProtectedSuspense routeName='RosterGroup'><RosterGroup/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/rostergroup'}>Roster Group</Link>
        }
      },
    
      /////////////////     Bulk    ////////////////
    
      {
        name: 'Profile',
        path: '/tas/bulkprofile',
        element: <ProtectedSuspense routeName='Profile'><BulkProfile/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkprofile'}>Profile</Link>
        }
      },
      {
        name: 'Bulk Department',
        path: '/tas/bulkdepartment',
        element: <ProtectedSuspense routeName='DepartmentBulk'><BulkDepartment/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkdepartment'}>Bulk Department</Link>
        }
      },
      {
        name: 'Bulk Cost Code',
        path: '/tas/bulkcostcode',
        element: <ProtectedSuspense routeName='CostCodeBulk'><BulkCostCode/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkcostcode'}>Bulk Cost Code</Link>
        }
      },
      // {
      //   name: 'Bulk Group',
      //   path: '/tas/bulkgroup',
      //   element: <ProtectedSuspense routeName='GroupBulk'><BulkGroup/></ProtectedSuspense>,
      //   handle: {
      //     crumb: () => <Link to={'/tas/bulkgroup'}>Bulk Group</Link>
      //   }
      // },
      {
        name: 'Bulk Room',
        path: '/tas/bulkroom',
        element: <ProtectedSuspense routeName='RoomBulk'><BulkRoom/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkroom'}>Bulk Room</Link>
        }
      },
      {
        name: 'Bulk Position',
        path: '/tas/bulkposition',
        element: <ProtectedSuspense routeName='PositionBulk'><BulkPosition/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkposition'}>Bulk Position</Link>
        }
      },
      {
        name: 'Bulk Employer',
        path: '/tas/bulkemployer',
        element: <ProtectedSuspense routeName='EmployerBulk'><BulkEmployer/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkemployer'}>Bulk Employer</Link>
        }
      },
      {
        name: 'Bulk Shift Status',
        path: '/tas/bulkshiftstatus',
        element: <ProtectedSuspense routeName='ShiftStatusBulk'><BulkShiftStatus/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/bulkshiftstatus'}>Bulk Shift Status</Link>
        }
      },
    
      /////////////////     Users    ////////////////
    
      {
        name: 'Roles',
        path: '/tas/roles',
        handle: {
          crumb: () => <Link to={'/tas/roles'}>Roles</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='Roles'><Roles/></ProtectedSuspense>,
          },
          {
            name: 'Role Detail',
            path: '/tas/roles/:roleId',
            loader: async ({params}) => {
              return axios.get(`tas/sysrole/${params.roleId}`).then((res) => {
                return {
                  loadedData: res.data,
                  documentTag: params.documentTag,
                  roleId: params.roleId
                }
              })
            },
            handle: {
              crumb: (e) => <Link to={`/tas/roles/${e.roleId}`}>{e.loadedData?.Name ? e.loadedData?.Name : 'Not Found'}</Link>
            },
            element: <ProtectedSuspense routeName='Roles'><RoleDetail/></ProtectedSuspense>,
          },
        ]
      },
      {
        name: 'Approval Configuration',
        path: '/tas/approvalconfig',
        element: <ProtectedSuspense routeName='ApprovalConfiguration'><ApprovalConfiguration/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/approvalconfig'}>Approval Configuration</Link>
        }
      },
      {
        name: 'Approval Groups',
        path: '/tas/approvalgroups',
        handle: {
          crumb: () => <Link to={'/tas/approvalgroups'}>Approval Groups</Link>
        },
        children: [
          {
            index: true,
            element: <ProtectedSuspense routeName='ApprovalGroups'><ApprovalGroups/></ProtectedSuspense>,
          },
          {
            name: 'Approval Groups Detail',
            path: '/tas/approvalgroups/:groupId',
            loader: async ({params}) => {
              return params
            },
            handle: {
              crumb: (e) => <Link to={`/tas/approvalgroups/${e.roleId}`}>Approval Groups Detail</Link>
            },
            element: <ProtectedSuspense routeName='ApprovalGroups'><ApprovalGroupDetail/></ProtectedSuspense>,
          },
        ]
      },
      {
        name: 'SMTP Configuration',
        path: '/tas/smtpconfig',
        element: <ProtectedSuspense routeName='SMTPConfiguration'><SMTPConfig/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/smtpconfig'}>SMTP Configuration</Link>
        }
      },
      {
        name: 'Notifications',
        path: '/tas/notifications',
        element: <ProtectedSuspense><Notifications/></ProtectedSuspense>
      },
      {
        name: 'PrivacyPolicy',
        path: '/tas/privacypolicy',
        element: <ProtectedSuspense routeName='PrivacyPolicy'><PrivacyPolicy/></ProtectedSuspense>,
        handle: {
          crumb: () => <Link to={'/tas/privacypolicy'}>Privacy Policy</Link>
        }
      },
    ]
  },

  ///////////////////////////////////                REQUEST                 ///////////////////////
  
  {
    name: 'Request',
    path: '/request',
    errorElement: <ErrorPage/>,
    element: <ProtectedSuspense routeName='DashboardRequest'><RequestDashboard/></ProtectedSuspense>,
    handle: {
      crumb: () => <Link to={'/tas/request'}>Dashboard</Link>
    }
  },
  {
    name: 'Task',
    path: '/request/task',
    handle: {
      crumb: (e) => <Link to={`/request/task`}>Task</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='Task'><Task/></ProtectedSuspense>,
      },
      {
        name: 'Non Site Travel',
        path: '/request/task/nonsitetravel/:documentId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/task/nonsitetravel/${e.documentId}`}>Non Site Travel Request</Link>
        },
        element: <ProtectedSuspense routeName='Task'><NonSiteTravel/></ProtectedSuspense>,
      },
      {
        name: 'Profile Changes',
        path: '/request/task/profilechanges/:documentId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/task/profilechanges/${e.documentId}`}>Profile Changes</Link>
        },
        element: <ProtectedSuspense routeName='Task'><SAMProfileChanges/></ProtectedSuspense>,
      },
      {
        name: 'De-Mobilisation',
        path: '/request/task/de-mobilisation/:documentId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/task/de-mobilisation/${e.documentId}`}>De-Mobilisation</Link>
        },
        element: <ProtectedSuspense routeName='Task'><DeMobilisation/></ProtectedSuspense>,
      },
      {
        name: 'Site Travel',
        path: '/request/task/sitetravel/:documentTag/:documentId',
        errorElement: <LocalError listUrl={'/request/task'}/>,
        loader: async ({params}) => {
          return axios.get(`tas/requestsitetravel/${params.documentTag}/${params.documentId}`).then((res) => {
            return {
              data: res.data,
              documentTag: params.documentTag,
              documentId: params.documentId
            }
          }).catch((err) => {
            if(err.response.status === 499){
              throw new Response("Not Found Request", { status: err.response.status })
            }else{
              throw new Response("Something went wrong", { status: err.response.status })
            }
          })
        },
        handle: {
          crumb: (e) => e ? <Link to={`/request/task/sitetravel/${e.documentId}`}>Site Travel</Link> : 'Error'
        },
        element: <ProtectedSuspense routeName='Task'><SiteTravelOTLLC/></ProtectedSuspense>,
      },
      {
        name: 'External Travel',
        path: '/request/task/externaltravel/:documentTag/:documentId',
        errorElement: <LocalError listUrl={'/request/task'}/>,
        loader: async ({params}) => {
          return axios.get(`tas/requestexternaltravel/${params.documentTag}/${params.documentId}`).then((res) => {
            return {
              data: res.data,
              documentTag: params.documentTag,
              documentId: params.documentId
            }
          }).catch((err) => {
            if(err.response.status === 499){
              throw new Response("Not Found Request", { status: err.response.status })
            }else{
              throw new Response("Something went wrong", { status: err.response.status })
            }
          })
        },
        handle: {
          crumb: (e) => e ? <Link to={`/request/task/externaltravel/${e.documentId}`}>External Travel</Link> : 'Error'
        },
        element: <ProtectedSuspense routeName='Task'><ExternalTravel/></ProtectedSuspense>,
      },
    ]
  },
  {
    name: 'Non Site Travel Request',
    path: '/request/nonsitetravel',
    handle: {
      crumb: () => <Link to={'/request/nonsitetravel'}>Non Site Travel Request</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='NonSiteTravel'><PeopleListSiteTravel /></ProtectedSuspense>,
      },
      {
        name: 'Approval Groups Detail',
        path: '/request/nonsitetravel/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/nonsitetravel/${e.empId}`}>Create a request</Link>
        },
        element: <ProtectedSuspense routeName='NonSiteTravel'><CreateNonSiteTravel/></ProtectedSuspense>,
      },
    ]
  },
  {
    name: 'TAS Profile Changes Request',
    path: '/request/tasprofilechanges',
    handle: {
      crumb: () => <Link to={'/request/tasprofilechanges'}>TAS Profile Changes Request</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='TASProfileChanges'><PeopleList /></ProtectedSuspense>,
      },
      {
        name: 'TAS Profile Changes Request Create',
        path: '/request/tasprofilechanges/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/tasprofilechanges/${e.empId}`}>Create a request</Link>
        },
        element: <ProtectedSuspense routeName='TASProfileChanges'><CreateSAMProfileChanges/></ProtectedSuspense>,
      },
    ]
  },
  {
    name: 'Site Travel Request',
    path: '/request/sitetravel',
    handle: {
      crumb: () => <Link to={'/request/sitetravel'}>Site Travel Request</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='SiteTravel'><PeopleListSiteTravel /></ProtectedSuspense>,
      },
      {
        name: 'Site Travel Request Create',
        path: '/request/sitetravel/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/sitetravel/${e.empId}`}>Create a request</Link>
        },
        element: <ProtectedSuspense routeName='SiteTravel'><CreateSiteTravel/></ProtectedSuspense>,
      },
    ]
  },
  {
    name: 'De-Mobilisation Request',
    path: '/request/de-mobilisation',
    handle: {
      crumb: () => <Link to={'/request/de-mobilisation'}>De-Mobilisation Request</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='De-Mobilisation'>
          <PeopleList fixedValue={{Active: 1}} hideFields={["Active"]} />
        </ProtectedSuspense>,
      },
      {
        name: 'De-Mobilisation Request Create',
        path: '/request/de-mobilisation/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/de-mobilisation/${e.empId}`}>Create a request</Link>
        },
        element: <ProtectedSuspense routeName='De-Mobilisation'><CreateDeMobilisation/></ProtectedSuspense>,
      },
    ]
  },
  {
    name: 'Existing Bookings',
    path: '/request/existingbookings',
    handle: {
      crumb: (e) => <Link to={`/request/existingbookings`}>Existing Bookings</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='ExistingBookings'><ExistingBooking/></ProtectedSuspense>,
      },
      {
        name: 'Existing Booking Detail',
        path: '/request/existingbookings/:empId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/existingbookings/${e.empId}`}>Existing Booking Detail</Link>
        },
        element: <ProtectedSuspense routeName='ExistingBookings'><ExistingBookingDetail/></ProtectedSuspense>,
      },
    ]
  },
  {
    name: 'Impersonate User',
    path: '/request/impersonateuser',
    handle: {
      crumb: (e) => <Link to={`/request/impersonateuser`}>Impersonate User</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='ImpersonateUser'><ImpersonateUser/></ProtectedSuspense>,
      },
      {
        name: 'Existing Booking Detail',
        path: '/request/impersonateuser/:impersoniteId',
        loader: async ({params}) => {
          return params
        },
        handle: {
          crumb: (e) => <Link to={`/request/existingbookings/${e.empId}`}>Impersonate & Tasks</Link>
        },
        element: <ProtectedSuspense routeName='ImpersonateUser'><ImpersonateUserTasks/></ProtectedSuspense>,
      },
    ]
  },
  {
    name: 'Hierarchical User',
    path: '/request/hierarchicaluser',
    handle: {
      crumb: (e) => <Link to={`/request/hierarchicaluser`}>Hierarchical User</Link>
    },
    errorElement: <ErrorPage/>,
    element: <ProtectedSuspense routeName='HierarchicalUser'><HierarchicalUser/></ProtectedSuspense>
  },
  {
    name: 'Cancel Request',
    path: '/request/cancelrequest',
    handle: {
      crumb: (e) => <Link to={`/request/cancelrequest`}>Cancel Request</Link>
    },
    errorElement: <ErrorPage/>,
    element: <ProtectedSuspense routeName='CancelRequest'><CancelRequest/></ProtectedSuspense>
  },
  {
    name: 'Active Transport',
    path: '/request/activetransport',
    handle: {
      crumb: (e) => <Link to={`/request/activetransport`}>Active Transport</Link>
    },
    errorElement: <ErrorPage/>,
    element: <ProtectedSuspense routeName='ActiveTransportRequest'><ActiveTransportSchedule/></ProtectedSuspense>
  },
  {
    name: 'Delegation Configuration',
    path: '/request/delegationconfig',
    element: <ProtectedSuspense routeName='DelegationConfiguration'><DelegationConfig/></ProtectedSuspense>,
    handle: {
      crumb: () => <Link to={'/request/delegationconfig'}>Delegation Config</Link>
    },
    errorElement: <ErrorPage/>,
  },
  {
    name: 'Shift Visual',
    path: '/request/shiftvisual',
    handle: {
      crumb: (e) => <Link to={`/request/shiftvisual`}>Shift Visual</Link>
    },
    errorElement: <ErrorPage/>,
    children: [
      {
        index: true,
        element: <ProtectedSuspense routeName='ShiftVisual'><RequestShiftVisual/></ProtectedSuspense>,
      },
      {
        name: 'Shift Visual Detail',
        path: '/request/shiftvisual/:empId',
       loader: async ({params}) => {
          return axios.get( `tas/employee/${params.empId}`).then((res) => {
            return res.data
          })
        },
        handle: {
          crumb: (e) => <Link to={`/request/shiftvisual/${e.Id}`}>{e.Firstname} {e.Lastname}</Link>
        },
        element: <ProtectedSuspense routeName='ShiftVisual'><RequestShiftVisualDetail/></ProtectedSuspense>,
      },
    ]
  },
  ///////////////////////////////////                REPORT                 ///////////////////////
  
  {
    name: 'Report',
    path: '/report',
    element: <ProtectedSuspense routeName='DashboardReport'><ReportDashboard/></ProtectedSuspense>,
    errorElement: <ErrorPage/>,
    handle: {
      crumb: () => <Link to={'/report'}>Dashboard</Link>
    }
  },
  {
    name: 'Template',
    path: '/report/template',
    element: <ProtectedSuspense routeName='Template'><Template/></ProtectedSuspense>,
    errorElement: <ErrorPage/>,
    handle: {
      crumb: () => <Link to={'/report/template'}>Template</Link>
    }
  },
  {
    name: 'Scheduler',
    path: '/report/scheduler',
    element: <ProtectedSuspense routeName='Scheduler'><Scheduler/></ProtectedSuspense>,
    errorElement: <ErrorPage/>,
    handle: {
      crumb: () => <Link to={'/report/scheduler'}>Scheduler</Link>
    }
  },

  ///////////////////////////////////                ERROR PAGES                 ///////////////////////
  
  {
    name: 'Error Page',
    path: '/tas/*',
    element:<Page404/>
  },
  {
    name: 'Error Page',
    path: '/request/*',
    element:<Page404/>
  },
  {
    name: 'Error Page',
    path: '/report/*',
    element:<Page404/>
  },
]

export default routes