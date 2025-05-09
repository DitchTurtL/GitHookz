using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class ForkResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public ForkResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
