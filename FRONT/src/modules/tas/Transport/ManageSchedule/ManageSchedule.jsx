import { Form, Table, Button, CustomTable, CustomSegmented } from 'components'
import React, { useCallback, useContext, useEffect, useRef, useState } from 'react'
import { Form as AntForm, Tag, Drawer } from 'antd'
import { SearchOutlined } from '@ant-design/icons'
import { Link } from 'react-router-dom'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import axios from 'axios'
import { exportDataGrid } from 'devextreme/excel_exporter'
import { saveAs } from 'file-saver-es'
import { Workbook } from 'exceljs'
import { CheckBox } from "devextreme-react";

const onExporting = (e) => {
  const workbook = new Workbook();
  const worksheet = workbook.addWorksheet('Employees');

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
        // if (gridCell.column.dataField === 'Phone') {
        //   excelCell.value = parseInt(gridCell.value, 10);
        //   excelCell.numFmt = '[<=9999999]###-####;(###) ###-####';
        // }
        // if (gridCell.column.dataField === 'Website') {
        //   excelCell.value = { text: gridCell.value, hyperlink: gridCell.value };
        //   excelCell.font = { color: { argb: 'FF0000FF' }, underline: true };
        //   excelCell.alignment = { horizontal: 'left' };
        // }
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
      saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Bookings.xlsx');
    });
  });
}


const title = 'Manage Schedule'

const memberColumns = [
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

function ManageScheduleList() {
  const [ data, setData ] = useState([])
  const [ transportMode, setTransportMode ] = useState([])
  const [ scheduleMembers, setScheduleMembers ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(100)
  const [ detailLoading, setDetailLoading ] = useState(false)
  const [ showDrawer, setShowDrawer ] = useState(false)


  const [ searchForm ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)
  const dataGrid = useRef(null)
  const employeeGrid = useRef(null)

  const defaultLocations = {
    DepartLocationId: state.referData?.locations?.find((item) => item.Code === 'UB').Id,
    ArriveLocationId: state.referData?.locations?.find((item) => item.Code === 'OT').Id,
    
  }

  useEffect(() => {
    action.changeMenuKey('/tas/manageschedule')
    referData()
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
      url: `tas/transportschedule/manageschedule`,
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

  const referData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/transportmode?Active=1`,
    }).then((res) => {
      setTransportMode(res.data.map((item) => ({value: item.Id, label: item.Code})))
    }).catch(() => {

    }).then(() => setLoading(false))
  }

  const handleClickBooking = useCallback((data) => {
    setShowDrawer(true)
    setDetailLoading(true)
    axios({
      method: 'get',
      url: `tas/transport/scheduledetail?scheduleId=${data.Id}`
    }).then((res) => {
      setScheduleMembers(res.data)
    }).catch((err) => {
      
    }).then(() => {
      setDetailLoading(false)
    })
  }, [])

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
        return e.data.Confirmed > 0 ? <button className='hover:underline' onClick={() => handleClickBooking(e.data)}>
            <div className={`${availableSeats <= 0 ? 'text-red-500 font-bold' : 'text-green-600 font-bold'}`}>{availableSeats}</div>
          </button>
          : 
          <div className={`${availableSeats <= 0 ? 'text-red-500 font-bold' : 'text-green-600'}`}>{availableSeats}</div>
        }
    },
    {
      label: 'EventDateETD',
      name: 'EventDateETD',
      alignment: 'left',
      width: 130,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD HH:mm')}</div>
      )
    },
    {
      label: 'EventDateETA',
      name: 'EventDateETA',
      alignment: 'left',
      width: 130,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD HH:mm')}</div>
      )
    },
  ]
  
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
          tableClass='max-h-[calc(100vh-335px)]'
        />
        <Drawer 
          open={showDrawer} 
          onClose={() => setShowDrawer(false)} 
          footer={false} 
          title='Booked Employees'
          width={800}
          bodyStyle={{padding: 5}}
        >
          <Table 
            data={scheduleMembers} 
            columns={memberColumns} 
            keyExpr='Id' 
            containerClass='shadow-none'
            loading={detailLoading}
            columnAutoWidth
            export={{enabled: true}}
            onExporting={onExporting}
          />
        </Drawer>
      </div>
    </>
  )
}

export default ManageScheduleList