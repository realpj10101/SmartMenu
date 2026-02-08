namespace api.Settings;

public class DeepSeekSettings
{
    public string ApiKey { get; init; } = "";
    public string BaseUrl { get; init; } = "https://api.deepseek.com";
    public string Model { get; init; } = "deepseek-chat";
}