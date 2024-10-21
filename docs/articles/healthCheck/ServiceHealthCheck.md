# DNV.Monitoring.HealthChecks.ServiceHealthCheck
The `DNV.Monitoring.HealthChecks.ServiceHealthCheck` package includes a health check that is used to check the downstream services of application.

## What it does
This health check implenments IHealthCheck interface and allow to pass in APIM subscription key and downstream service health probe URL. 
It could attach the downstream service response in health report and prevent circuit request.

## How to use

```
builder.Services.AddHealthChecks()
    .AddServiceHealthCheck(new ServiceHealthCheckOptions { Uri = "https://status.veracity.com/", SubscriptionKey = "~~~" }, "Veracity Status");
```

### Parameters
`options`

VeracityStatusHealthCheckOptions.
| Property | Description |
|--|--|
| Uri | Downstream service health probe URL ||
| HttpMethod | Call http method ||
| SubscriptionKey | APIM subscription key ||
| Healthy | Boolean function to check service response and judge if this is 'Healthy'. Default is checking by Http Status. If Healthy and Degraded defines with same Http Status, serious status wins, which is Degraded ||
| Degraded | Boolean function to check service response and judge if this is 'Degraded'. Default is checking by Http Status. If Degraded and Unhealthy defines with same Http Status, serious status wins, which is Unhealthy ||
| Unhealthy | Boolean function to check service response and judge if this is 'Unhealthy'. Default is checking by Http Status. ||

`name`

The health check name. Optional. If <c>null</c> the type name 'ServiceHealthCheck' will be used for the name.

`failureStatus`

The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c>
then the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.

`tags`

A list of tags that can be used to filter sets of health checks. Optional.

`timeout`

An optional <see cref="TimeSpan"/> representing the timeout of the check.

`configureHttpClient`

An optional setup action to configure the http client to call dependent service.

`configureHttpMessageHandler`

An optional setup action to configure the http client message handler.

