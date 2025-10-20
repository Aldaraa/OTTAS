import React, { useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form, Button, CustomTable, Modal } from 'components';
import { ControlOutlined, SearchOutlined } from '@ant-design/icons';
import axios from 'axios';
import { Form as AntForm, Checkbox } from 'antd';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import ls from 'utils/ls';

function arrayEquals(a, b) {
  let boolean = false
  if(a.length === b.length){
    a.forEach((item) => {
      boolean = b.includes(item);
    })
  }
  return boolean;
}

const defaultFields = ['StartDate', 'EndDate', 'Id', 'DocumentType', 'requestDocumentSearchCurrentStep']
const allFields = [
  "StartDate",
  "EndDate",
  "Id",
  "DocumentType",
  'requestDocumentSearchCurrentStep',
  "ApprovelType",
  "LastModifiedDate",
  "AssignedEmployeeId",
  "EmployerId",
  "RequestedEmployeeId",
  "Keyword",
]

const filterFields = [
  {
    label: 'Start Date',
    value: 'StartDate',
  },
  {
    label: 'End Date',
    value: 'EndDate',
  },
  {
    label: 'Request #',
    value: 'Id',
  },
  {
    label: 'Document Type',
    value: 'DocumentType',
  },
  {
    label: 'Current Step',
    value: 'requestDocumentSearchCurrentStep',
  },
  {
    label: 'Approvel Type',
    value: 'ApprovelType',
  },
  {
    label: 'Last Modified',
    value: 'LastModifiedDate',
  },
  {
    label: 'Assigned To',
    value: 'AssignedEmployeeId',
  },
  {
    label: 'Employer',
    value: 'EmployerId',
  },
  {
    label: 'Requested By',
    value: 'RequestedEmployeeId',
  },
  {
    label: 'Keyword',
    value: 'Keyword',
  },
]

const isExpiredSearchValues = (date) => {
  let isExipred = true
  if(date){
    isExipred = (dayjs(date) - dayjs().subtract(1, 'hour')) > 0 ? false : true
  }

  return isExipred
}

function SearchTaskForm({onFinish, form}) {
  const cacheData = ls.get('tr')
  const tFealds = ls.get('tFields') ? ls.get('tFields') : []
  const cacheSearch = isExpiredSearchValues(cacheData?.date) ? null : cacheData.sf
  const [ employees, setEmployees ] = useState([])
  const [ groups, setGroups ] = useState([])
  const [ selectedFields, setSelectedFields ] = useState(tFealds.length > 0 ? tFealds : defaultFields)
  const [ indeterminate, setIndeterminate] = useState(true);
  const [ checkAll, setCheckAll] = useState(false);
  const [ showModal, setShowModal ] = useState(false)

  const { state } = useContext(AuthContext)
  const authContext = useContext(AuthContext)

  useEffect(() => {
    getEmployees()
    getGroups()
  },[])

  const getEmployees = () => {
    axios({
      method: 'get',
      url: 'tas/requestgroupemployee',
    }).then((res) => {
      let tmp = []
      res.data?.map((item) => {
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
      res.data?.map((item) => {
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
      label: 'Document Type',
      name: 'DocumentType',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'w-full',
        allowClear: false,
        options: state.referData?.master?.documentTypes ? 
          [{value: null, label: 'All'}, ...state.referData?.master?.documentTypes] 
          : 
          [],
      },
    },
    {
      label: 'Current Step',
      name: 'requestDocumentSearchCurrentStep',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'w-full',
        allowClear: false,
        options: state.referData?.master?.searchCurrentSteps ? [
          {value: null, label: 'All'}, 
          ...state.referData?.master?.searchCurrentSteps
        ] : [],
      },
    },
    {
      label: 'Approvel Type',
      name: 'ApprovelType',
      className: 'col-span-6 xl:col-span-4 mb-2',
      type: 'select',
      inputprops: {
        className: 'w-full',
        options: groups.filter((item) => item.label === 'OT badging' || item.label === 'Data team' ? false : true)
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
        options: authContext.state.referData?.employers,
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
    {
      label: 'Keyword',
      name: 'Keyword',
      className: 'col-span-6 xl:col-span-4 mb-2',
    },
  ]

  const searchFields = useMemo(() => {
    return fields.filter((item) => selectedFields.includes(item.name))
  },[selectedFields, employees, groups])

  const onCheckAllChange = (e) => {
    let selectedKeys = e.target.checked ? allFields : []
    ls.set('tFields', selectedKeys);
    setSelectedFields(selectedKeys);
    setIndeterminate(false);
    setCheckAll(e.target.checked);
  };
  
  const onCheckDefaultChange = (e) => {
    let selectedKeys = e.target.checked ? defaultFields : [] 
    ls.set('tFields', selectedKeys);
    setSelectedFields(selectedKeys);
    setIndeterminate(selectedKeys.length < allFields.length);
  }

  const onCheckChange = (list) => {
    ls.set('tFields', list);
    let allFields = filterFields.map((item) => item.value)
    setSelectedFields(list);
    setIndeterminate(!!list.length && list.length < allFields.length);
    setCheckAll(list.length === allFields.length);
  }

  return (
    <div>
      <div className='rounded-t-ot bg-white px-3 py-2 shadow-md border-b'>
        <div className='flex items-center justify-between mb-2'>
          <div className='text-lg font-bold mb-3'>Search Task</div>
          <Button 
            htmlType='button' 
            onClick={() => setShowModal(true)} 
            className='px-2'
            icon={<ControlOutlined style={{fontSize: '16px'}}/>}
          >
            Filter
          </Button>
        </div>
        <Form 
          form={form} 
          fields={searchFields}
          className='grid grid-cols-12 gap-x-8' 
          onFinish={onFinish}
          size='small'
          initValues={cacheSearch ? 
            {
              ...cacheSearch.model,
              StartDate: cacheSearch.model.StartDate ? dayjs(cacheSearch.model.StartDate) : null,
              EndDate: null,
            } 
            : 
            {
              StartDate: dayjs().subtract(14, 'day'),
              EndDate: null,
              Id: "",
              DocumentType: null,
              requestDocumentSearchCurrentStep: "Pending",
              ApprovelType: null,
              LastModifiedDate: null,
              AssignedEmployeeId: null,
              EmployerId: null,
              RequestedEmployeeId: null,
              Keyword: "",
            }
          }
        >
          <div className='col-span-12 flex gap-4 justify-end'>
            <Button htmlType='submit' icon={<SearchOutlined/>}>Search</Button>
          </div>
        </Form>
      </div>
      <Modal open={showModal} onCancel={() => setShowModal(false)} title='Filter fields'>
        <div className='pb-2 mb-2 border-b'>
          <Checkbox key={'selectAll'} indeterminate={indeterminate} checked={checkAll} onChange={onCheckAllChange}>Select All</Checkbox>
          <Checkbox key={'default'} checked={arrayEquals(defaultFields, selectedFields)} onChange={onCheckDefaultChange}>
            Default
          </Checkbox>
        </div>
        <Checkbox.Group style={{width:'100%'}} value={selectedFields} onChange={onCheckChange}>
          <div className='grid grid-cols-12 gap-x-5'>
            {filterFields.map((item, i) => (
              <div className='col-span-6'>
                <Checkbox value={item.value} key={`filter-check-item-${i}`}>{item.label}</Checkbox>
              </div>
            ))}
          </div>
        </Checkbox.Group>
      </Modal>
    </div>
  )
}

export default SearchTaskForm