# Data Model
In this section, you learn the built-in data model in User Management Package. These model is defined in package - `DNVGL.Authorization.UserManagement.Abstraction`.
> You are allowed to extend data models with extra fields. [Extend Data model - Company, Role, User](/articles/userManagement/customModel)

## Company
| Field | Type | Description |
|--|--|--|
| Id | `string` | The primary key of the company |
| Name | `string` | |
| Description | `string` | |
| ServiceId | `string` | It is the veracity service id. |
| DomainUrl | `string` | It is domain url like *dnv.com*. or the secondary directory in url like *oilgas* in dnv.com/oilgas/phonebook. |
| Permissions | `string` | Permissions are combined as a string which use *semicolon(;)* as a delimiter. |
| Active | `bool` | |
| Deleted | `bool` | `true` if the company is soft deleted, otherwise `false`. |
| CreatedBy | `string` | |
| CreatedOnUtc | `DateTime` | |
| UpdatedBy | `string` | |
| UpdatedOnUtc | `DateTime` | |

## Role
| Field | Type | Description |
|--|--|--|
| Id | `string` | The primary key of the role |
| Name | `string` | |
| Description | `string` | |
| Active | `bool` | |
| Deleted | `bool` | `true` if the role is soft deleted, otherwise `false`. |
| Permissions | `string` | Permissions are combined as a string which use *semicolon(;)* as a delimiter. |
| CompanyId | `string` | The id of the company to which this role belongs. It is null if it is a global role.|
| CreatedBy | `string` | |
| CreatedOnUtc | `DateTime` | |
| UpdatedBy | `string` | |
| UpdatedOnUtc | `DateTime` | |

## User
| Field | Type | Description |
|--|--|--|
| Id | `string` | The primary key of the user |
| Email | `string` | |
| FirstName | `string` | |
| LastName | `string` | |
| VeracityId | `string` | It is an id provided by identity provider. |
| Description | `string` | |
| RoleIds | `string` | Role's ids are combined as a string which use *semicolon(;)* as a delimiter. |
| CompanyIds | `string` | Company's ids are combined as a string which use *semicolon(;)* as a delimiter. |
| Active | `bool` | |
| SuperAdmin | `bool` | `true` if the user is a super admin, otherwise `false`. |
| Deleted | `bool` | `true` if the user is soft deleted, otherwise `false`. |
| CreatedBy | `string` | |
| CreatedOnUtc | `DateTime` | |
| UpdatedBy | `string` | |
| UpdatedOnUtc | `DateTime` | |