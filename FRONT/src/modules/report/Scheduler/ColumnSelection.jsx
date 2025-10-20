import React, { useEffect, useState } from 'react';
import { Transfer } from 'antd';
import { Button } from 'components';

const ColumnSelection = ({data, onCancel, onSave, values}) => {
  const [ dataSource, setDataSource ] = useState([])
  const [targetKeys, setTargetKeys] = useState(values);

  useEffect(() => {
    if(data){
      setDataSource(data)
    }
  },[data])

  const handleChange = (newTargetKeys, direction, moveKeys) => {
    setTargetKeys(newTargetKeys);
  };

  const filterOption = (inputValue, option) => (option?.title ?? '').toLowerCase().includes(inputValue.toLowerCase());

  const handleCancel = () => {
    if(onCancel){
      onCancel()
    }
  }
  const handleSave = () => {
    if(onSave){
      onSave(targetKeys)
    }
  }

  return (
    <>
      <Transfer
        dataSource={dataSource}
        titles={['Columns', 'Selected Columns']}
        targetKeys={targetKeys}
        onChange={handleChange}
        showSearch
        filterOption={filterOption}
        render={(item) => item.title}
        oneWay
        listStyle={{
          width: 400,
          height: 400,
        }}
        style={{
          marginBottom: 16,
        }}
      />
      <div className='flex justify-end gap-3'>
        <Button type='primary' onClick={handleSave}>OK</Button>
        <Button onClick={handleCancel}>Cancel</Button>
      </div>
    </>
  );
};
export default ColumnSelection;