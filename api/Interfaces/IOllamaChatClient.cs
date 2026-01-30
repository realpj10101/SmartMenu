namespace api.Interfaces;

public interface IOllamaChatClient
{
    public Task<string> ChatAsync(string model, string system, string user, CancellationToken cancellationToken);
}