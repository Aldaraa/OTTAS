import React, { useContext } from 'react'
import { AuthContext } from 'contexts';
import ERROR403 from 'assets/illustrations/403.webp'
import { Navigate } from 'react-router-dom';

function GuestProtectedRoute({routeName, children}) {
  const { state } = useContext(AuthContext)
  if(state.userInfo?.Role === 'Guest' || state.userInfo?.Role === 'Supervisor'){
    if(routeName){
      if(state.userInfo?.CreateRequest === 1){
        return children
      }else{
        return(
          <div className='w-full h-full flex flex-col justify-center items-center bg-white shadow-md rounded-ot text-xl'>
            <img alt='error-403' src={ERROR403} className='h-[400px]'/>
            You don't have permission to access
          </div>
        )
      }  
    } 
    else{
      return children
    }
  }else{
    return(
      <Navigate to='/tas'/>
    )
  }
}

export default GuestProtectedRoute