import React, { useEffect, useRef, useState } from 'react'
import { PeolpeSelectionTable } from 'components';
import { Form as AntForm } from 'antd';
import { useLocation } from 'react-router-dom';
import BulkingView from './BulkingView';
import columns from './columns';

function ChangeGroup() {
  const routeLocation = useLocation();
  const [ selectedData, setSelectedData ] = useState([])
  const [ isEditing, setIsEditing ] = useState(false)

  const [ searchForm ] = AntForm.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
  },[])

  const handleSelect = (rows) => {
    setSelectedData(rows)
  }

  const handleReturn = () => {
    setIsEditing(true)
  }

  const handleRemove = (item, index) => {
    dataGrid.current?.instance.deselectRows([item.Id])
  }

  return (
    <div>
      <PeolpeSelectionTable
        ref={dataGrid}
        onSelect={handleSelect}
        onReturn={handleReturn}
        selectedRowsData={selectedData}
        className={`${isEditing ? 'hidden' : 'block' }`}
        columns={columns}
        searchDefaultValues={{Active: 1}}
        hideFields={['Active']}
      />
      <BulkingView
        data={selectedData} 
        changeIsEditing={setIsEditing}
        onRemove={handleRemove}
        handleChangeData={setSelectedData}
        className={`${isEditing ? 'block' : 'hidden' }`}
      />
    </div>
  )
}

export default ChangeGroup