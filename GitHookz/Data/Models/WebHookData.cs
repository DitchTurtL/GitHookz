namespace GitHookz.Data.Models;

public class WebHookData
{
    public string Type { get; set; }
    public string RecipientId { get; set; }
    public string RepoFullname { get; set; }

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
}
