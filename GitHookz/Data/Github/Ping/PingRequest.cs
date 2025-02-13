using Newtonsoft.Json;

public class PingRequest
{
    [JsonProperty("zen")]
    public string? Zen { get; set; }

    [JsonProperty("hook_id")]
    public int HookId { get; set; }

    [JsonProperty("hook")]
    public Hook? Hook { get; set; }

    [JsonProperty("repository")]
    public Repository? Repository { get; set; }

    [JsonProperty("sender")]
    public Sender? Sender { get; set; }
}