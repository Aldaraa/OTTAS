import { Select } from 'antd'
import { Button, Form, Modal, ReportFields } from 'components'
import React, { useEffect, useState } from 'react'
import { reportInstance } from 'utils/axios'
import ColumnSelection from '../Scheduler/ColumnSelection'

function RunTemplate({templates, form, onChangeParameters}) {
  const [ loading, setLoading ] = useState(false)
  const [ openModal, setOpenModal ] = useState(false)
  const [ templateDetail, setTemplateDetail ] = useState(null)

  const values = Form.useWatch([], form);

  useEffect(() => {

    if(values?.reportTemplateId){
      getTemplateDetail(values?.reportTemplateId)
    }
  },[values])

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
            filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            showSearch
            disabled
          />
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
            {
              templateDetail?.Columns?.length > 0 ?
              <tr>
                <td className='border border-slate-300 px-2'>Columns</td>
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
                      <td className='border border-slate-300 px-2'>{item.Descr} {item.Component}</td>
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
  )
}

export default RunTemplate