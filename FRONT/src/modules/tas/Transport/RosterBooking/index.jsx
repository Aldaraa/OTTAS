import React from 'react'
import { useNavigate } from 'react-router-dom'
import { PeopleSearch } from 'components'

function RosterBooking() {
  const navigate = useNavigate()
  return (
    <div className='bg-white px-0 pb-0 rounded-ot'>
      <PeopleSearch
        selectType={'link'}
        toLink={(data) => `/tas/people/search/${data.Id}/rosterexecute`}
        target='_blank'
        onRowDblClick={(e) => navigate(`/tas/people/search/${e.data.Id}/rosterexecute`)}
        style={{maxHeight: 'calc(100vh - 290px)'}}
      />
    </div>
  )
}

export default RosterBooking