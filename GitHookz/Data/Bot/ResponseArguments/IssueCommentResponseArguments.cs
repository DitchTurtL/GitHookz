using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class IssueCommentResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public IssueCommentResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
