import { CheckCircleOutlined, DeleteOutlined, EditOutlined, EyeOutlined, PlusCircleOutlined, SaveOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Modal, Tooltip } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import { twMerge } from 'tailwind-merge'
import OptionDetail from '../OptionDetail'
import { AuthContext } from 'contexts'
import { Input, InputNumber, Tag } from 'antd'
import { Popup } from 'devextreme-react'

const FinalOption = ({data, onClickViewBtn, onClickEditBtn, onClickDeleteBtn, isEditable}) => {
  return(
    <div className={twMerge(`flex flex-col gap-4 group cursor-pointer transition-all`)}>
      <div className='flex flex-col relative rounded-ot border overflow-hidden'> 
        <div className={twMerge('flex justify-between items-center p-2 border-b', data.Status === 'ISSUED' ? 'bg-green-50' : 'bg-orange-50')}>
          <Tooltip title={`${data.SelectedUserName} ${data.SelectedUserTeam}`}>
            <div className='flex gap-1 items-center'>
              <div className='text-base'>{data.Status === 'ISSUED' ? <CheckCircleOutlined className='text-green-700'/> : <PlusCircleOutlined className='text-primary'/>}</div>
              <div>Option #{data.OptionIndex}</div>
            </div>
          </Tooltip>
          <div className='text-right text-[12px]'>
            <span className=''>{Intl.NumberFormat().format(data.Cost)} ₮</span>
          </div>
        </div>
          <div className='absolute bottom-3 right-3 flex gap-1 items-center opacity-0 group-hover:opacity-100'>
            <Tooltip title='View Detail'>
              <Button
                className={`px-2 shadow-option1`}
                onClick={() => onClickViewBtn(data)}
                icon={<EyeOutlined/>}
                />
            </Tooltip>
            {
              isEditable ?
              <>
                <Tooltip title='Edit'>
                  <Button 
                    className={`px-2 shadow-option1`}
                    onClick={() => onClickEditBtn(data)}
                    icon={<EditOutlined/>}
                  />
                </Tooltip>
                <Tooltip title='Delete Option'>
                  <Button 
                    type='danger'
                    className={`px-2 shadow-option1`}
                    onClick={() => onClickDeleteBtn(data)}
                    icon={<DeleteOutlined/>}
                  />
                </Tooltip>
              </>
                : null
              }
          </div>
        <pre className={'p-2 text-left text-gray-500 w-full overflow-ellipsis overflow-hidden'} style={{fontSize: 11}}>
          {data.OptionData}
        </pre>
        {
          data.Comment ?
          <div className='px-2 pb-2'>
            <Tag>{data.Comment}</Tag>
          </div>
          : null
        }
      </div>
    </div>
  )
}

function FinalItinerary({ flightData, data, getFinalOptionsData, isDeletable}) {
  const [ loading, setLoading ] = useState(false)
  const [ openDrawer, setOpenDrawer ] = useState(false)
  const [ viewOptionData, setViewOptionData ] = useState(null)
  const [ editOptionData, setEditOptionData ] = useState(null)
  const [ showEditModal, setShowEditModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ editForm ] = Form.useForm()
  const { state } = useContext(AuthContext)

  useEffect(() => {
    getFinalOptionsData()
  },[])

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

  const onClickViewBtn = (option) => {
    setViewOptionData(option);
    setOpenDrawer(true); 
  }

  const onClickDeleteBtn = (option) => {
    setEditOptionData(option)
    setShowPopup(true)
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/requestnonsitetraveloption/${editOptionData?.Id}`,
    }).then((res) => {
      getFinalOptionsData()
      setShowPopup(false)
    }).catch(() => {

    }).finally(() => {
      setActionLoading(false)
    })
  }

  const hasPermission = (state.userInfo?.Role === 'TravelAdmin' || state.userInfo?.Role === 'SystemAdmin')

  return (
    <div className='p-2 rounded-bl-ot rounded-br-ot border border-gray-300 flex divide-x gap-3'>
      {
        hasPermission ?
        <>
          <div className='flex-1 pr-5 overflow-x-auto'>
            <div className='flex items-start gap-4'>
              {
                data.map((item, i) => (
                  <FinalOption 
                    key={`final-option-${i}`}
                    data={item}
                    onClickViewBtn={onClickViewBtn}
                    onClickEditBtn={handleEditOption}
                    onClickDeleteBtn={onClickDeleteBtn}
                    isEditable={isDeletable}
                  />
                ))
              }
            </div>
          </div>
        </>
        : 
        <div>Travel Admin only can view</div>
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
      <Popup
        visible={showPopup}
        showTitle={false}
        height={'auto'}
        width={350}
      >
        <div>Are you sure you want to delete this option?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default FinalItinerary