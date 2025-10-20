import React, { useEffect, useRef, useState } from 'react'
import { PeolpeSelectionTable } from 'components';
import { Form as AntForm } from 'antd';
import { useLocation } from 'react-router-dom';
import BulkingView from './BulkingView';
import columns from './columns';


function BulkRosterExecute() {
  const routeLocation = useLocation();
  const [ selectedData, setSelectedData ] = useState([])
  const [ currentSelectedData, setCurrentSelectedData ] = useState([])
  const [ isEditing, setIsEditing ] = useState(false)
  const [ searchForm ] = AntForm.useForm()
  const dataGrid = useRef(null)

  useEffect(() => {
    if(routeLocation.state){
      searchForm.setFieldValue(routeLocation.state.name, routeLocation.state.value)
    }
  },[])


  const handleSelect = (rows) => {
    setCurrentSelectedData(rows)
  }
  
  const handleReturn = () => {
    const selectedData = currentSelectedData.map((item) => ({
      ...item,
      Fullname: `${item.Firstname} ${item.Lastname}`,
    }))
    setSelectedData(selectedData)
    setIsEditing(true)
  }

  const handleChangeData = (data) => {
    setSelectedData(data)
  }
  return (
    <div>
        <BulkingView 
          data={selectedData} 
          handleChangeData={handleChangeData}
          changeIsEditing={setIsEditing}
          className={`${isEditing ? 'block' : 'hidden' }`}
          setSelectedData={setSelectedData}
          selectionTableRef={dataGrid}
        />
        <div className={`${isEditing ? 'hidden' : 'block' }`}>
          <PeolpeSelectionTable
            ref={dataGrid}
            onSelect={handleSelect}
            onReturn={handleReturn}
            selectedRowsData={currentSelectedData}
            columns={columns}
            searchDefaultValues={{Active: 1}}
            hideFields={['Active']}
          />
        </div>
        
    </div>
  )
}

export default BulkRosterExecute