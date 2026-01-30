using System.Text.Json.Serialization;

namespace api.DTOs;

public record OllamaChatRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")]
    OllamaMessage[] Messages,
    [property: JsonPropertyName("stream")] bool Stream
);

public record OllamaMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")]
    string Content
);

public record OllamaChatResponse(
    [property: JsonPropertyName("message")]
    OllamaMessage? Message
);