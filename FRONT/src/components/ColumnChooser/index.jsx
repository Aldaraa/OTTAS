import { DownOutlined } from '@ant-design/icons'
import { Checkbox, Dropdown } from 'antd'
import { useToken } from 'antd/es/theme/internal'
import { Button } from 'components'
import React from 'react'

const  ColumnChooser = React.memo(({options, checkedList=[], onChange, defaultColumns}) => {

  const token = useToken()
  const contentStyle = {
    backgroundColor: token.colorBgElevated,
    borderRadius: token.borderRadiusLG,
    boxShadow: token.boxShadowSecondary,
  };
  const onCheckAllChange = (e) => {
    onChange(e.target.checked ? defaultColumns : [])
  };

  return (
    <Dropdown 
      trigger={['click']}
      placement='bottomRight'
      dropdownRender={(menu) => (
        <div style={contentStyle}>
          <div className='bg-white shadow-mini-overlay p-2 rounded-ot'>
            <Checkbox.Group
              style={{
                width: '100%',
              }}
              value={checkedList}
              onChange={(e) => onChange(e)}
            >
              <div className='flex flex-col'>
                <Checkbox 
                  className='ml-2' 
                  // indeterminate={checkedList?.length > 0 && checkedList?.length < options?.length} 
                  // onChange={onCheckAllChange}
                  checked={checkedList?.length === options?.length}
                >
                  Check all
                </Checkbox>
                {
                  options?.map((item, index) => (
                    <Checkbox value={item.value} key={`column-item-${index}`}>{item.label}</Checkbox>
                    // <input type='checkbox' value={item.value} key={`column-item-${index}`}>{item.label}</input>
                  ))
                }
              </div>
            </Checkbox.Group>
          </div>
        </div>
      )}
    >
      <Button iconPosition='right' icon={<DownOutlined />}>
        Columns
      </Button>
    </Dropdown>
  )
}, (prevProps, curProps) => {
  if(JSON.stringify(prevProps.checkedList) !== JSON.stringify(curProps.checkedList)){
    return false
  }
  return true
})

export default ColumnChooser