import { LeftOutlined, RightOutlined } from '@ant-design/icons'
import { DatePicker, Drawer, Select } from 'antd'
import { Button } from 'components'
import dayjs from 'dayjs'
import React, { useCallback, useContext, useEffect, useState } from 'react'
import Schedule from './Schedule'
import axios from 'axios'
import BookingDetail from './BookingDetail'
import { AuthContext } from 'contexts'
import html2canvas from 'html2canvas';

const sortingAndGrouping = (data) => {
  const sortedData = data.sort((a, b) => a.ETD - b.ETD)
  const groupedData = sortedData.reduce((acc, current) => {
    let prevData = acc;
    const eventDate = dayjs(current.EventDate).format('YYYY-MM-DD')
    if(!prevData[eventDate]){
      prevData[eventDate] = []
    }
    prevData[eventDate].push(current)
    return prevData
  },{})

  return groupedData
}

function ScheduleCalendar() {
  const [ currentDate, setCurrentDate ] = useState(dayjs())
  const [ selectedMode, setSelectedMode ] = useState('Airplane')
  const [ selectedDir, setSelectedDir ] = useState('All')
  const [ selectedData, setSelectedData ] = useState(null)
  const [ openDrawer, setOpenDrawer ] = useState(false)
  const [ detailLoading, setDetailLoading ] = useState(false)
  const [ loading, setLoading ] = useState(false)
  const [ scheduleMembers, setScheduleMembers ] = useState([])
  const [ data, setData ] = useState([])
  const [ carriers, setCarriers ] = useState([])
  const [ selectedAirline, setSelectedAirline ] = useState('All')
  const [ timeGroup, setTimeGroup ] = useState('All')

  const { state } = useContext(AuthContext)

  useEffect(() => {
    getCarrier()
  },[])

  const getCarrier = () => {
    axios({
      method: 'get',
      url: 'tas/carrier?Active=1'
    }).then((res) => {
      setCarriers(res.data)
    })
  }

  useEffect(() => {
    getCalendarData()
  },[currentDate])

  const getCalendarData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/ActiveTransport/getcalendardata?eventDate=${dayjs(currentDate).format('YYYY-MM-DD')}`
    }).then((res) => {
      const groupedData = sortingAndGrouping(res.data, currentDate)
      setData(groupedData)
    }).catch((err) => {

    }).then(() => {
      setLoading(false)
    })
  }

  const handleChange = useCallback((e) => {
    // if(e.startOf('isoWeek').format('YYYY-MM-DD') !== currentDate.startOf('isoWeek').format('YYYY-MM-DD')){
      setCurrentDate((prev) => prev.startOf('isoWeek').format('YYYY-MM-DD') !== e.startOf('isoWeek').format('YYYY-MM-DD') ? e : prev)
    // }
  },[])

  const handlePrev = useCallback(() => {
    setCurrentDate((prev) => prev.subtract(1, 'week'))
  },[])

  const handleNext = useCallback(() => {
    setCurrentDate((prev) => prev.add(1, 'week'))
  },[])

  const handleSelect = useCallback((data) => {
    setSelectedData(data)
    setOpenDrawer(true)
    setDetailLoading(true)
    axios({
      method: 'get',
      url: `tas/transport/scheduledetail?scheduleId=${data.Id}`
    }).then((res) => {
      setScheduleMembers(res.data)
    }).catch((err) => {

    }).then(() => setDetailLoading(false))
  },[])

  const captureScreenshots = async () => {
    try {
      const element = document.getElementById('schedule-container');
      const divElements = element.querySelectorAll('.capture-div');
      if (!element) {
        console.error('Target element not found');
        return;
      }

      for (const div of divElements) {
        const listClasses = div.classList
        listClasses.remove('h-[calc((100vh-240px)/2)]')
      }
      const containerDiv = element.classList
      containerDiv.remove('overflow-auto')

      // Save original properties
      const originalScroll = {
        scrollLeft: element.scrollLeft,
        scrollTop: element.scrollTop
      };

      // Get full dimensions
      const fullWidth = element.scrollWidth;
      const fullHeight = element.scrollHeight;

      // Setup canvas with full dimensions
      const canvas = await html2canvas(element, {
        width: fullWidth,
        height: fullHeight,
        useCORS: true,
        scrollX: 0,
        scrollY: 0,
        windowWidth: fullWidth,
        windowHeight: fullHeight,
        x: element.offsetLeft,
        y: element.offsetTop,
        scale: 1
      });

      // Restore scroll positions
      element.scrollLeft = originalScroll.scrollLeft;
      element.scrollTop = originalScroll.scrollTop;

      // Trigger download
      const link = document.createElement('a');
      link.download = 'Schedule-calendar.png';
      link.href = canvas.toDataURL('image/png');
      link.click();
      link.remove();

      for (const div of divElements) {
        const listClasses = div.classList
        listClasses.add('h-[calc((100vh-240px)/2)]')
      }
      containerDiv.add('overflow-auto')

    } catch (error) {
      console.error('Screenshot failed:', error);
    }
  };

  return (
    <div className='w-full bg-white rounded-ot py-2 px-3'>
      <div className='flex items-center gap-6 mb-3'>
        {/* <div className='text-secondary2'>{currentDate.startOf('isoWeek').format('MM-DD')} — {currentDate.endOf('isoWeek').format('MM-DD')}</div> */}
        <div className='flex items-center gap-1'>
          <Button onClick={handlePrev} className='px-2' icon={<LeftOutlined/>}></Button>
          <DatePicker
            allowClear={false}
            defaultValue={currentDate}
            value={currentDate}
            onChange={handleChange}
            picker={'week'}
            size='small'
          />
          <Button onClick={handleNext} className='px-2' icon={<RightOutlined/>}></Button>
        </div>
        <div className='flex items-center gap-2'>
          <div className='text-secondary'>Mode:</div>
          <Select
            size='small'
            style={{width: 100}}
            value={selectedMode}
            onChange={(e) => setSelectedMode(e)}
            fieldNames={{value: 'Code', label: 'Code'}}
            options={[
              {value: 'All', Code: 'All'},
              ...state.referData.transportModes
            ]}
          />
        </div>
        {
          (selectedMode === 'All' || selectedMode === 'Airplane') 
          ?
          <div className='flex items-center gap-2'>
            <div className='text-secondary'>Airline:</div>
            <Select
              size='small'
              style={{width: 150}}
              value={selectedAirline}
              onChange={(e) => setSelectedAirline(e)}
              placeholder='All'
              options={[{Description: 'All'}, ...carriers]}
              popupMatchSelectWidth={false}
              fieldNames={{"label": 'Description', "value": 'Description'}}
            />
          </div>
          : null
        }
        <div className='flex items-center gap-2'>
          <div className='text-secondary'>Dir:</div>
          <Select
            size='small'
            style={{width: 120}}
            value={selectedDir}
            onChange={(e) => setSelectedDir(e)}
            options={[
              {value: 'All', label: 'All'},
              {value: 'IN', label: 'IN'}, 
              {value: 'OUT', label: 'OUT'}, 
              {value: 'EXTERNAL', label: 'EXTERNAL'}, 
            ]}
          />
        </div>
        <div className='flex items-center gap-2'>
          <div className='text-secondary'>Time:</div>
          <Select
            size='small'
            style={{width: 120}}
            value={timeGroup}
            onChange={(e) => setTimeGroup(e)}
            options={[
              {value: 'All', label: 'All'},
              {value: 'AM', label: 'AM'}, 
              {value: 'PM', label: 'PM'}, 
            ]}
          />
        </div>
        <Button onClick={captureScreenshots} className='text-xs'>Save</Button>
      </div>
      <Schedule
        data={data}
        date={currentDate}
        onSelect={handleSelect}
        selectedMode={selectedMode}
        selectedDir={selectedDir}
        timeGroup={timeGroup}
        selectedAirline={selectedAirline}
        loading={loading}
      />
      <Drawer
        title={<div>{selectedData?.Code} /{selectedData?.FromLocationCode} - {selectedData?.ToLocationCode}/ <span className='text-gray-500'>{dayjs(selectedData?.EventDate).format('YYYY-MM-DD')}</span></div>}
        open={openDrawer}
        onClose={() => setOpenDrawer(false)}
        width={900}
        styles={{body: {padding: '0px 8px 12px 8px' }}}
      >
        <BookingDetail data={scheduleMembers}/>
      </Drawer>
    </div>
  )
}

export default ScheduleCalendar