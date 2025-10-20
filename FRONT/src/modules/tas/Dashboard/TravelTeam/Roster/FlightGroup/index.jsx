import React, { useCallback, useContext, useEffect, useRef, useState } from 'react'
import { DatePicker, TreeSelect, Drawer, Tag } from 'antd'
import { Column, DataGrid, Toolbar, Item, ColumnFixing, ColumnChooser } from 'devextreme-react/data-grid';
import { Button } from 'components';
import { AuthContext } from 'contexts';
import dayjs from 'dayjs';
import axios from 'axios';
import { SearchOutlined } from '@ant-design/icons';
import { exportDataGrid } from 'devextreme/excel_exporter'
import { saveAs } from 'file-saver-es'
import { Workbook } from 'exceljs'
import GroupDetail from './GroupDetail';

const colors = {max: '#59d27a', mid: '#ecff6d', min: '#f89c98'}

const getColor = (value, min, max) => {
  if(value >= max ){
    return colors.max
  }else if(value > min && max > value){
    return colors.mid
  }else{ 
    return colors.min
  }
}


function groupByDayName(data) {
  return data.reduce((result, item) => {
    if (!result[item.DayName]) {
      result[item.DayName] = [];
    }
    result[item.DayName].push(item);
    return result;
  }, {});
}

const weekDays = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']

function FlightGroup() {
  const [ tableData, setTableData ] = useState([])
  const [ columns, setColumns ] = useState([])
  const [ currentDate, setCurrentDate ] = useState(dayjs())
  const [ selectedDepartments, setSelectedDepartments ] = useState([])
  const [ loading, setLoading ] = useState(false)
  const [ selectedCell, setSelectedCell ] = useState(null)
  const [ ranges, setRanges ] = useState({min: 0, mid: 0})
  const { state } = useContext(AuthContext)
  const dataGrid = useRef(null)

  useEffect(() => {
    getData()
  },[])

  const getData = useCallback(() => {
    setLoading(true)
    dataGrid.current?.instance.beginCustomLoading();
    axios({
      method: 'post',
      url: `tas/dashboardtransportadmin/transportgroup`,
      data: {
        currentDate: currentDate.format('YYYY-MM-DD'),
        departmentIds: selectedDepartments
      }
    }).then((res) => {
      calculateData(res.data)
    }).finally(() => {
      dataGrid.current?.instance.endCustomLoading()
      setLoading(false)
    }
    )
  },[currentDate, selectedDepartments])

  const calculateData = async (data) => {
    try {
      const values = data.filter((item) => item.Id !== 0).map((item) => item.Count)
      const max = Math.max(...values)
      const minRange = Math.floor(max/3)
      const maxRange = minRange*2
      setRanges({min: minRange, mid: maxRange})
      const sortedData = data.sort((a, b) => b.Count - a.Count)
      const groupedData = groupByDayName(sortedData)
      let final = []
      let cols = {}
      weekDays.forEach((key) => {
        let dayData = {DayName: key}
        let sum = 0
        groupedData[key]?.map((item) => {
          dayData[item.Id] = {
            ...item,
            color: item.Id !== 0 ? getColor(item.Count, minRange, maxRange) : null
          };
          cols[item.Id] = item.Description;
          sum += item.Count
        })
        dayData['Total'] = sum
        final.push(dayData)
      })
      setColumns(Object.keys(cols).map((item) => ({name: item, caption: cols[item]})))
      setTableData(final)
    } catch (e){
      console.log('error',e);
    }
  }

  const onCellPrepared = useCallback((e) => {
    if(e.rowType === 'data'){
      e.cellElement.style.backgroundColor = e.value?.color
      if(typeof e.value === 'object'){
        e.cellElement.textContent = e.value.Count
        e.cellElement.style.color = '#1269f5'
        e.cellElement.classList.add('tr-group-cell')
      }
    }
  },[])

  const onClickCell = useCallback((e) => {
    if(typeof e.value === 'object'){
      setSelectedCell({...e.value, rowIndex: e.rowIndex})
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
        saveAs(
          new Blob([buffer], { type: 'application/octet-stream' }), 
          `${currentDate.startOf('isoWeek').format('YYYY MMM DD')}-${currentDate.endOf('isoWeek').format('YYYY MMM DD')}_Person_by_Transport_Group.xlsx`
        );
      });
    });
  },[currentDate])

  return (
    <div className='pb-5'>
      <div className='flex justify-between mb-3'>
        <div className='font-bold text-lg'>Person by Transport Group Week</div>
        <div className='flex gap-3'>
          <div className='self-center text-gray-500 text-xs font-medium'>
            {currentDate.startOf('isoWeek').format('YYYY MMM DD')} <span className='font-normal'>—</span> {currentDate.endOf('isoWeek').format('YYYY MMM DD')}
          </div>
          <DatePicker
            picker='week'
            width={150}
            value={currentDate}
            onChange={(e) => setCurrentDate(e)}
          />
          <TreeSelect
            treeData={[...state.referData.reportDepartments]}
            fieldNames={{label: 'Name', value: 'Id', children: 'ChildDepartments'}}
            filterTreeNode={(input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase())}
            popupMatchSelectWidth={false}
            placeholder='Filter by departments'
            multiple
            allowClear
            showSearch
            style={{width: 400}}
            onChange={(e) => setSelectedDepartments(e)}
          />
          <Button icon={<SearchOutlined/>} type='primary' onClick={getData} loading={loading}/>
        </div>
      </div>
      <div className='border rounded-ot px-2 pt-0'>
        <DataGrid
          ref={dataGrid}
          id="employees"
          dataSource={tableData}
          keyExpr="DayName"
          columnAutoWidth={true}
          showRowLines={true}
          width="100%"
          showBorders={false}
          showColumnLines={true}
          columnChooser={{enabled: true, allowSearch: true, mode: 'select', sortOrder: 'asc'}}
          onCellPrepared={onCellPrepared}
          hoverStateEnabled
          onExporting={onExporting}
          export={{enabled: true}}
          onCellClick={onClickCell}
        >
          <Column dataField={'DayName'} width={100} fixed={true}/>
          <Column dataField={'Total'} width={60} fixed={true}/>
          {
            columns.map((col, i) => (
              col.name !== 'DayName' ?
              <Column 
                key={i}
                dataField={col.name}
                caption={col.caption}
                alignment={'right'}
              />
              : null
            ))
          }
          <Toolbar>
            <Item location='before'>
              <div className="informer">
                <div className='flex gap-3 items-center text-xs'>
                  <div className='flex items-center gap-1'>
                    <div className={`w-10 h-3 border`} style={{backgroundColor: colors.max}}></div>
                    <div>{`n >= ${ranges.mid}`}</div>
                  </div>
                  <div className='flex items-center gap-1'>
                    <div className={`w-10 h-3 border`} style={{backgroundColor: colors.mid}}></div>
                    <div>{`${ranges.min} > n < ${ranges.mid}`}</div>
                  </div>
                  <div className='flex items-center gap-1'>
                    <div className={`w-10 h-3 border`} style={{backgroundColor: colors.min}}></div>
                    <div>{`n <= ${ranges.min}`}</div>
                  </div>
                </div>
              </div>
            </Item>
            <Item name="columnChooserButton" />
            <Item name="exportButton" />
          </Toolbar>
        </DataGrid>
      </div>
      <GroupDetail 
        selectedCell={selectedCell}
        currentDate={currentDate}
      />
    </div>
  )
}

export default FlightGroup