import { DatePicker, Drawer } from 'antd'
import axios from 'axios'
import { Button, Table } from 'components'
import { AuthContext } from 'contexts'
import dayjs from 'dayjs'
import { Workbook } from 'exceljs'
import React, { useContext, useEffect, useState } from 'react'
import { twMerge } from 'tailwind-merge'
import { saveAs } from 'file-saver-es'
import { exportDataGrid } from 'devextreme/excel_exporter'

function AuditTable({tablename, open=false, onClose, title='', record=null, recordName=null, ...restProps}) {
  const [ startDate, setStartDate ] = useState(dayjs().subtract(6, 'months'))
  const [ endDate, setEndDate ] = useState(dayjs())
  const [ data, setData ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const { state } = useContext(AuthContext)

  useEffect(() => {
    getData()
  },[])

  useEffect(() => {
    if(record?.Id){
      setLoading(true)
      axios({
        method: 'get',
        url: `tas/audit/masteraudit/${tablename}?recordId=${record.Id}&${startDate ? `startdate=${dayjs(startDate).format('YYYY-MM-DD')}` : ''}${endDate ? `&enddate=${dayjs(endDate).format('YYYY-MM-DD')}` : ``}`
      }).then((res) => {
        setData(res.data)
      }).catch((err) => {
        
      }).then(() => setLoading(false))
    }else{
      axios({
        method: 'get',
        url: `tas/audit/masteraudit/${tablename}?${startDate ? `startdate=${dayjs(startDate).format('YYYY-MM-DD')}` : ''}${endDate ? `${startDate ? `&enddate=${dayjs(endDate).format('YYYY-MM-DD')}` : `enddate=${dayjs(endDate).format('YYYY-MM-DD')}`}` : ''}`
      }).then((res) => {
        setData(res.data)
      }).catch((err) => {
        
      }).then(() => setLoading(false))
    }
  },[record])

  const getData = () => {
    setLoading(true)
    if(record?.Id){
      axios({
        method: 'get',
        url: `tas/audit/masteraudit/${tablename}?recordId=${record.Id}&${startDate ? `startdate=${dayjs(startDate).format('YYYY-MM-DD')}` : ''}${endDate ? `&enddate=${dayjs(endDate).format('YYYY-MM-DD')}` : ``}`
      }).then((res) => {
        setData(res.data)
      }).catch((err) => {
        
      }).then(() => setLoading(false))
    }else{
      axios({
        method: 'get',
        url: `tas/audit/masteraudit/${tablename}?${startDate ? `startdate=${dayjs(startDate).format('YYYY-MM-DD')}` : ''}${endDate ? `${startDate ? `&enddate=${dayjs(endDate).format('YYYY-MM-DD')}` : `enddate=${dayjs(endDate).format('YYYY-MM-DD')}`}` : ''}`
      }).then((res) => {
        setData(res.data)
      }).catch((err) => {
        
      }).then(() => setLoading(false))
    }
  }

  const renderValue = (value, key) => {
    switch (key) {
      case 'Dob': return value ? dayjs(value).format('YYYY-MM-DD') : ''
      case 'CommenceDate': return value ? dayjs(value).format('YYYY-MM-DD') : ''
      case 'Gender': return value ? 'Male' : 'Female'
      case 'EmployerId': return state.referData.employers.find((item) => value === item.Id)?.label
      case 'LocationId': return state.referData.locations.find((item) => value === item.Id)?.label
      case 'NationalityId': return state.referData.nationalities.find((item) => value === item.Id)?.label
      case 'PeopleTypeId': return state.referData.peopleTypes.find((item) => value === item.Id)?.label
      case 'PositionId': return state.referData.positions.find((item) => value === item.Id)?.label
      case 'RosterId': return state.referData.rosters.find((item) => value === item.Id)?.label
      case 'StateId': return state.referData.states.find((item) => value === item.Id)?.label
      case 'CostCodeId': return state.referData?.costCodes.find((item) => value === item.Id)?.label
      case 'DepartmentId': return state.referData.departments.find((item) => value === item.Id)?.Name
      default: return value
    }
  }

  const column = [
    {
      label: 'Changed User',
      name: 'Username',
    },
    {
      label: 'Changed Date',
      name: 'DateCreated',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD')}</div>
      )
    },
  ]

  const onExporting = (e) => {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet(title);

    worksheet.columns = [
      { width: 5 }, { width: 30 }, { width: 25 }, { width: 15 }, { width: 25 }, { width: 40 },
    ];

    exportDataGrid({
      component: e.component,
      worksheet,
      keepColumnWidths: false,
      topLeftCell: { row: 2, column: 2 },
      customizeCell: ({ gridCell, excelCell }) => {
        if (gridCell.rowType === 'data') {
       
        }
        if (gridCell.rowType === 'group') {
          excelCell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'BEDFE6' } };
        }
        if (gridCell.rowType === 'totalFooter' && excelCell.value) {
          excelCell.font.italic = true;
        }
      },
    }).then(() => {
      workbook.xlsx.writeBuffer().then((buffer) => {
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${title}.xlsx`);
      });
    });
  }

  return (
    <Drawer open={open} onClose={onClose} title={<div>{title} {recordName}</div>} width={700} {...restProps}>
      <Table
        data={data}
        columns={column}
        allowColumnReordering={false}
        loading={loading}
        containerClass='shadow-none border'
        keyExpr='Id'
        tableClass='max-h-[calc(100vh-120px)]'
        renderDetail={{
          enabled: true,
          autoExpandAll: true,
          component: ({data}) => {
            const oldData = JSON.parse(data.data?.OldValues)
            const newData = JSON.parse(data.data.NewValues)
            let keys = []
            if(newData){
              keys = Object.keys(newData)
            }
            return (
              <div className='bg-white px-3'>
                <table className='border'>
                <thead>
                  <tr>
                    <th className='p-1 px-2 border'>Field</th>
                    <th className='p-1 px-2 border'>Previous value</th>
                    <th className='p-1 px-2 border'>New value</th>
                  </tr>
                </thead>
                <tbody>
                  {
                    keys.map((key, i) => (
                      <tr key={`row-${i}`} className={twMerge('hover:bg-blue-100 transition-all', i%2 === 0 ? 'bg-gray-100' :'')}>
                        <td className='p-1 px-2 border'>{key}</td>
                        <td className='p-1 px-2 border'>{oldData ? renderValue(oldData[key], key) : null}</td>
                        <td className='p-1 px-2 border'>{newData ? renderValue(newData[key], key) : null}</td>
                      </tr>
                    ))
                  }
                </tbody>
                </table>
              </div>
            )
          }
        }}
        // export={{enabled: true}}
        // onExporting={onExporting}
        toolbar={[
          {
            location: 'before',
            render: (e) =>
              <div className='flex items-center gap-2' >
                <DatePicker placeholder='Start Date' value={startDate} onChange={(e) => setStartDate(e)}/>
                -
                <DatePicker placeholder='End Date' value={endDate} onChange={(e) => setEndDate(e)}/>
                <Button htmlType='button' disabled={loading} onClick={getData}>Search</Button>
              </div>
          }
        ]}
      />
    </Drawer>
  )
}

export default AuditTable