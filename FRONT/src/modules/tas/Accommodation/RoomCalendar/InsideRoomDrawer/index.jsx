import React, { useEffect, useMemo, useRef, useState } from 'react'
import {lockedColumns, ownerColumns, roomEmpColumns} from './columns'
import axios from 'axios'
import { Accordion, CustomTable, Table } from 'components'
import { Drawer } from 'antd'
import { twMerge } from 'tailwind-merge'
import { LoadPanel } from 'devextreme-react'

const InsideRoomDrawer = React.memo(({open, handleCloseDrawer, selectedDate, store}) => {
  const [ ownerAndLocked, setOwnerAndLocked ] = useState(null)
  const [ roomEmpData, setRoomEmpData ] = useState([])
  const [ profileLoading, setProfileLoading ] = useState(false)
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(100)
  const [ loading, setLoading ] = useState(false)
  const ownerRef = useRef(null)

  useEffect(() => {
    if(selectedDate){
      setProfileLoading(true)
      axios({
        method: 'post',
        url: 'tas/room/ownerandlockdateprofile',
        data: {
          currentDate: selectedDate.date,
          roomId: selectedDate?.roomId
        }
      }).then((res) => {
        setOwnerAndLocked(res.data)
      }).catch((err) => {

      }).then(() => setProfileLoading(false))
    }
  },[selectedDate])

  useEffect(() => {
    if(selectedDate){
      getRoomEmployees()
    }
  },[selectedDate, pageIndex])

  const getRoomEmployees = () => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/room/dateprofile',
      data: {
        pageIndex: pageIndex+1,
        pageSize: pageSize,
        model: {
          currentDate: selectedDate.date,
          roomId: selectedDate?.roomId
        }
      }
    }).then((res) => {
      setRoomEmpData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).catch((err) => {
  
    }).then(() => setLoading(false))
  }

  const isVirtualRoom = useMemo(() => {
    const virtualRooms = [
      store.referData?.noRoomId?.Id,
      store.referData?.noRoomId?.NoAccommdationId,
      store.referData?.noRoomId?.KhanbogdRoomId
    ]

    return virtualRooms.includes(selectedDate?.roomId)

},[store, selectedDate])

  return (
    <Drawer
      title={<div className='flex justify-between'>{selectedDate?.roomNumber} <span className='text-gray-400'>{selectedDate?.date}</span></div>}
      open={open}
      footer={false}
      onClose={handleCloseDrawer}
      width={1000}
      destroyOnClose={false}
    >
      <div className='flex flex-col'>
        <Accordion
          title={<div className='py-1 font-bold'>Owners ({ownerAndLocked?.OwnerFutureBooking?.length })</div>}
          className={twMerge('mb-4 border', isVirtualRoom ? 'hidden' : 'block' )}
        >
          <Table 
            ref={ownerRef}
            data={ownerAndLocked?.OwnerFutureBooking ? ownerAndLocked?.OwnerFutureBooking : []}
            columns={ownerColumns}
            pager={ownerAndLocked?.OwnerFutureBooking > 20}
            loading={profileLoading}
            keyExpr={'Id'}
            containerClass='shadow-none border-none rounded-none'
            tableClass=''
          />
        </Accordion>
        <Accordion
          title={<div className='py-1 font-bold'>On Site Employees ({totalCount})</div>}
          className={twMerge('border', isVirtualRoom ? 'mb-0' : 'mb-4')}
        >
          <div id='on-site'>
            <CustomTable
              data={roomEmpData}
              keyExpr='Id'
              columns={roomEmpColumns}
              onChangePageSize={(e) => setPageSize(e)}
              onChangePageIndex={(e) => setPageIndex(e)}
              pageSize={pageSize}
              pageIndex={pageIndex}
              totalCount={totalCount}
              pagerPosition='bottom'
              showColumnLines={false}
              pageSizeDisabled={true}
              wordWrapEnabled={true}
              isPagination={totalCount > 100}
              containerClass={'shadow-none rounded px-0 border-none'}
              tableClass={twMerge(isVirtualRoom ? 'max-h-[calc(100vh-195px)] rounded-none border-none' : 'max-h-[500px]')}
            />
          </div>
          <LoadPanel visible={loading} position={{of: '#on-site'}}/>
        </Accordion>
        <Accordion
          title={<div className='py-1 font-bold'>Locked Employee in the room ({ownerAndLocked?.LockedEmployees?.length })</div>} 
          className={twMerge('mb-4 border', isVirtualRoom ? 'hidden' : 'block' )}
        >
          <Table
            data={ownerAndLocked?.LockedEmployees ? ownerAndLocked?.LockedEmployees : []}
            columns={lockedColumns}
            pager={ownerAndLocked?.OwnerFutureBooking > 20}
            loading={profileLoading}
            keyExpr={'Id'}
            containerClass='shadow-none border-none mb-4'
            tableClass='border-none'
          />
        </Accordion>
      </div>
    </Drawer>
  )
}, (prevProps, currentProps) => prevProps.open === currentProps.open && prevProps.selectedDate === currentProps.selectedDate)

export default InsideRoomDrawer