namespace Nanobin.API.Data;
public sealed class DatabaseInitService(PasteRepository repo) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
        => repo.InitialiseAsync();

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}