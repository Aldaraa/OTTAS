import { DepartmentTooltip, Tooltip } from "components"
import dayjs from "dayjs"
import { BsAirplaneFill } from "react-icons/bs"
import { FaFemale, FaMale } from "react-icons/fa"
import { Link } from "react-router-dom"

const ownerColumns = [
  {
    label: '#',
    name: 'Id',
    alignment: 'left',
    width: '50px',
    cellRender: (e) => (
      <span className='text-[12px]'>{e.value}</span>
    )
  },
  {
    label: 'Employee', 
    name: 'Firstname', 
    alignment: 'left', 
    cellRender: (e) => (
      <div className='flex items-center gap-3 group p-1'>
        <div id={`emp${e.data.Id}`} style={{fontSize: '12px'}} className='flex items-center gap-3'>
          {e.data.Gender === 1 ? <FaMale size={14} className='text-blue-600'/> : <FaFemale size={14} className='text-pink-500'/>} 
          <Link to={`/tas/roomassign/${e.data.Id}`} >
            <span className='cursor-pointer text-blue-500 hover:underline'>
              {e.value} {e.data.Lastname}
            </span>
          </Link>
        </div>
      </div>
    )
  },
  {
    label: 'SAPID', 
    name: 'SAPID', 
    alignment: 'left', 
    width: '70px',
    cellRender: (e) => (
      <span className='text-[12px] px-1'>
        {e.value}
      </span>
    )
  },
  {
    label: 'Department', 
    name: 'DepartmentName',
    alignment: 'left', 
    cellRender: (e) => (
      <DepartmentTooltip showStatus={false} id={e.data?.DepartmentId}>
        <span className='text-[12px] px-1 flex whitespace-pre-wrap'>
          {e.value}
        </span>
      </DepartmentTooltip>
    )
  },
  {
    label: 'Employer', 
    name: 'EmployerName', 
    alignment: 'left', 
    cellRender: (e) => (
      <span className='text-[12px] flex px-1'>
        {e.value}
      </span>
    )
  },
  {
    label: 'Res Type', 
    name: 'PeopleTypeName', 
    alignment: 'left', 
    cellRender: (e) => (
      <span className='text-[12px] flex px-1'>
        {e.value}
      </span>
    )
  },
  {
    label: 'Position', 
    name: 'PositionName', 
    alignment: 'left', 
    cellRender: (e) => (
      <span className='text-[12px] px-1 flex whitespace-pre-wrap'>
        {e.value}
      </span>
    )
  },
  {
    label: 'OnSite Date', 
    name: 'futureTransportDate', 
    alignment: 'left', 
    width: 110,
    cellRender: (e) => (
      <div className='flex items-center gap-2 text-[12px] px-1'>
        {
          e.data.futureTransportScheduleDescription &&
          <Tooltip title={e.data.futureTransportScheduleDescription}>
            <BsAirplaneFill id="airplane" size={14} color='1a66ff' style={{transform: 'rotate(135deg)', cursor: 'pointer'}}/>
          </Tooltip>
        }
        <div>{e.value ? dayjs(e.value).format('YYYY-MM-DD') : ''}</div>
      </div>
    )
  },
  {
    label: 'Out Date', 
    name: 'futureTransportOUTDate', 
    alignment: 'left', 
    width: 110,
    cellRender: (e) => (
      <div className='flex items-center gap-2 text-[12px] px-1'>
        {
          e.data.futureTransportScheduleDescription &&
          <Tooltip title={e.data.futureTransportScheduleDescription}>
            <BsAirplaneFill id="airplane" size={14} color='555' style={{transform: 'rotate(45deg)', cursor: 'pointer'}}/>
          </Tooltip>
        }
        <div>{e.value ? dayjs(e.value).format('YYYY-MM-DD') : ''}</div>
      </div>
    )
  },
]

export default ownerColumns