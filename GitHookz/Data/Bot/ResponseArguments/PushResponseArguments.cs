using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class PushResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public PushResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
