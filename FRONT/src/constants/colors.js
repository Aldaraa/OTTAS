const Directions = {
  "IN": {
    tagColor: 'blue',
    color: '#1a66ff',
    rotate: 'rotate(135deg)',
  },
  "OUT": {
    tagColor: 'default',
    color: '#555',
    rotate: 'rotate(45deg)',
  },
  "EXTERNAL": {
    tagColor: 'purple',
    color: '#8b5cf6',
    rotate: 'rotate(90deg)',
  },
}

const seatStatus = {
  'Confirmed': {
    tagColor: 'success',
  },
  'OverBooked': {
    tagColor: 'orange',
  },
}

const COLORS = {
  Directions,
  seatStatus,
};

export default COLORS;