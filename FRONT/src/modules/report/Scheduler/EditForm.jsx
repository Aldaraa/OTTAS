import { DatePicker, Input, InputNumber, Segmented, Select, Space } from 'antd'
import { AvailableDatePicker, AvailableTimeTooltip, Button, Form, Modal, ReportFields } from 'components'
import React, { useEffect, useState } from 'react'
import { reportInstance } from 'utils/axios'
import ColumnSelection from './ColumnSelection'
import { LoadingOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'

const setParamsValues = (parameters) => {
  let params = {}
  parameters?.map((item) => {
    if(item.Component.includes('DROPDOWN')){
      params[item.FieldName] = item.ParameterValue ? JSON.parse(item.ParameterValue) : [] 
    }else if(item.Component === 'TAS_DYNAMIC_DATE' && item.ParameterValue){
      if (item.ParameterValue.startsWith("{") && item.ParameterValue.endsWith("}")){
        params[item.FieldName] = {
          Days: item.Days,
          FieldValue: item.ParameterValue,
        }
      }else{
        params[item.FieldName] = {
          Days: null,
          FieldValue: null,
          Date: dayjs(item.ParameterValue)
        }
      }
    }else if(item.Component === 'TAS_STATIC_DATE'){
      params[item.FieldName] = dayjs(JSON.parse(item.ParameterValue))
    }
    else {
      params[item.FieldName] = JSON.parse(item.ParameterValue)
    }
  })

  return params
}

function EditScheduleForm({templates, form, onChangeParameters, editData, departments}) {
  const [ loading, setLoading ] = useState(false)
  const [ openModal, setOpenModal ] = useState(false)
  const [ templateDetail, setTemplateDetail ] = useState(null)
  const [ data, setData ] = useState(null)

  const values = Form.useWatch([], form);

  useEffect(() => {
    if(editData){
      getJobDetail()
    }
  },[editData])

  useEffect(() => {
    if(!editData && values?.reportTemplateId){
      getTemplateDetail(values?.reportTemplateId)
    }
  },[values])

  const getJobDetail = () => {
    reportInstance({
      method: 'get',
      url: `reportjob/${editData.Id}`
    }).then((res) => {
      setData(res.data)
      getTemplateDetail(res.data?.reportTemplateId)
      const parameters = setParamsValues(res.data?.Parameters)
      let commands = JSON.parse(res.data.Command)
      form.setFieldsValue({
        ...res.data,
        StartDate: res.data.StartDate ? dayjs(res.data.StartDate) : null,
        EndDate: res.data.EndDate ? dayjs(res.data.EndDate) : null,
        scheduleType: res.data?.ScheduleType,
        months: JSON.parse(res.data.Command)?.months,
        days: JSON.parse(res.data.Command)?.days,
        week: JSON.parse(res.data.Command)?.days,
        recureEvery: commands?.recureEvery,
        columnIds: res.data.Columns?.map(col => JSON.stringify(col.ColumnId)),
        parameters: parameters
      })
    }).catch((err) => {

    })
  }

  const getTemplateDetail = (id) => {
    setLoading(true)
    reportInstance({
      method: 'get',
      url: `tasreport/reporttemplate/${id}`
    }).then((res) => {
      let tmp = []
      let parameters = {}
      res.data.Columns.map((item, index) => {
        tmp.push({...item, key: `${item.Id}`, title: item.Caption, label: item.Caption, value: `${item.Id}`})
      })
      res.data.Parameters.map((item, index) => {
        parameters[`${item.FielName}`] = item
      })
      if(onChangeParameters){
        onChangeParameters(parameters)
      }
      setTemplateDetail({...res.data, Columns: tmp})
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSaveColumnsSelection = (e) => {
    form.setFieldValue('columnIds', e)
    setOpenModal(false)
  }


  return (
    <>
      {
        !data ? 
        <div className='h-[100px] col-span-12 flex items-center justify-center'>
          <LoadingOutlined style={{fontSize: 28}}/>
        </div> 
        :
        <div className='col-span-12 flex flex-col gap-6'>
          <div className='border rounded-ot py-4 px-6 pb-2'>
            <div className='text-xl mb-3'>Report</div>
            <Form.Item 
              className='col-span-6 mb-2'
              name='reportTemplateId'
              label='Report Template'
              rules={[{required: true, message: 'Report Template is required'}]}
            >
              <Select 
                options={templates}
                placeholder='Select template'
                disabled
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                showSearch
              />
            </Form.Item>
          </div>
          <div className='border rounded-ot px-6 py-4'>
            <div className='text-xl mb-3'>Job</div>
            <Form.Item
              className='col-span-6 mb-2'
              name='Name'
              label='Job name'
              rules={[{required: true, message: 'Job name is required'}]}
            >
              <Input/>
            </Form.Item>
            <Form.Item className='col-span-6 mb-2' name='Description' label='Job description'>
              <Input.TextArea/>
            </Form.Item> 
          </div>
          <div className='border rounded-ot px-4 py-4'>
            <div className='text-xl mb-3'>Actions</div>
            <table className='w-full table table-fixed border-collapse border rounded-t-md border-slate-300 text-xs'>
              <thead>
                <tr className='text-left text-gray-400'>
                  <th className='border border-slate-300 px-2 font-medium w-[140px]'>Name</th>
                  <th className='border border-slate-300 px-2 font-medium w-auto'>Description</th>
                  <th className='border border-slate-300 px-2 font-medium w-auto'>Value</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td className='border border-slate-300 px-2'>to Emails<span className='text-red-500'>*</span></td>
                  <td className='border border-slate-300 px-2'>Override the default to email address for the report with the supplied email address</td>
                  <td className='border border-slate-300 px-2 flex gap-2'>
                    <Form.Item
                      name={'subscriptionMails'}
                      className='my-1 flex-1'
                      rules={[{required: true, message: 'Emails is required'}]}
                    >
                      <Select
                        mode="tags"
                        style={{ width: '100%' }}
                        placeholder="Write email"
                      />
                    </Form.Item>
                  </td>
                </tr>
                {
                  templateDetail?.Columns?.length > 0 ?
                  <tr>
                    <td className='border border-slate-300 px-2'>Columns<span className='text-red-500'>*</span></td>
                    <td className='border border-slate-300 px-2'>Include these additional fields in the report</td>
                    <td className='border border-slate-300 px-2 flex gap-2'>
                      <Form.Item
                        name={'columnIds'}
                        className='my-1 flex-1'
                        // rules={[{required: true, message: 'Columns is required'}]}
                      >
                        <Select options={templateDetail?.Columns} mode='multiple' disabled/>
                      </Form.Item>
                      <Form.Item className='my-1'>
                        <Button onClick={() => setOpenModal(true)}>...</Button>
                      </Form.Item>
                    </td>
                  </tr>
                  : null
                }
                <Form.Item noStyle name='parameters'>
                  {
                    templateDetail?.Parameters.map((item) => {
                      return(
                        item.Component === 'TAS_DYNAMIC_DATE' ? 
                        <tr className='bg-[#e4f2fd]'>
                          <td className='border border-slate-300 px-2'>{item.Caption}</td>
                          <td className='border border-slate-300 px-2'>{item.Descr ? item.Descr : item.Component}</td>
                          <td className='border border-slate-300 px-2'>
                              {ReportFields(item, 'parameters', form, templateDetail)}
                          </td>
                        </tr>
                        :
                        <tr>
                          <td className='border border-slate-300 px-2'>{item.Caption}</td>
                          <td className='border border-slate-300 px-2'>{item.Descr}</td>
                          <td className='border border-slate-300 px-2'>
                            {ReportFields(item, 'parameters', form, templateDetail)}
                          </td>
                        </tr>
                      )
                    })
                  }
                </Form.Item>
              </tbody>
            </table>
          </div>
          <div className='border rounded-ot px-6 py-4'>
            <div className='text-xl mb-3'>Schedule</div>
            <Form.Item name={'scheduleType'} className='flex justify-center'>
              <Segmented options={['Daily', 'Weekly', 'Monthly', 'RunTime']}/>
            </Form.Item>
            <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.scheduleType !== curValues.scheduleType}>
              {
                ({getFieldValue}) => {
                  const scheduleType = getFieldValue('scheduleType')
                  return(
                    <>
                    <Form.Item label='Starting on' className='mb-2'>
                      <Space align='center'>
                        <Form.Item 
                          name='StartDate'
                          className='mb-0 inline-block' 
                          rules={[{required: true, message: 'Start date is required'}]}
                        >
                          <AvailableDatePicker/>
                        </Form.Item>
                        <AvailableTimeTooltip/>
                      </Space>
                    </Form.Item>
                      {
                        scheduleType !== 'RunTime' ?
                        <Form.Item 
                          label='Ends on'
                          name='EndDate'
                          className='mb-2'
                          rules={[{required: true, message: 'End date is required'}]}
                        >
                          <DatePicker format={'YYYY-MM-DD'}/>
                        </Form.Item>
                        : null
                      }
                      {
                        scheduleType === 'Daily' ?
                        <Form.Item 
                          label='Recure every day(s)' 
                          name='recureEvery'
                          className='mb-2'
                          rules={[{required: true, message: 'Recure Day is required'}]}
                        >
                          <InputNumber min={1} controls={false}/>
                        </Form.Item>
                        :
                        scheduleType === 'Weekly' ? 
                        <>
                          <Form.Item 
                            label='Recure every day(s)'
                            name='recureEvery'
                            className='mb-2'
                            rules={[{required: true, message: 'Recure Day is required'}]}
                            // tooltip="This is a required field"
                          >
                            <InputNumber min={1} controls={false}/>
                          </Form.Item>
                          <Form.Item 
                            name='week' 
                            label='Selected day(s)'
                            className='mb-2'
                            rules={[{required: true, message: 'Week day is required'}]}
                          >
                            <Select 
                              options={[
                                { label: 'Monday', value: 'MON'}, 
                                { label: 'Tuesday', value: 'TUE'}, 
                                { label: 'Wednesday', value: 'WED' }, 
                                { label: 'Thursday', value: 'THU' }, 
                                { label: 'Friday', value: 'FRI' }, 
                                { label: 'Saturday', value: 'SAT' }, 
                                { label: 'Sunday', value: 'SUN' }
                              ]} 
                              mode='multiple'
                            />
                          </Form.Item>
                        </>
                        :
                        scheduleType === 'Monthly' ? 
                        <>
                          <Form.Item 
                            name='months' 
                            label='Selected month(s)' 
                            className='mb-2' 
                            rules={[{required: true, message: 'Month is required'}]}
                          >
                            <Select 
                              options={[
                                { label: 'January', value: 'JAN'}, 
                                { label: 'February', value: 'FEB'}, 
                                { label: 'March', value: 'MAR' }, 
                                { label: 'April', value: 'APR' }, 
                                { label: 'May', value: 'MAY' }, 
                                { label: 'June', value: 'JUN' }, 
                                { label: 'July', value: 'JUL' },
                                { label: 'August', value: 'AUG' },
                                { label: 'September', value: 'SEP' },
                                { label: 'October', value: 'OCT' },
                                { label: 'November', value: 'NOV' },
                                { label: 'December', value: 'DEC' },
                              ]} 
                              mode='multiple'
                            />
                          </Form.Item>
                          <Form.Item 
                            name='days'
                            label='On date(s)'
                            className='mb-2'
                            rules={[{required: true, message: 'Date is required'}]}
                          >
                            <Select 
                              options={[
                                { label: '1', value: '1'},
                                { label: '2', value: '2'},
                                { label: '3', value: '3' },
                                { label: '4', value: '4' },
                                { label: '5', value: '5' },
                                { label: '6', value: '6' },
                                { label: '7', value: '7' },
                                { label: '8', value: '8' },
                                { label: '9', value: '9' },
                                { label: '10', value: '10' },
                                { label: '11', value: '11' },
                                { label: '12', value: '12' },
                                { label: '13', value: '13' },
                                { label: '14', value: '14' },
                                { label: '15', value: '15' },
                                { label: '16', value: '16' },
                                { label: '17', value: '17' },
                                { label: '18', value: '18' },
                                { label: '19', value: '19' },
                                { label: '20', value: '20' },
                                { label: '21', value: '21' },
                                { label: '22', value: '22' },
                                { label: '23', value: '23' },
                                { label: '24', value: '24' },
                                { label: '25', value: '25' },
                                { label: '26', value: '26' },
                                { label: '27', value: '27' },
                                { label: '28', value: '28' },
                                { label: '29', value: '29' },
                                { label: '30', value: '30' },
                              ]} 
                              mode='multiple'
                            />
                          </Form.Item>
                        </>
                        :
                        null
                      }
                    </>
                  )
                }
              }
            </Form.Item>
          </div>
          <Modal 
            width={700}
            title='Column Selection'
            open={openModal}
            onCancel={() => setOpenModal(false)}
          >
            <ColumnSelection 
              data={templateDetail?.Columns}
              onCancel={() => setOpenModal(false)}
              onSave={handleSaveColumnsSelection}
              values={form.getFieldValue('columnIds')}
            />
          </Modal>
        </div>
      }
    </>
  )
}

export default EditScheduleForm