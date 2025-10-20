import { Form, Table, Button, Modal, RoomSearch, Tooltip, Pagination, ExpandRowRoomDetail, DepartmentTooltip } from 'components'
import { Popup } from 'devextreme-react'
import { LoadPanel } from 'devextreme-react/load-panel'
import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form as AntForm, DatePicker, Drawer, Input } from 'antd'
import axios from 'axios'
import { LeftOutlined, RightOutlined, SearchOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import duration from 'dayjs/plugin/duration'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'
import CalendarTable from 'components/CalendarTable'
import { BsBriefcaseFill } from 'react-icons/bs'
import { FaFemale, FaMale } from 'react-icons/fa'
import hexToHSL from 'utils/hexToHSL'
import generateColor from 'utils/generateColor'
import getDaysArrayBetweenMonths from 'utils/getDaysArray'
import useQuery from 'utils/useQuery'
import OnSitePeople from './OnSitePeople'
import { twMerge } from 'tailwind-merge'
import ownerColumns from './ownerColumns'
import AuditModal from './AuditModal'

dayjs.extend(duration)
const dateFormat = 'YYYY-MM-DD'

function getDatesBetween(startDate, endDate) {
  const currentDate = new Date(startDate);
  const lastDate = new Date(endDate);
  const dates = [];
  while (currentDate <= lastDate) {
    dates.push(dayjs(currentDate).format('YYYY-MM-DD'));
    currentDate.setDate(currentDate.getDate() + 1);
  }
  return dates;
}

function RoomCalendarDetail() {
  const { roomId, startDate } = useParams()
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ selectedItems, setSelectedItems ] = useState([])
  const [ selectedRoom, setSelectedRoom ] = useState(null)
  const [ selectedDate, setSelectedDate ] = useState(startDate ? dayjs(startDate).startOf('month') : null)
  const [ showModal, setShowModal ] = useState(false)
  const [ toChangeRoom, setToChangeRoom ] = useState(null)
  const [ questionModal, setQuestionModal ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ selectedData, setSelectedData ] = useState(null)
  const [ availableRooms, setAvailableRooms ] = useState([])
  const [ rLoading, setRLoading ] = useState(false) 
  const [ empRoomDetailData, setEmpRoomDetailData ] = useState([])
  const [ detailLoading, setDetailLoading ] = useState(false)
  const [ searchable, setSearchable ] = useState(false)
  const [ showDrawer, setShowDrawer ] = useState(false)
  const [ selectedCalendarDate, setSelectedCalendarDate ] = useState(null)
  const [ showRoomChangeResponse, setShowRoomChangeResponse ] = useState(false)
  const [ changeResponse, setChangeResponse ] = useState(null)
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(10)
  const [ employeeCalendarData, setEmployeeCalendarData ] = useState([])
  const [ showAudit, setShowAudit ] = useState(false)
  
  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { state, action } = useContext(AuthContext)
  const dataGrid = useRef(null)
  const navigate = useNavigate()
  const query = useQuery()
  const screenName = `roomDetail_${roomId}`

  const values = AntForm.useWatch([], form);

  useEffect(() => {
    if(values){
      if(values?.CampId || values?.RoomNumber){
        setSearchable(true)
      }else{
        setSearchable(false)
      }
    }
  }, [values]);

  useEffect(() => {
    const searchValue = query.get('employeeId') || query.get('empId')
    if(searchValue && selectedRoom){
      searchForm.setFieldValue('keyword', searchValue)
      // handleSearchByKeyword({keyword: searchValue})
    }
  },[query, selectedRoom])

  useEffect(() => {
    if(employeeCalendarData.length > 0){
      getRoomCalendarData()
    }
  },[pageIndex])

  useEffect(() => {
    if(state.connectionMultiViewer && state.userInfo){
      joinRoom()
      state.connectionMultiViewer.on('UsersInScreen', (res, users) => {
        let multiViewers = users.map((item) => ({
          name: item.userName.split('_')[0],
          role: item.userName.split('_')[1],
          color: generateColor()
        }))
        action.setMultiViewers(multiViewers.filter(user => user.name !== `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}`))
      });
    }

    return () => {
      state.connectionMultiViewer && leaveRoom()
    }
  },[state.connectionMultiViewer, state.userInfo])

  useEffect(() => {
    if(selectedDate && selectedRoom){
      handleSearch()
    }
  },[selectedDate, selectedRoom])
  
  useEffect(() => {
    if(roomId && startDate){
      setSelectedRoom(state.referData?.rooms?.find((room) => room.Id === parseInt(roomId)))
    }
  },[roomId, startDate, state.referData?.rooms])

  const joinRoom = () => {
    try {
      state.connectionMultiViewer?.invoke(
        'JoinScreen', 
        `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`,
        screenName
      )
    }
    catch(e) {
    }
  }

  const leaveRoom = () => {
    action.setMultiViewers([])
    state.connectionMultiViewer?.invoke(
      'LeaveScreen',
      `${state.userInfo?.Firstname} ${state.userInfo?.Lastname}_${state.userInfo?.Role}`,
      screenName
    )
  }

  const getEmployeeRoomDetail = useCallback(() => {
    if(selectedData){
      setDetailLoading(true)
      axios({
        method: 'get',
        url: `tas/room/employeeprofile/${selectedData.Id}`
      }).then((res) => {
        setEmpRoomDetailData(res.data)
        setShowModal(true)
      }).finally(() => setDetailLoading(false))
    }
  },[selectedData])

  const handleSelectRoom = useCallback((data) => {
    navigate(`/tas/roomcalendar/${data.Id}/${selectedDate.format('YYYY-MM-DD')}`)
    setShowPopup(false);
  },[])

  const handleSearch = () => {
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/room/getmonthroomdata/owner/${dayjs(selectedDate).format('YYYY-MM-DD')}/${selectedRoom.Id}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setSearchLoading(false))
    getRoomCalendarData()
  }

  const getRoomCalendarData = () => {
    setLoading(true)
    axios({
      method: 'post',
      url: `tas/room/getmonthroomdata`,
      data: {
        pageIndex: pageIndex,
        pageSize: pageSize,
        model: {
          currentDate: dayjs(selectedDate).format('YYYY-MM-DD'),
          roomId: selectedRoom.Id,
          keyword: searchForm.getFieldValue('keyword'),
        }
      }
    }).then((res) => {
      setEmployeeCalendarData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSearchByKeyword = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: `tas/room/getmonthroomdata`,
      data: {
        pageIndex: 0,
        pageSize: pageSize,
        model: {
          currentDate: dayjs(selectedDate).format('YYYY-MM-DD'),
          roomId: selectedRoom.Id,
          keyword: values.keyword,
        }
      }
    }).then((res) => {
      setEmployeeCalendarData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).catch((err) => {
    }).then(() => setLoading(false))
  }

  const renderAnotherRoom = useCallback((data, currentDate, rowData, event, selectedItems) => {
    if(data){
      const dayData = data[currentDate]
      if(dayData){
        return(
          <Tooltip 
            title={<Link to={`/tas/roomcalendar/${dayData.RoomId}/${startDate}?employeeId=${rowData?.Id}`}>
                <span className='text-blue-500 hover:underline'>{dayData.RoomNumber}</span>
              </Link>
            }
          >
            <button
              className='flex-1 flex items-center justify-center text-[13px] bg-gray-200 relative'
              onClick={()=> handleClickonCell(event.columnIndex, rowData.Id, event)}
            >
              {
                selectedItems?.find((item) => (selectedData.Id === event.data.Id) && (item.date === dayjs(selectedDate).date(event.columnIndex-6).format('YYYY-MM-DD'))) ?
                <div className='absolute z-[90] inset-0 p-1 text border-[1px] border-red-500 flex bg-white bg-opacity-40 '></div> 
                : null
              }
              <BsBriefcaseFill color='gray'/>
            </button>
          </Tooltip>
        )
      }
      // else{
      //   return <div className={`relative cursor-pointer text-[11px] border-x flex-1 flex items-center justify-center`}>
      //     </div>
      // }
    }else{
      return null
    }
  },[startDate, selectedData, selectedDate])

  const CellRender = React.memo(({e, date, currentDate, selectedItems}) => {
    const cDate = date;
    let currentColor = e.data?.OccupancyData[cDate]?.ShiftColor;
    const currentDateData = e.data?.OccupancyData[cDate]
    const currentAnotherDateData = Object.keys(e.data?.AnotherRoomData).length > 0 ? e.data?.AnotherRoomData : null
    return(
      <div className='h-[26px] flex' style={selectedItems?.find((item) => (selectedData.Id === e.data.Id) && (item.date === dayjs(selectedDate).date(e.columnIndex-5).format('YYYY-MM-DD'))) ? {borderColor: 'red'} : {}}>
      {
        e.value ?
        <Tooltip 
          title={currentDateData?.transporScheduleCode}
        >
          <div 
            className={`relative cursor-pointer py-1 text-[11px] border-x flex-1 flex items-center justify-center`}
            style={{backgroundColor: currentColor, color: hexToHSL(currentColor) > 60 ? 'black' : 'white'}} 
            onClick={() => handleClickonCell(e.columnIndex, e.data.Id, e)}
          >
            {
              selectedItems?.find((item) => (selectedData.Id === e.data.Id) && (item.date === dayjs(selectedDate).date(e.columnIndex-6).format('YYYY-MM-DD')))
              ? 
              <div className='absolute z-[90] inset-0 p-1 text border-[1px] border-red-500 flex bg-white bg-opacity-40 '></div> 
              : ''
            }
            <span className='relative z-[100]'>{e.value}</span>
          </div>
        </Tooltip>
        :
        renderAnotherRoom(currentAnotherDateData, cDate, e.data, e, selectedItems)
      }
      </div>
    )
  })

  const handleClickDay = useCallback((date) => {
    setSelectedCalendarDate(date)
    setShowDrawer(true)
  },[])

  const generatedColumns = useMemo(() => {
    let tmp = [
      {
        label: '#',
        name: 'Id',
        alignment: 'left',
        width: 50,
        fixed: true,
        cellRender: (e) => <span className='text-[12px] bg-transparent'>{e.value}</span>
      },
      {
        label: 'Employee', 
        name: 'Firstname', 
        alignment: 'left',
        width: '150px',
        fixed: true,
        cellRender: (e) => (
          <div className='flex items-center gap-3 group'>
            <div id={`emp${e.data.Id}`} className='flex items-center gap-1'>
              {
                e.data.RoomOwner ? 
                <i className="dx-icon-home text-green-500"></i> 
                :
                <Tooltip
                  title={e.data.RoomNumber && <Link to={`/tas/roomcalendar/${e.data.RoomId}/${startDate}`}>
                      <span className='text-blue-500 hover:underline'>{e.data?.RoomNumber}</span>
                    </Link>
                  } 
                  color='white' 
                  overlayInnerStyle={{color: 'black', fontSize: '12px'}}
                >
                  <i className="dx-icon-home text-gray-400 text-[14px]"></i>
                </Tooltip> 
              } 
              {e.data.Gender === 1 ? <FaMale size={14} className='text-blue-600'/> : <FaFemale size={14} className='text-pink-500'/>} 
              <Link 
                to={`/tas/roomassign/${e.data.Id}?startDate=${startDate}&endDate=${dayjs(startDate).add(1, 'M').endOf('month').format('YYYY-MM-DD')}`} 
                className='cursor-pointer'
                style={{fontSize: '12px'}}
              >
                <span className={twMerge('whitespace-pre-wrap hover:underline text-blue-500', `${e.data.Id}` === query.get('employeeId') ? 'text-black bg-[#ffff05] ' : '')}>{e.value} {e.data.Lastname}</span>
              </Link>
            </div>
          </div>
        )
      },
      
      {
        label: 'SAPID', 
        name: 'SAPID', 
        alignment: 'left', 
        fixed: true,
        width: 'auto', 
        cellRender: (e) => (
          <span className='text-[12px] px-1'>
            {e.value}
          </span>
        )
      },
      {
        label: 'Department', 
        name: 'DepartmentName', 
        alignment: 'left', 
        fixed: true,
        width: '150px', 
        cellRender: (e) => (
          <DepartmentTooltip showStatus={false} id={e.data?.DepartmentId}>
            <span className='text-[12px] px-1 whitespace-pre-wrap flex'>
              {e.value}
            </span>
          </DepartmentTooltip>
        )
      },
      {
        label: 'Employer', 
        name: 'EmployerName', 
        alignment: 'left', 
        fixed: true,
        width: 'auto', 
        cellRender: (e) => (
          <span className='text-[12px] px-1 w-full'>
            {e.value}
          </span>
        )
      },
      {
        label: 'Res Type', 
        name: 'PeopleTypeCode', 
        alignment: 'left',
        width: 'auto',
        fixed: true,
        cellRender: (e) => (
          <span className='text-[12px] px-1'>
            {e.value}
          </span>
        )
      },
      {
        label: 'Position', 
        name: 'PositionName', 
        alignment: 'left',
        fixed: true,
        width: 'auto',
        cellRender: (e) => (
          <span className='text-[12px] px-1 flex whitespace-pre-wrap'>
            {e.value}
          </span>
        )
      },
    ]

    const days = getDaysArrayBetweenMonths(selectedDate, dayjs(selectedDate).add(1, 'M').endOf('month'))

    days.map((item, index) => {
      const day = dayjs(item).format('D')
      const weekDay = dayjs(item).format('dd')
      tmp.push({
        label: item.split('-')[2], 
        headerCellRender: (he, re) => {
          if(dayjs().format('YYYY-MM-DD') === item){
            return <button className={`text-blue-500 font-bold`} onClick={() => handleClickDay(item)}><div className='font-semibold'>{day}</div><div className='text-[10px] leading-none'>{weekDay}</div></button>
          }
          else return <span className='' onClick={() => handleClickDay(item)}><div className='font-semibold'>{day}</div><div className='text-[10px] leading-none'>{weekDay}</div></span>
        },
        name: `OccupancyData.${item}.ShiftCode`,
        alignment: 'center',
        width: 30,
        cellRender: (e) => <CellRender e={e} date={item} currentDate={selectedDate} selectedItems={selectedItems}/>,
      })
    })
    return tmp
  },[selectedDate, selectedItems, query])

  const handleClickonCell = useCallback((day, id, event) => {
    let currentDay = dayjs(selectedDate).date(day-6).format('YYYY-MM-DD')
    if(selectedData?.Id === event.data.Id){
      if(selectedItems.length >= 2){
        setSelectedItems([{ date: currentDay }])
      }
      else if(selectedItems.length === 1){
        if(selectedItems[0].date === currentDay){
          setSelectedItems([])
        }else{
          let rangeDates = getDatesBetween(selectedItems[0].date, currentDay)
          let rsDates = []
          let breakMap = false;
          rangeDates.map((date) => {
            if(event.data.OccupancyData[date] || event.data.AnotherRoomData[date]){
              if(!breakMap){
                rsDates.push({ date: date })
              }
            }else{
              breakMap = true
            }
          })
          setSelectedItems(rsDates)
        }
      }
      else if(selectedItems.length === 0){
        setSelectedItems([{date: currentDay, shift: event.value}])
      }
    }else{
      setSelectedData(event.data)
      setSelectedItems([{date: currentDay, shift: event.value}])
    }
  },[selectedItems, selectedData, selectedDate])

  const handleSubMonth = useCallback(() => {
    setSelectedDate(dayjs(selectedDate).subtract(1, 'month'))
  },[selectedDate])

  const handleAddMonth = useCallback(() => {
    setSelectedDate(dayjs(selectedDate).add(1, 'month'))
  },[selectedDate])

  const selectRoom = useCallback((data) => {
    setToChangeRoom(data)
    setQuestionModal(true)
  },[])

  const handleSubmit = () => {
    setSubmitLoading(true)
    let dates = selectedItems.map((item) => dayjs(item.date).format('YYYY-MM-DD'))
    axios({
      method: 'post',
      url: 'tas/employeestatus/changeroombydates',
      data: {
        employeeId: selectedData?.Id,
        dates: dates,
        roomId: toChangeRoom?.RoomId
      }
    }).then((res) => {
      setQuestionModal(false)
      setShowModal(false)
      handleSearch()
      setSelectedItems([])
      setShowRoomChangeResponse(true)
      setChangeResponse(res.data)
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
  }

  const roomSearchFields = [
    {
      label: 'Camp',
      name: 'CampId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.camps,
      }
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
      className: 'col-span-6 mb-2'
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      depindex: 'CampId',
      inputprops: {
        optionsurl: 'tas/roomtype?active=1&campId=',
        loading: state.customLoading,
        optionvalue: 'Id', 
        optionlabel: 'Description',
      }
    },
    {
      label: 'Bed Count',
      name: 'bedCount',
      className: 'col-span-6 mb-2',
      type: 'number',
      inputprops: {
        min: 0
      }
    },
    {
      label: 'Private',
      name: 'Private',
      className: 'col-span-6 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

  const roomColumns = [
    {
      label: 'Room Number',
      name: 'roomNumber',
      width: '150px'
    },
    {
      label: 'Bed #',
      name: 'BedCount',
      alignment: 'left',
    },
    {
      label: 'Owners',
      name: 'RoomOwners',
      alignment: 'left',
    },
    {
      label: 'On Site Employees',
      name: 'Employees',
      alignment: 'left',
    },
    {
      label: 'Owner In Date',
      name: 'OwnerInDate',
      alignment: 'center',
      cellRender:({value}) => (
        <div>{value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '-'}</div>
      )
    },
    {
      label: '',
      name: 'action',
      width: '90px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => selectRoom(e.data)}>Select</button>
        </div>
      )
    },
  ]

  const bookedDataCols = [
    {
      label: 'Date',
      name: 'EventDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
    {
      label: 'Bed Number',
      name: 'BedDescr',
    },
  ]

  const findAvailableRoom = (values) => {
    let dates = selectedItems.map((item) => item.date)
    setRLoading(true)
    axios({
      method: 'post',
      url: 'tas/room/findavailablebydates',
      data: {
        ...values,
        Private: values.Private,
        StartDate: selectedItems[0].date,
        EndDate: selectedItems[selectedItems.length-1].date,
      }
    }).then((res) => {
      setAvailableRooms(res.data)
    }).catch((err) => {

    }).then(() => setRLoading(false))
  }

  const focusedRow = useCallback((e) => {
    if(e){
      if(e?.rowType === 'data'){
        if(query.get('empId') == e?.data.Id){
          e.rowElement.classList.add('focused-row-by-person')
        }
      }
    }
  },[query])

  const handleCloseDrawer = useCallback(() => {
    setShowDrawer(false)
  },[])

  const handleCloseChangeRoom = useCallback(() => {
    setShowModal(false)
    setAvailableRooms([])
  },[])

  const isVirtual = [state.referData?.noRoomId?.Id, state.referData?.noRoomId?.NoAccommdationId, state.referData?.noRoomId?.KhanbogdRoomId].includes(parseInt(roomId))

  const toDirectLink = useMemo(() => {
    const toDirectDate = changeResponse?.BedInfo[0]?.EventDate ? dayjs(changeResponse.BedInfo[0].EventDate).format('YYYY-MM-DD') : dayjs().format('YYYY-MM-DD')
    const directLink = `/tas/roomcalendar/${toChangeRoom?.RoomId}/${toDirectDate}?employeeId=${selectedData?.Id}`
    return directLink
  },[selectedData, selectedItems])

  return (
    <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
      <Button className='mb-4' icon={<LeftOutlined/>} onClick={()=>navigate(-1)}>Back</Button>
      <div className='mb-5'>
        <div className='xl:col-span-8 2xl:col-span-6'>
          <div className='text-lg font-bold mb-3'>View Room Booking</div>
          <div className='flex justify-between mb-4 items-center'>
            <div className='flex gap-5'>
              <div className='flex'>
                <div className='border border-[#d9d9d9] flex items-center px-3 bg-gray-50 text-gray-600 border-r-0 rounded-l-[6px]'>
                  {selectedRoom && 
                    <div className='text-xs'>
                      <span>{selectedRoom?.Number} ({selectedRoom?.BedCount})</span>
                      <span> / {selectedRoom?.RoomTypeName} / </span>
                      <span>{selectedRoom?.CampName} /</span>
                    </div>
                  }
                </div>
                <Button className='rounded-l-none border' onClick={() => setShowPopup(true)}>
                  Select Room
                </Button>
              </div>
              <div className='flex gap-1'>
                <Button onClick={handleSubMonth} icon={<LeftOutlined/>}></Button>
                <DatePicker
                  picker='month' 
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e)}
                  allowClear={false}
                />
                <Button onClick={handleAddMonth} icon={<RightOutlined/>}></Button>
              </div>
            </div>
            {
              !isVirtual ?
              <Button onClick={() => setShowAudit(true)}>Audit</Button>
              : null
            }
          </div>
        </div>
      </div>
      {
        ![state.referData?.noRoomId?.Id, state.referData?.noRoomId?.NoAccommdationId, state.referData?.noRoomId?.KhanbogdRoomId].includes(parseInt(roomId)) &&
        <CalendarTable
          data={data?.sort((a, b) => {
            if (a.futureTransportDate < b.futureTransportDate) {
              return -1;
            }
            if (a.futureTransportDate > b.futureTransportDate) {
              return 1;
            }
            return 0;
          })}
          columns={ownerColumns}
          focusedRowEnabled={false}
          showColumnLines={true}
          className='border-t'
          containerClass='shadow-none flex-1 border pb-2 mb-5'
          keyExpr={'RoomId'}
          pager={data.length > 20}
          sorting={false}
          title={<div className='flex justify-between items-end gap-5 my-2 font-bold'>
            Owners
          </div>}
        />
      }
      <div className='border rounded-ot' id='c-table'>
        <LoadPanel visible={loading} position={{of: '#c-table'}}/>
        <CalendarTable
          ref={dataGrid}
          dataSource={employeeCalendarData}
          columns={generatedColumns}
          keyExpr='Id'
          showColumnLines={true}
          showRowLines={true}
          containerClass='shadow-none flex-1 border-none'
          rowAlternationEnabled={false}
          pager={false}
          paging={false}
          sorting={false}
          tableClass='max-h-[calc(100vh-350px)] border-t'
          columnMinWidth={30}
          onRowPrepared={focusedRow}
          title={<div className='flex flex-col'>
              <div className='flex justify-between items-center gap-5 my-2'>
                <div className='flex items-center gap-5'>
                  <div className='font-bold'>On Site Employees</div>
                  <div className='flex items-center gap-1 text-xs text-gray-600'>
                    <i className="dx-icon-home text-green-500"></i> 
                    Owner
                  </div>
                  <div className='flex items-center gap-1 text-xs text-gray-600'>
                    <i className="dx-icon-home text-gray-400"></i>
                    Guest
                  </div>
                  <div className='flex items-center gap-1 text-xs text-gray-600'>
                    <BsBriefcaseFill color='gray'/>
                    In Another Room
                  </div>
                </div>
                <div className='flex items-center gap-5'>
                  <Form form={searchForm} className='flex' onFinish={handleSearchByKeyword}>
                    <Form.Item name='keyword' className='mb-0'>
                      <Input placeholder='Search by keywords' width={300}/>
                    </Form.Item>
                  </Form>
                  <Button 
                    type={'primary'} 
                    onClick={getEmployeeRoomDetail} 
                    loading={detailLoading}
                    disabled={selectedItems.length === 0}
                  >
                    Change Room
                  </Button>
                </div>
              </div>
            </div>
          }
        />
        <Pagination
          data={employeeCalendarData}
          onChangePageSize={(e) => setPageSize(e)}
          onChangePageIndex={(e) => setPageIndex(e)}
          pageSize={pageSize}
          pageIndex={pageIndex}
          totalCount={totalCount}
          isPagination={totalCount > 10}
          pageSizeDisabled={true}
          containerClass='shadow-none'
        />
      </div>
     
      <Modal open={showPopup} onCancel={() => setShowPopup(false)} title='Room' width={900}>
        <RoomSearch
          onSelect={(e) => handleSelectRoom(e)} 
        />
      </Modal>
      <Modal 
        open={showModal} 
        onCancel={handleCloseChangeRoom} 
        title={`Change Room /${selectedItems[0]?.date}${selectedItems.length > 1 ? ` - ${selectedItems[selectedItems.length-1]?.date}` : ``}/`}
        width={900}
      >
        <div>
          <Form 
            form={form}
            fields={roomSearchFields}
            className={'border rounded-ot p-4 gap-x-8 mb-5'}
            onFinish={findAvailableRoom}
            initValues={{Private: null}}
          >
            <div className='col-span-12 flex justify-end'>
              <Button htmlType='submit' icon={<SearchOutlined/>} disabled={!searchable} loading={rLoading}>Search</Button>
            </div>
          </Form>
          <Table 
            containerClass='shadow-none border'
            columns={roomColumns}
            data={availableRooms}
            allowColumnReordering={false}
            loading={loading}
            keyExpr='RoomId'
            focusedRowEnabled={false}
            showRowLines={true}
            scrolling={true}
            tableClass='max-h-[400px]'
            pager={true}
            renderDetail={{
              enabled: true, 
              component: (data) => {
                return (
                  <ExpandRowRoomDetail 
                    selectedItems={selectedItems}
                    startDate={selectedItems[0]?.date}
                    endDate={selectedItems[selectedItems?.length-1]?.date}
                    propData={data}
                  />
                )
              }
            }}
          />
        </div>
      </Modal>
      <Popup
        visible={questionModal}
        showTitle={false}
        height={'auto'}
        width={470}
      >
        <div>
          Are you sure to change the room assignment for <b className='mr-1'>#{selectedData?.Id} {selectedData?.Firstname} {selectedData?.Lastname}</b>
          from <b>{selectedRoom?.Number}</b> room to <span className='text-success font-bold'>{toChangeRoom?.roomNumber}</span> room on the following days ? 
          <br/> 
          Dates: <b>{selectedItems[0]?.date}{selectedItems.length > 1 ? ` - ${selectedItems[selectedItems.length-1]?.date}` : ``}</b>
        </div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'success'} onClick={handleSubmit} loading={submitLoading}>Yes</Button>
          <Button onClick={() => setQuestionModal(false)} disabled={submitLoading}>No</Button>
        </div>
      </Popup>
      <Modal
        open={showRoomChangeResponse}
        onCancel={() => setShowRoomChangeResponse(false)} 
        title={`Room Changed`}
        width={700}
      >
        <div className='mb-5'>
          <div className='border rounded-ot p-3 flex justify-between'>
            <div className='flex flex-col'>
              <div className='flex gap-2'>
                <div>Fullname:</div>
                <Link to={`/tas/people/search/${changeResponse?.EmployeeId}`} >
                  <span className='cursor-pointer text-blue-500 hover:underline flex gap-2 items-center'>
                    {changeResponse?.Firstname} {changeResponse?.Lastname}
                  </span>
                </Link>
              </div>
              <div className='flex gap-2'>
                <div>Department:</div>
                <div>{changeResponse?.DepartmentName}</div>
              </div>
              <div className='flex gap-2'>
                <div>Employer:</div>
                <div>{changeResponse?.EmployerName}</div>
              </div>
              <div className='flex gap-2'>
                <div>Resource Type:</div>
                <div>{changeResponse?.PeopleTypeCode}</div>
              </div>
            </div>
            <div>
              {
                changeResponse?.RoomOwner ? 
                <div><i className="dx-icon-home text-green-500"></i> Owner</div>
                :
                <div><i className="dx-icon-home text-gray-400"></i> Not Owner</div>
              } 
              {
                changeResponse?.Gender === 1 ? 
                <div className='flex items-center gap-1'>
                  <FaMale className='text-blue-600'/> 
                  Male
                </div>
                : 
                <div className='flex items-center gap-1'>
                  <FaFemale className='text-pink-500'/>
                  Female
                </div>
              } 
            </div>
          </div>
          <div className='mt-5'>
            <Table 
              containerClass='shadow-none border'
              columns={bookedDataCols}
              data={changeResponse?.BedInfo}
              allowColumnReordering={false}
              loading={loading}
              keyExpr='RoomId'
              pager={changeResponse?.BedInfo?.length > 20}
              focusedRowEnabled={false}
              showRowLines={true}
              title={<div className='border-b text-sm font-bold py-2'>Booked Data</div>}
            />
          </div>
          {
            changeResponse?.BedInfo.length > 0 ?
            <div className='mt-5 flex justify-center'>
              <Link to={toDirectLink} target='_blank'>
                <Button type={'primary'}>View Booking</Button>
              </Link>
            </div>
            : null
          }
        </div>
      </Modal>
      <Drawer
        title={<div className='flex gap-3 items-center'>{selectedRoom?.Number} <span className='text-gray-400'>{selectedCalendarDate}</span></div>}
        open={showDrawer}
        footer={false}
        onClose={handleCloseDrawer}
        width={700}
      >
        <OnSitePeople date={selectedCalendarDate} room={selectedRoom}/>
      </Drawer>
      <AuditModal roomData={selectedRoom} open={showAudit} onCancel={() => setShowAudit(false)}/>
    </div>
  )
}

export default RoomCalendarDetail