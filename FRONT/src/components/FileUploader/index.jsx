import { Button } from 'components';
import React from 'react'

function FileUploader({children, accept, icon, onChange, loading, ...restprops}) {
  const hiddenFileInput = React.useRef(null);
  
  const handleClick = (event) => {
    hiddenFileInput.current.click();
  };

  const handleChange = async event => {
    await onChange(event.target.files[0])
    event.target.value = ''
  };

  return (
    <>
      <Button onClick={handleClick} icon={icon} loading={loading} {...restprops}>
        {children}
      </Button>
      <input 
        type="file"
        ref={hiddenFileInput}
        style={{display:'none'}} 
        onChange={handleChange}
        accept={accept}
      /> 
    </>
  );
}

export default FileUploader