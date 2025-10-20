import { PeopleSearch } from 'components'
import React, { useCallback } from 'react'
import { useNavigate } from 'react-router-dom'

const PeopleList = ({fixedValue=null, ...rest} ) => {
  const navigate = useNavigate()

  const onRowDnlClick = useCallback((e) => {
    navigate(`${e.data.Id}`)
  },[])

  const toLink = useCallback((data) => {
    return `${data.Id}`
  },[])

  return (
    <div className=''>
      <PeopleSearch
        selectType={'link'}
        toLink={toLink}
        onRowDblClick={onRowDnlClick}
        tableClass='max-h-[calc(100vh-290px)]'
        fixedValue={fixedValue}
        {...rest}
      />
    </div>
  )
}

export default PeopleList