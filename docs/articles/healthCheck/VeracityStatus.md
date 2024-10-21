# DNV.Monitoring.HealthChecks.VeracityStatus
The `DNV.Monitoring.HealthChecks.VeracityStatus` package converts the standard health check response to format that is compatiable with Veracity Status required format.

### Output format
`Status`

Veracity Status accepts status, Pass is health, Warn is degrade, Fail is unhealth.

`Duration`

Duration is health check execution duration.

`ServiceId`

Identifies the service. Use id like: identityApi, V3 or any other commonly used names for the service in question.

`ReleaseId`

in well-designed APIs, backwards-compatible changes in the service should not update a version number.APIs usually change their version number as infrequently as possible, to preserve stable interface.  However, implementation of an API may change much more frequently, which leads to the importance of having separate "release number" or "releaseId" that is different from the public version of the API.

`Version`

Public version of the service.

`Description`

Description is a human-friendly description of the service.

`HealthyHttpStatus`

The response http status when status is 'Healthy'.

`DegradedHttpStatus`

The response http status when status is 'Degraded'.

`UnhealthyHttpStatus`

The response http status when status is 'Unhealthy'.

`DependencyStatuses`

Array of dependencies probed. Like downstream services, database connections, etc.

| Property | Description |
|--|--|
| ComponentType | The probed name used when registering health check ||
| ...... | If dependency is downstream service and also responses with same format of Veracity Status, lists all response properties under current probe ||


## How to use
This health check implenments IHealthCheck interface and allow to pass in APIM subscription key and downstream service health probe URL. It could attach the downstream service response in health report and prevent circuit request.

To use ServiceHealthCheck when configuring HealthCheck:

```
app.MapVeracityHealthChecks("/health", option, enrichVeracityStatus);
```

### Parameters
`pattern`

The URL pattern of the health checks endpoint.

`options`

Class VeracityStatusHealthCheckOptions. The service information includes in response.

| Property | Description |
|--|--|
| ServiceId | (required) identifies the service. Use id like: identityApi, V3 or any other commonly used names for the service in question. ||
| ReleaseId | (optional) in well-designed APIs, backwards-compatible changes in the service should not update a version number.APIs usually change their version number as infrequently as possible, to preserve stable interface.  However, implementation of an API may change much more frequently, which leads to the importance of having separate "release number" or "releaseId" that is different from the public version of the API. ||
| Version | (optional) public version of the service. ||
| Description | (optional) is a human-friendly description of the service. ||

`enrichVeracityStatus`

After all component checks have been executed, this delegate will be invoked to update response of the varacity status format.
