using Newtonsoft.Json;

public class Config
{
    [JsonProperty("content_type")]
    public string? ContentType { get; set; }

    [JsonProperty("insecure_ssl")]
    public string? InsecureSsl { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }
}

