import { ActionForm, CustomSegmented, Form, Table, Button, Modal, PeopleSearch } from 'components'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm, DatePicker, Input, InputNumber, Select, Tag } from 'antd'
import axios from 'axios'
import { MinusCircleOutlined, PlusOutlined, SaveOutlined, SearchOutlined } from '@ant-design/icons'
import { render } from '@testing-library/react'
import tableSearch from 'utils/TableSearch'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import { Link, useNavigate } from 'react-router-dom'

const title = 'Events'
const initValues = {
  Code: '',
  Number: '',
  Description: '',
  Active: 1
}

function SeatBlock() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ startFlights, setStartFlights ] = useState([])
  const [ endFlights, setEndFlights ] = useState([])
  const [ showPeopleSearch, setShowPeopleSearch ] = useState(false)
  const [ personCount, setPersonCount ] = useState(0)

  const [ form ] = AntForm.useForm()
  const values = AntForm.useWatch([], form)
  const [ searchForm ] = AntForm.useForm()
  const { action } = useContext(AuthContext)
  const navigate = useNavigate()

  useEffect(() => {
    action.changeMenuKey('/tas/seatblock')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/visitevent'
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }
  
  const handleAddButton = () => {
    setEditData(null)
    form.resetFields()
    setShowModal(true)
  }
  
  const handleEditButton = (dataItem) => {
    setEditData(dataItem)
    getStartFlights(dayjs(dataItem.StartDate).format('YYYY-MM-DD'), 'IN')
    getEndFlights(dayjs(dataItem.EndDate).format('YYYY-MM-DD'), 'OUT')
    form.setFieldValue('Name', dataItem.Name)
    form.setFieldValue('StartDate', dayjs(dataItem.StartDate))
    form.setFieldValue('EndDate', dayjs(dataItem.EndDate))
    form.setFieldValue('InScheduleId', dataItem.InScheduleId)
    form.setFieldValue('OutScheduleId', dataItem.OutScheduleId)
    form.setFieldValue('HeadCount', dataItem.HeadCount)
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const columns = [
    {
      label: 'Name',
      name: 'Name',
    },
    {
      label: 'Requester',
      name: 'Requester',
    },
    {
      label: 'Start',
      name: 'StartDate',
      width: '200px',
      cellRender:(e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Ends',
      name: 'EndDate',
      width: '200px',
      cellRender:(e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
    {
      label: 'Duration (nights)',
      name: 'Description',
      cellRender:(e) => (
        <div>{dayjs(e.data.EndDate).get('D') - dayjs(e.data.StartDate).get('D')}</div>
      )
    },
    {
      label: 'Head Count',
      name: 'HeadCount',
      alignment: 'left',
      // cellRender:(e) => (
      //   <div>{dayjs(e.data.EndDate).get('D') - dayjs(e.data.StartDate).get('D')}</div>
      // )
    },
    // {
    //   label: 'Status',
    //   name: 'Status',
    //   width: '110px',
    //   alignment: 'start',
    //   cellRender: (e) => (
    //     <Tag color={e.value === 'Confirm' ? 'success' : 'orange'} className='text-[12px]'>{e.value}</Tag>
    //   )
    // },
    {
      label: '',
      name: '',
      width: '200px',
      alignment: 'start',
      cellRender: (e) => (
        <div className='flex items-center gap-2'>
          <Link to={`/tas/seatblock/${e.data.Id}`}>
            <button type='button' className='edit-button'>View</button>
          </Link>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    setActionLoading(true)
    if(editData){
      axios({
        method: 'put',
        url:'tas/visitevent',
        data: {
          ...values,
          StartDate: dayjs(values.StartDate).format('YYYY-MM-DD'),
          EndDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
          Id: editData.Id
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
        
      }).then(() => setActionLoading(false))
    }
    else{
      axios({
        method: 'post',
        url:'tas/visitevent',
        data: {
          ...values,
          HeadCount: values.people.length,
          StartDate: dayjs(values.StartDate).format('YYYY-MM-DD'),
          EndDate: dayjs(values.EndDate).format('YYYY-MM-DD'),
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/visitevent`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSearch = async (values) => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/visitevent?startDate=${values.StartDate ? dayjs(values.StartDate).format('YYYY-MM-DD') : ''}&endDate=${values.EndDate ? dayjs(values.EndDate).format('YYYY-MM-DD') : ''}&name=${values.name ? values.name : ''}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getStartFlights = (date, direction) => {
    axios({
      method: 'get',
      url: `tas/ActiveTransport/getdatetransport?eventDate=${date}&Direction=${direction}`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({ value: item.ScheduleId, label: `${item.Description} ${item.Seat}/${item.BookedCount}`})
      })
      setStartFlights(tmp)
    })
  }
  
  const getEndFlights = (date, direction) => {
    axios({
      method: 'get',
      url: `tas/ActiveTransport/getdatetransport?eventDate=${date}&Direction=${direction}`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({ value: item.ScheduleId, label: `${item.Description} ${item.Seat}/${item.BookedCount}`})
      })
      setEndFlights(tmp)
    })
  }

  const handleAddNewPerson = () => {
    if(form.getFieldValue('PersonCount') < 3000){
      form.setFieldValue('PersonCount', form.getFieldValue('PersonCount')+1)
      form.setFieldValue('PrevCount', form.getFieldValue('PersonCount'))
    }
  }

  const handleRemoveNewPerson = () => {
    form.setFieldValue('PersonCount', form.getFieldValue('PersonCount')-1)
    form.setFieldValue('PrevCount', form.getFieldValue('PersonCount'))
  }

  const fields = [
    {
      label: 'Name',
      name: 'Name',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Code is required'}], 
      inputprops: {
        // maxLength: 10
      }
    },
    {
      type: 'component',
      component: <>
      <Form.Item label={<span><span className='text-red-500'>*</span> Start Schedule</span>} className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item name={'StartDate'} className='mb-0 col-span-5'>
            <DatePicker className='w-full' onChange={(date, string) => {getStartFlights(string, 'IN'); form.setFieldValue('InScheduleId', null)}}/>
          </Form.Item>
          <Form.Item name={'InScheduleId'} className='mb-0 col-span-7' rules={[{required: true, message: 'Flight is required'}]}>
            <Select
              options={startFlights}
              className='w-full'
              popupMatchSelectWidth={false}
              allowClear
              showSearch
            />
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label={<span><span className='text-red-500'>*</span> End Schedule</span>} className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item name={'EndDate'} className='mb-0 col-span-5'>
            <DatePicker className='w-full' onChange={(date, string) => {getEndFlights(string, 'OUT'); form.setFieldValue('OutScheduleId', null)}}/>
          </Form.Item>
          <Form.Item name={'OutScheduleId'} className='mb-0 col-span-7' rules={[{required: true, message: 'Flight is required'}]}>
            <Select
              options={endFlights}
              className='w-full'
              popupMatchSelectWidth={false}
              allowClear
              showSearch
              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            />
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    // {
    //   label: 'Head Count',
    //   name: 'HeadCount',
    //   className: 'col-span-12 mb-2',
    //   rules: [{required: true, message: 'Head Count is required'}], 
    //   type: 'number',
    //   inputprops: {
    //     min: 0,
    //   }
    // },
    {
      type: 'component',
      component: <>
      <Form.Item label='Head Count' className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item name={'PrevCount'} className='mb-0 col-span-5'>
            <InputNumber className='w-full' max={3000}/>
          </Form.Item>
          <Form.Item noStyle shouldUpdate={(prev, cur) => prev.PrevCount !== cur.PrevCount}>
            {({getFieldValue}) => {
              return(
                <Form.Item className='mb-0 col-span-5'>
                  <Button 
                    onClick={() => form.setFieldValue('PersonCount', form.getFieldValue('PrevCount'))}
                    disabled={getFieldValue('PrevCount') === 0 || !getFieldValue('PrevCount')}
                  >
                    Generate
                  </Button>
                </Form.Item>
              )
            }}
          </Form.Item>
          <Form.Item name='PersonCount'>

          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
        <Form.Item noStyle shouldUpdate={(per, cur) => per.PersonCount !== cur.PersonCount}>
          {({ getFieldValue, setFieldValue }) => {
            if(getFieldValue('PersonCount')){
              let tmp = []
              for (let index = 0; index < getFieldValue('PersonCount'); index++) {
                tmp.push({
                  last: `L-${dayjs().format('YYMMDDHHmm')}-${index+1}`,
                  first: `F-${dayjs().format('YYMMDDHHmm')}-${index+1}`,
                })
              }
              setFieldValue('people', tmp)
            }
            return (
              <Form.List name="people" className='col-span-12'>
                {(fields, { add, remove }) => (
                  <>
                    {
                      fields.length > 0 ?
                      <div className='col-span-12 grid grid-cols-12 gap-x-2 gap-y-2 items-center max-h-[350px] overflow-auto mb-3 p-2 border rounded-ot'>
                        {fields.map(({ key, name, ...restField }) => (
                          <>
                            <Form.Item
                              {...restField}
                              className='col-span-4 mb-0'
                              name={[name, 'first']}
                              // rules={[{ required: true, message: 'Missing first name' },]}
                            >
                              <Input placeholder="Firstname" />
                            </Form.Item>
                            <Form.Item
                              {...restField}
                              className='col-span-4 mb-0'
                              name={[name, 'last']}
                              // rules={[{ required: true, message: 'Missing last name',},]}
                            >
                              <Input placeholder="Lastname" />
                            </Form.Item>
                            <Form.Item
                              noStyle
                              className='col-span-4 mb-0'
                              name={[name, 'Id']}
                              // rules={[{ required: true, message: 'Missing last name',},]}
                            >
                            </Form.Item>
                            <button 
                              type='button'
                              className='bg-[#FFE2E5] text-[#F64E60] hover:bg-red-200 rounded-md py-1 px-3 disabled:bg-gray-100  transition-all' 
                              onClick={handleRemoveNewPerson}
                            >
                              <MinusCircleOutlined />
                            </button>
                          </>
                        ))}
                      </div>
                      : null
                    }
                    <Form.Item className='col-span-12 mb-5'>
                      <Button 
                        className='w-full flex justify-center'
                        onClick={handleAddNewPerson}
                        block
                        disabled={getFieldValue('PersonCount') >= 3000}
                        icon={<PlusOutlined />}>
                        Add new person
                      </Button>
                    </Form.Item>
                  </>
                )}
              </Form.List>
            );
          }}
          
        </Form.Item>
      </>
    },
  ]

  const editFields = [
    {
      label: 'Name',
      name: 'Name',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Code is required'}], 
      inputprops: {
        // maxLength: 10
      }
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='Start Date' className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item name={'StartDate'} className='mb-0 col-span-5'>
            <DatePicker className='w-full' onChange={(date, string) => {getStartFlights(string, 'IN'); form.setFieldValue('InScheduleId', null)}}/>
          </Form.Item>
          <Form.Item name={'InScheduleId'} className='mb-0 col-span-7' rules={[{required: true, message: 'Flight is required'}]}>
            <Select
              options={startFlights}
              className='w-full'
              popupMatchSelectWidth={false}
              allowClear
              showSearch
              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            />
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      type: 'component',
      component: <>
      <Form.Item label='End Date' className='col-span-12 mb-2'>
        <div className='grid grid-cols-12 gap-2'>
          <Form.Item name={'EndDate'} className='mb-0 col-span-5'>
            <DatePicker className='w-full' onChange={(date, string) => {getEndFlights(string, 'OUT'); form.setFieldValue('OutScheduleId', null)}}/>
          </Form.Item>
          <Form.Item name={'OutScheduleId'} className='mb-0 col-span-7' rules={[{required: true, message: 'Flight is required'}]}>
            <Select
              options={endFlights}
              className='w-full'
              popupMatchSelectWidth={false}
              allowClear
              showSearch
              filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
            />
          </Form.Item>
        </div>
      </Form.Item>
      </>
    },
    {
      label: 'Head Count',
      name: 'HeadCount',
      className: 'col-span-12 mb-2',
      rules: [{required: true, message: 'Head Count is required'}], 
      type: 'number',
      inputprops: {
        min: 1,
        max: 100,
      }
    },
  ]

  const searchFields = [
    {
      label: 'Name',
      name: 'Name',
      className: 'col-span-3 mb-2',
    },
    {
      label: 'Start Date',
      name: 'StartDate',
      type: 'date',
      className: 'col-span-3 2xl:col-span-2 mb-2',
      // rules: [{required: true, message: 'Field is required'}]
    },
    {
      label: 'End Date',
      name: 'EndDate',
      type: 'date',
      className: 'col-span-3 2xl:col-span-2 mb-2',
      // rules: [{required: true, message: 'Field is required'}]
    },
  ]

  const handleCancel = () => {
    setShowModal(false)
  }

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <Form 
          form={searchForm}
          fields={searchFields}
          className='flex gap-x-8' 
          onFinish={handleSearch}
          noLayoutConfig={true}
        >
          <div className='flex gap-4 items-baseline col-span-1'>
            <Button 
              htmlType='submit'
              className='flex items-center' 
              loading={loading} 
              icon={<SearchOutlined/>}
            >
              Search
            </Button>
          </div>
        </Form>
      </div>
      <Table
        data={data}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        onRowDblClick={(e) => navigate(`/tas/seatblock/${e.data.Id}`)}
        pager={data > 20}
        tableClass='border-t max-h-[calc(100vh-282px)]'
        title={<div className='flex justify-between py-2 px-1'>
          <div className='flex items-center gap-1'>
            <span className='font-bold'>{data.length}</span>
            <span className=' text-gray-400'>results</span>
          </div>
          <Button icon={<PlusOutlined/>} onClick={handleAddButton}>Add {title}</Button>
        </div>}
      />
      <Modal 
        open={showModal}
        onCancel={() => setShowModal(false)}
        title={editData ? `Edit ${title}` : `Add ${title}`}
        width={700}
      >
        <Form 
          form={form}
          fields={editData ? editFields : fields}
          initValues={{people: []}}
          onFinish={handleSubmit}
          labelCol={{ xs: { span: 24 }, sm: { span: 4 } }}
        >
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button type='primary' onClick={() => form.submit()} loading={actionLoading} icon={<SaveOutlined/>} disabled={values?.people?.length === 0}>Save</Button>
            <Button onClick={handleCancel} disabled={actionLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to {editData?.Active === 1 ? 'deactivate' : 'reactivate'} this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={editData?.Active === 1 ? 'danger' : 'success'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default SeatBlock