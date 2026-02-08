namespace api.DTOs;

public record DeepSeekChatRequest(
    string Model,
    DeepSeekMessage[] Messages,
    double Temperature,
    int MaxTokens
);

public record DeepSeekMessage(
    string Role,
    string Content
);

public class DeepSeekChatResponse
{
    public List<Choice> Choices { get; set; } = [];
}

public class Choice
{
    public DeepSeekMessage? Message { get; set; }
}

