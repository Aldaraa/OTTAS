import axios from 'axios'
import { Button, Form } from 'components'
import React, { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'

function TravelInfo({flightData, getData, documentDetail, form}) {
  const [ loading, setLoading ] = useState(false)
  const [ isEditFlightData, setIsEditFlightData ] = useState(false)
  const [ travelAgent, setTravelAgent ] = useState([])
  const [ travelPurpose, setTravelPurpose ] = useState([])
  const { documentId } = useParams()

  useEffect(() => {
    getTravelPurposeList()
    getTicketAgentList()
  },[])

  const getTravelPurposeList = () => {
    axios({
      method: 'get',
      url: 'tas/requesttravelpurpose?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({
          value: item.Id,
          label: item.Description
        })
      })
      setTravelPurpose(tmp)
    }).catch((err) => {

    })
  }

  const getTicketAgentList = () => {
    axios({
      method: 'get',
      url: 'tas/requesttravelagent?Active=1',
    }).then((res) => {
      let tmp = []
      res.data?.map((item) => {
        tmp.push({
          value: item.Id,
          label: item.Description
        })
      })
      setTravelAgent(tmp)
    }).catch((err) => {

    })
  }

  const fields = [
    {
      label: 'Travel Purpose',
      name: 'RequestTravelPurposeId',
      className: 'col-span-4 mb-0',
      rules: [{required: true, message: 'Travel Purpose is required'}],
      type: 'select',
      inputprops: {
        options: travelPurpose
      }
    },
    {
      label: 'Cost',
      name: 'Cost',
      className: 'col-span-4 mb-0',
      rules: [{required: true, message: 'Cost is required'}],
      type: 'price',
      inputprops: {
        className: 'w-full'
      }
    },
    {
      label: 'Highest Cost',
      name: 'HighestCost',
      className: 'col-span-4 mb-0',
      rules: [{required: true, message: 'Cost is required'}],
      type: 'price',
      inputprops: {
        className: 'w-full'
      }
    },
    {
      label: 'Updated Cost',
      name: 'Cost2',
      className: 'col-span-4 mb-0',
      type: 'price',
      inputprops: {
        // disabled: true
      }
    },
    {
      label: 'Ticket Agent',
      name: 'RequestTravelAgentId',
      className: 'col-span-4 mb-0',
      type: 'select',
      rules: [{required: true, message: 'Ticket Agent is required'}],
      inputprops: {
        options: travelAgent
      }
    },
    {
      label: 'Issuer name',
      name: 'RequestTravelAgentSureName',
      className: 'col-span-4 mb-0',
      rules: [{required: true, message: 'Issuer name is required'}],
    },
  ]

  const handleSubmitTravelData = (values) => {
    setLoading(true)
    axios({
      method: 'put',
      url: 'tas/requestnonsitetravel/traveldata',
      data: {
        ...values,
        documentId: documentId,
      }
    }).then((res) => {
      getData(documentDetail?.EmployeeId)
      setIsEditFlightData(false)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  return (
    <div className='px-4 pb-4'>
      <Form
        form={form}
        editData={flightData}
        fields={fields}
        onFinish={handleSubmitTravelData}
        disabled={!isEditFlightData}
        className='gap-y-3 gap-x-4 w-full'
        noLayoutConfig={true}
        layout='vertical'
        size='small'
      >
        <div className='flex justify-end gap-2 text-xs col-span-12'>
          {
            isEditFlightData ? 
            <>
              <Button
                type='primary' 
                loading={loading}
                onClick={() => form?.submit()} 
                htmlType='button'
              >
                Save
              </Button>
              <Button 
                disabled={loading}
                onClick={() => setIsEditFlightData(false)} 
                htmlType='button'
              >
                Cancel
              </Button>
            </>
            :
            <Button   
              onClick={() => setIsEditFlightData(true)} 
              htmlType='button'
            >
              Edit
            </Button>  
          }
        </div>
      </Form>
    </div>
  )
}

export default TravelInfo