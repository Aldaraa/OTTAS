import { PeopleSearch } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'

function RequestShiftVisual() {
  const navigate = useNavigate()
  const { action } = useContext(AuthContext)

  useEffect(() => {
    action.changeMenuKey('/request/shiftvisual')
  },[])
  return (
    <PeopleSearch
      selectType={'link'}
      toLink={(data) => `${data.Id}`}
      onRowDblClick={(e) => navigate(`${e.data.Id}`)}
      tableClass='max-h-[calc(100vh-290px)]'
    />
  )
}

export default RequestShiftVisual