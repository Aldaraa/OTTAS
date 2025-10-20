import { ReloadOutlined, StopOutlined } from '@ant-design/icons'
import { Drawer } from 'antd'
import { Button, Table } from 'components'
import dayjs from 'dayjs'
import { Popup } from 'devextreme-react'
import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { reportInstance } from 'utils/axios'

function RunningReport({open, onClose, editData}) {
  const [ runningReports, setRunningReports ] = useState([])
  const [ selectedStopSession, setSelectedStopSession ] = useState(null)
  const [ showStopPopup, setShowStopPopup ] = useState(false)
  const [ sessionLoading, setSessionLoading ] = useState(false)

  useEffect(() => {
    getRunningReports()
  },[])

  const getRunningReports = () => {
    reportInstance({
      method: 'get',
      url: 'reportjob/sessions',
    }).then((res) => {
      setRunningReports(res.data)
    })
  }

  const handleStopSession = useCallback((row) => {
    setSelectedStopSession(row)
    setShowStopPopup(true)
  },[])

  const runningColumns = useMemo(() => [
    {
      label: 'Name',
      name: 'SessionName',
      alignment: 'left',
    },
    {
      label: 'Name',
      name: 'CreatedDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD HH:mm')}</div>
      )
    },
    {
      label: '',
      name: 'action',
      alignment: 'left',
      cellRender: (e) => (
        <button className='dlt-button' onClick={() => handleStopSession(e.data)}><StopOutlined/> Stop process</button>
      )
    },
  ],[])

  const stopSession = () => {
    setSessionLoading(true)
    reportInstance({
      method: 'post',
      url: 'reportjob/killsession',
      data: {
        killId: selectedStopSession.KillId
      }
    }).then((res) => {
      getRunningReports()
      setShowStopPopup(false)
    }).catch(() => {

    }).then(() => setSessionLoading(false))
  }
  return (
    <>
      <Drawer
        title='Running reports'
        open={open}
        onClose={onClose}
        width={600}
        extra={<Button icon={<ReloadOutlined/>}
        onClick={getRunningReports}>Refresh</Button>}
      >
        <Table
          columns={runningColumns}
          data={runningReports}
          containerClass='shadow-none border'
        />
      </Drawer>
      <Popup
        visible={showStopPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to stop this process?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={stopSession} loading={sessionLoading}>Yes</Button>
          <Button onClick={() => setShowStopPopup(false)} disabled={sessionLoading}>No</Button>
        </div>
      </Popup>
    </>
  )
}

export default RunningReport