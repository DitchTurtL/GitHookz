using Discord.Interactions;
using Discord;

namespace GitHookz.Modules;

public class WebHookModule : InteractionModuleBase<SocketInteractionContext>
{

    [SlashCommand("add_webhook", "Add a new webhook")]
    public async Task AddWebhook()
    {
        /*var user = await _userService.GetUser(Context.User);
        var challengeLink = await _challengeService.CreateChallengeLink(user);

        await RespondAsync($"New Challenge created. Use this link to update it: {challengeLink}");*/
    }
    /*
    [SlashCommand("challenges", "List all active challenges")]
    public async Task Challenges()
    {
        var challenges = await _challengeService.GetActiveChallenges();

        var embed = new EmbedBuilder()
            .WithTitle("Active Challenges")
            .WithDescription("Here are all the active challenges")
            .WithColor(Color.Blue);

        foreach (var challenge in challenges)
        {
            var url = StringConstants.BASE_CHALLENGE_URL + challenge.Stub;
            var description = $"[**{challenge.Title}**]({url})\n{challenge.Description}";
            embed.AddField("\u200B", description);
        }

        var channel = Context.Channel as ITextChannel;
        if (channel != null)
            await channel.SendMessageAsync(embed: embed.Build());
    }
    */

    public async Task Echo(string echo, [Summary(description: "mention the user")] bool mention = false)
    => await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));







}
