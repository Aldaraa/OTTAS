import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form as AntForm, AutoComplete, Checkbox, DatePicker, Input, InputNumber } from 'antd'
import { PlusOutlined, SaveOutlined, SyncOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { useParams } from 'react-router-dom'

function Accommodation({disabled, data, getData, documentDetail}) {
  // const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ hotels, setHotels ] = useState([])
  const dataGrid = useRef(null)

  const { state } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const hotelValue = AntForm.useWatch('Hotel', form)
  const { documentId } =  useParams()

  useEffect(() => {
    getHotels()
  },[])

  // const getData = () => {
  //   setLoading(true)
  //   axios({
  //     method: 'get',
  //     url: `tas/requestnonsitetravelaccommodation/${documentId}`
  //   }).then((res) => {
  //     setData(res.data)
  //   }).catch((err) => {

  //   }).then((res) => {
  //     setLoading(false)
  //   })
  // }

  const getHotels = () => {
    axios({
      method: 'get',
      url: 'tas/requestlocalhotel?Active=1',
    }).then((res) => {
      setHotels(res.data)
    }).catch((err) => {

    })
  }

  const hotelOptions = useMemo(() => {
    return hotels.filter((option) => (option?.Description ?? '').toLowerCase().includes(hotelValue?.toLowerCase() || ''))
  },[hotelValue])

  const fields = [
    {
      type: 'component',
      component: <Form.Item label='Check In' className='col-span-12 mb-2'>
        <div className='flex gap-5 items-start'>
          <Form.Item name='FirstNight' className='mb-0'>
            <DatePicker/>
          </Form.Item>
          <Form.Item
            name='FirstTime'
            className='mb-0'
            rules={[
              {required: true, message: 'Time is required'},
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('FirstTime').format('HH:mm') !== '00:00') {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('00:00 is not allowed!'));
                },
              }),
            ]}
          >
            <DatePicker.TimePicker needConfirm={false} format='HH:mm'/>
          </Form.Item>
          <Form.Item name='EarlyCheckIn' className='mb-0' getValueFromEvent={(e) => e.target.checked ? 1 : 0} valuePropName="checked">
            <Checkbox>Early Check In</Checkbox>
          </Form.Item>
        </div>
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item label='Check Out' className='col-span-12 mb-2'>
        <div className='flex gap-5 items-start'>
          <Form.Item noStyle shouldUpdate={(pre, cur) => pre.FirstNight !== cur.LastNight}>
            {({getFieldValue}) => {
              let defaultPickerValue = false
              if(!getFieldValue('LastNight')){
                defaultPickerValue = getFieldValue('FirstNight')
              }
              return(
                <>
                  <Form.Item name='LastNight' className='mb-0'>
                    <DatePicker defaultPickerValue={defaultPickerValue}/>
                  </Form.Item>
                  <Form.Item
                    name='LastTime'
                    className='mb-0'
                    rules={[
                      {required: true, message: 'Time is required'},
                      ({ getFieldValue }) => ({
                        validator(_, value) {
                          if (!value || getFieldValue('LastTime').format('HH:mm') !== '00:00') {
                            return Promise.resolve();
                          }
                          return Promise.reject(new Error('00:00 is not allowed!'));
                        },
                      }),
                    ]}
                  >
                    <DatePicker.TimePicker needConfirm={false} format='HH:mm'/>
                  </Form.Item>
                </>
              )
            }}
          </Form.Item>
          <Form.Item name='LateCheckOut' className='mb-0' getValueFromEvent={(e) => e.target.checked ? 1 : 0} valuePropName="checked">
            <Checkbox>
              Late Check Out
            </Checkbox>
          </Form.Item>
        </div>
      </Form.Item>
    },
    {
      type: 'component',
      names: ['Hotel'],
      component: <Form.Item 
        label='Hotel'
        name='Hotel'
        className='col-span-12 mb-2'
        required={[{required: true, message: 'Hotel is required'}]}
      >
        <AutoComplete
          options={hotelOptions}
          fieldNames={{value: 'Description'}}
          onSelect={(e, option) => {
            if(form.getFieldValue('EarlyCheckIn')){
              form.setFieldValue('EarlyCheckInCost', option.EarlyCheckInCost);
            }else{
              form.setFieldValue('EarlyCheckInCost', 0);
            }
            if(form.getFieldValue('LateCheckOut')){
              form.setFieldValue('LateCheckOutCost', option.LateCheckOutCost);
            }else{
              form.setFieldValue('LateCheckOutCost', 0);
            }
            form.setFieldValue('DayCost', option.DayCost || 0);
          }}
        />
      </Form.Item>
    },
    {
      type: 'component',
      component: <>
        <Form.Item name='EarlyCheckInCost' noStyle>
        </Form.Item>
        <Form.Item name='LateCheckOutCost' noStyle>
        </Form.Item>
        <Form.Item name='DayCost' noStyle>
        </Form.Item>
      </>
    },
    {
      label: 'City',
      name: 'City',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'City is required'}],
    },
    {
      label: 'Payment Condition',
      name: 'PaymentCondition',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.master?.paymentConditions,
        showSearch: false,
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.FirstNight !==cur.FirstNight || prev.LastNight !== cur.LastNight}>
        {({getFieldValue}) => {
          const isLongTime = dayjs(getFieldValue('LastNight')) > dayjs(getFieldValue('FirstNight')).add(1, 'day')
          return(
            <Form.Item name='Comment' label='Comment' className='col-span-12 mb-5' rules={[{required: isLongTime, message: 'Comment is required'}]}>
              <Input.TextArea maxLength={300} showCount/>
            </Form.Item>
          )
        }}
      </Form.Item>
    },
  ]

  const travelHotelFields = [
    {
      type: 'component',
      component: <Form.Item label='Check In' className='col-span-12 mb-2'>
        <div className='flex gap-5 items-start'>
          <Form.Item name='FirstNight' className='mb-0'>
            <DatePicker/>
          </Form.Item>
          <Form.Item
            name='FirstTime'
            className='mb-0'
            rules={[
              {required: true, message: 'Time is required'},
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('FirstTime').format('HH:mm') !== '00:00') {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('00:00 is not allowed!'));
                },
              }),
            ]}
          >
            <DatePicker.TimePicker needConfirm={false} format='HH:mm'/>
          </Form.Item>
          <Form.Item name='EarlyCheckIn' className='mb-0' getValueFromEvent={(e) => e.target.checked ? 1 : 0} valuePropName="checked">
            <Checkbox>Early Check In</Checkbox>
          </Form.Item>
        </div>
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item label='Check Out' className='col-span-12 mb-2'>
        <div className='flex gap-5 items-start'>
          <Form.Item noStyle shouldUpdate={(pre, cur) => pre.FirstNight !== cur.LastNight}>
            {({getFieldValue}) => {
              let defaultPickerValue = false
              if(!getFieldValue('LastNight')){
                defaultPickerValue = getFieldValue('FirstNight')
              }
              return(
                <Form.Item name='LastNight' className='mb-0'>
                  <DatePicker defaultPickerValue={defaultPickerValue} showTime format='YYYY-MM-DD HH:mm'/>
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item
            name='LastTime'
            className='mb-0'
            rules={[
              {required: true, message: 'Time is required'},
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('LastTime').format('HH:mm') !== '00:00') {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('00:00 is not allowed!'));
                },
              }),
            ]}
          >
            <DatePicker.TimePicker needConfirm={false} format='HH:mm'/>
          </Form.Item>
          <Form.Item name='LateCheckOut' className='mb-0' getValueFromEvent={(e) => e.target.checked ? 1 : 0} valuePropName="checked">
            <Checkbox>
              Late Check Out
            </Checkbox>
          </Form.Item>
        </div>
      </Form.Item>
    },
    {
      label: 'City',
      name: 'City',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'City is required'}],
    },
    {
      type: 'component',
      names: ['Hotel'],
      component: <Form.Item 
        label='Hotel'
        name='Hotel'
        className='col-span-12 mb-2'
        required={[{required: true, message: 'Hotel is required'}]}
      >
        <AutoComplete
          options={hotelOptions}
          fieldNames={{value: 'Description'}}
          onSelect={(e, option) => {
            if(form.getFieldValue('EarlyCheckIn')){
              form.setFieldValue('EarlyCheckInCost', option.EarlyCheckInCost);
            }
            if(form.getFieldValue('LateCheckOut')){
              form.setFieldValue('LateCheckOutCost', option.LateCheckOutCost);
            }
          }}
        />
      </Form.Item>
    },
    {
      label: 'Day Cost',
      name: 'DayCost',
      className: 'col-span-12 mb-2',
      type: 'price',
      inputprops: {
        min: 0
      }
    },
    {
      type: 'component',
      names: ['EarlyCheckInCost'],
      component: <Form.Item noStyle shouldUpdate={(pre, cur) => pre.EarlyCheckIn !== cur.EarlyCheckIn}>
        {({getFieldValue}) => {
          const isShow = getFieldValue('EarlyCheckIn')
          return(
            isShow ?
            <Form.Item label='Early Check In Cost' name='EarlyCheckInCost' className='col-span-12 mb-2'>
              <InputNumber
                controls={false}
                formatter={value => `${new Intl.NumberFormat().format(value)}`}
                min={0}
              />
            </Form.Item>
            : null
          )
        }}
      </Form.Item>
    },
    {
      type: 'component',
      names: ['LateCheckOutCost'],
      component: <Form.Item noStyle shouldUpdate={(pre, cur) => pre.LateCheckOut !== cur.LateCheckOut}>
        {({getFieldValue}) => {
          const isShow = getFieldValue('LateCheckOut')
          return(
            isShow ?
            <Form.Item label='Late Check Out Cost' name='LateCheckOutCost' className='col-span-12 mb-2'>
              <InputNumber
                controls={false}
                formatter={value => `${new Intl.NumberFormat().format(value)}`}
                min={0}
              />
            </Form.Item>
            : null
          )
        }}
      </Form.Item>
    },
    {
      label: 'Add Cost',
      name: 'AddCost',
      className: 'col-span-12 mb-2',
      type: 'price',
    },
    {
      label: 'Payment Condition',
      name: 'PaymentCondition',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.master?.paymentConditions,
        showSearch: false,
      }
    },
    {
      label: 'Comment',
      name: 'Comment',
      className: 'col-span-12 mb-2',
      type: 'textarea'
    },
  ]

  const columns = [
    {
      label: 'Check In',
      name: 'FirstNight',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
      )
    },
    {
      label: 'ECI',
      name: 'EarlyCheckIn',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    {
      label: 'Check Out',
      name: 'LastNight',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
      )
    },
    {
      label: 'LCO',
      name: 'LateCheckOut',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    {
      label: 'Night of Numbers',
      name: 'NightOfNumbers',
      alignment: 'left',
      // cellRender: (e) => (
      //   <div>{dayjs(dayjs(e.value) - dayjs(e.data.FirstNight)).get('D')}</div>
      // )
    },
    {
      label: 'City',
      name: 'City',
    },
    {
      label: 'Hotel',
      name: 'Hotel',
    },
    {
      label: 'Payment Condition',
      name: 'PaymentCondition',
      inputprops: {
        options: state.referData?.master?.paymentConditions,
      }
    },
    {
      label: 'Comment',
      name: 'Comment',
    },
    {
      label: '',
      name: 'action',
      width: 170,
      cellRender: (e) => (
        (!disabled || checkUpdateItinerary()) ?
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          {
            !disabled &&
            <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
          }
        </div>
        : null
      )
    },
  ]

  const handleSubmit = (values) => {
    if(editData){
      setSubmitLoading(true)
      axios({
        method: 'put',
        url:'tas/requestnonsitetravelaccommodation',
        data: {
          ...values,
          EarlyCheckInCost: values.EarlyCheckIn ? values.EarlyCheckInCost : 0,
          LateCheckOutCost: values.LateCheckOut ? values.LateCheckOutCost : 0,
          FirstNight: values.FirstNight ? dayjs(`${values.FirstNight.format('YYYY-MM-DD')} ${values.FirstTime.format('HH:mm')}`).format('YYYY-MM-DD HH:mm') : null,
          LastNight: values.LastNight ? dayjs(`${values.LastNight.format('YYYY-MM-DD')} ${values.LastTime.format('HH:mm')}`).format('YYYY-MM-DD HH:mm') : null,
          Id: editData.Id,
          HotelLocation: JSON.stringify(values.HotelLocation),
          DocumentId: documentId,
        }
      }).then((res) => {
        getData()
        setEditData(null)
        handleCancel()
      }).catch((err) => {
        
      }).then(() => setSubmitLoading(false))
    }
    else{
      setSubmitLoading(true)
      axios({
        method: 'post',
        url:'tas/requestnonsitetravelaccommodation',
        data: {
          ...values,
          EarlyCheckInCost: values.EarlyCheckIn ? values.EarlyCheckInCost : 0,
          LateCheckOutCost: values.LateCheckOut ? values.LateCheckOutCost : 0,
          FirstNight: values.FirstNight ? dayjs(`${values.FirstNight.format('YYYY-MM-DD')} ${values.FirstTime.format('HH:mm')}`).format('YYYY-MM-DD HH:mm') : null,
          LastNight: values.LastNight ? dayjs(`${values.LastNight.format('YYYY-MM-DD')} ${values.LastTime.format('HH:mm')}`).format('YYYY-MM-DD HH:mm') : null,
          DocumentId: documentId,
          HotelLocation: JSON.stringify(values.HotelLocation),
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setSubmitLoading(false))
    }
  }

  const handleDelete = () => {
    axios({
      method: 'delete',
      url: `tas/requestnonsitetravelaccommodation/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    })
  }

  const handleCancel = () => {
    form.resetFields()
    setShowModal(false)
  }
  
  const handleAddButton = () => {
    setEditData(null)
    setShowModal(true)
  }

  const handleEditButton = (dataItem) => {
    let tmp = {...dataItem}
    setEditData(tmp)
    form.setFieldsValue({
      ...dataItem,
      FirstNight: dataItem.FirstNight ? dayjs(dataItem.FirstNight) : null,
      LastNight: dataItem.LastNight ? dayjs(dataItem.LastNight) : null,
      FirstTime: dataItem.FirstNight ? dayjs(dataItem.FirstNight) : null,
      LastTime: dataItem.LastNight ? dayjs(dataItem.LastNight) : null,
    })
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const checkUpdateItinerary = () => {
    let boolean = false
    if((state.userInfo?.Role === 'SystemAdmin' || state.userInfo?.Role === 'TravelAdmin') && documentDetail?.CurrentStatus === 'Completed'){
      boolean = true
    }
    return boolean
  }

  const handleDuplicate = () => {
    form.validateFields().then((values) => {
      setSubmitLoading(true)
      axios({
        method: 'post',
        url:'tas/requestnonsitetravelaccommodation',
        data: {
          ...values,
          FirstNight: values.FirstNight ? dayjs(values.FirstNight).format('YYYY-MM-DD') : null,
          LastNight: values.LastNight ? dayjs(values.LastNight).format('YYYY-MM-DD') : null,
          DocumentId: documentId,
          HotelLocation: JSON.stringify(values.HotelLocation),
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setSubmitLoading(false))
    })
  }

  return (
    <div className='col-span-12'>
      <Table
        ref={dataGrid}
        data={data}
        columns={columns}
        allowColumnReordering={false}
        id="room"
        className={`overflow-hidden ${!state.userInfo?.ReadonlyAccess && 'border-t'}`}
        showRowLines={true}
        rowAlternationEnabled={false}
        loading={loading}
        pager={data.length > 20}
        containerClass='shadow-none border border-gray-300'
        title={
          <div className='flex justify-between items-center py-2 gap-3'>
            <div className='text-md font-bold pl-2'>Accommodations</div>
            <div className='flex gap-4 items-center'>
              {
                !disabled &&
                <>
                  <Button 
                    icon={<PlusOutlined />} 
                    onClick={handleAddButton}
                    className='text-xs'
                    htmlType='button'
                  >
                    Add Accommodation
                  </Button>
                  <Button 
                    icon={<SyncOutlined />} 
                    onClick={getData}
                    loading={loading}
                    className='text-xs'
                    htmlType='button'
                  >
                    Refresh
                  </Button>
                </>
              }
            </div>
          </div>
        }
      />
      <Modal 
        title={editData ? 'Edit Accommodation' : 'Add Accommodation'} 
        open={showModal} 
        onCancel={()=>setShowModal(false)} 
        width={650}
      >
        <Form
          form={form}
          fields={state.userInfo?.Role === 'TravelAdmin'  ? travelHotelFields : fields}
          // editData={editData}
          disabled={disabled && !checkUpdateItinerary()}
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSubmit}
          onCancel={handleCancel}
          labelCol={{flex: '130px'}}
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='submit' loading={submitLoading} type={'primary'} icon={<SaveOutlined/>}>Save</Button>
            {
              editData ? 
              <Button htmlType='button' loading={submitLoading} icon={<PlusOutlined/>} onClick={handleDuplicate}>Save & Add new</Button>
              : null
            }
            <Button htmlType='button' onClick={handleCancel}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button htmlType='button' type='danger'onClick={handleDelete}>Yes</Button>
          <Button htmlType='button' onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default Accommodation