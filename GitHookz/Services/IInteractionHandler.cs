using Discord;

namespace GitHookz.Services;

public interface IInteractionHandler
{
    Task InitializeAsync();
    Task SendMessageAsync(long channelId, string title, string authorName, string description, string content, string avatarUrl, string repoUrl);
    Task SendPushMessageAsync(long recipientId, Embed embed);
}