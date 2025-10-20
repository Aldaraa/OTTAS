import React, { useState, useEffect } from 'react';
import { Pie } from '@ant-design/plots';

const PieChart = (props) => {
  const data = [
    {
      type: 'In seatblock',
      value: 27,
    },
    {
      type: 'In overbooked',
      value: 25,
    },
  ];

  const config = {
    appendPadding: 10,
    data,
    angleField: 'value',
    colorField: 'type',
    radius: 0.9,
    legend: false,
    label: {
      type: 'inner',
      offset: '-30%',
      content: ({ percent, value }) => `${value}`,
      style: {
        fontSize: 14,
      },
    },
    interactions: [
      {
        type: 'element-active',
      },
    ],
    pieStyle: (e) => {
      if(e.type === 'In overbooked'){
        return {
          fill: '#7BCEEE'
        }
      }else{
        return {
          fill: '#FF8F4E'
        }
      }
    },
  };
  return <Pie {...config} {...props}/>;
};

export default PieChart

