import { Checkbox } from 'antd'
import { Modal, Tooltip } from 'components'
import React, { useCallback, useContext, useState } from 'react'
import ls from 'utils/ls';

const defaultFields = ['Lastname', 'Firstname', 'SAPID', 'Id', 'DepartmentId', 'EmployerId', 'Active']
const allFields = [
  "Firstname",
  "Lastname",
  "SAPID",
  'Id',
  "DepartmentId",
  "EmployerId",
  "PositionId",
  "roomNumber",
  "CampId",
  "LocationId",
  "FlightGroupMasterId",
  "RoomTypeId",
  "NRN",
  "PeopleTypeId",
  "CostCodeId",
  "RosterId",
  "futureBooking",
  "group",
  "hasRoom",
  "Mobile",
  "Active",
]

const filterFields = [
    {
      label: 'Firstname',
      value: 'Firstname',
    },
    {
      label: 'Lastname',
      value: 'Lastname',
    },
    {
      label: 'Person ID #',
      value: 'Id',
    },
    {
      label: 'SAP ID #',
      value: 'SAPID',
    },
    {
      label: 'Department',
      value: 'DepartmentId',
    },
    {
      label: 'Employer',
      value: 'EmployerId',
    },
    {
      label: 'Position',
      value: 'PositionId',
    },
    {
      label: 'Room number',
      value: 'roomNumber',
    },
    {
      label: 'Camp',
      value: 'CampId',
    },
    {
      label: 'Location',
      value: 'LocationId',
    },
    {
      label: 'Transport Group',
      value: 'FlightGroupMasterId',
    },
    {
      label: 'Room Type',
      value: 'RoomTypeId',
    },
    {
      label: 'NRN',
      value: 'NRN',
    },
    {
      label: 'Resource type',
      value: 'PeopleTypeId',
    },
    {
      label: 'Cost Code',
      value: 'CostCodeId',
    },
    {
      label: 'Roster',
      value: 'RosterId',
    },
    {
      label: 'Future Bookings',
      value: 'futureBooking',
    },
    {
      label: 'Group',
      value: 'group',
    },
    {
      label: 'Has room',
      value: 'hasRoom',
    },
    {
      label: 'Mobile',
      value: 'Mobile',
    },
    {
      label: '# Active only',
      value: 'Active',
    },
]

function arrayEquals(a, b) {
  return Array.isArray(a) &&
      Array.isArray(b) &&
      a.length === b.length &&
      a.every((val, index) => val === b[index]);
}

function ColumnFilter({open, onCancel, selectedFields, setSelectedFields, profileFields, hiddenFields=[]}) {
  const [ indeterminate, setIndeterminate] = useState(true);
  const [ checkAll, setCheckAll] = useState([]);

  const onCheckAllChange = useCallback((e) => {
    let selectedKeys = e.target.checked ? allFields : []
    ls.set('sFields', selectedKeys);
    setSelectedFields(selectedKeys);
    setIndeterminate(false);
    setCheckAll(e.target.checked);
  },[allFields]);
  
  const onCheckDefaultChange = useCallback((e) => {
    let selectedKeys = e.target.checked ? defaultFields : []
    ls.set('sFields', selectedKeys);
    setSelectedFields(selectedKeys);
    setIndeterminate(selectedKeys.length < allFields.length);
  },[defaultFields, allFields])

  const onCheckChange = useCallback((list) => {
    let allFields = filterFields?.map((item) => item.value)
    ls.set('sFields', list);
    setSelectedFields(list);
    setIndeterminate(!!list.length && list.length < allFields.length);
    setCheckAll(list.length === allFields.length);
  },[filterFields])

  return (
    <div>
      <Modal open={open} onCancel={onCancel} title='Filter fields'>
        <div className='pb-2 mb-2 border-b'>
          <Checkbox key={'all'} indeterminate={indeterminate} checked={checkAll} onChange={onCheckAllChange}>Select All</Checkbox>
          <Checkbox key={'default'} checked={arrayEquals(defaultFields, selectedFields)} onChange={onCheckDefaultChange}>
            Default
          </Checkbox>
        </div>
        <Checkbox.Group style={{width:'100%'}} value={selectedFields} onChange={onCheckChange}>
          <div className='grid grid-cols-12 gap-x-5'>
            {filterFields?.map((item, i) => (
              <div className='col-span-6' key={`check-item-${i}`}>
                <Checkbox value={item.value}>{profileFields[item.value] ? profileFields[item.value].Label : item.label}</Checkbox>
              </div>
            ))}
          </div>
        </Checkbox.Group>
      </Modal>
    </div>
  )
}

export default ColumnFilter