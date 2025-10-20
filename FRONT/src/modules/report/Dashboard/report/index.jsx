import React, { useEffect, useState } from 'react'
import { FileDoneOutlined, HomeOutlined, GlobalOutlined, FileTextOutlined } from '@ant-design/icons'
import { GiMining } from 'react-icons/gi'
import { useNavigate } from 'react-router-dom'
import { GiMongolia } from 'react-icons/gi'
import { TbCalendarDue, TbFileDescription, TbReport, TbReportSearch } from 'react-icons/tb'
import NumberFormat from 'utils/numberformat'

function Report({data=null, reportTotalValues=[], employeeWeekData=[], restprops}) {
  const [ selectedLegend, setSelectedLegend ] = useState(null)
  const navigate = useNavigate()

  return (
    <div className='bg-white rounded-ot shadow-md p-5 col-span-12'>
      <div className='mb-5 text-2xl font-bold'>Report</div>
      <div className=' grid grid-cols-12 gap-4'>
        <div className='col-span-12 grid grid-cols-12 2xl:grid-cols-10 gap-4'>
          <div className='col-span-12 lg:col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center'>
            <div>
              <div className='font-medium'>All Report Template</div>
              <div className='font-semibold text-xl'>{NumberFormat(data?.ALLReportTemplateCount)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <TbFileDescription style={{fontSize:'30px', color: 'white'}}/>
            </div>
          </div>
          <div className='col-span-12 lg:col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center'>
            <div>
              <div className='font-medium'>All Report Schedule</div>
              <div className='font-semibold text-xl'>{NumberFormat(data?.ALLReportScheduleCount)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <TbReport size={30} color='white'/>
            </div>
          </div>
          <div className='col-span-12 lg:col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center'>
            <div>
              <div className='font-medium'>Active Report Template</div>
              <div className='font-semibold text-xl'>{NumberFormat(data?.ActiveReportTemplateCount)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <FileDoneOutlined style={{fontSize:'25px', color: 'white'}}/>
            </div>
          </div>
          <div className='col-span-12 lg:col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center transition-all'>
            <div>
              <div className='font-medium'>Active Report Schedule</div>
              <div className='flex items-end gap-2 font-semibold text-xl'>{NumberFormat(data?.ActiveReportScheduleCount)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <TbReportSearch style={{fontSize:'30px', color: 'white'}}/>
            </div>
          </div>
          {/* hover:border-[#e57200] */}
          <div className='col-span-12 lg:col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center transition-all'>
            <div>
              <div className='font-medium'>Today Report Schedule</div>
              <div className='flex items-end gap-2 font-semibold text-xl'>{NumberFormat(data?.TodayReportScheduleCount)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <TbCalendarDue style={{fontSize:'30px'}} color='white'/>
            </div>
          </div>
        </div>
        <div className='col-span-12 grid grid-cols-12'>
          <div className='col-span-12 '>
            <div className='my-5 font-bold'>Report Types</div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Report