import { DownloadOutlined, ExceptionOutlined, LoadingOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Form, Modal, Table, Button as MyButton, DepartmentTooltip } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import { Button } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { FaFemale, FaMale } from 'react-icons/fa'
import { Link } from 'react-router-dom'

function OnSitePeople({date, room}) {
  const [ loading, setLoading ] = useState(false)
  const [ data, setData ] = useState([])
  const [ showModal, setShowModal ] =  useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const { state } = useContext(AuthContext)
  const [ form ] = Form.useForm()

  useEffect(() => {
    if(date && room){
      getData()
    }
  },[date, room])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'post',
      url: `tas/room/detaildateprofile`,
      data: {
        currentDate: date,
        roomId: room?.Id
      }
    }).then((res) => {
      setData(res.data)
    }).finally(() => setLoading(false))
  }

  const roomEmpColumns = [
    {
      label: 'Fullname',
      name: 'FullName',
      cellRender:(e) => (
        <div className='flex gap-1'>
          <div className='flex'>
            {e.data.Gender === 1 ? <FaMale size={14} className='text-blue-600'/> : <FaFemale size={14} className='text-pink-500'/>} 
            {e.data.RoomOwner ? <i className="dx-icon-home text-green-500"></i> : <i className="dx-icon-home text-gray-400"></i>}
          </div>
          <Link to={`/tas/roomassign/${e.data?.EmployeeId}/roombooking`}>
            <span className='cursor-pointer text-blue-500 hover:underline ml-1 whitespace-normal'>{e.value}</span>
          </Link>
        </div>
      )
    },
    {
      label: 'Department',
      name: 'DepartmentName',
      cellRender: (e) => (
        <DepartmentTooltip showStatus={false} id={e.data?.DepartmentId}>
          <span className=' whitespace-normal'>{e.value}</span>
        </DepartmentTooltip>
      )
    },
    {
      label: 'People Type',
      name: 'PeopleTypeCode',
    },
    {
      label: 'Employer',
      name: 'EmployerName',
      cellRender: (e) => (
        <span className=' whitespace-normal'>{e.value}</span>
      )
    },
  ]

  const fields = [
    {
      label: 'Resource Type',
      name: 'PeopleTypeIds',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        mode: 'multiple',
        placeholder: 'Select resource type',
        options: state.referData.peopleTypes,
      }
    },
  ]

  const handleDownloadExcel = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      responseType: 'blob',
      url: `tas/room/dateprofileexport`,
      data: {
        ...values,
        currentDate: date,
        roomId: room?.Id,
      }
    }).then(async (res) => {
      const url = await window.URL.createObjectURL(new Blob([res.data]));
      const fn = `TAS_${room.Number}_${dayjs(date).format('YYYY-MM-DD')}.xlsx`
      const link = await document.createElement('a');
      link.href = url;
      await link.setAttribute('download', fn); //or any other extension
      await document.body.appendChild(link);
      await link.click();
      setShowModal(false)
    }).catch((err) => {
    }).then(() => {
      setActionLoading(false)
    })
  }

  return (
    <div className='flex flex-col'>
      {
        loading ? 
        <div className='flex-1 flex justify-center items-center'>{<LoadingOutlined/>}</div>
        :
        <Table
          data={data}
          columns={roomEmpColumns}
          pager={data > 20}
          keyExpr={'Id'}
          loading={loading}
          isSearch={true}
          containerClass='shadow-none border overflow-hidden mb-4'
          tableClass='border-t max-h-[calc(100vh-150px)]'
          title={<div className='py-1 font-bold'>On Site Employees ({data?.length })</div>}
          toolbar={[
            {
              location: 'after',
              render: (e) =>
                <div className='flex items-center gap-2' >
                  <Button icon='xlsfile' onClick={() => setShowModal(true)}/>
                </div>
            },

          ]}
        />
      }
      <Modal open={showModal} title={'Download Onsite Employees'} onCancel={() => setShowModal(false)}>
        <Form
          form={form}
          fields={fields}
          onFinish={handleDownloadExcel}
          layout='vertical'
        >
          <div className='col-span-12 flex justify-end gap-4 mt-4'>
            <MyButton
              icon={<DownloadOutlined/>}
              type='primary'
              htmlType='button'
              onClick={form.submit}
              loading={actionLoading}
            >
              Download
            </MyButton>
          </div>
        </Form>
      </Modal>
    </div>
  )
}

export default OnSitePeople