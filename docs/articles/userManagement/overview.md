# Overview
In this section, you learn the different modes of user management, permission setup, package limitations, and decide the adoption of the package. 

## 1. Three user management modes
> one user is allowed to be assigned to mutiple companies. `Company_CompanyRole_User` is the default mode.

| Mode | Company | Role | User
|--|--|--|--|
| `Company_CompanyRole_User`| &check; | &check; Role is defined at company level. | &check; |
| `Company_GlobalRole_User`| &check; | &check; Role is defined at global level. | &check; |
| `Role_User`| &#9746; | &check; | &check; |

### Change default user management mode
> API endpoint and schema has slight difference on different modes.

```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement(
                new UserManagementOptions
                {
                    //...
                    Mode = UserManagementMode.Role_User
                });
            //...
        }
    }
```

## 2. Permission setup
### Build-in Permissions
The following are serveral built-in permissions used to authorize user management apis.
| Permission | Description |
|--|--|
| `Premissions.ManageUser` | permission to make change on user. |
| `Premissions.ViewUser` | permission to read user. |
| `Premissions.ManageRole` | permission to make change on role. |
| `Premissions.ViewRole` | permission to read role. |
| `Premissions.ManageCompany` | permission to make change on company. |
| `Premissions.ViewCompany` | permission to read company. |

### Create your own premissions
Define permissions by implementing interface - `IPermissionMatrix`. The following code defined two permissions.
> You are also allowed to manage permissions outside of the source code. [Here is the instruction](/articles/userManagement/permissionStore).
```cs
    public class PermissionBook : IPermissionMatrix
    {
        public enum WeatherPermission
        {
            //...

            [PermissionValue(id: "8", key: "ReadWeather", name: "Read Weather", group: "Weather", description: "ReadWeather")]
            ReadWeather,

            [PermissionValue(id: "9", key: "WriteWeather", name: "Write Weather", group: "Weather", description: "WriteWeather")]
            WriteWeather,

            //... other permissions
        }
    }
```

### Authorize API with permissions
Decorates API actions with permission.
>  You are also allowed to use Role-based authorization like `[Authorize(Roles = "****")]`. [Here is the instruction](/articles/userManagement/authorize).
```cs
        [HttpGet]
        [PermissionAuthorize(WeatherPermission.ReadWeather)]
        public IEnumerable<WeatherForecast> Get()
        {
            //... api logic
        }
```

## 3. Data Model
The predefeind data models of `Company`, `Role` and `User` may not fully meet your needs. Please check the [built-in model definition](/articles/userManagement/dataModel), and follow this [instruction](/articles/userManagement/customModel) to extend the data model.

## 4. Company/ Role/ User deletion
By default, the soft deletion is enabled. Change the default behavior to hard delete record.
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement().UseEFCore(new EFCoreOptions
                {
                    DbContextOptionsBuilder = options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;"),
                    HardDelete = true
                });
            //...
        }
    }
```

## 5. Data Access Implementation
We provide data access implementation in package - `DNVGL.Authorization.UserManagement.EFCore`. It has dependency on EF Core 5.0+. 

If you don't want to introduce EF Core in your project. then this package is not required to be installed. Here is an instruction to [Replace EF Core with your own data access](/articles/userManagement/dataAccess)

## 6. Performance &check;
By default, The package reads and check user's permission from database for every http request. You can change this behavior to load user's permission in to claim (in cookie), and then read, check user's premission from claim. The following is to setup such behavior.
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddAuthentication().AddCookie(o => o.Events.AddCookieValidateHandler(services));
            //...
        }
    }
```

## 7. Swagger UI
 By default, Swagger UI probably do not show *User Management APIs*. Then you could follow the [instructuon](/articles/userManagement/swagger) to get it displayed.