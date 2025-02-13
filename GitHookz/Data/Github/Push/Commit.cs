using Newtonsoft.Json;

namespace GitHookz.Data.Github.Push;

public class Commit
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("tree_id")]
    public string? TreeId { get; set; }

    [JsonProperty("distinct")]
    public bool Distinct { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("author")]
    public Author? Author { get; set; }

    [JsonProperty("committer")]
    public Committer? Committer { get; set; }

    [JsonProperty("added")]
    public List<object>? Added { get; set; }

    [JsonProperty("removed")]
    public List<object>? Removed { get; set; }

    [JsonProperty("modified")]
    public List<string>? Modified { get; set; }
}

