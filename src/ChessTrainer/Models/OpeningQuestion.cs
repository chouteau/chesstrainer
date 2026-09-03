namespace ChessTrainer.Models;

public sealed record OpeningQuestion(
    int Id,
    string Name,
    string Level,
    string MoveLine,
    string Description,
    IReadOnlyList<string> Moves);
