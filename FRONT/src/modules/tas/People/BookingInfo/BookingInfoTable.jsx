import React, { memo, useCallback } from 'react'
import DataGrid, {
  Column,
  Grouping,
  Export,
  Selection,
  Paging,
  Pager,
} from 'devextreme-react/data-grid';
import { jsPDF } from 'jspdf';
import { exportDataGrid } from 'devextreme/pdf_exporter';
import dayjs from 'dayjs';
import logo from 'assets/images/logo_mn_collaboration.png'

const exportFormats = ['pdf'];

const BookingInfoTable = memo(({data}) => {

  const onExporting = useCallback((e) => {
    const doc = new jsPDF();
    exportDataGrid({
      jsPDFDocument: doc,
      component: e.component,
      margin: {
        right: 10,
        left: 10,
        top: 20, 
        bottom: 10
      },
      customizeCell: ({gridCell, pdfCell}) => {
        if(gridCell.rowType === 'group' && gridCell.value){
          pdfCell.font.style = 'normal'
          pdfCell.textColor = 'blue'
          pdfCell.text = gridCell.value
        }
      },
    }).then(() => {
      const pageCount = doc.internal.getNumberOfPages();
      const currentDate = dayjs().format('YYYY-MM-DD HH:mm')
      for (let i = 1; i <= pageCount; i++) {
        doc.addImage(logo, 'png', 140, 8, 60, 10)
        doc.setPage(i);
        doc.setFontSize(10);
        doc.setTextColor('#888')
        doc.text(`Exported on: ${currentDate}`, 10, doc.internal.pageSize.height - 7);
        doc.text(`Page ${i} of ${pageCount}`, doc.internal.pageSize.width - 30, doc.internal.pageSize.height - 7);
      }
      doc.save(`Booking-Info_${currentDate}.pdf`);
    });

    e.cancel = true
  },[]);

  return (
    <div className={`px-2 bg-white rounded-ot shadow-md`}>
      <DataGrid
        dataSource={data}
        keyExpr="TransportId"
        allowColumnReordering={true}
        showColumnLines={false}
        onExporting={onExporting}
        className='max-h-[calc(100vh-325px)]'
      >
        <Selection mode="multiple" />
        <Grouping autoExpandAll={true} />
        <Paging defaultPageSize={100} />
        <Export enabled={true} formats={exportFormats} allowExportSelectedData={true} />

        <Column 
          dataField="Fullname"
          dataType="string"
          groupIndex={0} 
          groupCellRender={(e) => (
            <span>
              <span className=' text-blue-700 font-normal'>{e.value}</span> {e.groupContinuesMessage ? `- ${e.groupContinuesMessage}` : ''}
            </span>
          )}
        />
        <Column dataField="EventDate" dataType="string" cellRender={({value}) => <div>{value ? dayjs(value).format('YYYY-MM-DD') : null}</div>}/>
        <Column dataField="Direction" dataType="string" />
        <Column dataField="Description" dataType="string" />
        <Column dataField="TransportCode" dataType="string" />
        <Pager
          visible={true}
          allowedPageSizes={[100]} 
          showPageSizeSelector={true}
          showNavigationButtons={true} 
          showInfo={true}
          displayMode={'full'}
          infoText={(currentPage, totalPage, totalDataCount ) => `${totalDataCount} items in ${totalPage} pages`}
        />
      </DataGrid>
    </div>
  )
}, (prev, next) => JSON.stringify(prev.data) === JSON.stringify(next.data)  )

export default BookingInfoTable
