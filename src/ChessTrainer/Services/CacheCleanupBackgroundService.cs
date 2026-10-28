namespace ChessTrainer.Services;

public sealed class CacheCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CacheCleanupBackgroundService> _logger;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

    public CacheCleanupBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CacheCleanupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CacheCleanupBackgroundService démarré. Vérification de la rétention 24h.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var storage = scope.ServiceProvider.GetRequiredService<ChessGameStorageService>();
                var purged = storage.PurgeExpiredCaches();
                if (purged > 0)
                {
                    _logger.LogInformation("Nettoyage du cache terminé : {Count} dossiers expirés supprimés.", purged);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur durant le cycle de nettoyage du cache.");
            }

            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
