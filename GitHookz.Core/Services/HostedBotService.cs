using Microsoft.Extensions.Hosting;

namespace GitHookz.Core.Services;

public class HostedBotService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {

        return Task.CompletedTask;
    }
}
