import { Form, Table, Button, Modal, Tooltip } from 'components'
import { Popup } from 'devextreme-react'
import React, { useCallback, useContext, useEffect, useMemo, useState } from 'react'
import { Form as AntForm, Dropdown, Tag, notification } from 'antd'
import { CopyOutlined, DeleteOutlined, EditOutlined, LoadingOutlined, MoreOutlined, PlayCircleFilled, PlayCircleOutlined, PlusOutlined, SaveOutlined, SearchOutlined, UnorderedListOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'  
import ScheduleForm from './AddForm'
import dayjs from 'dayjs'
import advancedFormat from 'dayjs/plugin/advancedFormat'
import relativeTime from 'dayjs/plugin/relativeTime'
import { reportInstance } from 'utils/axios'
import EditScheduleForm from './EditForm'
import Log from './Log'
import { saveAs } from 'file-saver-es'
import { GoPrimitiveDot } from "react-icons/go";
import RunningReport from './RunningReport'

dayjs.extend(advancedFormat)
dayjs.extend(relativeTime)

const title = 'Scheduler'

const dropdownItems = [
  {
    key: 1,
    icon: <EditOutlined/>,
    label: <div className='w-full'>Edit</div>
  },
  {
    key: 2,
    icon: <CopyOutlined/>,
    label: <div className='w-full'>Clone</div>
  },
  {
    key: 3,
    icon: <DeleteOutlined color='red'/>,
    danger: true,
    label: <div className='w-full'>Delete</div>
  },
]

const rendeLastRunDate = (date) => {
  if(dayjs(date).diff(dayjs().subtract(30, 'd'), 'day') < 0){
    return(
      <div>{dayjs(date).format('YYYY MMM DD, HH:mm')}</div>
    )
  }else{
    return(
      <Tooltip title={dayjs(date).format('YYYY MMM DD, HH:mm')}>
        <div>{dayjs(date).fromNow()}</div>
      </Tooltip>
    )
  }
}

function Scheduler() {
  const [ data, setData ] = useState([])
  const [ templates, setTemplates ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ loading, setLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ paramObj, setParamObj ] = useState(null)
  const [ runLoading, setRunLoading ] = useState(null)
  const [ runBuildLoading, setRunBuildLoading ] = useState(false)
  const [ openDrawer, setOpenDrawer ] = useState(false)
  const [ cloneData, setCloneData ] = useState(null)
  const [ showLogModal, setShowLogModal ] = useState(false)
  const [ selectedLogJob, setSelectedLogJob ] = useState(null)



  const [ form ] = AntForm.useForm()
  const [ searchForm ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)
  const [api, contextHolder] = notification.useNotification();

  useEffect(() => {
    action.changeMenuKey('/report/scheduler')
    getData()
    getTemplate()
    getDateTypes()
  },[])

  const getData = () => {
    setLoading(true)
    reportInstance({
      method: 'get',
      url: 'ReportJob'
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getDateTypes = () => {
    if(!state.referData.dateTypes){
      reportInstance.get('tasreport/reporttemplate/datetypes').then((res) => {
        let tmp = res.data.DateVariables?.map((item) => ({
          value: item, 
          label: item, 
        }))
        action.setReferDataItem({ 'dateTypes': tmp })
      }).catch((err) => {
  
      })
    }
  }

  const getTemplate = () => {
    reportInstance({
      method: 'get',
      url: 'tasreport/reporttemplate',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        if(item.Active){
          tmp.push({label: item.Description, value: item.Id, ...item})
        }
      })
      setTemplates(tmp)
    })
  }
  
  const handleAddButton = () => {
    setEditData(null)
    form.resetFields()
    setShowModal(true)
  }

  const handleEditButton = (dataItem) => {
    setCloneData(null)
    setEditData(dataItem)
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    setCloneData(null)
    setEditData(dataItem)
    setShowPopup(true)
  }

  const handleRun = (id) => {
    setRunLoading(id)
    reportInstance({
      method: 'get',
      url: `reportjob/test/${id}`,
      responseType: 'blob',
    }).then((res) => {
      const filename = res.headers['content-disposition'].split("; ")[1].replace('filename=', '')
      saveAs(res.data, filename)
    }).catch(() => {

    }).then(() => setRunLoading(null))
  }

  const handleCloneButton = (row) => {
    setEditData(null)
    setCloneData(row)
    setShowModal(true)
  }

  const handleClickLog = (row) => {
    setShowLogModal(true)
    setSelectedLogJob(row)
  }

  const handleClickDropdown = useCallback((key, e) => {
    if(key == 1){
      handleEditButton(e.data)
    }else if(key == 2){
      handleCloneButton(e.data)
    }else if(key == 3){
      handleDeleteButton(e.data)
    }
  },[])

  const columns = useMemo(() => [
    {
      label: '',
      name: 'Active',
      width: '21px',
      alignment: 'center',
      showInColumnChooser: false,
      allowSorting: false,
      cellRender:(e) => (
        <GoPrimitiveDot color={e.value === 1 ? 'lime' : e.value === 0 ? 'lightgray' : 'red'}/>
      )
    },
    {
      label: 'Report Template',
      name: 'ReportTemplateName',

    },
    {
      label: 'Code',
      name: 'Code',
    },
    {
      label: 'Type',
      name: 'ScheduleType',
      alignment: 'left',
      width: 90,
      cellRender: (e) => (
        <Tag color={e.value === 'Daily' ? 'blue' : e.value === 'Weekly' ? 'purple' : e.value === 'Monthly' ? 'orange' : 'success'}>{e.value}</Tag>
      )
    },
    {
      label: 'Next Run',
      name: 'nextExecuteDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{e.value ? dayjs(e.value).format('YYYY MMMM DD, HH:mm') : ''}</div>
      )
    },
    {
      label: 'Last Run',
      name: 'lastExecuteDate',
      alignment: 'left',
      cellRender: ({value}) => (
        <div>{value ? rendeLastRunDate(value) : null}</div>
      )
    },
    {
      label: 'Last Status',
      name: 'lastExecuteStatus',
      alignment: 'left',
      cellRender: ({value}) => (
        value ?
        <Tag color={value === 'Success' ? 'success' : 'error'}>{value}</Tag>
        : null
      )
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{e.value ? dayjs(e.value).format('YYYY MMMM DD, HH:mm') : ''}</div>
      )
    },
    {
      label: 'End Date',
      name: 'EndDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{e.value ? dayjs(e.value).format('YYYY MMMM DD, HH:mm') : ''}</div>
      )
    },
    {
      label: 'Created User',
      name: 'CreatedUser',
      alignment: 'left',
    },
    {
      label: 'Created Date',
      name: 'CreatedDate',
      alignment: 'left',
      cellRender: (e) => (
        <div>{e.value ? dayjs(e.value).format('YYYY MMMM DD, HH:mm') : ''}</div>
      )
    },
    {
      label: '',
      name: 'action',
      width: '240px',
      cellRender: (e) => (
        <div className='flex gap-3'>
          <button 
            type='button'
            className='edit-button flex items-center gap-2'
            disabled={runLoading}
            onClick={() => handleRun(e.data.Id)}
          >
            { runLoading === e.data.Id ? <LoadingOutlined/> : <PlayCircleFilled/> }
            Run
          </button>
          <button   className='edit-button flex items-center gap-2' onClick={() => handleClickLog(e.data)}>
            <UnorderedListOutlined /> Log
          </button>
          <Dropdown
            trigger="click"
            placement='bottomRight'
            menu={{ onClick: ({key}) => handleClickDropdown(key, e), items: dropdownItems }}
          >
            <Button onClick={(e) => e.stopPropagation()} icon={<MoreOutlined/>}></Button>
          </Dropdown>
        </div>
      )
    },
  ],[runLoading])

  const handleSubmit = (values) => {
    let params = []
    if(values.parameters){
      Object.keys(values.parameters).map((fieldName) => {
        if(values.parameters[fieldName]){
          if(fieldName === 'StartDate' || fieldName === 'EndDate' || fieldName === 'CurrentDate'){
            params.push({
              parameterId: paramObj[fieldName].Id,
              parameterValue: values.parameters[fieldName].FieldValue,
              days: values.parameters[fieldName].Days,
            })
          }else{
            params.push({
              parameterId: paramObj[fieldName].Id,
              parameterValue: JSON.stringify(values.parameters[fieldName])
            })
          }
        }
      })
    }

    ///////////////////////////////   Job Edit   ///////////////////////////////

    if(editData){
      ///////////////////////////////  Edit Daily   ///////////////////////////////
      if(values.scheduleType === 'Daily'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { 
            scheduleStartDate: dayjs(values.startDate).format('YYYY-MM-DD HH:mm'),
            Id: editData.Id,
          }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'put',
              url: 'reportjob/daily',
              data: {
                id: editData.Id,
                name: values.Name,
                description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                startDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
                endDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
                subscriptionMails: values.subscriptionMails,
                recureEvery: values.recureEvery,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Edit Weekly   ///////////////////////////////
      else if(values.scheduleType === 'Weekly'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { 
            scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
            Id: editData.Id,
          }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'put',
              url: 'reportjob/weekly',
              data: {
                id: editData.Id,
                Name: values.Name,
                Description: values.Description,
                columnIds: values.columnIds,
                parameters: params,
                StartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
                EndDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
                subscriptionMails: values.subscriptionMails,
                recureEvery: values.recureEvery,
                days: values.week,
                reportTemplateId: values.reportTemplateId,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Edit Monthly   ///////////////////////////////
      else if(values.scheduleType === 'Monthly'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { 
            scheduleStartDate: dayjs(values.startDate).format('YYYY-MM-DD HH:mm'),
            Id: editData.Id,
          }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'put',
              url: 'reportjob/monthly',
              data: {
                id: editData.Id,
                Name: values.Name,
                Description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                StartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
                EndDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
                subscriptionMails: values.subscriptionMails,
                months: values.months,
                days: values.days,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Edit RunTime   ///////////////////////////////
      else if(values.scheduleType === 'RunTime'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { 
            scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
            Id: editData.Id,
          }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'put',
              url: 'reportjob/runtime',
              data: {
                id: editData.Id,
                Name: values.Name,
                Description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                executeDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
                subscriptionMails: values.subscriptionMails,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
    }
    ///////////////////////////////  Job Clone   ///////////////////////////////
    else if(cloneData){
      if(values.scheduleType === 'Daily'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: {
            scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
          }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/daily',
              data: {
                name: values.Name,
                description: values.Nescription,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                startDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
                endDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
                subscriptionMails: values.subscriptionMails,
                recureEvery: values.recureEvery,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Clone Weekly   ///////////////////////////////
      else if(values.scheduleType === 'Weekly'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/weekly',
              data: {
                Name: values.Name,
                Description: values.Description,
                columnIds: values.columnIds,
                parameters: params,
                StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') : null,
                EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
                subscriptionMails: values.subscriptionMails,
                recureEvery: values.recureEvery,
                days: values.week,
                reportTemplateId: values.reportTemplateId,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Clone Monthly   ///////////////////////////////
      else if(values.scheduleType === 'Monthly'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/monthly',
              data: {
                Name: values.Name,
                Description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') : null,
                EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
                subscriptionMails: values.subscriptionMails,
                months: values.months,
                days: values.days,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Clone RunTime   ///////////////////////////////
      else if(values.scheduleType === 'RunTime'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/runtime',
              data: {
                Name: values.Name,
                Description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                executeDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') : null,
                subscriptionMails: values.subscriptionMails,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
    }
    ///////////////////////////////  Job Create   ///////////////////////////////
    else {
      if(values.scheduleType === 'Daily'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/daily',
              data: {
                name: values.Name,
                description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                startDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm'),
                endDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
                subscriptionMails: values.subscriptionMails,
                recureEvery: values.recureEvery,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Create Weekly   ///////////////////////////////
      else if(values.scheduleType === 'Weekly'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/weekly',
              data: {
                Name: values.Name,
                Description: values.Description,
                columnIds: values.columnIds,
                parameters: params,
                StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') : null,
                EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
                subscriptionMails: values.subscriptionMails,
                recureEvery: values.recureEvery,
                days: values.week,
                reportTemplateId: values.reportTemplateId,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Create Monthly   ///////////////////////////////
      else if(values.scheduleType === 'Monthly'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/monthly',
              data: {
                Name: values.Name,
                Description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') : null,
                EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
                subscriptionMails: values.subscriptionMails,
                months: values.months,
                days: values.days,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
      ///////////////////////////////   Create RunTime   ///////////////////////////////
      else if(values.scheduleType === 'RunTime'){
        setActionLoading(true)
        reportInstance({
          method: 'post',
          url: 'reportjob/validatetime',
          data: { scheduleStartDate: dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') }
        }).then(({data}) => {
          if(data){
            reportInstance({
              method: 'post',
              url: 'reportjob/runtime',
              data: {
                Name: values.Name,
                Description: values.Description,
                reportTemplateId: values.reportTemplateId,
                columnIds: values.columnIds,
                parameters: params,
                executeDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD HH:mm') : null,
                subscriptionMails: values.subscriptionMails,
              }
            }).then(() => {
              setShowModal(false)
              getData()
            }).catch(() => {
      
            }).then(() => setActionLoading(false))
          }else{
            api.error({
              message: 'Duplicated Starting on date',
              duration: 5,
              description: 'Please change Starting on date'
            });
          }
        }).finally(() => setActionLoading(false))
      }
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    reportInstance({
      method: 'delete',
      url: `reportjob/delete/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = (values) => {
    setLoading(true)
    reportInstance({
      method: 'get',
      url: `ReportJob?templateId=${values.templateId ? values.templateId : ''}&keyword=${values.Keyword}`,
    }).then((res) => {
      setData(res.data)
    }).finally(() => setLoading(false))
  }

  const searchFields = [
    {
      label: 'Type',
      name: 'templateId',
      className: 'col-span-3 mb-2',
      type: 'select',
      inputprops: {
        options: templates
      }
    },
    {
      label: 'Keyword',
      name: 'Keyword',
      className: 'col-span-3 mb-2',
    },
  ]

  const handleCancel = () => {
    setShowModal(false)
  }

  const handleRunBuild = () => {
    setRunBuildLoading(true)
    const values = form.getFieldsValue()
    let params = []
    if(values.parameters){
      Object.keys(values.parameters).map((fieldName) => {
        if(values.parameters[fieldName]){
          if(fieldName === 'StartDate' || fieldName === 'EndDate' || fieldName === 'CurrentDate'){
            params.push({
              parameterId: paramObj[fieldName].Id,
              parameterValue: values.parameters[fieldName].FieldValue,
              Days: values.parameters[fieldName].Days
            })
          }else{
            params.push({
              parameterId: paramObj[fieldName].Id,
              parameterValue: JSON.stringify(values.parameters[fieldName])
            })
          }
        }
      })
    }
    reportInstance({
      method: 'post',
      url: 'reportjob/buildreport',
      responseType: 'blob',
      data: {
        reportTemplateId: values.reportTemplateId,
        columnIds: values.columnIds,
        parameters: params,
      }
    }).then((res) => {
      const filename = res.headers['content-disposition'].split("; ")[1].replace('filename=', '')
      saveAs(res.data, filename)
      setShowModal(false)
    }).catch(() => {

    }).then(() => setRunBuildLoading(false))
  }

  return (
    <div>
      <div className='rounded-ot bg-white p-5 mb-5 shadow-md'>
        <div className='text-lg font-bold mb-3'>{title}</div>
        <div className='w-full'>
          <Form 
            form={searchForm} 
            fields={searchFields} 
            className='grid grid-cols-12 gap-x-8' 
            onFinish={handleSearch}
            noLayoutConfig={true}
          >
            <div className='flex col-span-2 items-baseline'>
              <Button 
                htmlType='submit' 
                className='flex items-center' 
                loading={loading} 
                icon={<SearchOutlined/>}
              >
                Search
              </Button>
            </div>
          </Form>
        </div>
      </div>
      <Table
        data={data}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        tableClass='border-t max-h-[calc(100vh-285px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{data.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <div>
            {
              state.userInfo?.Role === 'SystemAdmin' ?
              <Button className='mr-4' onClick={() => setOpenDrawer(true)}>Running reports</Button>
              : null
            }
            <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add {title}</Button>
          </div>
        </div>}
      />
      <Modal 
        footer={
          <div className='col-span-12 flex justify-end items-center gap-2'>
            {
              cloneData ? 
              <>
                <Button type='primary' disabled={runBuildLoading} onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>
                  Save
                </Button>
                <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
              </>
              :
              <>
                { editData ? 
                  <Button type='primary' onClick={handleRunBuild} loading={runBuildLoading} icon={<PlayCircleOutlined />}>Run</Button>
                  :
                  null
                }
                <Button type='primary' disabled={runBuildLoading} onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>}>
                  Save
                </Button>
                <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
              </>
            }
          </div>
        } 
        open={showModal} 
        width={1100} 
        onCancel={() => setShowModal(false)} 
        title={cloneData ? `Clone ${title}` : editData ? `Edit ${title}` : `Add ${title}`}
      >
        <Form
          labelAlign='left'
          form={form}
          onFinish={handleSubmit}
          className={'gap-x-4'}
          labelCol={{sm: { span: 6}}}
          initValues={{scheduleType: 'Daily'}}
        >
          {
            (editData || cloneData) ?
            <EditScheduleForm
              onChangeParameters={(e) => setParamObj(e)}
              editData={cloneData ? cloneData : editData}
              templates={templates}
              form={form}
            />
            :
            <ScheduleForm
              onChangeParameters={(e) => setParamObj(e)}
              templates={templates}
              form={form}
            />
          }
        </Form>
      </Modal>
      <RunningReport open={openDrawer} onClose={() => setOpenDrawer(false)}/>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to {editData?.Active === 1 ? 'deactivate' : 'reactivate'} this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={editData?.Active === 1 ? 'danger' : 'success'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
      <Log show={showLogModal} onCancel={() => setShowLogModal(false)} data={selectedLogJob}/>
      {contextHolder}
    </div>
  )
}

export default Scheduler