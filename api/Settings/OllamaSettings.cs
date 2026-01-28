namespace api.Settings;

public class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string EmbedModel { get; set; } = "nomic-embed-text";
}