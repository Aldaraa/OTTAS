const getDisabledStatus = ({documentData, userData, groupTag, userGroupIds}) => {
  console.log('hello', documentData, userData, groupTag);
  if(groupTag === 'Completed' || groupTag === 'Cancelled'){
    return true
  }else{
    if(documentData?.AssignedEmployeeId){
      if(userData?.LineManagerIds?.includes(documentData?.AssignedEmployeeId) || 
        documentData?.AssignedEmployeeId === userData?.EmployeeId || 
        (userData?.Rolename === 'Data Approval' && groupTag === 'linemanager') ||
        userData?.Rolename === 'SystemAdministrator'
      ){
        return false
      }else{
        return true
      }
    }else{
      if(
          userData?.Rolename === 'SystemAdministrator' || 
          userData?.ApprovalGroupIds?.includes(documentData?.AssignedRouteConfigId)
      ){
        return false
      }else{
        return true
      }
    }
  }
};

const getActionPermission = ({documentData, userData, groupTag, userGroupIds}) => {
  let status = null
  if(documentData?.CurrentStatus === 'Completed' || documentData?.CurrentStatus === 'Cancelled'){
    status = false
  }else{
    if(documentData?.AssignedEmployeeId){
      if(userData?.LineManagerIds?.includes(documentData?.AssignedEmployeeId) || 
        documentData?.AssignedEmployeeId === userData?.EmployeeId || 
        (userData?.Rolename === 'Data Approval' && groupTag === 'linemanager') ||
        userData?.Rolename === 'SystemAdministrator'
      ){
        status = true
      }else{
        status = false
      }
    }else{
      if(
        userData?.Rolename === 'SystemAdministrator' || 
        userData?.ApprovalGroupIds?.includes(documentData?.AssignedRouteConfigId)
      ){
        status = true
      }else{
        status = false
      }
    }
  }
  return status
};

const checkUpdateItinerary = ({groupAndMembers, userData}) => {
  let boolean = false
  let flightGroup = groupAndMembers?.find((item) => item.GroupTag === 'travelflight')
  if(userData?.GroupIds?.includes(flightGroup?.GroupId)){
    boolean = true
  }
  return boolean
}

const checkPermission = {
  getDisabledStatus,
  getActionPermission,
  checkUpdateItinerary,
};

export default checkPermission;