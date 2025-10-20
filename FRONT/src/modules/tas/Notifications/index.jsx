import axios from 'axios'
import { Table } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useRef, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import CustomStore from 'devextreme/data/custom_store';

function Notifications() {
  const [ notifications, setNotifications ] = useState([])
  const [ newNotifCount, setNewNotifCount ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const dataGrid = useRef(null)

  const [ store, setStore ] = useState(new CustomStore({
    key: 'Id',
    load: (loadOptions) => {
      dataGrid.current?.instance.beginCustomLoading();
      let params = '';
      if(loadOptions.sort?.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      return axios({
        method: 'post',
        url: `tas/notification/all${params}`,
        data: {
          pageIndex: loadOptions.skip/loadOptions.take,
          pageSize: loadOptions.take
        }
      }).then((res) => {
        let count = 0
        res.data.data.map((item) => {
          if(item.ViewStatus === 1){
            count++            
          }
        })
        setNewNotifCount(count)
        setNotifications(res.data.data)
        return {
          data: res.data.data,
          totalCount: res.data.totalcount,
        }
      }).finally(() => dataGrid.current?.instance.endCustomLoading())
    }
  }))

  const navigate = useNavigate()

  const handleClickNotif = (data) => {
    axios({
      method: 'get',
      url: `tas/notification/detail/${data.NotifIndex}`
    }).then((res) => {
      navigate(res.data.link)
    }).catch((err) => {

    })
  }

  const columns = [
    {
      label: 'Notification',
      name: 'Description',
      cellRender: (e) => (
        <div 
          className={`cursor-pointer hover:underline ${e.data.ViewStatus === 0 ? 'font-bold' : ''}`}
          onClick={() => handleClickNotif(e.data)}
        >
          {e.value}
        </div>
      )
    },
    {
      label: 'Change Employee',
      name: 'ChangeEmployee',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
    {
      label: 'Date',
      name: 'RelativeTime',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
  ]
  return (
    <div className='bg-white rounded-ot shadow-md px-4 pb-4'>
      <div className='text-lg font-bold mb-3 py-3'>Notifications</div>
      <div className='mb-2'>New notifications <span className='font-bold'>{newNotifCount}</span></div>
      <Table 
        ref={dataGrid}
        data={store}
        columns={columns} 
        remoteOperations={true}
        containerClass='shadow-none border' 
        loading={loading}
        wordWrapEnabled={true}
        // pager={notifications.length > 20}
      />
    </div>
  )
}

export default Notifications