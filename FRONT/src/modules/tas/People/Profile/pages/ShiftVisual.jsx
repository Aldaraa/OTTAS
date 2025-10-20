import axios from 'axios'
import { Button, Form, Modal, ShiftCalendar } from 'components'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm,DatePicker,Drawer,Select, Space } from 'antd'
import { useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import hexToHSL from 'utils/hexToHSL'
import ls from 'utils/ls'

const RenderCell = React.memo(({date, isEdit, data, shiftStatus, onSiteShifts, offSiteShifts}) => {
  let currentDate = data?.find((item) => dayjs(item.EventDate).format('YYYY-MM-DD') === dayjs(date).format('YYYY-MM-DD'))
  let findedData = shiftStatus?.find((item) => item.Id === currentDate?.ShiftId)
  return(
    <div className='w-full flex flex-col' >
        <div className='flex flex-col items-start'>
          <div className='font-medium'>{dayjs(date).format('DD')} <span className='text-xs font-normal text-secondary2'>{dayjs(date).format('MMM')}</span></div>
        </div>
        {
          currentDate &&
          <div className='flex gap-3 items-center'>
            {/* <div className='w-5 h-5 rounded' style={{background: findedData?.ColorCode}}></div> */}
            <AntForm.Item className='m-0 p-0 flex-1 self-center'  name={[currentDate.index, 'ShiftId']}>
              <Select 
                disabled={!isEdit} 
                className='w-full rounded-md'
                popupMatchSelectWidth={false}
                optionlabelProp="label"
                bordered={false}
                style={{background: findedData?.ColorCode}}
                size='small'
                allowClear
                showSearch
                options={ findedData ? findedData.OnSite === 1 ? onSiteShifts : offSiteShifts : shiftStatus }
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                optionRender={(option) => {
                  return(
                    <Space>
                      {option.data.label}
                    </Space>
                  )
                }}
              >
                {
                  findedData ? findedData.OnSite === 1 ? 
                  onSiteShifts.map((item, index) => (
                    <Select.Option key={index} value={item.value} label={item.label}>
                      {item.content ? item.content : item.label}
                    </Select.Option>
                  ))
                  : 
                  offSiteShifts.map((item, index) => (
                    <Select.Option key={index} value={item.value} label={item.label}>
                      {item.content ? item.content : item.label}
                    </Select.Option>
                  )) 
                  : 
                  shiftStatus.map((item, index) => (
                    <Select.Option key={index} value={item.value} label={item.label}>
                      {item.content ? item.content : item.label}
                    </Select.Option>
                  ))
                }
              </Select>
            </AntForm.Item>
          </div>
        }
    </div>
  )
}, (prevProps, nextProps) => (JSON.stringify(prevProps.data) && JSON.stringify(nextProps.data)) 
    && (prevProps.isEdit !== nextProps.isEdit) 
    && (prevProps.shiftStatus !== nextProps.shiftStatus)
)

function ShiftVisual() {
  const [ data, setData ] = useState([])
  const [ selectedDate, setSelectedDate ] = useState(dayjs().startOf('M'))
  const [ shiftStatus, setShiftStatus ] = useState([])
  const [ bulkShiftStatus, setBulkShiftStatus ] = useState([])
  const [ onSiteShifts, setOnSiteShifts ] = useState([])
  const [ offSiteShifts, setOffSiteShifts ] = useState([])
  const [ isEdit, setIsEdit ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ bulkLoading, setBulkLoading ] = useState(false)

  const [form] = AntForm.useForm()
  const [bulkForm] = AntForm.useForm()
  const { employeeId } = useParams()
  const { state, action } = useContext(AuthContext)

  useEffect(() => {
    ls.set('pp_rt', 'shiftvisual')
    getData()
    getShift()
  },[selectedDate])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/employeestatus/visualstatusdates/${employeeId}/${dayjs(selectedDate).format('YYYY-MM-DD')}`
    }).then((res) => {
      let tmp = []
      res.data.map((item, i) => {
        tmp.push({
          index: i,
          ...item,
        })
      })
      form.setFieldsValue({shift: tmp})
      setData(tmp)
    }).catch((err) => {

    })
  }

  const getShift = () => {
    axios({
      method: 'get',
      url: 'tas/shift?Active=1',
    }).then((res) => {
      let tmp = []
      let onsites = []
      let offsites = []
      let bulk = []

      res.data.map((item) => {
        tmp.push({
          value: item.Id, 
          label: `${item.Code}`, 
          content: <div className='flex gap-1 text-xs'>
            <span>{item.Code}</span>
            {/* <span className='text-gray-400'>{item.OnSite === 1 ? 'On Site' : 'Off Site'}</span> */}
          </div>, 
          ...item
        })
        bulk.push({
          value: item.Id, 
          label: `${item.Code}-${item.Description}`, 
          content: <div className='flex gap-1 text-xs'>
            <span>{item.Code}</span>
            {/* <span className='text-gray-400'>{item.OnSite === 1 ? 'On Site' : 'Off Site'}</span> */}
          </div>,
          ...item
        })
        if(item.OnSite === 1){
          onsites.push({
            value: item.Id, 
            label: item.Code,
            content: <div className='flex gap-1 text-xs'>
              <span>{item.Code}</span>
              {/* <span className='text-gray-400'>{item.OnSite === 1 ? 'On Site' : 'Off Site'}</span> */}
            </div>,
            ...item})
        }
        else{
          offsites.push({
            value: item.Id, 
            label: item.Code,
            content: <div className='flex gap-1 text-xs'>
              <span>{item.Code}</span>
              {/* <span className='text-gray-400'>{item.OnSite === 1 ? 'On Site' : 'Off Site'}</span> */}
            </div>,
            ...item
          })
        }
      })
      setBulkShiftStatus(bulk)
      setShiftStatus(tmp)
      setOffSiteShifts(offsites)
      setOnSiteShifts(onsites)
    }).catch((err) => {
  
    })
  }

  const handleSubmit = (values) => {
    let tmp = []
    values.shift.map((item) => {
      if(item.ShiftId){
        tmp.push(item)
      }
    })
    axios({
      method: 'post',
      url: 'tas/employeestatus/visualstatusdatechange',
      data: {
        employeeId: employeeId,
        statusDates: [
          ...tmp
        ]
      }
    }).then((res) => {
      getData()
      getProfileData()
      setIsEdit(false)
    }).catch((err) => {

    })
  }

  const bulkFields = [
    {
      type: 'component',
      component: <AntForm.Item name='rangeDate' label='Date' className='col-span-12 mb-4' rules={[{required: true, message: 'Date is required'}]}>
        <DatePicker.RangePicker className='w-full'></DatePicker.RangePicker>
      </AntForm.Item>
    },
    {
      label: 'Shift',
      name: 'ShiftId',
      className: 'col-span-12 mb-4',
      rules: [{required: true, message: 'Shift is required'}],
      type: 'select',
      inputprops: {
        options: bulkShiftStatus
      }
    },
  ]

  const getProfileData = () => {
    axios({
      method: 'get',
      url: `tas/employee/${employeeId}`
    }).then((res) => {
      action.saveUserProfileData(res.data)
    }).catch((err) => {

    })
  }

  const handleSubmitBulkChange = (values) => {
    setBulkLoading(true)
    axios({
      method: 'post',
      url: 'tas/employeestatus/visualstatusbulkchange',
      data: {
        employeeId: employeeId,
        startDate: dayjs(values.rangeDate[0]).format('YYYY-MM-DD'),
        endDate: dayjs(values.rangeDate[1]).format('YYYY-MM-DD'),
        shiftId: values.ShiftId,
      }
    }).then((res) => {
      setShowModal(false)
      getData()
      getProfileData()
      // action.changedFlight(state.ChangedFlight+1)
    }).catch((err) => {

    }).then(() => setBulkLoading(false))
  }

  const renderShifts = (data=[], title, shiftCount=10) => {
    let index = 0
    let listItem = []
    while (index < data.length) {
      listItem.push(
        <div className='flex flex-col gap-1' key={index}>
          {index === 0 ? <div className='text-gray-500 font-bold mb-2'>{title}</div> : <div className='mb-2'>&nbsp;</div>}
          {
            data.slice(index, index+shiftCount).map((item, i) => (
              <div className='flex items-start gap-2' key={i}>
                <div className='w-[50px] text-center py-[2px] rounded-md' style={{backgroundColor: item.ColorCode}}>
                  <div className='text-xs' style={{color: item.ColorCode ? hexToHSL(item.ColorCode) > 60 ? 'black' : 'white' : 'black'}}>{item.Code}</div>
                </div>
                <div className='leading-none'>{item.Description} </div>
              </div>
            ))
          }
        </div>
      )
      index += shiftCount;
    }
    return listItem
  }

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      <div className='text-lg font-bold mb-3'>Shift Visual</div>
      <div className='grid grid-cols-12 gap-5'>
        <div className='col-span-12 2xl:col-span-6'>
          <AntForm form={form} component={false} onFinish={handleSubmit}>
            <AntForm.List name='shift'>
              {(fields) => (
                <>
                  <ShiftCalendar
                    containerClass={'shadow-none'} 
                    picker='month' 
                    onChange={(e) => setSelectedDate(dayjs(e).format('YYYY-MM-DD'))} 
                    data={data}
                    cellRender={(e) => (
                      <RenderCell 
                        date={e} 
                        isEdit={isEdit}
                        data={data}
                        shiftStatus={shiftStatus}
                        offSiteShifts={offSiteShifts}
                        onSiteShifts={onSiteShifts}
                      />
                    )}
                    currentDate={selectedDate}
                    extraComponent={ !state.userInfo?.ReadonlyAccess ? 
                      <div className='flex justify-end flex-1'>
                        <Button onClick={() => setShowModal(true)}>Bulk Change</Button>
                      </div> : null
                    }
                  />
                </>
              )}
            </AntForm.List>
            <AntForm.Item className='mt-2 flex justify-end'>
              {
                !state.userInfo?.ReadonlyAccess ?
                isEdit ? 
                <div className='flex gap-4'>
                  <Button type='success' onClick={() => form.submit()}>Save</Button>
                  <Button onClick={() => setIsEdit(false)}>Cancel</Button>
                </div>
                :
                <Button type='primary' onClick={() => setIsEdit(true)}>Edit</Button>
                :
                null
              }
            </AntForm.Item>
          </AntForm>
        </div>
        <div className='col-span-12 2xl:col-span-6 flex justify-around flex-wrap'>
          {renderShifts(onSiteShifts.sort((a, b) => (a.Code > b.Code) ? 1 : (a.Code < b.Code) ? -1 : 0), 'On Site Shifts', 16)}
          {renderShifts(offSiteShifts.sort((a, b) => (a.Code > b.Code) ? 1 : (a.Code < b.Code) ? -1 : 0), 'Off Site Shifts', 16)}
        </div>
      </div>
      <Modal 
        title='Bulk Change' 
        open={showModal}
        onCancel={() => {setShowModal(false); bulkForm.resetFields()}}
        destroyOnClose={true}
      >
        <Form fields={bulkFields} form={bulkForm} onFinish={handleSubmitBulkChange}>
        </Form>
        <div className='flex justify-end'>
          <Button type={'success'} onClick={() => bulkForm.submit()} loading={bulkLoading}>Save</Button>
        </div>
      </Modal>
    </div>
  )
}

export default ShiftVisual