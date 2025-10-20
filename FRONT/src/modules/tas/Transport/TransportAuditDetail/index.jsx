import React, { useContext, useEffect, useState } from 'react'
import { useLoaderData, useParams } from 'react-router-dom'
import { Button, Form, PeopleProfile } from 'components'
import { exportDataGrid } from 'devextreme/excel_exporter'
import { saveAs } from 'file-saver-es'
import { AuthContext } from 'contexts'
import { Workbook } from 'exceljs'
import { DatePicker, notification } from 'antd'
import axios from 'axios'
import dayjs from 'dayjs'

function TransportAuditDetail() {
  const data = useLoaderData()
  const [ loading, setLoading ] = useState(false)
  const { state } = useContext(AuthContext);
  const { empId } = useParams()
  const [ form ] = Form.useForm()
  const [ api, contextHolder] = notification.useNotification();

  const handleSearch = (values) => {
    const dates = {
      startDate: dayjs(values.fromDate).format('YYYY-MM-DD'),
      endDate: dayjs(values.toDate).format('YYYY-MM-DD'),
    }
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/audit/transportaudit/${empId}/${dates.startDate}/${dates.endDate}`,
      responseType: 'blob',
    }).then((res) => {
      if(res.status === 200){
        const url = window.URL.createObjectURL(res.data); 
        const a = document.createElement('a');
        a.href = url;
        a.download = `TAS_TRANSPORT_AUDIT_${dayjs().format('YYYY-MM-dd_HH-mm-ss')}.xlsx`
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
      }else{
        api.error({
          message: 'Audit data not found',
          duration: 5,
          // description: ''
        });
      }
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const columns = [
    {
      label: 'Owner', 
      name: 'RoomOwner', 
      alignment: 'center', 
      width: '55px',
      cellRender: (e) => (
        <span className='text-[12px] px-1'>
          {
            e.value ? 
            <i className="dx-icon-home text-green-500"></i> 
            :
            <i className="dx-icon-home text-gray-400 text-[14px]"></i>
          }
        </span>
      )
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
    },
    {
      label: 'Type',
      name: 'RoomType',
    },
    {
      label: 'Camp',
      name: 'Camp',
      // width: '80px',
      alignment: 'left'
    },
    {
      label: 'Date In',
      name: 'DateIn',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Last Night',
      name: 'LastNight',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd')}</div>
      )
    },
    {
      label: 'Days',
      name: 'Days',
      alignment: 'left',
    },
  ]

  const onExporting = (e) => {
    const workbook = new Workbook();
    const worksheet = workbook.addWorksheet('Companies');
  
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
          // if (gridCell.column.dataField === 'Phone') {
          //   excelCell.value = parseInt(gridCell.value, 10);
          //   excelCell.numFmt = '[<=9999999]###-####;(###) ###-####';
          // }
          // if (gridCell.column.dataField === 'Website') {
          //   excelCell.value = { text: gridCell.value, hyperlink: gridCell.value };
          //   excelCell.font = { color: { argb: 'FF0000FF' }, underline: true };
          //   excelCell.alignment = { horizontal: 'left' };
          // }
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
        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Companies.xlsx');
      });
    });
  }

  return (
    <div className='bg-white p-4 rounded-ot '>
      <PeopleProfile profileData={data}/>
      <div className='text-lg font-bold mb-3 mt-5'>Transport Audit</div>
      <Form 
        className='flex gap-5 mb-3'
        layout='horizontal'
        noLayoutConfig={true}
        onFinish={handleSearch}
        form={form}
        initValues={{fromDate: dayjs().subtract(1, 'day'), toDate: dayjs()}}
      >
        <Form.Item className='flex items-center mb-0' name='fromDate' label='From'>
          <DatePicker/>
        </Form.Item>
        <Form.Item className='flex items-center mb-0' name='toDate' label='To'>
          <DatePicker/>
        </Form.Item>
        <Form.Item className='mb-0'>
          <Button type='primary' onClick={() => form.submit()} loading={loading}>Get</Button>
        </Form.Item>
      </Form>
      {contextHolder}
    </div>
  )
}

export default TransportAuditDetail