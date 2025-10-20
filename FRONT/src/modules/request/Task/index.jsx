import React, { useCallback, useContext, useEffect, useRef, useState } from 'react'
import { Button, CustomTable, Modal, Tooltip } from 'components';
import axios from 'axios';
import { Form } from 'antd';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import ls from 'utils/ls';
import keys from 'lodash/keys';
import SearchTaskForm from './SearchTaskForm';
import { twMerge } from 'tailwind-merge';
import { UserSwitchOutlined } from '@ant-design/icons';
import ReAssignLM from './ReAssignLM';

const isValidRequest = (data) => {
  return !(data?.CurrentStatus === 'Declined' || data?.CurrentStatus === 'Completed' || data?.CurrentStatus === 'Cancelled')
}

const isExpiredSearchValues = (date) => {
  let isExipred = true
  if(date){
    isExipred = (dayjs(date) - dayjs().subtract(1, 'hour')) > 0 ? false : true
  }

  return isExipred
}

const toLink = (e) => {
  if(e.data.DocumentType === 'Non Site Travel'){
    return `/request/task/nonsitetravel/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'Profile Changes'){
    return `/request/task/profilechanges/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'De Mobilisation'){
    return `/request/task/de-mobilisation/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'ExternalTravel'){
    if(e.data.DocumentTag === 'ADD'){
      return `/request/task/externaltravel/addtravel/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "RESCHEDULE"){
      return `/request/task/externaltravel/reschedule/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "REMOVE"){
     return `/request/task/externaltravel/remove/${e.data.Id}`
    }
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
}

const defaultColumns = (columns, defaultIndexes, defaultSortIndex) => {
  let initCols = [...columns]
  if(defaultIndexes){
    let colIndexes = keys(defaultIndexes)
    colIndexes.map((colIndex) => {
      initCols[colIndex].groupIndex = defaultIndexes[colIndex]
    })
  }
  if(defaultSortIndex){
    let colIndexes = keys(defaultSortIndex)
    colIndexes.map((colIndex) => {
      initCols[colIndex].defaultSortOrder = defaultSortIndex[colIndex]
    })
  }
  return initCols
}

const dayColor = (day) => {
  let color = ''
  if(day < 0){
    color = 'bg-orange-500 text-white'
  }else if(day === 0 || day === 1){
    color = 'bg-red-500 text-white'
  }else if(day === 2 || day === 3){
    color = 'bg-yellow-500 text-white'
  }
  return color
}

function Task() {
  const cacheData = ls.get('tr')
  const initGrouping = ls.get('grouping')
  const initSorting = ls.get('sorting')
  const cacheSearch = isExpiredSearchValues(cacheData?.date) ? null : cacheData.sf
  const cacheResult = isExpiredSearchValues(cacheData?.date) ? null : cacheData.rd

  const routeLocation = useLocation();
  const [ data, setData ] = useState(cacheResult ? cacheResult.data : [])
  const [ totalCount, setTotalCount ] = useState(cacheResult ? cacheResult.totalcount : 0)
  const [ pageIndex, setPageIndex ] = useState(cacheSearch ? cacheSearch.pageIndex : 0)
  const [ pageSize, setPageSize ] = useState(cacheSearch ? cacheSearch.pageSize : 100)
  const [ selectedRow, setSelectedRow ] = useState(null)
  const [ showReAssignModal, setShowReAssignModal ] = useState(false)

  const { state } = useContext(AuthContext)
  const authContext = useContext(AuthContext)
  const navigate = useNavigate()
  const [ searchForm ] = Form.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
    if(!cacheData){
      getData()
    }
    authContext.action.changeMenuKey('/request/task')
  },[])

  useEffect(() => {
    if(data.length > 0){
      getData()
    }
  },[pageIndex, pageSize])

  const getData = () => {
    dataGrid.current?.instance.beginCustomLoading()
    axios({
      method: 'post',
      url: `tas/requestdocument/documentlist`,
      data: {
        model: searchForm.getFieldsValue(),
        pageIndex: pageIndex,
        pageSize: pageSize
      }
    }).then((res) => {
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).finally(() => dataGrid.current?.instance.endCustomLoading())
  }

  const getSearchData = (values) => {
    setPageIndex(0)
    dataGrid.current?.instance.beginCustomLoading()
    axios({
      method: 'post',
      url: `tas/requestdocument/documentlist`,
      data: {
        model: {
          ...values,
          StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : null,
          EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
        },
        pageIndex: 0,
        pageSize: pageSize,
      }
    }).then((res) => {
      ls.set('tr', {
        rd: res.data,
        sf: {
          model: {
            ...values,
            StartDate: values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : null,
            EndDate: values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : null,
          },
          pageIndex: 0,
          pageSize: pageSize,
        },
        date: dayjs().format('YYYY-MM-DD HH:mm')
      })
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).finally(() => dataGrid.current?.instance.endCustomLoading())
  }

  const handleClickReAssignBtn = useCallback((row) => {
    setSelectedRow(row)
    setShowReAssignModal(true)
  },[])

  const columns = [
    {
      label: '',
      name: 'DaysAway',
      dataType: 'string',
      width: '45px',
      alignment: 'left',
      cellRender: ({value}) => (
        <Tooltip title='Days Away'>
          <div className={twMerge('py-[1px] px-[3px] rounded inline-block text-xs min-w-[30px] text-center', dayColor(value))}>{value}</div>
        </Tooltip>
      )
    },
    {
      label: '#',
      name: 'Id',
      dataType: 'string',
      width: '65px',
      // alignment: 'center',
      cellRender: (e) => (
        <div className='flex items-center text-blue-500 hover:underline'>
          <Link to={toLink(e)} target='_blank'>
            {e.value}
          </Link>
        </div>
      )
    },
    {
      label: 'CurrentStatus',
      name: 'CurrentStatusGroup',
      alignment: 'left',
      // width: '110px'
    },
    {
      label: 'Description',
      name: 'Description',
      alignment: 'left',
      width: 380,
      minWidth: 100,
      cellRender: ({value}) => (
        <div className='truncate whitespace-pre-wrap'>{value}</div>
      )
      // width: '260px',
    },
    {
      label: 'Employer',
      name: 'EmployerName',
      alignment: 'left',
    },
    {
      label: 'Subject',
      name: 'EmployeeFullName',
      alignment: 'left',
      // width: 120,
    },
    {
      label: 'Date Requested',
      name: 'RequestedDate',
      alignment: 'left',
      width: 80,
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
      )
    },
    {
      label: 'Assigned To',
      name: 'AssignedEmployeeFullName',
      alignment: 'left',
      cellRender: ({value, data}) => (
        <div className='flex gap-2 items-center'>
          <div className={state.userInfo.Id === data.AssignedEmployeeId ? 'font-bold' : ''}>{value}</div>
          {
            (state.userInfo?.Role === "SystemAdmin" && value && isValidRequest(data)) ?
            <Tooltip title='Reassign Line Manager'>
              <Button className='px-[5px]' type='text' iconSize={18} icon={<UserSwitchOutlined />} onClick={() => handleClickReAssignBtn(data)}/>
            </Tooltip>
            : null
          }
        </div>
      )
    },
    {
      label: 'Requester',
      name: 'RequesterFullName',
      alignment: 'left',
      width: 120,
    },
    {
      label: 'Updated',
      name: 'UpdatedInfo',
      alignment: 'left',
    },
  ]

  const handleRowClick = (e) => {
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
  }

  const handleOptionChange = (e) => {
    if(e.name === 'columns'){
      if(e.fullName.includes('groupIndex')){
        let prevValues = ls.get('grouping') ? ls.get('grouping') : {}
        if(typeof e.value !== undefined){
          prevValues[e.fullName[8]] = e.value
          ls.set('grouping', prevValues)
        }else if(typeof e.previousValue !== undefined){
          let columnIndexes = keys(prevValues)
          columnIndexes.filter((index) => index !== e.fullName[8])
          let tmp = {}
          columnIndexes.map((colIndex, i) => tmp[colIndex] = i)
          ls.set('grouping', tmp)
        }
      }else if(e.fullName.includes('sortOrder')){
        let prevValues = ls.get('sorting') ? ls.get('sorting') : {}
        if(typeof e.value !== undefined){
          prevValues[e.fullName[8]] = e.value
          ls.set('sorting', prevValues)
        }else if(typeof e.previousValue !== undefined){
          let columnIndexes = keys(prevValues)
          columnIndexes.filter((index) => index !== e.fullName[8])
          let tmp = {}
          columnIndexes.map((colIndex, i) => tmp[colIndex] = i)
          ls.set('sorting', tmp)
        }
      }
    }
  }

  const handleReload = () => {
    searchForm.submit()
  }

  return (
    <div>
      <SearchTaskForm form={searchForm} onFinish={getSearchData}/>
      <CustomTable
        ref={dataGrid}
        data={data}
        keyExpr='Id'
        columns={defaultColumns(columns, initGrouping, initSorting)}
        onChangePageSize={(e) => setPageSize(e)}
        onChangePageIndex={(e) => setPageIndex(e)}
        pageSize={pageSize}
        pageIndex={pageIndex}
        totalCount={totalCount}
        pagerPosition='top'
        isGrouping={true}
        columnAutoWidth={true}
        showColumnLines={false}
        onRowDblClick={(e) => navigate(toLink(e))}
        wordWrapEnabled={true}
        onOptionChanged={handleOptionChange}
        tableClass='max-h-[calc(100vh-305px)]'
        containerClass='rounded-t-none'
      />
      <ReAssignLM
        open={showReAssignModal}
        selectedRow={selectedRow}
        onCancel={() => setShowReAssignModal(false)}
        reload={handleReload}
      />
    </div>
  )
}

export default Task