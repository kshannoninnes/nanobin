namespace Nanobin.API.Services;
public sealed class DatabaseInitService(SQLiteService repo) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
        => repo.InitialiseAsync();

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}