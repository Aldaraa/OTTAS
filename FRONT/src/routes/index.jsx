import { lazy, Suspense } from "react";
import { createBrowserRouter } from "react-router-dom";
import routes from './routes';
import Layout from "layouts";
import { Loading } from "components";
// import axios from "axios";

const ProtectedRoute = lazy(() => import("./ProtectedRoute"));
const Supervisor = lazy(() => import("modules/public/Supervisor"));
const Page404 = lazy(() => import("modules/public/Page404"));
const Page401 = lazy(() => import("modules/public/Page401"));

const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout></Layout>,
    // loader: async ({params}) => {
    //   return axios.get('auth/auth/loginadservice', { withCredentials: true }).then((res) => {
    //     return res.data
    //   }).catch((err) => {
    //     if(err.response.status === 499){
    //       throw new Response("Not Found Request", { status: err.response.status })
    //     }else{
    //       throw new Response("Something went wrong", { status: err.response.status })
    //     }
    //   })
    // },
    children: [...routes].map(foo => {
      return {
        ...foo,
      }
    })
  },
  {
    name: 'Supervisor',
    path: '/supervisor',
    element: <ProtectedRoute routeName='Supervisor'>
      <Suspense fallback={<Loading/>}><Supervisor/></Suspense>
    </ProtectedRoute>,
  },
  { 
    path: '/401',
    element: <Suspense fallback={<Loading/>}><Page401/></Suspense>,
  },
  {
    path: '/*',
    element: <Suspense fallback={<Loading/>}><Page404/></Suspense>,
  }
])

export default router