# Use Azure CosmosDB as database
 
## Prerequisites
Azure CosmosDB instance is created. The following instruction assumes there is a collection named `User` exists in database named `UserManagement`.

PM> `Install-Package Microsoft.EntityFrameworkCore.Cosmos`

## Register user management module in ASP.NET core project
```cs
    public class Startup
    {
        //...
        public void ConfigureServices(IServiceCollection services)
        {
            //...
            services.AddUserManagement().UseEFCore(
                new EFCoreOptions
                {
                    DbContextOptionsBuilder = options => options.UseCosmos("***Connection string***", "UserManagement"),
                    ModelBuilder = (modelBuilder) => modelBuilder.HasDefaultContainer("User"),
                });
            //...
        }
    }
```

## Create a super admin in database. 
The following is a user record template. Replace `"***"` with the real value. `Id` is the primary key and unique. 

> Create a container with the partition key set to ["__partitionKey"](https://docs.microsoft.com/en-us/ef/core/providers/cosmos/?tabs=dotnet-core-cli#partition-keys) in Azure Cosmos DB. 

> **_NOTE:_**  `Discriminator` and `id` are two field managed by EF Core. `id` is a string in the format of `[Discriminator]|[Id]`. For user record, `Discriminator` is a constant -  "User". 
```json
{
    "Id": "u1",
    "Active": true,
    "CreatedBy": "***",
    "CreatedOnUtc": "***",
    "Deleted": false,
    "Description": "***",
    "Discriminator": "User",
    "Email": "***",
    "FirstName": "***",
    "LastName": "***",
    "SuperAdmin": true,
    "UpdatedBy": "***",
    "UpdatedOnUtc": "***",
    "VeracityId": "***",
    "id": "User|u1"
}
```