import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form, Table, Button } from 'components';
import { SearchOutlined } from '@ant-design/icons';
import axios from 'axios';
import { Form as AntForm, Tag } from 'antd';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { AuthContext } from 'contexts';

import dayjs from 'dayjs';
import { Popup } from 'devextreme-react';
import CustomStore from 'devextreme/data/custom_store';

function CancelRequest() {
  const routeLocation = useLocation();
  const [ employees, setEmployees ] = useState([])
  const [ groups, setGroups ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)

  const [ store ] = useState(new CustomStore({
    key: 'Id',
    load: (loadOptions) => {
      // dataGrid.current?.instance.beginCustomLoading();
      let params = '';
      if(loadOptions.sort?.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      return  axios({
        method: 'post',
        url: `tas/requestdocument/documentlist/cancelled${params}`,
        data: {
          model: loadOptions.filter ? 
          {
            ...loadOptions.filter,
          } :
          {
            startDate: "",
            endDate: "",
            Firstname: "",
            documentType: "",
            employerId: null,
            assignedEmployeeId: null,
            requestedEmployeeId: null,
            lastModifiedDate: "",
          },
          pageIndex: loadOptions.skip/loadOptions.take,
          pageSize: loadOptions.take
        }
      }).then((res) => {
        return {
          data: res.data.data,
          totalCount: res.data.totalcount,
        }
      }).catch((res) => {
        throw res
      })
    }
  }))

  const { state, action} = useContext(AuthContext)
  const navigate = useNavigate()
  const [ searchForm ] = AntForm.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
    getEmployees()
    getGroups()
    action.changeMenuKey('/request/cancelrequest')
  },[])

  const getEmployees = () => {
    axios({
      method: 'get',
      url: 'tas/requestgroupemployee',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({
          value: item.EmployeeId,
          label: item.FullName
        })
      })
      setEmployees(tmp)
    }).catch((err) => {

    })
  }

  const getGroups = () => {
    axios({
      method: 'get',
      url: 'tas/requestgroup'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({
          value: item.Id,
          label: item.Description
        })
      })
      setGroups(tmp)
    }).catch((err) => {

    })
  }

  const fields = [
    {
      label: 'Start Date',
      name: 'StartDate',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'date',
      inputprops: {
        className: 'w-full',
      },
    },
    {
      label: 'End Date',
      name: 'EndDate',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'date',
      inputprops: {
        className: 'w-full',
      },
    },
    {
      label: 'Request #',
      name: 'Id',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'number',
      inputprops: {
        className: 'w-full',
      },
    },
    {
      label: 'Document',
      name: 'DocumentType',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'w-full',
        options: state.referData?.master?.documentTypes,
      },
    },
    {
      label: 'Approvel Type',
      name: 'ApprovelType',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'w-full',
        options: groups
      },
    },
    {
      label: 'Last Modified',
      name: 'LastModifiedDate',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'date',
      inputprops: {
        className: 'w-full',
      },
    },
    // {
    //   label: 'Subject',
    //   name: 'SubjectId',
    //   className: 'col-span-4 mb-2',
    //   type: 'select',
    //   inputprops: {
    //     className: 'w-full',
    //     options: state.referData?.employers,
    //   },
    // },
    {
      label: 'Assigned To',
      name: 'AssignedEmployeeId',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'w-full',
        options: employees,
      },
    },
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'w-full',
        options: state.referData?.employers,
      },
    },
    {
      label: 'Requested By',
      name: 'RequestedEmployeeId',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'max-w-[300px]',
        options: employees,
      },
    },
  ]

  const formItemLayout = {
    labelCol: {
      xs: { span: 24, },
      sm: { span: 8, },
    },
    wrapperCol: {
      xs: { span: 24, },
      sm: { span: 16, },
    },
  };

  const handleDeleteButton = useCallback((dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  },[])

  const handleDelete = useCallback(() => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/requestdocument/remove`,
      data: {
        documentId: editData.Id
      }
    }).then(() => {
      setShowPopup(false)
      // debouncedRefresh()
      // dataGrid.current?.instance.filter(searchForm.getFieldsValue())
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  },[editData, dataGrid])

  const columns = useMemo(() => [
    {
      label: 'DaysAway',
      name: 'DaysAway',
      dataType: 'string',
      width: '75px',
      alignment: 'center',
    },
    {
      label: 'Request #',
      name: 'Id',
      dataType: 'string',
      width: '75px',
      alignment: 'center',
    },
    {
      label: 'CurrentStatus',
      name: 'CurrentStatus',
      alignment: 'left',
      width: '100px',
      cellRender: (e) => (
        <>
        {
          e.value === 'Approved' ?
          <Tag color='blue'>{e.value}</Tag>
          :
          e.value === 'Cancelled' ?
          <Tag color='error'>{e.value}</Tag>
          :
          e.value === 'Completed' ?
          <Tag color='success'>{e.value}</Tag>
          :
          e.value === 'Declined' ?
          <Tag>{e.value}</Tag>
          :
          e.value === 'Saved' ?
          <Tag color='warning'>{e.value}</Tag>
          :
          <Tag color='orange'>{e.value}</Tag>
        }
        </>
      )
    },
    {
      label: 'Assigned Group',
      name: 'AssignedGroupName',
      alignment: 'left',
    },
    {
      label: 'Document',
      name: 'DocumentType',
      alignment: 'left',
      width: '120px',
    },
    {
      label: 'Employer',
      name: 'EmployerName',
      alignment: 'left',
    },
    {
      label: 'Requester',
      name: 'RequesterFullName',
      alignment: 'left',
    },
    {
      label: 'Assigned To',
      name: 'AssignedEmployeeFullName',
      alignment: 'left',
    },
    {
      label: 'Date Requested',
      name: 'RequestedDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
      )
    },
    {
      label: 'Subject',
      name: 'EmployeeFullName',
      alignment: 'left',
    },
    {
      label: 'Updated',
      name: 'UpdateInfo',
      alignment: 'left',
    },
    {
      label: '',
      name: '',
      width: '150px',
      alignment: 'center',
      showInColumnChooser: false,
      cellRender: (e) => (
        <div className='flex items-center gap-3'>
          <Link to={toLink(e)}><button type='button' className='edit-button'>View</button></Link>
          <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
        </div>
      )
    },
  ],[])

  const handleSearch = useCallback((values) => {
    dataGrid.current?.instance.filter(values)
  },[dataGrid])

  const toLink = useCallback((e) => {
    if(e.data.DocumentType === 'Non Site Travel'){
      return `/request/task/nonsitetravel/${e.data.Id}`
    }
    else if(e.data.DocumentType === 'Profile Changes'){
      return `/request/task/profilechanges/${e.data.Id}`
    }
    else if(e.data.DocumentType === 'De Mobilisation'){
      return `/request/task/de-mobilisation/${e.data.Id}`
    }
    else if(e.data.DocumentType === 'Site Travel'){
      if(e.data.DocumentTag === 'ADD'){
        return `/request/task/sitetravel/addtravel/${e.data.Id}`
      }
      else if(e.data.DocumentTag === "RESCHEDULE"){
        return `/request/task/sitetravel/reschedule/${e.data.Id}`
      }
      else if(e.data.DocumentTag === "REMOVE"){
        return `/request/task/sitetravel/remove/${e.data.Id}`
      }
    }
  }, [])
  
  const handleRowClick = useCallback((e) => {
    if(e.data.DocumentType === 'Non Site Travel'){
      navigate(`/request/task/nonsitetravel/${e.data.Id}`)
    }
    else if(e.data.DocumentType === 'Profile Changes'){
      navigate(`/request/task/profilechanges/${e.data.Id}`)
    }
    else if(e.data.DocumentType === 'De Mobilisation'){
      navigate(`/request/task/de-mobilisation/${e.data.Id}`)
    }
    else if(e.data.DocumentType === 'Site Travel'){
      if(e.data.DocumentTag === 'ADD'){
        navigate(`/request/task/sitetravel/addtravel/${e.data.Id}`)
      }
      else if(e.data.DocumentTag === "RESCHEDULE"){
        navigate(`/request/task/sitetravel/reschedule/${e.data.Id}`)
      }
      else if(e.data.DocumentTag === "REMOVE"){
        navigate(`/request/task/sitetravel/remove/${e.data.Id}`)
      }
    }
  },[])

  return (
    <div>
      <div className='rounded-t-ot bg-white px-3 py-2 shadow-md border-b'>
        <div className='flex items-center justify-between mb-2'>
          <div className='text-lg font-bold'>Search Task</div>
        </div>
        <Form 
          {...formItemLayout} 
          form={searchForm} 
          fields={fields}
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSearch}
          size='small'
          wrapperCol={{flex: 1}}
          labelCol={{flex: '100px'}}
        >
          <div className='col-span-12 flex gap-4 justify-end'>
            <Button htmlType='submit' icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Table
        ref={dataGrid}
        data={store}
        columns={columns}
        isHeaderFilter={false}
        isSearch={false}
        allowColumnReordering={false}
        remoteOperations={true}
        onRowDblClick={handleRowClick}
        containerClass='rounded-t-none'
        tableClass='max-h-[calc(100vh-280px)]'
      />
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default CancelRequest