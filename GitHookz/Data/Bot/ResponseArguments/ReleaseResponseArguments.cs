using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class ReleaseResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public ReleaseResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
