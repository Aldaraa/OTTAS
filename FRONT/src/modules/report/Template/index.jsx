import { Button, Form, Modal, Table } from 'components'
import { CheckBox } from 'devextreme-react'
import React, { useContext, useEffect, useRef, useState } from 'react'
import { AuthContext } from 'contexts'
import { reportInstance } from 'utils/axios'
import { Input } from 'antd'
import { PlayCircleOutlined } from '@ant-design/icons'
import RunTemplate from './RunTemplate'
import { saveAs } from 'file-saver-es'

function Template() {
  const [ renderData, setRenderData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ runBuildLoading, setRunBuildLoading ] = useState(false)
  const [ paramObj, setParamObj ] = useState(null)
  const [ showRunModal, setShowRunModal ] = useState(false)
  const [ selectedTemplate, setSelectedTemplate ] = useState([])
  const gridRef = useRef(null)
  const { action, state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    action.changeMenuKey('/report/template')
    getData()
    getDateTypes()
  },[])

  const getData = () => {
    gridRef.current.instance?.beginCustomLoading();
    reportInstance({
      method: 'get',
      url: 'tasreport/ReportTemplate'
    }).then((res) => {
      setRenderData(res.data?.map((item) => ({...item, value: item.Id, label: item.Description})))
    }).catch((err) => {

    }).then(() => gridRef.current?.instance.endCustomLoading())
  }

  const handleEdit = (data) => {
    reportInstance({
      method: 'get',
      url: `tasreport/ReportTemplate/${data.Id}`,
    }).then((res) => {
      res.data.Parameters.map((item) => {
        form.setFieldValue([item.FielName, 'Id'], item.Id)
        form.setFieldValue([item.FielName, 'Descr'], item.Descr)
      })
      setEditData(res.data)
      setShowModal(true)
    }).catch((err) => {

    })
  }

  const getDateTypes = () => {
    if(!state.referData.dateTypes){
      reportInstance.get('tasreport/reporttemplate/datetypes').then((res) => {
        let tmp = res.data.DateVariables?.map((item) => ({
          value: item, 
          label: item, 
        }))
        action.setReferDataItem({ 'dateTypes': tmp })
      }).catch((err) => {
  
      })
    }
  }

  const handleRunButton = (row) => {
    form.setFieldValue('reportTemplateId', row.Id)
    setSelectedTemplate(row)
    setShowRunModal(true)
  }
  const columns = [
    {
      label: 'Code',
      name: 'Code',
    },
    {
      label: 'Description',
      name: 'Description'
    },
    {
      label: 'Schedules',
      name: 'ScheduleCount',
      alignment: 'left',
    },
    {
      label: 'Active',
      name: 'Active',
      width: '80px',
      alignment: 'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Active === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '120px',
      alignment: 'center',
      cellRender: (event) => (
        <div className='flex gap-2 items-center justify-end'>
          <button className='edit-button' onClick={(e) => handleRunButton(event.data)}>Run</button>
          { 
            state.userInfo?.Role === 'SystemAdmin' ? 
            <button className='edit-button' onClick={(e) => handleEdit(event.data)}>Edit</button> :
            null
          }
        </div>
      )
    }
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    const keys = Object.keys(values)
    let tmp = []
    keys.map((key) => {
      tmp.push(values[key])
    })

    reportInstance({
      method: 'put',
      url: `tasreport/ReportTemplate/parameter`,
      data: {
        data: tmp
      }
    }).then((res) => {
      setShowModal(false)
      setEditData(null)
    }).catch((err) => {

    }).finally(() => setActionLoading(false))
  }

  const handleRunBuild = () => {
    setRunBuildLoading(true)
    const values = form.getFieldsValue()
    let params = []
    if(values.parameters){
      Object.keys(values.parameters).map((fieldName) => {
        if(values.parameters[fieldName]){
          if(fieldName === 'StartDate' || fieldName === 'EndDate' || fieldName === 'CurrentDate'){
            params.push({
              parameterId: paramObj[fieldName].Id,
              parameterValue: values.parameters[fieldName].FieldValue,
              Days: values.parameters[fieldName].Days,
            })
          }else{
            params.push({
              parameterId: paramObj[fieldName].Id,
              parameterValue: typeof values.parameters[fieldName] !== 'string' ? JSON.stringify(values.parameters[fieldName]) : values.parameters[fieldName]
            })
          }
        }
      })
    }
    reportInstance({
      method: 'post',
      url: 'reportjob/buildreport',
      responseType: 'blob',
      data: {
        reportTemplateId: values.reportTemplateId,
        columnIds: values.columnIds,
        parameters: params,
      }
    }).then((res) => {
      const filename = res.headers['content-disposition'].split("; ")[1].replace('filename=', '')
      saveAs(res.data, filename)
      setShowRunModal(false)
    }).catch((err) => {
    }).then(() => setRunBuildLoading(false))
  }



  return (
    <div>
      <Table
        ref={gridRef}
        data={renderData||[]}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        tableClass='border-t'
        // onRowPrepared={(e) => {
        //   if (e.rowType === "data") {
        //     if (e.data.Active === 1) {
        //       e.rowElement.style.backgroundColor = "#7ebe81";
        //     }
        //   }
        // }}
        rowAlternationEnabled={true}
        pager={false}
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{renderData?.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
        </div>}
      />
      <Modal
        open={showModal}
        title={`Edit Descriptions of ${editData?.Description}`}
        onCancel={() => setShowModal(false)}
        noLayoutConfig={true}
        >
        <Form
          form={form}
          onFinish={handleSubmit}
          layout='vertical'
          className='grid grid-cols-12 gap-x-8'
        >
          {
            editData?.Parameters.map((item) => (
              <div className='col-span-12'>
                <Form.Item noStyle name={[item.FielName, 'Id']} className='col-span-12'>
                </Form.Item>
                <Form.Item name={[item.FielName, 'Descr']} label={item.FielName} className='col-span-12 mb-2'>
                  <Input.TextArea/>
                </Form.Item>
              </div>
            ))
          }
          <div className='col-span-12 flex justify-end gap-4'>
            <Button type='primary' onClick={() => form.submit()}>Save</Button>
            <Button onClick={() => setShowModal(false)}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Modal 
        footer={
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button
              type='primary'
              onClick={() => form.submit()}
              loading={runBuildLoading}
              icon={<PlayCircleOutlined />}
            >
              Run
            </Button>
          </div>
        } 
        open={showRunModal} 
        width={1100} 
        onCancel={() => setShowRunModal(false)} 
        title={`Run Job`}
      >
        <Form
          labelAlign='left'
          form={form}
          onFinish={handleRunBuild}
          className={'gap-x-4'}
          labelCol={{sm: { span: 6}}}
          initValues={{scheduleType: 'Daily'}}
        >
          <RunTemplate onChangeParameters={(e) => setParamObj(e)} templates={renderData} form={form}/>
        </Form>
      </Modal>
    </div>
  )
}

export default Template