namespace GitHookz.Data.Models;

public class WebHookData
{
    public long Id { get; set; }
    public RecipientType Type { get; set; }
    public long GuildId { get; set; }
    public long RecipientId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RepoFullname { get; set; } = string.Empty;
    public string AddedBy { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.Now;
    public long AddedById { get; set; }

}
