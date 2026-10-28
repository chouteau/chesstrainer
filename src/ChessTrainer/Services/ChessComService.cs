using System.Net;
using System.Net.Http.Json;
using ChessTrainer.Models.ChessCom;

namespace ChessTrainer.Services;

public sealed record FetchProgress(
    int TotalArchives,
    int DownloadedArchives,
    int TotalGamesFetched,
    string CurrentStatus,
    bool IsCompleted = false,
    string? ErrorMessage = null);

public sealed class ChessComService
{
    private readonly HttpClient _httpClient;
    private readonly ChessGameStorageService _storageService;
    private readonly ILogger<ChessComService> _logger;

    public ChessComService(
        HttpClient httpClient,
        ChessGameStorageService storageService,
        ILogger<ChessComService> logger)
    {
        _httpClient = httpClient;
        _storageService = storageService;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://api.chess.com/pub/");
        _httpClient.DefaultRequestHeaders.UserAgent.Clear();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ChessTrainerApp/1.0 (contact: chesstrainer.chouteau.info)");
    }

    public async Task<IReadOnlyList<ChessComGame>> GetOrFetchPlayerGamesAsync(
        string username,
        int maxMonthsBack = 6,
        IProgress<FetchProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var cleanUser = ChessGameStorageService.SanitizeUsername(username);

        // 1. Vérifier si un cache valide (< 24h) existe déjà
        if (_storageService.HasValidCache(cleanUser, out var meta))
        {
            progress?.Report(new FetchProgress(
                meta!.TotalMonths,
                meta.TotalMonths,
                meta.TotalGames,
                $"Données chargées depuis le cache local ({meta.TotalGames} parties trouvées, expire à {meta.ExpiresAtUtc.ToLocalTime():HH:mm}).",
                IsCompleted: true));

            return await _storageService.LoadGamesAsync(cleanUser);
        }

        // 2. Sinon, interroger l'API Chess.com
        progress?.Report(new FetchProgress(0, 0, 0, $"Connexion à Chess.com pour le joueur '{cleanUser}'..."));

        ArchivesResponse? archivesResp;
        try
        {
            archivesResp = await _httpClient.GetFromJsonAsync<ArchivesResponse>(
                $"player/{cleanUser}/games/archives", cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            var msg = $"Joueur Chess.com '{cleanUser}' introuvable.";
            progress?.Report(new FetchProgress(0, 0, 0, msg, IsCompleted: true, ErrorMessage: msg));
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des archives pour {Username}", cleanUser);
            var msg = $"Impossible de contacter Chess.com : {ex.Message}";
            progress?.Report(new FetchProgress(0, 0, 0, msg, IsCompleted: true, ErrorMessage: msg));
            return [];
        }

        var allArchives = archivesResp?.Archives ?? [];
        if (allArchives.Count == 0)
        {
            var msg = $"Aucune partie archivée trouvée pour '{cleanUser}'.";
            progress?.Report(new FetchProgress(0, 0, 0, msg, IsCompleted: true));
            return [];
        }

        // Limiter selon maxMonthsBack (ex: 6 derniers mois pour réactivité, ou tous si maxMonthsBack <= 0)
        var archivesToFetch = (maxMonthsBack > 0 && allArchives.Count > maxMonthsBack)
            ? allArchives.TakeLast(maxMonthsBack).ToList()
            : allArchives.ToList();

        var totalArchives = archivesToFetch.Count;
        var allGames = new List<ChessComGame>();

        for (int i = 0; i < totalArchives; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var archiveUrl = archivesToFetch[i];
            // Format URL : https://api.chess.com/pub/player/{username}/games/{YYYY}/{MM}
            // On extrait la partie relative après https://api.chess.com/pub/
            var relativePath = archiveUrl.Replace("https://api.chess.com/pub/", "", StringComparison.OrdinalIgnoreCase);

            progress?.Report(new FetchProgress(
                totalArchives,
                i,
                allGames.Count,
                $"Téléchargement du mois {i + 1}/{totalArchives} ({GetMonthName(archiveUrl)})..."));

            try
            {
                var monthGames = await _httpClient.GetFromJsonAsync<MonthlyGamesResponse>(relativePath, cancellationToken);
                if (monthGames?.Games != null)
                {
                    allGames.AddRange(monthGames.Games);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erreur téléchargement archive {Url}", archiveUrl);
            }

            // Temporisation de courtoisie pour respecter la politesse API (rate limiting)
            await Task.Delay(120, cancellationToken);
        }

        // Sauvegarde dans le cache 24h
        await _storageService.SaveGamesAsync(cleanUser, allGames, totalArchives);

        progress?.Report(new FetchProgress(
            totalArchives,
            totalArchives,
            allGames.Count,
            $"Terminé ! {allGames.Count} parties récupérées avec succès (conservées 24h).",
            IsCompleted: true));

        return allGames;
    }

    private static string GetMonthName(string url)
    {
        var parts = url.TrimEnd('/').Split('/');
        if (parts.Length >= 2)
        {
            return $"{parts[^1]}/{parts[^2]}";
        }
        return url;
    }
}
