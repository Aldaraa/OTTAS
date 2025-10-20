import { CheckCircleOutlined, EditOutlined, EyeOutlined, PlusCircleOutlined, SaveOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Modal, Tooltip } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { twMerge } from 'tailwind-merge'
import OptionDetail from '../OptionDetail'
import { AuthContext } from 'contexts'
import { Input, InputNumber, Tag } from 'antd'

function FinalItinerary({ flightData, data, getFinalOptionsData}) {
  // const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ openDrawer, setOpenDrawer ] = useState(false)
  const [ viewOptionData, setViewOptionData ] = useState(null)
  const [ editOptionData, setEditOptionData ] = useState(null)
  const [ showEditModal, setShowEditModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)

  const { documentId } = useParams()
  const [ editForm ] = Form.useForm()
  const { state } = useContext(AuthContext)

  useEffect(() => {
    getFinalOptionsData()
  },[])

  // const getFinalOptionsData = () => {
  //   setLoading(true)
  //   axios({
  //     method: 'get',
  //     url: `tas/requestnonsitetraveloption/final/${documentId}`,
  //   }).then((res) => {
  //     setData(res.data)
  //   }).catch((err) => {

  //   }).finally(() => {
  //     setLoading(false)
  //   })
  // }

  const handleEditOption = (data) => {
    setEditOptionData({...data, DueDate: data.DueDate ? dayjs(data.DueDate) : null})
    editForm.setFieldsValue({...data, DueDate: data.DueDate ? dayjs(data.DueDate) : null})
    setShowEditModal(true)
  }

  const handleSubmitEditOption = (values) => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestnonsitetraveloption/fulldata',
      data: {
        ...values,
        id: editOptionData.Id,
        dueDate: dayjs(values.DueDate).format('YYYY-MM-DD HH:mm'),
      }
    }).then((res) => {
      editForm.resetFields()
      setShowEditModal(false)
      getFinalOptionsData()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleCancelEditOption = () => {
    editForm.resetFields()
    setShowEditModal(false)
  }

  return (
    <div className='p-2 rounded-bl-ot rounded-br-ot border border-gray-300 flex divide-x gap-3'>
      {
        state.userInfo?.Role === 'TravelAdmin' ?
        <>
          <div className='flex-1 pr-5 overflow-x-auto'>
            <div className='flex items-start gap-4'>
              {
                data.map((item, i) => (
                  <div 
                    key={i}
                    className={twMerge(`flex flex-col gap-4 group cursor-pointer transition-all`)}
                  >
                    <div className='flex flex-col relative rounded-ot border overflow-hidden'> 
                      <div className={twMerge('flex justify-between items-center p-2 border-b', item.Status === 'ISSUED' ? 'bg-green-50' : 'bg-orange-50')}>
                        <Tooltip title={`${item.SelectedUserName} ${item.SelectedUserTeam}`}>
                          <div className='flex gap-1 items-center'>
                            <div className='text-base'>{item.Status === 'ISSUED' ? <CheckCircleOutlined className='text-green-700'/> : <PlusCircleOutlined className='text-primary'/>}</div>
                          </div>
                        </Tooltip>
                        <div className='text-right text-[12px]'>
                          <span className=''>{Intl.NumberFormat().format(item.Cost)} ₮</span>
                        </div>
                      </div>
                      <div className='absolute bottom-3 right-3 flex gap-1 items-center opacity-0 group-hover:opacity-100'>
                        <Tooltip title='View Detail'>
                          <Button
                            className={`px-2 shadow-option1`}
                            onClick={() => { setOpenDrawer(true); setViewOptionData(item);}}
                            icon={<EyeOutlined/>}
                          />
                        </Tooltip>
                        <Tooltip title='Edit'>
                          <Button 
                            className={`px-2 shadow-option1`}
                            onClick={() => handleEditOption(item)}
                            icon={<EditOutlined/>}
                          />
                        </Tooltip>
                      </div>
                      <pre className={'p-2 text-left text-gray-500 w-full overflow-ellipsis overflow-hidden'} style={{fontSize: 11}}>
                        {item.OptionData}
                      </pre>
                      {
                        item.Comment ?
                        <div className='px-2 pb-2'>
                          <Tag>{item.Comment}</Tag>
                        </div>
                        : null
                      }
                    </div>
                  </div>
                ))
              }
            </div>
          </div>
        </>
        : null
      }
      <OptionDetail
        open={openDrawer}
        onClose={() => setOpenDrawer(false)}
        data={viewOptionData}
      />
      <Modal 
        open={showEditModal} 
        onCancel={() => setShowEditModal(false)} 
        title='Edit Option' 
      >
        <Form
          form={editForm}
          editData={editOptionData}
          size='small'
          disabled={false}
          className='grid grid-cols-12 gap-x-4 gap-y-3'
          onFinish={handleSubmitEditOption}
          layout='vertical'
        >
          <Form.Item label='Cost' name={'Cost'} className='col-span-6 mb-0'>
            <InputNumber className='w-full' controls={false} formatter={value => `${new Intl.NumberFormat().format(value)}`}/>
          </Form.Item>
          <Form.Item label='Itinerary' name={'OptionData'} className='col-span-12 mb-2'>
            <Input.TextArea
              autoSize={{minRows: 3, maxRows: 10}}
            />
          </Form.Item>
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='submit' loading={actionLoading} type={'primary'} icon={<SaveOutlined/>}>Save</Button>
            <Button htmlType='button' onClick={handleCancelEditOption}>Cancel</Button>
          </div>
        </Form>
      </Modal>
    </div>
  )
}

export default FinalItinerary