# OpenID with AAD PoC

A PoC for Azure Active Directory with OpenID authentication

If the solution started for the first time, the database must be created
```
dotnet ef database update
```

Before the start of the application setup the Authentication authority (AAD) and secrets to access it:
```
dotnet user-secrets set "ApplicationSettings:JwtKey" "***"
dotnet user-secrets set "ApplicationSettings:AAD:TenantId" "***"
dotnet user-secrets set "ApplicationSettings:AAD:ClientId" "***"
dotnet user-secrets set "ApplicationSettings:AAD:ClientSecret" "***"
```

Notes:

The initial files were generated with ```dotnet aspnet-codegenerator identity```

https://jwt.io/ allows checking JWT content

Azure Active Directory authentication is implemented through OpenID Connect interface with the __'AddOpenIdConnect()'__ helper middleware. Another option with __'AddAzureAD()'__ uses Microsoft AzureAD UI nuget package which is very popular when users search the web about how to integrate AzureAD to their web application. Now marked Obsolete (see https://github.com/aspnet/Announcements/issues/439).

Run a browser and type in the link 'https://localhost:5001/api/challenge'. The browser will show the form to enter credentials for Azure Active Directory OpenID Connect interface.
Then it automatically redirects to the backend.

In order to login with the local database (username and password) call 'https://localhost:5001/api/login':
```json
{
  "userName": "***",
  "password": "***"
}
```

After the user has logged in, check the data endpoint 'https://localhost:5001/api/data'. Do not forget to set the __'Authorization Bearer <token base64>'__ request header.