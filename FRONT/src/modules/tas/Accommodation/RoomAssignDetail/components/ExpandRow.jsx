import { Tabs, Tag } from 'antd';
import axios from 'axios'
import CalendarTable from 'components/CalendarTable';
import dayjs from 'dayjs';
import React, { useEffect, useState } from 'react'
import { FaFemale, FaMale } from 'react-icons/fa';
import { Link } from 'react-router-dom';

function ExpandRow({ propData, startDate }) {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  
  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/room/assignanalyze',
      data: {
        "startDate": dayjs(startDate).format('YYYY-MM-DD'),
        "endDate": dayjs(startDate).format('YYYY-MM-DD'),
        "roomId": propData.data.data.RoomId,
      }
    }).then((res) => {
      setData(res.data)
    }).finally(() => setLoading(false))
  }

  const generateColumns = [
    {
      label: 'Person #',
      name: 'Id', 
      alignment: 'left', 
      width: '70px', 
    },
    {
      label: 'Fullname', 
      name: 'Firstname', 
      alignment: 'left', 
      cellRender: (e) => (
        <div className='flex items-center gap-3 group px-1 py-[2px]'>
          <div id={`emp${e.data.Id}`} style={{fontSize: '12px'}} className='flex items-center gap-1'>
            <i className="dx-icon-home text-green-500"></i>
            {e.data.Gender === 1 ? <FaMale className='text-blue-600'/> : <FaFemale className='text-pink-500'/>} 
            <Link to={`/tas/roomassign/${e.data.Id}/roombooking`} target='_blank'>
              <span className='text-blue-500 hover:underline whitespace-pre-wrap'>{e.value} {e.data.Lastname}</span>
            </Link>
          </div>
        </div>
      )
    },
    // {
    //   label: 'SAPID', 
    //   name: 'SAPID', 
    //   alignment: 'left', 
    //   width: '60px', 
    //   cellRender: (e) => (
    //     <span className='text-[12px] px-1'>
    //       {e.value}
    //     </span>
    //   )
    // },
    {
      label: 'Department', 
      name: 'DepartmentName', 
      alignment: 'left', 
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
      label: 'In Date', 
      name: 'InDate', 
      alignment: 'center',
      width: 70,
      cellRender: ({value}) => (
        <span className='text-[12px]'>
          {value ? dayjs(value).format('YYYY-MM-DD') : null}
        </span>
      )
    },
  ]
  
  const historyCols = [
    {
      label: 'Person #', 
      name: 'Id', 
      alignment: 'left', 
      width: '70px', 
    },
    {
      label: 'Fullname', 
      name: 'Firstname', 
      alignment: 'left', 
      cellRender: (e) => (
        <div className='flex items-center gap-3 group px-1 py-1'>
          <div id={`emp${e.data.Id}`} style={{fontSize: '12px'}} className='flex items-center gap-1'>
            {e.data.RoomOwner ? 
              <i className="dx-icon-home text-green-500"></i>
              : 
              <i className="dx-icon-home text-gray-400"></i>
            }
            {e.data.Gender === 1 ? <FaMale className='text-blue-600'/> : <FaFemale className='text-pink-500'/>} 
            <Link to={`/tas/roomassign/${e.data.Id}/roombooking`} target='_blank'>
              <span className='cursor-pointer text-blue-500 hover:underline'>
                {e.value} {e.data.Lastname}
              </span>
            </Link>
          </div>
        </div>
      )
    },
    // {
    //   label: 'SAPID', 
    //   name: 'SAPID', 
    //   alignment: 'left', 
    //   width: '70px', 
    //   cellRender: (e) => (
    //     <span className='text-[12px] px-1'>
    //       {e.value}
    //     </span>
    //   )
    // },
    {
      label: 'Department', 
      name: 'DepartmentName', 
      alignment: 'left', 
      cellRender: (e) => (
        <span className='text-[12px]'>
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
      label: 'Shift', 
      name: 'ShiftCode', 
      alignment: 'center',
    },
    {
      label: 'Out Date', 
      name: 'OutDate', 
      alignment: 'center',
      width: 70,
      cellRender: ({value}) => (
        <span className='text-[12px]'>
          {value ? dayjs(value).format('YYYY-MM-DD') : null}
        </span>
      )
    },
  ]

  const items = [
    {
      key: '1',
      label: `Owner`,
      children: <div className='py-4 '>
        <CalendarTable
          data={data?.ownerInfo}
          columns={generateColumns}
          focusedRowEnabled={false}
          showColumnLines={true}
          className=''
          containerClass='shadow-none flex-1 border pb-2 mx-4'
          keyExpr={'Id'}
          pager={data?.ownerInfo?.length > 20}
          sorting={false}
          loading={loading}
          columnAutoWidth={true}
        />
      </div>
    },
    {
      key: '2',
      label: `OnSite`,
      children: <div className='py-4'>
        <CalendarTable
          data={data?.GuestFutureInfo}
          columns={historyCols}
          focusedRowEnabled={false}
          showColumnLines={true}
          className=''
          containerClass='shadow-none flex-1 pb-2 border mx-4'
          keyExpr={'Id'}
          pager={data?.HistoryInfo?.length > 20}
          sorting={false}
          loading={loading}
          columnAutoWidth={true}
        />
      </div>
    },
  ];

  return (
    <div>
       <div className='rounded-ot bg-white shadow-md'>
        <Tabs items={items} type='card' size='small'/>
      </div>
    </div>
  )
}

export default ExpandRow