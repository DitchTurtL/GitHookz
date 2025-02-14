using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class PullRequestReviewCommentResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public PullRequestReviewCommentResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
