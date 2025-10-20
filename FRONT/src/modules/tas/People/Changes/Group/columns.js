import { CheckBox } from "devextreme-react";
import { BsAirplaneFill } from "react-icons/bs";
import { GoPrimitiveDot } from "react-icons/go";
import { Link } from "react-router-dom";

export default [
  {
    label: 'Person #',
    name: 'Id',
    dataType: 'string',
    width: '70px',
    cellRender: (e) => (
      <div className="flex items-center justify-center">
        <Link to={`/tas/people/search/${e.data.Id}`}>
          <button type='button' className="text-blue-600 underline">{e.value}</button>
        </Link>
      </div>
    )
  },
  {
    label: '',
    name: 'Active',
    width: '21px',
    alignment: 'center',
    showInColumnChooser: false,
    allowSorting: false,
    cellRender:(e) => (
      <GoPrimitiveDot color={e.value === 1 ? 'lime' : 'lightgray'}/>
    )
  },
  {
    label: 'Firstname',
    name: 'Firstname',
  },
  {
    label: 'Lastname',
    name: 'Lastname',
  },
  {
    label: 'SAP #',
    name: 'SAPID',
    alignment: 'left',
  },
  {
    label: 'Department',
    name: 'DepartmentName',
  },
  {
    label: 'Employer',
    name: 'EmployerName',
  },
  {
    label: 'Roster',
    name: 'RosterName',
  },
  {
    label: 'Resource Type',
    name: 'PeopleTypeName',
  },
  {
    label: 'Room',
    name: 'RoomNumber',
    cellRender: (e) => (
      <span>{e.value}</span>
    )
  },
  {
    label: 'Gender',
    name: 'Gender',
    alignment: 'center',
    width:'70px',
    cellRender: (e) => (
      <span>{e.data.Gender === 1 ? 'M' : 'F'}</span>
    ),
    groupCellRender: (e) => (
      <span>{e.value === 1 ? 'M' : 'F'} {e.groupContinuesMessage ? `- ${e.groupContinuesMessage}` : ''}</span>
    )
  },
  {
    label: '',
    name: 'HasFutureTransport',
    width: '50px',
    cellRender: (e) => (
      <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    ),
    headerCellRender: (he, re) => (
      <BsAirplaneFill></BsAirplaneFill>
    )
  },
  {
    label: '',
    name: 'HasFutureRoomBooking',
    width: '50px',
    cellRender: (e, r) => (
      <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    ),
    headerCellRender: (he, re) => (
      <i className="dx-icon-home"></i>
    )
  },
  // {
  //   label: '',
  //   name: '',
  //   width: '90px',
  //   alignment: 'center',
  //   cellRender: (e) => (
  //     <div className='flex items-center'>
  //       <Link to={`/tas/people/search/${e.data.Id}`}>
  //         <button type='button' className='edit-button' >View</button>
  //       </Link>
  //     </div>
  //   )
  // },
]