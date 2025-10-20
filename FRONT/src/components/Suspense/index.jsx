import { Loading } from 'components'
import React from 'react'
import { Suspense as RSuspense } from 'react'
function Suspense({children}) {
  return (
    <RSuspense fallback={<Loading/>}>
      {children}
    </RSuspense>
  )
}

export default Suspense