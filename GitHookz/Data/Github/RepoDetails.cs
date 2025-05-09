namespace GitHookz.Data.Github;

public class RepoDetails
{
    public string RepositoryName { get; set; }
    public string RepositoryUrl { get; set; }

    public RepoDetails(string name, string url)
    {
        RepositoryName = name;
        RepositoryUrl = url;
    }
}
