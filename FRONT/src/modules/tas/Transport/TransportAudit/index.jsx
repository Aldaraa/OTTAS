import React, { useCallback, useEffect, useRef, useState } from 'react'
import { Button, Modal, PeolpeSelectionTable } from 'components';
import axios from 'axios';
import { Form as AntForm, DatePicker, notification } from 'antd';
import { useLocation } from 'react-router-dom';
import dayjs from 'dayjs';
import columns from './columns';

function TransportAudit() {
  const routeLocation = useLocation();
  const [ loading, setLoading ] = useState(false)
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ showActionModal, setShowActionModal ] = useState(false)
  const [ api, contextHolder] = notification.useNotification();
  const [ searchForm ] = AntForm.useForm()
  const [ form ] = AntForm.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
  },[])

  useEffect(() => {
    form.setFieldsValue({employeeIds: selectedData})
  },[selectedData])

  const handleSelect = useCallback((rows) => {
    setCurrentSelectedData(rows)
  },[])


  const handleReturn = () => {
    const selectedData = currentSelectedData.map((item) => ({
      ...item,
      Fullname: `${item.Firstname} ${item.Lastname}`,
    }))
    setSelectedData(selectedData)
    setShowActionModal(true)
  }

  const handleSubmit = (values) => {
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/audit/transportaudit',
      responseType: 'blob',
      data: {
        ...values,
        startDate: dayjs(values.startDate).format('YYYY-MM-DD'),
        endDate: dayjs(values.endDate).format('YYYY-MM-DD'),
        employeeIds: values.employeeIds.map((item) => item.Id),
      }
    }).then((res) => {
      if(res.status === 200){
        const url = window.URL.createObjectURL(res.data); 
        const a = document.createElement('a');
        a.href = url;
        a.download = `TAS_TRANSPORT_AUDIT_${dayjs().format('YYYY-MM-dd_HH-mm-ss')}.xlsx`
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        setShowActionModal(false)
        dataGrid.current?.instance.clearSelection()
        setSelectedData([])
      }else{
        api.error({
          message: 'Audit data not found',
          duration: 5,
          // description: ''
        });
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCloseModal = () => {
    setShowActionModal(false)
    setSelectedData(form.getFieldValue('employeeIds'))
  }

  const handleRemoveRow = (id) => {
    dataGrid.current?.instance.deselectRows([id])
  }

  return (
    <div>
      <PeolpeSelectionTable
        ref={dataGrid}
        onSelect={handleSelect}
        onReturn={handleReturn}
        selectedRowsData={currentSelectedData}
        columns={columns}
      />
      <Modal 
        open={showActionModal} 
        onCancel={handleCloseModal} 
        title={<div>Transport Audit <span className='text-gray-400 font-normal'>({selectedData?.length} people selected)</span></div>} 
        width={1000}
      >
        <AntForm 
          form={form}
          size='small'
          onFinish={handleSubmit}
        >
          <AntForm.List name='employeeIds'>
            {(fields, { remove }) => (
              <div className='border rounded-ot'>
              <table className='table-auto overflow-hidden w-full'>
                <thead className='text-[#959595]'> 
                  <tr className='text-left'>
                    <th className='border-b px-1 font-normal w-[30px]'>#</th>
                    <th className='border-b px-1 font-normal'>#Person</th>
                    <th className='border-b px-1 font-normal'>Fullname</th>
                    <th className='border-b px-1 font-normal'>Department</th>
                    <th className='border-b px-1 font-normal'></th>
                  </tr>
                </thead>
                <tbody >
                  {
                    fields.map(({key, name, ...restField}) => (
                      <tr className={`${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'white'}`}>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} name={[name, 'Id']} className='mb-0'>
                            <div className='text-[13px] w-[30px]'>{name+1}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} name={[name, 'Id']} className='mb-0'>
                            <div className='text-[13px]'>{form.getFieldValue(['employeeIds', name, 'Id'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} className='mb-0'>
                            <div className='text-[13px]'>{form.getFieldValue(['employeeIds', name, 'Fullname'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <AntForm.Item {...restField} className='mb-0'>
                            <div>{form.getFieldValue(['employeeIds', name, 'DepartmentName'])}</div>
                          </AntForm.Item>
                        </td>
                        <td className='py-1 border-t px-1'>
                          <button
                            type='button'
                            className='dlt-button text-xs'
                            onClick={() => {
                              handleRemoveRow(form.getFieldValue(['employeeIds', name, 'Id']))
                              remove(name);
                            }}
                          >
                            Remove
                          </button>
                        </td>
                      </tr>
                    ))
                  }
                </tbody>
              </table>
            </div>
            )}
          </AntForm.List>
          <div className='flex gap-4 mt-4'>
            <AntForm.Item label='StartDate' key='form-startdate' className='mb-0' name='startDate' rules={[{required: true}]}>
              <DatePicker/>
            </AntForm.Item>
            <AntForm.Item label='endDate' key='form-enddate' className='mb-0' name='endDate' rules={[{required: true}]}>
              <DatePicker/>
            </AntForm.Item>
          </div>
        </AntForm>
        {
          selectedData?.length > 0 &&
          <div className='flex items-center justify-end gap-5 mt-4'>
            <Button 
              type='primary' 
              onClick={() => form.submit()}
              loading={loading}
            >
              Download
            </Button>
            <Button 
              onClick={handleCloseModal}
              disabled={loading}
            >
              Cancel
            </Button>
          </div>
        }
      </Modal>
      {contextHolder}
    </div>
  )
}

export default TransportAudit