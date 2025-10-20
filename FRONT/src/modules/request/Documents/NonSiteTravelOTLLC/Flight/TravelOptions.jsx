import { MinusCircleOutlined } from '@ant-design/icons'
import { Input, InputNumber } from 'antd'
import { Button, Form, Modal } from 'components'
import React, { useMemo, useState } from 'react'
import { twMerge } from 'tailwind-merge'

const SelectedOption = ({options=[]}) => {
  return <>
    {
      options.map((item, i) => (
        <div 
          key={i}
          className={twMerge(`col-span-3 flex flex-col gap-4 justify-between p-2 border group cursor-pointer rounded-ot transition-all hover:border-blue-300`,'border-blue-400')}
        >
          <div className='flex flex-col'>
            <pre className={'text-xs text-left'} style={{fontSize: 11, fontFamily: "'Courier New', monospac"}}>
              {item.OptionData}
            </pre>
            <div className='text-right mt-2 text-green-500 text-[12px]'>Total Cost: {Intl.NumberFormat().format(item.Cost)} ₮</div>
          </div>
          <p className='text-gray-400 text-left italic text-xs'>selected by {item.SelectedUserName}</p>
        </div>
      ))
    }
  </>
}

const OptionItem = ({item, onClick, selectedOption, isShowActionButton}) => {
  const handleSelect = (option) => {
    if(onClick){
      onClick(option)
    }
  }
  return(
    <div 
      key={i}
      className={twMerge(
        `flex flex-col gap-4 justify-between p-2 border group cursor-pointer rounded-ot transition-all hover:border-blue-300`,
        selectedOption === item.Id && 'border-blue-700 bg-blue-50 bg-opacity-50',
      )}
      onClick={() => handleSelect(item)}
    >
      <div className='flex flex-col'>
        <div className='flex justify-between items-center'>
          <div className='text-red-500 mb-3 text-xs flex items-center gap-2'>
            {item.DueDate && 
              <>
                <span>Deadline:</span>
                <Timer eventDate={dayjs(item.DueDate).format('YYYY-MM-DD HH:mm')} whenEnd='Expired'/>
              </>
            } 
          </div>
          {
            isShowActionButton ?
            <div className='flex'>
              <Button 
                className={`text-xs hidden group-hover:block`}
                onClick={() => handleEditOption(item)}
                icon={<EyeOutlined/>}
              >
              </Button>
              <Button 
                className={`text-xs hidden group-hover:block`}
                onClick={() => handleEditOption(item)}
              >
                Edit
              </Button>
            </div>
            : null
          }
        </div>
        <pre className={'text-left text-gray-500'} style={{fontSize: 11}}>
          {item.OptionData}
        </pre>
        <div className='text-right mt-2 text-[12px]'>Total Cost: {Intl.NumberFormat().format(item.Cost)} ₮</div>
      </div>
      {item.Selected === 1 &&
        <p className=' text-green-600 text-left italic text-xs'>selected by {item.SelectedUserName}</p>
      }
    </div>
  )
}


function TravelOptions({disabled, travelOptions=[], currentGroup}) {
  const [ showAddModal, setShowAddModal ] = useState(false)

  const [ form ] = Form.useForm()

  const isRenderOption = useMemo(({currentGroup}) => {
    let render = false
    if(
      (currentGroup?.GroupTag === "travelflight" && currentGroup?.OrderIndex === 1) ||
      (currentGroup?.GroupTag === "requester" && currentGroup?.OrderIndex === 2) || 
      currentGroup?.GroupTag === "linemanager"
    ){
      render = true
    }
    return render
  },[currentGroup])

  const isShowActionButton = useMemo(() => {
    return currentGroup?.GroupTag === "travelflight" && currentGroup?.OrderIndex === 1
  },[currentGroup])


  const handleSubmit = () => {
    
  }
  
  return (
    <>
      {
        !disabled  ?
        <div className='flex-1 pr-5'>
          {
            isShowActionButton ?
            <div className='flex justify-between items-center gap-2 mb-4 pl-2'>
              <div className='font-bold'>Options <span className='font-normal'>({travelOptions.length})</span></div>
              <Button
                htmlType='button' 
                type='primary' 
                onClick={() => setShowAddModal(true)} 
                loading={optionLoading}
                className='text-xs'
                disabled={disabled}
              >
                Add Options
              </Button>
            </div>
            : null
          }
          <div className='flex flex-col gap-4'>
            {
              isRenderOption ?
              <div className='flex flex-wrap items-stretch gap-4'>
                {
                  travelOptions.map((item, i) => (
                    <OptionItem
                      item={item}
                      key={i}
                      selectedOption={selectedOption}
                      onClick={(option) => handleSelectOption(option.Id, option.DueDate)}
                      onClickEdit={(option) => handleEditOption(option)}
                      // onClickView={(option) => console.log('eee')}
                    />
                  ))
                }
              </div>
              : 
              <div className='flex flex-wrap items-stretch gap-4'>
                {
                  travelOptions.map((item, i) => (
                    item.Selected === 1 ?
                    <div 
                      key={i}
                      className={twMerge(`col-span-3 flex flex-col gap-4 justify-between p-2 border group cursor-pointer rounded-ot transition-all hover:border-blue-300`,'border-blue-400')}
                    >
                      <div className='flex flex-col'>
                        <div className='flex justify-between items-center'>
                          <div className='text-red-500 mb-3 flex items-center gap-2'>
                          </div>
                        </div>
                        <pre className={'text-xs text-left'} style={{fontSize: 11, fontFamily: "'Courier New', monospac"}}>
                          {item.OptionData}
                        </pre>
                        <div className='text-right mt-2 text-green-500 text-[12px]'>Total Cost: {Intl.NumberFormat().format(item.Cost)} ₮</div>
                      </div>
                      {item.Selected === 1 &&
                        <p className='text-gray-400 text-left italic text-xs'>selected by {item.SelectedUserName}</p>
                      }
                    </div>
                    : null
                  ))
                }
              </div>
            }
            {
              isChangedoptionvalue ? 
              <div className='flex gap-2 self-end'>
                <Button type='primary' className='text-xs' onClick={handleSaveFinalItinerary}>Save</Button>
                <Button type='danger' className='text-xs' onClick={cancelOptionChange}>Cancel</Button>
              </div>
              :
              null
            }
          </div>
        </div>
        : 
        <div className='flex-1 pr-5'>
          <div className='flex flex-wrap items-stretch gap-4'>
            {
              documentDetail?.CurrentStatus === 'Completed' &&  hasUpdatePermission ?
              travelOptions.map((item, i) => (
                <div 
                  key={i}
                  className={twMerge(`col-span-3 flex flex-col gap-4 justify-between p-2 border group cursor-pointer rounded-ot transition-all`,item.Selected === 1 ? 'border-blue-400' : '')}
                >
                  <div className='flex flex-col'>
                    <pre className={'text-xs text-left'} style={{fontSize: 11, fontFamily: "'Courier New', monospac"}}>
                      {item.OptionData}
                    </pre>
                    <div className='text-right mt-2 text-green-500 text-[12px]'>Total Cost: {Intl.NumberFormat().format(item.Cost)} ₮</div>
                  </div>
                  {item.Selected === 1 &&
                    <p className='text-gray-400 text-left italic text-xs'>selected by {item.SelectedUserName}</p>
                  }
                </div>
              ))
              : 
              <SelectedOption options={travelOptions.filter((item) => item.Selected === 1)}/>
            }
        </div>
      </div>
    }
      <Modal
        width={700}
        title={'Add Options'}
        open={showAddModal}
        onCancel={() => setShowAddModal(false)}
      >
        <Form
          form={form}
          onFinish={handleSubmit}
          noLayoutConfig={true}
          layout='vertical'
        >
          <Form.List name="options" className='col-span-12'>
            {(fields, { add, remove }) => (
              <div className='col-span-12 grid grid-cols-12 gap-x-2 gap-y-2 items-center'>
                {fields.map(({ key, name, ...restField }) => (
                  <>
                    <Form.Item
                      {...restField}
                      className='col-span-10 mb-0'
                      name={[name, 'optiontext']}
                      label={<span className='font-bold'>Option#{key+1}</span>}
                      // rules={[{ required: true, message: 'Missing first name' },]}
                    >
                      <Input.TextArea 
                        autoSize={{minRows: 3, maxRows: 10}}
                      />
                    </Form.Item>
                    <button 
                      type='button'
                      className='bg-[#FFE2E5] text-[#F64E60] hover:bg-red-200 rounded-md py-1 disabled:bg-gray-100  transition-all' 
                      onClick={() => remove(name)}
                    >
                      <MinusCircleOutlined />
                    </button>
                    <Form.Item
                      {...restField}
                      className='col-span-2 mb-0'
                      name={[name, 'hour']}
                      label='Deadline (hour)'
                      // rules={[{ required: true, message: 'Missing first name' },]}
                    >
                      <InputNumber controls={false} max={24} min={0}/>
                    </Form.Item>
                    <Form.Item
                      {...restField}
                      className='col-span-3 mb-0'
                      name={[name, 'minute']}
                      label='Deadline (minute)'
                      // rules={[{ required: true, message: 'Missing first name' },]}
                    >
                      <InputNumber controls={false} max={59} min={0}/>
                    </Form.Item>
                  </>
                ))}
                <Form.Item className='col-span-12'>
                  <Button className='w-full flex justify-center' onClick={() => add()} icon={<PlusOutlined />}>
                    Add Option
                  </Button>
                </Form.Item>
              </div>
            )}
          </Form.List>
          <div className='col-span-12 flex justify-end items-center gap-2'>
            <Button type='primary' onClick={() => form.submit()} loading={optionLoading} icon={<SaveOutlined/>}>Save</Button>
            <Button onClick={() => {setShowAddModal(false); form.resetFields()}} disabled={optionLoading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
    </>
  )
}

export default TravelOptions