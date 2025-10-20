import React, { useRef, useState } from 'react'
import { Modal as AntModal } from 'antd'
import { CloseOutlined } from '@ant-design/icons'
import Draggable from 'react-draggable';

function Modal({destroyOnClose=true, title, isDraggable=true, footer=null, ...props}) {
  const [disabled, setDisabled] = useState(true);
  const [ ss, setSs  ] = useState(null)
  const [bounds, setBounds] = useState({
    left: 0,
    top: 0,
    bottom: 0,
    right: 0,
  });
  const draggleRef = useRef(null);

  const onStart = (_event, uiData) => {
    const { clientWidth, clientHeight } = window.document.documentElement;
    const targetRect = draggleRef.current?.getBoundingClientRect();
    if (!targetRect) {
      return;
    }
    setSs(targetRect)
    setBounds({
      left: -targetRect.left + uiData.x - 200,
      right: clientWidth - (targetRect.right - uiData.x) + 200,
      top: -targetRect.top + uiData.y,
      bottom: clientHeight - (targetRect.bottom - uiData.y) + (targetRect.height - 50),
    });
  };

  return (
    <AntModal 
      footer={footer} 
      closeIcon={<div className='flex items-center justify-center'><CloseOutlined/></div>} 
      title={<div 
          className='leading-none pb-2 select-none'
          style={{width: '100%', cursor: isDraggable ? 'move' : 'default'}}
          onMouseOver={() => {
            if (disabled) {
              setDisabled(false);
            }
          }}
          onMouseOut={() => {
            setDisabled(true);
          }}
        >
          {title}
        </div>
      }
      styles={{mask: {
        background: 'rgba(0,0,0,0.2)'
      }}}
      // wrapClassName='overflow-hidden'
      destroyOnClose={destroyOnClose}
      modalRender={(modal) => (
        isDraggable ?
        <Draggable
          disabled={disabled}
          bounds={bounds}
          nodeRef={draggleRef}
          onStart={(event, uiData) => onStart(event, uiData)}
        >
          <div ref={draggleRef}>{modal}</div>
        </Draggable>
        : 
        <div ref={draggleRef}>{modal}</div>
      )}
      {...props}
    >
      {props.children}
    </AntModal>
  )
}

export default Modal