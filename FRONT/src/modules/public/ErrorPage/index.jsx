import React from 'react';
import { WarningTwoTone } from '@ant-design/icons';

function ErrorPage() {
  
  return (
    <div className='relative z-10 h-full rounded-ot flex flex-col justify-center items-center font-bold bg-white'>
      <WarningTwoTone twoToneColor={'#e57200'} style={{fontSize: 60}}/>
      <div className='text-2xl'>Something went wrong</div>
      {/* <Button onClick={() => navigate(-1)}>Go Back</Button> */}
    </div>
  )
}

export default ErrorPage