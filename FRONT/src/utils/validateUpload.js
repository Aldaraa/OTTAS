const requiredFileTypes = ['text/csv', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'application/vnd.ms-excel']

const validateFile = (file) => {
  if(requiredFileTypes.includes(file.type)){
    return true
  }
  else {
    return false
  }
}

export default validateFile