using GitHookz.Data.Models;

namespace GitHookz.Services;

public interface IDatabaseService
{
    bool AddWebHookData(WebHookData data);
    List<WebHookData> GetWebHookDataByRepoFullName(string repoName);
}