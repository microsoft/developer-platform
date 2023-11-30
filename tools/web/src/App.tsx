// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import './App.css';
// import { AuthenticatedTemplate, MsalAuthenticationResult, useMsalAuthentication } from '@azure/msal-react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { RootView } from './components/RootView';


const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 2,
      refetchOnMount: false,
      refetchOnWindowFocus: false,
      staleTime: 1000 * 60 * 1 // 1 minute
    }
  }
});

export const App: React.FC = () => (
  <QueryClientProvider client={queryClient}>
    <BrowserRouter>
      <Routes>
        <Route path='/*' element={<RootView />} />
      </Routes>
    </BrowserRouter>
    <ReactQueryDevtools initialIsOpen={false} position='bottom-right' />
  </QueryClientProvider>
);

export default App;
