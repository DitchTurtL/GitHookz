using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class StarResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public StarResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
