namespace GitHookz.Core.Services;

public interface IBotService
{
    Task StartAsync();
    Task StopAsync();
}