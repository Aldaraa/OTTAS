import axios from 'axios'
import { Button, Form, Modal, Table } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect, useMemo, useState } from 'react'
import { Form as AntForm, AutoComplete, Checkbox, DatePicker, Input } from 'antd'
import { PlusOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import range from 'utils/range'

function Accommodation({mainForm, getMaster}) {
  const [ loading, setLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ hotels, setHotels ] = useState([])
  const [ form ] = AntForm.useForm()
  const { state } = useContext(AuthContext)
  const [ editData, setEditData ] = useState(null)

  const hotelValue = AntForm.useWatch('Hotel', form)

  useEffect(() => {
    getHotels()
  },[])

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
          <Form.Item name='FirstNight' className='mb-0' rules={[{required: true, message: 'First night is required'}]}>
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
                  <Form.Item name='LastNight' className='mb-0' rules={[{required: true, message: 'Last night is required'}]}>
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
      component: <Form.Item 
        name='Hotel'
        required={[{required: true, message: 'Hotel is required'}]}
        label='Hotel'
        className='col-span-12 mb-2'
      >
        <AutoComplete
          options={hotelOptions}
          fieldNames={{value: 'Description'}}
        />
      </Form.Item>
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
      rules: [{required: true, message: 'Payment Condition is required'}],
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
            <Form.Item name='Comment' label='Comment' className='col-span-12 mb-8' rules={[{required: isLongTime, message: 'Comment is required'}]}>
              <Input.TextArea maxLength={300} showCount/>
            </Form.Item>
          )
        }}
      </Form.Item>
    },
  ]

  const handleDeleteButton = (row, event) => {
    event.stopPropagation()
    let tmp = [...mainForm.getFieldValue('accommodationData')]
    tmp.splice(row.rowIndex, 1)
    mainForm.setFieldValue('accommodationData', tmp)
    getMaster()
  }

  const handleEditButton = (row, event) => {
    event.stopPropagation()
    setEditData({
      ...row.data,
      FirstTime: dayjs(row.data.FirstNight),
      LastTime: dayjs(row.data.LastNight),
      index: row.rowIndex,
    })
    setShowModal(true)
  }

  const columns = [
    {
      label: 'Check In',
      name: 'FirstNight',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
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
      label: 'Night of Numbers',
      name: 'LastNight',
      cellRender: (e) => (
        <div>{dayjs(dayjs(e.value) - dayjs(e.data.FirstNight)).get('D') - 1}</div>
      )
    },
    {
      label: 'City',
      name: 'City',
    },
    {
      label: 'Hotel',
      name: 'Hotel',
    },
    // {
    //   label: 'HotelLocation',
    //   name: 'HotelLocation',
    //   cellRender: (e) => (
    //     <div>{Number.parseFloat(e.value[0]).toFixed(6)}, {Number.parseFloat(e.value[1]).toFixed(6)}</div>
    //   )
    // },
    {
      label: 'Payment Condition',
      name: 'PaymentCondition',
    },
    {
      label: 'Comment',
      name: 'Comment',
    },
    {
      label: '',
      name: 'action',
      width: '150px', 
      alignment: 'center',
      cellRender: (e) => (
        <div className='flex items-center gap-2'>
          <button type='button' className='edit-button' onClick={(event) => handleEditButton(e, event)}>Edit</button>
          <button type='button' className='dlt-button' onClick={(event) => handleDeleteButton(e, event)}>Delete</button>
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    if(editData){
      let tmp = [...mainForm.getFieldValue('accommodationData')]
      tmp[editData.index] = {
        ...values,
        HotelLocation: `${values.HotelLocation}`,
        FirstNight: dayjs(`${values.FirstNight.format('YYYY-MM-DD')} ${values.FirstTime.format('HH:mm')}`),
        LastNight: dayjs(`${values.LastNight.format('YYYY-MM-DD')} ${values.LastTime.format('HH:mm')}`),
      }
      mainForm.setFieldValue('accommodationData', [...tmp])
    }else{
      mainForm.setFieldValue('accommodationData', [
        ...mainForm.getFieldValue('accommodationData'),
        {
          ...values,
          HotelLocation: `${values.HotelLocation}`,
          FirstNight: dayjs(`${values.FirstNight.format('YYYY-MM-DD')} ${values.FirstTime.format('HH:mm')}`),
          LastNight: dayjs(`${values.LastNight.format('YYYY-MM-DD')} ${values.LastTime.format('HH:mm')}`),
        }
      ])
    }
    form.resetFields()
    getMaster()
    handleCloseModal()
  }

  const handleCloseModal = () => {
    form.resetFields()
    setShowModal(false)
  }

  const handleClickAdd = () => {
    setEditData(null)
    setShowModal(true)
  }

  return (
    <>
      <Form.Item name={'accommodationData'} noStyle>
      </Form.Item>
      <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.accommodationData !== curValues.accommodationData} className='col-span-12'>
        {
          ({getFieldValue}) => {
            return(
              <Table
                data={mainForm.getFieldValue('accommodationData')}
                columns={columns}
                allowColumnReordering={false}
                loading={loading}
                containerClass='shadow-none border border-gray-300'
                keyExpr='Id'
                pager={mainForm.getFieldValue('accommodationData').length > 20}
                title={
                  <div className='flex justify-between items-center py-2 gap-3 border-b'>
                    <div className='text-md font-bold pl-2'>Accommodation</div>
                    <div className='flex gap-4 items-center'>
                      <Button 
                        icon={<PlusOutlined />} 
                        onClick={handleClickAdd}
                        className='text-xs'
                        htmlType='button'
                      >
                        Add Accommodation
                      </Button>
                    </div>
                  </div>
                }
              />
            )
          }
        }
      </Form.Item>
      <Modal 
        title='Add Accommodation' 
        width={650} 
        open={showModal} 
        onCancel={()=>setShowModal(false)} 
      >
        <Form
          form={form}
          fields={fields}
          editData={editData}
          className='grid grid-cols-12 gap-x-8'
          onFinish={handleSubmit}
          labelCol={{flex: '130px'}}
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='submit' type='primary' icon={<PlusOutlined/>}>Add</Button>
            <Button htmlType='button' onClick={handleCloseModal} >Cancel</Button>
          </div>
        </Form>
      </Modal>
    </>
  )
}

export default Accommodation