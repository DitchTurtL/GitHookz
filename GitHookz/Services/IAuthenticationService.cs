using GitHookz.Data;

namespace GitHookz.Services;

public interface IAuthenticationService
{
    string CreateSession(SessionData sessionData);
    string GetSessionUrl(string sessionId);
    SessionData? GetAuthenticatedSession(string authId);

}