import axios from 'axios'
import { Button, FileUploader, Form, Table, Modal } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import tableSearch from 'utils/TableSearch'
import { Form as AntForm, Tag, notification } from 'antd'
import { DownloadOutlined, SearchOutlined, UploadOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import validateFile from 'utils/validateUpload'

function BulkRoom() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ roomTypes, setRoomTypes ] = useState([])
  const [ camps, setCamps ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ selectedRowKeys, setSelectedRowKeys ] = useState([])
  const [ downloadLoading, setDownloadLoading ] = useState(false)
  const [ uploadLoading, setUploadLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ uploadedData, setUploadedData ] = useState(null)
  const [ uploadFile, setUploadFile ] = useState(null)

  const { state } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const [api, contextHolder] = notification.useNotification();

  useEffect(() => {
    getData()
    getCamps()
    getRoomTypes()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/room'
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

  const getRoomTypes = () => {
    axios({
      method: 'get',
      url: 'tas/roomtype?Active=1'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({label: item.Description, value: item.Id}))
      setRoomTypes(tmp)
    }).catch((err) => {
  
    })
  }

  const getCamps = () => {
    axios({
      method: 'get',
      url: 'tas/camp?Active=1'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => tmp.push({label: item.Description, value: item.Id}))
      setCamps(tmp)
    }).catch((err) => {
  
    })
  }

  const fields = [
    {
      label: 'Room Number',
      name: 'Number',
      className: ' col-span-6 xl:col-span-3 mb-2'
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      className: 'col-span-6 xl:col-span-2 mb-2',
      type: 'number',
      inputprops: {
        className: 'w-full',
      }
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      className: 'col-span-6 xl:col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options: roomTypes
      }
    },
    {
      label: 'Camp',
      name: 'CampId',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        options: camps
      }
    },
    {
      label: 'Private',
      name: 'Private',
      className: 'col-span-2 xl:col-span-1 mb-2',
      type: 'check',
      inputprops: {
      }
    },
    {
      label: 'Active',
      name: 'Active',
      className: 'col-span-2 xl:col-span-1 mb-2',
      type: 'check',
      // hide: true,
      inputprops: {
        indeterminatewith: true,
      }
    },
    {
      label: 'No Room',
      name: 'VirtualRoom',
      className: 'col-span-3 lg:col-span-2 mb-2',
      type: 'check',
      inputprops: {
      }
    },
  ]

  const columns = [
    {
      label: 'Room Number',
      name: 'Number',
      width: '150px'
    },
    {
      label: 'Camp Name',
      name: 'CampName'
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      width: '80px'
    },
    {
      label: 'Room Type',
      name: 'RoomTypeName'
    },
    // {
    //   label: 'No Room',
    //   name: 'VirtualRoom',
    //   width: '100px',
    //   alignment: 'center',
    //   cellRender: (e) => (
    //     <CheckBox disabled iconSize={18} value={e.data.VirtualRoom === 1 ? true : 0}/>
    //   )
    // },
    {
      label: 'Private',
      name: 'Private',
      width: '60px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Private === 1 ? true : 0}/>
      )
    },
    {
      label: 'Active',
      name: 'Active',
      width: '90px',
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
      e.editorOptions.disabled = true;
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
      url: 'tas/room/bulkrequest',
      responseType: 'blob',
      data: {
        roomIds: selectedRowKeys
      }
    }).then((res) => {
      const url = window.URL.createObjectURL(new Blob([res.data]));
      const fn = `TAS_Room_Bulk_Download_${dayjs().format('YYYY-MM-DD hh:mm:ss')}.xlsx`
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
      formData.append('BulkRoomFile', file)
      if(file){
        axios({
          method: 'post',
          url: 'tas/room/bulkuploadpreview',
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
    formData.append('BulkRoomFile', uploadFile)
    if(uploadFile){
      axios({
        method: 'post',
        url: 'tas/room/bulkupload',
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

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='w-full'>
          <div className='text-lg font-bold mb-2'>Bulk Room</div>
          <Form
            form={form}
            fields={fields}
            className='grid grid-cols-12 gap-x-8' 
            onFinish={handleSearch}
            noLayoutConfig={true}
            size='small'
          >
            <div className='col-span-12 flex justify-end'>
              <Button htmlType='submit' loading={searchLoading} icon={<SearchOutlined/>}>Search</Button>
            </div>
          </Form>
        </div>
      </div>
      <Table
        dataSource={renderData}
        id="room"
        columns={columns}
        showRowLines={true}
        keyExpr={'Id'}
        selection={!state.userInfo?.ReadonlyAccess && {mode: 'multiple', recursive: false, showCheckBoxesMode: 'always',}}
        onSelectionChanged={handleSelect}
        onEditorPreparing={preparing}
        autoExpandAll={true}
        loading={loading}
        tableClass='max-h-[calc(100vh-305px)]'
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
              Download
            </Button>
            <FileUploader 
              icon={<UploadOutlined />} 
              onChange={handleUpload} 
              accept=".xlsx"
              loading={uploadLoading}
            >
              Upload
            </FileUploader>
          </div>
        </div> : ''}
      />
      <Modal title='Uploaded' open={showModal} onCancel={()=>setShowModal(false)}>
        <div>
          <div className='grid grid-cols-12 gap-2 mb-5'>
            <div className='col-span-6 text-green-500'>Added Row: <Tag className='ml-1' color='green'>{uploadedData?.AddRow}</Tag></div>
            <div className='col-span-6'>None Row: <Tag className='ml-1'>{uploadedData?.NoneRow}</Tag></div>
            <div className='col-span-6 text-red-500'>Deactivated Row: <Tag className='ml-1' color='red'>{uploadedData?.DeleteRow}</Tag></div>
            <div className='col-span-6'>Failed Row: <Tag className='ml-1'>{uploadedData?.FailedRows?.length}</Tag></div>
            <div className='col-span-6 text-blue-400'>Updated Row: <Tag className='ml-1' color='blue'>{uploadedData?.UpdateRow}</Tag></div>
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
      {contextHolder}
    </div>
  )
}

export default BulkRoom