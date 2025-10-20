import axios from 'axios';
import CustomStore from 'devextreme/data/custom_store';
import React, { useRef, useState } from 'react'
import DataGrid, {
  RemoteOperations,
  Column,
  Grouping,
  GroupPanel,
  Summary,
  GroupItem,
  Paging,
  Pager
} from "devextreme-react/data-grid";
import { Radio, Space } from 'antd';

function isNotEmpty(value) {
  return value !== undefined && value !== null && value !== '';
}

function Test() {
  const [ store, setStore ] = useState(new CustomStore({
    key: 'Id',
    load: (loadOptions) => {
      // console.log('load options', loadOptions);
      dataGrid.current?.instance.beginCustomLoading();
      let params = '?';
      [
        'skip',
        'take',
        'requireTotalCount',
        'requireGroupCount',
        'sort',
        'filter',
        'totalSummary',
        'group',
        'groupSummary',
      ].forEach((i) => {
        if (i in loadOptions && isNotEmpty(loadOptions[i])) { params += `${i}=${JSON.stringify(loadOptions[i])}&`; }
      });
      params = params.slice(0, -1);

      return axios({
        method: 'post',
        url: `tas/requestdocument/documentlist${params}`,
        data: {
          model: loadOptions.filter ? 
          loadOptions.filter :
          {
            startDate: "",
            endDate: "",
            Firstname: "",
            documentType: "",
            employerId: null,
            assignedEmployeeId: null,
            requestedEmployeeId: null,
            requestDocumentSearchCurrentStep: "",
            lastModifiedDate: "",
          },
          group: loadOptions.group,
          pageIndex: loadOptions.skip/loadOptions.take,
          pageSize: loadOptions.take
        }
      }).then((res) => {
        return {
          data: res.data.data,
          totalCount: res.data.totalcount,
        }
      }).finally(() => dataGrid.current?.instance.endCustomLoading())
    }
  }))

  const dataGrid = useRef(null)

  return (
    <>
      <DataGrid
        ref={dataGrid}
        dataSource={store}
        remoteOperations={true}
        height={420}
      >
        <RemoteOperations groupPaging={true} />
        <Grouping autoExpandAll={true} />
        <GroupPanel visible={true} />

        <Column dataField="Id" dataType="number" width={75} />
        <Column
          dataField="DaysAway"
          caption="DaysAway"
          width={150}
        />
        <Column
          dataField="Id"
          caption="Request #"
          width={150}
        />
        <Column
          dataField="CurrentStatus"
          caption="CurrentStatus"
          width={120}
        />
        <Column dataField="Employer" caption="EmployerName" />
        <Column
          dataField="DocumentType"
          caption="Document Type"
          dataType="date"
          format="yyyy-MM-dd"
          width={100}
        />
        <Column
          dataField="DocumentTag"
          caption="Document Tag"
          format="currency"
          width={100}
        />
        <Paging defaultPageSize={10} />
        <Pager
          showPageSizeSelector={true}
          allowedPageSizes={[10, 300, 500, 800, 1000]}
        />
        <Summary>
          <GroupItem column="Id" summaryType="count" />
        </Summary>
      </DataGrid>
      <Radio.Group>
        <Space>
          <Radio value={1} >
            <p className='font-bold'>sdfasdf</p>
            <div className='border rounded-ot px-2 py-1 flex flex-col bg-white'>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
            </div>
          </Radio>
          <Radio value={2}>
            <p className='font-bold'>sdfasdf</p>
            <div className='border rounded-ot px-2 py-1 flex flex-col bg-white'>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
            </div>
          </Radio>
          <Radio value={3}>
            <p className='font-bold'>sdfasdf</p>
            <div className='border rounded-ot px-2 py-1 flex flex-col bg-white'>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
            </div>
          </Radio>
          <Radio value={4}>
            <p className='font-bold'>sdfasdf</p>
            <div className='border rounded-ot px-2 py-1 flex flex-col bg-white'>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
              <div>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</div>
            </div>
          </Radio>
        </Space>
      </Radio.Group>
    </>
  )
}

export default Test