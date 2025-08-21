# DNV.Monitoring.HealthChecks.ServiceHealthCheck
The `DNV.Monitoring.HealthChecks.ServiceHealthCheck` package includes a health check that is used to check the downstream services of application.

## What it does
This health check implements IHealthCheck interface and allow to pass in APIM subscription key and downstream service health probe URL. 
It could attach the downstream service response in health report and prevent circuit request.

## How to use

```
builder.Services.AddHealthChecks()
    .AddServiceHealthCheck(new ServiceHealthCheckOptions { Uri = "https://status.veracity.com", SubscriptionKey = "~~~" }, "Veracity Status");
```

### Parameters
`options`

ServiceHealthCheckOptions
| Property | Description |
|--|--|
| Uri | Downstream service health probe URL ||
| HttpMethod | Http method to invoke down stream service, Get/Post ... ||
| SubscriptionKey | APIM subscription key ||
| Critical  | The HealthStatus that should be reported for current service when the component check fails. If critical is true, the HealthStatus.Unhealthy will be reported, otherwise HealthStatus.Degraded will be reported. ||
| ComponentType | Type of dependency service.  ||
| Healthy | Boolean function to check service response and judge if this is 'Healthy'. Default is checking by Http Status. If Healthy and Degraded defines with same Http Status, serious status wins, which is Degraded ||
| Degraded | Boolean function to check service response and judge if this is 'Degraded'. Default is checking by Http Status. If Degraded and Unhealthy defines with same Http Status, serious status wins, which is Unhealthy ||
| Unhealthy | Boolean function to check service response and judge if this is 'Unhealthy'. Default is checking by Http Status. ||

`name`

The health check name. This should be unique among all dependencies.

`tags`

(Optional) A list of tags that can be used to filter sets of health checks.

`timeout`

(Optional) TimeSpan representing the timeout of the check.

`configureHttpClient`

(Optional) setup action to configure the http client to call dependent service.

`configureHttpMessageHandler`

(Optional) setup action to configure the http client message handler.