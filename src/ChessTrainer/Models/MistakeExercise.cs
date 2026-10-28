namespace ChessTrainer.Models;

public sealed record MistakeExercise(
    string Id,
    string GameUrl,
    string OpeningName,
    string Eco,
    string PlayerColor,
    int PlayerRating,
    int OpponentRating,
    string OpponentName,
    string ResultCode,
    string Explanation,
    string FenBeforeMistake,
    string MistakeMoveSan,
    string MistakeMoveUci,
    DateTime PlayedAtUtc,
    int MoveNumber = 1)
{
    public bool IsWhite => PlayerColor == "white";
    public string SideLabel => IsWhite ? "Blancs" : "Noirs";
    public string FormattedDate => PlayedAtUtc.ToString("dd/MM/yyyy HH:mm");
}

public sealed record OpeningMistakeSummary(
    string OpeningName,
    string Eco,
    int TotalGames,
    int Losses,
    double LossRate,
    IReadOnlyList<MistakeExercise> Exercises);
