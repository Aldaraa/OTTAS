import { AccommodationPeopleSearch, PeopleSearch } from 'components'
import React from 'react'
import { useNavigate } from 'react-router-dom'

function RoomAssign() {
  const navigate = useNavigate()
  return (
    <AccommodationPeopleSearch
      selectType='link'
      toLink={(e) => `${e.Id}`}
      onRowDblClick={(e) => navigate(`/tas/roomassign/${e.data.Id}/roombooking`)}
      tableClass='max-h-[calc(100vh-300px)]'
    />
  )
}

export default RoomAssign