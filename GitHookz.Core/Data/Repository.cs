namespace GitHookz.Core.Data;

public class Repository
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int UserId { get; set; }
    public string? ChannelId { get; set; }
}
