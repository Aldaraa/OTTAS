import dayjs from "dayjs";

const disabledDate = (current) => {
  return current.endOf('D') < dayjs().endOf('D');
};

export default (transportMode, carrier, location) => {
  return [
    {
      label: 'Carrier',
      name: 'CarrierId',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Carrier is required'}],
      type: 'select',
      inputprops: {
        options: carrier
      }
    },
    {
      label: 'Transport Code',
      name: 'Code',
      className: 'col-span-6 mb-2',
      rules: [{required: true, message: 'Transport Code is required'}],
    },
    {
      label: 'Seats (Schedule)',
      name: 'Seats',
      className: 'col-span-6 mb-2',
    },
    {
      type: 'component',
      component: <div className="col-span-6"></div>

    },
    {
      label: 'ETD (Schedule)',
      name: 'ETD',
      className: 'col-span-6 mb-2',
      rules: [{ pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}],
      inputprops: {
        maxLength: 4
      }

    },
    {
      label: 'ETA (Schedule)',
      name: 'ETA',
      className: 'col-span-6 mb-2',
      rules: [{ pattern: new RegExp(/^(?:2[0-3]|[01][0-9])[0-5][0-9]$/), message: 'Enter correct time'}],
      inputprops: {
        maxLength: 4
      }

    },
    {
      label: 'From Date',
      name: 'StartDate',
      className: 'col-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true,
        disabledDate: disabledDate,
        format: 'YYYY-MM-DD'
      }
    },
     {
      label: 'To Date',
      name: 'EndDate',
      className: 'col-span-6 mb-2',
      type: 'date',
      inputprops: {
        showWeek: true,
      }
    },
  ]
}