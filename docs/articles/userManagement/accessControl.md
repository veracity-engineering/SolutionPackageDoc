# Resource Access Control
In this section, you learn how to control access on resource of a company. It is useful in user management modes  `Company_CompanyRole_User` and `Company_GlobalRole_User`.

## Prerequisites&check;
First of all, Specify a way to get company identity. there are three different ways to do that.
### 1. Put `companyId` in API route, or as query parameter.
> `companyId` is case senstive here.
```cs
    // From Route
    [Route("api/company/{companyId}/users")]
    public async Task<UserViewModel>  GetUsers([FromRoute] string companyId)
    {
        //... logic
    }

    // Or from query
    [Route("api/company/users")]
    public async Task<UserViewModel>  GetUsers([FromQuery] string companyId)
    {
        //... logic
    }
```

### 2. Put custom variable name of company id in API route, or as query parameter.
For example, Replace companyId with companyKey.


```cs
    // From Route
    [CompanyIdentityFieldNameFilter(companyIdInRoute: "companyKey")]
    [Route("api/company/{companyKey}/users")]
    public async Task<UserViewModel>  GetUsers([FromRoute] string companyKey)
    {
        //... logic
    }

    // Or from query
    [CompanyIdentityFieldNameFilter(companyIdInQuery: "companyKey")]
    [Route("api/company/users")]
    public async Task<UserViewModel>  GetUsers([FromQuery] string companyKey)
    {
        //... logic
    }
```

### 3. Put a global function to get company identity from `httpcontext`
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
                          GetCompanyIdentity = (httpContext) =>{
                              ///... customized logic
                          }
                      }
            //...
        }
    }
```


## Limit access to company users
If the company resouce is only accessible to users of that company. Put `AccessibleCompanyFilterAttribute` on you API endpoint.
```cs
        [AccessibleCompanyFilter]
        public async Task<IEnumerable<UserViewModel>> GetUsers([FromRoute] string companyId)
        {
            //...
        }
```

## Open access to user who have permission to view all companys' resource.
Like super admin, you can open access to those users if required. Put `AccessCrossCompanyPermissionFilterAttribute` on API and assign permsission need to check. For below case, any user who has *ViewCompany* permission can get users of every company.
```cs
        [AccessCrossCompanyPermissionFilter(Premissions.ViewCompany)]
        [AccessibleCompanyFilter]
        public async Task<IEnumerable<UserViewModel>> GetUsers([FromRoute] string companyId)
        {
            //...
        }
```