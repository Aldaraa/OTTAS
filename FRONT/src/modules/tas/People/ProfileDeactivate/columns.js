import { CheckBox } from "devextreme-react";
import { BsAirplaneFill } from "react-icons/bs";
import { GoPrimitiveDot } from "react-icons/go";
import { Link } from "react-router-dom";

export default [
  {
    label: 'Person #',
    name: 'Id',
    dataType: 'string',
  },
  {
    label: 'SAP ID #',
    name: 'SAPID',
    alignment: 'left',
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
    dataType: 'string',
  },
  {
    label: 'Lastname',
    name: 'Lastname',
    dataType: 'string',
  },
  {
    label: 'Department',
    name: 'DepartmentName',
    dataType: 'string',
  },
  {
    label: 'Employer',
    name: 'EmployerName',
    dataType: 'string',
  },
  {
    label: 'Roster',
    name: 'RosterName',
    dataType: 'string',
    enableFilter: true,
  },
  {
    label: 'Resource Type',
    name: 'PeopleTypeName',
    dataType: 'string',
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
    width:'60px',
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
    width: '35px',
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
    width: '35px',
    cellRender: (e, r) => (
      <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    ),
    headerCellRender: (he, re) => (
      <i className="dx-icon-home"></i>
    )
  },
  {
    label: '',
    name: '',
    width: '65px',
    alignment: 'center',
    cellRender: (e) => (
      <div className='flex items-center'>
        <Link to={`/tas/people/search/${e.data.Id}`}>
          <button type='button' className='edit-button' >View</button>
        </Link>
      </div>
    )
  },
]