import { Tabs } from 'antd';
import axios from 'axios'
import CalendarTable from 'components/CalendarTable';
import dayjs from 'dayjs';
import { CheckBox } from 'devextreme-react';
import React, { useEffect, useState } from 'react'
import { FaFemale, FaMale } from 'react-icons/fa';

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

const dateFormat = 'YYYY-MM-DD'

function SearchRoomResultDetail({ propData, startDate, endDate }) {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  
  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/room/analyze',
      data: {
        "startDate": startDate,
        "endDate": endDate,
        "roomId": propData.data.data.RoomId,
      }
    }).then((res) => {
      setData(res.data)
    }).finally(() => setLoading(false))
  }

  const CellRender = ({e, dateData}) => {
    return(
      <div className='h-[26px]'>
      {
        e.value &&
        // <Tooltip
        //   title={dateData?.transporScheduleCode} 
        //   color='white' 
        //   overlayInnerStyle={{color: 'black', fontSize: '12px'}}
        // >
          <div 
            className={`relative py-1 text-[11px] border inverSpan`}
            style={{backgroundColor: dateData.ShiftColor}} 
          >
            <span className='inverSpan'>{e.value}</span>
          </div>
        // </Tooltip>
      }
      </div>
    )
  }

  const generateColumns = (currentDate) => {
    let tmp = [
      {
        label: 'Employee', 
        name: 'Firstname', 
        alignment: 'left', 
        width: '150px', 
        cellRender: (e) => (
          <div className='flex items-center gap-3 group px-1'>
            <div id={`emp${e.data.Id}`} style={{fontSize: '12px'}} className='flex items-center gap-1'>
              {e.data.RoomOwner ? <i className="dx-icon-home text-green-500"></i> : <i className="dx-icon-home text-gray-400"></i>} 
              {e.data.Gender === 1 ? <FaMale className='text-blue-600'/> : <FaFemale className='text-pink-500'/>} 
              <span 
                // onClick={() => navigate(`/tas/people/search/${e.data?.Id}`)} 
                className='cursor-pointer text-blue-500 hover:underline'
                >
                {e.value} {e.data.Lastname}
              </span>
            </div>
          </div>
        )
      },
      {
        label: 'SAPID', 
        name: 'SAPID', 
        alignment: 'left', 
        width: '70px', 
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
        width: '150px', 
        cellRender: (e) => (
          <span className='text-[12px] px-1'>
            {e.value}
          </span>
        )
      },
      {
        label: 'Employer', 
        name: 'EmployerName', 
        alignment: 'left',
        cellRender: (e) => (
          <span className='text-[12px] px-1'>
            {e.value}
          </span>
        )
      },
      {
        label: 'Resource Type', 
        name: 'PeopleTypeCode', 
        alignment: 'left',
        cellRender: (e) => (
          <span className='text-[12px] px-1'>
            {e.value}
          </span>
        )
      },
      {
        label: 'Out Date', 
        name: 'OutDate', 
        cellRender: ({value}) => (
          <span className='text-[12px] px-1'>
            {value ? dayjs(value).format('YYYY-MM-DD') : null}
          </span>
        )
      },
    ]

    getDatesBetween(currentDate, endDate).map((date, index) => {
      tmp.push({
        label: dayjs(date).format('D'), 
        name: `DateInfo[${index}].ShiftCode`,
        alignment: 'center',
        width: '30px',
        cellRender: (e) => <CellRender e={e} dateData={e.data.DateInfo[index]}/>,
      })
    })
    return tmp
  }

  const historyCols = [
    {
      label: 'Employee', 
      name: 'Firstname', 
      alignment: 'left', 
      width: '150px', 
      cellRender: (e) => (
        <div className='flex items-center gap-3 group px-1 py-1'>
          <div id={`emp${e.data.Id}`} style={{fontSize: '12px'}} className='flex items-center gap-1'>
            {e.data.RoomOwner ? <i className="dx-icon-home text-green-500"></i> : <i className="dx-icon-home text-gray-400"></i>} 
            {e.data.Gender === 1 ? <FaMale className='text-blue-600'/> : <FaFemale className='text-pink-500'/>} 
            <span 
              // onClick={() => navigate(`/tas/people/search/${e.data?.Id}`)} 
              className='cursor-pointer text-blue-500 hover:underline'
              >
              {e.value} {e.data.Lastname}
            </span>
          </div>
        </div>
      )
    },
    {
      label: 'SAPID', 
      name: 'SAPID', 
      alignment: 'left', 
      width: '70px', 
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
      width: '150px', 
      cellRender: (e) => (
        <span className='text-[12px] px-1'>
          {e.value}
        </span>
      )
    },
    {
      label: 'Employer', 
      name: 'EmployerName', 
      alignment: 'left',
      cellRender: (e) => (
        <span className='text-[12px] px-1'>
          {e.value}
        </span>
      )
    },
    {
      label: 'Resource Type', 
      name: 'PeopleTypeName', 
      alignment: 'left',
      cellRender: (e) => (
        <span className='text-[12px] px-1'>
          {e.value}
        </span>
      )
    },
    {
      label: '',
      headerCellRender: (he, re) => (
        <i className="dx-icon-home"></i>
      ),
      name: 'HotelCheck', 
      alignment: 'center',
      cellRender: (e, r) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
    },
  ]

  const items = [
    {
      key: '1',
      label: `Current`,
      children: <div className='py-4 '>
        <CalendarTable
          data={data?.currentInfo}
          columns={generateColumns(startDate)}
          focusedRowEnabled={false}
          showColumnLines={true}
          className='border-t'
          containerClass='shadow-none flex-1 border pb-2 mx-4'
          keyExpr={'Id'}
          pager={data?.currentInfo?.length > 20}
          sorting={false}
          loading={loading}
          columnAutoWidth={true}
          title={<div className='flex justify-between items-center gap-5 my-2'>
            <div className='flex items-center gap-5'>
                <div className='flex items-center gap-1 text-xs text-gray-600'>
                  <i className="dx-icon-home text-green-500"></i> 
                  Owner
                </div>
                <div className='flex items-center gap-1 text-xs text-gray-600'>
                  <i className="dx-icon-home text-gray-400"></i>
                  Guest
                </div>
              </div>
          </div>}
        />
      </div>
    },
    {
      key: '2',
      label: `History`,
      children: <div className='py-4'>
        <CalendarTable
          data={data?.HistoryInfo}
          columns={historyCols}
          focusedRowEnabled={false}
          showColumnLines={true}
          className='border-t'
          containerClass='shadow-none flex-1 border pb-2 mx-4'
          keyExpr={'Id'}
          pager={data?.HistoryInfo?.length > 20}
          sorting={false}
          loading={loading}
          columnAutoWidth={true}
          // renderDetail={{
          //   enabled: true, 
          //   component: (data) => (
          //     <div>ss</div>
          //   )
          // }}
          title={<div className='flex justify-between items-center gap-5 my-2'>
            <div className='flex items-center gap-5'>
                <div className='flex items-center gap-1 text-xs text-gray-600'>
                  <i className="dx-icon-home text-green-500"></i> 
                  Owner
                </div>
                <div className='flex items-center gap-1 text-xs text-gray-600'>
                  <i className="dx-icon-home text-gray-400"></i>
                  Guest
                </div>
              </div>
          </div>}
        />
      </div>
    },
  ];

  return (
    <div>
       <div className='rounded-ot bg-white shadow-md'>
        <Tabs items={items} type='card'/>
      </div>
    </div>
  )
}

export default SearchRoomResultDetail