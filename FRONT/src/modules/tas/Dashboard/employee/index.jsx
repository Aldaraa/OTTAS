import React, { useEffect, useState } from 'react'
import BarChart from './bar-chart'
import PieChart from './pie-chart'
import { TeamOutlined, HomeOutlined, GlobalOutlined, WomanOutlined, UserDeleteOutlined } from '@ant-design/icons'
import { GiMining } from 'react-icons/gi'
import { useNavigate } from 'react-router-dom'
import { GiMongolia } from 'react-icons/gi'
import NumberFormat from 'utils/numberformat'

function EmployeeSection({employee, empTotalValues=[], employeeWeekData=[], restprops}) {
  const [ selectedLegend, setSelectedLegend ] = useState(null)
  const navigate = useNavigate()

  const handleClickPeopleWidget = (state) => {
    setSelectedLegend(null)
    navigate('/tas/people/search', {state: state ? state : null})
  }

  return (
    <div className='bg-white rounded-ot shadow-md p-5 col-span-12'>
      <div className='mb-5 text-2xl font-bold'>Employee</div>
      <div className=' grid grid-cols-12 gap-4'>
        <div className='col-span-12 grid grid-cols-12 2xl:grid-cols-10 gap-4'>
          <div className='col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center cursor-pointer' onClick={() => handleClickPeopleWidget()}>
            <div>
              <div className='font-medium'>Total Employees</div>
              <div className='font-semibold text-xl'>{NumberFormat(employee?.TotalEmployees)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <TeamOutlined style={{fontSize:'25px', color: 'white'}}/>
            </div>
          </div>
          <div className='col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center cursor-pointer' onClick={() => handleClickPeopleWidget()}>
            <div>
              <div className='font-medium'>Mongolian Employees</div>
              <div className='font-semibold text-xl'>{NumberFormat(employee?.EmployeeMN)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <GiMongolia size={38} color='white'/>
            </div>
          </div>
          <div className='col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center cursor-pointer' onClick={() => handleClickPeopleWidget()}>
            <div>
              <div className='font-medium'>Foreign Employees</div>
              <div className='font-semibold text-xl'>{NumberFormat(employee?.EmployeeOtherCountries)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <GlobalOutlined style={{fontSize:'25px', color: 'white'}}/>
            </div>
          </div>
          <button type='button' className='col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center transition-all'  onClick={() => setSelectedLegend('off')}>
            <div>
              <div className='font-medium'>Off Site Employees</div>
              <div className='flex items-end gap-2 font-semibold text-xl'>{NumberFormat(employee?.OffSiteEmployees)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <HomeOutlined style={{fontSize:'25px', color: 'white'}}/>
            </div>
          </button>
          {/* hover:border-[#e57200] */}
          <button type='button' className='col-span-6 xl:col-span-4 2xl:col-span-2 border rounded-ot p-3 flex justify-between items-center transition-all' onClick={() => setSelectedLegend('on')}>
            <div>
              <div className='font-medium'>On Site Employees</div>
              <div className='flex items-end gap-2 font-semibold text-xl'>{NumberFormat(employee?.OnsiteEmployees)}</div> 
            </div>
            <div className='bg-[#FF8F4E] rounded-ot flex w-[50px] h-[50px] justify-center items-center'>
              <GiMining style={{fontSize:'25px'}} color='white'/>
            </div>
          </button>
        </div>
        <div className='col-span-12 grid grid-cols-12'>
          <div className='col-span-12 '>
            <div className='mb-5'>Next week data</div>
            <BarChart 
              height={280} 
              annotations={!selectedLegend && empTotalValues} 
              data={employeeWeekData} 
              selectedLegend={selectedLegend}
            />
            {/* <PieChart className='col-span-6' data={employee}/> */}
          </div>
        </div>
      </div>
    </div>
  )
}

export default EmployeeSection