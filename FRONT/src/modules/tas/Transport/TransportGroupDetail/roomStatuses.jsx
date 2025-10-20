import { Button, Table } from 'components'
import { CheckBox } from 'devextreme-react'
import React, { useState } from 'react'
import { useParams } from 'react-router-dom'
import axios from 'axios'

function RoomStatuses({data, handleClose, refreshData}) {
  const [ changedRows, setChangedRows ] = useState([])
  const [ loading, setLoading ] = useState(false)

  const {groupId} =  useParams()

  const columns = [
    {
      label: 'Code',
      name: 'Code',
      allowEditing: false,
    },
    {
      label: 'Description',
      name: 'Description',
      allowEditing: false,
    },
    {
      label: 'On Site',
      name: 'OnSite',
      width: '80px',
      alignment: 'center',
      allowEditing: false,
      cellRender: (e) => (
        <CheckBox disabled iconSize={18} value={e.value === 1 ? true : 0} />
      )
    },
    {
      name: "Allow",
      width: '80px',
      allowEditing: true,
      dataType: 'boolean',
      editorType: 'dxCheckBox',
      alignment: 'right',
      editorOptions: {
        onValueChanged: function(e) {
          e.component.option('value', e.value ? 1 : 0);
        }
      }
    },
  ]

  const handleSubmit = (event) => {
    let tmp = [];
    event.changes.map((item) => {
      tmp.push({allow: item.data.Allow, shiftId: item.key})
    })
    setLoading(true)
    axios({
      method: 'post',
      url: `tas/FlightGroupShift/${groupId}`,
      data: tmp,
    }).then((res) => {
      refreshData()
    }).catch((err) => {

    }).then(() => setLoading(false))
  }

  function onEditorPreparing(e) {
    if(e.dataField == "Allow" && e.parentType === "dataRow") {
      const defaultValueChangeHandler = e.editorOptions.onValueChanged;
      e.editorName = "dxCheckBox"; 
      e.editorOptions.onValueChanged = function (args) {  
        e.setValue(args.value)
        defaultValueChangeHandler(args);
      }
    }
  }

  return (
    <div>
      <Table
        data={data}
        columns={columns}
        keyExpr='ShiftId'
        containerClass='shadow-none'
        onEditorPreparing={onEditorPreparing}
        edit={{mode: 'batch', allowUpdating: true, startEditAction: 'dblClick'}}
        onSaving={handleSubmit}
      />
    </div>
  )
}

export default RoomStatuses