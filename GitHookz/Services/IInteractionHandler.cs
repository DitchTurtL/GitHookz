namespace GitHookz.Services;

public interface IInteractionHandler
{
    Task InitializeAsync();
    Task SendMessageAsync(string channelId, string title, string authorName, string description, string content, string avatarUrl, string repoUrl);
}