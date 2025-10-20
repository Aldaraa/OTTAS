import axios from 'axios'
import { Button, FileUploader, Form, Modal, Table } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import tableSearch from 'utils/TableSearch'
import { Form as AntForm, Tag, notification } from 'antd'
import { DownloadOutlined, SearchOutlined, UploadOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import validateFile from 'utils/validateUpload'

function BulkEmployer() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ downloadLoading, setDownloadLoading ] = useState(false)
  const [ uploadLoading, setUploadLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ uploadedData, setUploadedData ] = useState(null)
  const [ uploadFile, setUploadFile ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ showEmpModal, setShowEmpModal ] = useState(false)
  const [ selectedExportRow, setSelectedExportRow ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ uploadEmpLoading, setUploadingEmpLoading ] = useState(false)
  const [ uploadedEmpData, setUploadedEmpData ] = useState(null)
  const [ uploadEmpFile, setUploadEmpFile ] = useState(null)

  const { state, action } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const [api, contextHolder] = notification.useNotification();

  useEffect(() => {
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/employer'
    }).then((res) => {
      setData(res.data)
      if(form.getFieldValue('Active')){
        let tmp = res.data.filter((item) => item.Active === 1)
        setRenderData(tmp)
      }else{
        setRenderData(res.data)
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const fields = [
    {
      label: 'Code',
      name: 'Code',
      className: 'col-span-4 xl:col-span-3 2xl:col-span-2 mb-2',
      inputprops: {
        maxLength: 10
      }
    },
    {
      label: 'Number',
      name: 'Number',
      className: 'col-span-4 xl:col-span-3 2xl:col-span-2 mb-2'
    },
    {
      label: 'Description',
      name: 'Description',
      className: 'col-span-4 xl:col-span-3 2xl:col-span-2 mb-2'
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-1 mb-2',
      type: 'check',
      // hide: true,
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

  const handleDownloadButton = (row) => {
    setSelectedExportRow(row)
    setShowPopup(true)
  }

  const columns = [
    {
      label: 'Code',
      name: 'Code',
      width: '80px'
    },
    {
      label: 'Description',
      name: 'Description'
    },
    {
      label: 'Email',
      name: 'Email',
    },
    {
      label: '# Resource',
      name: 'EmployeeCount',
      alignment: 'left',
      cellRender: (e) => (
        e.value > 0 
        ?
        <button onClick={() => handleDownloadButton(e.data)} className='text-blue-400 hover:underline'>{e.value}</button>
        : <span>{e.value}</span>
      )
    },
    {
      label: 'Active',
      name: 'Active',
      width: '80px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Active === 1 ? true : 0}/>
      )
    },
  ]

  const handleSelect = (e) => {
    setSelectedRowKeys(e.selectedRowKeys)
  }

  const preparing = (e) => {
    if(state.userInfo?.ReadonlyAccess){
      // e.editorOptions.disabled = true;
    }
  }

  const handleSearch = (values) => {
    setSearchLoading(true)
    tableSearch(values, data).then((res) => {
      setRenderData(res)
      setSearchLoading(false)
    })
  }

  const handleDownload = () => {
    let tmp = []
    setDownloadLoading(true)
    axios({
      method: 'post',
      url: 'tas/employer/bulkrequest',
      responseType: 'blob',
      data: {
        employerIds: selectedRowKeys
      }
    }).then(async (res) => {
      const url = await window.URL.createObjectURL(new Blob([res.data]));
      const fn = `TAS_Employer_Bulk_Download_${dayjs().format('YYYY-MM-DD hh:mm:ss')}.xlsx`
      const link = await document.createElement('a');
      link.href = url;
      await link.setAttribute('download', fn); //or any other extension
      await document.body.appendChild(link);
      await link.click();
      await setDownloadLoading(false)
    }).catch((err) => {
      setDownloadLoading(false)
    })
  }

  const handleUpload = (file) => {
    if(validateFile(file)){
      setUploadFile(file)
      setUploadLoading(true)
      const formData = new FormData();
      formData.append('BulkEmployerFile', file)
      if(file){
        axios({
          method: 'post',
          url: 'tas/employer/bulkuploadpreview',
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

  const handleUploadEmployees = (file) => {
    if(validateFile(file)){
      setUploadEmpFile(file)
      setUploadingEmpLoading(true)
      const formData = new FormData();
      formData.append('BulkEmployerFile', file)
      if(file){
        axios({
          method: 'post',
          url: 'tas/employer/bulkuploademployeespreview',
          headers: {
            'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
          },
          data: formData,
        }).then((res) => {
          setUploadedEmpData(res.data)
          setShowEmpModal(true)
        }).catch((err) => {
    
        }).then(() => setUploadingEmpLoading(false))
      }
    }else{
      api.error({
        message: 'Error',
        placement: 'top',
        duration: 0,
        description: <div className='flex items-center justify-center'>
          You can only upload an excel file !
        </div>
      });
    }
  }

  const handleSubmitUpload = () => {
    setUploadLoading(true)
    const formData = new FormData();
    formData.append('BulkEmployerFile', uploadFile)
    if(uploadFile){
      axios({
        method: 'post',
        url: 'tas/employer/bulkupload',
        headers: {
          'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        },
        data: formData,
      }).then((res) => {
        setShowModal(false)
        setUploadFile(null)
        form.submit()
      }).catch((err) => {
  
      }).then(() => setUploadLoading(false))
    }
  }

  const handleSubmitUploadEmployees = () => {
    setUploadLoading(true)
    const formData = new FormData();
    formData.append('BulkEmployerFile', uploadEmpFile)
    if(uploadEmpFile){
      axios({
        method: 'post',
        url: `tas/employer/bulkuploademployees`,
        headers: {
          'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        },
        data: formData,
      }).then((res) => {
        setShowEmpModal(false)
        setUploadEmpFile(null)
        form.submit()
        getData()
      }).catch((err) => {
  
      }).then(() => setUploadLoading(false))
    }
  }


  const errorColumns = [
    {
      label: 'Excel Row Index',
      name: 'ExcelRowIndex',
      alignment: 'left',
      width: '120px',
    },
    {
      label: 'Error',
      name: 'Error',
      alignment: 'left'
    },
  ]

  const handleDownloadRow = () => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/employer/bulkrequestemployees',
      responseType: 'blob',
      data: {
        employerId: selectedExportRow.Id
      }
    }).then(async (res) => {
      const url = await window.URL.createObjectURL(new Blob([res.data]));
      const fn = `TAS_${selectedExportRow.Description}_Employees_${dayjs().format('YYYY-MM-DD hh:mm:ss')}.xlsx`
      const link = await document.createElement('a');
      link.href = url;
      await link.setAttribute('download', fn); //or any other extension
      await document.body.appendChild(link);
      await link.click();
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => {
      setActionLoading(false)
    })
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>Employer Code</div>
        <Form
          form={form}
          fields={fields}
          className='flex gap-x-8' 
          onFinish={handleSearch}
          noLayoutConfig={true}
        >
          <div>
            <Button htmlType='submit' loading={searchLoading} icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Table
        dataSource={renderData}
        id="room"
        className={`overflow-hidden max-h-[calc(100vh-250px)] ${!state.userInfo?.ReadonlyAccess && 'border-t'}`}
        columns={columns}
        showRowLines={true}
        keyExpr={'Id'}
        selection={{mode: 'multiple', recursive: false, showCheckBoxesMode: 'always', allowSelectAll: true, selectAllMode: 'page'}}
        selectedRowKeys={selectedRowKeys}
        rowAlternationEnabled={false}
        onSelectionChanged={handleSelect}
        onEditorPreparing={preparing}
        autoExpandAll={true}
        loading={loading}
        title={!state.userInfo?.ReadonlyAccess ? <div className='flex justify-between py-2 gap-3 border-b'>
          <div className='ml-2'>
            <span className='font-bold'>{selectedRowKeys.length}</span> people selected
          </div>
          <div className='flex gap-3'>
            <Button 
              disabled={selectedRowKeys.length === 0} 
              icon={<DownloadOutlined />} 
              onClick={handleDownload}
              loading={downloadLoading}
            >
              Download{selectedRowKeys.length > 0 ? ` (${selectedRowKeys.length})` : ''}
            </Button>
            <FileUploader 
              icon={<UploadOutlined />} 
              onChange={handleUpload} 
              accept=".xlsx"
              loading={uploadLoading}
            >
              Upload
            </FileUploader>
            <FileUploader 
              icon={<UploadOutlined />} 
              onChange={handleUploadEmployees} 
              accept=".xlsx"
              loading={uploadEmpLoading}
            >
              Upload Employees
            </FileUploader>
          </div>
        </div> : ''}
      />
      <Modal title='Uploaded' open={showModal} onCancel={()=>setShowModal(false)}>
        <div className='mt-5 text-xs'>
          <div className='grid grid-cols-12 gap-2 mb-5'>
            <div className='col-span-4 text-green-500'>Added Row: <Tag className='ml-1' color='green'>{uploadedData?.AddRow}</Tag></div>
            <div className='col-span-4 text-blue-400'>Updated Row: <Tag className='ml-1' color='blue'>{uploadedData?.UpdateRow}</Tag></div>
            <div className='col-span-4 text-red-500'>Deactivated Row: <Tag className='ml-1' color='red'>{uploadedData?.DeleteRow}</Tag></div>
            <div className='col-span-4'>Failed Row: <Tag className='ml-1'>{uploadedData?.FailedRows?.length}</Tag></div>
            <div className='col-span-4'>None Row: <Tag className='ml-1'>{uploadedData?.NoneRow}</Tag></div>
          </div>
          {
            uploadedData?.FailedRows?.length > 0 &&
            <Table columns={errorColumns} data={uploadedData?.FailedRows} containerClass='shadow-none border-t' tableClass='max-h-[100px]'/>
          }
        </div>
        <div className='flex justify-end gap-5 items-center'>
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
      <Modal title='Uploaded Employee' open={showEmpModal} onCancel={()=>setShowEmpModal(false)}>
        <div className='mt-5 text-xs'>
          <div className='grid grid-cols-12 gap-2 mb-5'>
            <div className='col-span-4 text-blue-400'>Updated Row: <Tag className='ml-1' color='blue'>{uploadedEmpData?.UpdateRow}</Tag></div>
            <div className='col-span-4'>Failed Row: <Tag className='ml-1'>{uploadedEmpData?.FailedRows?.length}</Tag></div>
            <div className='col-span-4'>None Row: <Tag className='ml-1'>{uploadedEmpData?.NoneRow}</Tag></div>
          </div>
          {
            uploadedEmpData?.FailedRows?.length > 0 &&
            <Table columns={errorColumns} data={uploadedEmpData?.FailedRows} containerClass='shadow-none border-t' tableClass='max-h-[100px]'/>
          }
        </div>
        <div className='flex justify-end gap-5 items-center'>
          <Button 
            type='primary'
            loading={uploadLoading}
            onClick={() => handleSubmitUploadEmployees()}
          >
            Process
          </Button>
          <Button  
            onClick={() => {setShowEmpModal(false); setUploadEmpFile(null);}}
            disabled={uploadLoading}
          >
            Cancel
          </Button>
        </div>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={'auto'}
        width={350}
      >
        <div>
          <div>Are you sure you want to download this record data?</div>
          <div className='flex gap-5 mt-3 justify-center'>
            <Button type={'success'} onClick={handleDownloadRow} loading={actionLoading}>Yes</Button>
            <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
          </div>
        </div>
      </Popup>
      {contextHolder}
    </div>
  )
}

export default BulkEmployer