import { MinusCircleOutlined, PlusOutlined, SaveOutlined, SwapRightOutlined } from '@ant-design/icons'
import { DatePicker, Input, Select } from 'antd'
import axios from 'axios'
import { Button, Form, Modal } from 'components'
import React, { useCallback, useEffect, useState } from 'react'
import dayjs from 'dayjs'
import BusStation from './BusStation'

function StationModal({editData, ...restprops}) {
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ stations, setStations ] = useState([])
  const [ form ] = Form.useForm()
  const busstops = Form.useWatch('busstops', form)

  useEffect(() => {
    getStations()
  },[])
  
  const getStations = useCallback(() => {
    axios({
      method: 'get',
      url: 'tas/busstop?Active=1',
    }).then((res) => {
      setStations(res.data)
    })
  },[])

  const fields = [
    {
      type: 'component',
      component: <>
        <div className='col-span-12 flex items-center gap-3 mb-5'>
          <Form.Item name='startDate' className='mb-0'>
            <DatePicker placeholder='Start Date'></DatePicker>
          </Form.Item>
          <SwapRightOutlined />
          <Form.Item name='endDate' className='mb-0'>
            <DatePicker placeholder='End Date'></DatePicker>
          </Form.Item>
        </div>
        <Form.List name="busstops" className='col-span-12'>
          {(fields, { add, remove }) => (
            <>
              {
                fields.length > 0 ?
                <div className='col-span-12 flex flex-col gap-y-2 max-h-[350px] overflow-auto mb-3'>
                  <div>Stations ({fields.length}):</div>
                  {fields.map(({ key, name, ...restField }) => (
                    <div className='flex gap-x-2 items-start'>
                      <Form.Item
                        {...restField}
                        className='col-span-5 mb-0'
                        name={[name, 'description']}
                        rules={[{required: true, message: 'Station is required'}]}
                      >
                        <BusStation placeholder='Station'/>
                        {/* <Select options={stations.map((item) => ({value: item.Description, label: item.Description}))} placeholder="Firstname" /> */}
                      </Form.Item>
                      <Form.Item
                        name={[name, 'etd']}
                        className='col-span-5 mb-0'
                        rules={[{required: true, message: 'ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                      >
                        <Input
                          maxLength={4}
                          placeholder='ETD'
                          />
                      </Form.Item>
                      <button 
                        type='button'
                        className='bg-[#FFE2E5] text-[#F64E60] hover:bg-red-200 rounded-md py-1 px-3 disabled:bg-gray-100 transition-all' 
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

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: 'tas/activetransport/busstop',
      data: {
        ...values,
        startDate: dayjs(values.startDate).format('YYYY-MM-DD'),
        endDate: dayjs(values.endDate).format('YYYY-MM-DD'),
        id: editData?.Id,
      }
    }).then((res) => {
      restprops.onCancel()
    }).catch((err) => {

    }).then(() => {
      setActionLoading(false)
    })
  }

  const handleCancel = useCallback(() => {
    restprops?.onCancel()
  },[])

  return (
    <Modal width={800} title={<div>Set Bus Stop <span className='text-secondary2'>/ {editData?.Code} /</span></div>} {...restprops}>
      <Form form={form} fields={fields} layout='vertical' noLayoutConfig={true} onFinish={handleSubmit}>
        <div className='col-span-12 flex justify-end items-center gap-2'>
          <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>} disabled={busstops?.length === 0}>Save</Button>
          <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
        </div>
      </Form>
    </Modal>
  )
}

export default StationModal