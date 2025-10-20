import axios from 'axios'
import { Tooltip } from 'components'
import React, { useCallback, useEffect, useState } from 'react'
import { FaUsers } from 'react-icons/fa'
import { Link } from 'react-router-dom'
import { twMerge } from 'tailwind-merge'

const Members = ({data, children, toLink}) => {
  return(
    (data?.AdminName || data?.SupervisorName) ?
    <Tooltip
      trigger='click'
      title={<div>
        {data?.AdminName ?
          <div className='flex gap-1'>
            Admin:
            {toLink ? 
              <Link to={`${toLink}${data.AdminId}`} >
                <span className='text-blue-400 hover:underline'>{data?.AdminName}</span>
              </Link>
              :
              <span className='text-blue-400'>{data?.AdminName}</span>
            }
          </div>
          : null
        }
        {data?.SupervisorName ?
          <div className='flex gap-1'>
            Supervisor:
            {
              toLink ?
              <Link to={`${toLink}${data.SupervisorId}`} >
                <span className='text-blue-400 hover:underline'>{data?.SupervisorName}</span>
              </Link>
              : 
              <span className='text-blue-400'>{data?.SupervisorName}</span>
            }
          </div>
          : null
        }
      </div>}
    >
      <span className='cursor-pointer hover:text-black'>
        {children}
      </span>
    </Tooltip>
    : children
  )
}

const DepartmentList = React.memo(({data, id, toLink=''}) => {
  const [ parentDepartment, setParentDepartment ] = useState([])

  useEffect(() => {
    if(id){
      getParentDepartments()
    }
  },[id])

  const getParentDepartments = useCallback(() => {
    axios({
      method: 'get',
      url: `tas/department/parent/${id}`,
    }).then((res) => {
      setParentDepartment(res.data.sort((a, b) => b.DepartmentLevel - a.DepartmentLevel))
    }).catch((err) => {

    })
  }, [id])

  return (
    <div>
      <div className='flex gap-3 items-center'>
        {/* <div><FaUsers size={15}/></div> */}
        <div className='text-gray-400'>{data?.DepartmentCurrentStatus ? 'Temporary' : 'Permanent'} Department</div>
      </div>
      {
        parentDepartment.map((item, i) => (
          <div key={i} className={(twMerge('relative leading-tight ', i === parentDepartment.length-1 ? 'text-primary' : ''))}>
            <span className='text-gray-500 mr-1'>•</span>  <span className='font-medium'><Members toLink={toLink} data={item}>{item.Name}</Members></span>
              { item?.ManagerName ?
                <span>
                  <span className='mx-1'>|</span>{toLink ? <Link to={`${toLink}${item.ManagerId}`}><span className='text-blue-400 hover:underline'>{item?.ManagerName}</span></Link> : <span className='text-blue-400'>{item?.ManagerName}</span>}
                </span>
                : null
              }
            </div>
        ))
      }
    </div>
  )
}, (prev, next) => prev.id === next.id && prev.trigger === next.trigger
)

export default DepartmentList