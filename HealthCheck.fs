namespace FAspNetCore

open App.Metrics

module HealthCheck =
    open System
    open System.Runtime.InteropServices
    open System.Threading
    open System.Threading.Tasks
    open Microsoft.Extensions.Diagnostics.HealthChecks
    open Microsoft.Extensions.Logging

    let random = Random(1_129_361_415).Next

    type HealthCheck (logger : ILogger<HealthCheck>, metrics : IMetrics) =
        interface IHealthCheck with
            member self.CheckHealthAsync (context : HealthCheckContext, [<Optional; DefaultParameterValue(CancellationToken())>] cancellationToken : CancellationToken) : Task<HealthCheckResult> =
                task {
                    use _ = metrics.Measure.Timer.Time(AppMetrics.requestTime, context.Registration.Name)
    
                    cancellationToken.ThrowIfCancellationRequested()
                    
                    logger.LogInformation("Performing health check.")
                    
                    let healthCheckDelayMilliseconds = 250 + random 750

                    let! isHealthy =
                        task {
                            do! Task.Delay(healthCheckDelayMilliseconds, cancellationToken)
                            return random 2 > 0
                        }

                    return
                        if isHealthy
                        then
                            logger.LogInformation("Healthy")
                            HealthCheckResult.Healthy("A healthy result.")
                        else
                            logger.LogWarning("Unhealthy")
                            HealthCheckResult(context.Registration.FailureStatus, "An unhealthy result.")
                }
