import { EditOutlined, EyeOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, Form, Modal, Timer, Tooltip } from 'components'
import dayjs from 'dayjs'
import React, { useContext, useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { twMerge } from 'tailwind-merge'
import OptionDetail from '../OptionDetail'
import AddOptionForm from './AddOptionForm'
import EditOptionForm from './EditOptionForm'
import { AuthContext } from 'contexts'

function ProcessingItinerary({hasActionPermission, currentGroup, isEditable, getData, documentData, userData}) {
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ editOptionData, setEditOptionData ] = useState(null)
  const [ selectedOption, setSelectedOption ] = useState(null)
  const [ selectedViewOption, setSelectedViewOption ] = useState(null)
  const [ openDrawer, setOpenDrawer ] = useState(null)
  const [ showModal, setShowModal ] = useState(false)
  const [ isChangedoptionvalue, setIsChangedoptionvalue ] = useState(false)
  const { documentId } = useParams()

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
    setLoading(true)
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

    }).then(() => setLoading(false))
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

  return (
    <div className='p-2 rounded-bl-ot rounded-br-ot border border-gray-300'>
      {
        currentGroup?.GroupTag === "requester" && currentGroup?.OrderIndex === 2 ?
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
      <div className='flex divide-x gap-3'>
        <div className='flex-1 overflow-x-auto'>
          <div className='flex items-start gap-4 pb-2'>
            {
              data.map((item, i) => (
                <div 
                  key={i}
                  className={twMerge(
                    `flex flex-col justify-between border group cursor-pointer rounded-ot transition-all hover:border-blue-300 relative`,
                    selectedOption === item.Id && 'border-blue-700 bg-blue-50 bg-opacity-50',
                  )}
                  onClick={() => handleSelectOption(item, item.DueDate)}
                >
                  <div className='flex justify-between items-center p-2 border-b'>
                    <div className='font-bold leading-none'>Option #1</div>
                    {item.DueDate && 
                      <div className='text-red-500 text-xs flex items-center gap-2'>
                        <span>Deadline:</span>
                        <Timer eventDate={dayjs(item.DueDate).format('YYYY-MM-DD HH:mm')} whenEnd='Expired'/>
                      </div>
                    }
                  </div>
                  <pre className={'text-left text-gray-500 p-2'} style={{fontSize: 11}}>
                    {item.OptionData}
                  </pre>
                  <div className='p-2 text-xs flex flex-col items-start'>
                    <div>Total Cost: {Intl.NumberFormat().format(item.Cost)} ₮</div>
                    {item.Selected === 1 &&
                      <p className=' text-green-600 text-left italic'>selected by {item.SelectedUserName}</p>
                    }
                  </div>
                  <div className='absolute bottom-3 right-3 flex gap-1 items-center opacity-0 group-hover:opacity-100'>
                    <Tooltip title='View Detail'>
                      <Button 
                        className={`px-2 shadow-option1`}
                        onClick={() => { setOpenDrawer(true); setSelectedViewOption(item);}}
                        icon={<EyeOutlined/>}
                        />
                    </Tooltip>
                    {/* {
                      isEditable ?
                      <Tooltip title='Edit'>
                        <Button 
                          className={`px-2 shadow-option1`}
                          onClick={() => handleEditOption(item)}
                          icon={<EditOutlined/>}
                        />
                      </Tooltip>
                      : null
                    } */}
                  </div>
                </div>
              ))
            }
          </div>
        </div>
      </div>
      {
        isChangedoptionvalue ? 
        <div className='flex justify-end gap-2 mt-3'>
          <Button type='primary' className='text-xs' onClick={handleSaveFinalItinerary}>Save</Button>
          <Button type='danger' className='text-xs' onClick={handleCancelButton}>Cancel</Button>
        </div>
        :
        null
      }
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
          <AddOptionForm onCancel={() => setShowModal(false)} getOptionsData={getOptionsData}/>
        }
      </Modal>
    </div>
  )
}

export default ProcessingItinerary