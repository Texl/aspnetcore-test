namespace FAspNetCore.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open App.Metrics
open FAspNetCore

[<AutoOpen>]
module private ForWeatherForecastController =
    let random = Random()

[<ApiController>]
[<Route("[controller]")>]
type WeatherForecastController (logger : ILogger<WeatherForecastController>, metrics : IMetrics) =
    inherit ControllerBase()

    let summaries =
        [|
            "Freezing"
            "Bracing"
            "Chilly"
            "Cool"
            "Mild"
            "Warm"
            "Balmy"
            "Hot"
            "Sweltering"
            "Scorching"
        |]

    let httpCodes =
        [|
            "200"
            "400"
            "401"
            "404"
        |]

    [<HttpGet>]
    [<Route("{days:int:min(1)}")>]
    member self.Get (days : int) : IActionResult =
        logger.LogInformation("weather")
        use _ = metrics.Measure.Timer.Time(AppMetrics.requestTime, "health")
        
        let httpCode = httpCodes.[random.Next httpCodes.Length]
        metrics.Measure.Meter.Mark(AppMetrics.httpStatus, httpCode)
        
        match httpCode with
        | "200" ->
            Array.init days (fun index ->
                { Date = DateTime.Now.AddDays(float index)
                  TemperatureC = random.Next(-20,55)
                  Summary = summaries.[random.Next(summaries.Length)] })
            |> fun r -> self.Ok(r)
        | "400" -> self.BadRequest()
        | "401" -> self.Unauthorized()
        | "404" -> self.NotFound()
        | _ -> failwith $"Unhandled code: {httpCode}"
         