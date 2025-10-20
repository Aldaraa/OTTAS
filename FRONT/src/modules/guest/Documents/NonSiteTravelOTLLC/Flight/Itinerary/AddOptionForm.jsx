import { MinusCircleOutlined, PlusOutlined, SaveOutlined } from '@ant-design/icons';
import { DatePicker, Input } from 'antd';
import axios from 'axios';
import { Button, Form } from 'components';
import dayjs from 'dayjs';
import React, { useState } from 'react'
import { useParams } from 'react-router-dom';

function AddOptionForm({getOptionsData, onCancel}) {
  const [ loading, setLoading ] = useState(false)
  const [ form ] = Form.useForm()
  const { documentId } = useParams()

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/requestnonsitetraveloption',
      data: {
        documentId,
        optionData: values.options.map((item) => ({
          ...item,
          selected: false,
          dueDate: dayjs(item.dueDate).format('YYYY-MM-DD HH:mm')
        })),
        newcost: null,
      }
    }).then((res) => {
      getOptionsData()
      onCancel()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }
  return (
    <Form
      form={form}
      onFinish={handleSubmit}
      noLayoutConfig={true}
      layout='vertical'
    >
      <Form.List name="options" className='col-span-12'>
        {(fields, { add, remove }) => (
          <div className='col-span-12 grid grid-cols-12 gap-x-2 gap-y-2 items-center'>
            {fields.map(({ key, name, ...restField }) => (
              <>
                <Form.Item
                  {...restField}
                  className='col-span-10 mb-0'
                  name={[name, 'optiontext']}
                  label={<span className='font-bold'>Option#{key+1}</span>}
                  // rules={[{ required: true, message: 'Missing first name' },]}
                >
                  <Input.TextArea 
                    autoSize={{minRows: 3, maxRows: 10}}
                  />
                </Form.Item>
                <button 
                  type='button'
                  className='bg-[#FFE2E5] text-[#F64E60] hover:bg-red-200 rounded-md py-1 disabled:bg-gray-100  transition-all' 
                  onClick={() => remove(name)}
                >
                  <MinusCircleOutlined />
                </button>
                <Form.Item
                  className='col-span-6 mb-0'
                  name={[name, 'dueDate']}
                  label='Deadline'
                  rules={[{required: true, message: 'Deadline is required !'}]}
                >
                  <DatePicker className='w-full' showTime showSecond={false} format='YYYY-MM-DD HH:mm'/>
                </Form.Item>
              </>
            ))}
            <Form.Item className='col-span-12'>
              <Button className='w-full flex justify-center' onClick={() => add()} icon={<PlusOutlined />}>
                Add Option
              </Button>
            </Form.Item>
          </div>
        )}
      </Form.List>
      <div className='col-span-12 flex justify-end items-center gap-2'>
        <Button type='primary' onClick={() => form.submit()} loading={loading} icon={<SaveOutlined/>}>Save</Button>
        <Button onClick={() => {onCancel(); form.resetFields()}} disabled={loading}>Cancel</Button>
      </div>
    </Form>
  )
}

export default AddOptionForm