using GitHookz.Data.Models;

namespace GitHookz.Services;

public interface IDatabaseService
{
    void AddWebHookData(WebHookData data);
    List<WebHookData> GetWebHookDataByRepoFullName(string repoName);
}