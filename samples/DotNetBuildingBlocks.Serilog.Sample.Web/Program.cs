using DotNetBuildingBlocks.Logging.Context;
using DotNetBuildingBlocks.Logging.DependencyInjection;
using DotNetBuildingBlocks.Serilog.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDotNetBuildingBlocksLogging();

builder.Host.UseDotNetBuildingBlocksSerilog(options =>
{
    options.ApplicationName = "Samples.OrderApi";
    options.ApplicationVersion = "1.0.0";
    options.UseConsole = true;
    options.IncludeActivityEnricher = true;
    options.IncludeCorrelationEnricher = true;
    options.IncludeMachineName = true;
    options.IncludeEnvironmentName = true;
    options.ReadFromConfiguration = false;
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Samples.OrderApi", status = "ok" }));

app.MapGet("/orders/{orderId:int}", (int orderId, ILogger<Program> logger) =>
{
    var correlationId = Guid.NewGuid().ToString("N");

    using var correlationScope = logger.BeginCorrelationScope(correlationId);
    using var entityScope = logger.BeginEntityScope("Order", orderId, "GetOrder");

    logger.LogInformation("Fetching order {OrderId}.", orderId);

    return Results.Ok(new
    {
        orderId,
        correlationId,
        status = "loaded"
    });
});

app.Run();

public partial class Program;
