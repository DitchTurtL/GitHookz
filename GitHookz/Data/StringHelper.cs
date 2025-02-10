namespace GitHookz.Data;

public static class StringHelper
{
    public static string GetRepoNameFromUrl(string repoUrl)
    {
        return repoUrl.Split('/').Last();
    }
}
