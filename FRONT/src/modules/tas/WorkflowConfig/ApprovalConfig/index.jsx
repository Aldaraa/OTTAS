import { Table, Button, Modal, Form } from 'components'
import { Popup } from 'devextreme-react'
import React, { useContext, useEffect, useState } from 'react'
import { Form as AntForm, Select } from 'antd'
import axios from 'axios'
import { AuthContext } from 'contexts'

const title = 'Approval Configuration'

function ApprovalConfiguration() {
  const [ data, setData ] = useState([])
  const [ renderData, setRenderData ] = useState([])
  const [ groups, setGroups ] = useState([])
  const [ editData, setEditData ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ showModal, setShowModal ] = useState(false)
  const [ searchLoading, setSearchLoading ] = useState(false)
  const [ documentTypes, setDocumentTypes ] = useState([])
  const [ selectedType, setSelectedType ] = useState(null)
  const [ isReorder, setIsReorder ] = useState(false)
  const [ optionLoading, setOptionLoading ] = useState(false)
  const [ actionLoading, setActionLoading ] = useState(false)

  const [ form ] = AntForm.useForm()
  const { action } = useContext(AuthContext)

  useEffect(() => {
    action.changeMenuKey('/tas/approvalconfig')
    getData()
    getGroups()
  },[])
  
  useEffect(() => {
    getGroupData()
  },[selectedType])

  const getData = () => {
    setLoading(true)
    axios({
      method: 'get',
      url: 'tas/requestgroupconfig/documenttypes'
    }).then((res) => {
      setDocumentTypes(res.data)
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  const getGroupData = () => {
    setSearchLoading(true)
    axios({
      method: 'get',
      url: `tas/requestgroupconfig/documentapproval/${selectedType}`
    }).then((res) => {
      setRenderData(res.data)
      setData(res.data)
    }).catch((err) => {

    }).then(() => setSearchLoading(false))
  }
  
  const getGroups = () => {
    setOptionLoading(true)
    axios({
      method: 'get',
      url: 'tas/requestgroup'
    }).then((res) => {
      setGroups(res.data.map((item) => ({...item, label: item.Description, value: item.Id})))
    }).catch((err) => {

    }).then(() => setOptionLoading(false))
  }

  const columns = (type) => {
    if(type === 'Non Site Travel'){
      return[
        {
          label: 'Group Name',
          name: 'GroupName'
        },
        {
          label: 'Rule Action',
          name: 'RuleAction'
        },
      ]
    }else{
      return [
        {
          label: 'Group Name',
          name: 'GroupName'
        },
      ]
    }
  } 

  const handleSubmit = (values) => {
    if(editData){
      setActionLoading(true)
      axios({
        method: 'put',
        url: `tas/requestgroupconfig/documentapproval`,
        data: {
          ...values,
          document: selectedType,
          id: editData.Id,
        }
      }).then((res) => {
        setShowModal(false)
        getGroupData()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
    else{
      setActionLoading(true)
      axios({
        method: 'post',
        url: `tas/requestgroupconfig/documentapproval`,
        data: {
          ...values,
          document: selectedType,
        }
      }).then((res) => {
        setShowModal(false)
        getGroupData()
      }).catch((err) => {
  
      }).then(() => setActionLoading(false))
    }
  }

  const handleDelete = () => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/requestgroupconfig/${editData.Id}`,
    }).then(() => {
      getGroupData()
      setShowPopup(false)
    }).catch((err) => {

    }).then(() => setActionLoading(false))
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
    const toIndex = newTasks.findIndex((item) => item.Id === visibleRows[e.toIndex]?.data.Id);
    const fromIndex = newTasks.findIndex((item) => item.Id === e.itemData.Id);

    newTasks.splice(fromIndex, 1);
    newTasks.splice(toIndex, 0, e.itemData);
    setRenderData(newTasks)
  }

  const closeModal = () => {
    form.resetFields()
    setShowModal(false)
  }

  return (
    <div>
      {
        <>
          <div className='rounded-ot bg-white px-3 py-2 mb-3 shadow-md'>
            <div className='text-lg font-bold mb-2'>{title}</div>
            <div className='w-1/3'>
              <div className='flex gap-4 justify-start col-span-12'>
                <Select 
                  options={documentTypes} 
                  fieldNames={{value: 'Name', label: 'Name'}} 
                  className='w-[300px]'
                  onChange={(e) => setSelectedType(e)}
                  loading={loading}
                />
              </div>
            </div>
          </div>
          {
            selectedType &&
            <Table
              data={renderData}
              columns={columns(selectedType)}
              allowColumnReordering={false}
              loading={searchLoading}
              keyExpr='Id'
              sorting={false}
              pager={renderData.length > 20}
              rowDragging={isReorder ? {
                onReorder:(e) => onReorder(e),
                onDragEnd:(e) => onReorderEnd(e),
                onDragStart: (e) => onReorderStart(e)
              } : false}
            />
          }
          <Modal 
            title='Add Group'
            open={showModal} 
            onCancel={() => setShowModal(false)}
          >
            <Form
              form={form}
              fields={fields}
              editData={editData}
              onFinish={handleSubmit}
            >
              <div className='col-span-12 flex justify-end gap-3 mt-5'>
                <Button type='primary' htmlType='submit' loading={actionLoading}>Save</Button>
                <Button htmlType='button' onClick={closeModal} disabled={actionLoading}>Cancel</Button>
              </div>
            </Form>
          </Modal>
          <Popup
            visible={showPopup}
            showTitle={false}
            height={110}
            width={350}
          >
            <div>Are you sure you want to {editData?.Active === 1 ? 'deactivate' : 'reactivate'} this record?</div>
            <div className='flex gap-5 mt-4 justify-center'>
              <Button type={editData?.Active === 1 ? 'danger' : 'success'} onClick={handleDelete} loading={actionLoading}>Yes</Button>
              <Button onClick={() => setShowPopup(false)} disabled={actionLoading}>No</Button>
            </div>
          </Popup>
        </>
      }
    </div>
  )
}

export default ApprovalConfiguration