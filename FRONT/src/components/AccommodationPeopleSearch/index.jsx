import React, { useRef, useState } from 'react'
import { Table, SearchPeopleForm, Button } from 'components';
import { CheckBox } from 'devextreme-react';
import { BsAirplaneFill, BsBriefcaseFill } from 'react-icons/bs'
import axios from 'axios';
import { Link } from 'react-router-dom';
import CustomStore from 'devextreme/data/custom_store';
import { GoPrimitiveDot } from 'react-icons/go';

function AccommodationPeopleSearch({selectType='button', onSelect, toLink, target='', onRowDblClick=null, defaultColumns=[], renderSelectCell, containerClass='', tableClass='', actionText='Select', ...restProps}) {
  const [ store ] = useState(new CustomStore({
    key: 'Id',
    load: (loadOptions) => {
      let params = '';
      if(loadOptions.sort.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      if(loadOptions.filter){
        dataGrid.current?.instance.beginCustomLoading();
        return axios({
          method: 'post',
          url: `tas/employee/searchaccommodation${params}`,
          data: {
            model: loadOptions.filter ? 
            loadOptions.filter :
            {
              CampId: '',
              DepartmentId: '',
              Firstname: "",
              FlightGroupMasterId: "",
              Lastname: "",
              LocationId: "",
              NRN: "",
              RoomId: "",
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
              Active: 1
            },
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        }).finally(() => dataGrid.current?.instance.endCustomLoading())
      }else{
        return new Promise((resolve) => {
          resolve({
            totalCount: 0,
            data: [],
          })
        })
      }
    }
  }))

  const dataGrid = useRef(null)

  const columns = [
    {
      label: 'Person #',
      name: 'Id',
      dataType: 'string',
      cellRender: ({value, data}) => (
        <div className='text-blue-500 hover:underline'>
          <Link to={`${data.Id}/roombooking`}>
            {value}
          </Link>
        </div>
      )
      // fixed: 'left',
    },
    {
      label: 'SAP ID #',
      name: 'SAPID',
      alignment: 'left',
      width: '90px',
      // fixed: 'left',
      visible: false,
    },
    {
      label: '',
      name: 'TodayOnsite',
      width: '50px',
      alignment: 'center',
      showInColumnChooser: false,
      allowSorting: true,
      // fixed: 'left',
      cellRender: ({value}) => (
        <CheckBox disabled iconSize={18} value={value}/>
      ),
      headerCellRender: (he, re) => (
        <BsBriefcaseFill />
      )
    },
    {
      label: 'Firstname',
      name: 'Firstname',
      dataType: 'string',
      // fixed: 'left',
      cellRender:({value, data}) => (
        <div className='flex items-center gap-[2px]'>
          <GoPrimitiveDot color={data?.Active === 1 ? 'lime' : 'lightgray'}/>
          <div>{value}</div>
        </div>
      )
    },
    {
      label: 'Lastname',
      name: 'Lastname',
      // fixed: 'left',
    },
    {
      label: 'Department',
      name: 'DepartmentName',
      dataType: 'string',
      width: '220px',
    },
    {
      label: 'Employer',
      name: 'EmployerName',
      dataType: 'string',
    },
    // {
    //   label: 'Roster',
    //   name: 'RosterName',
    //   dataType: 'string',
    //   width: '70px',
    // },
    {
      label: 'Resource Type',
      name: 'PeopleTypeName',
      dataType: 'string',
    },
    {
      label: 'Roster',
      name: 'RosterName',
      dataType: 'string',
      cellRender: (e) => (
        <div>{e.value}</div>
      )
    },
    {
      label: 'Room',
      name: 'RoomNumber',
      cellRender: (e) => (
        <span>{e.value}</span>
      )
    },
    {
      label: 'Gender',
      name: 'Gender',
      alignment: 'center',
      width:'70px',
      cellRender: (e) => (
        <span>{e.data.Gender === 1 ? 'M' : 'F'}</span>
      ),
      groupCellRender: (e) => (
        <span>{e.value === 1 ? 'M' : 'F'} {e.groupContinuesMessage ? `- ${e.groupContinuesMessage}` : ''}</span>
      )
    },
    {
      label: '',
      name: 'HasFutureTransport',
      width: '50px',
      showInColumnChooser: false,
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
      headerCellRender: (he, re) => (
        <BsAirplaneFill></BsAirplaneFill>
      )
    },
    {
      label: '',
      name: 'HasFutureRoomBooking',
      width: '50px',
      showInColumnChooser: false,
      cellRender: (e, r) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      ),
      headerCellRender: (he, re) => (
        <i className="dx-icon-home"></i>
      )
    },
  ]

  const handleSearch = (values) => {
    dataGrid.current?.instance.filter(values)
  }

  return (
    <div className={containerClass}>
      <div className='rounded-t-ot bg-white px-3 py-2'>
        <SearchPeopleForm
          containerClass='bg-white rounded-ot py-2'
          onSearch={handleSearch}
          initialValues={{Active: 1}}
          showClearButton={true}
        />
      </div>
      <Table 
        ref={dataGrid}
        data={store}
        columns={
          defaultColumns.length > 0 ? 
          columns.map((col) => (defaultColumns.includes(col.name) || col.name === 'buttons') ? col : {...col, visible: false}) 
          : 
          columns
        }
        isHeaderFilter={false}
        isSearch={false}
        allowColumnReordering={false}
        remoteOperations={true}
        onRowDblClick={onRowDblClick}
        hoverStateEnabled={true}
        containerClass='shadow-none border rounded-t-none '
        columnChooser={{enabled: true, mode: 'select'}}
        tableClass={tableClass}
        toolbar={[
          {
            location: 'before',
            render: (e) =>
              <div className='flex items-center gap-4 text-xs' >
                <div className='flex gap-1'>
                  <BsBriefcaseFill className='text-[#959595] text-sm'/> <span>On-Site Status</span>
                </div>
                <div className='flex gap-1'>
                  <BsAirplaneFill className='text-[#959595] text-sm'/> <span>Future Transport</span>
                </div>
                <div className='flex items-center gap-1'>
                  <i className="dx-icon-home text-[#959595] text-sm"></i>
                  <span>Future Room booking</span>
                </div>

              </div>
          },
        ]}
        {...restProps}
      />
    </div>
  )
}

export default AccommodationPeopleSearch