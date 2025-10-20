import { Select } from 'antd'
import { Form } from 'components'
import dayjs from 'dayjs'
import React, { memo } from 'react'

const RenderCell = ({date, isEdit, data, shiftStatus}) => {
  let currentDate = data?.find((item) => dayjs(item.EventDate).format('YYYY-MM-DD') === dayjs(date).format('YYYY-MM-DD'))
  let findedData = shiftStatus.all.find((item) => item.Id === currentDate?.ShiftId)
  return(
    <div className='h-[50px] w-full flex justify-between gap-2' >
        <div className='flex flex-col items-start'>
          <div className='font-medium'>{dayjs(date).format('DD')}</div>
        </div>
        {
          currentDate &&
          <Form.Item className='m-0 p-0 flex-1 self-center'  name={[currentDate.index, 'ShiftId']}>
            <Select 
              disabled={!isEdit} 
              bordered={false} 
              style={{background: findedData?.ColorCode}}
              className='w-full rounded-md'
              popupMatchSelectWidth={false}
              optionlabelProp="label"
              size='small'
            >
              {
                findedData ? findedData.OnSite === 1 ? 
                shiftStatus.onsite.map((item, index) => (
                  <Select.Option key={index} value={item.value} label={item.label}>
                    {/* {item.content ? item.content : item.label} */}
                    {item.label}
                  </Select.Option>
                ))
                : 
                shiftStatus.offsite.map((item, index) => (
                  <Select.Option key={index} value={item.value} label={item.label}>
                    {/* {item.content ? item.content : item.label} */}
                    {item.label}
                  </Select.Option>
                )) 
                : 
                shiftStatus.all.map((item, index) => (
                  <Select.Option key={index} value={item.value} label={item.label}>
                    {/* {item.content ? item.content : item.label} */}
                    {item.label}
                  </Select.Option>
                ))
              }
            </Select>
          </Form.Item>
        }
    </div>
  )
}

export default RenderCell