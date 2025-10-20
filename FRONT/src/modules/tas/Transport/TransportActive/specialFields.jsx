import { Form, InputNumber, Input, Select } from "antd"

export default (transportMode, carrier, location, costCode) => {
  return [
    {
      label: 'Mode',
      name: 'transportModeId',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Mode is required'}],
      type: 'select',
      inputprops: {
        options: transportMode
      }
    },
    {
      label: 'Cost Code',
      name: 'costCodeId',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Cost Code is required'}],
      type: 'select',
      inputprops: {
        options: costCode
      }
    },
    {
      label: 'Transport Code',
      name: 'code',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Transport Code is required'}],
    },
    {
      label: 'Carrier',
      name: 'carrierId',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Carrier is required'}],
      type: 'select',
      inputprops: {
        options: carrier
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.toLocationId !== curValues.toLocationId}>
        {({getFieldValue}) => {
          return (
            <Form.Item 
              label='Port Of Departure'
              name='fromLocationId'
              className='col-span-6 mb-2'
              rules={[{required: true, message: 'Port Of Departure is required'}]}
            >
              <Select 
                options={location.filter((item) => item.Id !== getFieldValue('toLocationId'))}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item> 
          ) 
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId}>
        {({getFieldValue}) => {
          return (
            <Form.Item 
              label='Port Of Arrive'
              name='toLocationId'
              className='col-span-6 mb-2'
              rules={[{required: true, message: 'Port Of Arrive is required'}]}
            >
              <Select
                options={location.filter((item) => item.Id !== getFieldValue('fromLocationId'))}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item> 
          ) 
        }}
      </Form.Item>
    },
    {
      label: 'Date',
      name: 'eventDate',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Date is required'}],
      type: 'date',
      inputprops: {
        className: 'w-full',
        showWeek: true,
      }
    },
    {
      label: 'Seats',
      name: 'Seats',
      type: 'number',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Seats is required'}],
      inputprops: {
        min: 0
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId || prevValues.toLocationId !== curValues.toLocationId}>
        {({getFieldValue, setFieldValue}) => {
          let fromIsOnsite = location?.find((item) => item.Id === getFieldValue('fromLocationId'))?.onSite;
          let toIsOnsite = location?.find((item) => item.Id === getFieldValue('toLocationId'))?.onSite;
          setFieldValue('etd', null)
          setFieldValue('eta', null)
          setFieldValue('outETD', null)
          setFieldValue('outETA', null)
          setFieldValue('outSeats', null)

          if(toIsOnsite === 1 && fromIsOnsite === 0){
            return (
              <>
                <Form.Item 
                  label='In ETD'
                  name='etd'
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'In ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
                <Form.Item 
                  label='In ETA'
                  name='eta'
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'In ETA is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
                <Form.Item 
                  label='Out ETD'
                  name='outETD'
                  className='col-span-6 mb-2'
                  rules={[{ pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
                <Form.Item noStyle shouldUpdate={(prev, cur) => prev.outETD !== cur.outETD || prev.outETA !== cur.outETA}>
                  {({getFieldValue, setFieldValue}) => {
                    let isRequired = false
                    if(getFieldValue('outETD')){
                      isRequired = true
                    }
                    return(
                      <Form.Item 
                        label='Out ETA'
                        name='outETA'
                        className='col-span-6 mb-2'
                        rules={[{required: isRequired, message: 'Out ETA is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                      >
                        <Input 
                          maxLength={4}
                        />
                      </Form.Item>
                    )
                  }}
                </Form.Item>
                <Form.Item noStyle shouldUpdate={(prev, cur) => prev.outETD !== cur.outETD || prev.outETA !== cur.outETA}>
                  {({getFieldValue, setFieldValue}) => {
                    let isRequired = false
                    if(getFieldValue('outETD') && getFieldValue('outETA')){
                      isRequired = true
                    }
                    return(
                      <Form.Item 
                        label='Out Seats'
                        name='outSeats'
                        className='col-span-6 mb-2'
                        rules={[{required: isRequired, message: 'Out Seats is required'}]}
                      >
                        <InputNumber 
                          maxLength={4}
                          min={0}
                        />
                      </Form.Item>
                    )
                  }}
                </Form.Item>
              </>
            )
          }else if(toIsOnsite === 0 && fromIsOnsite === 1){
            return (
              <>
                <Form.Item 
                  label='Out ETD'
                  name='etd'
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'In ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
                <Form.Item noStyle shouldUpdate={(prev, cur) => prev.outETD !== cur.outETD || prev.outETA !== cur.outETA}>
                  {({getFieldValue, setFieldValue}) => {
                    let isRequired = false
                    if(getFieldValue('outETD')){
                      isRequired = true
                    }
                    return(
                      <Form.Item 
                        label='Out ETA'
                        name='eta'
                        className='col-span-6 mb-2'
                        rules={[{required: isRequired, message: 'Out ETA is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                      >
                        <Input 
                          maxLength={4}
                        />
                      </Form.Item>
                    )
                  }}
                </Form.Item>
              </>
            )
          }
          else{
            return (
              <>
                <Form.Item 
                  label='In ETD'
                  name='etd'
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'In ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
                <Form.Item 
                  label='In ETA'
                  name='eta'
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'In ETA is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
                {/* <Form.Item 
                  label='Out ETD'
                  name='outETD'
                  className='col-span-6 mb-2'
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
                <Form.Item 
                  label='Out ETA'
                  name='outETA'
                  className='col-span-6 mb-2'
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item> */}
              </>
            )
          }
        }}
      </Form.Item>
    },
    {
      label: 'Air Craft Code',
      name: 'AircraftCode',
      className: 'col-span-6 mb-2',
    },
  ]
}