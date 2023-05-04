# User Management Configuration

## Config Database
[Use Azure CosmosDB as database](/articles/userManagement/cosmos)

[Use Azure SQL Server as database](/articles/userManagement/sqlserver)

User other databases.
> The package can use all database engines supported by EF Core 5.0+. Here is a list -  [EF Core 5.0 Database providers](https://docs.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli)

[User Management Configuration](/articles/userManagement/configuration)

## Config unauthorized behavior
The package provides the following two built-in methodologies to handle unauthorized access. 
| Name | Description|
|--|--|
| Return403ForbiddenCode | It will send 403 reponse to the client directly. **It is default hehavior**|
| ThrowExceptionDirectly | It will throw `UnauthorizedAccessException` directly. Then developer should capture this exception and handle properly. |

Change the default heavior to throw a exception directly.
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement(
                //...
                PermissionOptions = new PermissionOptions()
                      {
                          //...
                          HandleUnauthorizedAccess = BuiltinUnauthorizedAccessHandler.ThrowExceptionDirectly
                      })
            //...
        }
    }
```

Customize unauthorized behavior
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement(
                //...
                PermissionOptions = new PermissionOptions()
                      {
                          //...
                          HandleUnauthorizedAccess = (httpContext, missedPermission) =>{
                              ///... customized logic
                          }
                      }
            //...
        }
    }
```

## Config logic to get user id in Identity Provider
By default, The pacakge reads user id from the Claim which type is `userId`. It is feasible in case Veracity Identity Provider (Azure AD B2C). Setup your own logic if required.
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement(
                //...
                PermissionOptions = new PermissionOptions()
                      {
                          //...
                          GetUserIdentity = (User) => User.Claims.FirstOrDefault(t => t.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value
                      }
            //...
        }
    }
```