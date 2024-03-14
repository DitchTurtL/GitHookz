namespace GitHookz.Data.Models;

public class WebHookData
{
    public string Type { get; set; } = string.Empty;
    public string RecipientId { get; set; } = string.Empty;
    public string RepoFullname { get; set; } = string.Empty;

    /// <summary>
    ///     T:{thread_id}:{webhook_url}
    ///     C:{channel_id}:{webhook_url}
    /// </summary>
    public WebHookData(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentNullException(nameof(data));
        if (!data.Contains(":"))
            throw new ArgumentException("Invalid data format", nameof(data));
        if (data.Split(':').Length != 3)
            throw new ArgumentException("Invalid data format", nameof(data));

        var split = data.Split(':');
        Type = split[0];
        RecipientId = split[1];
        RepoFullname = split[2];
    }

    public WebHookData(string type, string recipientId, string repoFullname)
    {
        Type = type;
        RecipientId = recipientId;
        RepoFullname = repoFullname;
    }

    public override string ToString()
    {
        return $"{Type}:{RecipientId}:{RepoFullname}";
    }
}
