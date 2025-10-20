import { Form, Button } from 'components';
import React from 'react';
import { Form as AntForm } from 'antd';
import { LeftOutlined, SaveOutlined } from '@ant-design/icons';
import { twMerge } from 'tailwind-merge';

function ActionForm({form, editData, columns, onBack, onFinish, loading, title='', containerClassName='', formClass='', formContainerClass='', itemLayouts, initValues}) {

  const [ actionForm ] = AntForm.useForm()
  return (
    <div className={twMerge(`bg-white rounded-ot p-5 shadow-md`, containerClassName)}>
       <div className='text-lg font-bold mb-3'>{editData ? `Edit ${title}` : `Add ${title}`}</div>
      <div className={twMerge(`lg:w-1/2 xl:w-2/5 2xl:w-1/3`, formContainerClass)}>
        <Form 
          initValues={initValues}
          form={form ? form : actionForm} 
          // size='small' 
          editData={editData} 
          fields={columns.filter(el => !el.hide)} 
          onFinish={onFinish} 
          className={formClass}
          itemLayouts={itemLayouts}
        />
        <div className='gap-3 flex justify-end mt-3'>
          <Button onClick={onBack} icon={<LeftOutlined />}>Back</Button>
          <Button type='primary' onClick={() => form ? form.submit() : actionForm.submit()} loading={loading} icon={<SaveOutlined/>}>Save</Button>
        </div>
      </div>
    </div>
  )
}

export default ActionForm