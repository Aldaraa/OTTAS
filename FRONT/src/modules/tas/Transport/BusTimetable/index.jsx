import { Form, Button, CustomTable } from 'components'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm, Tag } from 'antd'
import { SearchOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { CheckBox } from "devextreme-react";
import BusStopModal from '../TransportSchedule/BusStopModal'

const title = 'Bus Stop Schedule'

function BusTimetable() {
  const [ data, setData ] = useState([])
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(100)
  const [ showBusStopModal, setShowBusStopModal ] = useState(false)
  const [ editData, setEditData ] = useState(null)


  const [ searchForm ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)
  const dataGrid = useRef(null)

  const defaultLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
    
  }

  useEffect(() => {
    action.changeMenuKey('/tas/busstop')
    getData(searchForm.getFieldsValue())
  },[])
  
  useEffect(() => {
    if(data.length > 0){
      getData(searchForm.getFieldsValue())
    }
  },[pageIndex, pageSize])

  const getData = (values) => {
    dataGrid.current?.instance.beginCustomLoading()
    axios({
      method: 'post',
      url: `tas/transportschedule/busstopschedule`,
      data: {
        model: {
          ...values,
          StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : '',
          EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : '',
        },
        pageIndex: pageIndex,
        pageSize: pageSize
      }
    }).then((res) => {
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).finally(() => dataGrid.current?.instance.endCustomLoading())
  }

  const searchFields = [
    {
      label: 'Mode',
      name: 'TransportModeId',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.transportModes
      }
    },
    {
      label: 'Transport Code',
      name: 'Code',
      className: 'col-span-12 lg:col-span-6 mb-2',
    },
    {
      label: 'Port Of Departure',
      name: 'DepartLocationId',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.locations
      }
    },
    {
      label: 'Port Of Arrive',
      name: 'ArriveLocationId',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.locations
      }
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true
      }
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true
      }
    },
  ]

  const fetchBusStopDetail = (row) => {
    setShowBusStopModal(true)
    setEditData(row)
  }

  const columns = [
    {
      label: 'Travel Date',
      name: 'EventDate',
      alignment: 'left',
      groupIndex: 0,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Code',
      name: 'Code',
      alignment: 'left',
    },
    {
      label: 'Description',
      name: 'Description',
      alignment: 'left',
      // defaultSortOrder: 'asc'
    },
    {
      label: 'Type',
      name: 'Special',
      alignment: 'center',
      cellRender: (e, r) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
    },
    {
      label: 'Mode',
      name: 'TransportMode',
      alignment: 'left',
      width: 80,
    },
    {
      label: 'Direction',
      name: 'Direction',
      alignment: 'center',
      width: 80,
      cellRender: (e) => (
        e.value &&
        <Tag color={e.value === 'IN' ? 'green' : e.value === 'OUT' ? 'blue' : 'magenta'} className='text-center text-[10px] font-semibold'>{e.value}</Tag>
      )
    },
    {
      label: 'Seats #',
      name: 'Seats',
      width: 60,
      alignment: 'center',
    },
    {
      label: 'Available Seats',
      name: 'availableseats',
      alignment: 'center',
      width: 140,
      cellRender: (e) => {
        const availableSeats = e.data.Seats - e.data.Confirmed
        return e.data.Confirmed > 0 ? 
          <div className={`${availableSeats <= 0 ? 'text-red-500 font-bold' : 'text-green-600 font-bold'}`}>{availableSeats}</div>
          : 
          <div className={`${availableSeats <= 0 ? 'text-red-500 font-bold' : 'text-green-600 font-bold'}`}>{availableSeats}</div>
        }
    },
    {
      label: 'EventDateETD',
      name: 'EventDateETD',
      alignment: 'left',
      width: 130,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('HH:mm')}</div>
      )
    },
    {
      label: 'EventDateETA',
      name: 'EventDateETA',
      alignment: 'left',
      width: 130,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('HH:mm')}</div>
      )
    },
    {
      label: 'Bus Stop',
      name: 'BusstopStatus',
      alignment: 'left',
      cellRender: (e) => (
        <CheckBox value={e.value ? true : false} disabled iconSize={18}/>
      )
    },
    {
      label: '',
      name: 'buttons',
      alignment: 'left',
      width: 100,
      cellRender: (e) => (
        <button type='button' className='edit-button' onClick={(event) => fetchBusStopDetail(e.data)}>Bus Stop</button>
      )
    },
  ]

  const onReset = () => {
    getData(searchForm.getFieldsValue())
    setShowBusStopModal(false)
    setEditData(null)
  }
  
  return (
    <>
      <div>
        <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
          <div className='text-lg font-bold mb-2'>{title}</div>
          <Form 
            form={searchForm} 
            fields={searchFields}
            className='grid grid-cols-12 gap-x-8 max-w-[800px]' 
            onFinish={getData}
            size='small'
            initValues={{
              TransportModeId: null,
              DepartLocationId: defaultLocations.DepartLocationId,
              ArriveLocationId: defaultLocations.ArriveLocationId,
              Code: '',
              StartDate: dayjs(),
              EndDate: dayjs().add(1, 'days'),
            }}
          >
            <div className='flex gap-4 items-baseline col-span-12 justify-end'>
              <Button htmlType='submit' icon={<SearchOutlined/>}>Search</Button>
            </div>
          </Form>
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
          totalCount={totalCount}
          isGroupedCount={true}
          isGrouping={true}
          showColumnLines={false}
          wordWrapEnabled={true}
          pagerPosition='top'
          tableClass='max-h-[calc(100vh-335px)] rounded-none border-none'
        />
      </div>
      <BusStopModal
        open={showBusStopModal} 
        onCancel={() => setShowBusStopModal(false)} 
        title={`Set Bus Stop (${dayjs(editData?.EventDate).format('YYYY-MM-DD')})`}
        width={800}
        rowData={editData}
        onReset={onReset}
      />
    </>
  )
}

export default BusTimetable