import React, { useCallback, useContext, useEffect, useRef, useState } from 'react'
import { Table, Button, FileUploader, Modal, SearchPeopleForm } from 'components';
import { CheckBox } from 'devextreme-react';
import { BsAirplaneFill } from 'react-icons/bs'
import { GoPrimitiveDot } from 'react-icons/go'
import { CloseOutlined, DownloadOutlined, UploadOutlined } from '@ant-design/icons';
import axios from 'axios';
import { Form as AntForm, Tag, notification } from 'antd';
import { useLocation } from 'react-router-dom';
import { AuthContext } from 'contexts';
import CustomStore from 'devextreme/data/custom_store';
import dayjs from 'dayjs';
import validateFile from 'utils/validateUpload';
import isArray from 'lodash/isArray';
import { saveAs } from 'file-saver-es'
import { exportDataGrid } from 'devextreme/excel_exporter'
import { Workbook } from 'exceljs'

function Profile() {
  const routeLocation = useLocation();
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ downloadLoading, setDownloadLoading ] = useState(false)
  const [ uploadLoading, setUploadLoading ] = useState(false)
  const [ uploadedData, setUploadedData ] = useState(null)
  const [ showModal, setShowModal ] = useState(false)
  const [ uploadFile, setUploadFile ] = useState(null)

  const [ store ] = useState(new CustomStore({
    key: 'Id',
    load: (loadOptions) => {
      let params = '';
      if(loadOptions.sort?.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      if(loadOptions.filter && !isArray(loadOptions.filter)){
        dataGrid.current?.instance.beginCustomLoading();
        return axios({
          method: 'post',
          url: `tas/employee/search${params}`,
          data: {
            model: {
              ...loadOptions.filter,
            },
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => dataGrid.current?.instance.endCustomLoading())
      }
      else if(!isArray(loadOptions.filter) && typeof loadOptions.filter === 'undefined'){
        dataGrid.current?.instance.beginCustomLoading();
        return axios({
          method: 'post',
          url: `tas/employee/search${params}`,
          data: {
            model: {
              CampId: '',
              DepartmentId: '',
              Firstname: "",
              FlightGroupMasterId: "",
              Lastname: "",
              LocationId: "",
              NRN: "",
              roomNumber: "",
              RoomTypeId: "",
              costCodeId: "",
              employerId: "",
              futureBookingId: "",
              group: "",
              id: "",
              peopleTypeId: "",
              roomAssignment: "",
              rosterId: "",
              sapId: "",
              mobile: "",
              hasRoom: "",
              futureBooking: "",
            },
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => dataGrid.current?.instance.endCustomLoading())
      }
    }
  }))

  const { state, action } = useContext(AuthContext)
  const [ searchForm ] = AntForm.useForm()
  const [api, contextHolder] = notification.useNotification();
  const dataGrid = useRef(null)

  useEffect(() => {
    action.changeMenuKey('/tas/bulkprofile')
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
  },[])

  const columns = [
    {
      label: 'Person #',
      name: 'Id',
      dataType: 'string',
      width: '70px',
    },
    {
      label: '',
      name: 'Active',
      width: '21px',
      alignment: 'center',
      cellRender:(e) => (
        <GoPrimitiveDot color={e.value === 1 ? 'lime' : 'lightgray'}/>
      )
    },
    {
      label: 'Firstname',
      name: 'Firstname',
      dataType: 'string',
    },
    {
      label: 'Lastname',
      name: 'Lastname',
      dataType: 'string',
    },
    {
      label: 'Department',
      name: 'DepartmentName',
      dataType: 'string',
      width: '220px',
    },
    {
      label: 'Employer',
      name: 'EmployerName',
      dataType: 'string',
    },
    {
      label: 'Roster',
      name: 'RosterName',
      dataType: 'string',
      width: '70px',
    },
    {
      label: 'Resource Type',
      name: 'PeopleTypeName',
      dataType: 'string',
    },
    {
      label: 'Room',
      name: 'RoomNumber',
      dataType: 'string',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
    {
      label: 'Gender',
      name: 'Gender',
      alignment: 'center',
      width:'70px',
      cellRender: (e) => (
        <span>{e.data.Gender === 1 ? 'M' : 'F'}</span>
      ),
      groupCellRender: (e) => (
        <span>{e.value === 1 ? 'M' : 'F'} {e.groupContinuesMessage ? `- ${e.groupContinuesMessage}` : ''}</span>
      )
    },
    {
      label: '',
      name: 'HasFutureTransport',
      width: '50px',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
      headerCellRender: (he, re) => (
        <BsAirplaneFill></BsAirplaneFill>
      )
    },
    {
      label: '',
      name: 'HasFutureRoomBooking',
      width: '50px',
      cellRender: (e, r) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
      headerCellRender: (he, re) => (
        <i className="dx-icon-home"></i>
      )
    },
  ]

  const handleSearch = useCallback((values) => {
    dataGrid.current?.instance.filter(values)
  }, [])

  const handleSelect = useCallback((e) => {
    setSelectedRowKeys(e.selectedRowKeys)
  }, [])

  const handleDownload = (keys) => {
    setDownloadLoading(true)
    axios({
      method: 'post',
      url: 'tas/employee/bulkrequest',
      responseType: 'blob',
      data: {
        empIds: keys
      }
    }).then((res) => {
      const url = window.URL.createObjectURL(new Blob([res.data]));
      const fn = `TAS_Employee_Bulk_Download_${dayjs().format('YYYY-MM-DD hh:mm:ss')}.xlsx`
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', fn); //or any other extension
      document.body.appendChild(link);
      link.click();
    }).catch((err) => {

    }).then(() => setDownloadLoading(false))
  }

  

  const handleUpload = (file) => {
    if(validateFile(file)){
      setUploadLoading(true)
      setUploadFile(file)
      const formData = new FormData();
      formData.append('BulkEmployeeFile', file)
      if(file){
        axios({
          method: 'post',
          url: 'tas/employee/bulkuploadpreview',
          headers: {
            'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
          },
          data: formData,
        }).then((res) => {
          setUploadedData(res.data)
          setShowModal(true)
        }).catch((err) => {
    
        }).then(() => setUploadLoading(false))
      }
    }else{
      api.error({
        message: 'Error',
        placement: 'top',
        duration: 5,
        description: <div className='flex items-center justify-center'>
          You can only upload an excel file !
        </div>
      });
    }
  }

  const handleSubmitUpload = () => {
    setUploadLoading(true)
    const formData = new FormData();
    formData.append('BulkEmployeeFile', uploadFile)
    if(uploadFile){
      axios({
        method: 'post',
        url: 'tas/employee/bulkupload',
        headers: {
          'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        },
        data: formData,
      }).then((res) => {
        setShowModal(false)
        setUploadFile(null)
        if(res.data.length > 0){
          searchForm.setFieldValue('Id', res.data.map(i => `${i}`).join(' ').slice(0, 5000))
        }
        searchForm.submit()
      }).catch((err) => {
  
      }).then(() => setUploadLoading(false))
    }
  }

  const errorColumns = [
    {
      label: 'Row Index',
      name: 'ExcelRowIndex',
      alignment: 'left',
      width: '80px',
    },
    {
      label: 'Person #',
      name: 'PersonId',
    },
    {
      label: 'Fullname',
      name: 'Fullname',
    },
    {
      label: 'SAP #',
      name: 'SAPID',
    },
    {
      label: 'Error',
      name: 'Error',
      alignment: 'left',
      cellRender: ({value}) => (
        <div>
          <ul className='pl-3 list-outside list-disc'>{value?.map((item) => <li>{item}</li>)}</ul>
        </div>
      )
    }, 
  ]

  const onExporting = (e) => {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('Bulk_Profile_Changes_Failed_Rows');
  
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
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Companies.xlsx');
      });
    });
  }

  const handleClearAll = () => {
    dataGrid.current?.instance.clearSelection()
  }

  return (
    <div>
      <>
        <SearchPeopleForm
          containerClass='bg-white rounded-ot px-3 py-2 mb-3'
          onSearch={handleSearch}
        />
        <Table
          ref={dataGrid}
          data={store}
          columns={columns}
          allowColumnReordering={false}
          remoteOperations={true}
          showRowLines={true}
          selection={{
            mode: 'multiple',
            recursive: false,
            showCheckBoxesMode: 'always',
            allowSelectAll: true,
            selectAllMode: 'page',
          }}
          tableClass='max-h-[calc(100vh-345px)]'
          // selectedRowKeys={selectedRowKeys}
          onSelectionChanged={handleSelect}
          title={!state.userInfo?.ReadonlyAccess ? <div className='flex justify-between py-2 gap-3 border-b'>
            <div className='flex items-center gap-3'>
              <div className='ml-2'>
                <span className='font-bold'>{selectedRowKeys.length}</span> people selected
              </div>
              {
                selectedRowKeys.length > 0 ? 
                <Button className='text-xs' onClick={handleClearAll} icon={<CloseOutlined/>}>Clear All</Button> 
                : null
              }
            </div>
            <div className='flex gap-3'>
              <Button 
                // className='text-xs'
                loading={selectedRowKeys.length === 0 && downloadLoading}
                icon={<DownloadOutlined />} 
                onClick={() => handleDownload([])}
              >
                Download All
              </Button>
              <Button 
                // className='text-xs'
                loading={selectedRowKeys.length > 0 && downloadLoading}
                disabled={!selectedRowKeys.length || downloadLoading} 
                icon={<DownloadOutlined />} 
                onClick={() => handleDownload(selectedRowKeys)}
              >
                Download
              </Button>
              <FileUploader 
                // className='text-xs'
                icon={<UploadOutlined />} 
                onChange={handleUpload} 
                accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                loading={uploadLoading}
              >
                Upload
              </FileUploader>
            </div>
          </div> : ''}
        />
        <Modal title='Uploaded' open={showModal} onCancel={()=>setShowModal(false)} width={800}>
          <div>
            <div className='grid grid-cols-12 gap-2 mb-5'>
              <div className='col-span-6 text-green-500'>Added Row: <Tag className='ml-1' color='green'>{uploadedData?.AddRow}</Tag></div>
              <div className='col-span-6'>None Row: <Tag className='ml-1'>{uploadedData?.NoneRow}</Tag></div>
              <div className='col-span-6 text-blue-400'>Updated Row: <Tag className='ml-1' color='blue'>{uploadedData?.UpdateRow}</Tag></div>
              {/* <div className='col-span-6 text-red-500'>Deactivated Row: <Tag className='ml-1' color='red'>{uploadedData?.DeleteRow ? uploadedData?.DeleteRow : 0}</Tag></div> */}
            </div>
            {
              uploadedData?.FailedRows?.length > 0 &&
              <Table
                export={{enabled: true}}
                onExporting={onExporting}
                columns={errorColumns}
                data={uploadedData?.FailedRows}
                containerClass='shadow-none border mb-5'
                tableClass='max-h-[500px]'
                toolbar={[
                  {
                    location: 'before',
                    render: (e) => <div className='text-md'>Failed Rows: <span className='font-bold text-red-500'>{uploadedData?.FailedRows?.length}</span></div>
                  }
                ]}
              />
            }
          </div>
          <div className='flex justify-end gap-5 items-center'>
            <div>If you press the Process button, it will skip failed rows</div>
            <Button 
              type='primary'
              loading={uploadLoading}
              onClick={() => handleSubmitUpload()}
            >
              Process
            </Button>
            <Button  
              onClick={() => {setShowModal(false); setUploadFile(null)}}
              disabled={uploadLoading}
            >
              Cancel
            </Button>
          </div>
        </Modal>
      </>
      {contextHolder}
    </div>
  )
}

export default Profile