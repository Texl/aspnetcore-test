open System
open App.Metrics.Formatters.Json
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open FAspNetCore

#nowarn "20"

let builder = WebApplication.CreateBuilder(Environment.GetCommandLineArgs())

builder.Services.AddControllers()

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck<HealthCheck.HealthCheck>("random failure 1")
    .AddCheck<HealthCheck.HealthCheck>("random failure 2")
    .AddCheck<HealthCheck.HealthCheck>("random failure 3")

// AppMetrics
builder.Host
    .UseMetricsEndpoints(fun o ->
        o.EnvInfoEndpointOutputFormatter <- EnvInfoJsonOutputFormatter()
        o.MetricsEndpointOutputFormatter <- MetricsJsonOutputFormatter()
        o.MetricsTextEndpointEnabled <- false)

let app = builder.Build()
app.UseHttpsRedirection() 
app.UseAuthorization() 
app.MapControllers()
app.MapHealthChecks("/health")
app.Run()
