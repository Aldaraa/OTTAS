import { Table } from 'components'
import React from 'react'

function SkippedEmployees({data}) {
  const columns = [
    {
      label: 'Person #',
      name: 'SAPID',
      alignment: 'left'
    },
    {
      label: 'SAP ID #',
      name: 'SAPID',
      alignment: 'left'
    },
    {
      label: 'Full Name',
      name: 'FullName',
    },
  ]
  return (
    <div>
      <Table
        data={data}
        columns={columns}
        keyExpr='EmployeeId'
      />
    </div>
  )
}

export default SkippedEmployees