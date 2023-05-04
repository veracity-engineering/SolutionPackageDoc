# Veracity - My Services API v3 Clients
Packages from the `DNVGL.Veracity.Services.Api` namespace provide lightweight .NET clients for Veracity My Services API v3 built on top of packages from the Solution Package.

These packages allow developers to query and manipulate data from My services including user profiles, service profiles, notification messages, company profiles, admin roles and subscriptions.

# View Points

As a client to API v3, the package is divided in to the following view points:
| Name | Description | Supported authentication |
|--|--|--|
| [Directory](/articles/DNVGL.Veracity.Services.Api/DNVGL.Veracity.Services.Api.Directory.md) | Allows fetching and updating resources without a focus on a specifc user or service resource.  Recommended for core platform applications. | Client credentials |
| [My](/articles/DNVGL.Veracity.Services.Api/DNVGL.Veracity.Services.Api.My.md) | Allows fetching information and making requests for a user authenticated by user credential flow. | User credentials |
| [This](/articles/DNVGL.Veracity.Services.Api/DNVGL.Veracity.Services.Api.This.md) | Allows manipulation and retrieval of information related to a service where an application is authenticated as a service owner. | Client credentials |
