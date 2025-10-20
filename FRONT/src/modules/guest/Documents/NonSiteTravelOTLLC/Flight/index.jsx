import axios from 'axios'
import { Button, Modal, Form, Table, Tooltip } from 'components'
import { AuthContext } from 'contexts'
import { CheckBox, Popup } from 'devextreme-react'
import React, { useContext, useEffect, useMemo, useRef, useState } from 'react'
import { Form as AntForm, Tabs } from 'antd'
import { PlusOutlined, QuestionCircleOutlined, SaveOutlined, SyncOutlined } from '@ant-design/icons'
import dayjs from 'dayjs'
import { useParams } from 'react-router-dom'
import { FinalItinerary, ProcessingItinerary, UpdateItinerary } from './Itinerary'

function Flight({disabled, documentDetail, currentGroup, getData, data, hasActionPermission, userInfo}) {
  const [ loading, setLoading ] = useState(false)
  const [ submitLoading, setSubmitLoading ] = useState(false)
  const [ airports, setAirports ] = useState([])
  const [ showModal, setShowModal ] = useState(false)
  const [ editData, setEditData ] = useState(null)
  const [ showPopup, setShowPopup ] = useState(false)
  const [ tabKey, setTabKey ] = useState(documentDetail?.CurrentStatus === 'Submitted' ? 1 :2)
  const [ finalOptions, setFinalOptions ] = useState([])
  const dataGrid = useRef(null)

  const { state, action } = useContext(AuthContext)
  const [ form ] = AntForm.useForm()
  const [ finalFlightForm ] = AntForm.useForm()
  const { documentId } =  useParams()

  useEffect(() => {
    getAirportData()
  },[])

  useEffect(() => {
    if(data){
      finalFlightForm.setFieldsValue(data)
    }
  },[data])

  useEffect(() => {
    if(documentDetail?.CurrentStatus === 'Completed'){
      setTabKey(3)
    }
  },[currentGroup])

  const getAirportData = () => {
    axios({
      method: 'get',
      url: 'tas/requestairport'
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        tmp.push({ value: item.Id, label: `${item.Code} (${item.Country})`})
      })
      setAirports(tmp)
    }).catch((err) => {

    })
  }

  const fields = [
    {
      label: 'Travel Date',
      name: 'TravelDate',
      className: 'col-span-12 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true
      }
    },
    {
      label: 'Favor Time',
      name: 'FavorTime',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: state.referData?.master?.favorTimes
      }
    },
    {
      label: 'Depart Location',
      name: 'DepartLocationId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: airports,
      }
    },
    {
      label: 'Arrive Location',
      name: 'ArriveLocationId',
      className: 'col-span-12 mb-2',
      type: 'select',
      inputprops: {
        options: airports,
      }
    },
    {
      label: 'Comment',
      name: 'Comment',
      className: 'col-span-12 mb-2',
      type: 'textarea'
    },
    {
      label: 'ETD',
      name: 'ETD',
      className: 'col-span-12 mb-2',
      type: 'check'
    },
  ]

  const columns = [
    {
      label: 'Travel Date',
      name: 'TravelDate',
      cellRender: (e) => (
        <div>{dayjs(e.value).format('YYYY-MM-DD ddd ')}</div>
      )
    },
    {
      label: 'Favor Time',
      name: 'FavorTime',
    },
    {
      label: 'Depart Location',
      name: 'DepartLocationName',
      alignment: 'left',
    },
    {
      label: 'Arrive Location',
      name: 'ArriveLocationName',
      alignment: 'left',
    },
    {
      label: 'Comment',
      name: 'Comment',
    },
    {
      label: 'ETD',
      name: 'ETD',
      alignment: 'left',
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0}/>
      )
    },
    {
      label: '',
      name: 'actionBtn',
      width: `${disabled ? '0px' : '150px'}`,
      cellRender: (e) => (
        !disabled &&
        <div className='flex gap-4'>
          <button type='button' className='edit-button' onClick={() => handleEditButton(e.data)}>Edit</button>
          <button type='button' className='dlt-button' onClick={() => handleDeleteButton(e.data)}>Delete</button>
        </div>
      )
    },
  ]

  const handleEditButton = (dataItem) => {
    setEditData(dataItem)
    setShowModal(true)
  }

  const handleDeleteButton = (dataItem) => {
    setEditData(dataItem)
    setShowPopup(true)
  }

  const handleDelete = () => {
    axios({
      method: 'delete',
      url: `tas/requestnonsitetravelflight/${editData.Id}`,
    }).then(() => {
      getData()
      setShowPopup(false)
    }).catch((err) => {

    })
  }

  const handleSubmit = (values) => {
    if(editData){
      setSubmitLoading(true)
      axios({
        method: 'put',
        url:'tas/requestnonsitetravelflight',
        data: {
          ...values,
          Id: editData.Id,
          DocumentId: documentId,
        }
      }).then((res) => {
        getData()
        setEditData(null)
        handleCancel()
      }).catch((err) => {
        
      }).then(() => setSubmitLoading(false))
    }
    else{
      setSubmitLoading(true)
      axios({
        method: 'post',
        url:'tas/requestnonsitetravelflight',
        data: {
          ...values,
          DocumentId: documentId,
        }
      }).then((res) => {
        getData()
        handleCancel()
      }).catch((err) => {
  
      }).then(() => setSubmitLoading(false))
    }
  }

  const handleCancel = () => {
    form.resetFields()
    setShowModal(false)
  }

  const handleAddButton = () => {
    setEditData(null)
    setShowModal(true)
  }

  const isEditable = useMemo(() => {
    let returnStatus = false
    if(currentGroup?.GroupTag === "requester" && documentDetail?.AssignedEmployeeId === userInfo?.EmployeeId && currentGroup?.OrderIndex === 2){
      returnStatus = true
    }
    return returnStatus
  }, [currentGroup, documentDetail, userInfo, state.userInfo])

  const getFinalOptionsData = () => {
    axios({
      method: 'get',
      url: `tas/requestnonsitetraveloption/final/${documentId}`,
    }).then((res) => {
      setFinalOptions(res.data)
    }).catch((err) => {

    }).finally(() => {
      // setLoading(false)
    })
  }

  return (
    <div className='col-span-1' id='flight'>
      <Tabs 
        type='card'
        activeKey={tabKey}
        destroyInactiveTabPane={false}
        defaultActiveKey={documentDetail?.CurrentStatus === 'Submitted' ? 1 : 2}
        onChange={(e) => setTabKey(e)}
        items={[
          {
            label: 'Request',
            key: 1,
            children: <div className='px-0 py-1 rounded-bl-ot rounded-br-ot border border-gray-300'>
              <Table
                tableRef={dataGrid}
                data={data?.FlightData}
                columns={columns}
                allowColumnReordering={false}
                id="room"
                className={`overflow-hidden ${!state.userInfo?.ReadonlyAccess && 'border-t'}`}
                showRowLines={true}
                rowAlternationEnabled={false}
                loading={loading}
                pager={data?.FlightData?.length > 20}
                containerClass='shadow-none'
                title={
                  <div className='flex justify-between items-center py-2 gap-3'>
                    <div className='text-md font-bold pl-2'>Flight</div>
                    <div className='flex gap-4 items-center'>
                      {
                        !disabled && data?.FlightData?.length === 0 &&
                        <Button 
                          icon={<PlusOutlined />} 
                          onClick={handleAddButton}
                          className='text-xs'
                          htmlType='button'
                        >
                          Add Flight
                        </Button>
                      }
                      <Button 
                        icon={<SyncOutlined />} 
                        onClick={getData}
                        loading={loading}
                        className='text-xs'
                        htmlType='button'
                      >
                        Refresh
                      </Button>
                    </div>
                  </div>
                }
              />
            </div>
          },
          data?.FlightData?.length > 0 && 
          {
            label: <div className='flex items-center gap-2'>
              <div>{documentDetail?.CurrentStatus === 'Completed' ? 'Final Itinerary' : 'Process Itinerary'}</div> 
              {
                !disabled && 
                (
                  documentDetail?.RuleAction?.split(',').includes('Create Flight Option') ? 
                  <Tooltip title='This tab has a required field' titleClass='bottom-full'>
                    <QuestionCircleOutlined className='text-gray-400 hover:text-gray-500 hover:cursor-help'/>
                  </Tooltip>
                  : null
                )
              }
            </div>,
            key: 2,
            children: documentDetail?.CurrentStatus === 'Completed' ? 
              <FinalItinerary
                flightData={data}
                data={finalOptions}
                getFinalOptionsData={getFinalOptionsData}
              />
               :
              <ProcessingItinerary
                getData={getData}
                flightData={data}
                currentGroup={currentGroup}
                isEditable={isEditable}
                documentData={documentDetail}
                userData={userInfo}
                hasActionPermission={hasActionPermission}
              />
          },
          documentDetail?.CurrentStatus === 'Completed' ?
          {
            label: 'Update Itinerary',
            key: 3,
            children: <UpdateItinerary
              getData={getData}
              setTabKey={setTabKey}
              getFinalOptionsData={getFinalOptionsData}
            />
          }
          : null
        ]}
      >
      </Tabs>
      <Modal title={editData ? 'Update Itinerary' : 'Add Itinerary'} open={showModal} onCancel={handleCancel}>
        <Form
          form={form}
          fields={fields}
          editData={editData}
          size='small' 
          className='grid grid-cols-12 gap-x-8' 
          onFinish={handleSubmit}
        >
          <div className='col-span-12 flex justify-end gap-3'>
            <Button htmlType='submit' loading={submitLoading} type={'primary'} icon={<SaveOutlined/>}>Save</Button>
            <Button htmlType='button' onClick={handleCancel}>Cancel</Button>
          </div>
        </Form>
      </Modal>
      <Popup
        visible={showPopup}
        showTitle={false}
        height={110}
        width={350}
      >
        <div>Are you sure you want to delete this record?</div>
        <div className='flex gap-5 mt-4 justify-center'>
          <Button htmlType='button' type='danger'onClick={handleDelete}>Yes</Button>
          <Button htmlType='button' onClick={() => setShowPopup(false)}>No</Button>
        </div>
      </Popup>
    </div>
  )
}

export default Flight