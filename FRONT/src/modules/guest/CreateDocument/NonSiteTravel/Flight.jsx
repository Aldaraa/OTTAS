import axios from 'axios'
import { Button, Form, Table, Modal } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { PlusOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'

function Flight({mainForm, getMaster}) {
  const [ airports, setAirports ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ editData, setEditData ] = useState(null)

  const { state, action } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    axios({
      method: 'get',
      url: 'tas/requestairport'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({ value: item.Id, label: `${item.Code} (${item.Country})`})
      })
      setAirports(tmp)
    }).catch((err) => {

    }).then(() => setLoading(false))
  },[])


  function fetchAirport(value) {
    return axios({
      method: 'get',
      url: `tas/requestairport/search/${value}`,
    }).then((res) => 
      res.data.map((item) => ({
        value: item.Id, label: `${item.Code} (${item.Country})`
      })),
    )
  }

  const fields = [
    {
      label: 'Travel Date',
      name: 'travelDate',
      className: 'col-span-12 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true
      }
    },
    {
      label: 'Favor Time',
      name: 'FavorTime',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.master?.favorTimes
      }
    },
    {
      label: 'Depart Location',
      name: 'departLocationId',
      className: 'col-span-12 mb-2',
      type: 'searchSelect',
      inputprops: {
        fetchOptions: fetchAirport,
        showSearch: true,
        placeholder: 'Search location',
        allowClear: true,
      }
    },
    {
      label: 'Arrive Location',
      name: 'arriveLocationId',
      className: 'col-span-12 mb-2',
      type: 'searchSelect',
      inputprops: {
        fetchOptions: fetchAirport,
        showSearch: true,
        placeholder: 'Search location',
        allowClear: true,
      }
    },
    // {
    //   label: 'Arrive Location',
    //   name: 'arriveLocationId',
    //   className: 'col-span-12 mb-2',
    //   type: 'select',
    //   inputprops: {
    //     options: airports,
    //   }
    // },
    {
      label: 'Comment',
      name: 'comment',
      className: 'col-span-12 mb-2',
      type: 'textarea'
    },
    {
      label: 'ETD',
      name: 'etd',
      className: 'col-span-12 mb-2',
      type: 'check'
    },
  ]

  const handleDeleteButton = (row, event) => {
    event.stopPropagation()
    let tmp = [...mainForm.getFieldValue('flightData')]
    tmp.splice(row.rowIndex, 1)
    mainForm.setFieldValue('flightData', tmp)
    getMaster()
    // handleChange((prevData) => ({
    //   ...prevData,
    //   flightData: tmp
    // }))
  }

  const handleEditButton = (row, event) => {
    event.stopPropagation()
    setEditData({
      ...row.data,
      travelData: row.data.travelData ? dayjs(row.data.travelData) : null,
      index: row.rowIndex
    })
    setShowModal(true)
  }

  const columns = [
    {
      label: 'Travel Date',
      name: 'travelDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd ')}</div>
      )
    },
    {
      label: 'Favor Time',
      name: 'FavorTime',
    },
    {
      label: 'Depart Location',
      name: 'departLocationId',
      alignment: 'left',
      cellRender: (e) => (
        <div>{airports?.find((item) => e.value === item.value)?.label }</div>
      )
    },
    {
      label: 'Arrive Location',
      name: 'arriveLocationId',
      alignment: 'left',
      cellRender: (e) => (
        <div>{airports?.find((item) => e.value === item.value)?.label }</div>
      )
    },
    {
      label: 'Comment',
      name: 'comment',
    },
    {
      label: 'ETD',
      name: 'etd',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '150px', 
      alignment: 'center',
      cellRender: (e) => (
        <div className='flex items-center gap-2'>
          <button type='button' className='edit-button' onClick={(event) => handleEditButton(e, event)}>Edit</button>
          <button type='button' className='dlt-button' onClick={(event) => handleDeleteButton(e, event)}>Delete</button>
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    if(editData){
      let tmp = [...mainForm.getFieldValue('flightData')]
      tmp[editData.index] = {...values, travelDate: values.travelDate ? dayjs(values.travelDate).format('YYYY-MM-DD') : null}
      mainForm.setFieldValue('flightData', [...tmp])
    }else{
      mainForm.setFieldValue('flightData', [...mainForm.getFieldValue('flightData'), {...values, travelDate: values.travelDate ? dayjs(values.travelDate).format('YYYY-MM-DD') : null}])
    }
    getMaster()
    handleCloseModal(false)
  }

  const handleCloseModal = () => {
    form.resetFields()
    setShowModal(false)
  }

  return (
    <>
      <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.flightData !== curValues.flightData} className='col-span-12'>
        {
          ({getFieldValue}) => {
            return(
              <Table
                data={mainForm.getFieldValue('flightData')}
                // data={data}
                columns={columns}
                allowColumnReordering={false}
                loading={loading}
                containerClass='shadow-none border border-gray-300'
                keyExpr='Id'
                pager={mainForm.getFieldValue('flightData')?.length > 20}
                title={
                  <div className='flex justify-between items-center py-2 gap-3 border-b'>
                    <div className='text-md font-bold pl-2'>Flight</div>
                    <div className='flex gap-4 items-center'>
                      <Button 
                        htmlType='button'
                        icon={<PlusOutlined />} 
                        onClick={() => setShowModal(true)}
                        className='text-xs'
                        // disabled={mainForm.getFieldValue('flightData')?.length > 0}
                      >
                        Add Flight
                      </Button>
                    </div>
                  </div>
                }
              />
            )
          }
        }
      </Form.Item>
      <Modal title='Add Filght' open={showModal} onCancel={handleCloseModal}>
        <Form
          form={form}
          fields={fields}
          size='small' 
          editData={editData}
          className='grid grid-cols-12 gap-x-8'
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='button' onClick={() => form.submit()} type='primary' icon={<PlusOutlined/>}>Add</Button>
            <Button htmlType='button' onClick={handleCloseModal}>Cancel</Button>
          </div>
        </Form>
      </Modal>
    </>
  )
}

export default Flight