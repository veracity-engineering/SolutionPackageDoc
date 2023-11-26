# Customize Data Access Implementation

In this section, you learn how to replace built-in EFCore implementation with your own data access implementation.

## Prerequisites
PM> `Install-Package DNVGL.Authorization.UserManagement.Abstraction`

PM> `Install-Package DNVGL.Authorization.Web`

## Implement interfaces
The following four interfaces needs to be implemented.
```cs
 public class YouOwnCompanyRepository: ICompany<Company>
{
    //...
}

 public class YouOwnRoleRepository: IRole<Role>
 {
     //...
 }

 public class YouOwnUserRepository: IUser<User>
 {
     //...
 }

 public class YouOwnUserPermissionReader : IUserPermissionReader
 {
     //...
 }
```

## Register interface implementation
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement();

            services.AddScoped<IUserPermissionReader, YouOwnUserPermissionReader>()
                    .AddScoped<IRole<Role>, YouOwnRoleRepository>()
                    .AddScoped<IUser<User>, YouOwnUserRepository>()
                    .AddScoped<ICompany<Company>, YouOwnCompanyRepository>();
            //...
        }
    }
```