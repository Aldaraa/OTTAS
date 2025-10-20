import { Select } from 'antd'
import axios from 'axios'
import React from 'react'
import { useEffect } from 'react'
import { useState } from 'react'

function ISelect({dependentvalue,  optionsurl, setFieldValue, _field, name, isDefault, getFieldValue, optionvalue, optionlabel,...props}) {
  const [ options, setOptions ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ init, setInit ] = useState(true)

  useEffect(() => {
    if(dependentvalue){
      getOptions()
      if(typeof isDefault === 'boolean'){

      }else{
        if(!init) {
          setFieldValue(name, '')
        }
        else {
          setInit(false)
        }
      }
    }

  },[dependentvalue])

  const getOptions = () => {
    setLoading(true)
    if(optionsurl) {
      if(typeof optionsurl === 'string') {
        axios({
          method: 'get',
          url: `${optionsurl}${dependentvalue}`,
        }).then(res => {
          let tmp = []
          res.data.map((item,i) => {
            tmp.push({
              ...item,
              value: item[optionvalue] ? item[optionvalue] : item.id,
              label: item[optionlabel] ? item[optionlabel] : item.name,
            })
          })
          setOptions(tmp)  
        }).catch(err => {
        }).then(() => {
          setLoading(false)
        })
      }
      else{
        setOptions(optionsurl)
      }
    }
  }

  const handleOnChange = async (value, option) => {
    // await setFieldValue(`address`, option)
    await setFieldValue(`${_field.name}`, value)
  }

  return (
    <Select loading={loading} allowClear {...props}>
      {
        options.map((item, i) => (
          <Select.Option value={item.value} key={`select-option-${i}`}>{item.label}</Select.Option>
        ))
      }
    </Select>
  )
}

export default ISelect