import { DatePicker } from 'antd'
import axios from 'axios'
import { Button, Form, Table } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { twMerge } from 'tailwind-merge'
import ls from 'utils/ls'


function Audit() {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ auditData, setAuditData ] = useState([])
  const [ form ] = Form.useForm()
  
  const { employeeId } = useParams()
  const { state } = useContext(AuthContext)
  
  useEffect(() => {
    ls.set('pp_rt', 'audit')
    getData()
  },[])

  const getData = () => {
    setLoading(true)
    let values = form.getFieldsValue()
    axios({
      method: 'get',
      url: `tas/audit/profileaudit/${employeeId}?${values.StartDate ? `startdate=${dayjs(values.StartDate).format('YYYY-MM-DD')}` : ''}${values.EndDate ? `${values.StartDate ? '&' : ''}enddate=${dayjs(values.EndDate).format('YYYY-MM-DD')}` : ''}`
    }).then((res) => {
      setData(res.data)
      setAuditData(res.data.AuditData)
    }).catch((err) => {
      
    }).then(() => setLoading(false))
  }

  const renderValue = (value, key) => {
    switch (key) {
      case 'Dob': return value ? dayjs(value).format('YYYY-MM-DD') : ''
      case 'CommenceDate': return value ? dayjs(value).format('YYYY-MM-DD') : ''
      case 'Gender': return value ? 'Male' : 'Female'
      case 'EmployerId': return state.referData.employers?.find((item) => value === item.Id)?.label
      case 'LocationId': return state.referData.locations?.find((item) => value === item.Id)?.label
      case 'NationalityId': return state.referData.nationalities?.find((item) => value === item.Id)?.label
      case 'PeopleTypeId': return state.referData.peopleTypes?.find((item) => value === item.Id)?.label
      case 'PositionId': return state.referData.positions?.find((item) => value === item.Id)?.label
      case 'RosterId': return state.referData.rosters?.find((item) => value === item.Id)?.label
      case 'StateId': return state.referData.states?.find((item) => value === item.Id)?.label
      case 'CostCodeId': return state.referData?.costCodes.find((item) => value === item.Id)?.label
      case 'DepartmentId': return state.referData.departmentsmini?.find((item) => value === item.Id)?.Name
      default: return value
    }
  }

  const column = [
    {
      label: 'Changed User',
      name: 'Username',
    },
    {
      label: 'Changed Date',
      name: 'DateCreated',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
  ]

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      <div className='flex justify-between mb-3'>
        <div>
          <div className='text-lg font-bold'>Profile Audit</div>
          <div className='text-sm flex gap-4 text-secondary2'>
            Profile created on {dayjs(data?.CreatedDate).format('YYYY-MM-DD')} { data?.CreatedFullName ? `by ${<Link to={`/tas/people/search/${data?.CreatedUserId}`}><span>{data?.CreatedFullName}</span></Link>}` : null} 
          </div>
        </div>
        <Form
          className='flex items-center gap-2'
          form={form}
          onFinish={getData}
          initValues={{StartDate: dayjs().subtract(6, 'months'), EndDate: dayjs()}}
        >
          <Form.Item name='StartDate' className='mb-0'>
            <DatePicker placeholder='Start Date'/>
          </Form.Item>
          -
          <Form.Item name='EndDate' className='mb-0'>
            <DatePicker placeholder='End Date'/>
          </Form.Item>
          <Button htmlType='button' disabled={loading} onClick={() => form.submit()}>Search</Button>
        </Form>
      </div>
      <Table
        data={auditData}
        columns={column}
        allowColumnReordering={false}
        loading={loading}
        containerClass='shadow-none border'
        keyExpr='Id'
        tableClass='max-h-[calc(100vh-400px)]'
        pager={{showPageSizeSelector: data.length > 100}}
        renderDetail={{
          enabled: true,
          component: ({data}) => {
            const oldData = JSON.parse(data.data.OldValues)
            const newData = JSON.parse(data.data.NewValues)
            let keys = []
            if(newData){
              keys = Object.keys(newData)
            }
            return (
              <div className='bg-white px-3'>
                <table className='border'>
                  <thead>
                    <tr>
                      <th className='p-1 px-2 border'>Field</th>
                      <th className='p-1 px-2 border'>Previous value</th>
                      <th className='p-1 px-2 border'>New value</th>
                    </tr>
                  </thead>
                  <tbody>
                    {
                      keys.map((key, i) => (
                        <tr className={twMerge('hover:bg-blue-100 transition-all', i%2 === 0 ? 'bg-gray-100' :'')}>
                          <td className='p-1 px-2 border'>{key}</td>
                          <td className='p-1 px-2 border'>{renderValue(oldData[key], key)}</td>
                          <td className='p-1 px-2 border'>{renderValue(newData[key], key)}</td>
                        </tr>
                      ))
                    }
                  </tbody>
                </table>
              </div>
            )
          }
        }}
      />
    </div>
  )
}

export default Audit