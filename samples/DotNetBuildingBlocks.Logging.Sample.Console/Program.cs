using DotNetBuildingBlocks.Logging.Context;
using DotNetBuildingBlocks.Logging.DependencyInjection;
using DotNetBuildingBlocks.Logging.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddDotNetBuildingBlocksLogging();
services.AddTransient<OrderProcessor>();

var serviceProvider = services.BuildServiceProvider();
var processor = serviceProvider.GetRequiredService<OrderProcessor>();

await processor.ProcessAsync(orderId: 1001, correlationId: Guid.NewGuid().ToString("N"));

internal sealed class OrderProcessor(ILogger<OrderProcessor> logger)
{
    public async Task ProcessAsync(int orderId, string correlationId)
    {
        using var correlationScope = logger.BeginCorrelationScope(correlationId);
        using var operationScope = logger.BeginOperationScope("ProcessOrder", correlationId);
        using var entityScope = logger.BeginEntityScope("Order", orderId, "ProcessOrder");

        logger.LogOperationStarted("ProcessOrder", orderId);

        try
        {
            using var propertyScope = logger.BeginPropertyScope(
                ("UserId", "system-user"),
                ("TenantId", "default"),
                ("Feature", "Checkout"));

            logger.LogInformation("Loading order data.");
            await Task.Delay(100);

            logger.LogRetryAttempt("ChargePayment", 1, 3);
            await Task.Delay(100);

            throw new InvalidOperationException("Payment gateway rejected the request.");
        }
        catch (Exception exception)
        {
            logger.LogDependencyFailure(exception, "PaymentGateway", orderId);
            logger.LogOperationFailed(exception, "ProcessOrder", orderId);
            logger.LogUnhandledException(exception, "ProcessOrder", orderId);
        }
        finally
        {
            logger.LogOperationCompleted("ProcessOrder", orderId);
        }
    }
}
