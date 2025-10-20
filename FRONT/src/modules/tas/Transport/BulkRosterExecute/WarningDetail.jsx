import { Tabs, Tag } from "antd";
import { Table } from "components";
import COLORS from "constants/colors";
import dayjs from "dayjs";

const ReduceDateList = (list) => {
  let tmp = []
  let lastRange = null
  let lastItem = null
  list?.forEach((item, i) => {
    if(!lastRange){
      lastRange = {
        startDate: item.EventDate,
        ShiftCode: item.ShiftCode,
        endDate: item.EventDate
      }
    }else {
      let isStraight = dayjs(lastItem.EventDate).add(1, 'day').format('YYYY-MM-DD') === dayjs(item?.EventDate).format('YYYY-MM-DD')
      if(isStraight && lastItem.ShiftCode === item.ShiftCode){
        lastRange = {
          ...lastRange,
          endDate: item.EventDate,
        }
      }else{
        tmp.push({
          ...lastRange,
          EventDate: `${dayjs(lastRange.startDate).format('YYYY-MM-DD')} ${lastRange.startDate !== lastRange.endDate && `– ${dayjs(lastRange.endDate).format('YYYY-MM-DD')}`}`,
        })
        lastRange = {
          startDate: item.EventDate,
          ShiftCode: item.ShiftCode,
          endDate: item.EventDate
        }
      }
    }
    if(i === list.length-1){
      tmp.push({
        ...lastRange,
        EventDate: `${dayjs(lastRange.startDate).format('YYYY-MM-DD')} ${lastRange.startDate !== lastRange.endDate ? `– ${dayjs(lastRange.endDate).format('YYYY-MM-DD')}` : ''}`,
      })
    }
    lastItem = item
  })
  return tmp
}

const empCol = [
  {
    label: 'Event Date',
    name: 'EventDate',
  },
  {
    label: 'Shift Code',
    name: 'ShiftCode',
  },
]

const SiteCol = [
  {
    label: 'Event Date',
    name: 'EventDate',
  },
  {
    label: 'Shift Code',
    name: 'ShiftCode',
  },
]

const WarningDetail = (e) => {
  let items = []
  if(e.data?.data?.EmployeeRoomData?.length > 0){
    items.push({
      key: '1',
      label: `Employee Room`,
      children: <div className='pt-4'>
        <Table
          data={e.data?.data?.EmployeeRoomData} 
          columns={empCol} 
          focusedRowEnabled={false}
          pager={e.data?.data?.EmployeeRoomData?.length > 20}
          tableClass='border px-2 overflow-hidden rounded-ot select-none max-h-[300px]'
          containerClass='pb-2 shadow-none pl-0 pr-0'
        />
      </div>,
    },)
  }
  if(e.data?.data?.OnsiteData){
  items.push({
      key: '2',
      label: `On Site`,
      children: <div className='p-4'>
        <table className='text-xs'>
          <tr>
            <td>In Date:</td>
            <td className='pl-2'>{e.data?.data?.OnsiteData?.InTransportDate ? dayjs(e.data?.data?.OnsiteData?.InTransportDate).format('YYYY-MM-DD') : null}</td>
            <td className='pl-3'>{e.data?.data?.OnsiteData?.INActiveTransportCode} {e.data?.data?.OnsiteData?.INScheduleDescription}</td>
          </tr>
          <tr>
            <td>Out Date:</td>
            <td className='pl-2'>{e.data?.data?.OnsiteData?.OutTransportDate ? dayjs(e.data?.data?.OnsiteData?.OutTransportDate).format('YYYY-MM-DD') : null}</td>
            <td className='pl-3'>{e.data?.data?.OnsiteData?.OUTActiveTransportCode} {e.data?.data?.OnsiteData?.OUTScheduleDescription}</td>
          </tr>
          <tr>
            <td>Status:</td>
            <td className='pl-2'>
              <Tag className='text-xs' color={COLORS.seatStatus[e.data?.data?.OnsiteData?.Status]?.tagColor}>
                {e.data?.data?.OnsiteData?.Status}
              </Tag>
            </td>
          </tr>
        </table>
      </div>,
    },)
  }
  if(e.data?.data?.OffSiteStatus?.length > 0){
    const reducedData = ReduceDateList(e.data?.data?.OffSiteStatus)
    items.push({
      key: '3',
      label: `Off Site`,
      children: <div className='pt-4'>
        <Table 
          data={reducedData} 
          columns={SiteCol} 
          pager={reducedData?.length > 20}
          tableClass='border px-2 overflow-hidden rounded-ot max-h-[300px]'
          containerClass='pb-2 shadow-none pl-0 pr-0'
        />
      </div>,
    },)
  }
  
  return(
    <div className='bg-white rounded-ot shadow-md border p-2'>
      <Tabs items={items} type='card'/>
    </div>
  )
}

export default WarningDetail