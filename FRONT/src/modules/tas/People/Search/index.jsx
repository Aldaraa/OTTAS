import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form, Table, Button, CustomSegmented, Tooltip, ColumnFilter } from 'components';
import { ControlOutlined, InfoCircleOutlined, SearchOutlined, UserOutlined } from '@ant-design/icons';
import axios from 'axios';
import AddEdit from './add';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { AuthContext } from 'contexts';
import DataSource from 'devextreme/data/data_source';
import isArray from 'lodash/isArray';
import ls from 'utils/ls';
import { CheckBox } from "devextreme-react";
import Highlighter from "react-highlight-words";
import { BsAirplaneFill } from "react-icons/bs";
import { GoPrimitiveDot } from "react-icons/go";

const defaultFields = ['Lastname', 'Firstname', 'SAPID', 'Id', 'DepartmentId', 'EmployerId', 'Active']

function Search() {
  const sFealds = ls.get('sFields')
  const routeLocation = useLocation();
  const cacheData = ls.get('el');
  const cacheSearch = cacheData ? cacheData.sf : null
  const cacheResult = cacheData ? cacheData.rd : null
  const [ selectedFields, setSelectedFields ] = useState(sFealds ? sFealds : defaultFields)
  const [ showModal, setShowModal ] = useState(false)
  const [ currentMode, setCurrentMode ] = useState('search')
  const [ filterValue, setFilterValue ] = useState([])

  let inited = false
  const [ store ] = useState(new DataSource({
    key: 'Id',
    load: (loadOptions) => {
      let params = '';
      let options = {}
      if(loadOptions.sort?.length > 1){
        params = `?sortby=${loadOptions.sort[0].selector}&sortdirection=${loadOptions.sort[0].desc ? 'desc' : 'asc'}`
      }
      if(loadOptions.filter && !isArray(loadOptions?.filter)){
        options = loadOptions.filter
      }
      if(!inited){
        inited = true
        return new Promise((resolve) => {
          resolve({
            totalCount: cacheData ? cacheResult.totalcount : 0,
            data: cacheData ? cacheResult.data : [],
          })
        })
      }else{
        return axios({
          method: 'post',
          url: `tas/employee/search${params}`,
          data: {
            model: options,
            pageIndex: loadOptions.skip/loadOptions.take,
            pageSize: loadOptions.take
          }
        }).then((res) => {
          if(options?.searchTypeSave){
            ls.set('el',{rd: res.data, sf: {model: loadOptions.filter}})
          }else{
            ls.set('el',null)
          }
          setFilterValue([
            loadOptions.filter?.Lastname,
            loadOptions.filter?.Firstname,
            loadOptions.filter?.NRN,
            loadOptions.filter?.Mobile,
            loadOptions.filter?.roomNumber,
          ])
          return {
            data: res.data.data,
            totalCount: res.data.totalcount,
          }
        })
      }
    }
  }))

  const { state, action } = useContext(AuthContext)
  const navigate = useNavigate()
  const [ searchForm ] = Form.useForm()
  const [ addForm ] = Form.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
    action.changeMenuKey('/tas/people/search')
  },[])

  const fields = useMemo(() => {
    return [
      {
        label: state.referData.profileFields['Firstname']?.Label,
        name: 'Firstname',
        className: 'col-span-4 mb-2',
        inputprops: {
          className: 'w-full',
        },
      },
      {
        label: state.referData.profileFields['Lastname']?.Label,
        name: 'Lastname',
        className: 'col-span-4 mb-2',
        inputprops: {
          className: 'w-full',
        },
      },
      {
        label: 'Room number',
        name: 'roomNumber',
        className: 'col-span-4 mb-2',
        inputprops: {
          className: 'w-full',
        },
      },
      {
        label: state.referData.profileFields['DepartmentId']?.Label,
        name: 'DepartmentId',
        className: 'col-span-4 mb-2',
        type: 'treeSelect',
        inputprops: {
          treeData: state.referData?.departments,
          fieldNames: {label: 'Name', value: 'Id', children: 'ChildDepartments'},
          allowClear: true,
          showSearch: true,
          style: {display: 'contents'},
          filterTreeNode: (input, option) => (option?.Name ?? '').toLowerCase().includes(input.toLowerCase()),
        }
      },
      {
        label: 'Camp',
        name: 'CampId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.camps,
        },
      },
      {
        label: state.referData.profileFields['LocationId']?.Label,
        name: 'LocationId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.locations,
        },
      },
      {
        label: state.referData.profileFields['FlightGroupMasterId']?.Label,
        name: 'FlightGroupMasterId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.transportGroups,
        },
      },
      {
        label: 'Room Type',
        name: 'RoomTypeId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.roomTypes,
        },
      },
      {
        label: state.referData.profileFields['NRN']?.Label,
        name: 'NRN',
        className: 'col-span-4 mb-2',
        inputprops: {
          className: 'w-full',
        },
      },
      {
        label: state.referData.profileFields['PeopleTypeId']?.Label,
        name: 'PeopleTypeId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.peopleTypes,
        },
      },
      {
        label: state.referData.profileFields['CostCodeId']?.Label,
        name: 'CostCodeId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          options: state.referData?.costCodes,
        },
      },
      {
        label: state.referData.profileFields['EmployerId']?.Label,
        name: 'EmployerId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.employers,
        },
      },
      {
        label: state.referData.profileFields['PositionId']?.Label,
        name: 'PositionId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.positions,
        },
      },
      {
        label: state.referData.profileFields['RosterId']?.Label,
        name: 'RosterId',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: state.referData?.rosters,
        },
      },
      {
        label: 'Future Bookings',
        name: 'futureBooking',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: [{label: 'Has future booking', value: 1}, {label: 'No future booking', value: 0}]
        },
      },
      {
        label: 'Group',
        name: 'group',
        className: 'col-span-4 mb-2',
        // type: 'select',
        inputprops: {
          className: 'w-full',
        },
      },
      {
        label: 'Has room',
        name: 'hasRoom',
        className: 'col-span-4 mb-2',
        type: 'select',
        inputprops: {
          className: 'w-full',
          options: [{label: 'Yes', value: 1}, {label: 'No', value: 0}]
        },
      },
      {
        label: state.referData.profileFields['Mobile']?.Label,
        name: 'Mobile',
        className: 'col-span-4 mb-2',
        inputprops: {
          className: 'w-full',
        },
      },
      {
        label: state.referData.profileFields['SAPID']?.Label,
        name: 'SAPID',
        className: 'col-span-4 mb-2',
        type: 'textarea',
        inputprops: {
          className: 'w-full',
          autoSize: {
            minRows: 1,
            maxRows: 4,
          },
          showCount: true,
          maxLength: 5000,
        },
      },
      {
        label: 'Person #',
        name: 'Id',
        className: 'col-span-4 mb-2',
        type: 'textarea',
        inputprops: {
          className: 'w-full',
          autoSize: {
            minRows: 1,
            maxRows: 4,
          },
          showCount: true,
          maxLength: 5000
        },
      },
      {
        label: <div className='flex gap-2'>
          <span>Active Only</span> 
          <Tooltip title={<span>checked = Active <br/>  unchecked = Inactive <br/> both check = Both</span>}>
            <InfoCircleOutlined className='text-gray-400' size={16}/>
          </Tooltip>
        </div>,
        name: 'Active',
        className: 'col-span-4 mb-2',
        type: 'check',
        inputprops: {
          className: 'w-full',
          indeterminatewith: true
        },
      },
    ]
  },[state])

  const columns = useMemo(() => {
    return [
      {
        label: 'Person #',
        name: 'Id',
        dataType: 'string',
        width: '70px',
        cellRender: (e) => (
          <div className="flex items-center justify-center">
            <Link to={`/tas/people/search/${e.data.Id}`}>
              <button type='button' className="text-blue-600 underline">{e.value}</button>
            </Link>
          </div>
        )
      },
      {
        label: 'SAP ID #',
        name: 'SAPID',
        alignment: 'left',
        width: '90px',
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
        cellRender: (e) => (
          <Highlighter textToHighlight={e.value} searchWords={filterValue} autoEscape/>
        )
      },
      {
        label: 'Lastname',
        name: 'Lastname',
        dataType: 'string',
        cellRender: (e) => (
          <Highlighter textToHighlight={e.value} searchWords={filterValue} autoEscape/>
        )
      },
      {
        label: 'Department',
        name: 'DepartmentName',
        dataType: 'string',
        // width: '220px',
      },
      {
        label: 'Employer',
        name: 'EmployerName',
        dataType: 'string',
      },
      {
        label: 'Roster',
        name: 'RosterName',
        dataType: 'string',
        // width: '70px',
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
    ]
  },[filterValue])

  const handleSearch = (values) => {
    dataGrid.current?.instance.filter({...values, searchTypeSave: false})
  }

  const onChangeSegment = (e) => {
    if(e !== 'search'){
      setCurrentMode('add')
      addForm.setFieldsValue({...searchForm.getFieldsValue()})
    }
    else{
      setCurrentMode('search')
    }
  }

  const handleSaveAndSearch = () => {
    dataGrid.current?.instance.filter({...searchForm.getFieldsValue(), searchTypeSave: true})
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



  return (
    <div className='relative'>
      <CustomSegmented value={currentMode} onChange={onChangeSegment} notDisabled={true}/>
      {
        currentMode === 'search' ?
        <>
          <div className='rounded-t-ot bg-white py-2 px-4 border-b shadow-md'>
            <div className='flex items-center justify-between mb-2'>
              <div className='text-lg font-bold'>Search people</div>
              <Button 
                htmlType='button' 
                onClick={() => setShowModal(true)} 
                className='px-2'
                icon={<ControlOutlined style={{fontSize: '16px'}}/>}
              >
                Filter
              </Button>
            </div>
            <Form
              form={searchForm}
              fields={fields.filter((item) => selectedFields.includes(item.name))} 
              className='grid grid-cols-12 grid-row-3 gap-x-8 grid-flow-row-dense'
              onFinish={handleSearch}
              initValues={cacheData ? cacheSearch.model : null}
              size='small'
              wrapperCol={{flex: 1}}
              labelCol={{flex: '100px'}}
            >
              <div className='col-span-12 flex gap-4 justify-end mt-1'>
                <Link to={`/tas/people/search/${state.userInfo?.Id}`}>
                  <Button htmlType='button' icon={<UserOutlined/>}>Use me</Button>
                </Link>
                <Button htmlType='button' icon={<SearchOutlined/>} onClick={handleSaveAndSearch}>Save & Search</Button>
                <Button htmlType='submit' icon={<SearchOutlined/>}>Search</Button>
              </div>
            </Form>
          </div>
          <div className='sticky top-0 z-20'>
            <Table
              ref={dataGrid}
              data={store}
              columns={columns}
              isHeaderFilter={false}
              isSearch={false}
              allowColumnReordering={false}
              remoteOperations={true}
              onRowDblClick={(e) => navigate(`/tas/people/search/${e.data.Id}`)}
              containerClass='pt-0 rounded-b-ot rounded-t-none'
              defaultPageSize={100}
              style={{maxHeight: 'calc(100vh - 330px)'}}
              columnChooser={{enabled: false, mode: 'select', visible: false}}
              onContextMenuPreparing={onContextMenuPreparing}
            />
          </div>
        </>
        :
        <AddEdit
          changeMode={setCurrentMode}
          profileFields={state.referData.profileFields}
          form={addForm}
        />
      }
      <ColumnFilter
        open={showModal}
        onCancel={() => setShowModal(false)}
        selectedFields={selectedFields}
        setSelectedFields={setSelectedFields}
        profileFields={state.referData.profileFields}
      />
    </div>
  )
}

export default Search