using System.Text.Json;
using System.Text.Json.Serialization;
using ChessTrainer.Models.ChessCom;

namespace ChessTrainer.Services;

public sealed record CacheMetadata(
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("lastSyncUtc")] DateTime LastSyncUtc,
    [property: JsonPropertyName("expiresAtUtc")] DateTime ExpiresAtUtc,
    [property: JsonPropertyName("totalGames")] int TotalGames,
    [property: JsonPropertyName("totalMonths")] int TotalMonths);

public sealed class ChessGameStorageService
{
    private readonly string _cacheRoot;
    private readonly ILogger<ChessGameStorageService> _logger;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public ChessGameStorageService(IWebHostEnvironment env, ILogger<ChessGameStorageService> logger)
    {
        _logger = logger;
        _cacheRoot = Path.Combine(env.ContentRootPath, "App_Data", "cache");
        EnsureDirectoryExists(_cacheRoot);
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public string GetPlayerDirectory(string username)
    {
        var safeUsername = SanitizeUsername(username);
        return Path.Combine(_cacheRoot, safeUsername);
    }

    public static string SanitizeUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Le pseudonyme ne peut pas être vide.", nameof(username));

        // Un pseudo chess.com ne contient que lettres, chiffres, tirets et underscores
        var clean = new string(username.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray()).ToLowerInvariant();
        return string.IsNullOrEmpty(clean) ? "anonymous" : clean;
    }

    public bool HasValidCache(string username, out CacheMetadata? metadata)
    {
        metadata = null;
        var dir = GetPlayerDirectory(username);
        if (!Directory.Exists(dir))
        {
            return false;
        }

        var metaPath = Path.Combine(dir, "metadata.json");
        if (!File.Exists(metaPath))
        {
            DeletePlayerDirectory(username);
            return false;
        }

        try
        {
            var json = File.ReadAllText(metaPath);
            metadata = JsonSerializer.Deserialize<CacheMetadata>(json);
            if (metadata is null || DateTime.UtcNow >= metadata.ExpiresAtUtc)
            {
                _logger.LogInformation("Cache expiré (>24h) pour {Username}, suppression...", username);
                DeletePlayerDirectory(username);
                metadata = null;
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur lecture métadonnées cache pour {Username}", username);
            DeletePlayerDirectory(username);
            return false;
        }
    }

    public async Task SaveGamesAsync(string username, IReadOnlyList<ChessComGame> games, int totalMonths)
    {
        var dir = GetPlayerDirectory(username);
        EnsureDirectoryExists(dir);

        var now = DateTime.UtcNow;
        var metadata = new CacheMetadata(
            Username: username.ToLowerInvariant(),
            LastSyncUtc: now,
            ExpiresAtUtc: now.Add(CacheDuration),
            TotalGames: games.Count,
            TotalMonths: totalMonths
        );

        var metaPath = Path.Combine(dir, "metadata.json");
        var gamesPath = Path.Combine(dir, "games.json");

        var options = new JsonSerializerOptions { WriteIndented = true };
        await File.WriteAllTextAsync(metaPath, JsonSerializer.Serialize(metadata, options));
        await File.WriteAllTextAsync(gamesPath, JsonSerializer.Serialize(games, options));

        _logger.LogInformation("Sauvegarde de {Count} parties pour {Username} avec expiration le {Expires}", games.Count, username, metadata.ExpiresAtUtc);
    }

    public async Task<IReadOnlyList<ChessComGame>> LoadGamesAsync(string username)
    {
        if (!HasValidCache(username, out _))
        {
            return [];
        }

        var dir = GetPlayerDirectory(username);
        var gamesPath = Path.Combine(dir, "games.json");
        if (!File.Exists(gamesPath))
        {
            return [];
        }

        try
        {
            var json = await File.ReadAllTextAsync(gamesPath);
            var games = JsonSerializer.Deserialize<List<ChessComGame>>(json);
            return games ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des parties pour {Username}", username);
            return [];
        }
    }

    public void DeletePlayerDirectory(string username)
    {
        try
        {
            var dir = GetPlayerDirectory(username);
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
                _logger.LogInformation("Cache supprimé pour {Username}", username);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de supprimer le répertoire pour {Username}", username);
        }
    }

    public int PurgeExpiredCaches()
    {
        if (!Directory.Exists(_cacheRoot)) return 0;

        var purgedCount = 0;
        var now = DateTime.UtcNow;

        foreach (var dir in Directory.GetDirectories(_cacheRoot))
        {
            try
            {
                var metaPath = Path.Combine(dir, "metadata.json");
                var shouldDelete = false;

                if (File.Exists(metaPath))
                {
                    var json = File.ReadAllText(metaPath);
                    var metadata = JsonSerializer.Deserialize<CacheMetadata>(json);
                    if (metadata is null || now >= metadata.ExpiresAtUtc)
                    {
                        shouldDelete = true;
                    }
                }
                else
                {
                    // Aucun metadata ou dossier orphelin plus vieux de 24h
                    var dirTime = Directory.GetCreationTimeUtc(dir);
                    if (now - dirTime > CacheDuration)
                    {
                        shouldDelete = true;
                    }
                }

                if (shouldDelete)
                {
                    Directory.Delete(dir, true);
                    purgedCount++;
                    _logger.LogInformation("Purge automatique du cache expiré : {Dir}", Path.GetFileName(dir));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erreur lors de la purge de {Dir}", dir);
            }
        }

        return purgedCount;
    }
}
