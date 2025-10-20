import React, { useContext } from 'react'
import { AuthContext } from 'contexts';
import ERROR403 from 'assets/illustrations/403.webp'
import { Navigate } from 'react-router-dom';

function ProtectedRoute({routeName, children}) {
  const { state } = useContext(AuthContext)

  if(state.userInfo?.Menu.find((item) => item.Code === routeName) || !routeName){
    return children
  }else{
    // action.onError("You don't have permission to access")
    return(
      state.userInfo?.callbackRoute ?
      <Navigate to={state.userInfo?.callbackRoute}/>
      :
      <div className='w-full h-full flex flex-col justify-center items-center bg-white shadow-md rounded-ot text-xl'>
        <img alt='error-403' src={ERROR403} className='h-[400px]'/>
        You don't have permission to access
      </div>
    )
  }
}

export default ProtectedRoute