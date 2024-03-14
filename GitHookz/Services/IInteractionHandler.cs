namespace GitHookz.Services;

public interface IInteractionHandler
{
    Task InitializeAsync();
    Task SendMessageAsync(string channelId);
}