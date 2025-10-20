import { DeleteOutlined, PlusOutlined } from '@ant-design/icons'
import { Divider, Input, Popconfirm, Select, Space } from 'antd'
import axios from 'axios'
import { Button } from 'components'
import React, { useCallback, useEffect, useRef, useState } from 'react'

function BusStation({...restprops}) {
  const [ stations, setStations ] = useState([])
  const [ name, setName ] = useState('')
  const [ edititem, setEditItem ] = useState(null)
  const [ loading, setLoading ] = useState(false)
  const inputRef = useRef(null);

  
  useEffect(() => {
    getStations()
  },[])
  
  const getStations = useCallback(() => {
    axios({
      method: 'get',
      url: 'tas/busstop?Active=1',
    }).then((res) => {
      setStations(res.data)
    })
  },[])
  
  const onNameChange = useCallback((event) => {
    setName(event.target.value);
  },[])

  const addStation = (e) => {
    e.preventDefault();
    setLoading(true)
    axios({
      method: 'post',
      url: 'tas/busstop',
      data: {
        description: name,
        active: 1,
      }
    }).then((res) => {
      setName('')
      getStations()
      // setStations(res.data)
    }).catch(() => {

    }).then(() => {
      setLoading(false)
    })
    // setItems([...items, name || `New item`]);
    // setName('');
    // setTimeout(() => {
    //   inputRef.current?.focus();
    // }, 0);
  };

  const onClickDelete = useCallback((e, item) => {
    e.preventDefault();
    e.stopPropagation();
    setEditItem(item)
  },[])

  const handleDeleteStation = useCallback((e) => {
    e.preventDefault();
    e.stopPropagation();
    setLoading(true)
    axios({
      method: 'delete',
      url: `tas/busstop`,
      data: {
        id: edititem?.Id
      }
    }).then((res) => {
      getStations()
    }).catch((err) => {
      
    }).then(() => {
      setLoading(false)
    })
  },[edititem])

  const onPopupClick = useCallback((e) => {
    e.stopPropagation();
    e.preventDefault();
  },[])
  
  return (
     <Select
        {...restprops}
        className='w-full'
        options={stations.map((item) => ({...item, value: item.Description, label: item.Description}))}
        style={{width: 250}}
        popupMatchSelectWidth={false}
        optionRender={(option) => (
          <div className='flex justify-between items-center'>
            <span>{option.data.label}</span>
            <Popconfirm 
              title='Are you sure to remove ?'
              onConfirm={handleDeleteStation}
              cancelButtonProps={{ style: {backgroundColor: 'white'}}}
              okButtonProps={{loading: loading}}
              onPopupClick={onPopupClick}
              // onCancel={(e) => console.log('e', e)}
            >  
              <Button type='danger' className='px-1' onClick={(e) => onClickDelete(e, option.data)}><DeleteOutlined/></Button>
            </Popconfirm>
          </div>
        )}
        dropdownRender={(menu) => (
          <>
            {menu}
            <Divider style={{ margin: '8px 0' }} />
            <Space style={{ padding: '0 8px 4px' }}>
              <Input
                placeholder="Please enter item"
                ref={inputRef}
                value={name}
                onChange={onNameChange}
                onKeyDown={(e) => e.stopPropagation()}
              />
              <Button type="text" icon={<PlusOutlined />} loading={loading} onClick={addStation}>
                Add
              </Button>
            </Space>
          </>
        )}
      >
      </Select>
  )
}

export default BusStation