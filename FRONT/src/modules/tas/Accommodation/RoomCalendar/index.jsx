import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Button, Form } from 'components'
import dayjs from 'dayjs'
import axios from 'axios';
import { Link } from 'react-router-dom';
import { AuthContext } from 'contexts';
import CalendarTable from 'components/CalendarTable';
import CellRender from './CellRender';
import DataSource from 'devextreme/data/data_source';
import isArray from 'lodash/isArray';
import InsideRoomDrawer from './InsideRoomDrawer';
import { twMerge } from 'tailwind-merge';

const dateFormat = 'YYYY-MM-DD'

function RoomCalendar() {
  const { state, action } = useContext(AuthContext)
  const [ selectedDate, setSelectedDate ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showInsideRoom, setShowInsideRoom ] = useState(false)
  const [ startDate, setStartDate ] = useState(dayjs().startOf('month'))
  const [ store ] = useState(new DataSource({
    key: 'RoomId',
    load: (loadOptions) => {
      let params = null;
      if(loadOptions.sort?.length > 0){
        params = {
          sortby: loadOptions.sort[0].selector,
          sortDirection: loadOptions.sort[0].desc ? 'desc' : 'asc' 
        }
      }
      setLoading(true)
      return axios({
        method: 'post',
        url: `tas/room/statusbetweendates`,
        data: (loadOptions.filter && !isArray(loadOptions?.filter)) ? 
          {
            model: {
              ...loadOptions.filter,
              startDate: dayjs(loadOptions.filter.startDate).format('YYYY-MM-DD'),
              endDate: dayjs(loadOptions.filter.endDate).format('YYYY-MM-DD'),
            },
            sort: {
              sortby: params?.sortby || '',
              sortDirection: params?.sortDirection || '',
            },
            pageIndex: (loadOptions.skip/loadOptions.take)+1,
            pageSize: loadOptions.take,
          } :
          {
            model: state.roomSearchValues ? {
              ...state.roomSearchValues,
              startDate: dayjs(state.roomSearchValues.startDate).format('YYYY-MM-DD'),
              endDate: dayjs(state.roomSearchValues.endDate).format('YYYY-MM-DD'),
            } : {
              startDate: dayjs().startOf('month').format('YYYY-MM-DD'),
              endDate: dayjs().endOf('month').format('YYYY-MM-DD'),
              campId: null,
              roomTypeId: null ,
              private: null,
              bedCount: null,
              roomNumber: '',
            },
            sort: {
              sortby: params?.sortby || '',
              sortDirection: params?.sortDirection || '',
            },
            pageIndex: (loadOptions.skip/loadOptions.take)+1,
            pageSize: loadOptions.take
          },
        }).then((res) => {
        return {
          data: res.data.data,
          totalCount: res.data.totalcount,
        }
      }).finally(() => {
        setLoading(false)
      })
    }
  }))

  const [ form ] = Form.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(state.roomSearchValues){
      form.setFieldsValue(state.roomSearchValues)
    }
  },[state.roomSearchValues])

  const generateColumns = useCallback(() => {
    let tmp = []
    if(form){
      if(startDate){
        tmp = [
          {
            label: 'Room', 
            name: 'RoomNumber', 
            alignment: 'left', 
            width: '80px',
            fixed: 'left',
            allowSorting: true,
            cellRender: (e) => (
              <div className='flex items-center'>
                <Link to={`/tas/roomcalendar/${e.data.RoomId}/${startDate.startOf('month').format(dateFormat)}`}>
                  <span className='text-xs flex items-center gap-3 group cursor-pointer text-blue-500 hover:underline'>{e.value}</span>
                </Link>
              </div>
            )
          },
          {
            label: 'Beds', 
            name: 'BedCount', 
            alignment: 'left', 
            fixed: 'left',
            width: '30px', 
            allowSorting: true,
            cellRender: (e) => (
              <div className='text-center'>{e.value}</div>
            )
          },
          {
            label: '', 
            name: 'RoowOwners', 
            alignment: 'center', 
            fixed: 'left',
            width: '35px', 
            allowSorting: true,
            cellRender: (e) => (
              <div className='text-center'>{e.value}</div>
            ),
            headerCellRender: (e) => (
              <i className="dx-icon-home text-green-500"></i>
            )
          },
        ]
        for (let index = 1; index <= dayjs(startDate).daysInMonth(); index++) {
          tmp.push({
            label: index,
            headerCellRender: (he, re) => {
              return(
                <div className={twMerge('flex flex-col items-center', (dayjs(startDate).date(index).format('D') === dayjs().format('D')) ? 'text-blue-400 font-bold' : '')}>
                  <div>{dayjs(startDate).date(index).format('D')}</div>
                  <div className='text-[10px] leading-none'>{dayjs(startDate).date(index).format('dd')}</div>
                </div>
              )
            }, 
            name: `OccupancyData.${dayjs(startDate).date(index).format(dateFormat)}.ActiveBeds`,
            alignment: 'center',
            allowSorting: false,
            cellRender: (e) => {
              return(
                <CellRender 
                  event={e} 
                  index={index} 
                  currentDate={startDate} 
                  selectedDate={selectedDate} 
                  handleSelectDate={handleSelectDate}
                />
              )
            }
          })
        }
      }
    }
    return tmp
  },[form, startDate])

  const handleSearchAllDay = (values) => {
    setStartDate(dayjs(values.startDate).startOf('month'))
    let finalSearchValues = {
      ...values, 
      startDate: dayjs(values.startDate).startOf('month'),
      endDate: dayjs(values.startDate).endOf('month'),
    }
    dataGrid.current?.instance.filter(finalSearchValues)
    action.saveRoomSearchValues(finalSearchValues)
  }

  const handleSelectDate = (RoomId, date, event, status, buttonEvent) => {
    buttonEvent?.stopPropagation()
    if((RoomId !== selectedDate?.roomId) || (date !== selectedDate?.date)){
      // if(status !== 'empty'){
        setShowInsideRoom(true)
        setSelectedDate({roomId: RoomId, date: date, roomNumber: event.data?.RoomNumber})
      // }
    }
  }

  const fields = useMemo(() => [
    {
      label: 'Month',
      name: 'startDate',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-3 mb-2',
      type: 'date',
      rules: [{required: true, message: 'Month is required'}],
      inputprops: {
        allowClear: false,
        picker: 'month',
      }
    },
    {
      label: 'Camp',
      name: 'campId',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-3 mb-2',
      type: 'select',
      // rules: [{required: true, message: 'Camp is required'}],
      inputprops: {
        options: state.referData?.camps,
      }
    },
    {
      label: 'Room Type',
      name: 'roomTypeId',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-3 mb-2',
      type: 'select',
      depindex: 'campId',
      // rules: [{required: true, message: 'Room Type is required'}],
      inputprops: {
        optionsurl: 'tas/roomtype?active=1&campId=',
        loading: state.customLoading,
        optionvalue: 'Id', 
        optionlabel: 'Description',
      }
    },
    {
      label: 'Room Number',
      name: 'roomNumber',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-3 mb-2',
    },
    {
      label: 'Bed Count',
      name: 'bedCount',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-3 mb-2',
      type: 'number',
      inputprops: {
        min: 0
      }
    },
    {
      label: 'Private',
      name: 'private',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-2 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      } 
    },
    {
      label: 'Locked Room',
      name: 'locked',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-2 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      } 
    },
    {
      label: 'Has Owner',
      name: 'hasOwner',
      className: 'col-span-12 xl:col-span-3 2xl:col-span-2 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      } 
    },
  ],[state])

  const handleCloseDrawer = () => {
    setShowInsideRoom(false)
  }

  return (
    <>
      <div className='rounded-ot'>
        <div className='flex gap-4 items-start z-50'>
          <Form
            form={form}
            fields={fields}
            className='flex-1 grid grid-cols-12 gap-x-5 border p-3 rounded-ot bg-white shadow-card'
            onFinish={handleSearchAllDay}
            noLayoutConfig={true}
            initValues={{
              startDate: dayjs().startOf('month'), 
              private: null,
              locked: null, 
              hasOwner: null,
            }}
          >
            <div className='col-span-12 flex justify-end'>
              <Button 
                className='py-0' 
                htmlType='submit' 
                type='primary' 
                loading={loading}
              >
                Search
              </Button>
            </div>
          </Form>
        </div>
        <div className='mt-5 flex gap-4 items-start'>
          <CalendarTable
            ref={dataGrid}
            data={store}
            columns={generateColumns(selectedDate)}
            focusedRowEnabled={false}
            remoteOperations={true}
            containerClass='shadow-none flex-1 border'
            tableClass='max-h-[calc(100vh-280px)] border-t'
            scrolling={true}
            allowColumnReordering={false}
            title={<div className='flex justify-end gap-5 my-2'>
              <div className='flex items-center gap-1 text-xs text-gray-600'>
                <div className="h-3 w-6 bg-[#ebedf0] rounded"></div>
                Full
              </div>
              <div className='flex items-center gap-1 text-xs text-gray-600'>
                <div className="h-3 w-6 bg-[#fadb14] rounded"></div>
                Have a person
              </div>
              <div className='flex items-center gap-1 text-xs text-gray-600'>
                <div className="h-3 w-6 bg-[#00c75e] rounded"></div>
                Empty
              </div>
              <div className='flex items-center gap-1 text-xs text-gray-600'>
                <div className="h-3 w-6 bg-[#ff5b5b] rounded"></div>
                Double
              </div>
              <div className='flex items-center gap-1 text-xs text-gray-600'>
                <i className="dx-icon-home text-green-500 "></i>
                Owner
              </div>
              <div className='flex items-center gap-1 text-xs text-gray-600'>
                <i className="dx-icon-home text-gray-400"></i>
                Guest
              </div>
            </div>}
          />
        </div>
      </div>
      <InsideRoomDrawer
        open={showInsideRoom}
        handleCloseDrawer={handleCloseDrawer}
        store={state}
        selectedDate={selectedDate}
      />
    </>
  )
}

export default RoomCalendar