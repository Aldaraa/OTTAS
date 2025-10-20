import React, { useEffect, useLayoutEffect } from 'react'
import { Collapse } from 'antd'
import { useState } from 'react';

const { Panel } = Collapse;

function CustomPanel({form, fields, label, values, hidePercent, ...restProps}) {
  const [ percent, setPercent ] = useState(0)

  useLayoutEffect(() => {
    let filled = 0;

    let totalFields = 0
    fields.map((item) => item.name && totalFields++)
    let percent = 0
    if(fields){
      fields.map((field) => {
        if(values[`${field.name}`]){
          filled += 1
        }
      })
      percent = parseInt((filled * 100)/totalFields);
    }
    setPercent(percent)
  },[fields, values])

  return (
    <Panel key={restProps.panelKey} {...restProps} header={<span>{label} { hidePercent ? "" : `- ${percent}%`}</span>}>
      {restProps.children}
    </Panel>
  )
}


function FormCollapse({form, ...restProps}) {
  return (
    <Collapse {...restProps}>
      {restProps.children}
    </Collapse>
  )
}

FormCollapse.CustomPanel = CustomPanel

export default FormCollapse