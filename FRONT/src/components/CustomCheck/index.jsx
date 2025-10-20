import { Checkbox } from 'antd'
import React, { useEffect, useState } from 'react'

function CustomCheck({_field, editData,  form, indeterminatewith, value, ...props}) {
  const [ indeterminate, setIndeterminate ] = useState(false)

  useEffect(() => {
    if(typeof value !== 'number'){
      setIndeterminate(true)
    }else(
      setIndeterminate(false)
    )
  },[value])
  
  const handleChangeCheck = (name) => {
    // let boolean
    if(form.getFieldValue(name) === 1){
      // setIndeterminate(true)
      form.setFieldValue(name, null)
      // boolean=true
    }
    else if(form.getFieldValue(name) === 0){
      form.setFieldValue(name, 1)
      // boolean=true
    }
    else{
      form.setFieldValue(name, 0)
      // setIndeterminate(false)
      // boolean=false
    }
    // return boolean
  }

  return (
    <Checkbox
      {...props}
      indeterminate={indeterminate}
      // checked={form.getFieldValue(_field.name) === 1 ? true : false}
      checked={value ? true : false}
      onChange={(e) => handleChangeCheck(_field.name)}
    >
      {props.children}
    </Checkbox>
  )
}

export default CustomCheck