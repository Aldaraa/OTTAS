import { SearchOutlined } from '@ant-design/icons'
import axios from 'axios'
import { Button, CustomTable, Form, Table } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox } from 'devextreme-react'
import React, { useContext, useEffect, useRef, useState } from 'react'
import tableSearch from 'utils/TableSearch'
import CustomStore from 'devextreme/data/custom_store'

function RoomSearch({onSelect, fromDate, toDate}) {
  const [ data, setData ] = useState([])
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ totalCount, setTotalCount ] = useState(0)
  const [ pageIndex, setPageIndex ] = useState(0)
  const [ pageSize, setPageSize ] = useState(100)
  const [ isIinit, setIsInit ] = useState(false)

  const dataGrid = useRef(null)
  const { state } = useContext(AuthContext)
  const [ form ] = Form.useForm()
  useEffect(() => {
    setIsInit(true)
  },[])

  useEffect(() => {
    if(isIinit){
      getData()
    }
  },[pageIndex, pageSize])

  const getData = () => {
    const values = form.getFieldsValue()
    dataGrid.current?.instance.beginCustomLoading()
    axios({
      method: 'post',
      url: `tas/room/search`,
      data: {
        model: values,
        pageIndex: pageIndex,
        pageSize: pageSize
      }
    }).then((res) => {
      setData(res.data.data)
      setTotalCount(res.data.totalcount)
    }).catch((err) => {

    }).finally(() => dataGrid.current?.instance.endCustomLoading())
  }

  const fields = [
    {
      label: 'Camp',
      name: 'CampId',
      className: 'col-span-6 mb-2',
      type: 'select',
      // rules: [{required: true, message: 'Camp is required'}],
      inputprops: {
        options: state.referData?.camps,
      }
    },
    {
      label: 'Room Type',
      name: 'RoomTypeId',
      className: 'col-span-6 mb-2',
      type: 'select',
      // rules: [{required: true, message: 'Room Type is required'}],
      inputprops: {
        options: state.referData.roomTypes,
      }
    },
    {
      label: 'Room Number',
      name: 'RoomNumber',
      className: 'col-span-6 mb-2',
    },
    // {
    //   label: 'Bed Count',
    //   name: 'BedCount',
    //   className: 'col-span-6 mb-2',
    //   type: 'number',
    // },
    {
      label: 'Private',
      name: 'Private',
      className: 'col-span-6 mb-2',
      type: 'check',
      inputprops: {
        indeterminatewith: true,
      }
    },
  ]

  const columns = [
    {
      label: 'Room Number',
      name: 'Number',
      width: '150px'
    },
    {
      label: 'Camp Name',
      name: 'CampName'
    },
    {
      label: 'Bed Count',
      name: 'BedCount',
      width: '100px',
      dataType: 'string',
    },
    {
      label: 'Room Type',
      name: 'RoomTypeName'
    },
    {
      label: 'Private',
      name: 'Private',
      width: '60px',
      dataType: 'string',
      align:'center',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.data.Private === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'action',
      width: '90px',
      cellRender: (e) => (
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => onSelect(e.data)}>Select</button>
        </div>
      )
    },
  ]

  return (
    <div>
      <Form
        form={form}
        fields={fields}
        initValues={{Private: null}}
        size='small' 
        className='grid grid-cols-12 gap-x-8 border rounded-ot p-4 mb-4' 
        onFinish={getData}
      >
        <div className='col-span-12 flex justify-end'>
          <Button
            htmlType='submit'
            loading={searchLoading}
            icon={<SearchOutlined/>}
          >
            Search
          </Button>
        </div>
      </Form>
      <CustomTable
        ref={dataGrid}
        data={data}
        keyExpr='Id'
        columns={columns}
        onChangePageSize={(e) => setPageSize(e)}
        onChangePageIndex={(e) => setPageIndex(e)}
        pageSize={pageSize}
        pageIndex={pageIndex}
        totalCount={totalCount}
        pagerPosition='bottom'
        showColumnLines={false}
        wordWrapEnabled={true}
        containerClass='shadow-none px-0 mt-2' 
        tableClass='max-h-[500px] border'
      />
    </div>
  )
}

export default RoomSearch