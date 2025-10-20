import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom';
import { Pie } from '@ant-design/plots';

const PieChart = ({data, ...restProps}) => {
  // const G = G2.getEngine('canvas');
  const total = data?.MaleEmployees + data?.FemaleEmployees
  const chartData = [
    {
      sex: 'Male',
      sold: data?.MaleEmployees,
    },
    {
      sex: 'Female',
      sold: data?.FemaleEmployees,
    },
  ];
  const config = {
    appendPadding: 10,
    data: chartData,
    angleField: 'sold',
    colorField: 'sex',
    radius: 0.66,
    color: ['#1890ff', '#f04864'],
    // label: {
    //   content: (obj) => {
    //     const group = new G.Group({});
    //     group.addShape({
    //       type: 'image',
    //       attrs: {
    //         x: 10,
    //         y: 0,
    //         width: 40,
    //         height: 50,
    //         img:
    //           obj.sex === 'Male'
    //             ? 'https://gw.alipayobjects.com/zos/rmsportal/oeCxrAewtedMBYOETCln.png'
    //             : 'https://gw.alipayobjects.com/zos/rmsportal/mweUsJpBWucJRixSfWVP.png',
    //       },
    //     });
    //     group.addShape({
    //       type: 'text',
    //       attrs: {
    //         x: 35,
    //         y: 54,
    //         text: `${obj.sex} ${Math.round((obj.sold * 100)/total)}%` ,
    //         textAlign: 'center',
    //         textBaseline: 'top',
    //         fill: obj.sex === 'Male' ? '#1890ff' : '#f04864',
    //       },
    //     });
    //     return group;
    //   },
    // },
    interactions: [
      {
        type: 'element-active',
      },
    ],
    statistic:{
      title: false,
      content: {
        style: {
          whiteSpace: 'pre-wrap',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
        },
        content: 'AntV\nG2Plot',
      },
    }
  };
  return <Pie {...config} {...restProps}/>;
};

export default PieChart