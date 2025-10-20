import React, { useContext, useEffect, useState } from 'react'
import { Form, Button, Tooltip, ColumnFilter } from 'components';
import { CloseOutlined, ControlOutlined, InfoCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { AuthContext } from 'contexts';
import ls from 'utils/ls';

const defaultFields = ['Lastname', 'Firstname', 'SAPID', 'Id', 'DepartmentId', 'EmployerId', 'Active']

function SearchPeopleForm({containerClass='', onSearch, initialValues={}, hideFields=[], extraContent, showClearButton=false, ...restProps}) {
  const sFealds = ls.get('sFields')
  const [ showModal, setShowModal ] = useState(false)
  const [ selectedFields, setSelectedFields ] = useState(sFealds ? sFealds : defaultFields)
  const [ searchForm ] = Form.useForm()
  const { state } = useContext(AuthContext)

  const fields = [
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
        // style: { width: '100%'}
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

  const formItemLayout = {
    labelCol: {
      xs: {
        span: 24,
      },
      sm: {
        span: 8,
      },
    },
    wrapperCol: {
      xs: {
        span: 24,
      },
      sm: {
        span: 16,
      },
    },
  };

  const handleSearch = (values) => {
    if(onSearch){
      onSearch(values)
    }
  }


  return (
    <div className={containerClass}>
      <div className='flex justify-between items-center mb-2'>
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
        fields={fields.filter((item) => selectedFields.includes(item.name) && !hideFields.includes(item.name))} 
        className='grid grid-cols-12 gap-x-8'
        initValues={initialValues}
        {...formItemLayout}
        onFinish={handleSearch}
        size='small'
      >
        <div className='col-span-12 flex gap-4 justify-end mt-2'>
          {extraContent}
          {showClearButton ? 
            <Button icon={<CloseOutlined/>} onClick={() => searchForm.resetFields()}>Clear Filter</Button>
            : null
          }
          <Button htmlType='submit' icon={<SearchOutlined/>}>Search</Button>
        </div>
      </Form>
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

export default SearchPeopleForm