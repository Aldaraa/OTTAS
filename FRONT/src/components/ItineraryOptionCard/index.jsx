import { DeleteOutlined, EditOutlined, EyeOutlined } from '@ant-design/icons'
import { Button, Timer, Tooltip } from 'components'
import dayjs from 'dayjs'
import React from 'react'
import { twMerge } from 'tailwind-merge'

function ItineraryOptionCard({onSelect, onClickViewBtn, onClickEditBtn, onClickDeleteBtn, selectedOption, data, isEditable, isDeletable}) {
  return (
    <div 
      className={twMerge(
        `flex flex-col justify-between border group cursor-pointer rounded-ot transition-all hover:border-blue-300 relative`,
        selectedOption === data.Id && 'border-blue-700 bg-blue-50 bg-opacity-50',
      )}
      onClick={() => onSelect(data, data.DueDate)}
    >
      <div className='flex justify-between items-center p-2 border-b'>
        <div className='font-bold leading-none'>Option #{data.OptionIndex}</div>
        {data.DueDate && 
          <div className='text-red-500 text-xs flex items-center gap-2'>
            <Timer eventDate={dayjs(data.DueDate).format('YYYY-MM-DD HH:mm')} whenEnd='Expired'/>
          </div>
        }
      </div>
      <pre className={'text-left text-gray-500 p-2'} style={{fontSize: 11}}>
        {data.OptionData}
      </pre>
      <div className='p-2 text-xs flex flex-col items-start'>
        <div>Total Cost: {Intl.NumberFormat().format(data.Cost)} ₮</div>
        {data.Selected === 1 &&
          <p className=' text-green-600 text-left italic'>selected by {data.SelectedUserName}</p>
        }
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
          <Tooltip title='Edit'>
            <Button 
              className={`px-2 shadow-option1`}
              onClick={() => onClickEditBtn(data)}
              icon={<EditOutlined/>}
            />
          </Tooltip>
          : null
        }
        {
          isDeletable ? 
          <Tooltip title='Delete Option'>
            <Button 
              className={`px-2 shadow-option1`}
              onClick={() => onClickDeleteBtn(data)}
              icon={<DeleteOutlined/>}
              type={'danger'}
            />
          </Tooltip>
          : null
        }
      </div>
    </div>
  )
}

export default ItineraryOptionCard