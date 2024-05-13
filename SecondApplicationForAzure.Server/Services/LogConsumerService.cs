using SecondApplicationForAzure.Model;
using SecondApplicationForAzure.Services.Services.Logs;

namespace SecondApplicationForAzure.Server.Services;

public class LogConsumerService : BackgroundService
{
    private readonly ILogger<LogConsumerService> _logger;
    private readonly IServiceProvider _provider;

    public LogConsumerService(
        ILogger<LogConsumerService> logger,
        IServiceProvider provider)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service running.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is working.");

        using (var scope = _provider.CreateScope())
        {
            var scopedProcessingService = scope.ServiceProvider.GetRequiredService<ILogService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<SecondAppDbContext>();

            await scopedProcessingService.ReadLogAsync(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}