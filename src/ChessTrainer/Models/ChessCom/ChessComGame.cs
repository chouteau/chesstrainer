using System.Text.Json.Serialization;

namespace ChessTrainer.Models.ChessCom;

public sealed record ArchivesResponse(
    [property: JsonPropertyName("archives")] IReadOnlyList<string>? Archives);

public sealed record MonthlyGamesResponse(
    [property: JsonPropertyName("games")] IReadOnlyList<ChessComGame>? Games);

public sealed record ChessComGame(
    [property: JsonPropertyName("url")] string? Url,
    [property: JsonPropertyName("pgn")] string? Pgn,
    [property: JsonPropertyName("time_control")] string? TimeControl,
    [property: JsonPropertyName("end_time")] long EndTime,
    [property: JsonPropertyName("rated")] bool? Rated,
    [property: JsonPropertyName("fen")] string? Fen,
    [property: JsonPropertyName("time_class")] string? TimeClass,
    [property: JsonPropertyName("rules")] string? Rules,
    [property: JsonPropertyName("white")] ChessComPlayer? White,
    [property: JsonPropertyName("black")] ChessComPlayer? Black,
    [property: JsonPropertyName("eco")] string? Eco,
    [property: JsonPropertyName("accuracies")] ChessComAccuracies? Accuracies)
{
    public DateTime EndTimeUtc => DateTimeOffset.FromUnixTimeSeconds(EndTime).UtcDateTime;

    public bool IsPlayerWhite(string username) =>
        string.Equals(White?.Username, username, StringComparison.OrdinalIgnoreCase);

    public bool IsPlayerBlack(string username) =>
        string.Equals(Black?.Username, username, StringComparison.OrdinalIgnoreCase);

    public ChessComPlayer? GetPlayer(string username) =>
        IsPlayerWhite(username) ? White : (IsPlayerBlack(username) ? Black : null);

    public ChessComPlayer? GetOpponent(string username) =>
        IsPlayerWhite(username) ? Black : (IsPlayerBlack(username) ? White : null);

    public string GetPlayerColor(string username) =>
        IsPlayerWhite(username) ? "white" : "black";
}

public sealed record ChessComPlayer(
    [property: JsonPropertyName("username")] string? Username,
    [property: JsonPropertyName("rating")] int Rating,
    [property: JsonPropertyName("result")] string? Result,
    [property: JsonPropertyName("@id")] string? Id,
    [property: JsonPropertyName("uuid")] string? Uuid);

public sealed record ChessComAccuracies(
    [property: JsonPropertyName("white")] double? White,
    [property: JsonPropertyName("black")] double? Black);
