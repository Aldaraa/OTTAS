import axios from 'axios'
import { Button, Table } from 'components'
import { AuthContext } from 'contexts'
import React, { useContext, useEffect, useState } from 'react'

const columns = [
  {
    label: 'Code',
    name: 'Code',
    allowSorting: false
  },
  {
    label: 'Description',
    name: 'Description',
    allowSorting: false
  },
]

function Template({employeeData, refreshData}) {
  const [ data, setData ] = useState([])
  const [ selectedReportKeys, setSelectedReportKeys ] = useState([])
  const [ actionLoading, setActionLoading ] = useState(false)
  const { state } = useContext(AuthContext)

  useEffect(() => {
    if(employeeData)
    getReports()
  }, [employeeData])

  const getReports = () => {
    axios({
      method: 'get',
      url: `tas/sysroleemployeereporttemplate/${employeeData?.EmployeeId}`
    }).then((res) => {
      let tmp = []
      res.data.map((item) => {
        if(item.Permission){
          tmp.push(item.Id)
        }
      })
      setSelectedReportKeys(tmp)
      setData(res.data)
    }).catch((err) => {

    })
  }

  const handleSavePermission = () => {
    setActionLoading(true)
    let tmp = []
    selectedReportKeys.map((id) => {
      tmp.push({reportTemplateId: id, permission: 1})
    })
    axios({
      method: 'post',
      url: 'tas/sysroleemployeereporttemplate',
      data: {
        employeeId: employeeData?.EmployeeId,
        reportTemplatePermissions: [...tmp]
      }
    }).then((res) => {
      getReports()
      refreshData()
    }).catch((err) => {

    }).then(() => setActionLoading(false))
  }

  const handleSelect = (e) => {
    const selectedData = e.component.getSelectedRowsData('leavesOnly');
    let lastRowKeys = selectedData.map((item) => item.Id)
    setSelectedReportKeys(lastRowKeys)
  }

  const preparing = (e) => {
    if(state.userInfo?.ReadonlyAccess ){
      e.editorOptions.disabled = true;
    }
  }

  return (
    <div className='mt-3'>
      <Table
        dataSource={data}
        id="employees"
        className='border rounded-ot overflow-hidden'
        containerClass='px-0 shadow-none'
        keyExpr="Id"
        parentIdExpr="Id"
        columns={columns}
        showRowLines={true}
        selection={{mode: 'multiple', recursive: true, showCheckBoxesMode: 'always', allowSelectAll: true, selectAllMode: 'page'}}
        rowAlternationEnabled={false}
        selectedRowKeys={selectedReportKeys}
        onSelectionChanged={handleSelect}
        onEditorPreparing={preparing}
        autoExpandAll={true}
        pager={false}
        // tableClass='max-h-[calc(100vh-200px)]'
        // style={{maxHeight: 'calc(100vh-200px)'}}
      />
      {
        !state.userInfo?.ReadonlyAccess &&
        <div className='flex justify-end mt-4'>
          <Button type='primary' onClick={handleSavePermission} loading={actionLoading}>Save</Button>
        </div>
      }
    </div>
  )
}

export default Template