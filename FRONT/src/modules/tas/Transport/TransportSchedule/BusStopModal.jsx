import { Button, Form, Modal } from 'components'
import React, { useEffect, useState } from 'react'
import BusStation from '../TransportActive/BusStation'
import { Input, Timeline } from 'antd'
import { LoadingOutlined, MinusCircleOutlined, PlusOutlined, SaveOutlined } from '@ant-design/icons'
import axios from 'axios'
import { twMerge } from 'tailwind-merge'

const formLayout = {
  labelCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 10,
    },
  },
  wrapperCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 16,
    },
  },
}

function BusStopModal({rowData, onReset, ...restprops}) {
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ fetchLoading, setFetchLoading ] = useState(true)
  const [ editData, setEditData ] = useState()
  const [ isEdit, setIsEdit ] = useState(false)

  const [ busStopForm ] = Form.useForm()

  useEffect(() => {
    if(rowData?.BusstopStatus){
      fetchBusStopDetail(rowData)
    }else{
      setFetchLoading(false)
    }
  },[rowData])

  const fetchBusStopDetail = (row) => {
    setFetchLoading(true)
    axios({
      method: 'get',
      url: `tas/transportschedule/busstop/${row.Id}`
    }).then((res) => {
      setEditData({...row, busStops: [...res.data]})
      busStopForm.setFieldValue('busstops', res.data)
    }).catch((err) => {

    }).then(() => setFetchLoading(false))
  }

  const busStopFields = [
    {
      type: 'component',
      component: <>
        <Form.List name="busstops" className='col-span-12'>
          {(fields, { add, remove }) => (
            <>
              {
                fields.length > 0 ?
                <div className='col-span-12 flex flex-col  gap-y-2 max-h-[400px] overflow-auto mb-3'>
                  {fields.map(({ key, name, ...restField }) => (
                    <div className='flex gap-x-2 items-start'>
                      <Form.Item
                        {...restField}
                        className='mb-0'
                        name={[name, 'Description']}
                      >
                        <BusStation placeholder='Station'/>
                      </Form.Item>
                      <Form.Item 
                        name={[name, 'ETD']}
                        className='mb-0 w-[130px]'
                        rules={[{required: true, message: 'In ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                        >
                        <Input
                          maxLength={4}
                          placeholder='ETD'
                          className='py-[6px]'
                        />
                      </Form.Item>
                      <button 
                        type='button'
                        className='bg-[#FFE2E5] text-[#F64E60] hover:bg-red-200 rounded-md px-3 py-1 disabled:bg-gray-100 transition-all' 
                        onClick={() => remove(name)}
                      >
                        <MinusCircleOutlined />
                      </button>
                    </div>
                  ))}
                </div>
                : null
              }
              <Form.Item className='col-span-12 mb-5'>
                <Button
                  className='w-full flex justify-center'
                  onClick={() => add()}
                  block
                  icon={<PlusOutlined />}>
                  Add new station
                </Button>
              </Form.Item>
            </>
          )}
        </Form.List>
      </>
    },
  ]

  const handleSubmitBusStop = (values) => {
    setSubmitLoading(true)
    axios({
      method: 'put',
      url: 'tas/transportschedule/busstop',
      data: {
        id: editData.Id,
        ...values
      }
    }).then((res) => {
      onReset()
    }).catch((err) => {

    }).then(() => setSubmitLoading(false))
  }

  const onCancel = () => {
    setIsEdit(false)
    busStopForm.setFieldValue('busstops', editData.busStops)
  }

  return (
    <Modal
      {...restprops}
      destroyOnClose={false}
      // open={showBusStopModal} 
      // onCancel={() => setShowBusStopModal(false)} 
      // title={`Set Bus Stop (${dayjs(editData?.EventDate).format('YYYY-MM-DD')})`}
      // width={800}
    >
      {
        fetchLoading ? 
        <div className='h-[100px] flex justify-center items-center'>
          <LoadingOutlined style={{fontSize: 26}}/>
        </div>
        :
        <>
          {
            editData?.busStops?.length > 0 ?
            <>
              <div className={twMerge(isEdit ? 'hidden' : 'block mt-4')}>
                <Timeline 
                  mode='left'
                  items={editData?.busStops.map((item) => ({children: <div className='flex gap-2'><div>{item.ETD}</div> <div>{item.Description}</div></div>}))}
                />
                <div className='flex justify-end'><Button onClick={() => setIsEdit(true)}>Edit</Button></div>                  
              </div>
              <div className={twMerge(isEdit ? 'block' : 'hidden')}>
                <Form
                  fields={busStopFields}
                  className='gap-x-5'
                  form={busStopForm}
                  editData={editData}
                  onFinish={handleSubmitBusStop}
                  itemLayouts={formLayout}
                >
                  <div className='col-span-12 gap-3 flex justify-end mt-3'>
                    <Button type='primary' onClick={() => busStopForm.submit()} loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
                    <Button onClick={onCancel} disabled={submitLoading}>Cancel</Button>
                  </div>
                </Form>
              </div>
            </>

            : 
            <Form
              fields={busStopFields}
              className='gap-x-5'
              form={busStopForm}
              editData={editData}
              onFinish={handleSubmitBusStop}
              itemLayouts={formLayout}
            >
              <div className='col-span-12 gap-3 flex justify-end mt-3'>
                <Button type='primary' onClick={() => busStopForm.submit()} loading={submitLoading} icon={<SaveOutlined/>}>Save</Button>
                <Button onClick={() => restprops?.onCancel() || false} disabled={submitLoading}>Cancel</Button>
              </div>
            </Form>
          }
        </>
      }
    </Modal>
  )
}

export default BusStopModal