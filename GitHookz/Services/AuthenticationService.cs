using Blazored.SessionStorage;
using GitHookz.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace GitHookz.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IDatabaseService _databaseService;
    private List<SessionData> Sessions { get; set; } = [];

    private string? baseUrl;

    public AuthenticationService(ILogger<AuthenticationService> logger, IDatabaseService databaseService)
    {
        _logger = logger;
        _databaseService = databaseService;

        // Call this once to load the base URL from the environment variable
        //  and throw an error if it's not set.
        GetBaseUrl();

        // Test an authentication session
        var testRepoUrl = "https://www.github.com/DitchTurtL/GitHookz";
        var testSession = new SessionData
        {
            UserId = "testUserId",
            UserName = "DitchTurtL",
            ChannelId = "testChannelId",
            ChannelName = "#GitHookz",
            RepositoryName = StringHelper.GetRepoNameFromUrl(testRepoUrl),
            RepositoryUrl = testRepoUrl,
            SessionId = "12345"
        };
        Sessions.Add(testSession);
    }


    private string? GetBaseUrl() 
    {
        if (!string.IsNullOrEmpty(baseUrl)) return baseUrl;

        var url = Environment.GetEnvironmentVariable("BASE_URL");

        if (string.IsNullOrEmpty(url))
        {
            _logger.LogError("BASE_URL environment variable is not set.");
            return null;
        }

        baseUrl = url;
        return baseUrl;
    }

    public string CreateSession(SessionData sessionData)
    {
        // Record the id/username for the request in the database
        _databaseService.GetOrAddUser(new() { DiscordId = sessionData.UserId, Name = sessionData.UserName });

        // Add the session to the list of active sessions
        Sessions.Add(sessionData);
        return sessionData.SessionId;
    }

    public string GetSessionUrl(string sessionId)
    {
        return $"{GetBaseUrl()}add-project/{sessionId}";
    }

    public SessionData? GetAuthenticatedSession(string authId)
    {
        return Sessions.FirstOrDefault(s => s.SessionId == authId);
    }
}
