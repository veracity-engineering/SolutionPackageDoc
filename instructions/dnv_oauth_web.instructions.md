---
description: 'Comprehensive usage instructions for DNV.OAuth.Web package - OpenID Connect and JWT authentication for ASP.NET Core web applications'
applyTo: '**/OAuth/Web/**/*.cs'
---

# DNV.OAuth.Web Authentication Package

Instructions for using the DNV.OAuth.Web package, which simplifies OpenID Connect (OIDC) and JWT Bearer authentication in ASP.NET Core web applications.

## Project Context

- **Package**: DNV.OAuth.Web
- **Target Frameworks**: .NET 6.0, .NET 8.0
- **Dependencies**: Microsoft.AspNetCore.Authentication.JwtBearer, Microsoft.AspNetCore.Authentication.OpenIdConnect
- **Purpose**: Simplify authentication setup with Veracity, Azure AD B2C, or other OIDC providers

## Configuration Management

### Using appsettings.json and User Secrets

**CRITICAL**: Never store sensitive values like `ClientSecret` in appsettings.json. Use User Secrets for development and secure storage (Azure Key Vault, environment variables) for production.

#### appsettings.json (Non-Sensitive Settings)

```json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
      "ClientId": "your-client-id-guid",
      "CallbackPath": "/signin-oidc",
      "ResponseType": "code",
      "Scopes": ["openid", "profile", "offline_access"]
    },
    "Jwt": {
      "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
      "Audience": "your-api-audience"
    }
  }
}
```

#### User Secrets (Development - Sensitive Settings)

Initialize user secrets:
```powershell
dotnet user-secrets init
dotnet user-secrets set "Authentication:Oidc:ClientSecret" "your-client-secret-value"
dotnet user-secrets set "Authentication:Jwt:ClientSecret" "your-jwt-secret-value"
```

Or edit secrets.json directly:
```json
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "your-client-secret-value"
    },
    "Jwt": {
      "ClientSecret": "your-jwt-secret-value"
    }
  }
}
```

#### Production Configuration

Use Azure Key Vault, environment variables, or secure configuration providers:

```csharp
// Program.cs or Startup.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

// Or environment variables
Environment.SetEnvironmentVariable("Authentication__Oidc__ClientSecret", "value");
```

## Core Authentication Scenarios

### 1. Basic OpenID Connect Authentication

Use this for traditional web applications where users authenticate through a browser with redirect flows.

#### Minimal Setup with Configuration

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
      "ClientId": "your-client-id",
      "Scopes": ["openid", "profile"]
    }
  }
}

// User secrets (secrets.json)
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "your-secret-here"
    }
  }
}
```

```csharp
// Program.cs (.NET 6+)
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

```csharp
// Startup.cs (Traditional)
public void ConfigureServices(IServiceCollection services)
{
    services.AddOidc(o =>
    {
        Configuration.GetSection("Authentication:Oidc").Bind(o);
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseAuthentication();
    app.UseAuthorization();
}
```

#### With Authorization Code Flow (Recommended for Token Caching)

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
      "ClientId": "your-client-id",
      "CallbackPath": "/signin-oidc",
      "ResponseType": "code",
      "Scopes": ["openid", "profile", "offline_access"]
    }
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}

// User secrets
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "your-secret-here"
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add distributed cache for token storage
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddDistributedRedisCache(o =>
    {
        builder.Configuration.GetSection("Redis").Bind(o);
    });
}

builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

**Key Points**:
- `ResponseType = Code` enables MSAL integration for token caching
- Include `"offline_access"` scope to obtain refresh tokens
- Tokens are automatically cached and refreshed using MSAL
- PKCE is automatically enabled for security

#### Custom Cookie Configuration

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/...",
      "ClientId": "your-client-id",
      "ResponseType": "code",
      "Scopes": ["openid", "profile", "offline_access"]
    },
    "Cookie": {
      "Name": "MyAppAuth",
      "SlidingExpirationHours": 4
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOidc(
    oidcOptions =>
    {
        builder.Configuration.GetSection("Authentication:Oidc").Bind(oidcOptions);
    },
    cookieOptions =>
    {
        var cookieConfig = builder.Configuration.GetSection("Authentication:Cookie");
        cookieOptions.Cookie.Name = cookieConfig["Name"] ?? "MyAppAuth";
        var hours = cookieConfig.GetValue<int>("SlidingExpirationHours", 4);
        cookieOptions.ApplySlidingLifetime(TimeSpan.FromHours(hours));
    }
);
```

#### Custom Events and Token Validation

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/...",
      "ClientId": "your-client-id",
      "ResponseType": "code"
    }
  }
}

// User secrets
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "your-secret"
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
    
    o.Events = new OpenIdConnectEvents
    {
        OnAuthorizationCodeReceived = context =>
        {
            // Custom logic after receiving authorization code
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Custom logic after token validation
            return Task.CompletedTask;
        }
    };
    o.SecurityTokenValidator = new CustomTokenValidator();
});
```

### 2. JWT Bearer Authentication

Use this for APIs that accept JWT tokens in the Authorization header.

#### Single Authority

```json
// appsettings.json
{
  "Authentication": {
    "Jwt": {
      "ApiScheme": {
        "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
        "Audience": "your-api-audience"
      }
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwt("ApiScheme", o =>
    {
        builder.Configuration.GetSection("Authentication:Jwt:ApiScheme").Bind(o);
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

#### Multiple Authorities (Multi-Tenant Scenarios)

```json
// appsettings.json
{
  "Authentication": {
    "Jwt": {
      "MultiAuth": {
        "Audience": "your-api-audience",
        "AuthorizationPolicyName": "MultiAuthPolicy",
        "AddAsDefault": true,
        "Authorities": [
          {
            "SchemePostfix": "Veracity",
            "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0"
          },
          {
            "SchemePostfix": "AzureAD",
            "Authority": "https://login.microsoftonline.com/{tenant-id}/v2.0"
          }
        ]
      }
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwt("MultiAuth", o =>
    {
        builder.Configuration.GetSection("Authentication:Jwt:MultiAuth").Bind(o);
    });

var app = builder.Build();

// Load JWT configurations at startup
await app.LoadJwtConfiguration(new[] { "MultiAuth.Veracity", "MultiAuth.AzureAD" });

app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

**Key Features**:
- Automatic issuer-based scheme selection from JWT token
- Creates separate authentication schemes for each authority
- Consolidates them under a single authorization policy

#### Configuration-Based Setup with Multiple Schemes

```json
// appsettings.json
{
  "Authentication": {
    "Jwt": {
      "Api1": {
        "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
        "Audience": "api1-audience"
      },
      "Api2": {
        "Authority": "https://login.microsoftonline.com/{tenant-id}/v2.0",
        "Audience": "api2-audience"
      }
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwt(builder.Configuration.GetSection("Authentication:Jwt").GetChildren());

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

#### Custom Claims Validation

```json
// appsettings.json
{
  "Authentication": {
    "Jwt": {
      "ApiScheme": {
        "Authority": "https://login.veracity.com/...",
        "Audience": "your-api-audience"
      }
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwt("ApiScheme", o =>
    {
        builder.Configuration.GetSection("Authentication:Jwt:ApiScheme").Bind(o);
        
        o.CustomClaimsValidator = claims =>
        {
            var hasRequiredClaim = claims.Any(c => c.Type == "custom_claim");
            return hasRequiredClaim 
                ? (true, string.Empty) 
                : (false, "Missing required custom claim");
        };
    });
```

### 3. Cookie Session Management

#### Default Session Behavior

By default, cookie authentication includes:
- **Sliding expiration**: 8 hours (configurable)
- **Automatic renewal**: Cookies are refreshed on each request
- **Secure defaults**: HttpOnly, Secure, SameSite=Lax

#### Distributed Cache Ticket Store

Stores authentication tickets in distributed cache instead of cookies, reducing cookie size.

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/...",
      "ClientId": "your-client-id",
      "ResponseType": "code"
    }
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}

// User secrets
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "your-secret"
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddDistributedRedisCache(o =>
    {
        builder.Configuration.GetSection("Redis").Bind(o);
    });
}

builder.Services.AddDistributedCacheTicketStore();
builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

**Benefits**:
- Smaller cookie size (only session ID stored)
- Better performance for large authentication tickets
- Centralized session management

#### Global Session Management

Tracks all user sessions and enables centralized sign-out across multiple browser sessions.

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/...",
      "ClientId": "your-client-id",
      "ResponseType": "code"
    }
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddDistributedRedisCache(o =>
    {
        builder.Configuration.GetSection("Redis").Bind(o);
    });
}

builder.Services.AddGlobalSessionTicketStore();
builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
})
.PostConfigure<CookieAuthenticationOptions>(
    CookieAuthenticationDefaults.AuthenticationScheme,
    o => o.ApplyGlobalSessionLifetime()
);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

**Features**:
- Tracks all active sessions per user
- Sign-out invalidates all user sessions
- Prevents session reuse after sign-out

#### Custom Cache Expiration

```json
// appsettings.json
{
  "Cache": {
    "SlidingExpirationHours": 12,
    "AbsoluteExpirationDays": 7
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTicketStoreDistributedCacheOptions(o =>
{
    var cacheConfig = builder.Configuration.GetSection("Cache");
    o.SlidingExpiration = TimeSpan.FromHours(
        cacheConfig.GetValue<int>("SlidingExpirationHours", 12)
    );
    o.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(
        cacheConfig.GetValue<int>("AbsoluteExpirationDays", 7)
    );
});

builder.Services.AddDistributedCacheTicketStore();
```

#### Custom Sliding Lifetime

```json
// appsettings.json
{
  "Authentication": {
    "Cookie": {
      "SlidingExpirationMinutes": 30
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOidc(
    oidcOptions =>
    {
        builder.Configuration.GetSection("Authentication:Oidc").Bind(oidcOptions);
    },
    cookieOptions =>
    {
        var minutes = builder.Configuration.GetValue<int>(
            "Authentication:Cookie:SlidingExpirationMinutes", 30
        );
        cookieOptions.ApplySlidingLifetime(
            TimeSpan.FromMinutes(minutes),
            ignoreSliding: context => context.Request.Path.StartsWithSegments("/api")
        );
    }
);
```

#### Disable Default Lifetime Management

```csharp
services.AddOidc(
    oidcOptions => { /* ... */ },
    cookieOptions =>
    {
        cookieOptions.DisableDefaultLifetime();
        // Manage cookie lifetime manually
        cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(30);
        cookieOptions.SlidingExpiration = false;
    }
);
```

### 4. Sign-Out

#### Standard Sign-Out

```csharp
[HttpPost]
public async Task<IActionResult> SignOut()
{
    await HttpContext.SignOut(
        new[] { 
            CookieAuthenticationDefaults.AuthenticationScheme, 
            OpenIdConnectDefaults.AuthenticationScheme 
        },
        returnUrl: "/");
    return Redirect("/");
}
```

**Behavior**:
- Clears local authentication cookie
- Redirects to identity provider for sign-out
- Returns to specified URL after sign-out

#### Multi-Scheme Sign-Out

```csharp
await HttpContext.SignOut(
    new[] { 
        CookieAuthenticationDefaults.AuthenticationScheme,
        "CustomScheme1",
        "CustomScheme2",
        OpenIdConnectDefaults.AuthenticationScheme 
    });
```

**Note**: OIDC scheme should be last to ensure proper redirect behavior.

## Advanced Configuration Patterns

### Combining OIDC and JWT

For applications that need both user authentication and API endpoints:

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/...",
      "ClientId": "web-client-id",
      "ResponseType": "code",
      "Scopes": ["openid", "profile", "offline_access"]
    },
    "Jwt": {
      "ApiScheme": {
        "Authority": "https://login.veracity.com/...",
        "Audience": "api-audience"
      }
    }
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "MyApp:"
  }
}

// User secrets
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "web-secret"
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    builder.Services.AddDistributedRedisCache(o =>
    {
        builder.Configuration.GetSection("Redis").Bind(o);
    });
}

// OIDC for web UI
builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
});

// JWT for API endpoints
builder.Services.AddAuthentication()
    .AddJwt("ApiScheme", o =>
    {
        builder.Configuration.GetSection("Authentication:Jwt:ApiScheme").Bind(o);
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

```csharp
// Use on controllers/actions
[Authorize] // Uses default OIDC
public class WebController : Controller { }

[Authorize(AuthenticationSchemes = "ApiScheme")]
[ApiController]
public class ApiController : ControllerBase { }
```

### Using Authentication Builder

For more control over authentication setup:

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/...",
      "ClientId": "your-client-id"
    },
    "Jwt": {
      "ApiScheme": {
        "Authority": "https://login.veracity.com/...",
        "Audience": "api-audience"
      }
    }
  }
}
```

```csharp
var webBuilder = WebApplication.CreateBuilder(args);

var authBuilder = webBuilder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
});

var oidcOptions = new OidcOptions();
webBuilder.Configuration.GetSection("Authentication:Oidc").Bind(oidcOptions);

authBuilder.AddOidc(oidcOptions);
authBuilder.AddJwt("ApiScheme", o =>
{
    webBuilder.Configuration.GetSection("Authentication:Jwt:ApiScheme").Bind(o);
});
```

### Custom Token Validator

```csharp
public class CustomTokenValidator : DNVTokenValidator
{
    public CustomTokenValidator() : base() { }
    
    public CustomTokenValidator(Func<IEnumerable<Claim>, (bool, string)> customValidator) 
        : base(customValidator) { }
    
    // Override validation logic
}

services.AddOidc(o =>
{
    o.SecurityTokenValidator = new CustomTokenValidator();
});
```

### Form Post Authentication Method

By default, OIDC uses FormPost. To use redirect:

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/...",
      "ClientId": "your-client-id",
      "AuthenticationMethod": "RedirectGet"
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
    // Override if needed - AuthenticationMethod from config won't bind automatically
    o.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
});
```

**Note**: FormPost includes automatic CSP (Content Security Policy) script hash configuration.

## Configuration Options Reference

### OidcOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Authority` | `string` | Veracity default | Identity provider authority URL |
| `ClientId` | `string` | Required | Application client ID |
| `ClientSecret` | `string` | Required | Application client secret |
| `Scopes` | `string[]` | `[ClientId]` | OAuth scopes to request |
| `CallbackPath` | `string` | `/signin-oidc` | Redirect path after authentication |
| `ResponseType` | `string` | `"code"` | OAuth response type (use `"code"` for token caching) |
| `AuthenticationMethod` | `OpenIdConnectRedirectBehavior` | `FormPost` | Authentication redirect method |
| `Events` | `OpenIdConnectEvents` | `null` | Custom OIDC event handlers |
| `SecurityTokenValidator` | `ISecurityTokenValidator` | `DNVTokenValidator` | Custom token validator |

### JwtOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Authority` | `string` | Veracity default | Identity provider authority URL |
| `Audience` | `string` | `null` | Expected audience in JWT tokens |
| `ClientId` | `string` (deprecated) | `null` | Use `Audience` instead |
| `Authorities` | `List<AuthorityItem>` | `[]` | Multiple authorities for multi-tenant |
| `TokenValidationParameters` | `TokenValidationParameters` | `null` | Custom token validation settings |
| `Events` | `JwtBearerEvents` | `null` | Custom JWT event handlers |
| `CustomClaimsValidator` | `Func<IEnumerable<Claim>, (bool, string)>` | `null` | Additional claims validation |
| `SecurityTokenValidator` | `ISecurityTokenValidator` | `DNVTokenValidator` | Custom token validator |
| `AuthorizationPolicyName` | `string` | `"JwtDefaultPolicy"` | Policy name for multi-authority |
| `AddAsDefault` | `bool` | `true` | Set as default policy for multi-authority |

## Best Practices

### Security

- **Always use HTTPS** in production environments
- **Use authorization code flow** (`ResponseType = Code`) for web applications
- **Include `offline_access`** scope to enable token refresh
- **PKCE is enabled by default** - do not disable unless necessary
- **Validate custom claims** when required by business logic
- **Use distributed cache** (Redis) for production token/session storage

### Cookie Management

- **Default 8-hour expiration** aligns with Veracity security requirements
- **Use sliding expiration** to improve user experience
- **Consider distributed cache ticket store** for large authentication tickets
- **Use global session management** if you need centralized sign-out

### Token Caching

- **Use `IDistributedCache`** for production (Redis recommended)
- **Configure cache expiration** to match token lifetimes
- **Monitor cache performance** and size

### Multi-Tenant Applications

- **Use multiple authorities** in JWT configuration
- **Load JWT configuration** at startup with `LoadJwtConfiguration`
- **Issuer-based routing** is automatic - no custom logic needed

### Configuration

- **NEVER commit secrets** to source control (use .gitignore for appsettings.Development.json with secrets)
- **Use User Secrets** for local development (`dotnet user-secrets set`)
- **Use Azure Key Vault** for production secrets with managed identity
- **Use environment variables** as alternative for containerized deployments
- **Store non-sensitive settings** in appsettings.json (Authority, ClientId, Audience, Scopes)
- **Use configuration binding** with `.Bind()` for cleaner code
- **Separate OIDC and JWT** configurations under `Authentication` section
- **Use hierarchical configuration** (Authentication:Oidc:ClientId)

## Common Patterns

### Veracity Authentication

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0",
      "ClientId": "your-veracity-client-id",
      "ResponseType": "code",
      "Scopes": ["openid", "profile", "offline_access"]
    }
  }
}

// User secrets (dotnet user-secrets set "Authentication:Oidc:ClientSecret" "your-secret")
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "your-veracity-client-secret"
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

### Azure AD B2C Authentication

```json
// appsettings.json
{
  "Authentication": {
    "Oidc": {
      "Authority": "https://login.microsoftonline.com/{tenant-id}/v2.0",
      "ClientId": "your-azure-client-id",
      "ResponseType": "code",
      "Scopes": ["openid", "profile", "offline_access"]
    }
  }
}

// User secrets
{
  "Authentication": {
    "Oidc": {
      "ClientSecret": "your-azure-client-secret"
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOidc(o =>
{
    builder.Configuration.GetSection("Authentication:Oidc").Bind(o);
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

### API with Multiple Token Sources

```json
// appsettings.json
{
  "Authentication": {
    "Jwt": {
      "ApiAuth": {
        "Audience": "my-api-audience",
        "AuthorizationPolicyName": "MultiSourcePolicy",
        "AddAsDefault": true,
        "Authorities": [
          {
            "SchemePostfix": "Veracity",
            "Authority": "https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0"
          },
          {
            "SchemePostfix": "AzureAD",
            "Authority": "https://login.microsoftonline.com/{tenant-id}/v2.0"
          }
        ]
      }
    }
  }
}
```

```csharp
var builder = WebApplication.CreateBuilder(args);

// Accepts tokens from both Veracity and Azure AD
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwt("ApiAuth", o =>
    {
        builder.Configuration.GetSection("Authentication:Jwt:ApiAuth").Bind(o);
    });

var app = builder.Build();

await app.LoadJwtConfiguration(new[] { "ApiAuth.Veracity", "ApiAuth.AzureAD" });

app.UseAuthentication();
app.UseAuthorization();
app.Run();
```

## Troubleshooting

### Cookie Too Large

**Symptom**: HTTP 400 Bad Request or cookie exceeds size limit

**Solution**: Use distributed cache ticket store
```csharp
services.AddDistributedCacheTicketStore();
```

### Token Not Refreshing

**Symptom**: Users forced to re-authenticate after token expiration

**Solution**: Ensure authorization code flow and offline_access scope
```csharp
o.ResponseType = OpenIdConnectResponseType.Code;
o.Scopes = new[] { "openid", "profile", "offline_access" };
```

### Multiple Sessions Not Cleared on Sign-Out

**Symptom**: User can still access app in another browser after sign-out

**Solution**: Use global session management
```csharp
services.AddGlobalSessionTicketStore();
```

### JWT Token Not Validated

**Symptom**: 401 Unauthorized for valid tokens

**Solutions**:
1. Check audience configuration matches token
2. Verify authority URL is correct
3. For .NET 8, ensure `UseSecurityTokenValidators = true`
4. Check token issuer matches configured authority

### CSP Violation with Form Post

**Solution**: The package automatically includes the required script hash (`sha256-N4ps+XP2YXr4JI2/sWVoER7gSQH2UxrXbN3v6MvHM4I=`) in your CSP header when using FormPost.

## Package Dependencies

When using DNV.OAuth.Web, the following packages are automatically referenced:
- `DNV.OAuth.Core` - Core OAuth functionality and MSAL integration
- `DNV.OAuth.Abstractions` - Shared interfaces and options
- `Microsoft.AspNetCore.Authentication.OpenIdConnect`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Microsoft.Identity.Client` (via Core package)

## Related Packages

- **DNV.OAuth.Core**: Core OAuth functionality, MSAL integration, token caching
- **DNV.OAuth.Api.HttpClient**: OAuth for HttpClient scenarios (API-to-API calls)
- **DNV.OAuth.Abstractions**: Shared interfaces and base options

## Version Information

- Supports .NET 6.0 and .NET 8.0
- Uses latest Microsoft.Identity.Client (MSAL)
- Compatible with Veracity and Azure AD B2C

## Migration Notes

### From DNVGL.OAuth.Web to DNV.OAuth.Web

Namespace change:
```csharp
// Old
using DNVGL.OAuth.Web;

// New
using DNV.OAuth.Web;
```

### Deprecated Properties

- `JwtOptions.ClientId` → Use `JwtOptions.Audience` instead

## Additional Resources

- [Microsoft Authentication Library (MSAL) Documentation](https://learn.microsoft.com/azure/active-directory/develop/msal-overview)
- [OpenID Connect Protocol](https://openid.net/connect/)
- [ASP.NET Core Authentication](https://learn.microsoft.com/aspnet/core/security/authentication/)

---

## API Reference

### Extension Methods

#### OAuthExtensions (OpenID Connect)

**Namespace**: `DNV.OAuth.Web`

##### AddOidc

Adds OpenID Connect authentication to the application.

```csharp
// Method 1: Add to IServiceCollection with Action<OidcOptions>
public static AuthenticationBuilder AddOidc(
    this IServiceCollection services,
    Action<OidcOptions> oidcSetupAction,
    Action<CookieAuthenticationOptions>? cookieSetupAction = null,
    Action<DistributedCacheEntryOptions>? cacheSetupAction = null
)
```

**Parameters**:
- `oidcSetupAction` (required): Action to configure OIDC options
- `cookieSetupAction` (optional): Action to configure cookie options
- `cacheSetupAction` (optional): Action to configure distributed cache options for token storage

**Returns**: `AuthenticationBuilder` for further configuration

**Throws**: `ArgumentNullException` if `oidcSetupAction` is null

**Example**:
```csharp
services.AddOidc(o =>
{
    o.Authority = "https://login.veracity.com/...";
    o.ClientId = "your-client-id";
    o.ClientSecret = "your-secret";
});
```

---

```csharp
// Method 2: Add to IServiceCollection with OidcOptions instance
public static AuthenticationBuilder AddOidc(
    this IServiceCollection services,
    OidcOptions oidcOptions,
    Action<CookieAuthenticationOptions>? cookieSetupAction = null,
    Action<DistributedCacheEntryOptions>? cacheSetupAction = null
)
```

**Parameters**:
- `oidcOptions` (required): Pre-configured OIDC options instance
- `cookieSetupAction` (optional): Action to configure cookie options
- `cacheSetupAction` (optional): Action to configure distributed cache options

**Returns**: `AuthenticationBuilder` for further configuration

**Throws**: `ArgumentNullException` if `oidcOptions` is null

---

```csharp
// Method 3: Add to AuthenticationBuilder with Action<OidcOptions>
public static AuthenticationBuilder AddOidc(
    this AuthenticationBuilder builder,
    Action<OidcOptions> oidcSetupAction,
    Action<CookieAuthenticationOptions>? cookieSetupAction = null,
    Action<DistributedCacheEntryOptions>? cacheSetupAction = null
)
```

**Parameters**:
- `oidcSetupAction` (required): Action to configure OIDC options
- `cookieSetupAction` (optional): Action to configure cookie options
- `cacheSetupAction` (optional): Action to configure distributed cache options

**Returns**: `AuthenticationBuilder` for further configuration

---

```csharp
// Method 4: Add to AuthenticationBuilder with OidcOptions instance
public static AuthenticationBuilder AddOidc(
    this AuthenticationBuilder builder,
    OidcOptions oidcOptions,
    Action<CookieAuthenticationOptions>? cookieSetupAction = null,
    Action<DistributedCacheEntryOptions>? cacheSetupAction = null
)
```

**Parameters**:
- `oidcOptions` (required): Pre-configured OIDC options instance
- `cookieSetupAction` (optional): Action to configure cookie options
- `cacheSetupAction` (optional): Action to configure distributed cache options

**Returns**: `AuthenticationBuilder` for further configuration

**Features**:
- Automatically configures Cookie and OpenID Connect authentication schemes
- Enables PKCE (Proof Key for Code Exchange) by default
- Supports authorization code flow with MSAL token caching
- Applies DNVTokenValidator by default
- Handles FormPost CSP configuration automatically

---

#### JwtExtensions (JWT Bearer Authentication)

**Namespace**: `DNV.OAuth.Web`

##### AddJwt

Adds JWT Bearer authentication to the application.

```csharp
// Method 1: Single scheme with JwtOptions instance
public static AuthenticationBuilder AddJwt(
    this AuthenticationBuilder builder,
    string authenticationSchema,
    JwtOptions jwtOptions
)
```

**Parameters**:
- `authenticationSchema` (required): Name of the authentication scheme
- `jwtOptions` (required): JWT configuration options

**Returns**: `AuthenticationBuilder` for further configuration

---

```csharp
// Method 2: Single scheme with Action<JwtOptions>
public static AuthenticationBuilder AddJwt(
    this AuthenticationBuilder builder,
    string authenticationSchema,
    Action<JwtOptions> setupAction
)
```

**Parameters**:
- `authenticationSchema` (required): Name of the authentication scheme
- `setupAction` (required): Action to configure JWT options

**Returns**: `AuthenticationBuilder` for further configuration

**Throws**: `ArgumentNullException` if `setupAction` is null

**Example**:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwt("ApiScheme", o =>
    {
        o.Authority = "https://login.veracity.com/...";
        o.Audience = "your-api-audience";
    });
```

---

```csharp
// Method 3: Multiple schemes with Action<IDictionary<string, JwtOptions>>
public static AuthenticationBuilder AddJwt(
    this AuthenticationBuilder builder,
    Action<IDictionary<string, JwtOptions>> setupAction
)
```

**Parameters**:
- `setupAction` (required): Action to configure multiple JWT schemes

**Returns**: `AuthenticationBuilder` for further configuration

**Throws**: `ArgumentNullException` if `setupAction` is null

---

```csharp
// Method 4: Multiple schemes from configuration
public static AuthenticationBuilder AddJwt(
    this AuthenticationBuilder builder,
    IEnumerable<IConfigurationSection> sections
)
```

**Parameters**:
- `sections` (required): Configuration sections containing JWT options

**Returns**: `AuthenticationBuilder` for further configuration

**Throws**: `ArgumentNullException` if `sections` is null

**Example**:
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwt(Configuration.GetSection("Authentication:Jwt").GetChildren());
```

---

```csharp
// Method 5: Multiple schemes with dictionary
public static AuthenticationBuilder AddJwt(
    this AuthenticationBuilder builder,
    IDictionary<string, JwtOptions> schemaOptions
)
```

**Parameters**:
- `schemaOptions` (required): Dictionary of scheme names to JWT options

**Returns**: `AuthenticationBuilder` for further configuration

**Throws**: `ArgumentNullException` if `schemaOptions` is null or empty

**Features**:
- Supports multiple authorities for multi-tenant scenarios
- Automatic issuer-based scheme selection
- Applies DNVTokenValidator by default
- Configures authorization policies for multi-authority setups
- Supports custom claims validation
- 8-hour refresh interval for configuration

---

##### LoadJwtConfiguration

Loads JWT configuration and builds issuer-to-scheme mapping for automatic scheme selection.

```csharp
public static async Task LoadJwtConfiguration(
    this WebApplication app,
    IEnumerable<string> schemes
)
```

**Parameters**:
- `schemes` (required): Collection of authentication scheme names to load

**Returns**: `Task` representing the asynchronous operation

**Usage**: Call this method after building the application and before running it when using multiple authorities.

**Example**:
```csharp
var app = builder.Build();
await app.LoadJwtConfiguration(new[] { "MultiAuth.Veracity", "MultiAuth.AzureAD" });
app.Run();
```

---

#### CookieExtensions (Cookie Session Management)

**Namespace**: `DNV.OAuth.Web`

##### AddDistributedCacheTicketStore

Adds distributed cache-based ticket store for authentication tickets.

```csharp
public static IServiceCollection AddDistributedCacheTicketStore(
    this IServiceCollection services
)
```

**Returns**: `IServiceCollection` for further configuration

**Features**:
- Stores authentication tickets in distributed cache
- Reduces cookie size (only session ID in cookie)
- Default sliding expiration: 8 hours
- Requires `IDistributedCache` implementation

**Example**:
```csharp
services.AddDistributedMemoryCache();
services.AddDistributedCacheTicketStore();
```

---

##### AddGlobalSessionTicketStore

Adds global session management with ticket store that tracks all user sessions.

```csharp
public static IServiceCollection AddGlobalSessionTicketStore(
    this IServiceCollection services
)
```

**Returns**: `IServiceCollection` for further configuration

**Features**:
- Tracks all active sessions per user
- Enables centralized sign-out across all sessions
- Uses distributed cache for session tracking
- Default sliding expiration: 8 hours

**Example**:
```csharp
services.AddDistributedMemoryCache();
services.AddGlobalSessionTicketStore();
```

---

##### AddTicketStoreDistributedCacheOptions

Configures distributed cache options for ticket store.

```csharp
public static OptionsBuilder<DistributedCacheEntryOptions> AddTicketStoreDistributedCacheOptions(
    this IServiceCollection services,
    Action<DistributedCacheEntryOptions> setupAction
)
```

**Parameters**:
- `setupAction` (required): Action to configure cache options

**Returns**: `OptionsBuilder<DistributedCacheEntryOptions>` for further configuration

**Example**:
```csharp
services.AddTicketStoreDistributedCacheOptions(o =>
{
    o.SlidingExpiration = TimeSpan.FromHours(12);
    o.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
});
```

---

##### ApplySlidingLifetime

Applies sliding expiration to cookie authentication.

```csharp
public static CookieAuthenticationOptions ApplySlidingLifetime(
    this CookieAuthenticationOptions options,
    TimeSpan timeSpan,
    Func<HttpContext, bool>? ignoreSliding = null
)
```

**Parameters**:
- `options` (required): Cookie authentication options to configure
- `timeSpan` (required): Sliding expiration duration
- `ignoreSliding` (optional): Function to determine if sliding should be ignored for specific requests

**Returns**: `CookieAuthenticationOptions` for method chaining

**Throws**: `ArgumentNullException` if `options` is null

**Example**:
```csharp
services.AddOidc(
    oidcOptions => { /* ... */ },
    cookieOptions =>
    {
        cookieOptions.ApplySlidingLifetime(
            TimeSpan.FromMinutes(30),
            ignoreSliding: context => context.Request.Path.StartsWithSegments("/api")
        );
    }
);
```

---

##### ApplyGlobalSessionLifetime

Applies global session management to cookie authentication.

```csharp
public static CookieAuthenticationOptions ApplyGlobalSessionLifetime(
    this CookieAuthenticationOptions options
)
```

**Parameters**:
- `options` (required): Cookie authentication options to configure

**Returns**: `CookieAuthenticationOptions` for method chaining

**Throws**: `ArgumentNullException` if `options` is null

**Features**:
- Validates session cookies across all browser sessions
- Integrates with GlobalSessionTicketStore if configured
- Cleans all sessions on sign-out

**Example**:
```csharp
services.AddOidc(oidcOptions)
    .PostConfigure<CookieAuthenticationOptions>(
        CookieAuthenticationDefaults.AuthenticationScheme,
        o => o.ApplyGlobalSessionLifetime()
    );
```

---

##### DisableDefaultLifetime

Disables the default 8-hour sliding lifetime management.

```csharp
public static CookieAuthenticationOptions DisableDefaultLifetime(
    this CookieAuthenticationOptions options
)
```

**Parameters**:
- `options` (required): Cookie authentication options to configure

**Returns**: `CookieAuthenticationOptions` for method chaining

**Usage**: Use when you want to manually manage cookie lifetime without default behavior.

**Example**:
```csharp
services.AddOidc(
    oidcOptions => { /* ... */ },
    cookieOptions =>
    {
        cookieOptions.DisableDefaultLifetime();
        cookieOptions.ExpireTimeSpan = TimeSpan.FromDays(30);
        cookieOptions.SlidingExpiration = false;
    }
);
```

---

#### HttpContextExtensions (Sign-Out)

**Namespace**: `DNV.OAuth.Web`

##### SignOut

Signs out from multiple authentication schemes in proper order.

```csharp
public static async Task SignOut(
    this HttpContext context,
    IEnumerable<string> schemes,
    string? returnUrl = null
)
```

**Parameters**:
- `context` (required): Current HTTP context
- `schemes` (required): Collection of authentication scheme names to sign out from
- `returnUrl` (optional): URL to return to after sign-out (only used with OIDC)

**Returns**: `Task` representing the asynchronous operation

**Throws**: `ArgumentException` if `schemes` is empty

**Features**:
- Signs out from Cookie scheme first
- Signs out from custom schemes in order
- Signs out from OIDC scheme last (with redirect)
- Properly handles return URL for identity provider

**Example**:
```csharp
[HttpPost]
public async Task<IActionResult> SignOut()
{
    await HttpContext.SignOut(
        new[] 
        { 
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme 
        },
        returnUrl: "/"
    );
    return Redirect("/");
}
```

---

### Options Classes

#### OidcOptions

**Namespace**: `DNV.OAuth.Web`

**Inherits**: `OAuth2Options` (from DNV.OAuth.Abstractions)

Configuration options for OpenID Connect authentication.

**Properties**:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Authority` | `string` | Veracity default | Identity provider authority URL |
| `ClientId` | `string` | Required | Application client ID |
| `ClientSecret` | `string` | Required | Application client secret |
| `Scopes` | `string[]` | `[ClientId]` | OAuth scopes to request |
| `CallbackPath` | `string` | `/signin-oidc` | Redirect path after authentication |
| `ResponseType` | `string` | `"code"` | OAuth response type |
| `AuthenticationMethod` | `OpenIdConnectRedirectBehavior` | `FormPost` | Authentication redirect method |
| `Events` | `OpenIdConnectEvents` | `null` | Custom OIDC event handlers |
| `SecurityTokenValidator` | `ISecurityTokenValidator` | `DNVTokenValidator` | Custom token validator |

---

#### JwtOptions

**Namespace**: `DNV.OAuth.Web`

Configuration options for JWT Bearer authentication.

**Properties**:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Authority` | `string` | Veracity default | Identity provider authority URL |
| `Audience` | `string` | `null` | Expected audience in JWT tokens |
| `ClientId` | `string` (deprecated) | `null` | Use `Audience` instead |
| `Authorities` | `List<AuthorityItem>` | `[]` | Multiple authorities for multi-tenant |
| `TokenValidationParameters` | `TokenValidationParameters` | `null` | Custom token validation settings |
| `Events` | `JwtBearerEvents` | `null` | Custom JWT event handlers |
| `CustomClaimsValidator` | `Func<IEnumerable<Claim>, (bool, string)>` | `null` | Additional claims validation function |
| `SecurityTokenValidator` | `ISecurityTokenValidator` | `DNVTokenValidator` | Custom token validator |
| `AuthorizationPolicyName` | `string` | `"JwtDefaultPolicy"` | Policy name for multi-authority |
| `AddAsDefault` | `bool` | `true` | Set as default policy for multi-authority |
| `AuthSchemeMap` | `IDictionary<string, string>` | Static | Issuer-to-scheme mapping (static) |

**Nested Class**: `JwtOptions.AuthorityItem`

| Property | Type | Description |
|----------|------|-------------|
| `SchemePostfix` | `string` | Postfix for authentication scheme name |
| `Authority` | `string` | Authority URL for this tenant |

---

### Constants

#### CookieKeys

**Namespace**: `DNV.OAuth.Abstractions.Constants`

**Properties**:
- `Auth`: Default cookie name for authentication
- `Session`: Cookie name for session tracking
- `TickitKeyPrefix`: Prefix for ticket cache keys
- `TickitSessionKeyPrefix`: Prefix for session cache keys

---

#### OidcMessageExtensions

**Namespace**: `DNV.OAuth.Web.Oidc`

**Constants**:
- `FormPostScriptHashCode`: CSP hash for FormPost script (`"sha256-N4ps+XP2YXr4JI2/sWVoER7gSQH2UxrXbN3v6MvHM4I="`)
- `FormPostScript`: JavaScript for automatic form submission

---

### Default Values

#### Cookie Authentication
- **Default Cookie Name**: `CookieKeys.Auth`
- **Default Sliding Expiration**: 8 hours
- **Default Scheme**: `CookieAuthenticationDefaults.AuthenticationScheme`

#### OpenID Connect
- **Default Authority**: `"https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0"`
- **Default Response Type**: `"code"`
- **Default Callback Path**: `"/signin-oidc"`
- **Default Authentication Method**: `OpenIdConnectRedirectBehavior.FormPost`
- **PKCE**: Always enabled

#### JWT Bearer
- **Default Authority**: `"https://login.veracity.com/tfp/a68572e3-63ce-4bc1-acdc-b64943502e9d/b2c_1a_signinwithadfsidp/v2.0"`
- **Configuration Refresh Interval**: 8 hours
- **Default Policy Name**: `"JwtDefaultPolicy"`

#### Token Validation
- **Default Validator**: `DNVTokenValidator` (from DNV.OAuth.Core)
- **.NET 8.0+**: `UseSecurityTokenValidator = true` / `UseSecurityTokenValidators = true`

---

### Behavior Notes

#### Authorization Code Flow
- Automatically enabled when `ResponseType` contains `"code"`
- Requires `IDistributedCache` implementation
- Uses MSAL (Microsoft.Identity.Client) for token management
- Automatically handles token refresh with refresh tokens
- Requires `"offline_access"` scope for refresh tokens

#### Multi-Authority JWT
- Creates separate authentication schemes per authority
- Scheme naming: `{BaseSchemeName}.{SchemePostfix}`
- Automatic issuer-based routing
- Creates unified authorization policy
- Requires calling `LoadJwtConfiguration()` after app build

#### Session Management
- **DistributedCacheTicketStore**: Stores tickets in cache, session ID in cookie
- **GlobalSessionTicketStore**: Tracks all sessions, enables cross-session sign-out
- **Session Path Tracking**: Supports multiple app paths with session isolation

#### Token Validator
- **DNVTokenValidator**: Custom validator from DNV.OAuth.Core
- Validates standard claims (issuer, audience, expiration)
- Supports custom claims validation via delegate
- Automatically applied to both OIDC and JWT authentication
