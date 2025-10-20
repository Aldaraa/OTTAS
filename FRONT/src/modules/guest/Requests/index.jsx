import React, { useContext, useEffect, useRef, useState } from 'react'
import { Form, Button, CustomTable, Tooltip } from 'components';
import { LeftOutlined } from '@ant-design/icons';
import axios from 'axios';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import ls from 'utils/ls';
import keys from 'lodash/keys';
import SearchTaskForm from './SearchTaskForm';
import { twMerge } from 'tailwind-merge';

const isExpiredSearchValues = (date) => {
  let isExipred = true
  if(date){
    isExipred = (dayjs(date) - dayjs().subtract(1, 'hour')) > 0 ? false : true
  }

  return isExipred
}

const toLink = (e) => {
  if(e.data.DocumentType === 'Non Site Travel'){
    return `/guest/request/nonsitetravel/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'Profile Changes'){
    return `/guest/request/profilechanges/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'De Mobilisation'){
    return `/guest/request/de-mobilisation/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'Site Travel'){
    if(e.data.DocumentTag === 'ADD'){
      return `/guest/request/sitetravel/addtravel/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "RESCHEDULE"){
      return `/guest/request/sitetravel/reschedule/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "REMOVE"){
     return `/guest/request/sitetravel/remove/${e.data.Id}`
    }
  }
}

const defaultColumns = (columns, defaultIndexes) => {
  let initCols = [...columns]
  if(defaultIndexes){
    let colIndexes = keys(defaultIndexes)
    colIndexes.map((colIndex) => {
      initCols[colIndex].groupIndex = defaultIndexes[colIndex]
    })
  }
  return initCols
}

function Task() {
  const cacheData = ls.get('tr')
  const initGrouping = ls.get('grouping')
  const cacheSearch = isExpiredSearchValues(cacheData?.date) ? null : cacheData.sf
  const cacheResult = isExpiredSearchValues(cacheData?.date) ? null : cacheData.rd

  const routeLocation = useLocation();
  const [ data, setData ] = useState(cacheResult ? cacheResult.data : [])
  const [ totalCount, setTotalCount ] = useState(cacheResult ? cacheResult.totalcount : 0)
  const [ pageIndex, setPageIndex ] = useState(cacheSearch ? cacheSearch.pageIndex : 0)
  const [ pageSize, setPageSize ] = useState(cacheSearch ? cacheSearch.pageSize : 100)

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
    dataGrid.current?.instance.beginCustomLoading()
    axios({
      method: 'post',
      url: `tas/requestdocument/documentlist`,
      data: {
        model: values,
        pageIndex: pageIndex,
        pageSize: pageSize,
      }
    }).then((res) => {
      ls.set('tr', {
        rd: res.data,
        sf: {
          model: values,
          pageIndex: pageIndex,
          pageSize: pageSize,
        },
        date: dayjs().format('YYYY-MM-DD HH:mm')
      })
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).finally(() => dataGrid.current?.instance.endCustomLoading())
  }

  const columns = [
    {
      label: '',
      name: 'DaysAway',
      dataType: 'string',
      width: '45px',
      alignment: 'left',
      cellRender: ({value}) => (
        <Tooltip title='Days Away'>
          <div className={twMerge('py-[1px] px-[3px] rounded inline-block text-xs min-w-[30px] text-center', value > 5 ? 'bg-green-600 text-white' : (0 < value && value <= 5) ? 'bg-orange-500 text-white' : 'bg-red-500 text-white')}>{value}</div>
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
          <Link to={toLink(e)}>
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
      width: 120,
      cellRender: (e) => (
        <div className={state.userInfo.Id === e.data.AssignedEmployeeId ? 'font-bold' : ''}>{e.value}</div>
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
    // {
    //   label: '',
    //   name: '',
    //   width: '65px',
    //   alignment: 'center',
    //   showInColumnChooser: false,
    //   cellRender: (e) => (
    //     <div className='flex items-center'>
    //       <Link to={toLink(e)} preventScrollReset={true}>
    //         <button type='button' className='edit-button'>View</button>
    //       </Link>
    //     </div>
    //   )
    // },
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
    if(e.name === 'columns' && e.fullName.includes('groupIndex')){
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
      // ls.set('grouping', prevValues)
    }
  }

  return (
    <div>
      <Link to={'/guest'}>
        <Button icon={<LeftOutlined/>}>Back</Button>
      </Link>
      <SearchTaskForm containerClass='shadow-none border mt-3' form={searchForm} onFinish={getSearchData}/>
      <CustomTable
        ref={dataGrid}
        data={data}
        keyExpr='Id'
        columns={defaultColumns(columns, initGrouping)}
        onChangePageSize={(e) => setPageSize(e)}
        onChangePageIndex={(e) => setPageIndex(e)}
        pageSize={pageSize}
        pageIndex={pageIndex}
        totalCount={totalCount}
        pagerPosition='top'
        isGrouping={true}
        columnAutoWidth={true}
        showColumnLines={false}
        // onRowDblClick={(e) => navigate(toLink(e))}
        wordWrapEnabled={true}
        onOptionChanged={handleOptionChange}
        tableClass='max-h-[calc(100vh-200px)]'
        containerClass='shadow-none border'
      />
    </div>
  )
}

export default Task