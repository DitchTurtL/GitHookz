using GitHookz.Data;
using GitHookz.Data.Models;
using Serilog;

namespace GitHookz.Services;

public class DatabaseService : IDatabaseService
{
    private List<WebHookData> webHookDatas = new List<WebHookData>();

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        // Make sure the data directory exists
        if (!Directory.Exists(StringConstants.BASE_DATA_PATH))
            Directory.CreateDirectory(StringConstants.BASE_DATA_PATH);

        Log.Information("Loading WebHook Data");
        if (File.Exists(StringConstants.WEBHOOK_DATA_FILE))
        {
            var lines = File.ReadAllLines(StringConstants.WEBHOOK_DATA_FILE);
            foreach (var line in lines)
            {
                WebHookData hook;
                try
                {
                    hook = new WebHookData(line);
                }
                catch (Exception)
                {
                    Log.Error("Invalid webhook data: {line}", line);
                    continue;
                }
                webHookDatas.Add(hook);
            }
        }
    }

    public bool AddWebHookData(WebHookData data)
    {
        Log.Information($"Adding webhook data for {data}");

        // Check for an existing hook
        var existingHook = webHookDatas.FirstOrDefault(x => x.Type == data.Type && x.RecipientId == data.RecipientId && x.RepoFullname == data.RepoFullname);
        if (existingHook != null)
            return false;

        // Add the hook
        webHookDatas.Add(data);

        Log.Information($"Writing webhook data to file: {StringConstants.WEBHOOK_DATA_FILE}");
        File.AppendAllLines(StringConstants.WEBHOOK_DATA_FILE, new string[] { data.Type + ":" + data.RecipientId + ":" + data.RepoFullname });
        return true;
    }

    public List<WebHookData> GetWebHookDataByRepoFullName(string repoName)
    {
        return webHookDatas.Where(x => x.RepoFullname.Equals(repoName)).ToList();
    }

}
