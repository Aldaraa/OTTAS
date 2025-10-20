import { Form, Table, Button, Modal} from 'components'
import React, { useCallback, useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { LeftOutlined } from '@ant-design/icons'
import { useNavigate, useParams } from 'react-router-dom'
import { AuthContext } from 'contexts'

const title = 'Transport Group Detail'

const fields = [
  {
    label: 'Code',
    name: 'Code',
    className: 'col-span-12 mb-2',
    inputprops: {
      maxLength: 10
    }
  },
  {
    label: 'Description',
    name: 'Description',
    className: 'col-span-12 mb-2'
  },
  {
    label: 'Active',
    name: 'Active',
    className: 'col-span-12 mb-2',
    type: 'check',
    inputprops: {
      indeterminatewith: true,
    }
  },
]

function TransportGroupDetail() {
  const [ data, setData ] = useState(null)
  const [ detailData, setDetailData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ clusters, setClusters ] = useState([])
  const [ showModal, setShowModal ] = useState(false)
  const [ editItem, setEditItem ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)

  const [ form ] = AntForm.useForm()
  const [ shiftForm ] = AntForm.useForm()
  const { groupId } = useParams()
  const navigate = useNavigate()
  const { action, state } = useContext(AuthContext)
  const dataGrid  = useRef(null)
  
  useEffect(() => {
    action.changeMenuKey('/tas/transportgroup')
    getData()
    getClusters()
  },[])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: `tas/FlightGroupmaster/${groupId}`,
    }).then((res) => {
      setData(res.data)
      setDetailData(res.data.Detail)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getClusters = () => {
    axios({
      method: 'get',
      url: 'tas/cluster?Active=1',
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({ value: item.Id, label: `${item.Code} - ${item.Description}`, ...item})
      })
      setClusters(tmp)
    }).catch((err) => {

    })
  }

  const filterClusterOptions = useCallback((options) => {
    return {
      store: clusters,
      filter: options.data ? [['DayNum', '=', options.data?.DayNum], ['Direction', '=', options.data?.Direction]] : null,
    }
  },[clusters])


  const handleClickChangeShift = useCallback((row) => {
    setEditItem(row)
    setShowModal(true)
  },[])

  const shiftField = [
    { 
      label: 'Shift',
      name: 'ShiftId',
      type: 'select',
      className: 'col-span-12 mb-2',
      inputprops: {
        options: state.referData?.roomStatuses
      }
    }
  ]

  const columns = useMemo(() => [
    {
      label: '#',
      name: 'Id',
      width: '100px',
      allowEditing: false,
    },
    {
      label: 'Day',
      name: 'DayNum',
      allowEditing: false,
    },
    {
      label: 'Direction',
      name: 'Direction',
      allowEditing: false,
    },
    {
      label: 'Shift',
      name: 'ShiftCode',
      alignment: 'left',
      allowEditing: false,
    },
    {
      label: 'Cluster',
      name: 'ClusterId',
      haveLookup: {
        dataSource: (options) => filterClusterOptions(options),
        valueExpr: 'value',
        displayExpr: 'label',
        allowClearing: true,
      },
    },
    {
      label: '',
      name: 'action',
      width: '170px',
      alignment: 'center',
      cellRender: (e) => (
        <button className='edit-button' onClick={() => handleClickChangeShift(e.data)}>Change Shift</button>
      )
    },
  ], [clusters])

  const handleSave = useCallback((e) => {
    let tmp = []
    e.changes.map((item) => {
      tmp.push({flightGroupDetailId: item.key, clusterId: item.data.ClusterId})
    })
    axios({
      method: 'post',
      url: 'tas/flightgroupdetail/setcluster',
      data: {
        request: tmp
      }
    }).then((res) => {
      
    }).catch((err) => {

    })
  },[])

  const handleCancelModal = useCallback(() => {
    setShowModal(false)
  },[])

  const handleSubmitShift = useCallback((values) => {
    setActionLoading(true)
    axios({
      method: 'put',
      url: 'tas/flightgroupdetail/shift',
      data: {
        ...values,
        id: editItem.Id,
      }
    }).then((res) => {
      getData()
      handleCancelModal()
    }).catch((err) => {

    }).finally(() => setActionLoading(false))
  },[editItem])

  const onEditorPreparing = useCallback((e) => {
    e.editorOptions.showClearButton = true;
  },[])

  return (
    <div>
      <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
        <div className='text-lg font-bold mb-2'>{title}</div>
        <div className='w-1/3'>
          <Form 
            form={form} 
            fields={fields} 
            editData={data}
            className='grid grid-cols-12 gap-x-8' 
            size='small'
            disabled={true}
          />
          <div className='flex gap-4 justify-end'>
            <Button onClick={() => navigate(-1)} icon={<LeftOutlined/>}>Back</Button>
          </div>
        </div>
      </div>
      <Table
        ref={dataGrid}
        data={detailData}
        columns={columns}
        allowColumnReordering={false}
        loading={loading}
        keyExpr='Id'
        edit={{mode: 'batch', allowUpdating: true, startEditAction: 'dblClick'}}
        onEditorPreparing={onEditorPreparing}
        onSaving={handleSave}
        pager={{showPageSizeSelector: false, showNavigationButtons: false}}
        defaultPageSize={100}
        tableClass='max-h-[calc(100vh-290px)]'
      />
      <Modal title='Change Shift' open={showModal} onCancel={handleCancelModal}>
        <Form 
          form={shiftForm}
          fields={shiftField} 
          editData={editItem}
          onFinish={handleSubmitShift}
          labelCol={{flex: '90px'}}
        >
          <div className='col-span-12 flex gap-3 justify-end'>
            <Button htmlType='submit' type='primary' loading={actionLoading}>Save</Button>
            <Button onClick={handleCancelModal} >Cancel</Button>
          </div>
          
        </Form>
      </Modal>
    </div>
  )
}

export default TransportGroupDetail