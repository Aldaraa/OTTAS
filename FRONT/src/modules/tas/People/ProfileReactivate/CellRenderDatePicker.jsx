import { CheckCircleFilled, CloseCircleFilled, LoadingOutlined } from "@ant-design/icons"
import { DatePicker, Form } from "antd"
import axios from "axios"
import dayjs from "dayjs"
import { isBoolean } from "lodash"
import { useState } from "react"

const CellRenderDatePicker = ({name, form, rowData}) => {
  const [ value, setValue ] = useState(false)
  const [ loading, setLoading ] = useState(false)

  const handleDateValidation = (empId, date, name) => {
    if(date){
      setLoading(true)
      axios({
        method: 'get',
        url: `tas/employee/deactive/check/${empId}/${dayjs(date).format('YYYY-MM-DD')}`,
      }).then((res) => {
        form.setFieldValue(name, res.data.DateValidationStatus)
      }).catch((err) => {
        form.setFieldValue(name, false)
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
            handleDateValidation(rowData.Id, event, ['rosters', name, 'isAvailable'])
            setValue(event ? event : false)
          }}
        />
      </Form.Item>
      <Form.Item
        noStyle
        shouldUpdate={(prev, next) => prev.rosters[name]?.isAvailable !== next.rosters[name]?.isAvailable}
        className='m-0'
      >
        {({getFieldValue}) => {
          return(
            <Form.Item name={[name, 'isAvailable']} className="mb-0">
              {
                loading ?
                <LoadingOutlined/>
                :
                (
                  isBoolean(getFieldValue(['rosters', name, 'isAvailable'])) &&
                  (
                    getFieldValue(['rosters', name, 'isAvailable']) ?
                    <CheckCircleFilled style={{color: '#52c41a'}}/>
                    :
                    <CloseCircleFilled style={{color: '#ff4d4f'}}/>
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