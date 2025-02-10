
namespace GitHookz.Services;

public class HostedBotService : IHostedService
{
    private readonly ILogger<HostedBotService> _logger;
    private readonly IBotService _botService;

    public HostedBotService(ILogger<HostedBotService> logger, IBotService botService)
    {
        _logger = logger;
        _botService = botService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Bot Service is starting.");
        await _botService.StartBotAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Bot Service is stopping.");
        
        return Task.CompletedTask;
    }
}
