import axios from "axios"

const fetchAPIs = [
  axios.get('tas/GroupMaster/profiledata').then((res) => {
    return res.data
  }), 
  axios.get('tas/department?Active=1').then((res) => {
    return res.data.map((item) => ({...item, selectable: false}))
  }),
  axios.get('tas/costcode?Active=1').then((res) => {
    return res.data.map((item) => ({
      ...item,
      value: item.Id, 
      label: `${item.Number} ${item.Description} `,
    }))
  }),
  axios.get('tas/employer?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: item.Description, 
      ...item
    }))
  }),
  axios.get('tas/peopletype?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Code}`, 
      ...item
    }))
  }),
  axios.get('tas/position?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Description}`, 
      ...item
    }))
  }),
  axios.get('tas/roster?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Name}`, 
      ...item
    }))
  }),
  axios.get('tas/roomtype?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Description}`, 
      ...item
    }))
  }),
  axios.get('tas/location?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Description}`, 
      ...item
    }))
  }),
  axios.get('tas/nationality?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Description}`, 
      ...item
    }))
  }),
  axios.get('tas/state?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Description}`, 
      ...item
    }))
  }),
  axios.get('tas/shift?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Code} - ${item.Description}`, 
      ...item
    }))
  }),
  axios.get('tas/room?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Number} - ${item.RoomTypeName}`, 
      ...item
    }))
  }),
  axios.get('tas/camp?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Description}`, 
      ...item
    })) 
  }),
  axios.get('tas/flightgroupmaster?Active=1&fullCluster=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: `${item.Description}`, 
      ...item
    }))
  }),
  axios.get('tas/room/getvirtualroomid').then((res) => {
    return res.data
  }),
  axios.get('tas/transportmode?Active=1').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: item.Code, 
      ...item
    }))
  }),
  axios.get('tas/requestdocument/master').then((res) => {
    return {
      documentTypes: res.data.RequestDocumentType?.map((item) => ({
        value: item,
        label: item,
      })),
      paymentConditions: res.data.PaymentCondition?.map((item) => ({
        value: item,
        label: item,
      })),
      searchCurrentSteps: res.data.DocumentSearchCurrentStep?.map((item) => ({
        value: item,
        label: item,
      })),
      favorTimes: res.data.RequestDocumentFavorTime?.map((item) => ({
        value: item,
        label: item,
      })),
    }
  }),
  axios.get('tas/department/minimum').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: item.Name, 
      ...item
    }))
  }),
  axios.get('tas/requestgroup').then((res) => {
    return res.data.map((item) => ({
      value: item.Id, 
      label: item.Description, 
      ...item
    }))
  }),
  axios.get('tas/profilefield').then((res) => {
    let tmp = {}
    res.data.map((item) => {
      tmp[item.ColumnName] = item
    })
    return tmp
  }),
]

export default fetchAPIs