import React, { forwardRef, useContext, useEffect, useState } from 'react'
import { Table, Button, SearchPeopleForm } from 'components';
import axios from 'axios';
import { Form as AntForm } from 'antd';
import { useLocation } from 'react-router-dom';
import { AuthContext } from 'contexts';
import CustomStore from 'devextreme/data/custom_store';
import isArray from 'lodash/isArray';
import { CloseOutlined } from '@ant-design/icons';

const PeopleSelectionTable = forwardRef(({ onSelect, onReturn, selectedRowsData=[], max=100, className='', columns=[], searchDefaultValues={}, hideFields=[] }, ref) => {
  const routeLocation = useLocation();
  const [ store ] = useState(new CustomStore({
    key: 'Id',
    load: (loadOptions) => {
      let params = '';
      if(loadOptions.sort?.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      if(loadOptions.filter && !isArray(loadOptions.filter)){
        ref.current.instance.beginCustomLoading();
        return axios({
          method: 'post',
          url: `tas/employee/search${params}`,
          data: {
            model: {
              ...loadOptions.filter,
              ...searchDefaultValues,
            },
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => ref.current.instance.endCustomLoading())
      }
      else if(!isArray(loadOptions.filter) && typeof loadOptions.filter === 'undefined'){
        ref.current.instance.beginCustomLoading();
        return axios({
          method: 'post',
          url: `tas/employee/search${params}`,
          data: {
            model: {
              CampId: '',
              DepartmentId: '',
              Firstname: "",
              FlightGroupMasterId: "",
              Lastname: "",
              LocationId: "",
              NRN: "",
              roomNumber: "",
              RoomTypeId: "",
              costCodeId: "",
              employerId: "",
              futureBookingId: "",
              group: "",
              id: "",
              peopleTypeId: "",
              roomAssignment: "",
              rosterId: "",
              sapId: "",
              mobile: "",
              hasRoom: "",
              futureBooking: "",
              ...searchDefaultValues, 
            },
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => ref.current.instance.endCustomLoading())
      }
    }
  }))

  const { state } = useContext(AuthContext)
  const [ searchForm ] = AntForm.useForm()

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
  },[routeLocation, searchForm])

  const handleSearch = (values) => {
    ref.current.instance.filter(values)
  }

  const handleSelect = (e) => {
    if(onSelect){
      if(e.selectedRowKeys.length > max){
        let deselectRows = e.selectedRowKeys.slice(max)
        ref.current.instance.deselectRows(deselectRows)
      }
      if(onSelect){
        onSelect(e.selectedRowsData)
      }
    }
  }

  const handleReturn = () => {
    if(onReturn){
      onReturn()
    }
  }

  const handleClearAll = () => {
    ref.current.instance.clearSelection()
  }

  return (
    <div className={className}>
      <SearchPeopleForm
        containerClass='bg-white rounded-ot px-3 py-2 mb-3'
        onSearch={handleSearch}
        initialValues={searchDefaultValues}
        hideFields={hideFields}
      />
      <Table
        ref={ref}
        data={store}
        // pager={{showPageSizeSelector:  > 100}}
        columns={columns}
        allowColumnReordering={false}
        remoteOperations={true}
        id="room"
        showRowLines={true}
        selection={{mode: 'multiple', recursive: false, showCheckBoxesMode: 'always', allowSelectAll: true, selectAllMode: 'page'}}
        rowAlternationEnabled={true}
        onSelectionChanged={handleSelect}
        tableClass='max-h-[calc(100vh-315px)]'
        title={!state.userInfo?.ReadonlyAccess ? <div className='flex justify-between items-center py-2 gap-3 border-b'>
          <div className='ml-2'><span className='font-bold'>{selectedRowsData.length}</span> people selected {selectedRowsData.length === max && <span className='text-red-400'>(full)</span>}</div>
          <div className='flex items-center gap-3'>
            <Button onClick={handleClearAll} disabled={selectedRowsData.length < 1} icon={<CloseOutlined/>}>Clear All</Button>
            <Button onClick={handleReturn} disabled={selectedRowsData.length === 0}>Add Selection & Change</Button>
          </div>
        </div> : ''}
      />
    </div>
  )
})

export default PeopleSelectionTable