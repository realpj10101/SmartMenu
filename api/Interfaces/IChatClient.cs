namespace api.Interfaces;

public interface IChatClient
{
    public Task<string> ChatAsync(string model, string system, string user, CancellationToken cancellationToken);
}