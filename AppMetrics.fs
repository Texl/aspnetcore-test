namespace FAspNetCore

module AppMetrics =
    open App.Metrics

    let requestTime =
        Timer.TimerOptions(
            Name="Request Time",
            MeasurementUnit=Unit.Calls,
            DurationUnit=TimeUnit.Milliseconds,
            RateUnit=TimeUnit.Milliseconds)
    
    let httpStatus =
        Meter.MeterOptions(
            Name="HTTP Status",
            MeasurementUnit=Unit.Calls)
