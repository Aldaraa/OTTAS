import { Modal, Table } from 'components'
import dayjs from 'dayjs'
import React from 'react'
import { Link } from 'react-router-dom'

const getUrl = (data) => {
  if(data.DocumentType === 'Non Site Travel'){
    return `/request/task/nonsitetravel/${data.Id}`
  }
  else if(data.DocumentType === 'Profile Changes'){
    return `/request/task/profilechanges/${data.Id}`
  }
  else if(data.DocumentType === 'De Mobilisation'){
    return `/request/task/de-mobilisation/${data.Id}`
  }
  else if(data.DocumentType === 'Site Travel'){
    if(data.DocumentTag === 'ADD'){
      return `/request/task/sitetravel/addtravel/${data.Id}`
    }
    else if(data.DocumentTag === "RESCHEDULE"){
      return `/request/task/sitetravel/reschedule/${data.Id}`
    }
    else if(data.DocumentTag === "REMOVE"){
     return `/request/task/sitetravel/remove/${data.Id}`
    }
  }
}

const duplicatedColumns = [
  {
    label: '#',
    name: 'Id',
    width: 65,
    cellRender: (e) => (
      <div className='flex items-center text-blue-500 hover:underline' style={{ textAlign: 'left' }}>
        <Link to={getUrl(e.data)} target='_blank'>
          {e.data.Id}
        </Link>
      </div>
    )
  },
  {
    label: 'CurrentAction',
    name: 'CurrentAction',
    width: 110,
  },
  {
    label: 'CreatedAt',
    name: 'CreatedAt',
    width: 180,
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm:ss')}</div>
    )
  },
  {
    label: 'AssignedEmployee',
    name: 'AssignedEmployee',
  },
  {
    label: 'Description',
    name: 'Description',
  },
  {
    label: 'Document Type',
    name: 'DocumentType',
    width: 180,
    cellRender: (e) => (
      <div>{e.value}{e.data.DocumentTag ? ` - ${e.data.DocumentTag}` : ''}</div>
    )
  }
]



function DuplicatedDocument({open=false, data=[], onCancel}) {
  return (
    <Modal
      width={1200} 
      open={open}
      title={'Request Duplicated'} 
      // forceRender={true}
      onCancel={onCancel}
    >
      <div>
        <Table
          data={data}
          columns={duplicatedColumns}
          containerClass='shadow-none border'
          pager={false}
        />
      </div>
    </Modal>
  )
}

export default DuplicatedDocument