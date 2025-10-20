import { UploadOutlined, CloseOutlined, FileOutlined } from '@ant-design/icons';
import PropTypes from 'prop-types';
import {
  Form,
  Input,
  Tabs,
  DatePicker,
  Select,
  Upload,
  Button,
  InputNumber,
  Divider,
  TreeSelect,
  Switch,
  Checkbox,
  Row,
  Col,
  TimePicker,
  Radio
} from 'antd';
import React, { useEffect } from 'react';
import ISelect from 'components/Select';
import dayjs from 'dayjs'
import FormCollapse from 'components/Collapse';
import { useState } from 'react';
import { ColorBox } from 'devextreme-react';
import { CustomCheck, RoomSelection, SearchSelection, Tooltip } from 'components';
import { twMerge } from 'tailwind-merge';
import PhoneInput from 'antd-phone-input';

const { CustomPanel } = FormCollapse;

const cyrillic = new RegExp(/([A-Za-z])/g)
// const registrRegex = new RegExp(/[A-Za-z][A-Za-z][0-9]+/i)

const formItemLayout = {
  labelCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 8,
    },
  },
  wrapperCol: {
    xs: {
      span: 24,
    },
    sm: {
      span: 16,
    },
  },
};

const CustomForm = ({ fields, editData, itemLayouts, initValues, noLayoutConfig, className='', ...restProps }) => {
  const [antForm] = Form.useForm();
  const [values, setValues] = useState([]);
  const [ isAddFile, setIsAddFile ] = useState(false)
  const [ isEdit, setIsEdit ] = useState(true)
  const form = restProps.form || antForm;

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

  const reinitValues = () => {
    if(editData){
      setValues(editData)
      form.setFieldsValue(editData)
      fields?.forEach((item) => {
        if (item.type === 'date') {
          if(editData[item.name]){
            form.setFieldValue(item.name, editData[item.name] ? dayjs(editData[item.name]) : null);
          }
          else{
            form.setFieldValue(item.name, '');
          }
        } else if (item.type === 'collapse') {
          item.children?.forEach((child, i) => { 
            if (child.children) {
              child.children?.forEach((childItem) => {
                if(childItem.type === 'component') {
                  if(childItem.inputType === 'date'){
                    childItem.names?.forEach((name) => {
                      if(editData[name]){
                        form.setFieldValue(
                          name,
                          dayjs(editData[name])
                        );
                      }
                    })
                  }
                }
                else if (childItem.type === 'date') {
                  form.setFieldValue(
                    childItem.name,
                    editData[childItem.name] ? dayjs(editData[childItem.name]) : null
                  );
                }else{
                  form.setFieldValue(childItem?.name, editData[childItem?.name]);
                }
              });
            }
          });
        }
        // else if(item.type === 'check'){
        //   if(item.inputprops?.indeterminatewith){
        //     if(editData[item.name] === null){
        //       setIndeterminate(true)
        //     }
        //   }
        // }
        else if(item.type === 'component') {
          if(item.inputType === 'date'){
            item.names?.forEach((name) => {
              form.setFieldValue(
                name,
                editData[name] ? dayjs(editData[name]) : null
              );
            })
          }
        }
        else if(item.type === 'select'){
          form.setFieldValue(item.name, editData[item.name] ? editData[item.name] : null);
        }
        else if(item.type === 'treeSelect'){
          form.setFieldValue(item.name, editData[item.name] ? editData[item.name] : null);
        }
        else{
          form.setFieldValue(item.name, editData[item.name]);
        }
      });
    }
  }

  useEffect(() => {
    if(editData){
      reinitValues()
    }
  }, [editData]); // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    if(!restProps.disabled){
      setIsEdit(restProps.disabled)
    }
    else{
      if(!isEdit){
        reinitValues()
      }
      setIsEdit(restProps.disabled)
    }
    
  },[restProps.disabled, isEdit]) // eslint-disable-line react-hooks/exhaustive-deps

  const handleChangeEveryday = (checked, name) => {
    if(checked){
      form.setFieldValue(name, [
        'Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday',
      ])
    }
    else{
      form.setFieldValue(name, [])
    }
  }
  
  const handleChangeWeekday = (checked, name) => {
    if(checked){
      form.setFieldValue(name, [
        'Monday','Tuesday','Wednesday','Thursday','Friday'
      ])
    }else{
      form.setFieldValue(name, [])
    }
  }

  const handlePressEnter = (e) => {
    if(!e.shiftKey){
      e.preventDefault()
      form.submit()
    }
  }
  

  const renderFormItem = (_field, _fieldIndex) => {
    switch (_field.type) {
      case 'collapse':
        return (
          <FormCollapse
            defaultActiveKey={['0']}
            {..._field}
            key={`form-collapse-${_fieldIndex}`}
          >
            {_field.children?.map((item, itemIndex) => (
              <CustomPanel
                key={`${itemIndex}`}
                fields={item.children}
                label={item.label}
                form={form}
                values={values}
                hidePercent={_field.hidePercent}
              >
                <div className="grid grid-cols-12 gap-4">
                  {item.children &&
                    item.children.map((child, childIndex) =>
                      renderFormItem(child, childIndex)
                    )}
                </div>
              </CustomPanel>
            ))}
          </FormCollapse>
        );
      case 'tabs':
        return <Tabs></Tabs>;
      case 'date':
        return (
          <Form.Item
            {..._field}
            key={`form-item-${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
          >
            <DatePicker {..._field.inputprops} />
          </Form.Item>
        );
      case 'rangeDate':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <DatePicker.RangePicker {..._field.inputprops} />
          </Form.Item>
        );
      case 'time':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <TimePicker {..._field.inputprops} onSelect={(e) => form.setFieldValue(_field.name, e)} />
          </Form.Item>
        );
      case 'select':
        if (_field.depindex) {
          return (
            <Form.Item
              noStyle
              shouldUpdate={(preValue, curValue) =>
                preValue[_field.depindex] !==
                curValue[_field.depindex]
              }
              className={_field.className}
              key={`form-item-${_field.name}-${_fieldIndex}`}
            >
              {({ getFieldValue, setFieldValue }) => {
                return (
                  <Form.Item
                    {..._field}
                    label={getLabel(_field)}
                    key={`form-item-c${_field.name}-${_fieldIndex}`}
                    
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
              label={getLabel(_field)}
              key={`form-item-${_field.name}-${_fieldIndex}`}
            >
              <Select 
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
                popupMatchSelectWidth={false}
                // style={_field.inputprops?.style}
                // className={_field.inputprops?.className}
                // optionlabelProp="label"
                // loading={_field.inputprops?.loading}
                {..._field.inputprops}
              >
                {/* {_field.inputprops.options?.map((item, index) => (
                  <Option key={`${_field.name}-${index}`} value={item.value} label={item.label}>
                    {item.content ? item.content : item.label}
                  </Option>
                ))} */}
              </Select>
            </Form.Item>
          );
        }

      case 'searchSelect':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <SearchSelection {..._field.inputprops}/>
          </Form.Item>
        );
      case 'treeSelect':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <TreeSelect {..._field.inputprops} />
          </Form.Item>
        );
      case 'file':
        return (
          <>
          {/* // < className={`flex gap-5 items-start ${_field.className}`} key={`form-item-${_field.name}-${_fieldIndex}`}> */}
            {editData && editData[_field.name] ? (
              <>
                {
                  isAddFile ?
                  <div className='w-full flex justify-between' key={_fieldIndex}>
                    <Form.Item
                      valuePropName="file"
                      {..._field}
                      label={getLabel(_field)}
                      key={`form-item-c${_field.name}-${_fieldIndex}`}
                    >
                      <Upload beforeUpload maxCount={1} {..._field.inputprops}>
                        <Button icon={<UploadOutlined />}>Upload File</Button>
                      </Upload>
                    </Form.Item>
                    <Button className='mt-7' type='primary' onClick={() =>setIsAddFile(false)}>
                      <CloseOutlined/>
                    </Button>
                  </div>
                  :
                  <div className='w-full flex justify-between gap-5' key={`form-item-c${_field.name}-${_fieldIndex}`}>
                    <div>
                      <label className='block mb-2'>{getLabel(_field)}</label>
                      <Tooltip title={editData[_field.name]}>
                        <a
                          className="mt-2 flex items-start gap-2"
                          href={`${process.env.REACT_APP_CDN_URL}${
                          editData[_field.name]
                          }`}
                          target="_blank"
                          rel="noreferrer"
                        >
                          <FileOutlined size={30}/> <span className='text-xs'>{editData[_field.name]}</span>
                        </a>
                      </Tooltip>
                    </div>
                    <Button className='mt-7' type='primary' onClick={() => setIsAddFile(true)}>Солих</Button>
                  </div>
                }
              </>
            ) : 
            <Form.Item
              valuePropName="file"
              {..._field}
              label={getLabel(_field)}
              key={`form-item-c${_field.name}-${_fieldIndex}`}
            >
              <Upload beforeUpload maxCount={1} {..._field.inputprops}>
                <Button icon={<UploadOutlined />}>Upload File</Button>
              </Upload>
            </Form.Item>
            }
          </>
        );
      case 'files': 
        return (
          <Form.Item valuePropName="file" {..._field} key={`form-item-${_field.name}-${_fieldIndex}`}>
              <Upload beforeUpload {..._field.inputprops}>
                <Button icon={<UploadOutlined />}>File Upload</Button>
              </Upload>
          </Form.Item>
        )
      case 'image':
        return (
          <div className={`flex flex-col ${_field.className}`} key={`form-item-${_field.name}-${_fieldIndex}`}>
            <Form.Item
              valuePropName="file"
              key={`form-item-c${_field.name}-${_fieldIndex}`}
              {..._field}
              label={getLabel(_field)}
            >
              <Upload beforeUpload maxCount={1} accept='.pdf, .jpg, .jpeg, .png, .gif, .bmp, .webp' {..._field.inputprops}>
                <Button icon={<UploadOutlined />}>Upload Image</Button>
              </Upload>
            </Form.Item>
            {editData && editData[_field.name] ? (
              <img
                alt='uploaded-img'
                className="max-w-[100px]"
                src={`${process.env.REACT_APP_CDN_URL}${editData[_field.name]}`}
              />
            ) : null}
          </div>
        );
      case 'textarea':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <Input.TextArea
              // placeholder={`${_field.label} оруулах`}
              onPressEnter={handlePressEnter}
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'number':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <InputNumber
              min={0}
              controls={false}
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'phone':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <PhoneInput
              enableSearch
              min={0}
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'price': 
        return(
          <Form.Item 
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
          >
            <InputNumber
              controls={false}
              formatter={value => `${new Intl.NumberFormat().format(value)}`}
              {..._field.inputprops}
            />
          </Form.Item>
        )
      case 'cyrillic':
        return (
          <Form.Item
            {..._field}
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
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
            label={getLabel(_field)}
            key={`form-item-${_field.name}-${_fieldIndex}`}
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
      case 'latin':
        return (
          <Form.Item
            {..._field}
            key={`form-item-${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
            onKeyPress={(event) => {
              if (!/[A-aZ-a]/.test(event.key)) {
                event.preventDefault();
              }
            }}
          >
            <Input
              {..._field.inputprops}
            />
          </Form.Item>
        );
      case 'switch':
        return (
          <Form.Item
            {..._field}
            key={`form-item-${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
            valuePropName="checked"
            >
            <Switch {..._field.inputprops} />
          </Form.Item>
        );
      case 'color':
        return (
          <Form.Item
            {..._field}
            key={`form-item-${_field.name}-${_fieldIndex}`}
            label={getLabel(_field)}
            valuePropName="value"
          >
            <ColorBox {..._field.inputprops} onValueChange={(e) => form.setFieldValue(_field.name, e)}/>
          </Form.Item>
        );
        case 'percent':
          return (
            <Form.Item
              {..._field}
              label={getLabel(_field)}
              key={`form-item-${_field.name}-${_fieldIndex}`}
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
        case 'password':
          return (
            <Form.Item
              {..._field}
              key={`form-item-${_field.name}-${_fieldIndex}`}
              label={getLabel(_field)}
            >
              <Input.Password
                {..._field.inputprops}
              />
            </Form.Item>
          );
        case 'radio':
          return (
            <Form.Item
              {..._field}
              key={`form-item-${_field.name}-${_fieldIndex}`}
              label={getLabel(_field)}
            >
              <Radio.Group {..._field.inputprops}/>
            </Form.Item>
          );
        case 'check':
          return (
            _field.inputprops?.indeterminatewith ? 
            <Form.Item
              valuePropName="value"
              {..._field}
              key={`form-item-${_field.name}-${_fieldIndex}`}
              label=''
            >
              <CustomCheck
                {..._field.inputprops}
                form={form}
                _field={_field}
                editData={editData}
              >
                <span className='text-xs'>{_field.label}</span>
              </CustomCheck>
            </Form.Item>
            :
            <Form.Item
              valuePropName="checked"
              {..._field}
              key={`form-item-${_field.name}-${_fieldIndex}`}
              label=''
            >
              <Checkbox
                {..._field.inputprops}
                onChange={(e) => {form.setFieldValue(_field.name, e.target.checked ? 1 : 0);}}
                indeterminate={!!_field.inputprops?.indeterminatewith}
              >
                <span className='text-xs'>{_field.label}</span>
              </Checkbox>
            </Form.Item>
            
        );
        case 'room':
          return(
            <RoomSelection {..._field} form={form}/>
          )
        case 'checkDays':
          return (
            <div {..._field} key={`form-item-${_field.name}-${_fieldIndex}`}>
              <Form.Item label='Week day' className='mb-0'>
                <Checkbox className='w-1/2 mb-2' onChange={(e) => handleChangeEveryday(e.target.checked, _field.name)}>
                  Everyday 
                </Checkbox>
                <Checkbox className='mb-2' onChange={(e) => handleChangeWeekday(e.target.checked, _field.name)}>
                  Weekday
                </Checkbox>
                <Form.Item name={_field.name} noStyle rules={_field.rules}>
                  <Checkbox.Group style={{width: '100%'}}>
                    <Row className='w-full gap-y-2'>
                      <Col span={4.8}>
                        <Checkbox value={'Monday'}>Mon</Checkbox>
                      </Col>
                      <Col span={4.8}>
                        <Checkbox value={'Tuesday'}>Tue</Checkbox>
                      </Col>
                      <Col span={4.8}>
                        <Checkbox value={'Wednesday'}>Wed</Checkbox>
                      </Col>
                      <Col span={4.8}>
                        <Checkbox value={'Thursday'}>Thu</Checkbox>
                      </Col>
                      <Col span={4.8}>
                        <Checkbox value={'Friday'}>Fri</Checkbox>
                      </Col>
                      <Col span={5}>
                        <Checkbox value={'Saturday'}>Sat</Checkbox>
                      </Col>
                      <Col span={5}>
                        <Checkbox value={'Sunday'}>Sun</Checkbox>
                      </Col>
                    </Row>
                  </Checkbox.Group>
                </Form.Item>
              </Form.Item>
            </div>
        );
      case 'component':
        return _field.component;
      case 'divider':
        return (
          <Form.Item {..._field} key={`form-item-${_field.name}-${_fieldIndex}`}>
            <Divider key={`divider-${_field.label}`} {..._field.inputprops}>
              {_field.text}
            </Divider>
          </Form.Item>
        );
      default:
        return (
          <Form.Item
            key={`form-item-${_field.name}-${_fieldIndex}`}
            {..._field}
            label={getLabel(_field)}
          >
            <Input
              {..._field.inputprops}
            />
          </Form.Item>
        );
    }
  };

  const initialValues = () => {
    let tmp = {}
    fields?.forEach((item) => {
      if(item.children){
        item.children?.forEach((child) => {
          if(child.children){
            child.children?.forEach((child) => {
              if(child.type === 'date'){
                tmp[child.name] = '';
              }
              else if(child.type === 'select'){
                if(child.inputprops?.mode === 'multiple'){
                  tmp[child.name] = [];
                }else{
                  tmp[child.name] = null;
                }
              }
              else if(child.type === 'treeSelect'){
                tmp[child.name] = null;
              }
              else if(child.type === 'switch'){
                tmp[child.name] = false;
              }
              else if(child.type === 'check'){
                tmp[child.name] = 0;
              }
            })
          }
          else{
            if(child.type === 'date'){
              tmp[child.name] = '';
            }
            else if(child.type === 'select'){
              if(child.inputprops?.mode === 'multiple'){
                tmp[child.name] = [];
              }else{
                tmp[child.name] = null;
              }
            }
            else if(child.type === 'treeSelect'){
              tmp[child.name] = null;
            }
            else if(child.type === 'switch'){
              tmp[child.name] = false;
            }
            else if(child.type === 'check'){
              tmp[child.name] = 0;
            }
          }
        })
      }
      else{
        if(item.type === 'date'){
          tmp[item.name] = '';
        }
        else if(item.type === 'select'){
          if(item.inputprops?.mode === 'multiple'){
            tmp[item.name] = [];
          }else{
            tmp[item.name] = null;
          }
        }
        else if(item.type === 'treeSelect'){
          tmp[item.name] = null;
        }
        else if(item.type === 'switch'){
          tmp[item.name] = false;
        }
        else if(item.type === 'check'){
          if(item.name === 'Active'){
            tmp[item.name] = 1;
          }
          else{
            tmp[item.name] = 0;
          }
        }
        else{
          tmp[item.name] = '';
        }
      }
    })
    return tmp
  }

  return (
    <Form
      labelCol={!noLayoutConfig && formItemLayout.labelCol}
      wrapperCol={!noLayoutConfig && formItemLayout.wrapperCol}
      labelAlign='left'
      className={twMerge('grid grid-cols-12',className)}
      initialValues={initValues ? initValues : initialValues()}
      {...itemLayouts}  
      // onValuesChange={(value, values) => setValues(values)}
      labelWrap={true}
      preserve={false}
      {...restProps}
    >
      {fields.map((item, itemIndex) => renderFormItem(item, itemIndex ))}
      {
        restProps.children
      }
    </Form>
  );
};

CustomForm.propTypes = {
  fields: PropTypes.array,
};

CustomForm.defaultProps = {
  fields: [],
};

CustomForm.List = Form.List;
CustomForm.Item = Form.Item;   
CustomForm.useForm = Form.useForm;   
CustomForm.useWatch = Form.useWatch;   
export default CustomForm;
