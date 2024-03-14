using GitHookz.Data;
using GitHookz.Data.Models;
using Serilog;

namespace GitHookz.Services;

public class DatabaseService
{
    private List<WebHookData> webHookDatas = new List<WebHookData>();

    // T:{thread_id}:{webhook_url}
    // C:{channel_id}:{webhook_url}
    public DatabaseService()
    {
        if (!Directory.Exists(StringConstants.BASE_DATA_PATH))
            Directory.CreateDirectory(StringConstants.BASE_DATA_PATH);

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

    public void AddWebHookData(WebHookData data)
    {
        webHookDatas.Add(data);
        File.AppendAllLines(StringConstants.WEBHOOK_DATA_FILE, new string[] { data.Type + ":" + data.RecipientId + ":" + data.WebHookUrl });
    }



}
