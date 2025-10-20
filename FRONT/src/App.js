import { StyleProvider } from '@ant-design/cssinjs';
import { ConfigProvider } from 'antd';
import { AuthContextProvider } from 'contexts';
import { Suspense } from 'react';
import { RouterProvider } from 'react-router-dom';
import router from 'routes';
import AxiosComponent from 'utils/axios';
import dayjs from 'dayjs'
import 'dayjs/locale/en'
import updateLocale from 'dayjs/plugin/updateLocale';
var isoWeek = require('dayjs/plugin/isoWeek')
var advancedFormat = require('dayjs/plugin/advancedFormat')
dayjs.extend(isoWeek)
dayjs.extend(updateLocale)
dayjs.extend(advancedFormat)
dayjs.locale('en')
dayjs.updateLocale('en', {
  weekStart: 1,
});

function App() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <StyleProvider hashPriority='high'>
        <ConfigProvider 
          theme={{
            token: {
              colorPrimary: '#e57200', 
              fontFamily: `'Roboto', sans-serif`,
            }, 
            hashed: false,
            components: {
              Menu: {
                collapsedWidth: 50,
              },
              Statistic: {
                contentFontSize: 20
              }
            }
          }} 
          prefixCls='power'
        >
          <AuthContextProvider>
            <AxiosComponent>
              <RouterProvider router={router}>
              </RouterProvider>
            </AxiosComponent>
          </AuthContextProvider>
        </ConfigProvider>
      </StyleProvider>
    </Suspense>
  );
}

export default App;
