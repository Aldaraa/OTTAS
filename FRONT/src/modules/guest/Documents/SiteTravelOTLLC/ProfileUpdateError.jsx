import { CloseCircleTwoTone } from '@ant-design/icons'
import { Table } from 'components'
import React from 'react'
import { Link } from 'react-router-dom'

function ProfileUpdateError({errordata}) {

  const errorColumns = [
    {
      label: '',
      name: 'FullName',
      alignment:'left',
      wrap: true,
      cellRender: (e) => (
        <div className='flex items-center'>
          <Link to={`/tas/people/search/${e.data?.EmployeeId}`}>
            <span className='text-blue-500 hover:underline whitespace-normal'>{e.value} ({e.data?.EmployeeId})</span>
          </Link>
        </div>
      )
    },
    {
      label: 'ADAccount',
      name: 'ADAccount',
      alignment:'left',
      // width: '50px',
      cellRender: (e) => (
        <div>
          {typeof e.value === 'boolean' ? e.value ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
        </div>
      )
    },
    {
      label: 'SAP ID',
      name: 'SAPID',
      alignment:'center',
      width: '60px',
      cellRender: (e) => (
        <div>
          {typeof e.value === 'boolean' ? e.value ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
        </div>
      )
    },
    {
      label: 'Mobile',
      name: 'Mobile',
      alignment:'center',
      width: '60px',
      cellRender: (e) => (
        <div>
          {typeof e.value === 'boolean' ? e.value ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
        </div>
      )
    },
    {
      label: 'NRN',
      name: 'NRN',
      alignment:'center',
      width: '50px',
      cellRender: (e) => (
        <div>
          {typeof e.value === 'boolean' ? e.value ? '' : <CloseCircleTwoTone twoToneColor='#eb2f96' style={{fontSize: '16px'}}/> : ''}
        </div>
      )
    },
  ]

  return (
    <Table data={errordata} columns={errorColumns} containerClass='shadow-none border' pager={false}/>
  )
}

export default ProfileUpdateError