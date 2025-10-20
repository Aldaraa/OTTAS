import { Badge } from 'antd'
import { DocumentTransportList, ProfileInfoHeader } from 'components'
import dayjs from 'dayjs'
import React from 'react'

function ProfileCalendar({profile}) {

  return (
    <div className='flex gap-5 xl:flex-row flex-col xl:items-start'>
      {(profile?.DateCreated && dayjs(profile?.DateCreated).diff(dayjs().subtract(2, 'd')) > 0) ?
        <Badge.Ribbon text='New Employee' color='blue'>
          <ProfileInfoHeader data={profile}/>
        </Badge.Ribbon>
        :
        <ProfileInfoHeader data={profile}/>
      }
      <DocumentTransportList 
        profileData={profile}
        employeeId={profile?.Id}
      />
    </div>
  )
}

export default ProfileCalendar