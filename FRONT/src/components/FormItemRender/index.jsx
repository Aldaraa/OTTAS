import React, { useCallback, useContext, useEffect, useState } from 'react'
import { Form,
  Input,
  DatePicker,
  Select,
  Upload,
  Button,
  InputNumber,
  TreeSelect,
  Switch,
  Checkbox,
  Image, } from 'antd'
  import { DeleteOutlined, UploadOutlined } from '@ant-design/icons';
import ISelect from 'components/Select';
import { SearchSelection, Tooltip } from 'components';
import { Popup } from 'devextreme-react';
import axios from 'axios';
import { AuthContext } from 'contexts';

const cyrillic = new RegExp(/([A-Za-z])/g)
  
const getLabel = (_field) => {
  return (
    <span
      className={`${
        _field.inputprops?.disabled ? 'text-[#888]' : ''
      }`}
    >
      {_field.label}
    </span>
  );
};

const renderElement = (obj) => {
  if (!obj) return null;

  const { type, props } = obj;
  const children = Array.isArray(props.children)
    ? props.children.map((child, index) => {
        if (typeof child === 'object') {
          return renderElement(child);
        } else {
          return child;
        }
      })
    : props.children;

  return React.createElement(type, props, children);
};

const DeleteButtonProfileImage = ({title, Id, formInterceptor, name, imageUrl}) => {
  const [ image, setImage ] = useState(imageUrl)
  const [ actionLoading, setActionLoading ] = useState(false)
  const [ showPopup, setShowPopup ] = useState(false)
  const { action, state } = useContext(AuthContext)

  useEffect(() => {
    return () => {
      if(imageUrl && !image){
        action.saveUserProfileData({...state.userProfileData, PassportImage: null})
      }
    }
  },[image, imageUrl])

  useEffect(() => {
    setImage(imageUrl)
  },[imageUrl])

  const handleDelete = useCallback(() => {
    setActionLoading(true)
    axios({
      method: 'delete',
      url: `tas/employee/removepassportimage`,
      data: {
        employeeId: Id,
      }
    }).then((res) => {
      setImage(null)
      setShowPopup(false)
    }).catch((err) => {

    }).finally(() => setActionLoading(false))
  },[Id, formInterceptor])

  const onClickDeleteBtn = useCallback((e) => {
    e.stopPropagation()
    setShowPopup(true)
  },[])

  const handleCancel = useCallback((e) => {
    e.stopPropagation()
    setShowPopup(false)
  },[])



  return(
    image ?
    <div className='inline-block mt-3 relative'>
      <Image
        className="max-w-[200px] rounded-ot"
        src={image}
      />
      <div className='absolute right-0 bottom-0'>
        <Tooltip title='Delete image'>
          <Button danger type='primary' className='flex' icon={<DeleteOutlined/>} onClick={onClickDeleteBtn}/>
        </Tooltip>
        <Popup
          visible={showPopup}
          showTitle={false}
          height={'auto'}
          width={350}
        >
          <div>
            <div>Are you sure you want to delete?</div>
            <div className='flex gap-5 mt-3 justify-center'>
              <Button danger onClick={handleDelete} loading={actionLoading}>Yes</Button>
              <Button onClick={handleCancel} disabled={actionLoading}>No</Button>
            </div>
          </div>
        </Popup>
      </div>
    </div>
    : null
  )
}
  
function FormItemRender({fields=[], form, editData, getData, ...restProps}) { 

  const renderItem = (_field, _fieldIndex) => {
    switch (_field?.type) {
      case 'date':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
            <DatePicker {..._field.inputprops} />
          </Form.Item>
        );
      case 'select':
        if (_field.depindex) {
          return (
            <Form.Item
              key={`cform-item-f${_field.name}-${_fieldIndex}`}
              noStyle
              shouldUpdate={(preValue, curValue) =>
                preValue[_field.depindex] !==
                curValue[_field.depindex]
              }
              className={_field.className}
            >
              {({ getFieldValue, setFieldValue }) => {
                return (
                  <Form.Item
                    key={`cform-item-${_field.name}-${_fieldIndex}`}
                    {..._field}
                    label={getLabel(_field)}
                    
                  >
                    <ISelect
                      setFieldValue={setFieldValue}
                      getFieldValue={getFieldValue}
                      _field={_field}
                      name={_field.name}
                      optionsurl={_field.inputprops.optionsurl}
                      dependentvalue={getFieldValue(_field.depindex)}
                      // placeholder={`${_field.label} сонгох`}
                      showSearch={_field.inputprops.showSearch}
                      filterOption={(input, option) =>
                        (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
                      }
                      {..._field.inputprops}
                    />
                  </Form.Item>
                );
              }}
            </Form.Item>
          );
        } else {
          return (
            <Form.Item
              {..._field}
              key={`cform-item-f${_field.name}-${_fieldIndex}`}
              label={getLabel(_field)}
            >
              <Select 
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                className={_field.inputprops.className}
                {..._field.inputprops}
              >
                {/* {_field.inputprops.options?.map((item, index) => (
                  <Select.Option 
                    key={index} 
                    value={item.value} 
                    label={item.label}
                  >
                      {item.content ? item.content : item.label}
                    </Select.Option>
                ))} */}
              </Select>
            </Form.Item>
          );
        }
      case 'treeSelect':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
            <TreeSelect {..._field.inputprops} />
          </Form.Item>
        );
      case 'searchSelect':{
        if(editData){
          form?.setFieldValue(
            _field.name, 
            editData[_field.name] ? 
            _field.inputprops?.editName ?
            `${editData[_field.inputprops.editName[0]]} ${editData[_field.inputprops.editName[1]]}`
            :
            null
            :
            null
          )
        }
        return (
          <Form.Item
            {..._field}
            key={`form-item-${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
            trigger='onChange'
          >
              <SearchSelection {..._field.inputprops}/>
          </Form.Item>
        );
      }
      case 'files': 
        return (
          <Form.Item {..._field} valuePropName="file" key={`cform-item-f${_field.name}-${_fieldIndex}`}>
              <Upload beforeUpload {..._field.inputprops}>
                <Button icon={<UploadOutlined />}>File Upload</Button>
              </Upload>
          </Form.Item>
        )
      case 'image':
        return (
          <div className={`flex flex-col ${_field.className}`} key={`cform-item-f${_field.name}-${_fieldIndex}`}>
            <Form.Item
              valuePropName="file"
              key={`cform-item-c${_field.name}-${_fieldIndex}`}
              {..._field}
              label={getLabel(_field)}
              >
              <Upload beforeUpload maxCount={1} accept='.pdf, .jpg, .jpeg, .png, .gif, .bmp, .webp' {..._field.inputprops}>
                <Button icon={<UploadOutlined />}>Upload Image</Button>
              </Upload>
            </Form.Item>
            <Form.Item key={`cform-item-f-label${_field.name}-${_fieldIndex}`} labelCol={_field.labelCol}  label={<div>Preview</div>}>
              {editData && editData[_field.name === 'PassportRawImage' ? 'PassportImage' : _field.name] ? 
                <DeleteButtonProfileImage 
                  Id={editData?.Id}
                  formInterceptor={form}
                  name={_field.name}
                  imageUrl={`${process.env.REACT_APP_CDN_URL}${editData[_field.name === 'PassportRawImage' ? 'PassportImage' : _field.name]}`}
                />
              : null}
            </Form.Item>
          </div>
        );
      case 'textarea':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
            <Input.TextArea
              // placeholder={`${_field.label} оруулах`}
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'phone':
        return (
          <Form.Item
            key={`form-item-${_field.name}-${_fieldIndex}`}
            {..._field}
            label={getLabel(_field)}
          >
            <InputNumber
              stringMode
              min={0}
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'number':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
            <InputNumber
              controls={false}
              {..._field.inputprops}
            />
          </Form.Item>
        );
        
      case 'price': 
        return(
          <Form.Item 
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
            <InputNumber
              controls={false}
              formatter={value => `${new Intl.NumberFormat().format(value)}`}
              // placeholder={`${_field.label} оруулах`}
              {..._field.inputprops}
            />
          </Form.Item>
        )
      case 'cyrillic':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
            rules={[
              ..._field.rules,
              ({getFieldValue}) => ({
                validator(_, value) {
                  if(cyrillic.test(value)) {
                    return Promise.reject(new Error("Please use only cyrillic characters"))
                  }
                  return Promise.resolve()
                }
              })
            ]}
          >
            <Input
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'stringNumber':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
            onKeyPress={(event) => {
              if (!/[0-9]/.test(event.key)) {
                  event.preventDefault();
              }
            }}
          >
            <Input
              // placeholder={`${_field.label} оруулах`}
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'switch':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
            valuePropName="checked"
            >
            <Switch {..._field.inputprops} />
          </Form.Item>
        );
      case 'percent':
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
          <InputNumber
            controls={false}
            formatter={(value) => `${value}%`}
            parser={(value) => value.replace('%', '')}
            // placeholder={`${_field.label} оруулах`}
            {..._field.inputprops}
            />
        </Form.Item>
      );
      case 'check':
        return (
        <Form.Item
          {..._field}
          valuePropName="checked"
          key={`cform-item-f${_field.name}-${_fieldIndex}`}
          label={_field.label}
        >
          <Checkbox
            {..._field.inputprops}
            onChange={(e) => form.setFieldValue(_field.name, e.target.checked ? 1 : 0)}
          >
          </Checkbox>
        </Form.Item>
      );
      case 'component':
        return _field.component;
      default:
        return (
          <Form.Item
            {..._field}
            key={`cform-item-f${_field?.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
            <Input
              {..._field.inputprops}
            />
          </Form.Item>
        );
    }
  };

  return (
    <React.Suspense>
      { 
        fields.map((item, index) => (
          item ? renderItem(item, index) : null
        ))
      }
    </React.Suspense>
  )
}

export default FormItemRender