import { LeftOutlined } from '@ant-design/icons'
import { Form, Input } from 'antd'
import { Button, Modal, PeopleSearch, Table } from 'components'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import axios from 'axios'

function BulkReplace({data=[], handleChangeData, changeIsEditing, className, refreshData}) {
  const [ currentRow, setCurrentRow ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ showResponse, setShowResponse ] = useState(false)
  const [ replaceResponse, setReplaceResponse ] = useState([])
  const [ form ] = Form.useForm()
  const { eventId } = useParams()
  const navigate = useNavigate()

  useEffect(() => {
    form.setFieldsValue({rosters: data})
  },[data])
  
  const handleBack = () => {
    handleChangeData(form.getFieldValue('rosters'))
    changeIsEditing(false)
  }

  const handleRowSelect = (row) => {
    setCurrentRow(row)
    setShowModal(true)
  }

  const selectEmployee = (e) => {
    form.setFieldValue(['rosters', currentRow, 'EmployeeName'], `${e.Firstname} ${e.Lastname}`)
    form.setFieldValue(['rosters', currentRow, 'NewEmployeeId'], e.Id)
    setShowModal(false)
  }

  const handleSubmit = (values) => {
    setLoading(true)
    let tmp = []
    values?.rosters.map((item) => {
      tmp.push({oldEmployeeId: item.EmployeeId, newEmployeeId: item.NewEmployeeId})
    })
    axios({
      method: 'post',
      url: 'tas/visitevent/replaceprofilemultiple',
      data: {
        employees: tmp,
        eventId: eventId,
      }
    }).then((res) => {
      if(res.data && res.data?.length > 0){
        setReplaceResponse(res.data)
        setShowResponse(true)
      }else{
        refreshData()
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCancel = () => {
    refreshData()
    handleBack()
    setShowResponse(false)
  }

  return (
    <div className={`rounded-ot bg-white p-5 mb-5 shadow-md ${className}`}>
      <Button className='mb-3' onClick={handleBack} icon={<LeftOutlined/>}>Back</Button>
      <Form 
        form={form} 
        size='small'
        onFinish={handleSubmit}
        initialValues={{
          rosters: [], 
          startDate: null, 
          durationMonth: 0, 
          flightGroupMasterId: null
        }}
      >
        <Form.List name='rosters'>
          {(fields, {remove}) => (
            <div className='border rounded-ot overflow-auto'>
              <table className='table-auto overflow-scroll w-full'>
                <thead className='text-[#959595]'> 
                  <tr className='text-left font-normal'>
                    <th className='border-b px-1 sticky left-0 z-10 bg-white'>#</th>
                    <th className='border-b px-1 sticky left-[38px] z-10 bg-white'>Fullname</th>
                    <th className='border-b px-1'>
                      Replacement
                    </th>
                    <th className='border-b px-1'>In Event Date</th>
                    <th className='border-b px-1'>In</th>
                    <th className='border-b px-1'>Out Event Date</th>
                    <th className='border-b px-1'>Out</th>
                    <th className='border-b px-1'></th>
                  </tr>
                </thead>
                <tbody>
                  {
                    fields.map(({key, name, ...restField}) => (
                      <tr>
                        <td className={`p-1 border-t sticky left-0 z-10 ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <div className='w-[30px]'>{name+1}</div>
                        </td>
                        <td className={`p-1 border-t sticky left-[38px] z-10 ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                            <Link to={`/tas/people/search/${form.getFieldValue(['rosters', name, 'Id'])}`}>
                              <span className='text-blue-500 hover:underline'>{form.getFieldValue(['rosters', name, 'Firstname'])} {form.getFieldValue(['rosters', name, 'Lastname'])}</span>
                            </Link>
                        </td>
                        <td className={`p-1 border-t flex items-start gap-2 ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <Form.Item
                            {...restField}
                            name={[name, 'NewEmployeeId']}
                            noStyle
                            // rules={[
                            //   {
                            //     required: true,
                            //     message: 'Replace employee is required',
                            //   },
                            // ]}
                          >
                          </Form.Item>
                          <Form.Item
                            {...restField}
                            name={[name, 'EmployeeName']}
                            className='mb-0 flex-1'
                            rules={[
                              {
                                required: true,
                                message: 'Field is required',
                              },
                            ]}
                          >
                            <Input readOnly placeholder='Replace employee'/>
                          </Form.Item>
                          <Button type='primary' className='py-1 text-xs' onClick={() => handleRowSelect(name)}>Select</Button>
                        </td>
                        <td className={`p-1 border-t ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <div className='w-[180px] text-[13px]'>{dayjs(form.getFieldValue(['rosters', name, 'InEventDate'])).format('YYYY-MM-DD')}</div>
                        </td>
                        <td className={`p-1 border-t ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <div className='w-[150px] text-[13px]'>{form.getFieldValue(['rosters', name, 'InDescr'])}</div>
                        </td>
                        <td className={`p-1 border-t ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <div className='w-[180px] text-[13px]'>{dayjs(form.getFieldValue(['rosters', name, 'OutEventDate'])).format('YYYY-MM-DD')}</div>
                        </td>
                        <td className={`p-1 border-t ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <div className='w-[150px] text-[13px]'>{form.getFieldValue(['rosters', name, 'OutDescr'])}</div>
                        </td>
                        <td className={`p-1 border-t ${(name+1)%2 === 0 ? 'bg-[#fafafa]' : 'bg-white'}`}>
                          <button type='button' className='dlt-button text-xs' onClick={() => remove(name)}>Remove</button>
                        </td>
                      </tr>
                    ))
                  }
                </tbody>
              </table>
            </div>
          )}
        </Form.List>
        <div className='col-span-12 flex gap-5 mt-8'>
          <Form.Item className='m-0'>
            <Button 
              type='primary' 
              onClick={() => form.submit()}
              loading={loading}
            >
              Submit
            </Button>
          </Form.Item>
        </div>
      </Form>
      <Modal title='Replace Employee' open={showModal} onCancel={() => setShowModal(false)} width={1000}>
        <PeopleSearch onSelect={selectEmployee} onRowDblClick={(e) => navigate(`/tas/people/search/${e.data.Id}`)}/>
      </Modal>
      {/* <Modal 
        open={showResultModal} 
        onCancel={() => {setShowResultModal(false);}} 
        title='Roster response'
        width={700}
      >
        <Tabs items={items} type='card'/>
      </Modal> */}
      <Modal title='Skipped Employees' open={showResponse} onCancel={handleCancel}>
        <Table
          containerClass='shadow-none border overflow-hidden px-0'
          data={replaceResponse}
          columns={[
            {
              label: 'Fullname',
              name: 'Firstname',
              alignment: 'left',
              cellRender: (e) => (
                <div>{e.value} {e.data.Lastname}</div>
              )
            },
            {
              label: '',
              name: 'Reason',
              alignment: 'left',
              cellRender: (e) => (
                <div className='text-red-400'>{e.value}</div>
              )
            },
          ]}
        />
      </Modal>
    </div>
  )
}

export default BulkReplace