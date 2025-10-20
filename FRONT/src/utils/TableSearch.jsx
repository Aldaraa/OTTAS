function filter(array, key, value, childName) {
  const getNodes = (result, object) => {
    if(typeof value === 'string' && object[key]?.toLowerCase().includes(value?.toLowerCase())){
      result.push(object);
      return result;
    }
    else if(object[key] === value){
      result.push(object);
      return result;
    }

    if (Array.isArray(object[childName])) {
      const nodes = object[childName].reduce(getNodes, []);
      if (nodes.length) result.push({ ...object, nodes });
    }

    return result;
  };

  return array.reduce(getNodes, []);
}

const filterData = (data, searchValues, childName) => {
  let resultData = []
  const keys = Object.keys(searchValues)
  data.map((item) => {
    if(item[childName].length > 0){
      resultData.push({...item, [childName]: filterData(item[childName], searchValues, childName)})
    }else{
      let isMatch = false
      keys.map((key) => {
        if((searchValues[key] !== '') && (typeof searchValues[key] !== 'undefined') && (searchValues[key] !== null)){
          if(typeof searchValues[key] === 'string'){
            isMatch = isMatch || item[key]?.toLowerCase().includes(searchValues[key]?.toLowerCase())
          } 
          else{
            isMatch = isMatch || (item[key] === searchValues[key])
          }
        }
        else{
          isMatch = true
        }
      })
      if(isMatch){
        resultData.push(item)
      }
    }
  })

  return resultData
}

export default function tableSearch(values, data, dataStructure, childName) {
  if(dataStructure === 'tree'){
    return filterData(data, values, childName);
  }
  else{
    return new Promise(function(resolve, reject){
      const keys = Object.keys(values)
      let resultData = [...data]
      keys.map((key) => {
        if((values[key] !== '') && (typeof values[key] !== 'undefined') && (values[key] !== null)){
          if(typeof values[key] === 'string'){
            resultData = resultData.filter(item => item[key]?.toLowerCase().includes(values[key]?.toLowerCase()))
          } 
          else{
            resultData = resultData.filter(item => item[key] === values[key])
  
          }
        }
      })
      resolve(resultData);
    })
  }

}

// const filterDepartments = (data, searchValues, childName) => {
//   let isMatch = false
//   let hasMatchingChild = false
//   const keys = Object.keys(searchValues)
  
//   const result = data.filter((department) => {
//     keys.map((key) => {
//       if((searchValues[key] !== '') && (typeof searchValues[key] !== 'undefined') && (searchValues[key] !== null)){
//         if(typeof searchValues[key] === 'string'){
//           isMatch = (department[key].toLowerCase().includes(searchValues[key].toLowerCase()));
//         } 
//         else{
//           isMatch = isMatch || (department[key] === searchValues[key]);
//         }
//       }
//     })
//     if(department[childName].length > 0){
//       hasMatchingChild = department[childName].some(child => filterDepartments([child], searchValues, childName).length > 0);
//     }
//     return isMatch || hasMatchingChild
//   });
//   return result
  
// };

// export default function tableSearch(values, data, dataStructure, childName) {
//   if(dataStructure === 'tree'){
//     return filterDepartments(data, values, childName);
//   }
//   else{
//     return new Promise(function(resolve, reject){
//       const keys = Object.keys(values)
//       let resultData = [...data]
//       keys.map((key) => {
//         console.log('search by value', key);
//         if((values[key] !== '') && (typeof values[key] !== 'undefined') && (values[key] !== null)){
//           if(typeof values[key] === 'string'){
//             resultData = resultData.filter(item => item[key]?.toLowerCase().includes(values[key]?.toLowerCase()))
//           } 
//           else{
//             resultData = resultData.filter(item => item[key] === values[key])
  
//           }
//         }
//       })
//       resolve(resultData);
//     })
//   }

// }
