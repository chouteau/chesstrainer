namespace ChessTrainer.Chess;

public sealed class BoardState
{
    public const string StartFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private readonly char?[] squares = new char?[64];

    public string SideToMove { get; private set; } = "white";
    public string? EnPassantSquare { get; private set; }

    public char? this[string square] => squares[ToIndex(square)];

    public static BoardState FromFen(string? fen)
    {
        var board = new BoardState();
        var sections = (string.IsNullOrWhiteSpace(fen) ? StartFen : fen).Split(' ');
        var rank = 7;
        var file = 0;

        foreach (var symbol in sections[0])
        {
            if (symbol == '/')
            {
                rank--;
                file = 0;
            }
            else if (char.IsDigit(symbol))
            {
                file += symbol - '0';
            }
            else
            {
                board.squares[(rank * 8) + file] = symbol;
                file++;
            }
        }

        board.SideToMove = sections.Length > 1 && sections[1] == "b" ? "black" : "white";
        board.EnPassantSquare = sections.Length > 3 && sections[3] != "-" ? sections[3] : null;
        return board;
    }

    public void Apply(string uci)
    {
        if (uci.Length < 4)
        {
            return;
        }

        var from = uci[..2];
        var to = uci.Substring(2, 2);
        var fromIndex = ToIndex(from);
        var toIndex = ToIndex(to);
        var piece = squares[fromIndex];

        if (piece is null)
        {
            throw new InvalidOperationException($"Aucune pièce sur la case de départ {from} pour le coup {uci}.");
        }

        if (char.ToLowerInvariant(piece.Value) == 'p' && to == EnPassantSquare && squares[toIndex] is null)
        {
            var capturedPawnIndex = toIndex + (char.IsUpper(piece.Value) ? -8 : 8);
            squares[capturedPawnIndex] = null;
        }

        if (char.ToLowerInvariant(piece.Value) == 'k' && Math.Abs((fromIndex % 8) - (toIndex % 8)) == 2)
        {
            var rank = fromIndex / 8;
            var kingSide = (toIndex % 8) > (fromIndex % 8);
            var rookFrom = (rank * 8) + (kingSide ? 7 : 0);
            var rookTo = (rank * 8) + (kingSide ? 5 : 3);
            squares[rookTo] = squares[rookFrom];
            squares[rookFrom] = null;
        }

        squares[toIndex] = uci.Length == 5
            ? (char.IsUpper(piece.Value) ? char.ToUpperInvariant(uci[4]) : char.ToLowerInvariant(uci[4]))
            : piece;
        squares[fromIndex] = null;

        EnPassantSquare = null;
        if (char.ToLowerInvariant(piece.Value) == 'p' && Math.Abs(toIndex - fromIndex) == 16)
        {
            EnPassantSquare = ToSquare((fromIndex + toIndex) / 2);
        }

        SideToMove = SideToMove == "white" ? "black" : "white";
    }

    public static int ToIndex(string square) =>
        ((square[1] - '1') * 8) + (square[0] - 'a');

    public static string ToSquare(int index) =>
        $"{(char)('a' + (index % 8))}{(char)('1' + (index / 8))}";
}
