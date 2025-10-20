import { DeleteOutlined, EditOutlined, EyeOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, ItineraryOptionCard, Loading, Modal, Skeleton, Timer, Tooltip } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { twMerge } from 'tailwind-merge'
import OptionDetail from '../OptionDetail'
import AddOptionForm from './AddOptionForm'
import EditOptionForm from './EditOptionForm'
import { AuthContext } from 'contexts'
import { Popup } from 'devextreme-react'

function ProcessingItinerary({hasActionPermission, currentGroup, isEditable, getData, documentData, isDeletable}) {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(true)
  const [ editOptionData, setEditOptionData ] = useState(null)
  const [ selectedOption, setSelectedOption ] = useState(null)
  const [ selectedViewOption, setSelectedViewOption ] = useState(null)
  const [ openDrawer, setOpenDrawer ] = useState(null)
  const [ showModal, setShowModal ] = useState(false)
  const [ isChangedoptionvalue, setIsChangedoptionvalue ] = useState(false)
  const [ maxOptionIndex, setMaxOptionIndex ] = useState(0)
  const [ showPopup, setShowPopup ] = useState(false)
  const { documentId } = useParams()
  const [ actionLoading, setActionLoading ] = useState(false)

  const [ form ] = Form.useForm()
  const { action, state } = useContext(AuthContext)


  useEffect(() => {
    getOptionsData()
  },[])

  const getOptionsData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/requestnonsitetraveloption/${documentId}`,
    }).then((res) => {
      setData(res.data)
      let indexes = res.data.map((item) => item.OptionIndex);
      let maxIndex = Math.max.apply(null, indexes)
      setMaxOptionIndex(maxIndex)
      action.setTravelOptions(res.data)
    }).catch((err) => {

    }).finally(() => {
      setLoading(false)
    })
  }

  const handleSelectOption = (item, dueDate) => {
    if(currentGroup?.GroupTag === "linemanager" || (currentGroup?.GroupTag === "requester" && currentGroup?.OrderIndex === 2) && (dayjs(dueDate).diff(dayjs())) > 0){
      if(hasActionPermission){
        setSelectedOption(item.Id)
        setIsChangedoptionvalue(true)
      }
    }
  }

  const handleEditOption = (data) => {
    setEditOptionData({...data, DueDate: data.DueDate ? dayjs(data.DueDate) : null})
    form.setFieldsValue({...data, DueDate: data.DueDate ? dayjs(data.DueDate) : null})
    setShowModal(true)
  }

  const handleSaveFinalItinerary = () => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestnonsitetraveloption',
      data: {
        selected: 1,
        id: selectedOption,
      }
    }).then((res) => {
      getOptionsData()
      setIsChangedoptionvalue(false)
      getData()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleAddButton = () => {
    setEditOptionData(null)
    setShowModal(true)
  }

  const handleCancelButton = (e) => {
    e.stopPropagation()
    setIsChangedoptionvalue(false)
    setSelectedOption(null)
  }

  const onClickViewBtn = (item) => {
    setSelectedViewOption(item);
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
      getOptionsData()
      setShowPopup(false)
    }).catch(() => {

    }).finally(() => {
      setActionLoading(false)
    })
  }

  return (
    <>
      <div className='p-2 rounded-bl-ot rounded-br-ot border border-gray-300'>
        {
          loading ?
          <div className='grid grid-cols-4 gap-4'>
            <Skeleton className='h-[200px]'/>
            <Skeleton className='h-[200px]'/>
            <Skeleton className='h-[200px]'/>
          </div>
          : 
          <>
            {
              currentGroup?.GroupTag === "travelflight" && currentGroup?.OrderIndex === 1 ?
              <div className='flex justify-between items-center gap-2 mb-4 pl-2'>
                <div className='font-bold'>Options <span className='font-normal'>({data.length})</span></div>
                <Button 
                  htmlType='button' 
                  type='primary' 
                  onClick={handleAddButton} 
                  className='text-xs'
                >
                  Add Options
                </Button>
              </div> 
              : null
            }
            <div className='overflow-x-auto'>
              <div className='flex items-start gap-4 pb-2'>
                {
                  data.map((item, i) => (
                    <ItineraryOptionCard
                      key={`itinerary-option-${i}`}
                      data={item}
                      selectedOption={selectedOption}
                      onClickDeleteBtn={onClickDeleteBtn}
                      onClickEditBtn={handleEditOption}
                      onClickViewBtn={onClickViewBtn}
                      onSelect={handleSelectOption}
                      isDeletable={isDeletable}
                      isEditable={isEditable}
                    />
                  ))
                }
              </div>
            </div>
          </>
        }
        {
          isChangedoptionvalue ? 
          <div className='flex justify-end gap-2 mt-3'>
            <Button type='primary' className='text-xs' onClick={handleSaveFinalItinerary}>Save</Button>
            <Button type='danger' className='text-xs' onClick={handleCancelButton}>Cancel</Button>
          </div>
          :
          null
        }
      </div>
      <OptionDetail
        open={openDrawer}
        onClose={() => setOpenDrawer(false)}
        data={selectedViewOption}
      />
      <Modal 
        width={700}
        title={editOptionData ? 'Edit Option' : 'Add Options'}
        open={showModal}
        onCancel={() => setShowModal(false)}
      >
        {
          editOptionData ?
          <EditOptionForm
            editData={editOptionData}
            getOptionsData={getOptionsData}
            onCancel={() => {setShowModal(false); setEditOptionData(null);}}
          />
          :
          <AddOptionForm maxOptionIndex={maxOptionIndex} onCancel={() => setShowModal(false)} getOptionsData={getOptionsData} />
        }
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to delete this option?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
        </div>
      </Popup>
    </>
  )
}

export default ProcessingItinerary