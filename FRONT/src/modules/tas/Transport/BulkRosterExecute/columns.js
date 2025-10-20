import { CheckBox } from "devextreme-react";
import { BsAirplaneFill } from "react-icons/bs";
import { Link } from "react-router-dom";

const columns = [
  {
    label: 'Person #',
    name: 'Id',
    dataType: 'string',
    width: '70px',
  },
  // {
  //   label: '',
  //   name: 'Active',
  //   width: '21px',
  //   alignment: 'center',
  //   cellRender:(e) => (
  //     <GoPrimitiveDot color={e.value === 1 ? 'lime' : 'lightgray'}/>
  //   )
  // },
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
    width: '220px',
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
    width: '70px',
  },
  {
    label: 'Resource Type',
    name: 'PeopleTypeName',
    dataType: 'string',
  },
  {
    label: 'Room',
    name: 'RoomNumber',
    dataType: 'string',
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
  {
    label: '',
    name: '',
    width: '90px',
    alignment: 'center',
    cellRender: (e) => (
      <div className='flex items-center'>
        <Link to={`/tas/people/search/${e.data.Id}`}>
          <button type='button' className='edit-button'>View</button>
        </Link>
      </div>
    )
  },
]

export default columns