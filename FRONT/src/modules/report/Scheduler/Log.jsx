import { Tag } from 'antd'
import { Modal, Table } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import { reportInstance } from 'utils/axios'

const statusColor = {
  'Success': 'success',
  'Error': 'error',
}

const columns = [
  {
    label: 'Execute Date',
    name: 'ExecuteDate',
    cellRender: ({value}) => (
      <div>{value ? dayjs(value).format('YYYY-MM-DD HH:mm') : null}</div>
    )
  },
  {
    label: 'Status',
    name: 'ExecuteStatus',
    width: 150,
    cellRender: ({value}) => (
      <Tag color={statusColor[value]}>{value}</Tag>
    )
  },
  {
    label: 'Description',
    name: 'Descr',
  },
]

function Log({show, onCancel, data}) {
  const [ logData, setLogData ] = useState([])
  const [ loading, setLoading ] = useState(false)

  useEffect(() => {
    if(data){
      getLogData()
    }
  },[data])

  const getLogData = () => {
    setLoading(true)
    reportInstance({
      method: 'get',
      url: `reportjob/log/${data.Id}`
    }).then((res) => {
      setLogData(res.data)
    }).catch((err) => {

    }).then(() => {
      setLoading(false)
    })
  }
  return (
    <Modal open={show} onCancel={onCancel} title={<div>{data?.Code} <span className='text-md text-gray-400 font-normal'>( last 30 days log )</span></div>} width={700}>
      <div className='pb-3 grid grid-cols-12 gap-5'>
        <table className='text-xs col-span-6'>
          <tbody>
            <tr>
              <td className='w-[120px] text-gray-500'>Template: </td>
              <td>{data?.ReportTemplateName}</td>
            </tr>
            <tr>
              <td className='text-gray-500'>Schedule Type:</td>
              <td>{data?.ScheduleType}</td>
            </tr>
          </tbody>
        </table>
        <table className='text-xs col-span-6'>
          <tbody>
            <tr>
              <td className='w-[120px] text-gray-500'>Start Date: </td>
              <td>{data?.StartDate ? dayjs(data?.StartDate).format('YYYY-MM-DD HH:mm') : null}</td>
            </tr>
            <tr>
              <td className='text-gray-500'>End Date:</td>
              <td>{data?.EndDate ? dayjs(data?.EndDate).format('YYYY-MM-DD HH:mm') : null}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <Table 
        columns={columns}
        data={logData}
        keyExpr={'Id'}
        containerClass='shadow-none px-0'
        tableClass='max-h-[calc(100vh-280px)] border-t'
      />
    </Modal>
  )
}

export default Log