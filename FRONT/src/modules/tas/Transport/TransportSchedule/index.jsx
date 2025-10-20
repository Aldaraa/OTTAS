import React, { useEffect, useState, useRef, useContext, useCallback } from 'react'
import { CloseOutlined, LeftOutlined, DeleteOutlined, SaveOutlined, LoadingOutlined } from '@ant-design/icons'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { Form, Table, Button, Modal } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import { DatePicker, Drawer, Input, Tag } from 'antd'
import { AuthContext } from 'contexts'
import { saveAs } from 'file-saver-es'
import { Workbook } from 'exceljs'
import axios from 'axios'
import dayjs from 'dayjs'
import { exportDataGrid } from 'devextreme/excel_exporter'
import { twMerge } from 'tailwind-merge'
import BusStopModal from './BusStopModal'
import RealETDModal from './RealETDModal'

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

const onExporting = (e) => {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('Employee List');

    worksheet.columns = [
      { width: 5 }, { width: 30 }, { width: 25 }, { width: 15 }, { width: 25 }, { width: 40 },
    ];

    exportDataGrid({
      component: e.component,
      worksheet,
      keepColumnWidths: false,
      topLeftCell: { row: 2, column: 2 },
      customizeCell: ({ gridCell, excelCell }) => {
        if (gridCell.rowType === 'data') {
          if (gridCell.column.dataField === 'Phone') {
            excelCell.value = parseInt(gridCell.value, 10);
            excelCell.numFmt = '[<=9999999]###-####;(###) ###-####';
          }
          if (gridCell.column.dataField === 'Website') {
            excelCell.value = { text: gridCell.value, hyperlink: gridCell.value };
            excelCell.font = { color: { argb: 'FF0000FF' }, underline: true };
            excelCell.alignment = { horizontal: 'left' };
          }
        }
        if (gridCell.rowType === 'group') {
          excelCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'BEDFE6' } };
        }
        if (gridCell.rowType === 'totalFooter' && excelCell.value) {
          excelCell.font.italic = true;
        }
      },
    }).then(() => {
      workbook.xlsx.writeBuffer().then((buffer) => {
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Booked Employees.xlsx');
      });
    });
  }

function TransportSchedule() {
  const [ data, setData ] = useState([])
  const [ detailData, setDetailData ] = useState(null)
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ transportMode, setTransportMode ] = useState([])
  const [ location, setLocation ] = useState([])
  const [ carrier, setCarrier ] = useState([])
  const [ selectedRows, setSelectedRows ] = useState([])
  const [ detailLoading, setDetailLoading ] = useState(false)
  const [ scheduleMembers, setScheduleMembers ] = useState({overBookedCount: null, confirmedCount: null, data: []})
  const [ showModal, setShowModal ] = useState(false)
  const [ showEditModal, setShowEditModal ] = useState(false)
  const [ fetchLoading, setFetchLoading ] = useState(false)
  const [ showBusStopModal, setShowBusStopModal ] = useState(false)
  const [ showETDModal, setShowETDModal ] = useState(false)
  const [ year, setYear ] = useState(dayjs())

  const dataref = useRef({checkBoxUpdating: false, selectAllCheckBox: null})
  const [ form ] = Form.useForm()
  const [ actionForm ] = Form.useForm()
  const navigate = useNavigate()
  const { transportId } = useParams()
  const { action, state } = useContext(AuthContext)
  
  useEffect(() => {
    action.changeMenuKey('/tas/activetransport')
    getOtherData()
  },[])

  useEffect(() => {
    getData()
  },[year, transportId])

  const getData = useCallback(() => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/activetransport/schedule/${transportId}?year=${year ? dayjs(year).format('YYYY') : dayjs().format('YYYY')}`
    }).then((res) => {
      setData(res.data.schedules)
      setDetailData({
        ...res.data,
        StartDate: res.data.schedules?.length > 0 ? dayjs(res.data.schedules?.length[0]?.EventDate).format('YYYY-MM-DD') : null,
        EndDate: res.data.schedules?.length > 0 ? dayjs(res.data.schedules[res.data.schedules.length-1]?.EventDate).format('YYYY-MM-DD') : null,
      })
      form.setFieldsValue(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  },[transportId, year])

  const getOtherData = () => {
    axios.all([
      `tas/transportmode?Active=1`,
      `tas/location?Active=1`,
      `tas/carrier?Active=1`,
      ].map((endpoint) => axios.get(endpoint)))
    .then(axios.spread((mode, location, carrier) => {
      setTransportMode(mode.data?.map((item) => ({value: item.Id, label: item.Code})))
      setLocation(location.data.map((item) => ({value: item.Id, label: `${item.Code} - ${item.Description}`})))
      setCarrier(carrier.data.map((item) => ({value: item.Id, label: `${item.Code} - ${item.Description}`})))
    })).catch(() => {

    })
  }

  const handleClickBooking = (data) => {
    setShowModal(true)
    setDetailLoading(true)
    axios({
      method: 'get',
      url: `tas/transport/scheduledetail?scheduleId=${data.Id}`
    }).then((res) => {
      let overBookedCount = []
      let confirmedCount = []
      res.data.forEach((item) => {
        if(item.Status === 'Confirmed'){
          confirmedCount.push(item)
        }else if(item.Status === 'Over Booked'){
          overBookedCount.push(item)
        }
      })
      setScheduleMembers({
        overBookedCount: overBookedCount.length,
        confirmedCount: confirmedCount.length,
        data: res.data
      })
    }).catch((err) => {

    }).then(() => setDetailLoading(false))
  }

  const fetchDetail = (row) => {
    setShowEditModal(true)
    setFetchLoading(true)
    axios({
      method: 'get',
      url: `tas/transportschedule/${row.Id}`
    }).then((res) => {
      setEditData(res.data)
      actionForm.setFieldsValue(res.data)
    }).catch((err) => {

    }).then(() => setFetchLoading(false))
  }

  const fetchBusStopDetail = (row) => {
    setShowBusStopModal(true)
    setEditData(row)
  }

  const handleClickEditETD = (row) => {
    setEditData(row)
    setShowETDModal(true)
  }

  const columns = [
    {
      label: 'Transport Date',
      name: 'EventDate',
      alignment: 'left',
      allowEditing: false,
      cellRender: (e) => (
        <div>{dayjs(e.data.EventDate).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Code',
      name: 'Code',
      allowEditing: false,
    },
    {
      label: 'Description',
      name: 'Description',
      allowEditing: true,
    },
    {
      label: 'ETD',
      name: 'ETD',
      alignment: 'left',
      allowEditing:  false,
    },
    {
      label: 'ETA',
      name: 'ETA',
      alignment: 'left',
      allowEditing: false,
    }, 
    {
      label: 'Real ETD',
      name: 'RealETD',
      alignment: 'left',
      allowEditing: false,
    }, 
    {
      label: 'Remark',
      name: 'Remark',
      alignment: 'left',
      allowEditing: false,
    }, 
    {
      label: '# Seats',
      name: 'Seats',
      alignment: 'left',
      allowEditing: false,
    },
    {
      label: 'Available Seats',
      name: 'Bookings',
      alignment: 'left',
      allowEditing: false,
      cellRender: (e) => (
        <>
          <button
            type='button'
            className={twMerge('hover:underline font-bold', e.data.Seats - e.value <= 0 ? 'text-red-400' : 'text-green-600 ')}
            onClick={() => handleClickBooking(e.data)}
            >
            {e.data.Seats - e.value}
          </button>
        </>
      )
    },
    {
      label: 'Bus Stop',
      name: 'BusstopStatus',
      alignment: 'left',
      allowEditing: false,
      cellRender: (e) => (
        <CheckBox value={e.value} disabled iconSize={18}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '250px',
      alignment: 'center',
      allowEditing: false,
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={(event) => fetchBusStopDetail(e.data)}>Bus Stop</button>
          <button type='button' className='edit-button' onClick={(event) => handleClickEditETD(e.data)}>Edit ETD</button>
          <button type='button' className='edit-button' onClick={(event) => fetchDetail(e.data)}>Edit</button>
        </div>
      )
    },
  ]
  
  const memberColumns = [
    {
      label: '№',
      name: '',
      cellRender: (e, i) => (
        <span>{e.rowIndex+1}</span>
      )
    },
    {
      label: 'Fullname',
      name: 'FullName',
      alignment: 'left',
      cellRender: (e) => (
        <Link to={`/tas/people/search/${e.data.EmployeeId}`}>
          <span className='text-blue-500 hover:underline'>{e.value}</span>
        </Link>
      )
    },
    {
      label: 'Department',
      name: 'Department',
    },
    {
      label: 'Employer',
      name: 'Employer',
    },
    {
      label: 'Description',
      name: 'Description',
      alignment: 'left',
    },
    {
      label: 'Status',
      name: 'Status',
      alignment: 'left',
      width: '110px',
      cellRender: (e) => (
        <Tag color={e.value === 'Confirmed' ? 'success' : 'orange'} className='text-xs'>{e.value}</Tag>
      )
    },
    {
      label: 'Completion Date',
      name: 'DateCreated',
      alignment: 'left',
      cellRender: ({value}) => (
        <div>{dayjs(value).format('YYYY-MM-DD HH:mm')}</div>
      )
    },
  ]

  const handleDelete = () => {
    axios({
      method: 'delete',
      url: 'tas/transport/schedule',
      data: {
        scheduleIds: selectedRows,
      }
    }).then((res) => {
      setShowPopup(false)
      getData()
      dataref.current.instance.clearSelection()
      setSelectedRows([])
    }).catch((err) => {

    })
  }

  const handleSubmitEditSchedule = (values) => {
    setSubmitLoading(true)
    axios({
      method: 'put',
      url: 'tas/transportschedule',
      data: {
        id: editData.Id,
        ...values
      }
    }).then((res) => {
      getData()
      setEditData(null)
      setShowEditModal(false)
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
  }

  const editFields = [
    {
      label: 'Transport Mode',
      name: 'TransportModeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops:{
        options: state.referData?.transportModes,
      }
    },
    {
      label: 'Carrier',
      name: 'CarrierId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: carrier,
      }
    },
    {
      label: 'Code',
      name: 'TransportCode',
      className: 'col-span-6 mb-2',
    },
    {
      label: 'Seats',
      name: 'Seats',
      className: 'col-span-6 mb-2',
    },
    {
      type: 'component',
      component: <>
        <Form.Item 
          label='ETD (Schedule)'
          name='ETD'
          className='col-span-6 mb-3'
          rules={[{ pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
          >
          <Input
            maxLength={4}
            />
        </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
        <Form.Item 
          label='ETA (Schedule)'
          name='ETA'
          className='col-span-6 mb-3'
          rules={[{ pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
        >
          <Input
            maxLength={4}
          />
        </Form.Item>
      </>
    },
  ]

  const onEditorPreparing = useCallback((e) => {
    let dataGrid = e.component;
    if (e.parentType === "dataRow") {
      if(e.row?.data?.EventDate && e.dataField === "RealETD"){
        e.editorOptions.readOnly = dayjs().diff(dayjs(e.row?.data?.EventDate)) < 0;
      }
    }
    if (e.type !== 'selection') return;
    if (e.parentType === 'dataRow' && e.row){
      if(!isSelectable(e.row.data)){
        e.editorOptions.disabled = true;
      }
      // console.log('condition', e.row.data.EventDate, e.row.data.EventDate && (dayjs().diff(dayjs(e.row?.data?.EventDate)) < 0));
      // if(e.dataField === 'RealETD' && e.row.data.EventDate){
      //   e.editorOptions.readOnly =  dayjs().diff(dayjs(e.row?.data?.EventDate)) < 0;
      // }
  
    }

    if (e.parentType === "headerRow") {
      e.editorOptions.onInitialized = (e) => {
        if (e.component)
          dataref.current.selectAllCheckBox = e.component;
      };
      e.editorOptions.value = isSelectAll(dataGrid);
      e.editorOptions.onValueChanged = (e) => {
        if (!e.event) {
          if (e.previousValue && dataref.current.checkBoxUpdating)
            e.component.option("value", e.previousValue);
          return;
        }
        if (isSelectAll(dataGrid) === e.value)
          return;
        e.value ? dataGrid.selectAll() : dataGrid.deselectAll();
        e.event.preventDefault();
      }
    }
  }, [])

  const isSelectable = useCallback((item) => {
    return item?.Bookings === 0;
  },[])

  function isSelectAll(dataGrid){
    let items = [];
    dataGrid.getDataSource().store().load().then((data) => {
      items = data
    });

    let selectableItems = items.filter(isSelectable);
    let selectedRowKeys = dataGrid.option("selectedRowKeys");
    if (!selectedRowKeys || !selectedRowKeys.length) {
      return false;
    }
    return selectedRowKeys.length >= selectableItems.length ? true : undefined;
  }

  const handleSelectionChange = useCallback((e) => {
    let deselectRowKeys = []
    e.selectedRowsData.map((item) => {
      if (!isSelectable(item)) deselectRowKeys.push(e.component.keyOf(item))
    })
    if (deselectRowKeys.length) {
      e.component.deselectRows(deselectRowKeys)
    }
    dataref.current.checkBoxUpdating = true;
    const selectAllCheckBox = dataref.current.selectAllCheckBox;
    selectAllCheckBox?.option("value", isSelectAll(e.component));
    dataref.current.checkBoxUpdating = false;
    if(JSON.stringify(selectedRows) !== JSON.stringify(dataref.current?.instance?.getSelectedRowKeys())){
      setSelectedRows(dataref.current?.instance?.getSelectedRowKeys())
    }
  },[])

  const handleClear = useCallback(() => {
    dataref.current.instance.clearSelection()
    setSelectedRows([])
  },[dataref])
  
  const onUpdateDescription = useCallback((e) => {
    const updatedData = e.changes[0]
    if(updatedData.data?.Description){
      axios({
        method: 'put',
        url: 'tas/transportschedule/description',
        data: {
          id: updatedData.key,
          description: updatedData.data.Description,
        }
      }).then((res) => {
        getData()
      }).catch((err) => {
  
      })
    }
  },[])

  const onReset = useCallback(() => {
    getData()
    setEditData(null)
    setShowBusStopModal(false)
  },[])
  
  const allowUpdating = useCallback((e) => {
    if(e.row.data?.EventDate && (dayjs().diff(dayjs(e.row?.data?.EventDate)) < 0)){
      return false
    }
    return true
  }, [])

  const handleChangeYear = useCallback((date) => {
    setYear(date)
  },[])

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
          <div className='flex gap-10'>
            <div className=''>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-28 text-[12px] text-secondary'>Mode:</div>
                <div>{detailData?.TransportModeName}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-28 text-[12px] text-secondary'>Transport Code:</div>
                <div>{detailData?.Code}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-28 text-[12px] text-secondary'>Direction:</div>
                <div>{detailData?.Direction}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-28 text-[12px] text-secondary'>Port Of Departure:</div>
                <div>{detailData?.fromLocationName}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-28 text-[12px] text-secondary'>Port Of Arrive:</div>
                <div>{detailData?.toLocationName}</div>
              </div>
            </div>
            <div className=''>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-16 text-[12px] text-secondary'>Carrier:</div>
                <div>{detailData?.CarrierName}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-16 text-[12px] text-secondary'>Seats:</div>
                <div>{detailData?.Seats}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-16 text-[12px] text-secondary'>Direction:</div>
                <div>{detailData?.Direction}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-16 text-[12px] text-secondary'>ETD:</div>
                <div>{detailData?.ETD}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-16 text-[12px] text-secondary'>ETA:</div>
                <div>{detailData?.ETA}</div>
              </div>
            </div>
            <div className=''>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-[100px] text-[12px] text-secondary'>Start Date:</div>
                <div>{detailData?.StartDate}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-[100px] text-[12px] text-secondary'>End Date:</div>
                <div>{detailData?.EndDate}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-[100px] text-[12px] text-secondary'>Week days:</div>
                <div>{detailData?.DayNum}</div>
              </div>
              <div className='flex items-center gap-4'>
                <div className='flex-initial w-[100px] text-[12px] text-secondary'>Frequency Weeks:</div>
                <div>{detailData?.FrequencyWeeks}</div>
              </div>
            </div>
          </div>
          <div className='flex gap-4 justify-end'>
            <Button onClick={() => navigate(-1)} icon={<LeftOutlined/>}>Back</Button>
          </div>
      </div>
      <Table
        ref={dataref}
        data={data}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        selection={{
          mode: 'multiple', 
          recursive: false, 
          showCheckBoxesMode: 'always', 
          allowSelectAll: true,
          selectAllMode: 'page',
          selectRows: (e) => { return !isSelectable(e)}
        }}
        isFilterRow={true}
        style={{maxHeight: 'calc(100vh - 400px)'}}
        onSelectionChanged={handleSelectionChange}
        onEditorPreparing={onEditorPreparing}
        editing={{mode: 'cell', allowUpdating: true}}
        onSaving={onUpdateDescription}
        keyExpr={'Id'}
        title={!state.userInfo?.ReadonlyAccess ? <div className='flex justify-between items-center py-2 gap-3 border-b border-gray-300'>
          <div className='flex items-center gap-2 ml-2'>
            <label className='text-sm font-medium text-secondary2'>Year:</label>
            <DatePicker.YearPicker placeholder='Year' value={year} allowClear={false} onChange={handleChangeYear} style={{width: 100}}/>
          </div>
          <div className='flex items-center gap-3'>
            <div className='mr-2'><span className='font-bold'>{selectedRows.length}</span> people selected {selectedRows.length === 50 && <span className='text-red-400'>(full)</span>}</div>
            <Button 
              icon={<CloseOutlined/>}
              onClick={handleClear}
              disabled={selectedRows.length === 0}
            >
              Clear Selection
            </Button>
            <Button 
              icon={<DeleteOutlined />} 
              onClick={() => setShowPopup(true)} 
              disabled={selectedRows.length === 0}
              type='danger'
            >
              Delete Schedules
            </Button>
          </div>
        </div> : ''}
      />
      <Popup
        visible={showPopup}
        showTitle={false}
        height={120}
        width={350}
      >
        <div>Are you sure you want to deactivate this records?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type='danger' onClick={handleDelete}>Yes</Button>
          <Button onClick={() => setShowPopup(false)}>
            No
          </Button>
        </div>
      </Popup>
      <Modal
        open={showEditModal} 
        onCancel={() => setShowEditModal(false)} 
        title={`Edit Schedule (${dayjs(editData?.EventDate).format('YYYY-MM-DD')})`}
        width={800}
      >
        {
          fetchLoading ? 
          <div className='h-[100px] flex justify-center items-center'>
            <LoadingOutlined style={{fontSize: 26}}/>
          </div>
          :
          <Form
            fields={editFields.filter((el) => !el.hide)}
            className='pt-2 gap-x-8'
            form={actionForm}
            editData={editData}
            onFinish={handleSubmitEditSchedule}
            itemLayouts={formLayout}
            labelCol={{flex: '100px'}}
          >
            <div className='col-span-12 gap-3 flex justify-end mt-3'>
              <Button type='primary' onClick={() => actionForm.submit()} loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
              <Button onClick={() => setShowEditModal(false)} disabled={submitLoading}>Cancel</Button>
            </div>
          </Form>
        }
      </Modal>
      <BusStopModal
        open={showBusStopModal} 
        onCancel={() => setShowBusStopModal(false)} 
        title={`Set Bus Stop (${dayjs(editData?.EventDate).format('YYYY-MM-DD')})`}
        width={800}
        rowData={editData}
        onReset={onReset}
      />
      <RealETDModal
        open={showETDModal} 
        onCancel={() => setShowETDModal(false)} 
        title={`Edit Real ETD (${dayjs(editData?.EventDate).format('YYYY-MM-DD')})`}
        width={600}
        rowData={editData}
        onReset={getData}
      />
      <Drawer 
        open={showModal} 
        onClose={() => setShowModal(false)} 
        footer={false} 
        title='Booking detail'
        width={800}
        bodyStyle={{padding: 5}}
      >
        <Table 
          data={scheduleMembers?.data} 
          columns={memberColumns} 
          keyExpr='Id' 
          containerClass='shadow-none'
          loading={detailLoading}
          columnAutoWidth
          export={{enabled: true}}
          onExporting={onExporting}
          style={{maxHeight: 'calc(100vh - 80px)'}}
          toolbar={[
            {
              location: 'before',
              render: (e) => <div className='flex gap-5'>
                <span className='font-medium'>
                  Confirmed: <span className='text-green-500 font-bold'>{scheduleMembers?.confirmedCount}</span>
                </span>
                <span className='font-medium'>
                  Over Booked: <span className='text-orange-500 font-bold'>{scheduleMembers?.overBookedCount}</span>
                </span>
              </div>
            }
          ]}
        />
      </Drawer>
    </div>
  )
}

export default TransportSchedule