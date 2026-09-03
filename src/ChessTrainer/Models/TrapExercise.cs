namespace ChessTrainer.Models;

public sealed record TrapMove(string Uci, string San, string Hint);

public sealed record TrapExercise(
    int Id,
    string Title,
    string ShortTitle,
    string Difficulty,
    string Category,
    string Opening,
    string PlayerColor,
    string Variant,
    string InitialFen,
    IReadOnlyList<TrapMove> Moves)
{
    public bool IsWhite => PlayerColor == "white";
    public string SideLabel => IsWhite ? "Attaque · Blancs" : "Défense · Noirs";
    public int UserMoveCount => (Moves.Count + 1) / 2;
}
