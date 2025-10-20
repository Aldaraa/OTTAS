import axios from 'axios'
import dayjs from 'dayjs'
import React, { useEffect, useState } from 'react'
import purify from 'dompurify';

function VersionDetail({versionId}) {
  const [ data, setData ] = useState([])

  useEffect(() => {
    if(versionId){
      getData()
    }
  },[versionId])

  const getData = () => {
    axios({
      method: 'get',
      url: `tas/sysversion/releasenote/${versionId}`
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {

    })
  }

  return (
    <div className='mt-3'>
      <div className='font-medium mb-3'>Version Notes</div>
      <div className=' whitespace-pre-wrap' dangerouslySetInnerHTML={{__html: purify.sanitize(data?.ReleaseNote)}}></div>
      <div className='flex justify-end mt-3'>
        <div className='text-xs text-gray-400'>Released date: {dayjs(data?.ReleaseDate).format('YYYY-MM-DD')}</div>
      </div>
    </div>
  )
}

export default VersionDetail