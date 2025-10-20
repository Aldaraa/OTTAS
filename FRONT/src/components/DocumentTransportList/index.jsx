import React, { useState } from 'react'
import dayjs from 'dayjs'
import { CalendarOutlined } from '@ant-design/icons';
import { Button, FlightList, Modal, VerticalCalendar } from 'components';
import { twMerge } from 'tailwind-merge';
import isoWeek from 'dayjs/plugin/isoWeek'
dayjs.extend(isoWeek)

function DocumentTransportList({data, listData, containerClass='', employeeId, profileData, ...restProps}) {
  const [ showModal, setShowModal ] = useState(false)

  return (
    <div id='calendar' className={twMerge(`flex-1`, containerClass)}>  
      <div className='w-full overflow-hidden bg-white rounded-ot border px-2 pb-2'>
        <div className='flex justify-between items-center my-2 px-2'>
          <div className='flex gap-3 items-center font-medium'>
            Transport information
          </div>
          <div className='flex gap-3'>
            <Button icon={<CalendarOutlined />} onClick={() => setShowModal(true)}></Button>
          </div>
        </div>
        <FlightList 
          className='shadow-none border-none px-2'
          height={130}
          showHeader={false}
          employeeId={employeeId}
          profileData={profileData}
        />
      </div>
      <Modal title='Transport Calendar' destroyOnClose={false} open={showModal} onCancel={() => setShowModal(false)} width={1200}>
        <VerticalCalendar employeeId={employeeId}/>
      </Modal>
    </div>
  )
}

export default DocumentTransportList