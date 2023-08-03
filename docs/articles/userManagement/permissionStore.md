# Configure Permission definition out of source code.
In this section, you learn how to configure permissions out of source code. Permission data is allowed to be stored in different forms like Json file, xml file or in database.

## Prerequisites
PM> `Install-Package DNVGL.Authorization.Web`

## Implement interface `IPermissionRepository` 
Interface `IPermissionRepository` is required to be implemented to load meta data of all permssions.
```cs
    public class OwnPermissionRepository : IPermissionRepository
    {
        public async Task<IEnumerable<PermissionEntity>> GetAll()
        {
            //... logic
        }
    }
```

### `PermissionEntity` Definition
| Field | Type | Description |
|--|--|--|
| Id | `string` | The primary key of a permission |
| Key | `string` | A readable unique key of a permission |
| Name | `string` | Permission name |
| Description | `string` | Permission description |
| Group | `string` | Permission group |

## Create mandatory permissions
| Key |
|--|
|ManageUser|
|ViewUser|
|ViewRole|
|ManageRole|
|ViewCompany|
|ManageCompany|

## Register `IPermissionRepository` service
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement().UseEFCore(new EFCoreOptions
                {
                    DbContextOptionsBuilder = options => options.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=UserManagement;Trusted_Connection=Yes;")
                }).UsePermissionRepository<OwnPermissionRepository>();
            //...
        }
    }
```