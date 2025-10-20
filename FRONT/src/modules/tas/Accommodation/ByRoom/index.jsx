import { Table, Button, Modal, RoomSearch, Tooltip, CustomTable, DepartmentTooltip } from 'components'
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { DatePicker } from 'antd'
import axios from 'axios'
import { SearchOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { Link } from 'react-router-dom'
import { FaFemale, FaMale } from 'react-icons/fa'

const { RangePicker } = DatePicker;

function ByRoom() {
  const [ data, setData ] = useState([])
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ selectedDate, setSelectedDate ] = useState([dayjs().subtract(15, 'd'), dayjs().add(15, 'd')])
  const [ selectedRoom, setSelectedRoom ] = useState(null)
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(100)

  const dataGrid = useRef(null)

  useEffect(() => {
    if(data.length > 0){
      handleSearch()
    }
  },[pageIndex])

  const columns = useMemo(() => [
    {
      label: '#',
      name: 'EmployeeId',
      width: '60px',
      alignment: 'left'
    },
    {
      label: '', 
      name: 'RoomOwner', 
      alignment: 'center', 
      width: '20px',
      cellRender: (e) => (
        <span className='text-[12px]'>
          {
            e.value ? 
            <i className="dx-icon-home text-green-500"></i> 
            :
            <Tooltip
              title={e.data.RoomNumber && <span className='text-blue-500 hover:underline'>{e.data?.RoomNumber}</span>} 
              color='white' 
              overlayInnerStyle={{color: 'black', fontSize: '12px'}}
            >
              <i className="dx-icon-home text-gray-400 text-[14px]"></i>
            </Tooltip> 
          }
        </span>
      )
    },
    {
      label: 'Details',
      name: 'FullName',
      alignment: 'left',
      cellRender: (e) => (
        <div className='flex items-center gap-3 group'>
          <div id={`emp${e.data.Id}`} style={{fontSize: '12px'}} className='flex items-center gap-1'>
            {e.data.Gender === 1 ? <FaMale size={14} className='text-blue-600'/> : <FaFemale size={14} className='text-pink-500'/>} 
            <Link to={`/tas/byroom/${e.data.EmployeeId}/roombooking?startDate=${dayjs(selectedDate[0]).format('YYYY-MM-DD')}&endDate=${dayjs(selectedDate[1]).format('YYYY-MM-DD')}`}>
              <span className='hover:underline text-blue-500'>{e.value}</span>
            </Link>
          </div>
        </div>
      )
    },
    {
      label: 'SAPID', 
      name: 'SAPID', 
      alignment: 'left', 
      width: '80px',
      cellRender: (e) => (
        <span className='text-[12px]'>
          {e.value}
        </span>
      )
    },
    {
      label: 'Department', 
      name: 'DepartmentName', 
      alignment: 'left', 
      cellRender: (e) => (
        <DepartmentTooltip id={e.data?.DepartmentId} trigger='click' showStatus={false}>
          <span className='text-[12px] text-blue-500 cursor-pointer hover:text-blue-400 transition-all'>{e.value}</span>
        </DepartmentTooltip>
      )
    },
    {
      label: 'Employer', 
      name: 'EmployerName', 
      alignment: 'left', 
      cellRender: (e) => (
        <span className='text-[12px]'>
          {e.value}
        </span>
      )
    },
    {
      label: 'Res.Type', 
      name: 'PeopleTypeCode', 
      alignment: 'left', 
      cellRender: (e) => (
        <span className='text-[12px]'>
          {e.value}
        </span>
      )
    },
    {
      label: 'Mobile', 
      name: 'PersonalMobile', 
      alignment: 'left', 
      cellRender: (e) => (
        <span className='text-[12px]'>{e.value}</span>
      )
    },
    {
      label: 'Date In',
      name: 'DateIn',
      // groupIndex: 0,
      defaultSortOrder: 'asc',
      cellRender: (e) => (
        <div className='text-xs'>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Last Night',
      name: 'LastNight',
      cellRender: (e) => (
        <div className='text-xs'>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Days',
      name: 'Days',
      alignment: 'center',
      width: '50px',
      cellRender: (e) => (
        <div className='text-xs'>{e.value}</div>
      )
    },
    {
      label: '',
      name: 'action',
      width: '130px',
      cellRender: (e) => (
        <div>
          <Link to={`/tas/roomcalendar/${e.data.RoomId}/${dayjs(e.data.DateIn).startOf('month').format('YYYY-MM-DD')}?employeeId=${e.data.EmployeeId}`}>
            <button type='button' className='edit-button'>Room Calendar</button>
          </Link>
        </div>
      )
    },
  ],[selectedDate])

  const handleSelectRoom = useCallback((data) => {
    setSelectedRoom(data); 
    setShowPopup(false)
  }, [])

  const handleSearch = () => {
    dataGrid.current?.instance.beginCustomLoading()
    setSearchLoading(true)
    axios({
      method: 'post',
      url: `tas/employeestatus/roombookingbyroom`,
      data: {
        pageIndex: pageIndex,
        pageSize: pageSize,
        model: {
          roomId: selectedRoom.Id,
          startDate: dayjs(selectedDate[0]).format('YYYY-MM-DD'),
          endDate: dayjs(selectedDate[1]).format('YYYY-MM-DD'),
        }
      }
    }).then((res) => {
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).catch((err) => {

    }).then(() => {
      dataGrid.current?.instance.endCustomLoading()
      setSearchLoading(false)
    })
  }

  return (
    <div className='rounded-ot bg-white px-3 pt-2 shadow-md'>
      <div className='text-lg font-bold mb-2'>View Room Booking by Room</div>
      <div className='flex gap-5 mb-4'>
        <div className='flex'>
          <div className='min-w-[300px] w-auto border border-[#d9d9d9] flex items-center px-3 bg-gray-50 text-gray-600 border-r-0 rounded-l-[6px]'>
            {selectedRoom && 
              <div className='flex gap-3'>
                <div>{selectedRoom?.Number} - {selectedRoom?.BedCount}</div>
                <div>/{selectedRoom?.RoomTypeName}/</div>
                <div>/{selectedRoom?.CampName}/</div>
              </div>
            }
          </div>
          <Button className='rounded-l-none border' onClick={() => setShowPopup(true)}>
            Select Room
          </Button>
        </div>
        <RangePicker 
          value={selectedDate}
          onChange={(e) => setSelectedDate(e)}
        />
        <Button 
          onClick={handleSearch} 
          loading={searchLoading} 
          icon={<SearchOutlined/>}
          disabled={!selectedRoom || (!selectedDate[0] && !selectedDate[1])}
        >
          Search
        </Button>
      </div>
      <CustomTable
        ref={dataGrid}
        data={data}
        keyExpr='Id'
        columns={columns}
        onChangePageSize={(e) => setPageSize(e)}
        onChangePageIndex={(e) => setPageIndex(e)}
        pageSize={pageSize}
        pageIndex={pageIndex}
        allowColumnReordering={false}
        showColumnLines={false}
        totalCount={totalCount}
        isPagination={true}
        containerClass='shadow-none pl-0 pr-0'
        pagerPosition='bottom'
        tableClass='max-h-[calc(100vh-240px)]'
        isGrouping={false}
        searchPanel={{enabled: true}}
        hoverStateEnabled={true}
        wordWrapEnabled={true}
      />
      <Modal
        destroyOnClose={false}
        open={showPopup} 
        onCancel={() => setShowPopup(false)} 
        footer={null} 
        title='Room' 
        width={900}
      >
        <RoomSearch onSelect={handleSelectRoom}/>
      </Modal>
    </div>
  )
}

export default ByRoom