import { Table, Button, Modal, Form } from 'components'
import { Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm } from 'antd'
import axios from 'axios'
import { OrderedListOutlined, PlusOutlined, SaveOutlined } from '@ant-design/icons'
import { AuthContext } from 'contexts'

const title = 'Flight Rules'

function FlightConfig() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ groups, setGroups ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ tableLoading, setTableLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ isReorder, setIsReorder ] = useState(false)
  const [ optionLoading, setOptionLoading ] = useState(false)
  const [ masterData, setMasterData ] = useState(null)

  const [ form ] = AntForm.useForm()
  const { action, state } = useContext(AuthContext)

  useEffect(() => {
    action.changeMenuKey('/tas/nonsitetravelconfig')
    getData()
    getGroups()
    getMasterData()
  },[])

  const getData = () => {
    setTableLoading(true)
    axios({
      method: 'get',
      url: `tas/requestnonsiteflightgroupconfig`
    }).then((res) => {
      setRenderData(res.data)
      setData(res.data)
    }).catch((err) => {

    }).then(() => setTableLoading(false))
  }
  
  const getGroups = () => {
    setOptionLoading(true)
    axios({
      method: 'get',
      url: 'tas/requestdocument/nonsitetravel/groups'
    }).then((res) => {
      setGroups(res.data)
    }).catch((err) => {

    }).then(() => setOptionLoading(false))
  }

  const getMasterData = () => {
    setOptionLoading(true)
    axios({
      method: 'get',
      url: 'tas/requestdocument/master'
    }).then((res) => {
      let tmp = {}
      tmp.DocumentSearchCurrentStep = res.data?.DocumentSearchCurrentStep.map((item) => ({value: item, label: item}))
      tmp.PaymentCondition = res.data?.PaymentCondition.map((item) => ({value: item, label: item}))
      tmp.DocumentNonSiteRuleType = res.data?.DocumentNonSiteRuleType.map((item) => ({value: item, label: item}))
      tmp.RequestDocumentType = res.data?.RequestDocumentType.map((item) => ({value: item, label: item}))
      tmp.RequestNonDocumentActions = res.data?.RequestNonDocumentActions.map((item) => ({value: item, label: item}))
      setMasterData(tmp)
    }).catch((err) => {

    }).then(() => setOptionLoading(false))
  }

  const handleAddButton = () => {
    setEditData(null)
    setShowModal(true)
  }

  const handleEditButton = (dataItem) => {
    let tmp = dataItem.RuleAction.split(',').map((item) => ({value: item, label: item}))
    setEditData({...dataItem, RuleAction: tmp })
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    let tmp = dataItem.RuleAction.split(',').map((item) => ({value: item, label: item}))
    setEditData({...dataItem, RuleAction: tmp })
    setShowPopup(true)
  }

  const columns = [
    {
      label: 'Group Name',
      name: 'GroupName'
    },
    {
      label: 'Rule Action',
      name: 'RuleAction'
    },
    {
      label: '',
      name: 'action',
      width: '150px',
      cellRender: (e) => (
        e.data?.ReadOnly === 0 &&
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
        </div>
      )
    },
  ]

  const handleSubmit = (values) => {
    let ruleAction = ''
    values.RuleAction?.map((rule, i) => {
      if(i === 0){
        ruleAction = ruleAction.concat(rule)
      }else{
        ruleAction = ruleAction.concat(', ',rule)
      }
    })

    if(editData){
      setLoading(true)
      axios({
        method: 'put',
        url: `tas/requestnonsiteflightgroupconfig`,
        data: {
          ...values,
          RuleAction: ruleAction,
          Id: editData.Id
        }
      }).then((res) => {
        closeModal()
        getData()
      }).catch((err) => {

      }).then(() => setLoading(false))
    }
    else{
      setLoading(true)
      axios({
        method: 'post',
        url: `tas/requestnonsiteflightgroupconfig`,
        data: {
          ...values,
          RuleAction: ruleAction
        }
      }).then((res) => {
        closeModal()
        getData()
      }).catch((err) => {

      }).then(() => setLoading(false))
    }
  }

  const handleDelete = () => {
    setLoading(true)
    axios({
      method: 'delete',
      url: `tas/requestnonsiteflightgroupconfig`,
      data: {
        id: editData.Id
      }
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const fields = [
    {
      label: 'Group',
      name: 'GroupId',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Group is required'}],
      inputprops: {
        options: groups,
        fieldNames: {value: 'Id', label: 'Description'},
        loading: optionLoading,
      }
    },
    {
      label: 'Rule Action',
      name: 'RuleAction',
      className: 'col-span-12 mb-2',
      type: 'select',
      rules: [{required: true, message: 'Rule Action is required'}],
      inputprops: {
        options: masterData?.RequestNonDocumentActions,
        mode: 'multiple'
      }
    },
  ]

  const onReorderStart = (e) => {
    if(e.fromIndex === 0 || e.fromIndex === renderData.length - 1){
      e.cancel = true
    }
  }

  const onReorderEnd = (e) => {
    if(e.toIndex === 0 || e.toIndex === renderData.length - 1){
      e.cancel = true
    }
  }

  const onReorder = (e) => {
    const visibleRows = e.component.getVisibleRows();
    const newTasks = [...renderData];
    const toIndex = newTasks?.findIndex((item) => item.Id === visibleRows[e.toIndex]?.data.Id);
    const fromIndex = newTasks?.findIndex((item) => item.Id === e.itemData.Id);

    newTasks.splice(fromIndex, 1);
    newTasks.splice(toIndex, 0, e.itemData);
    setRenderData(newTasks)
  }

  const handleSaveReorder = () => {
    setLoading(true)
    let Ids = []
    renderData.map((item, index) => {
      if(!(index === 0 || index === renderData.length-1)){
        Ids.push(item.Id)
      }
    })
    axios({
      method: 'put',
      url: 'tas/requestnonsiteflightgroupconfig/order',
      data: {
        Ids: Ids,
      }
    }).then((res) => {
      getData()
      setIsReorder(false)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const handleCancelReorder = () => {
    setRenderData(data)
    setIsReorder(false)
  }

  const closeModal = () => {
    form.resetFields()
    setShowModal(false)
  }

  return (
    <div>
      <Table
        title={<div className='flex justify-between items-center py-2'>
            <div className='font-bold ml-2'>{title}</div>
            <div className='flex items-center gap-3'>
              {
                !state.userInfo?.ReadonlyAccess &&
                (
                  isReorder ? 
                  <div className='flex items-center gap-3'>
                    <Button type='primary' icon={<SaveOutlined />} onClick={handleSaveReorder} loading={loading}>Save Reorder</Button>
                    <Button onClick={handleCancelReorder} disabled={loading}>Cancel</Button>
                  </div>
                  :
                  <>
                    <Button icon={<PlusOutlined />} onClick={handleAddButton}>Add Group</Button>
                    <Button icon={<OrderedListOutlined />} onClick={() => setIsReorder(true)}>Change Seq</Button>
                  </>
                )
              }
            </div>
          </div>
        }
        tableClass='border-t'
        sorting={false}
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        loading={tableLoading}
        keyExpr='Id'
        pager={renderData.length > 20}
        rowDragging={isReorder ? {
          onReorder:(e) => onReorder(e),
          onDragEnd:(e) => onReorderEnd(e),
          onDragStart: (e) => onReorderStart(e)
        } : false}
      />
      <Modal 
        title={`${editData ? 'Edit' : 'Add'} Group (Flight Rule)`}
        open={showModal} 
        onCancel={closeModal}
      >
        <Form
          form={form}
          fields={fields}
          editData={editData}
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end gap-3 mt-5'>
            <Button type='primary' htmlType='submit' loading={loading}>Save</Button>
            <Button htmlType='button' onClick={closeModal} disabled={loading}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to deactivate this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button type={'danger'} onClick={handleDelete} loading={loading}>Yes</Button>
          <Button onClick={() => setShowPopup(false)} disabled={loading}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default FlightConfig