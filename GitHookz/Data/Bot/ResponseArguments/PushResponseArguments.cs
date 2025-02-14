using GitHookz.Data.Github;
using Octokit;

namespace GitHookz.Data.Bot.ResponseArguments;

public class PushResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }
    public SenderDetails SenderDetails { get; set; }
    public string BranchName { get; set; }
    public string CommitMessage { get; set; }
    public int AddedCount { get; set; }
    public int RemovedCount { get; set; }
    public int ModifiedCount { get; set; }

    public PushResponseArguments(RepoDetails repositoryDetails, SenderDetails senderDetails, string branchName, string commitMessage)
    {
        RepositoryDetails = repositoryDetails;
        SenderDetails = senderDetails;
        BranchName = branchName;
        CommitMessage = commitMessage;
    }
}
