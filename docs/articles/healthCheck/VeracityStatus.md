# DNV.Monitoring.HealthChecks.VeracityStatus
The `DNV.Monitoring.HealthChecks.VeracityStatus` package converts the health check response to format that is compatible with Veracity Status required format.

### Output sample

![Sample](Sample.png)

## How to use
After setup HealthCheck, use MapVeracityHealthChecks extension method to format response to Veracity Status needed.

```
app.MapVeracityHealthChecks("/health", option, enrichVeracityStatus);
```

### Parameters
`pattern`

The URL of the HealthCheck endpoint.

`options`

Class VeracityStatusHealthCheckOptions. The service information includes in response.

| Property | Description |
|--|--|
| ServiceId | (required) Identifier of the service. The service id can be fetched from https://developer.veracity.com. ||
| ServiceName | (required) Name of the service. ||
| ReleaseId | (optional) Internal release Id of the service. ||
| Version | (optional) Public version of the service. ||
| Description | (optional) Human-friendly description of the service. ||
| Message | (optional) Default error message in human-readable form. ||
| Tags | (optional) Tags to describe the service. ||
| SubCode | (optional) Defined and set by the team implementing the service. Child status code should not influence parent's status code. ||
| HealthyHttpStatus | (required) HealthCheck end point response http status when status is 'Healthy'. Default is 200. ||
| DegradedHttpStatus | (required) HealthCheck end point response http status when status is 'Degraded'. Default is 424. ||
| UnhealthyHttpStatus | (required) HealthCheck end point response http status when status is 'Unhealthy'. Default is 503. ||

`enrichVeracityStatus`

After all component checks have been executed and the compoents response collected, this delegate will be invoked to update response of the veracity status format. E.g. to generate dynamic error message and subCode.