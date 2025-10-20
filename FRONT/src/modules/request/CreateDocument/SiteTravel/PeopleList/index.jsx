import React, { useCallback, useRef, useState } from 'react'
import { Table, Button, SearchPeopleForm } from 'components';
import { CheckBox } from 'devextreme-react';
import { BsAirplaneFill } from 'react-icons/bs'
import { LeftOutlined, PlusOutlined } from '@ant-design/icons';
import axios from 'axios';
import { Link, useNavigate } from 'react-router-dom';
import CustomStore from 'devextreme/data/custom_store';
import { GoPrimitiveDot } from 'react-icons/go';
import { twMerge } from 'tailwind-merge';
import AddPerson from './AddPerson';

function PeopleListSiteTravel({selectType='button', onSelect, toLink, target='', defaultColumns=[], renderSelectCell, ...restProps}) {
  const [ showAddPerson, setShowAddPerson ] = useState(false)
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
          url: `tas/employee/search${params}`,
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
  const navigate = useNavigate()

  const columns = [
    {
      label: 'Person #',
      name: 'Id',
      dataType: 'string',
      width: '80px',
    },
    {
      label: 'SAP ID #',
      name: 'SAPID',
      alignment: 'left',
      width: '90px',
      visible: false,
    },
    {
      label: '',
      name: 'Active',
      width: '21px',
      alignment: 'center',
      showInColumnChooser: false,
      allowSorting: false,
      cellRender:(e) => (
        <GoPrimitiveDot color={e.value === 1 ? 'lime' : 'lightgray'}/>
      )
    },
    {
      label: 'Firstname',
      name: 'Firstname',
      dataType: 'string',
    },
    {
      label: 'Lastname',
      name: 'Lastname',
      dataType: 'string',
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
    {
      label: 'Resource Type',
      name: 'PeopleTypeName',
      dataType: 'string',
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
    {
      label: '',
      name: 'buttons',
      width: '90px',
      alignment: 'center',
      showInColumnChooser: false,
      cellRender: (e) => (
        <div className='flex items-center gap-3'>
          <Link to={`${e.data.Id}`} target={target}><button type='button' className='edit-button'>Select</button></Link>
        </div>
      )
    },
  ]

  const handleSearch = (values) => {
    dataGrid.current?.instance.filter(values)
  }

  const onContextMenuPreparing = useCallback((e) => {
    if(e.target === 'header'){
      if (!e.items) e.items = [];
      e.items.push({
          text: 'Column Chooser',
          icon: 'columnchooser',
          onItemClick: () => {
            e.component?.showColumnChooser()
          }
      });
    }
  },[])

  const onAddPerson = () => {
    setShowAddPerson(true)
  }

  return (
    <>
      <div className={twMerge('bg-white px-3 py-2 rounded-ot shadow-card', showAddPerson ? 'block' : 'hidden')}>
        <div></div>
        <Button
          className='mb-3' 
          onClick={() => setShowAddPerson(false)}
          icon={<LeftOutlined />}
        >
          Back
        </Button>

        <AddPerson changeMode={setShowAddPerson}/>
      </div>
      <div className={twMerge(`bg-white rounded-ot shadow-card`, showAddPerson ? 'hidden' : 'block')}>
        <div className='px-3 py-2'>
          <SearchPeopleForm
            containerClass='bg-white rounded-ot py-2'
            onSearch={handleSearch}
            initialValues={{Active: 1}}
            extraContent={<Button htmlType='button' icon={<PlusOutlined/>} onClick={onAddPerson}>Add Person</Button>}
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
          onRowDblClick={(e) => navigate(`${e.data.Id}`)}
          containerClass='shadow-none border rounded-t-none'
          tableClass='max-h-[calc(100vh-290px)]'
          columnChooser={{enabled: false, mode: 'select', visible: false}}
          onContextMenuPreparing={onContextMenuPreparing}
          {...restProps}
        />
      </div>
    </>
  )
}

export default PeopleListSiteTravel