import React from 'react'
import { Workbook } from 'exceljs'
import { exportDataGrid } from 'devextreme/excel_exporter'
import { saveAs } from 'file-saver-es'
import { Table } from 'components'
import dayjs from 'dayjs'

const onExporting = (e) => {
  const workbook = new Workbook();
  const worksheet = workbook.addWorksheet('Skipped Rosters');

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
      saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Skipped_roster.xlsx');
    });
  });
}

const columns = [
  {
    label: 'Fullname',
    name: 'FullName',
  },
  {
    label: 'In Date',
    name: 'InTransportDate',
    cellRender: ({value}) => (
      <span>{dayjs(value).format('YYYY-MM-DD')}</span>
    )
  },
  {
    label: 'Out Date',
    name: 'OutTransportDate',
    cellRender: ({value}) => (
      <span>{dayjs(value).format('YYYY-MM-DD')}</span>
    )
  },
]

const SkippedRosters = ({data=[]}) => {

  return(
    <Table
      data={data}
      columns={columns}
      isGrouping={true}
      keyExpr={'EmployeeId'}
      tableClass='max-h-[600px]'
      autoExpandAllGroup={false}
      export={{enabled: true}}
      onExporting={onExporting}
    />
  )
}

export default SkippedRosters