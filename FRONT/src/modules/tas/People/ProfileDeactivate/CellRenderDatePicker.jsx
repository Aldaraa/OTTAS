import { CheckCircleFilled, CloseCircleFilled, LoadingOutlined } from "@ant-design/icons"
import { DatePicker, Form } from "antd"
import axios from "axios"
import { Tooltip } from "components"
import dayjs from "dayjs"
import { isBoolean } from "lodash"
import { useEffect, useState } from "react"
import { BsAirplaneFill } from "react-icons/bs"

const CellRenderDatePicker = ({name, form, rowData, dateLoading}) => {
  const [ loading, setLoading ] = useState(false)

  useEffect(() => {
    setLoading(dateLoading)
  },[dateLoading])

  const handleDateValidation = (empId, date, name) => {
    if(date){
      setLoading(true)
      axios({
        method: 'get',
        url: `tas/employee/deactive/check/${empId}/${dayjs(date).format('YYYY-MM-DD')}`,
      }).then((res) => {
        form.setFieldValue(['rosters', name, 'DateValidationStatus'], res.data.DateValidationStatus)
        form.setFieldValue(['rosters', name, 'FutureTransportValidationStatus'], res.data.FutureTransportValidationStatus)
      }).catch((err) => {
        form.setFieldValue(['rosters', name, 'DateValidationStatus'], false)
        form.setFieldValue(['rosters', name, 'FutureTransportValidationStatus'], false)
      }).then(() => setLoading(false))
    }
  }

  const disabledDate = (current) => {
    // Can not select days before today and today
    return current < dayjs().subtract(1, 'day').endOf('day') || current > dayjs().add(7, 'day').endOf('day');
  };

  return(
    <div className='flex mb-0 w-full items-center gap-2'>
      <Form.Item 
        className='m-0 flex-1'
        name={[name, 'eventDate']}
        rules={[{required: true, message: 'Event Date is required'}]}
      >
        <DatePicker
          className='w-full'
          disabledDate={disabledDate}
          onChange={(event) => {
            handleDateValidation(rowData.Id, event, name)
          }}
        />
      </Form.Item>
      <Form.Item
        noStyle
        shouldUpdate={(prev, next) => prev.rosters[name]?.DateValidationStatus !== next.rosters[name]?.DateValidationStatus}
        className='m-0'
      >
        {({getFieldValue}) => {
          return(
            <Form.Item name={[name, 'DateValidationStatus']} className="mb-0">
              {
                loading ?
                <LoadingOutlined/>
                :
                (
                  isBoolean(getFieldValue(['rosters', name, 'DateValidationStatus'])) &&
                  (
                    getFieldValue(['rosters', name, 'DateValidationStatus']) ?
                    <Tooltip title='The Date is available'>
                      <CheckCircleFilled style={{color: '#52c41a'}}/>
                    </Tooltip>
                    :
                    <Tooltip title='The Date is unavailable'>
                      <CloseCircleFilled style={{color: '#ff4d4f'}}/>
                    </Tooltip>
                  )
                )
              }
            </Form.Item>
          )
        }}
      </Form.Item>
      <Form.Item
        noStyle
        shouldUpdate={(prev, next) => prev.rosters[name]?.FutureTransportValidationStatus !== next.rosters[name]?.FutureTransportValidationStatus}
        className='m-0'
      >
        {({getFieldValue}) => {
          return(
            <Form.Item name={[name, 'FutureTransportValidationStatus']} className="mb-0">
              {
                loading ?
                <LoadingOutlined/>
                :
                (
                  isBoolean(getFieldValue(['rosters', name, 'FutureTransportValidationStatus'])) &&
                  (
                    getFieldValue(['rosters', name, 'FutureTransportValidationStatus']) ?
                    <Tooltip title='Has Future transport'>
                      <BsAirplaneFill style={{color: '#ff4d4f'}}/>
                    </Tooltip>
                    :
                    <Tooltip title='No Future transport'>
                      <BsAirplaneFill style={{color: '#52c41a'}}/>
                    </Tooltip>
                  )
                )
              }
            </Form.Item>
          )
        }}
      </Form.Item>
    </div>
  )
}

export default CellRenderDatePicker