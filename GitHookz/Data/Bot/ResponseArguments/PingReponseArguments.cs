using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class PingReponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public PingReponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
