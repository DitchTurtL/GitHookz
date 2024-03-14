namespace GitHookz.Data.Models;

public class RepoDetails
{
    public string Owner { get; set; }
    public string Repo { get; set; }
    public string Url { get; set; }

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