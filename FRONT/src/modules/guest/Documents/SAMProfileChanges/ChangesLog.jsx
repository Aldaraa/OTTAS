import { Table } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useContext } from 'react'

function ChangesLog({data}) {
  const { state } = useContext(AuthContext)

  const renderValue = (name, value) => {
    switch (name) {
      case 'Dob': return value ? dayjs(value).format('YYYY-MM-DD') : ''
      case 'CommenceDate': return value ? dayjs(value).format('YYYY-MM-DD') : ''
      case 'Gender': return value == '0' ? 'Female' : 'Male'
      case 'EmployerId': return state.referData.employers.find((item) => value)?.label
      case 'LocationId': return state.referData.locations.find((item) => value)?.label
      case 'NationalityId': return state.referData.nationalities.find((item) => value)?.label
      case 'PeopleTypeId': return state.referData.peopleTypes.find((item) => value)?.label
      case 'PositionId': return state.referData.positions.find((item) => value)?.label
      case 'RosterId': return state.referData.rosters.find((item) => value)?.label
      case 'StateId': return state.referData.states.find((item) => value)?.label
      case 'CostCodeId': return state.referData.costCodes.find((item) => value)?.label
      case 'DepartmentId': return state.referData.departments.find((item) => value)?.Name
      default: return value
    }
  }

  const column = [
    {
      label: 'Field Name',
      name: 'FieldName',
    },
    {
      label: 'Old Value',
      name: 'OldValue',
      cellRender: (e) => (
        <div>{renderValue(e.data.FieldName, e.data.OldValue)}</div>
      )
    },
    {
      label: 'New Value',
      name: 'NewValue',
      cellRender: (e) => (
        <div>{renderValue(e.data.FieldName, e.data.NewValue)}</div>
      )
    },
  ]
  return (
    <Table
      data={data}
      columns={column}
      allowColumnReordering={false}
      containerClass='shadow-none'
      keyExpr='FieldName'
      pager={{pageSize: [100]}}
    />
  )
}

export default ChangesLog