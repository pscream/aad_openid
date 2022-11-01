import React from "react";
import { QueryClient, QueryClientProvider } from "react-query";
import MicrosoftLogin from "react-microsoft-login";

import Chat from "./Chat";
import "./App.css";

const queryClient = new QueryClient();

function App() {
  const authHandler = (err, data) => {
    if (err) {
      return err;
    }
    const {
      expiresOn,
      idToken: { rawIdToken, preferredName },
    } = data;
    localStorage.setItem("accessToken", rawIdToken);
    localStorage.setItem("expiresOn", expiresOn);
    localStorage.setItem("preferredName", preferredName);
    window.location.reload(false);
  };

  const accessToken = localStorage.getItem("accessToken");
  const expiresOn = localStorage.getItem("expiresOn");
  const preferredName = localStorage.getItem("preferredName");
  const isExpired = new Date(expiresOn).getTime() < Date.now();

  return (
    <QueryClientProvider client={queryClient}>
      {preferredName && accessToken && !isExpired ? (
        <Chat />
      ) : (
        <div>
          <MicrosoftLogin
            clientId={process.env.REACT_APP_MS_CLIENT_ID}
            buttonTheme="dark"
            prompt="select_account"
            tenantUrl={`https://login.microsoftonline.com/${process.env.REACT_APP_MS_TENANT_ID}/`}
            redirectUri={process.env.REACT_APP_MS_REDIRECT_URI} // default - http://localhost:3000/
            authCallback={authHandler}
          />
        </div>
      )}
    </QueryClientProvider>
  );
}

export default App;
