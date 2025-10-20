import { Suspense } from 'components'
import React from 'react'
import ProtectedRoute from 'routes/ProtectedRoute'

function ProtectedSuspense({children, ...restprops}) {
  return (
    <ProtectedRoute {...restprops}>
      <Suspense>
          {children}
      </Suspense>
    </ProtectedRoute>
  )
}

export default ProtectedSuspense