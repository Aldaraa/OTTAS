import { Table } from 'components'
import React, { useContext, useEffect, useRef, useState } from 'react'
import axios from 'axios'
import { Link } from 'react-router-dom'
import { AuthContext } from 'contexts'

function Roles() {
  const [ renderData, setRenderData ] = useState([])

  const { action } = useContext(AuthContext)
  const gridRef = useRef(null)
  
  useEffect(() => {
    action.changeMenuKey('/tas/roles')
    getData()
  },[])

  const getData = () => {
    gridRef.current?.instance.beginCustomLoading();
    axios({
      method: 'get',
      url: 'tas/sysrole'
    }).then((res) => {
      setRenderData(res.data)
    }).catch((err) => {

    }).then(() => gridRef.current?.instance.endCustomLoading())
  }

  const columns = [
    {
      label: 'Name',
      name: 'Name',
      cellRender: (e) => (
        e.value === 'Guest' ?
        <div>{e.value}</div>
        :
        <div className='flex items-center'>
          <Link to={`/tas/roles/${e.data.Id}`}>
            <button type='button' className='text-blue-500 hover:underline'>{e.value}</button>
          </Link>
        </div>
      )
    },
    {
      label: 'Description',
      name: 'Description'
    },
    {
      label: '# Resource',
      name: 'EmployeeCount',
      alignment: 'left',
      cellRender: (e) => (
        <span>{e.value === -1 ? '-' : e.value}</span>
      )
    },
  ]

  return (
    <div>
      <Table
        ref={gridRef}
        data={renderData}
        columns={columns}
        allowColumnReordering={false}
        keyExpr='Id'
        pager={false}
      />
    </div>
  )
}

export default Roles