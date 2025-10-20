import axios from 'axios'
import { Button, Form, Modal, ShiftCalendar } from 'components'
import React, { useCallback, useContext, useEffect, useState } from 'react'
import { Dropdown, Input } from 'antd'
import { useParams } from 'react-router-dom'
import dayjs from 'dayjs'
import { AuthContext } from 'contexts'
import hexToHSL from 'utils/hexToHSL'
import { twMerge } from 'tailwind-merge'
import ChangeRoom from 'modules/request/Documents/SiteTravelOTLLC/ChangeRoom'

const items = [
  {label: 'Set RR', key: '1'},
  {label: 'Set DS with Virtual Room', key: '2'},
]

const RenderCell = React.memo(({date, data, selectedData, onSelect, handleClickMenu}) => {
  let currentData = data[date]

  return(
    <Dropdown
      menu={{items: items, onClick: ({key}) => handleClickMenu(key, date)}}
      trigger={['contextMenu']}
    >
      <button
        onClick={() => onSelect(currentData)}
        className={twMerge(
          'flex-1 flex flex-col -m-1 p-1 border border-white hover:border-gray-200',
          (selectedData && (dayjs(selectedData?.EventDate).format('YYYY-MM-DD') === date)) ?
          'border-red-500 hover:border-red-500 bg-gray-200' :
          null
        )}
      >
        <div className='flex flex-col w-full items-start gap-1'>
          <div className='font-medium'>
            {dayjs(date).format('DD')} <span className='text-[11px] font-normal text-secondary2'>{dayjs(date).format('MMM')}</span>
          </div>
          <div className='leading-none self-end p-1 rounded text-[11px]' style={{backgroundColor: currentData?.Color, color: currentData?.Color ? hexToHSL(currentData?.Color) > 60 ? 'black' : 'white' : 'black'}}>
            {currentData?.ShiftCode}
          </div>
        </div>
      </button>
    </Dropdown>
  )
}, (prevProps, nextProps) => {
  if(JSON.stringify(prevProps.data) !== JSON.stringify(nextProps.data) || JSON.stringify(prevProps.selectedData) !== JSON.stringify(nextProps.selectedData)){
    return false
  }
  return true
})

function ShiftVisual() {
  const [ data, setData ] = useState([])
  const [ selectedDate, setSelectedDate ] = useState(dayjs().startOf('M'))
  const [ shiftStatus, setShiftStatus ] = useState([])
  // const [ bulkShiftStatus, setBulkShiftStatus ] = useState([])
  const [ onSiteShifts, setOnSiteShifts ] = useState([])
  const [ offSiteShifts, setOffSiteShifts ] = useState([])
  const [ showModal, setShowModal ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ selectedData, setSelectedData ] = useState(null)
  const [ dateInfo, setDateInfo ] = useState(null)
  const [ isSelected, setIsSelected ] = useState(false)

  const [form] = Form.useForm()
  const { employeeId } = useParams()
  const { state } = useContext(AuthContext)

  useEffect(() => {
    getData()
    getShift()
  },[selectedDate]) // eslint-disable-line

  useEffect(() => {
    if(selectedData) {
      getDateInfo(selectedData.EventDate)
    }
  },[selectedData]) // eslint-disable-line

  const getData = useCallback(() => {
    axios({
      method: 'get',
      url: `tas/employeestatus/visualstatusdates/${employeeId}/${dayjs(selectedDate).format('YYYY-MM-DD')}`
    }).then((res) => {
      let tmp = {}
      res.data.forEach((item, i) => {
        tmp[dayjs(item.EventDate).format('YYYY-MM-DD')] = item
      })
      setData(tmp)
    }).catch((err) => {

    })
  },[employeeId, selectedDate])

  const getDateInfo = useCallback((eventDate) => {
    if(eventDate){
      axios({
        method: 'get',
        url: `tas/safemode/employeestatus/${employeeId}/${dayjs(eventDate).format('YYYY-MM-DD')}`
      }).then((res) => {
        if(res.data.Id !== 0){
          setDateInfo(res.data)
        }else{
          setDateInfo({
            ...res.data,
            CostCodeId: state.userProfileData?.CostCodeId,
            DepartmentId: state.userProfileData?.DepartmentId,
            EmployerId: state.userProfileData?.EmployerId,
            PositionId: state.userProfileData?.PositionId,
          })
        }
      }).catch((err) => {
        
      })
    }
  },[employeeId, state.userProfileData])

  const getShift = () => {
    axios({
      method: 'get',
      url: 'tas/shift?Active=1',
    }).then((res) => {
      let tmp = []
      let onsites = []
      let offsites = []
      let bulk = []

      res.data.forEach((item) => {
        tmp.push({
          value: item.Id, 
          label: `${item.Code}`, 
          content: <div className='flex gap-1 text-xs'>
            <span>{item.Code}</span>
            {/* <span className='text-gray-400'>{item.OnSite === 1 ? 'On Site' : 'Off Site'}</span> */}
          </div>, 
          ...item
        })
        bulk.push({
          value: item.Id, 
          label: `${item.Code}-${item.Description}`, 
          content: <div className='flex gap-1 text-xs'>
            <span>{item.Code}</span>
          </div>,
          ...item
        })
        if(item.OnSite === 1){
          onsites.push({
            value: item.Id, 
            label: item.Code,
            content: <div className='flex gap-1 text-xs'>
              <span>{item.Code}</span>
            </div>,
            ...item})
        }
        else{
          offsites.push({
            value: item.Id, 
            label: item.Code,
            content: <div className='flex gap-1 text-xs'>
              <span>{item.Code}</span>
            </div>,
            ...item
          })
        }
      })
      // setBulkShiftStatus(bulk)
      setShiftStatus(tmp)
      setOffSiteShifts(offsites)
      setOnSiteShifts(onsites)
    }).catch((err) => {
  
    })
  }

  const handleSubmit = (values) => {
    setActionLoading(true)
    axios({
      method: 'post',
      url: 'tas/safemode/employeestatus',
      data: {
        ...values,
        employeeId: parseInt(employeeId),
        EventDate: dayjs(selectedData?.EventDate).format('YYYY-MM-DD'),
      }
    }).then((res) => {
      getData()
      handleCancelEdit()
    }).catch((err) => {

    }).finally(() => setActionLoading(false))
  }

  const fields = [
    {
      label: 'Department',
      name: 'DepartmentId',
      className: 'col-span-6 mb-2',
      type: 'treeSelect',
      inputprops: {
        treeData: state.referData?.departments,
        fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
        allowClear: true,
        showSearch: true,
        filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
      }
    },
    {
      label: 'Employer',
      name: 'EmployerId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.employers,
      }
    },
    {
      label: 'Cost Code',
      name: 'CostCodeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.costCodes,
      }
    },
    {
      label: 'Position',
      name: 'PositionId',
      className: 'col-span-6 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.positions,
      }
    },
    {
      label: 'Shift',
      name: 'ShiftId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: shiftStatus,
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prev, cur) => prev.ShiftId !== cur.ShiftId}>
        {({getFieldValue}) => {
          const shiftId = getFieldValue('ShiftId')
          const isOnSite = onSiteShifts.find((item) => item.Id === shiftId)
          return(
            <div className='col-span-12 flex gap-4 items-end'>
              <Form.Item name='RoomId' noStyle rules={[{required: !!isOnSite, message: 'Room is required'}]}/>
              <Form.Item name='RoomNumber' className={twMerge('mb-2 flex-1', isOnSite ? 'block' : 'hidden')} label='Room'>
                <Input disabled className='w-full'/>
              </Form.Item>
              <Form.Item className={twMerge('mb-2', isOnSite ? 'block' : 'hidden')}>
                <Button onClick={() => setShowModal(true)}>Change</Button>
              </Form.Item>
            </div>
          )
        }}
      </Form.Item>
    },
  ]

  const handleSelectRoom = useCallback((room) => {
    form.setFieldValue('RoomId', room.RoomId)
    form.setFieldValue('RoomNumber', room.roomNumber)
  },[form])

  const handleSelectDate = (dateData) => {
    if(isSelected && selectedData?.EventDate === dateData.EventDate){
      setIsSelected(false)
      setSelectedData(null)
      setDateInfo(null)
    }else{
      setIsSelected(true)
      setSelectedData(dateData)
    }
  }

  const handleClickMenu = (key, date) => {
    if(key === '1'){
      axios({
        method: 'put',
        url: `tas/safemode/setrr`,
        data: {
          employeeId: parseInt(employeeId),
          eventDate: date
        }
      }).then((res) => {
        const shiftData = offSiteShifts.find((item) => item.Id === res.data)
        setData((prev) => ({
          ...prev,
          [date]: {
            ...prev[date],
            ShiftId: res.data,
            Color: shiftData?.ColorCode,
            ShiftDescription: shiftData?.Description,
            ShiftCode: shiftData.Code,
          }
        }))
        // getData()
      }).catch((err) => {

      })
    }
    if(key === '2'){
      axios({
        method: 'put',
        url: `tas/safemode/setds`,
        data: {
          employeeId: parseInt(employeeId),
          eventDate: date
        }
      }).then((res) => {
        const shiftData = onSiteShifts.find((item) => item.Id === res.data)
        setData((prev) => ({
          ...prev,
          [date]: {
            ...prev[date],
            ShiftId: res.data,
            Color: shiftData?.ColorCode,
            ShiftDescription: shiftData?.Description,
            ShiftCode: shiftData?.Code,
          }
        }))
      }).catch((err) => {

      })
    }
  }

  const handleCancelEdit = useCallback(() => {
    setIsSelected(false)
    setSelectedData(null)
  },[])

  return (
    <div className='bg-white rounded-ot p-4 col-span-12 shadow-md'>
      <div className='grid grid-cols-12 gap-5'>
        <div className='col-span-6'>
          <ShiftCalendar
            containerClass={'shadow-none'} 
            picker='month' 
            onChange={(e) => setSelectedDate(dayjs(e).format('YYYY-MM-DD'))} 
            data={data}
            selectedData={selectedData}
            extraComponent={<div className='flex-1 flex justify-end items-center text-base font-bold'>Change Shift</div>}
            cellRender={(e, data) => (
              <RenderCell
                date={e}
                // isEdit={isEdit}
                data={data}
                shiftStatus={shiftStatus}
                offSiteShifts={offSiteShifts}
                onSiteShifts={onSiteShifts}
                onSelect={(data) => handleSelectDate(data)}
                selectedData={selectedData}
                handleClickMenu={handleClickMenu}
              />
            )}
            currentDate={selectedDate}
          />
        </div>
        <div className='col-span-6 mt-6'>
          {
            isSelected ?
            <Form
              form={form}
              fields={fields}
              noLayoutConfig
              layout={'vertical'}
              editData={dateInfo}
              onFinish={handleSubmit}
              className='gap-x-4'
            >
              <div className='col-span-12 flex justify-end gap-5 mt-4'>
                <Button
                  type='primary'
                  htmlType='submit'
                  onClick={() => form.submit()}
                  loading={actionLoading}
                >
                  {dateInfo?.ShiftId ? 'Save' : 'Add'}
                </Button>
                <Button onClick={handleCancelEdit}>Cancel</Button>
              </div>
            </Form>
            : null
          }
        </div>
      </div>
      <Modal
        title={<span>Select Room <b>{dayjs(selectedData?.EventDate).format('YYYY-MM-DD')}</b></span>}
        open={showModal}
        onCancel={() => setShowModal(false)}
        width={900}
      >
        <ChangeRoom
          startDate={dayjs(selectedData?.EventDate).format('YYYY-MM-DD')}
          endDate={dayjs(selectedData?.EventDate).format('YYYY-MM-DD')}
          handleSelect={(e) => handleSelectRoom(e)}
          form={form}
          closeModal={() => setShowModal(false)}
        />
      </Modal>
    </div>
  )
}

export default ShiftVisual