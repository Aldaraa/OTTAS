import { CarFilled, CheckOutlined, CloseCircleFilled, CloseOutlined, HomeFilled } from '@ant-design/icons'
import { Alert, Form as AntForm, InputNumber, Select, notification } from 'antd'
import axios from 'axios'
import { Button, Form, Table, Modal, Calendar, DatePicker } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { BsAirplaneFill } from 'react-icons/bs'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import hexToHSL from 'utils/hexToHSL'
import ls from 'utils/ls'

const dateFormat = 'YYYY-MM-DD'

const ConfirmedRosters = ({data=[]}) => {
  const columns = [
    {
      label: 'Date',
      name: 'EventDate',
      cellRender: (e) => (
        <span>{dayjs(e.value).format('YYYY-MM-DD')}</span>
      )
    },
    {
      label: 'TransportCode',
      name: 'TransportCode',
    },
    {
      label: 'TransportMode',
      name: 'TransportMode',
    },
    {
      label: 'Direction',
      name: 'Direction',
    },
    {
      label: 'Shift',
      name: 'ShiftCode',
      width: 50,
      cellRender: (e) => (
        <div className='text-center rounded ' style={{background:e.data.ShiftColorCode, color: hexToHSL(e.data.ShiftColorCode) > 60 ? 'black' : 'white'}}>
          {e.value}
        </div>
      )
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Seats',
      name: 'Seats',
    },
    {
      label: 'Available seats',
      name: 'Confirmed',
      cellRender: ({data}) => {
        const availableSeats = data.Seats-data.Confirmed-data.OverBooked
        return <div className={availableSeats <= 0 ? 'text-red-400' : 'text-green-400'}>{availableSeats}</div>
      }
    },
  ]

  return(
    <Table 
      data={data}
      columns={columns}
      containerClass='shadow-none p-0 border overflow-hidden'
      tableClass='max-h-[600px]'
    />
  )
}

function RosterDetail({data}) {
  // const [ data, setData ] = useState(null)
  const [ previewData, setPreviewData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  // const [ warningModal, setWarningModal ] = useState(false)
  const [ formValues, setFormValues ] = useState(null)
  // const [ errorMessage, setErrorMessage ] = useState([])
  const [ annualYearData, setAnnualYearData ] = useState([])
  const [ calendarDate, setCalendarDate ] = useState(false)
  const [ rosterAlert, setRosterAlert ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showResultModal, setShowResultModal ] = useState(false)
  const [ resultData, setResultData ] = useState([])
  
  const {employeeId} = useParams()
  const { state, action } = useContext(AuthContext)
  const [form] =  AntForm.useForm()
  const [ api, contextHolder ] = notification.useNotification()

  useEffect(() => {
    ls.set('pp_rt', 'rosterexecute')
  },[]) // eslint-disable-line
  
  useEffect(() => {
    form.setFieldsValue({...state.userProfileData, RoomId: ''})
  },[state.userProfileData]) // eslint-disable-line

  const handleChangeFieldValue = () => {
    if(form.getFieldValue('startDate') && form.getFieldValue('monthDuration')){
      setTimeout(() => {
        axios({
          method: 'get',
          url: `tas/employeestatus/annualyear/${employeeId}/${dayjs(form.getFieldValue('startDate')).format('YYYY-MM-DD')}/${form.getFieldValue('monthDuration')}`
        }).then((res) => {
          setAnnualYearData(res.data)
        }).catch((err) => {

        })
      },[])
    }
  }

  const fields = [
    {
      type: 'component',
      component: <Form.Item label='Start Date' name={'startDate'} className='col-span-12 mb-2' rules={[{required: true, message: 'Start Date is required'}]}>
        <DatePicker onChange={handleChangeFieldValue} allowClear={false} showWeek/>
      </Form.Item>,
    },
    {
      type: 'component',
      component: 
        <Form.Item label='Roster Duration (months)' name={'monthDuration'} className='col-span-12 mb-2' rules={[{required: true, message: 'Roster Duration is required'}]}>
          <InputNumber controls={false} onChange={handleChangeFieldValue} min={1} max={18}/>
        </Form.Item>
    },
    {
      label: 'Roster ',
      name: 'RosterId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Roster is required'}],
      inputprops: {
        options: state.referData?.rosters,
        allowClear: false,
      }
    },
    {
      label: 'Department',
      name: 'DepartmentId',
      className: 'col-span-12 mb-2',
      type: 'treeSelect',
      rules: [{required: true, message: 'Department is required'}],
      inputprops: {
        treeData: state.referData?.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      }
    },
    {
      label: 'Position',
      name: 'PositionId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Position is required'}],
      inputprops: {
        options: state.referData?.positions,
        allowClear: true
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.DepartmentId !== cur.DepartmentId}>
        {({getFieldValue, setFieldValue}) => {
          let selectedDepartment = state.referData.departments?.find((department) => department.Id === getFieldValue('DepartmentId'))
          if(selectedDepartment && selectedDepartment.CostCodeId){
            setFieldValue('CostCodeId', selectedDepartment.CostCodeId)
          }
          return (
            <Form.Item 
              name='CostCodeId' 
              label='CostCodeId'
              rules={[{required: true, message: 'Cost Code is required'}]}
              className='col-span-12 mb-2'
            >
              <Select
                options={state.referData.costCodes}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Employer is required'}],
      type: 'select',
      inputprops: {
        options: state.referData?.employers,
        allowClear: false,
      }
    },
    {
      label: 'Transport Group',
      name: 'FlightGroupMasterId',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Transport Group is required'}],
      type: 'select',
      inputprops: {
        options: state.referData?.transportGroups,
        allowClear: false,
      }
    },
  ]

  const warningColumns = [
    {
      label: 'Transport Date',
      name: 'EventDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Direction',
      name: 'Direction',
    },
    {
      label: '# Day',
      name: 'DayNum',
    },
    {
      label: 'Cluster',
      name: 'ClusterId',
      cellRender: (e) => (
        !e.value ?
        <CloseOutlined  style={{color: 'red', fontSize: 16}}></CloseOutlined> 
        :
        <CheckOutlined style={{color: 'green', fontSize: 16}}/> 
      )
    },
  ]

  const handleSubmitPreview = (values) => {
    setCalendarDate(values.startDate)
    setFormValues(values)
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/employee/rosterexecutepreview',
      data: {
        ...values,
        startDate: values.startDate ? dayjs(values.startDate).format('YYYY-MM-DD') : null,
        employeeId: parseInt(employeeId)
      }
    }).then((res) => {
      setPreviewData(res.data)
      if(res.data.OnsiteData){
        setRosterAlert(res.data.OnsiteData)
        setShowModal(true)
        setLoading(false)
      }else{
        handleExecuteRoster(values)
      }
    }).catch((err) => {

    }).then(() => {
      setLoading(false)
    })
  }

  const handleExecuteRoster = (values) => {
    setActionLoading(true)
    axios({
      method: 'post', 
      url: 'tas/employee/rosterexecute',
      data: {
        ...values,
        startDate: values.startDate ? dayjs(values.startDate).format('YYYY-MM-DD') : null,
        confirmed: true,
        employeeId: parseInt(employeeId)
      }
    }).then((res) => {
      setResultData(res.data)
      setShowModal(false)
      setLoading(false)
      setShowResultModal(true)
    }).catch((err) => {
      api.error({
        message: 'Warning',
        description: err.response.data.message,
      })
    }).then(() => setActionLoading(false))
  }


  const customCellRender = (e, data) => {
    let newStatusOfcurrentDate = data.newStatusDates?.find((item) => dayjs(item.EventDate).format(dateFormat) === dayjs(e).format(dateFormat))
    let oldStatusOfcurrentDate = data.oldStatusDates?.find((item) => dayjs(item.EventDate).format(dateFormat) === dayjs(e).format(dateFormat))

    return(
      <div className='w-full'>
        <div className='w-full flex justify-between'>
          <div className='text-start'>{dayjs(e).format('DD')}</div>
        </div>
        <div className='flex items-center justify-between leading-none min-h-[22px]'>
          {
            oldStatusOfcurrentDate ? 
            <div className='flex items-center gap-1 rounded-full bg-blue-200 px-1 text-xs'> 
              {
                oldStatusOfcurrentDate?.Direction && 
                <div className='text-blue-500 pl-1'>
                  {
                    newStatusOfcurrentDate?.Direction === 'IN' ?  
                    <BsAirplaneFill style={{transform: 'rotate(135deg)'}}/> :  
                    <BsAirplaneFill style={{transform: 'rotate(45deg)'}}/>
                  }
                </div>
              }
              {
                oldStatusOfcurrentDate?.ShiftCode && 
                <div 
                  style={{
                    // background: newStatusOfcurrentDate['Color'] && newStatusOfcurrentDate['Color'],
                  }}
                  className={`rounded-full p-1 flex justify-center items-center`}
                >
                  {oldStatusOfcurrentDate?.ShiftCode}
                </div>
              }
            </div>
            :
            <div></div>
          }
          {
            newStatusOfcurrentDate ? 
            <div className='flex items-center gap-1 rounded-full px-1 bg-green-200 text-xs'>
              {
                newStatusOfcurrentDate?.Direction && 
                <div className='text-success pl-1'>
                  {
                    newStatusOfcurrentDate?.Direction === 'IN' ? 
                  <BsAirplaneFill style={{transform: 'rotate(135deg)'}}/> :  
                  <BsAirplaneFill style={{transform: 'rotate(135deg)'}}/>
                  }
                </div>
              }
              {
                newStatusOfcurrentDate?.ShiftCode && 
                <div 
                  className={`rounded-full p-1 flex justify-center items-center`}
                >
                  {newStatusOfcurrentDate?.ShiftCode}
                  
                </div>
              }
            </div>
            :
            <div></div>
          }
        </div>
      </div>
    )
  }

  const annualCols = [
    {
      label: 'Shift',
      name: 'ShiftCode',
      width: '50px',
      cellRender: (e) => (
        <span className='px-1 rounded' style={{background: e.data.Color, color: hexToHSL(e.data.Color) > 60 ? 'black' : 'white'}}>
          {e.value}
        </span>
      )
    },
    {
      label: 'Description',
      name: 'Description',
    },
    {
      label: 'Date',
      name: 'EvenDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
  ]

  const handleDeleteRoster = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/employee/deletetransport/${employeeId}/${dayjs(rosterAlert.EventDate).format('YYYY-MM-DD')}`
    }).then(() => {
      setRosterAlert(null)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const getProfileData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
    }).catch((err) => {

    })
  }

  const handleCancel = () => {
    setShowResultModal(false)
    getProfileData()
  }

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      <div className='flex gap-5'>
        <div className='lg:w-2/3 '>
          <Form 
            form={form} 
            fields={fields}
            initValues={{...state.userProfileData, RoomId: '', startDate: dayjs()}}
            onFinish={handleSubmitPreview}
            wrapperCol={{flex: '500px'}}
            labelCol={{flex: '200px'}}
          />
          <div className='flex gap-4 mt-5'>
            {/* <Button type='primary' icon={<EyeFilled/>} onClick={() => setShowModal(true)}>Preview</Button> */}
            {
              !state.userInfo?.ReadonlyAccess ?
              <Button type='primary' onClick={() => form.submit()} loading={loading}>Roster</Button>
              :
              null
            }
          </div>
        </div>
        {
          annualYearData.length > 0 &&
          <div className='flex-1'>
            <Table 
              data={annualYearData}
              columns={annualCols}
              containerClass='shadow-none border'
              tableClass='border-t'
              pager={annualYearData.length > 20}
              title={<div className='py-1 font-bold'>Future Annual Leave</div>}
            />
          </div>
        }
      </div>
      <Modal 
        title='Roster Preview' 
        open={showModal} 
        onCancel={() => setShowModal(false)} 
        bodyStyle={{marginTop: '20px'}} 
        width={1000}
      >
        <div className='flex flex-col gap-3'>
          <Alert 
            message={<div className='text-sm font-medium'>There are {previewData?.EmployeeOffSiteStatus?.length} warning(s)</div>}
            type='warning'
            showIcon
            description={<div className='text-xs'>
              The person is already off site for the date supplied
            </div>}
            className='p-2'
          />
          <div className='border rounded-ot flex justify-between px-5 py-2'>
            <div>
              <div>Roster booking</div>
              <div className='text-success'><HomeFilled/> <CarFilled/></div>
            </div>
            <div>
              <div>Roster preview warning/error</div>
              <div className='text-red-500'><HomeFilled/> <CarFilled/></div>
            </div>
            <div>
              <div>Roster preview</div>
              <div className='text-blue-500'><HomeFilled/> <CarFilled/></div>
            </div>
          </div>
          <div className='grid grid-cols-12'>
            <Calendar 
              containerClass='shadow-none' 
              headerClass='px-0' 
              currentDate={calendarDate}
              onChange={(e) => setCalendarDate(dayjs(e).startOf('M').format('YYYY-MM-DD'))}
              cellRender={customCellRender}
              data={previewData}
            />
          </div>
          {
            rosterAlert ?
            <div className='bg-[#fff3f1] border border-red-300 rounded-ot px-5 py-2 max-w-[400px] mx-auto'>
              <div className='font-bold flex items-center gap-3'>
                <CloseCircleFilled color='red' className='text-[24px] text-red-500'/>
                <span>Roster start date is onsite</span>
              </div>
              <div className='flex gap-2 ml-9'>
                <div>{dayjs(rosterAlert.InTransportDate).format('YYYY-MM-DD')}</div>
                &mdash;
                <div>{dayjs(rosterAlert.OutTransportDate).format('YYYY-MM-DD')}</div>
                (On site)
              </div>
              <div className='flex flex-col items-center'>
                <div className='text-center mt-3'>Do you want to delete this record ?</div>
                <Button 
                  type='danger'
                  onClick={handleDeleteRoster}
                  loading={actionLoading}
                >
                  Delete
                </Button>
              </div>
            </div>
            :
            <div className='m-footer mt-4'>
              <Button type='primary' onClick={() => handleExecuteRoster(formValues)} loading={actionLoading}>Confirm</Button>
            </div>
          }
        </div>
      </Modal>
      {/* <Modal 
        open={warningModal} 
        onCancel={() => setWarningModal(false)} 
        title={<div className='text-orange-500'>Warning</div>}
        width={700}
      >
        <Table 
          containerClass='shadow-none' 
          data={errorMessage}
          keyExpr={'EventDate'}
          pager={false}
          columns={warningColumns}
        />
      </Modal> */}
      <Modal 
        open={showResultModal} 
        onCancel={handleCancel} 
        title='Roster response'
        width={900}
      >
        <ConfirmedRosters data={resultData}/>
      </Modal>
      {contextHolder}
    </div>
  )
}

export default RosterDetail