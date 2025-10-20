import { CloseCircleTwoTone } from '@ant-design/icons'
import axios from 'axios'
import { Button, Table } from 'components'
import dayjs from 'dayjs'
import React, { useMemo, useState } from 'react'
import WarningDetail from '../WarningDetail'

function Changing({data, onChangeData, handleProcess, onCancel, processLoading, handleRemoveFromModal, form}) {
  const [ actionLoading, setActionLoading ] = useState(false)

  const handleOnSiteDelete = (row) => {
    setActionLoading(row.EmpId)
    axios({
      method: 'delete',
      url: `tas/employee/deletetransport/${row.EmpId}/${dayjs(row.OnsiteData.EventDate).format('YYYY-MM-DD')}`
    }).then(() => {
      onChangeData(data.filter((item) => item.EmpId !== row.EmpId))
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const disabledStatusOnSiteDeleteAllBtn = useMemo(() => {
    const onSiteEmployeeList = data.filter((item) => item.EmpOnSiteStatus)
    return onSiteEmployeeList.length === 0
  },[data])

  const isLoadingDeleteAllBtn = useMemo(() => {
    return (typeof actionLoading === 'boolean') && (actionLoading === true)
  },[actionLoading])

  const errorColumns = [
    {
      label: 'Fullname',
      name: 'Fullname',
      alignment: 'left',
      width: '200px',
      // cellRender: (e) => (
      //   <Link to={`/tas/people/search/${e.data.Id}`}>{e.value}</Link>
      // )
    },
    {
      label: 'On Site Status',
      name: 'EmpOnSiteStatus',
      alignment: 'center',
      cellRender:(e) => (
        <div>{e.value ? <CloseCircleTwoTone twoToneColor='red' style={{fontSize: '16px'}}/> : ''}</div>
      )
    },
    {
      label: 'Off Site Status',
      name: 'EmpOffSiteStatus',
      alignment: 'center',
      cellRender:(e) => (
        <div>{e.value ? <CloseCircleTwoTone twoToneColor='red' style={{fontSize: '16px'}}/> : ''}</div>
      )
    },
    {
      label: '',
      name: 'action',
      alignment: 'right',
      width: 220,
      cellRender: (e) => (
        <div className='flex gap-3'>
          {
            e.data.EmpOnSiteStatus ?
            <Button
              type='danger'
              onClick={() => handleOnSiteDelete(e.data)}
              disabled={actionLoading}
              loading={e.data.EmpId === actionLoading}
            >
              On Site Delete
            </Button>
            : null
          }
          <button type='button' className='dlt-button' onClick={() => handleRemoveFromModal(e.data)} >Remove</button>
        </div>
      )
    },
  ]

  const handleDeleteAllOnSite = () => {
    setActionLoading(true)
    const onSiteEmployeeList = data.filter((item) => item.EmpOnSiteStatus)
    const ids = onSiteEmployeeList.map((item) => item.EmpId)
    const onsiteDate = form.getFieldValue('startDate').format('YYYY-MM-DD')
    axios({
      method: 'delete',
      url: `tas/employee/deletetransportbulk`,
      data: {
        employeeIds: ids,
        onsiteDate: onsiteDate,
      }
    }).then(() => {
      onChangeData(data.filter((item) => !ids.includes(item.EmpId)))
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  return (
    <>
      <div className='mt-0 text-xs'>
        <div className='flex justify-end items-center mb-2'>
          <Button type='danger' onClick={handleDeleteAllOnSite} disabled={disabledStatusOnSiteDeleteAllBtn} loading={isLoadingDeleteAllBtn}>
            On Site Delete all
          </Button>
        </div>
        <Table
          data={data}
          columns={errorColumns}
          containerClass='shadow-none border overflow-hidden' 
          renderDetail={{enabled: true, component: WarningDetail}}
          pager={data?.length > 20}
        />
      </div>
      <div className='flex justify-end gap-5 items-center mt-3'>
        <Button 
          type='primary'
          loading={processLoading}
          onClick={() => handleProcess()}
        >
          Process
        </Button>
        <Button  
          onClick={onCancel}
          disabled={processLoading}
        >
          Cancel
        </Button>
      </div>
    </>
  )
}

export default Changing