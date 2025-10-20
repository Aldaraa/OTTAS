import axios from 'axios'
import { Tooltip } from 'components'
import React, { useCallback, useState } from 'react'
import { twMerge } from 'tailwind-merge'

const DepartmentTooltip = React.memo(({data, children, showStatus=true, id, trigger='hover'}) => {
  const [ parentDepartment, setParentDepartment ] = useState([])
  const [ show, setShow ] = useState(false)
  const [ isInited, setIsInited ] = useState(false)

  const getParentDepartments = useCallback(() => {
    axios({
      method: 'get',
      url: `tas/department/parent/${id}`,
    }).then((res) => {
      setIsInited(true)
      setParentDepartment(res.data.sort((a, b) => b.DepartmentLevel - a.DepartmentLevel))
      setShow(true)
    }).catch((err) => {

    })
  }, [id])

  const handleClick = useCallback((isOpen) => {
    if(isOpen){
      if(!isInited){
        getParentDepartments()
      }else{
        setShow(true)
      }
    }
    if(!isOpen) setShow(false)
  },[isInited])

  return (
    <Tooltip
      trigger={trigger}
      onOpenChange={handleClick}
      open={show}
      key={id}
      title={<div>
        {showStatus ? <div className='text-gray-400'>{data?.DepartmentCurrentStatus ? 'Temporary' : 'Permanent'} Department</div> : null}
        {
          parentDepartment.map((item, i) => (
            <div key={i} className={(twMerge('relative flex px-2', i === parentDepartment.length-1 ? 'text-primary' : ''))} style={{marginLeft: `${i*14}px`}}>
              <div className='mr-1 text-gray-500'>•</div><div className=' leading-tight'>{item.Name}</div>
            </div>
          ))
        }
      </div>
    }>
      {children}
    </Tooltip>
  )
}, (prev, next) => JSON.stringify(prev.data) === JSON.stringify(next.data) && 
    prev.showStatus === next.showStatus && 
    prev.id === next.id && prev.trigger === next.trigger
)

export default DepartmentTooltip