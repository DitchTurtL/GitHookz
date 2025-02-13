namespace GitHookz.Data;

public class ProjectData
{
    public int Id { get; set; }
    public string? RepositoryName { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? OwnerId { get; set; }
    public string? ChannelId { get; set; }
    public string? ChannelName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsPublished { get; set; }



}
