using Newtonsoft.Json;

namespace GitHookz.Data.Github.Push;

public class Pusher
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }
}

