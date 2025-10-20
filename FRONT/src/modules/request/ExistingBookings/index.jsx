import { PeopleSearch } from 'components'
import { AuthContext } from 'contexts';
import React, { useContext, useEffect } from 'react'
import { useNavigate } from 'react-router-dom';

function ExistingBooking() {
  const navigate = useNavigate()
  const { action } = useContext(AuthContext)
  useEffect(() => {
    action.changeMenuKey('/request/existingbookings')
  },[])

  return (
    <div className='rounded-ot shadow-card bg-white'>
      <PeopleSearch
        selectType={'link'}
        toLink={(data) => `${data.Id}`}
        fixedValue={{Active: 1}}
        hideFields={['Active']}
        onRowDblClick={(e) => navigate(`/request/existingbookings/${e.data.Id}`)}
        tableClass='max-h-[calc(100vh-270px)]'
      />
    </div>
  )
}

export default ExistingBooking