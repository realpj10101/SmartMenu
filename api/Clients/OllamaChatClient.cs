using System.Text.Json.Serialization;
using api.DTOs;
using api.Interfaces;

namespace api.Clients;

public class OllamaChatClient : IOllamaChatClient
{
    private readonly HttpClient _http;

    public OllamaChatClient(IHttpClientFactory factory)
        => _http = factory.CreateClient("OllamaClient");

    public async Task<string> ChatAsync(string model, string system, string user, CancellationToken cancellationToken)
    {
        OllamaChatRequest req = new(
            model,
            new[]
            {
                new OllamaMessage("system", system),
                new OllamaMessage("user", user)
            },
            Stream: false
        );

        HttpResponseMessage res = await _http.PostAsJsonAsync("/api/chat", req, cancellationToken);
        res.EnsureSuccessStatusCode();
        
        var body = await res.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: cancellationToken);
        return body?.Message?.Content ?? "";
    }
}