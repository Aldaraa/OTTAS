const getDob = (register) => {
  if(parseInt(register[4]) > 1){
    return `20${register.slice(2,4)}-${register[4]-2}${register[5]}-${register.slice(6,8)}`
  }else{
    return `19${register.slice(2,4)}-${register.slice(4,6)}-${register.slice(6,8)}`
  }
};

const getGender = (register) => {
  if(parseInt(register[8]) % 2 === 1){
    return 1
  }else{
    return 0
  }
};

const mnRegister = {
  getDob,
  getGender,
};

export default mnRegister;