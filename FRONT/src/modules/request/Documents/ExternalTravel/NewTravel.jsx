import { DatePicker, Input, Select } from 'antd';
import axios from 'axios';
import { Button, Form, Modal, TransportSearch } from 'components';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import React, { useContext, useEffect, useState } from 'react'

function NewExternalTravel({documentDetail, refreshData, disabled}) {
  const [ loading, setLoading ] = useState(false)
  const [ transportSelectionInitValues, setTransportSelectionInitValues ] = useState(null)
  const [ showLTSelection, setShowLTSelectionModal ] = useState(false)
  const [ isEdit, setIsEdit ] =  useState(false)

  const { state, action } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    if(documentDetail){
      initFormDefaultValues(documentDetail.TravelData)
    }
  },[documentDetail])

  const initFormDefaultValues = (data) => {
    form.setFieldsValue({
      ...data,
      firstTransport: {
        Date: data.FirstScheduleDate ? dayjs(data.FirstScheduleDate) : null,
        Description: `${data.FirstScheduleDescription}`,
        flightId: data.FirstScheduleId,
      },
      lastTransport: {
        Date: data.LastScheduleDate ? dayjs(data.LastScheduleDate) : null,
        Description: `${data.LastScheduleDescription}`,
        flightId: data.LastScheduleId,
      }
    })
  }


  const handleSelectionButtonFT = () => {
    let tmp = {
      StartDate: form.getFieldValue(['firstTransport', 'Date']) ? form.getFieldValue(['firstTransport', 'Date']) : null,
      EndDate: null,
    }
    setTransportSelectionInitValues({...tmp, transportType: 'first'})
    setShowLTSelectionModal(true)
  }

  const handleSelectionButtonLT = () => {
    let tmp = {
      StartDate: form.getFieldValue(['lastTransport', 'Date']) ? form.getFieldValue(['lastTransport', 'Date']) : null,
      EndDate: null,
    }
    setTransportSelectionInitValues({...tmp, transportType: 'last'})
    setShowLTSelectionModal(true)
  }

  const onChangeFirstDate = (e) => {
    action.setFlightCalendarDate(dayjs(e).format('YYYY-MM-DD'))
    form.setFieldValue(['firstTransport', 'flightId'], null)
    form.setFieldValue(['firstTransport', 'Description'], null)
  }

  const fields = [
    {
      type: 'component',
      component: <>
      <Form.Item label='First Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item name={['firstTransport', 'Date']} className='mb-0' rules={[{required: true, message: 'First Transport Date is required'}]}>
            <DatePicker showWeek onChange={onChangeFirstDate}/>
          </Form.Item>
          <Form.Item className='mb-0 w-[100px]' rules={[{required: true, message: 'Direction is required'}]}>
            <Input disabled value={'EXTERNAL'}/>
          </Form.Item>
          <Form.Item noStyle name={['firstTransport', 'flightId']}>
          </Form.Item>
          <Form.Item name={['firstTransport', 'Description']} className='flex-1 mb-0'>
            <Input readOnly/>
          </Form.Item>
          <Form.Item
            className='mb-0'
            shouldUpdate={(pre, cur) => pre.firstTransport?.Date !== cur.firstTransport?.Date}
          >
            {
              ({getFieldValue}) => (
                <Button
                  className='text-xs py-[2px]'
                  type={'primary'}
                  onClick={handleSelectionButtonFT}
                  disabled={!getFieldValue(['firstTransport', 'Date']) || !isEdit}
                >
                  ...
                </Button>
              )
            }
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='Last Transport' className='col-span-12 mb-2'>
        <div className='flex gap-2'>
          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.firstTransport?.Date !== cur.firstTransport?.Date}>
            {({getFieldValue}) => {
              let defaultPickerValue = false
              if(!getFieldValue(['lastTransport', 'Date'])){
                defaultPickerValue = getFieldValue(['firstTransport', 'Date'])
              }
              return(
                <Form.Item name={['lastTransport', 'Date']} className='mb-0'>
                  <DatePicker defaultPickerValue={defaultPickerValue} showWeek className='w-full'/>
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item className='mb-0 w-[100px]'>
            <Input disabled value={'EXTERNAL'}/>
          </Form.Item>
          <Form.Item noStyle name={['lastTransport', 'flightId']}>
          </Form.Item>
          <Form.Item name={['lastTransport', 'Description']} className='flex-1 mb-0'>
            <Input readOnly/>
          </Form.Item>
          <Form.Item
            className='col-span-1 mb-0'
            shouldUpdate={(pre, cur) => pre.lastTransport?.Date !== cur.lastTransport?.Date}
          >
            {
              ({getFieldValue}) => (
                <Button
                  className='text-xs py-[2px]'
                  type={'primary'}
                  onClick={handleSelectionButtonLT}
                  disabled={!getFieldValue(['lastTransport', 'Date']) || !isEdit}
                >
                  ...
                </Button>
              )
            }
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      label: 'Department',
      name: 'DepartmentId',
      className: 'col-span-12 mb-2',
      type: 'treeSelect',
      rules: [{required: true, message: 'Department is required'}],
      inputprops: {
        treeData: state.referData?.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      }
    },
    {
      label: 'Position',
      name: 'PositionId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Position is required'}],
      inputprops: {
        options: state.referData?.positions,
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.DepartmentId !== cur.DepartmentId}>
        {({getFieldValue, setFieldValue}) => {
          let selectedDepartment = state.referData.departments?.find((department) => department.Id === getFieldValue('DepartmentId'))
          if(selectedDepartment && selectedDepartment.CostCodeId){
            setFieldValue('CostCodeId', selectedDepartment.CostCodeId)
          }
          return (
            <Form.Item 
              name='CostCodeId' 
              label='CostCodeId'
              rules={[{required: true, message: 'Cost Code is required'}]}
              className='col-span-12 mb-2'
            >
              <Select
                options={state.referData.costCodes}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item>
          )
        }}
      </Form.Item>
    },
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Employer is required'}],
      inputprops: {
        options: state.referData?.employers,
      }
    },
  ]

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestexternaltravel/addtravel',
      data: {
        firstScheduleId: values.firstTransport.flightId,
        lastScheduleId: values.lastTransport.flightId ? values.lastTransport.flightId : null,
        departmentId: values.DepartmentId,
        positionId: values.PositionId,
        employerId: values.EmployerId,
        costCodeId: values.CostCodeId,
        Id: documentDetail?.TravelData.Id,
      }
    }).then((res) => {
      refreshData()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleSelect = (event) => {
    if(transportSelectionInitValues.transportType === 'first'){
      form.setFieldValue(['firstTransport', 'Date'], dayjs(event.EventDate))
      form.setFieldValue(['firstTransport', 'Description'], `${event.Code} ${event.Description}`)
      form.setFieldValue(['firstTransport', 'flightId'], event.Id)
      setShowLTSelectionModal(false)
    }else{
      form.setFieldValue(['lastTransport', 'Date'], dayjs(event.EventDate))
      form.setFieldValue(['lastTransport', 'Description'], `${event.Code} ${event.Description}`)
      form.setFieldValue(['lastTransport', 'flightId'], event.Id)
      setShowLTSelectionModal(false)
    }
  }

  const onCancel = (e) => {
    e.stopPropagation()
    setIsEdit(false)
    initFormDefaultValues(documentDetail?.TravelData)
  }

  return (
    <div>
      <Form
        form={form} 
        fields={fields}
        onFinish={handleSubmit}
        labelCol={{flex: '150px'}} 
        wrapperCol={{flex: 1}}
        disabled={!isEdit}
        size='small'
      >
        {
          !disabled ?
          <div className='col-span-12 gap-3 flex justify-end mt-3'>
            {
              isEdit ? 
              <>
                <Button type='primary' onClick={() => form.submit()} loading={loading}>Save</Button>
                <Button onClick={onCancel}>Cancel</Button>
              </>
              :
              <Button onClick={() => setIsEdit(true)}>Edit</Button>
            }
          </div>
          : null
        }
      </Form>
      <Modal
        title='Select Transport'
        open={showLTSelection}
        zIndex={1100}
        width={900}
        onCancel={() => setShowLTSelectionModal(false)}
        destroyOnClose={false}
      >
        <TransportSearch
          initialSearchValues={transportSelectionInitValues}
          handleSelect={handleSelect}
          isExternal={true}
        />
      </Modal>
    </div>
  )
}

export default NewExternalTravel