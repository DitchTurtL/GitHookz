namespace GitHookz.Data.Github;

public class SenderDetails
{
    public string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ProfileUrl { get; set; }

    public SenderDetails(string username)
    {
        Username = username;
    }

    public SenderDetails(string username, string? avatarUrl, string? profileUrl)
    {
        Username = username;
        AvatarUrl = avatarUrl;
        ProfileUrl = profileUrl;
    }
}
