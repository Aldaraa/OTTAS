import React, { useEffect } from 'react'
import { useLocation } from 'react-router-dom';

function ScrollTop({children}) {
  const { pathname } = useLocation();

  useEffect(() => {
    window.scrollTo({
      behavior: 'smooth',
      top: 0,
    });
  }, [pathname]);

  return null
}

export default ScrollTop