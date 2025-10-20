import { Form, Button, CustomTable } from 'components'
import React, { useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form as AntForm, Tag, Select, DatePicker } from 'antd'
import { SearchOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'

function TransportSearch({initialSearchValues, handleSelect, directionDisabled, isExternal=false}) {
  const [ data, setData ] = useState([])
  const [ transportMode, setTransportMode ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(100)

  const [ searchForm ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)
  const dataGrid = useRef(null)

  useEffect(() => {
    referData()
  },[])

  useEffect(() => {
    if(initialSearchValues){
      setPageIndex(0)
      searchForm.setFieldsValue(initialSearchValues)
      searchForm.submit()
    }
  },[initialSearchValues])
  
  useEffect(() => {
    if(data.length > 0){
      searchForm.submit()
    }
  },[pageIndex, pageSize])

  const getData = (values) => {
    setLoading(true)
    dataGrid.current?.instance.beginCustomLoading()
    axios({
      method: 'post',
      url: `tas/transportschedule/manageschedule`,
      data: {
        model: {
          ...values,
          StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : null,
          EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
          External: isExternal,
        },
        pageIndex: pageIndex,
        pageSize: pageSize
      }
    }).then((res) => {
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).finally(() => {
      dataGrid.current?.instance.endCustomLoading()
      setLoading(false)
    })
  }

  const referData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/transportmode`,
    }).then((res) => {
      setTransportMode(res.data.map((item) => ({value: item.Id, label: item.Code})))
    }).catch(() => {

    }).then(() => setLoading(false))
  }

  const externalLocations = useMemo(() => {
    return state.referData?.locations.filter((loc) => loc.Code !== 'OT')
  },[])

  const searchFields = [
    {
      label: 'Mode',
      name: 'TransportModeId',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: transportMode
      }
    },
    {
      label: 'Transport Code',
      name: 'Code',
      className: 'col-span-12 lg:col-span-6 mb-2',
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.DepartLocationId !== cur.DepartLocationId}>
        {({getFieldValue}) => {
          let selectedLocation = state.referData?.locations.find(item => item.value === getFieldValue('DepartLocationId'))
          let isDisabled = selectedLocation?.Code === 'OT'
          return(
            <Form.Item name='DepartLocationId' label='Port Of Departure' className='col-span-12 lg:col-span-6 mb-2'>
              <Select
                options={state.referData?.locations}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                popupMatchSelectWidth={false}
                allowClear
                showSearch
                disabled={isDisabled}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.ArriveLocationId !== cur.ArriveLocationId}>
        {({getFieldValue}) => {
          let selectedLocation = state.referData?.locations.find(item => item.value === getFieldValue('ArriveLocationId'))
          let isDisabled = selectedLocation?.Code === 'OT'

          return(
            <Form.Item name='ArriveLocationId' label='Port Of Arrive' className='col-span-12 lg:col-span-6 mb-2'>
              <Select
                options={state.referData?.locations}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                popupMatchSelectWidth={false}
                allowClear
                showSearch
                disabled={isDisabled}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true,
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.StartDate !== cur.StartDate}>
        {({getFieldValue, setFieldValue}) => {
          let defaultPickerValue = false
          const startDate = getFieldValue('StartDate')
          const endDate = getFieldValue('EndDate')
          if(!endDate && startDate){
            defaultPickerValue = startDate
          }
          if(startDate && endDate){
            if(dayjs(endDate).diff(startDate) < 0){
              setFieldValue('EndDate', null)
            }
          }
          return(
            <Form.Item label='End Date' name='EndDate' className='col-span-12 lg:col-span-6 mb-2'>
              <DatePicker
                showWeek
                defaultPickerValue={defaultPickerValue}
                disabledDate={startDate && endDate ? (current) => current && current < dayjs(startDate).endOf('day') : false}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
  ]

  const externalSearchFields = [
    {
      label: 'Mode',
      name: 'TransportModeId',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: transportMode
      }
    },
    {
      label: 'Transport Code',
      name: 'Code',
      className: 'col-span-12 lg:col-span-6 mb-2',
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.ArriveLocationId !== cur.ArriveLocationId}>
        {({getFieldValue}) => {
          const availableLocations = externalLocations.filter((item) => getFieldValue('ArriveLocationId') !== item.value)
          return(
            <Form.Item name='DepartLocationId' label='Port Of Departure' className='col-span-12 lg:col-span-6 mb-2'>
              <Select
                options={availableLocations}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                popupMatchSelectWidth={false}
                allowClear
                showSearch
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.DepartLocationId !== cur.DepartLocationId}>
        {({getFieldValue}) => {
          const availableLocations = externalLocations.filter((item) => getFieldValue('DepartLocationId') !== item.value)
          return(
            <Form.Item name='ArriveLocationId' label='Port Of Arrive' className='col-span-12 lg:col-span-6 mb-2'>
              <Select
                options={availableLocations}
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                popupMatchSelectWidth={false}
                allowClear
                showSearch
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true,
      }
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-12 lg:col-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true,
      }
    },
  ]

  const columns = [
    {
      label: 'Date',
      name: 'EventDate',
      alignment: 'left',
      width: 90,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Description',
      name: 'Description',
      alignment: 'left',
      cellRender: (e) => `${e.data.Code} ${e.value}` 
    },
    {
      label: 'Mode',
      name: 'TransportMode',
      alignment: 'left',
      width: 70,
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
      label: 'Status',
      name: 'EventDate',
      width: 60,
      alignment: 'left',
      cellRender: (e) => <div>{dayjs(e.value) - dayjs() > 0 ? <Tag color='success'>Open</Tag> : 'Closed' }</div>
    },
    {
      label: 'Seats',
      name: 'Seats',
      width: 60,
      alignment: 'center',
    },
    {
      label: 'Available Seats',
      name: 'availableseats',
      alignment: 'center',
      // width: 70,
      cellRender: (e) => {
        const availableSeats = e.data.Seats - e.data.Confirmed
        return <div className={`${availableSeats > 0 ? 'text-green-600 font-bold' : 'text-red-400'}`}>{availableSeats}</div>
      }
    },
    // {
    //   label: 'Confirmed',
    //   name: 'Confirmed',
    //   alignment: 'center',
    //   width: 80,
    // },
    // {
    //   label: 'Overbooked',
    //   name: 'OvertBooked',
    //   alignment: 'center',
    //   cellRender: (e) => (
    //     <span className={e.value > 0 ? `text-orange-500 font-bold` : ''}>{e.value}</span>
    //   )
    // },
    {
      label: '',
      name: 'action',
      alignment: 'center',
      width: 80,
      cellRender: (e) => (
        <button className={`edit-button`} onClick={() => handleSelection(e.data)} disabled={loading}>Select</button>
      )
    },
  ]

  const handleSelection = (e) => {
    if(handleSelect){
      handleSelect(e)
    }
  }

  
  return (
    <div>
      <div className='rounded-ot bg-white border p-5 mb-5'>
        <Form 
          form={searchForm} 
          fields={isExternal ? externalSearchFields : searchFields}
          className='grid grid-cols-12 gap-x-8 max-w-[800px]' 
          onFinish={getData}
          initValues={initialSearchValues ? 
            initialSearchValues : 
            {
              TransportModeId: null,
              DepartLocationId: null,
              ArriveLocationId: null,
              Code: '',
              StartDate: dayjs(),
              EndDate: dayjs().add(30, 'days'),
            }
          }
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
        showColumnLines={false}
        wordWrapEnabled={true}
        containerClass='border shadow-none'
        pagerPosition='top'
        rowAlternationEnabled={false}
        tableClass='max-h-[400px]'
        onRowPrepared={(e) => {
          if (e.rowType === "data") {
            if (dayjs(e.data.EventDate) - dayjs() < 0) {
              e.rowElement.style.backgroundColor = "#c5e8b8";
            }
          }
        }}
      />
    </div>
  )
}

export default TransportSearch