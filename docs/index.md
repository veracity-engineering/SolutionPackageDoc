[!include[Home](articles/Home.md)]

# Packages

Here you will find a collection of packages and tools for developers to support rapid, robust and extensible development of applications and services within DNV.

Packages are available from **nuget.org** you can find a full list of available packages [here](https://www.nuget.org/profiles/Veracity-Engineering).

Supported features may be found under categories:

- [Common](#common)
- [Ambient Context](#ambient-context)
- [Domain Driven Design](#domain-driven-design-ddd)
- [Security](#security)
- [Veracity](#veracity)

---
## Common
| Package | Description |
|---------|-------------|
| [DNVGL.Common.Core](/docs/common/core.md) | *Missing* |

---
## Ambient Context
| Package | Description |
|---------|-------------|
| DNV.Context.Abstractions | *Missing* |
| DNV.Context.HttpClient | *Missing* |
| DNV.Context.AspNet | *Missing* |
| DNV.Context.ServiceBus | *Missing* |

----
## Domain Driven Design (DDD)
| Package | Description |
|---------|-------------|
| DNVGL.Domain.Seedwork | *Missing* |
| DNV.Application.Abstractions | *Missing* |
| DNV.Application.CQRS.Abstractions | *Missing* |
| DNVGL.Domain.EventHub.MediatR | *Missing* |

---
## Security
### Web Security
| Package | Description |
|---------|-------------|
| [DNVGL.Web.Security](/docs/security/web.md) | *Missing* |

### Authentication (OAuth)
| Package | Description |
|---------|-------------|
| DNV.OAuth.Abstractions | *Missing* |
| [DNV.OAuth.Core](/docs/security/oauth/core.md) | *Missing* |
| [DNVGL.OAuth.Web](/docs/security/oauth/web.md) | A library for developers to simplify the work of setting up OpenId Connection authentication (OIDC) such as Veracity or Azure AD B2C for ASP.NET Core web project. |
| DNVGL.OAuth.Web.Swagger | *Missing* |
| [DNVGL.OAuth.Api.HttpClient](/docs/security/oauth/api-httpclient.md) | A library which provides a factory for producing authenticated HttpClients for API integration via OAuth. |
| DNV.OAuth.Web.Extensions | *Missing* |

---
## Veracity
### API
| Package | Description |
|---------|-------------|
| [DNVGL.Veracity.Services.Api.My](/docs/veracity/services/api-my.md) | Provides a client to resources available under the '**My**' view point of API v3. This view point is appropriate if you intend to use Veracity as an identity provider for your application. Only **User credentials** authentication is supported by this package. |
| [DNVGL.Veracity.Services.Api.This](/docs/veracity/services/api-this.md) | Provides a client to resources available under the '**This**' view point of API v3.  This view point is appropriate for service owners integrating with Veracity enabling management of their service and sub-service subscriptions. Only **Client credentials** authentication is supported by this package.  |
| [DNVGL.Veracity.Services.Api.Directory](/docs/veracity/services/api-directory.md) | Provides a client to resources available under the '**Directory**' view point of API v3.  This view point is appropriate for core Veracity applications where resources are not restricted to any context. Only **Client credentials** authentication is supported by this package. |