import { FileTextOutlined, LoadingOutlined, ReloadOutlined } from '@ant-design/icons'
import { Tag } from 'antd'
import axios from 'axios'
import { Button, Modal, Table, Tooltip } from 'components'
import dayjs from 'dayjs'
import React, { useCallback, useEffect, useState } from 'react'

const actions = {
  Deleted: 'error',
  Updated: 'blue',
  Created: 'success'
}

const GroupLog = React.memo(({groupId, employeeId, name=''}) => {
  const [ loading, setLoading ] = useState(true)
  const [ data, setData ] = useState([])
  const [ show, setShow ] = useState(false)

  useEffect(() => {
    getLogData()
  },[])

  const getLogData = useCallback(() => {
    axios({
      method: 'get',
      url: `tas/audit/groupmembers/${employeeId}/${groupId}`,
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  },[groupId, employeeId])

  const onClick = useCallback(() => {
    setShow(true)
  },[])

  const columns = [
    {
      label: 'Data',
      name: 'Description',
    },
    {
      label: 'Date',
      name: 'CreatedDate',
      cellRender: ({value}) => (
        <div style={{wordSpacing: 4}}>{dayjs(value).format(`YYYY-MM-DD HH:mm`)}</div>
      )
    },
    {
      label: 'Action',
      name: 'Action',
      cellRender: ({value}) => (
        <Tag color={actions[value]}>{value}</Tag>
        // <div style={{wordSpacing: 4}}>{dayjs(value).format(`YYYY-MM-DD HH:mm`)}</div>
      )
    },
    {
      label: 'User',
      name: 'ChangedUser',
    },
  ]

  return (
    <div>
      <Tooltip title='History'>
        <Button onClick={onClick} icon={<FileTextOutlined />} className='py-2'></Button>
      </Tooltip>
      <Modal
        title={name}
        open={show}
        onCancel={() => setShow(false)}
        width={700}
      >
        {
          loading ?
          <LoadingOutlined/> :
          <Table 
            title={<div className='flex justify-between border-b py-1'>
              <div>Changes</div>
              <Tooltip title='Reload'>
                <Button icon={<ReloadOutlined/>} onClick={getLogData}/>
              </Tooltip>
              </div>
            }
            columns={columns}
            data={data}
            containerClass='border shadow-none'
            pager={data.length > 100}
          />
        }
      </Modal>
    </div>
  )
}, (prev, next) => (prev.groupId === next.groupId) && (prev.employeeId === next.employeeId) && (prev.name === next.name))

export default GroupLog