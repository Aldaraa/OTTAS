import { Drawer } from 'antd'
import React, { useCallback, useEffect, useState } from 'react'
import { Column, DataGrid, ColumnFixing } from 'devextreme-react/data-grid';
import axios from 'axios';
import dayjs from 'dayjs';
import { exportDataGrid } from 'devextreme/excel_exporter'
import { saveAs } from 'file-saver-es'
import { Workbook } from 'exceljs'

function GroupDetail({ selectedCell, currentDate}) {
  const [ detailLoading, setDetailLoading ] = useState(false)
  const [ detailData, setDetailData ] = useState([])
  const [ showDrawer, setShowDrawer ] = useState(false)

  useEffect(() => {
    if(selectedCell){
      getDetail(selectedCell)
    }
  },[selectedCell])

  const getDetail = useCallback((e) => {
    setShowDrawer(true)
    setDetailLoading(true)
    axios({
      method: 'post',
      url: `/tas/dashboardtransportadmin/transportgroup/employees`,
      data: {
        currentDate: currentDate.format('YYYY-MM-DD'),
        daynum: e.DayName,
        groupMasterId: e.Id,
      }
    }).then((res) => {
      setDetailData(res.data)
    }).catch(() => {

    }).finally(() => {
      setDetailLoading(false)
    })
  },[currentDate])

  const cellPrepared = useCallback((e) => {
    if(e.rowType === 'data' ){
      if(e.column.dataField === 'Status'){
        if(e.value === 'Confirmed'){
          e.cellElement.style.color = '#52c41a'
          e.cellElement.style.fontSize = '12px'
        }else{
          e.cellElement.style.color = '#d46b08'
          e.cellElement.style.fontSize = '12px'
        }
      }
      if(e.column.dataField === 'FullName'){
        e.cellElement.style.color = '#1269f5'
        e.cellElement.classList.add('tr-group-cell')
      }
    }
  },[])

  const onExporting = useCallback((e) => {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('Data');
    exportDataGrid({
      component: e.component,
      worksheet,
      keepColumnWidths: true,
    }).then(() => {
      workbook.xlsx.writeBuffer().then((buffer) => {
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${currentDate.format('YYYY-MM-DD')}_${selectedCell?.Description}.xlsx`);
      });
    });
  },[currentDate, selectedCell])

  const onCellClick = (e) => {
    console.log('eeee', e);
    if(e.rowType === 'data' && e.column.dataField === 'FullName'){
      window.open(`/tas/people/search/${e.data.EmployeeId}/flight`, "_blank", "noreferrer")
    }
  }

  return (
    <Drawer
      title={<div className='font-normal text-sm flex justify-between'>
        <div>
          <div className='font-bold'>{selectedCell?.Description}</div>
          <div className='text-gray-500'>{dayjs(currentDate).set('d', selectedCell?.rowIndex+1).format('YYYY-MM-DD ddd')}</div>
        </div>
        <div className='font-medium text-gray-500'>result: {detailData?.length}</div>
      </div>}
      open={showDrawer}
      onClose={() => setShowDrawer(false)}
      loading={detailLoading}
      width={700}
    >
      <DataGrid 
        id="gridContainer"
        dataSource={detailData}
        keyExpr="Id"
        className='border rounded-ot overflow-hidden'
        allowColumnReordering={true}
        allowColumnResizing={true}
        columnAutoWidth={true}
        showBorders={true}
        hoverStateEnabled={true}
        onCellPrepared={cellPrepared}
        onExporting={onExporting}
        onCellClick={onCellClick}
        export={{enabled: true}}
        rowAlternationEnabled={true}
      >
        <ColumnFixing enabled={true} />
        <Column dataField='SAPID' fixed/>
        <Column dataField='FullName' fixed/>
        <Column dataField='DepartmentName' caption={'Department'}/>
        <Column dataField='ActiveTransportCode' caption={'Code'}/>
        <Column dataField='ScheduleCode' caption={'Description'}/>
        <Column dataField='Status'/>
      </DataGrid>
    </Drawer>
  )
}

export default GroupDetail