import dayjs from "dayjs";

function getDaysArrayBetweenMonths(startDate, endDate) {
  const daysArray = [];
  let currentDate = dayjs(startDate);

  while (currentDate.isBefore(endDate) || currentDate.isSame(endDate, 'day')) {
    daysArray.push(currentDate.format('YYYY-MM-DD'));
    currentDate = currentDate.add(1, 'day');
  }

  return daysArray;
}

export default getDaysArrayBetweenMonths