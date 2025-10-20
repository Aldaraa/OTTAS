import { Divider, Input, Select, Space } from 'antd'
import { Form } from 'components'
import React from 'react'

function HasAdditionOptionSelect({onChangeItem}) {

  return (
    <Form.Item className='mb-4 col-span-12'>
      <Form.Item >

      </Form.Item>
      <Select
        style={{
          width: '100%',
        }}
        placeholder="custom dropdown render"
        dropdownRender={(menu) => (
          <>
            {menu}
            <Divider
              style={{
                margin: '8px 0',
              }}
            />
            <Space
              style={{
                padding: '0 8px 4px',
              }}
            >
              <Input
                placeholder="Please enter item"
                size='middle'
                onChange={(e) => onChangeItem(e.target.value)}
                onKeyDown={(e) => e.stopPropagation()}
              />
            </Space>
          </>
        )}
        options={[{label: 'Shangri-La', value: 'Shangri-La'}]}
      />
    </Form.Item>
  )
}

export default HasAdditionOptionSelect