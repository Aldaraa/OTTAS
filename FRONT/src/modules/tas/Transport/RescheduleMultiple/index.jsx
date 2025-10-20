import { Form, Table, Button, Modal, TransportSearch } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm, DatePicker, Input, Select, Tag } from 'antd'
import axios from 'axios'
import { SearchOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import hexToHSL from 'utils/hexToHSL'

const title = 'Reschedule people to different transport'
const max = 200;

function RescheduleMultiple() {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ location, setLocation ] = useState([])
  const [ fromDate, setFromDate ] = useState(null)
  const [ direction, setDirection ] = useState(null)
  const [ flightOptions, setFlightOptions ] = useState([]);
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ transportLocation, setTransportLocation ] = useState({toLocation: null, fromLocation: null})
  const [ showSkipped, setShowSkipped ] = useState(false)
  const [ skippedData, setSkippedData ] = useState([])

  const [ form ] = AntForm.useForm()
  const fromLocation = AntForm.useWatch('fromLocationId', form)
  const toLocation = AntForm.useWatch('toLocationId', form)
  const existingDate = AntForm.useWatch('fromDate', form)
  const [ rescheduleForm ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)

  useEffect(() => {
    if(data?.Peoples?.length > 0){
      setSelectedRowKeys([])
      setData([])
    }
  },[fromLocation, toLocation, existingDate])

  useEffect(() => {
    action.changeMenuKey('/tas/reschedule')
    getLocations()
  },[])

  useEffect(() => {
    if(fromDate && direction && transportLocation.fromLocation && transportLocation.toLocation){
      let date = dayjs(fromDate).format('YYYY-MM-DD')
      axios({
        method: 'get',
        url: `tas/ActiveTransport/getdatetransport?eventDate=${date}&Direction=${direction}&fromLocationId=${transportLocation.fromLocation}&toLocationId=${transportLocation.toLocation}`
      }).then((res) => {
        let tmp = []
        res.data.map((item) => {
          tmp.push({ value: item.ScheduleId, label: `${item.Description} ${item.Seat}/${item.BookedCount}`})
        })
        form.setFieldValue(['lastTransport', 'flightId'], null)
        setFlightOptions(tmp)
      })
    }
  },[fromDate, direction, transportLocation])
  
  const getLocations = () => {
    axios({
      method: 'get',
      url: 'tas/location?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({value: item.Id, label: `${item.Code} - ${item.Description}`, ...item})
      })
      setLocation(tmp)
    }).catch((err) => {
  
    })
  }

  const columns = [
    {
      label: 'Person #',
      name: 'EmployeeId',
      width: '80px',
      alignment: 'left',
    },
    {
      label: 'Person',
      name: 'FullName',
    },
    {
      label: 'Department',
      name: 'Department',
    },
    {
      label: 'Transport Code',
      name: 'TransportCode'
    },
    {
      label: 'Position',
      name: 'Position',
      alignment: 'left',
    },
    {
      label: 'Cost Code',
      name: 'CostCodeDescr',
      alignment: 'left',
    },
    {
      label: 'Employer',
      name: 'EmployerName',
      alignment: 'left',
    },
    {
      label: 'Shift',
      name: 'ShiftCode',
      alignment: 'left',
      cellRender: (e) => (
        <div className={`text-[11px] font-normal w-[50px] text-center py-1 rounded`} style={{ background: e.data.ShiftCodeColor, color: hexToHSL(e.data.ShiftCodeColor) > 60 ? 'black' : 'white'}}>
          {e.value}
        </div>
      )
    },
    {
      label: 'Transport Status',
      name: 'Status',
      alignment: 'left',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'green' : 'orange'}>{e.value}</Tag>
      )
    },
    {
      label: 'Created Date',
      name: 'CreatedDate',
      alignment: 'left',
      cellRender: (e) => (
        <span>{dayjs(e.value).format('YYYY-MM-DD HH:mm:ss')}</span>
      )
    },
  ]

  const skippedColumns = [
    {
      label: 'Person #',
      name: 'Id',
      width: '80px',
      alignment: 'left',
    },
    {
      label: 'Fullname',
      name: 'Fullname',
    },
  ]

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url:'tas/transport/reschedule',
      data: {
        ...values,
        transportIds: selectedRowKeys,
      }
    }).then((res) => {
      if(res.data.length > 0){
        setSkippedData(res.data)
        setShowSkipped(true)
      }
      setShowModal(false)
      handleSearch(form.getFieldsValue())
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSearch = (values) => {
    setSearchLoading(true)
    axios({
      method: 'post',
      url:'tas/transport/searchreschedulepeople',
      data: {
        fromDate: values.fromDate ? dayjs(values.fromDate).format('YYYY-MM-DD') : '',
        toDate: values.toDate ? dayjs(values.toDate).format('YYYY-MM-DD') : '',
        fromLocationId: values.fromLocationId,
        toLocationId: values.toLocationId,
        direction: values.direction,
        scheduleId: values.scheduleId
      }
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setSearchLoading(false))
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='Reschedule From This Date' name='fromDate' className='col-span-6 mb-2' rules={[{required: true, message: 'From Date is required'}]}>
        <DatePicker onChange={(e) => {setFromDate(e); form.setFieldValue('scheduleId', null);}} className='w-full' showWeek/>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='Reschedule To This Date' name='toDate' className='col-span-6 mb-2' rules={[{required: true, message: 'To Date is required'}]}>
        <DatePicker className='w-full' showWeek/>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.toLocationId !== curValues.toLocationId}>
        {({getFieldValue, setFieldValue}) => {
          return (
            <Form.Item 
              label='Departure Location'
              name='fromLocationId'
              className='col-span-6 mb-3'
              rules={[{required: true, message: 'Departure Location is required'}]}
            >
              <Select 
                onChange={(e) => {setFieldValue('scheduleId', null); setTransportLocation(prev => ({...prev, fromLocation: e}))}} 
                options={location.filter((item) => item.Id !== getFieldValue('toLocationId'))} 
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item> 
          ) 
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId}>
        {({getFieldValue, setFieldValue}) => {
          return (
            <Form.Item 
              label='Arrive Location'
              name='toLocationId'
              className='col-span-6 mb-3'
              rules={[{required: true, message: 'Arrive Location is required'}]}
            >
              <Select 
                onChange={(e) => {setFieldValue('scheduleId', null); setTransportLocation(prev => ({...prev, toLocation: e}))}} 
                options={location.filter((item) => item.Id !== getFieldValue('fromLocationId'))} 
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item> 
          ) 
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId || prevValues.toLocationId !== curValues.toLocationId }>
        {({getFieldValue, setFieldValue}) => {
          let fromIsOnsite = location?.find((item) => item.Id === getFieldValue('fromLocationId'))?.onSite;
          let toIsOnsite = location?.find((item) => item.Id === getFieldValue('toLocationId'))?.onSite;
          if(toIsOnsite === 1){
            setFieldValue('direction', 'IN')
            setDirection('IN')
          }
          else if(fromIsOnsite === 1){
            setFieldValue('direction', 'OUT')
            setDirection('OUT')
          }
          else{
            setFieldValue('direction', '')
            setDirection(null)
          }
          return (
            <Form.Item 
              label='Direction'
              name='direction'
              className='col-span-6 mb-3'
            >
              <Select 
                disabled={true}
                options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]}
                onChange={(e) => setDirection(e)}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <>
        <Form.Item 
          noStyle
          shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId || prevValues.toLocationId !== curValues.toLocationId}
        >
          {({getFieldValue, setFieldValue, getFieldsValue}) => {
            return(
              fromDate && direction && 
              <Form.Item label='Schedule' name={'scheduleId'} className='mb-3 col-span-6' rules={[{required: true, message: 'Schedule is required'}]}>
                <Select
                  options={flightOptions}
                  className='w-full'
                  popupMatchSelectWidth={false}
                  allowClear
                  showSearch
                  filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                />
              </Form.Item>
            )
          }}
        </Form.Item>
      </>
    },
  ]

  const checkFields = () => {
    let keys = Object.keys(form?.getFieldsValue())
    let status = 0
    keys.map((item) => {
      if(form?.getFieldValue(item)){
        status++
      }
    })
    return status !== keys.length
  }

  const handleSelect = (e) => {
    if (e.selectedRowKeys.length > max) { 
      const invalidKeys = e.selectedRowKeys.slice(max); 
      e.component.deselectRows(invalidKeys);
    }else{
      setSelectedRowKeys(e.selectedRowKeys)
    }
  }

  const handleSelectionButtonFT = () => {
    const formValues = form.getFieldsValue()
    let tmp = {
      StartDate: form.getFieldValue('toDate') ? form.getFieldValue('toDate') : null,
      EndDate: form.getFieldValue('toDate') ? form.getFieldValue('toDate') : null,
      DepartLocationId: formValues.fromLocationId,
      ArriveLocationId: formValues.toLocationId,
    }
    setTransportSelectionInitValues({...tmp, transportType: 'first'})
    setShowLTSelectionModal(true)
  }

  const rescheduleFields = [
    {
      type: 'component',
      component: <Form.Item label='Date' className='col-span-12 mb-3'>
        <Input disabled value={form.getFieldValue('toDate') ? dayjs(form.getFieldValue('toDate')).format('YYYY-MM-DD') : null}/>
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item label='To Transport' className='col-span-12 mb-3' >
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item noStyle name={'scheduleId'} rules={[{required: true, message: 'To Transport is required'}]}>
            </Form.Item>
          <Form.Item name={'scheduleDescription'} className='col-span-10 mb-0'>
            <Input readOnly/>
          </Form.Item>
          <Form.Item className='col-span-2 mb-0'>
            <Button
              className='text-xs py-[5px]'
              type={'primary'}
              onClick={handleSelectionButtonFT}
              // disabled={!isEdit}
            >
              ...
            </Button>
          </Form.Item>
        </div>
      </Form.Item>
    },
    {
      label: 'Shift',
      name: 'shiftId',
      className: 'col-span-12 mb-3',
      type: 'select',
      rules: [{required: true, message: 'Shift is required'}],
      inputprops: {
        options: data?.ShifData,
        fieldNames: {label: 'Description', value: 'Id'}
      }
    }
  ]

  const handleSelectTransport = (event) => {
    rescheduleForm.setFieldValue('scheduleDescription', `${event.Code} ${event.Description}`)
    rescheduleForm.setFieldValue('scheduleId', event.Id)
    setShowLTSelectionModal(false)
  }
  
  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <Form
          form={form}
          fields={fields}
          className='grid grid-cols-12 gap-x-8'
          size='small'
          onFinish={handleSearch}
        >
          <Form.Item noStyle shouldUpdate>
            {() => {
              return(
                <Form.Item className='col-span-12 flex gap-4 justify-end mb-0'>
                  <Button 
                    onClick={() => form.submit()} 
                    className='flex items-center' 
                    htmlType='button'
                    loading={searchLoading} 
                    icon={<SearchOutlined/>}
                    disabled={checkFields()}
                  >
                    Search
                  </Button>
                </Form.Item>
              ) 
            }}
          </Form.Item>
        </Form>
      </div>
      <Table
        className={`overflow-hidden ${!state.userInfo?.ReadonlyAccess && 'border-t'}`}
        data={data?.Peoples}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        selection={!state.userInfo?.ReadonlyAccess && {mode: 'multiple', recursive: false, showCheckBoxesMode: 'always', allowSelectAll: true, selectAllMode: 'page'}}
        onSelectionChanged={handleSelect}
        isFilterRow={true}
        pager={{pageSize: [300, 500, 700, 1000]}}
        defaultPageSize={300}
        style={{maxHeight: 'calc(100vh - 345px)'}}
        title={!state.userInfo?.ReadonlyAccess ? <div className='flex justify-between py-2 gap-3'>
          <div className='ml-2'><span className='font-bold'>{selectedRowKeys.length}</span> people selected {selectedRowKeys.length === max && <span className='text-red-400'>(full)</span>}</div>
          <Button 
            disabled={selectedRowKeys.length === 0} 
            onClick={() => setShowModal(true)}
          >
            Reschedule
          </Button>
        </div> : ''}
      />
      <Modal
        title={`Accommadation warning (${skippedData.length})`}
        open={showSkipped}
        onCancel={() => setShowSkipped(false)}
        width={700}
      >
        <Table
          data={skippedData}
          columns={skippedColumns}
          containerClass='shadow-none'
        />
      </Modal>
      <Modal open={showModal} onCancel={() => setShowModal(false)} title='Reschedule' zIndex={999}>
        <Form
          form={rescheduleForm}
          fields={rescheduleFields}
          onFinish={handleSubmit}
          className='grid grid-cols-12'
        />
        <div className='flex justify-end gap-4'>
          <Button
            htmlType='button'
            type='primary'
            onClick={() => rescheduleForm.submit()}
            loading={loading}
          >
            Save
          </Button>
          <Button
            htmlType='button'
            disabled={loading}
            onClick={() => setShowModal(false)}
          >
            Cancel
          </Button>
        </div>
      </Modal>
      <Modal 
        title='Select Transport'
        open={showLTSelection}
        width={900}
        onCancel={() => setShowLTSelectionModal(false)}
        destroyOnClose={false}
        zIndex={1000}
      >
        <TransportSearch
          initialSearchValues={transportSelectionInitValues}
          handleSelect={handleSelectTransport}
        />
      </Modal>
    </div>
  )
}

export default RescheduleMultiple