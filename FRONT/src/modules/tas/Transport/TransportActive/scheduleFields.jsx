import { Form, Input, InputNumber, Select } from "antd"

const generateToLocation = (option, locations, setFunction) => {
  const siteLocation = locations.find((location) => location.Code === 'OT')
  if(option?.Code !== 'OT'){
    setFunction('toLocationId', siteLocation?.Id || null)
  }
}

export default (transportMode, carrier, location) => {
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
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.transportModeId !== curValues.transportModeId}>
        {({getFieldValue}) => {
          return (
            transportMode.find((item) => item.value === getFieldValue('transportModeId'))?.label === 'Drive' ?
            <Form.Item 
              label='Carrier'
              name='carrierId'
              className='col-span-6 mb-2'
              rules={[{required: true, message: 'Carrier is required'}]}
            >
              <Select
                options={carrier}
                allowClear
                showSearch
                filterOption={(input, option) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase())}
              />
            </Form.Item> 
            :
            <div className="col-span-6 mb-2"></div>
          ) 
        }}
      </Form.Item>
    },
    {
      label: 'Transport Code',
      name: 'code',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Transport Code is required'}],
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.transportModeId !== curValues.transportModeId}>
        {({getFieldValue}) => {
          return (
            transportMode.find((item) => item.value === getFieldValue('transportModeId'))?.label !== 'Drive' &&
            <Form.Item 
              label='Carrier'
              name='carrierId'
              className='col-span-6 mb-2'
              rules={[{required: true, message: 'Carrier is required'}]}
            >
              <Select
                options={carrier}
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
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.transportModeId !== curValues.transportModeId}>
        {({getFieldValue}) => {
          return (
            transportMode.find((item) => item.value === getFieldValue('transportModeId'))?.label === 'Drive' &&
            <Form.Item 
              label='Direction'
              name='Direction'
              className='col-span-6 mb-2'
              rules={[{required: true, message: 'Carrier is required'}]}
            >
              <Select options={[{value: 'IN', label: 'IN'}, {value: 'OUT', label: 'OUT'}]}/>
            </Form.Item> 
          ) 
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.toLocationId !== curValues.toLocationId || prevValues.transportModeId !== curValues.transportModeId || prevValues.Direction !== curValues.Direction}>
        {({getFieldValue, setFieldValue}) => {
          const currentMode = transportMode.find((item) => item.value === getFieldValue('transportModeId'))
          const currentDirection = getFieldValue('Direction')
          const drvId = location.find((item) => item.Code === 'DRV')
          if(currentMode?.label === 'Drive'){
            if(currentDirection === 'IN'){
              setFieldValue('fromLocationId', drvId.Id)
            }else if(currentDirection === 'OUT'){
              setFieldValue('fromLocationId', location.find((item) => item.Code === 'OT').Id)
            }
          }
          return (
            currentMode?.label !== 'Drive' ?
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
                onChange={(e, option) => generateToLocation(option, location, setFieldValue)}
              />
            </Form.Item> 
            :
            <Form.Item 
              label='Port Of Departure'
              name='fromLocationId'
              className='col-span-6 mb-2'
              rules={[{required: true, message: 'Port Of Departure is required'}]}
            >
              <Select 
                disabled 
                options={location.filter((item) => item.Id !== getFieldValue('toLocationId'))} 
              />
            </Form.Item> 
          ) 
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId || prevValues.transportModeId !== curValues.transportModeId || prevValues.Direction !== curValues.Direction}>  
        {({getFieldValue, setFieldValue}) => {
          const currentMode = transportMode.find((item) => item.value === getFieldValue('transportModeId'))
          const currentDirection = getFieldValue('Direction')
          const drvId = location.find((item) => item.Code === 'DRV')
          if(currentMode?.label === 'Drive'){
            if(currentDirection === 'IN'){
              const siteLocationId = location.find((item) => item.onSite === 1).value
              setFieldValue('toLocationId', siteLocationId)
            }else if(currentDirection === 'OUT'){
              setFieldValue('toLocationId', drvId.Id)
            }
          }
          return (
            currentMode?.label !== 'Drive' ?
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
            : 
            <Form.Item 
              label='Port Of Arrive'
              name='toLocationId'
              className='col-span-6 mb-2'
              rules={[{required: true, message: 'Port Of Arrive is required'}]}
            >
              <Select
                disabled
                options={location.filter((item) => item.Id !== getFieldValue('fromLocationId'))} 
                allowClear
              />
            </Form.Item> 
          ) 
        }}
      </Form.Item>
    },
    {
      label: 'Week day',
      name: 'dayNums',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Week day is required'}],
      type: 'checkDays',
    },
    {
      label: 'Frequency Weeks',
      name: 'frequencyWeeks',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Frequency Weeks is required'}],
      type: 'number',
      inputprops: {
        min: 0
      }
    },
    {
      label: 'Start Date',
      name: 'startDate',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Start Date is required'}],
      type: 'date',
      inputprops: {
        className: 'w-full',
        showWeek: true,
      }
    },
    {
      label: 'End Date',
      name: 'endDate',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'End Date is required'}],
      type: 'date',
      inputprops: {
        className: 'w-full',
        showWeek: true,
      }
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId || prevValues.toLocationId !== curValues.toLocationId || prevValues.transportModeId !== curValues.transportModeId }>
        {({getFieldValue, setFieldValue}) => {
          let fromIsOnsite = location?.find((item) => item.Id === getFieldValue('fromLocationId'))?.onSite;
          let toIsOnsite = location?.find((item) => item.Id === getFieldValue('toLocationId'))?.onSite;
          let currentMode = transportMode.find((item) => item.value === getFieldValue('transportModeId'))
          if(currentMode?.label === 'Drive'){
            return(
              getFieldValue('Direction') === 'OUT' ?
              <> 
                <Form.Item 
                  label='Out ETD'
                  name='outETD'
                  className='col-span-6 mb-2'
                  rules={[{required: true, message: 'Out ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                >
                  <Input 
                    maxLength={4}
                  />
                </Form.Item>
              </>
              :
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
              </>
            )
          }
          else {
            if(fromIsOnsite === 0 && toIsOnsite === 0){
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
                </>
              )
            }
            else if(fromIsOnsite === 1 && toIsOnsite === 0){
              return(
                <>
                  <Form.Item 
                    label='Out ETD'
                    name='outETD'
                    className='col-span-6 mb-3'
                    rules={[{required: true, message: 'Out ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                  >
                    <Input
                      maxLength={4}
                    />
                  </Form.Item>
                  <Form.Item 
                    label='Out ETA'
                    name='outETA'
                    className='col-span-6 mb-3'
                    rules={[{required: true, message: 'Out ETA is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                  >
                    <Input 
                      maxLength={4}
                    />
                  </Form.Item>
                </>
              )
            }
            else{
              return(
                <>
                  <Form.Item 
                    label='In ETD'
                    name='etd'
                    className='col-span-6 mb-3'
                    rules={[{required: true, message: 'In ETD is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                  >
                    <Input 
                      maxLength={4}
                    />
                  </Form.Item>
                  <Form.Item 
                    label='In ETA'
                    name='eta'
                    className='col-span-6 mb-3'
                    rules={[{required: true, message: 'In ETA is required'}, { pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                  >
                    <Input 
                      maxLength={4}
                    />
                  </Form.Item>
                  <Form.Item 
                    label='Out ETD'
                    name='outETD'
                    className='col-span-6 mb-3'
                    rules={[{ pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                  >
                    <Input
                      // onSelect={(e) => setFieldValue('outETD', e)}
                      maxLength={4}
                    />
                  </Form.Item>
                  <Form.Item 
                    label='Out ETA'
                    name='outETA'
                    className='col-span-6 mb-3'
                    rules={[{ pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}]}
                  >
                    <Input 
                      // onSelect={(e) => setFieldValue('outETA', e)} 
                      maxLength={4}
                    />
                  </Form.Item>
                </>
              )
            }

          }
        }}
      </Form.Item>
    },
    {
      type: 'component',
      component: <Form.Item noStyle shouldUpdate={(prevValues, curValues) => prevValues.fromLocationId !== curValues.fromLocationId || prevValues.toLocationId !== curValues.toLocationId || prevValues.transportModeId !== curValues.transportModeId}>
        {({getFieldValue}) => {
          let fromIsOnsite = location?.find((item) => item.Id === getFieldValue('fromLocationId'))?.onSite;
          let toIsOnsite = location?.find((item) => item.Id === getFieldValue('toLocationId'))?.onSite;
          let currentMode = transportMode.find((item) => item.value === getFieldValue('transportModeId'))
          return currentMode?.label !== 'Drive' && <>
            {
              !(fromIsOnsite === 0 && toIsOnsite === 0) &&
              <Form.Item 
                label='Out Seats'
                name='outSeats'
                className='col-span-6 mb-2'
                rules={[{required: (fromIsOnsite === 0 && toIsOnsite === 1) || (fromIsOnsite === 1 && toIsOnsite === 0), message: 'Out Seats is required'}]}
              >
                <InputNumber controls={false} disabled={fromIsOnsite === 0 && toIsOnsite === 0} min={0}/>
              </Form.Item> 
            }
            {
              !(fromIsOnsite === 1 && toIsOnsite === 0) &&
              <Form.Item 
                label='In Seats'
                name='inSeats'
                className='col-span-6 mb-2'
                rules={[{required: (fromIsOnsite === 0 && toIsOnsite === 1) || (fromIsOnsite === 0 && toIsOnsite === 0), message: 'In Seats is required'}]}
              >
                <InputNumber min={0} controls={false}/>
              </Form.Item>
            }
          </>
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