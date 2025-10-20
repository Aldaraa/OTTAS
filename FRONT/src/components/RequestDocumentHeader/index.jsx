import { Tag } from 'antd'
import { Accordion, Button, Modal, Table } from 'components'
import dayjs from 'dayjs'
import React, { useRef, useState } from 'react'
import { useReactToPrint } from 'react-to-print'
import { BsCalendarCheck } from 'react-icons/bs'
import { Link } from 'react-router-dom'
import { CheckBox } from 'devextreme-react'
import { LoadingOutlined, PrinterFilled } from '@ant-design/icons'
import axios from 'axios'

const toLink = (e) => {
  if(e.data.DocumentType === 'Non Site Travel'){
    return `/request/task/nonsitetravel/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'Profile Changes'){
    return `/request/task/profilechanges/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'De Mobilisation'){
    return `/request/task/de-mobilisation/${e.data.Id}`
  }
  else if(e.data.DocumentType === 'Site Travel'){
    if(e.data.DocumentTag === 'ADD'){
      return `/request/task/sitetravel/addtravel/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "RESCHEDULE"){
      return `/request/task/sitetravel/reschedule/${e.data.Id}`
    }
    else if(e.data.DocumentTag === "REMOVE"){
     return `/request/task/sitetravel/remove/${e.data.Id}`
    }
  }
}

const nonSiteTravelCols = [
  {
    label: 'Travel Date',
    name: 'TravelDate',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd ')}</div>
    )
  },
  {
    label: 'Favor Time',
    name: 'FavorTime',
  },
  {
    label: 'Depart Location',
    name: 'DepartLocationName',
    alignment: 'left',
  },
  {
    label: 'Arrive Location',
    name: 'ArriveLocationName',
    alignment: 'left',
  },
  {
    label: 'Comment',
    name: 'Comment',
  },
  {
    label: 'ETD',
    name: 'ETD',
    alignment: 'left',
    cellRender: (e) => (
      <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
    )
  },
]

const getTagColor = (status) => {
  switch (status) {
    case 'Confirmed': return 'green'
    case 'Waiting': return 'green'
    case 'Submitted': return 'orange'
  }
}
const siteTravelCols = [
  {
    label: '',
    name: 'Direction',
    width: 60,
  },
  {
    label: 'Date',
    name: 'EventDate',
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
    )
  },
  {
    label: 'Description',
    name: 'Description',
  },
  {
    label: 'Status',
    name: 'Status',
    width: 100,
    cellRender: (e) => (
      <Tag color={getTagColor(e.value)}>{e.value}</Tag>
    )
  },
]

const pendingRequestCols = [
  {
    label: 'Type',
    name: 'DocumentType',
    width: 100
  },
  {
    label: 'Date',
    name: 'RequestedDate',
    width: 80,
    cellRender: (e) => (
      <div>{dayjs(e.value).format('YYYY-MM-DD ddd HH:mm')}</div>
    )
  },
  {
    label: 'Description',
    name: 'Description',
  },
  {
    label: '#',
    name: 'Id',
    alignment: 'left',
    width: 80,
    cellRender: (e) => (
      <div className='flex items-center text-blue-500 hover:underline'>
        <Link to={toLink(e)} target='_blank'>
          {e.value}
        </Link>
      </div>
    )
  },
  {
    label: 'Status',
    name: 'CurrentStatus',
    width: 82,
    cellRender: (e) => (
      <Tag color={getTagColor(e.value)}>{e.value}</Tag>
    )
  },
]

function RequestDocumentHeader({documentDetail, profileData}) {
  const [ showModal, setShowModal ] = useState(false)
  const [ existingBookings, setExistingBookings ] = useState(null)
  const [ loading, setLoading ] = useState(false)

  const printRef = useRef(null)

  const getExistingBookings = (employeeId) => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/requestdocument/existingbooking/${employeeId}`
    }).then((res) => {
      setExistingBookings(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }
  
  const handleClickCalendar = () => {
    setShowModal(true)
    getExistingBookings(documentDetail.EmployeeId)
  }

  const reactToPrintFn = useReactToPrint({ contentRef: printRef, bodyClass: `w-[${printRef.current?.clientWidth}]`, });

  return (
    <>
      <Accordion 
        className='col-span-12 shadow-md rounded-ot overflow-hidden sticky top-0 z-[100]' 
        title='Version' 
        defaultOpen={false}
        whenClosedTitle={<div className='divide-x flex text-gray-500'>
          <div className='pr-5'>
            Request # <span className='ml-4'>{documentDetail?.Id}</span>
          </div>
          <div className='px-5'>
            Requested by
            <span className='text-blue-400 ml-4'>{documentDetail?.RequesterFullName}</span>
          </div>
          <div className='pl-5'>
            Requested <span className='ml-4'>{dayjs(documentDetail?.RequestedDate).format('YYYY-MM-DD ddd HH:mm')}</span>
          </div>
        </div>}
      >
        <div className='flex p-4'>
          <div className='flex-1 flex justify-between text-xs'>
            <div className='flex flex-col gap-1'>
              <div>
                <span className='mr-3 text-secondary2'>Request #</span>
                <span className='font-bold'>{documentDetail?.Id}</span>
              </div>
              <div>
                <span className='mr-3 text-secondary2'>Request by:</span> 
                <span className='text-blue-400'>{documentDetail?.RequesterFullName}</span>
              </div>
            </div>
            <div className='flex flex-col gap-1'>
              <div>
                <span className='mr-3 text-secondary2'>Requested:</span> 
                <span>{dayjs(documentDetail?.RequestedDate).format('YYYY-MM-DD ddd HH:mm')}</span>
              </div>
              <div>
                <span className='mr-3 text-secondary2'>Mail of Requester:</span> 
                <span><a href={`mailto:${documentDetail?.RequesterMail}`}>{documentDetail?.RequesterMail}</a></span>
              </div>
            </div>
            <div className='flex flex-col gap-1'>
              <div>
                <span className='mr-3 text-secondary2'>Subject:</span> 
                <span className='text-blue-400'>{documentDetail?.EmployeeFullName}</span>
              </div>
              <div>
                <span className='mr-3 text-secondary2'>Mobile of Requester:</span> 
                <span><a href={`tel:${documentDetail?.RequesterMobile}`}>{documentDetail?.RequesterMobile}</a></span>
              </div>
            </div>
            <div className='flex flex-col gap-1'>
              <div>
                <span className='mr-3 text-secondary2'>Modified:</span> 
                <span>{documentDetail?.UpdatedInfo}</span>
              </div>

            </div>
            <div className='flex items-center justify-center gap-8 min-w-[100px]'>
              <button type='button' onClick={handleClickCalendar}>
                <BsCalendarCheck size={26}/>
              </button>
            </div>
          </div>
        </div>
      </Accordion>
      <Modal
        open={showModal}
        title='Existing Bookings'
        width={800}
        onCancel={() => setShowModal(false)}
        
      >
        {
          loading ? 
          <div className='p-8 flex justify-center items-center'>
            <LoadingOutlined style={{fontSize: 24}}/>
          </div>
          :
          <>
            <div ref={printRef} className='flex flex-col gap-5'>
              <div>
                <div><span className='text-gray-500 '>Name:</span> {existingBookings?.FullName} ({existingBookings?.EmployeeId})</div>
                <div><span className='text-gray-500'>Department:</span> {existingBookings?.Department}</div>
                <div><span className='text-gray-500'>Employer:</span> {existingBookings?.Employer}</div>
              </div>
              <Table
                data={existingBookings?.SiteTravel}
                columns={siteTravelCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                title={<div className='border-b py-1 font-bold'>Site Travel Requests</div>}
              />
              <Table
                data={existingBookings?.NonSiteTravel}
                columns={nonSiteTravelCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                title={<div className='border-b py-1 font-bold'>Non Site Travel Requests</div>}
              />
              <Table
                data={existingBookings?.PendingRequest}
                columns={pendingRequestCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                tableClass='overflow-hidden text-xs'
                title={<div className='border-b py-1 font-bold'>Pending Requests</div>}
              />
              {/* <Table
                data={existingBookings?.OtherRequest}
                columns={pendingRequestCols}
                pager={false}
                containerClass='shadow-none border border-gray-300'
                tableClass={'max-h-[270px]'}
                title={<div className='border-b py-1 font-bold'>Other Requests</div>}
              /> */}
            </div>
            <div className='flex justify-end mt-5'>
              <Button onClick={reactToPrintFn} type={'primary'} icon={<PrinterFilled/>}>Print</Button>
              {/* <ReactToPrint
                bodyClass='p-5'
                content={() => printRef.current}
                
                trigger={() => }
              /> */}
            </div>
          </>
        }
      </Modal>
    </>
  )
}

export default RequestDocumentHeader