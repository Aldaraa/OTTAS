const get = (key) => {
  return JSON.parse(localStorage.getItem(key));
};

const set = (key, value) => {
  localStorage.setItem(key, JSON.stringify(value));
};

const remove = (key) => {
  localStorage.removeItem(key);
};

const clear = () => {
  localStorage.clear();
};

const ls = {
  get,
  set,
  remove,
  clear,
};

export default ls;
