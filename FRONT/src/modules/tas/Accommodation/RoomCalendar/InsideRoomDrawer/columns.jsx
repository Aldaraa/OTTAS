const { DepartmentTooltip } = require("components");
const dayjs = require("dayjs");
const { FaMale, FaFemale } = require("react-icons/fa");
const { Link } = require("react-router-dom");

const generateDocumentTag = (tag) => {
  switch (tag) {
    case 'ADD': return 'addtravel'
    case 'REMOVE': return 'remove'
    case 'RESCHEDULE': return 'reschedule'
  }
}

const ownerColumns = [
  {
    label: 'Person #',
    name: 'Id',
    alignment: 'left',
  },
  {
    label: 'Fullname',
    name: 'FullName',
    cellRender:(e) => (
      <div className='flex'>
        {e.data.Gender === 1 ? <FaMale size={14} className='text-blue-600'/> : <FaFemale size={14} className='text-pink-500'/>} 
        <i className="dx-icon-home text-green-500 mr-1"></i>
        <Link to={`/tas/roomassign/${e.data?.EmployeeId}/roombooking`}>
          <span className='cursor-pointer text-blue-500 hover:underline whitespace-normal'>{e.value}</span>
        </Link>
      </div>
    )
  },
  {
    label: 'Department',
    name: 'DepartmentName',
    cellRender: (e) => (
      <DepartmentTooltip id={e.data?.DepartmentId} showStatus={false}>
        <span className=' whitespace-normal'>{e.value}</span>
      </DepartmentTooltip>
    )
  },
  {
    label: 'People Type',
    name: 'PeopleTypeCode',
  },
  {
    label: 'Employer',
    name: 'EmployerName',
    cellRender: (e) => (
      <span className=' whitespace-normal'>{e.value}</span>
    )
  },
  {
    label: 'Shift',
    name: 'ShiftCode',
    width: 50,
  },
  {
    label: 'Start',
    name: 'StartDate',
    width: '80px',
    cellRender:(e) => (
      <div>{ e.value ? dayjs(e.value).format('YYYY-MM-DD') : ''}</div>
    )
  },
  {
    label: 'End',
    name: 'EndDate',
    width: '80px',
    cellRender:(e) => (
      <div>{ e.value ? dayjs(e.value).format('YYYY-MM-DD') : ''}</div>
    )
  },
]

const lockedColumns = [
  {
    label: 'Person #',
    name: 'Id',
    alignment: 'left',
  },
  {
    label: 'Fullname',
    name: 'FullName',
    cellRender:(e) => (
      <div className='flex'>
        {e.data.Gender === 1 ? <FaMale size={14} className='text-blue-600'/> : <FaFemale size={14} className='text-pink-500'/>} 
        {e.data.RoomOwner ? <i className="dx-icon-home text-green-500 mr-1"></i> : <i className="dx-icon-home text-gray-400 mr-1"></i>}
        <Link to={`/tas/roomassign/${e.data?.EmployeeId}/roombooking`}>
          <span className='cursor-pointer text-blue-500 hover:underline whitespace-normal'>{e.value}</span>
        </Link>
      </div>
    )
  },
  {
    label: 'Department',
    name: 'DepartmentName',
    cellRender: (e) => (
      <DepartmentTooltip id={e.data?.DepartmentId} showStatus={false}>
        <span className=' whitespace-normal'>{e.value}</span>
      </DepartmentTooltip>
    )
    // alignment:'center',
  },
  {
    label: 'People Type',
    name: 'PeopleTypeCode',
  },
  {
    label: 'Employer',
    name: 'EmployerName',
    cellRender: (e) => (
      <span className=' whitespace-normal'>{e.value}</span>
    )
  },
  {
    label: 'Shift',
    name: 'ShiftCode',
    width: 50,
  },
  {
    label: 'Start',
    name: 'StartDate',
    width: '80px',
    cellRender:(e) => (
      <div>{ e.value ? dayjs(e.value).format('YYYY-MM-DD') : ''}</div>
    )
  },
  {
    label: 'End',
    name: 'EndDate',
    width: '80px',
    cellRender:(e) => (
      <div>{ e.value ? dayjs(e.value).format('YYYY-MM-DD') : ''}</div>
    )
  },
  {
    label: 'Document',
    name: 'DocumentId',
    alignment: 'left',
    cellRender: (e) => (
      <div className='flex items-center text-blue-500 hover:underline'>
        <Link to={`/request/task/sitetravel/${generateDocumentTag(e.data.DocumentTag)}/${e.value}`} target='_blank'>
          {e.value}
        </Link>
      </div>
    )
  },
]

const roomEmpColumns = [
  {
    label: 'Person #',
    name: 'Id',
    alignment: 'left',
  },
  {
    label: 'Fullname',
    name: 'FullName',
    cellRender:(e) => (
      <div className='flex gap-1'>
        <div className='flex'>
          {e.data.Gender === 1 ? <FaMale size={14} className='text-blue-600'/> : <FaFemale size={14} className='text-pink-500'/>} 
          {e.data.RoomOwner ? <i className="dx-icon-home text-green-500"></i> : <i className="dx-icon-home text-gray-400"></i>}
        </div>
        <Link to={`/tas/roomassign/${e.data?.EmployeeId}/roombooking`}>
          <span className='cursor-pointer text-blue-500 hover:underline ml-1 whitespace-normal'>{e.value}</span>
        </Link>
      </div>
    )
  },
  {
    label: 'Department',
    name: 'DepartmentName',
    cellRender: (e) => (
      <DepartmentTooltip id={e.data?.DepartmentId} showStatus={false}>
        <span className=' whitespace-normal'>{e.value}</span>
      </DepartmentTooltip>
    )
  },
  {
    label: 'People Type',
    name: 'PeopleTypeCode',
  },
  {
    label: 'Employer',
    name: 'EmployerName',
    cellRender: (e) => (
      <span className=' whitespace-normal'>{e.value}</span>
    )
  },
  {
    label: 'Shift',
    name: 'ShiftCode',
    width: 50,
  },
  {
    label: 'End',
    name: 'EndDate',
    width: '80px',
    cellRender:(e) => (
      <div>{ e.value ? dayjs(e.value).format('YYYY-MM-DD') : ''}</div>
    )
  },
]

export { ownerColumns, lockedColumns, roomEmpColumns }