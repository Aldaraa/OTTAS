import { EditOutlined, SaveOutlined } from '@ant-design/icons'
import { notification } from 'antd'
import axios from 'axios'
import { Button, HTMLEditor, Skeleton } from 'components'
import React, { useCallback, useEffect, useRef, useState } from 'react'
import purify from 'dompurify';

function Agreement() {
  const [ data, setData ] = useState(null)
  const [ loading, setLoading ] = useState(true)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ isEdit, setIsEdit ] = useState(false)

  const [api, contextHolder] = notification.useNotification();
  const editorRef = useRef(null)

  useEffect(() => {
    getData()
  },[])

  const getData = useCallback(() => {
    axios({
      method: 'get',
      url: 'tas/agreement'
    }).then((res) => {
      setData(res.data)
    }).catch((err) => {
      
    }).then(() => setLoading(false))
  },[])

  const handleSubmit = () => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/agreement/create',
      data:{ 
        agreementText: editorRef.current.instance.option('value')
      }
    }).then(() => {
      setIsEdit(false)
      getData()
    }).catch(({err}) => {
      if(err.response.status === 400){
        api.error({
          message: 'Error',
          duration: 5,
          description: <div>
            {err.response?.data?.message}
          </div>
        });
      }
    }).then(() => setActionLoading(false))
  }


  return (
    <div className='flex justify-center'>
      <div className='min-w-[400px] max-w-[800px] rounded-ot shadow-card bg-white p-5 flex flex-col w-full'>
        <div className='text-lg font-bold mb-3'>Privacy Policy</div>
        {
          loading ? 
          <Skeleton/>
          :
          <>
            {
              isEdit ? 
              <>
                <HTMLEditor className='flex-1' ref={editorRef} defaultValue={data.AgreementText}/>
                <div className='flex justify-end mt-5 self-end gap-4'>
                  <Button type='primary' onClick={handleSubmit} loading={actionLoading} icon={<SaveOutlined/>}>Save</Button>
                  <Button onClick={() => setIsEdit(false)} disabled={actionLoading}>Cancel</Button>
                </div>
              </>
              :
              <>
                <div dangerouslySetInnerHTML={{__html: purify.sanitize(data?.AgreementText)}}></div>
                <Button className='self-end mt-5' type='primary' onClick={() => setIsEdit(true)} icon={<EditOutlined/>}>Edit</Button>
              </>
            }
          </>
        } 
      </div>
      {contextHolder}
    </div>
  )
}

export default Agreement