namespace Nanobin.Components.Services;

public class PasteCleanupService(IServiceProvider serviceProvider, ILogger<PasteCleanupService> logger)
    : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(6);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Paste cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredPastes(stoppingToken);
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error during paste cleanup");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        logger.LogInformation("Paste cleanup service stopped");
    }

    private async Task CleanupExpiredPastes(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var pasteService = scope.ServiceProvider.GetRequiredService<PasteService>();

        try
        {
            logger.LogDebug("Beginning cleanup of expired pastes...");
            
            var deletedCount = await pasteService.DeleteExpiredPastesAsync();
            if (deletedCount > 0)
            {
                logger.LogDebug("Cleanup completed: {DeletedCount} expired pastes removed", deletedCount);
            }
            else
            {
                logger.LogDebug("Cleanup completed: No expired pastes found");
            }
        }
        catch (IOException e)
        {
            logger.LogWarning("Failed to cleanup expired pastes");
            throw;
        }
    }
}