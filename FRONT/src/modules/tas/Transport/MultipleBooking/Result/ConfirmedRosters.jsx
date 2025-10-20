import React from 'react'
import { Workbook } from 'exceljs'
import hexToHSL from 'utils/hexToHSL'
import { exportDataGrid } from 'devextreme/excel_exporter'
import { saveAs } from 'file-saver-es'
import { Table } from 'components'
import dayjs from 'dayjs'

const onExporting = (e) => {
  const workbook = new Workbook();
  const worksheet = workbook.addWorksheet('Confirmed Rosters');

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
        if(gridCell.column.name === 'Available'){
          const availableSeats = gridCell.data.Seats - gridCell.data.Confirmed - gridCell.data.OverBooked
          excelCell.value = availableSeats
          
          if(availableSeats <= 0){
            excelCell.font = {color: { argb: 'FFFF0000'}}
            }
          }
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
      saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Confirmed_rosters.xlsx');
    });
  });
}

const columns = [
  {
    label: 'Fullname',
    name: 'FullName',
    groupIndex: 0,
  },
  {
    label: 'Date',
    name: 'EventDate',
    width: 80,
    cellRender: (e) => (
      <span>{dayjs(e.value).format('YYYY-MM-DD')}</span>
    )
  },
  {
    label: 'Transport Code',
    name: 'TransportCode',
    width: 120,
  },
  {
    label: 'Transport Mode',
    name: 'TransportMode',
    width: 120,
  },
  {
    label: 'Dir',
    name: 'Direction',
    width: 50,
  },
  {
    label: 'Shift',
    name: 'ShiftCode',
    width: 50,
    cellRender: (e) => (
      <div className='text-center rounded ' style={{background: e.data.ShiftColorCode, color: hexToHSL(e.data.ShiftColorCode) > 60 ? 'black' : 'white'}}>
        {e.value}
      </div>
    )
  },
  {
    label: 'Description',
    name: 'Description',
  },
  {
    label: 'Seats',
    name: 'Seats',
  },
  {
    label: 'Available seats',
    name: 'Confirmed',
    cellRender: ({data}) => {
      const availableSeats = data.Seats-data.Confirmed-data.OverBooked
      return <div className={availableSeats <= 0 ? 'text-red-400' : 'text-green-400'}>{availableSeats}</div>
    }
  },
  // {
  //   label: 'Confirmed',
  //   name: 'Confirmed',
  // },
  // {
  //   label: 'Overbooked',
  //   name: 'OverBooked',
  // },
]

const ConfirmedRosters = ({data=[]}) => {
  return(
    <Table
      data={data}
      columns={columns}
      isGrouping={true}
      keyExpr={'EmployeeId'}
      autoExpandAllGroup={true}
      tableClass='max-h-[600px]'
      export={{enabled: true}}
      onExporting={onExporting}
      isGroupedCount={true}
    />
  )
}

export default ConfirmedRosters