import React, { useContext } from 'react'
import Header from './header';
import Sidebar from './sidebar';
import { Breadcrumb } from 'components';
import Page403 from 'modules/public/Page403';
import { Outlet } from 'react-router-dom';
import { AuthContext } from 'contexts';

function MainLayout() {
  const { state } = useContext(AuthContext);
  
  return (
    <>
      <Header />
      <Sidebar />
      <main>
        <Breadcrumb />
        {state.pageForbidden ? <Page403 /> : <Outlet />}
      </main>
    </>
  )
}

export default MainLayout