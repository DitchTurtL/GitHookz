namespace GitHookz.Data.Models;

public class RepoDetails
{
    public string Owner { get; set; } = string.Empty;
    public string Repo { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public static RepoDetails GetRepoDetails(string url)
    {
        var parts = url.Split('/');
        return new RepoDetails
        {
            Owner = parts[3],
            Repo = parts[4],
            Url = url
        };
    }
}