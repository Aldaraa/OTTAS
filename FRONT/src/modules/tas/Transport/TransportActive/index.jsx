import { Form, Button, Modal, CustomTable, Button as MyButton, Skeleton, } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { memo, useCallback, useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm, Dropdown, Tag, notification } from 'antd'
import axios from 'axios'
import { PlusOutlined, SaveOutlined, SearchOutlined, DownloadOutlined, FileExcelOutlined, MoreOutlined } from '@ant-design/icons'
import scheduleFields from './scheduleFields'
import specialFields from './specialFields'
import transportFields from './transportFields'
import { Link, useNavigate } from 'react-router-dom'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import ls from 'utils/ls'
import StationModal from './StationModal'
import ExtendModal from './ExtendModal'
import AirCraftModal from './AirCraftModal'
import { useWatch } from 'antd/es/form/Form'
import RealETDByDateModal from './RealETDByDateModal'

const title = 'Active Transport Types'


const formLayout = {
  labelCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 10,
    },
  },
  wrapperCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 16,
    },
  },
}

const items = (row, date) => {
  if(row.Active === 1){
    if(row.Direction !== 'EXTERNAL'){
      return [
        {
          label: 'Extend',
          key: '0',
        },
        {
          label: 'Change Air Craft',
          key: '1',
        },
        {
          label: 'Bus stop',
          key: '2',
        },
        {
          label: 'Deactivate',
          key: '3',
          danger: true,
        },
         {
          label: 'Edit Real ETD',
          key: '5',
          disabled: (date ?? dayjs(date).isValid()) ? false : true,
        },
      ]
    }else{
      return [
        {
          label: 'Change Air Craft',
          key: '1',
        },
        {
          label: 'Bus stop',
          key: '2',
        },
        {
          label: 'Deactivate',
          key: '3',
          danger: true,
        },
      ]
    }
  }else{
    return [
      {
        label: 'Reactivate',
        key: '4',
        danger: true,
        disabled: true,
      },
    ]
  }
};

const MemoWrapper = memo(({children, ...restprops}) => {
  return children
}, (prev, next) => JSON.stringify(prev.data) === JSON.stringify(next.data) && (prev.pageIndex === next.pageIndex) && prev.pageSize === next.pageSize && prev.totalCount === next.totalCount)

function TransportActive() {
  const searchValues = ls.get('atv')
  const [ data, setData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ showAddSchedule, setShowAddSchedule ] = useState(false)
  const [ showAddSpecial, setShowAddSpecial ] = useState(false)
  const [ showEditModal, setShowEditModal ] = useState(false)
  const [ transportMode, setTransportMode ] = useState([])
  const [ location, setLocation ] = useState([])
  const [ carrier, setCarrier ] = useState([])
  const [ costCode, setCostCode ] = useState([])
  const [ showExportModal, setExportShowModal ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ openExtendModal, setOpenExtendModal ] = useState(false)
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(100)
  const [ openStationModal, setOpenStationModal ] = useState(false)
  const [ loading, setLoading ] = useState(true)
  const [ editBusStopData, setEditBusStopData ] = useState(null)
  const [ showCraftModal, setShowCraftModal ] = useState(null)
  const [ showRealETDModal, setShowRealETDModal ] = useState(false)
  

  const [ actionLoading, setActionLoading ] = useState(false)
  const [ form ] = AntForm.useForm()
  const [ scheduleForm ] = AntForm.useForm()
  const [ specialForm ] = AntForm.useForm()
  const [ transportForm ] = AntForm.useForm()

  const [excelform ] = AntForm.useForm()
  const scheduleDate = useWatch(['ScheduleDate'], form)
  const dataGrid = useRef(null);

  const [ api, contextHolder ] = notification.useNotification()

  const navigate = useNavigate()
  const { action, state } = useContext(AuthContext)
  
  const defaultLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id
    
  }

  useEffect(() => {
    action.changeMenuKey('/tas/activetransport')
    getOtherData()
  },[])


  useEffect(() => {
    getData()
  },[pageIndex])

  const getData = () => {
    const values = form.getFieldsValue()
    dataGrid.current?.instance.beginCustomLoading();
    axios({
      method: 'post',
      url: 'tas/activetransport/getactivetransports',
      data: {
        model: {
          ...values,
          ScheduleDate: values.ScheduleDate ? dayjs(values.ScheduleDate).format('YYYY-MM-DD') : null,
        },
        pageIndex: pageIndex,
        pageSize: pageSize
      }
    }).then((res) => {
      ls.set('atv', {...values, ScheduleDate: dayjs(values.ScheduleDate).format('YYYY-MM-DD')})
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).finally(() => dataGrid.current?.instance.endCustomLoading())
  }

  const getOtherData = useCallback(() => {
    axios.all([
      `tas/transportmode?Active=1`,
      `tas/location?Active=1`,
      'tas/carrier?Active=1',
      `tas/costcode?Active=1`,
    ].map((endpoint) => axios.get(endpoint))).then(axios.spread((trModes, locations, carriers, costCodes) => {
      setTransportMode(trModes.data.map((item) => ({value: item.Id, label: item.Code, ...item})))
      setLocation(locations.data.map((item) => ({value: item.Id, label: `${item.Code} - ${item.Description}`, ...item})))
      setCarrier(carriers.data.map((item) => ({value: item.Id, label: `${item.Code} - ${item.Description}`})))
      setCostCode(costCodes.data.map((item) => ({value: item.Id, label: `${item.Code} - ${item.Description}`})))
    })).catch(() => {
      // setLoading(false)
    }).finally(() => setLoading(false))
  },[])

  const handleDeleteButton = useCallback((dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  },[])

  const handleExtendButton = useCallback((dataItem) => {
    setEditData(dataItem)
    setOpenExtendModal(true)
  },[])

  const handleSetStationButton = useCallback((dataItem) => {
    setEditBusStopData(dataItem)
    setOpenStationModal(true)
  },[])

  const handleChangeAirCraftClick = useCallback((dataItem) => {
    setShowCraftModal(true)
    setEditData(dataItem)
  },[])

  const handleChangeRealETD = useCallback((dataItem) => {
    console.log('clicked', dataItem);
    setShowRealETDModal(true)
    setEditData(dataItem)
  },[])

  const handleMoreClick = (e, row) => {
    console.log('object', e, row);
    e.domEvent.stopPropagation()
    if(e.key === '0'){
      handleExtendButton(row)
    }
    if(e.key === '1'){
      handleChangeAirCraftClick(row)
    }
    if(e.key === '2'){
      handleSetStationButton(row)
    }
    if((e.key === '4') || (e.key === '3')){
      handleDeleteButton(row)
    }
    if((e.key === '5')){
      handleChangeRealETD(row)
    }
  }

  const columns = [
    {
      label: '#',
      name: 'Id',
      width: '80px',
      alignment: 'left',
      allowEditing: false,
      cellRender: (e) => (
        <div>
          <Link to={`/tas/activetransport/${e.data.Id}`}>
            <span type='button' className="text-blue-600 hover:underline">{e.value}</span>
          </Link>
        </div>
      )
    },
    {
      label: 'Day',
      name: 'DayNum',
      width: '100px',
      alignment: 'left',
      allowEditing: false,
    },
    {
      label: 'Mode',
      name: 'TransportModeName',
      width: '70px',
      alignment: 'left',
      allowEditing: false,
    },
    {
      label: 'Dir',
      name: 'Direction',
      width: '80px',
      allowEditing: false,
      cellRender: (e) => (
        <Tag color={e.value === 'IN' ? 'green' : e.value === 'OUT' ? 'blue' : 'magenta'} className='text-center text-[10px] font-semibold'>{e.value}</Tag>
      )
    },
    {
      label: 'Code',
      name: 'Code',
      allowEditing: false,
    },
    {
      label: 'Air Craft Code',
      name: 'AircraftCode',
      allowEditing: false,
    },
    {
      label: 'Description',
      name: 'Description',
      allowEditing: true,
    },
    {
      label: 'Schedule Start Date',
      name: 'ScheduleStartDate',
      allowEditing: false,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Schedule End Date',
      name: 'ScheduleEndDate',
      allowEditing: false,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Special',
      name: 'Special',
      width: '70px',
      alignment: 'center',
      allowEditing: false,
      allowFiltering: false,
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Special === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '130px',
      alignment: 'center',
      allowEditing: false,
      allowSorting: false,
      cellRender: (e) => (
        <div className='flex justify-end gap-3' onClick={(e) => e.stopPropagation()}>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          <Dropdown menu={{onClick: (event) => handleMoreClick(event, e.data), items: items(e.data, scheduleDate)}} placement="bottomLeft" >
            <Button icon={<MoreOutlined />}/>
          </Dropdown>
        </div>
      )
    },
  ]

  const handleEditButton = useCallback((e) => {
    setEditData({...e, StartDate: dayjs(), EndDate: dayjs(e.ScheduleEndDate)})
    setShowEditModal(true)
  },[])


  const handleDelete = useCallback(() => {
    axios({
      method: 'delete',
      url: `tas/activetransport`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {
      setShowPopup(false)
      api.error({
        message: 'Warning',
        duration: 0,
        btn: false,
        description: err.response.data.message
      });
    })
  },[editData])

  const searchFields = [
    {
      label: 'Port Of Departure',
      name: 'fromLocationId',
      className: 'col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options: location
      }
    },
    {
      label: 'Port Of Arrive',
      name: 'toLocationId',
      className: 'col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options: location
      }
    },
    {
      label: 'Transport Code',
      name: 'transportCode',
      className: 'col-span-3 mb-2',
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-1 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
    {
      label: 'Mode',
      name: 'TransportModeId',
      className: 'col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options: transportMode
      }
    },
    {
      label: 'Week Days',
      name: 'dayNum',
      className: 'col-span-3 mb-3',
      type: 'select',
      inputprops: {
        options: [
          {value: 'Monday', label: 'Monday'},
          {value: 'Tuesday', label: 'Tuesday'},
          {value: 'Wednesday', label: 'Wednesday'},
          {value: 'Thursday', label: 'Thursday'},
          {value: 'Friday', label: 'Friday'},
          {value: 'Saturday', label: 'Saturday'},
          {value: 'Sunday', label: 'Sunday'},
        ],
      }
    },
    {
      label: 'Schedule Date',
      name: 'ScheduleDate',
      className: 'col-span-3 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true,
      }
    },
  ]

  const handleSubmitSchedule = (values) => { 
    setSubmitLoading(true)
    const selectedMode = transportMode.find((mode) => mode.value === values.transportModeId)
    if(selectedMode?.label === 'Drive'){
      axios({
        method: 'post',
        url:'tas/transportschedule/createscheduledrive',
        data: {
          startDate: values.startDate ? dayjs(values.startDate).format('YYYY-MM-DD') : null,
          endDate: values.endDate ? dayjs(values.endDate).format('YYYY-MM-DD') : null,
          ...values,
        }
      }).then((res) => {
        getData()
        scheduleForm.resetFields();
        setShowAddSchedule(false)
      }).catch((err) => {
        
      }).then(() => setSubmitLoading(false))
    }else{
      axios({
        method: 'post',
        url:'tas/transportschedule/createschedule',
        data: {
          ...values,
          startDate: values.startDate ? dayjs(values.startDate).format('YYYY-MM-DD') : null,
          endDate: values.endDate ? dayjs(values.endDate).format('YYYY-MM-DD') : null,
          etd: values.etd, 
          eta: values.eta,
          outETD: values.outETD,
          outETA: values.outETA,
          inSeats: values.inSeats,
          outSeats: values.outSeats,
        }
      }).then((res) => {
        getData()
        scheduleForm.resetFields();
        setShowAddSchedule(false)
      }).catch((err) => {
  
      }).then(() => setSubmitLoading(false))
    }

  }

  const handleSubmitSpecial = (values) => { 
    setSubmitLoading(true)
    axios({
      method: 'post',
      url:'tas/activetransport/createspecial',
      data: {
        ...values,
        eventDate: values.eventDate ? dayjs(values.eventDate).format('YYYY-MM-DD') : null,
        etd: values.etd, 
        eta: values.eta, 
      }
    }).then((res) => {
      getData()
      specialForm.resetFields();
      setShowAddSpecial(false)
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
  }

  const handleSubmitTransportEdit = (values) => {
    setSubmitLoading(true)
    axios({
      method: 'put',
      url:'tas/activetransport',
      data: {
        ...values,
        StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : null,
        EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
        id: editData.Id,
      }
    }).then((res) => {
      getData()
      transportForm.resetFields();
      setShowEditModal(false)
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
  }


  
  const fields = [
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-3 mb-2',
      type: 'date',
      rules: [{required: true, message: 'Start date is required'}],
      inputprops: {
        showWeek: true,
        className: 'w-full'
      }
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-3 mb-2',
      type: 'date',
      rules: [{required: true, message: 'End date is required'}],
      inputprops: {
        showWeek: true,
        className: 'w-full'
      }
    },

    {
      label: 'Mode',
      name: 'TransportModeId',
      className: 'col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options: transportMode
      }
    },
    {
      label: 'Direction',
      name: 'Direction',
      className: 'col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options : [
          {value: 'IN', label: 'IN'}, 
          {value: 'OUT', label: 'OUT'}, 
        ]
      }
    }
  ]

  const onUpdateDescription = useCallback((e) => {
    const updatedData = e.changes[0]
    axios({
      method: 'put',
      url: 'tas/activetransport/changedescr',
      data: {
        id: updatedData.key,
        description: updatedData.data.Description,
      }
    }).then((res) => {
      getData()
    }).catch((err) => {

    })
  },[])

  const handleDownloadExcel = (values) => {
    setActionLoading(true)
    axios({
      method: 'put',
      responseType: 'blob',
      url: `tas/transportschedule/scheduleexport`,
      data: {
        StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : null,
        EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
        transportModeId : values.TransportModeId ? values.TransportModeId : null,
        Direction : values.Direction ? values.Direction : null
      }
    }).then(async (res) => {
      const url = await window.URL.createObjectURL(new Blob([res.data]));
      const fn = `TAS_TRANSPORT_SCHEDULE_${dayjs(values.startdate).format('YYYY-MM-DD')}.xlsx`
      const link = await document.createElement('a');
      link.href = url;
      await link.setAttribute('download', fn); //or any other extension
      await document.body.appendChild(link);
      await link.click();
      setExportShowModal(false)
    }).catch((err) => {
    }).then(() => {
      setActionLoading(false)
    })
  }

  return (
    <div>
      {contextHolder}
      <div className={loading ? 'block' : 'hidden'}>
        <Skeleton className='w-full h-[150px] mb-3'/>
        <Skeleton className='w-full h-[250px]'/>
      </div>
      <div className={loading ? 'hidden' : 'block'}>
        <div className='rounded-ot bg-white px-3 pb-3 pt-2 mb-3 shadow-md'>
          <div className='text-lg font-bold mb-2'>{title}</div>
          <Form
            form={form}
            fields={searchFields}
            className='grid grid-cols-12 gap-x-5'
            onFinish={getData}
            noLayoutConfig={true}
            labelCol={{flex: '110px'}}
            size='small'
            initValues={{
              Active : 1,
              ...searchValues,
              fromLocationId: defaultLocations.DepartLocationId,
              toLocationId: defaultLocations.ArriveLocationId,
              ScheduleDate: (searchValues?.ScheduleDate && dayjs(searchValues?.ScheduleDate).isValid()) ? dayjs(searchValues?.ScheduleDate) : dayjs(),
            }}
          >
            <div className='flex gap-4 justify-end col-span-12'>
              <Button htmlType='submit' icon={<SearchOutlined/>}>Search</Button>
              <Button htmlType='submit' onClick={() => setExportShowModal(true)} icon={<FileExcelOutlined />} >Export</Button>
            </div>
          </Form>
        </div>
        <MemoWrapper
          data={data} 
          pageSize={pageSize}
          pageIndex={pageIndex}
          totalCount={totalCount}
        >
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
            pagerPosition='bottom'
            showColumnLines={false}
            onRowDblClick={(e) => navigate(`/tas/activetransport/${e.data.Id}`)}
            wordWrapEnabled={true}
            filterRow={{visible: true, applyFilter: 'auto'}}
            tableClass='max-h-[calc(100vh-360px)] border-t'
            editing={{mode: 'cell', allowUpdating: true}}
            onSaving={onUpdateDescription}
            title={<div className='flex justify-between py-2 px-1'>
              <div className='flex items-center gap-1'>
                <span className='font-bold'>{totalCount}</span>
                <span className=' text-gray-400'>results</span>
              </div>
              <div className='flex items-center gap-2'>
                <Button onClick={() => setShowAddSpecial(true)} icon={<PlusOutlined />}>Add Special</Button>
                <Button onClick={() => setShowAddSchedule(true)} icon={<PlusOutlined />}>Add Schedule</Button>
              </div>
            </div>}
          />
        </MemoWrapper>
      </div>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to {editData?.Active === 1 ? 'deactivate' : 'reactivate'} this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={editData?.Active === 1 ? 'danger' : 'success'} onClick={handleDelete}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
      <ExtendModal
        transportMode={transportMode}
        carrier={carrier}
        location={location}
        open={openExtendModal}
        onReload={getData}
        editData={editData}
        onCancel={() => setOpenExtendModal(false)}
      />
      <Modal 
        open={showAddSchedule} 
        title='Add Schedule' 
        onCancel={() => setShowAddSchedule(false)}
        width={900}
      >
        <Form 
          form={scheduleForm}
          fields={scheduleFields(transportMode, carrier, location)} 
          onFinish={handleSubmitSchedule}
          className={'gap-x-4'}
          {...formLayout}
        >
          <div className='col-span-12 flex justify-end items-center gap-2 mt-4'>
            <Button type='primary' onClick={() => scheduleForm.submit()} loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={() => { setShowAddSchedule(false); scheduleForm.resetFields()}} disabled={submitLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Modal 
        open={showAddSpecial} 
        title='Add Special' 
        onCancel={() => setShowAddSpecial(false)}
        width={900}
      >
        <Form 
          form={specialForm}
          fields={specialFields(transportMode, carrier, location, costCode)} 
          onFinish={handleSubmitSpecial}
          className='gap-x-4'
          {...formLayout}
        >
          <div className='col-span-12 flex justify-end items-center gap-2 mt-4'>
            <Button type='primary' onClick={() => specialForm.submit()} loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={() => { setShowAddSpecial(false); specialForm.resetFields()}} disabled={submitLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Modal 
        open={showEditModal} 
        title='Edit Transport' 
        onCancel={() => setShowEditModal(false)}
        width={700}
      >
        <Form 
          form={transportForm}
          fields={transportFields(transportMode, carrier, location, costCode)} 
          onFinish={handleSubmitTransportEdit} 
          editData={editData}
          className={'gap-x-4'}
          {...formLayout}
        >
          <div className='col-span-12 flex justify-end items-center gap-2 mt-4'>
            <Button type='primary' onClick={() => transportForm.submit()} loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={() => { setShowEditModal(false); transportForm.resetFields()}} disabled={submitLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Modal open={showExportModal} onCancel={() => setExportShowModal(false)} title='Export Transport schedule' width={750}>
        <Form
          form={excelform}
          fields={fields}
          onFinish={handleDownloadExcel}
          layout='vertical'
          initValues={{
            StartDate: dayjs(),
            EndDate: dayjs().add(2, 'M'),
            TransportModeId : transportMode.find(x=> x.Code === "Airplane")?.Id,
            Direction : "IN",
          }}
          className='gap-x-4'
          noLayoutConfig={true}
        >
          <div className='col-span-12 flex justify-end gap-4 mt-4'>
            <MyButton
              icon={<DownloadOutlined/>}
              type='primary'
              htmlType='button'
              onClick={excelform.submit}
              loading={actionLoading}
            >
              Download
            </MyButton>
          </div>
        </Form>
      </Modal>
      <StationModal open={openStationModal} onCancel={() => setOpenStationModal(false)} editData={editBusStopData}/>
      <AirCraftModal open={showCraftModal} onCancel={() => setShowCraftModal(false)} editData={editData} refresh={getData}/>
      <RealETDByDateModal
        open={showRealETDModal} 
        onCancel={() => setShowRealETDModal(false)} 
        title={`Edit Real ETD (${dayjs(scheduleDate).format('YYYY-MM-DD')})`}
        width={600}
        rowData={editData}
        onReset={getData}
        scheduleDate={scheduleDate}
      />
    </div>
  )
}

export default TransportActive