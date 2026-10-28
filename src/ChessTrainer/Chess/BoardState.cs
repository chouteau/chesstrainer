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

    public string ToFen()
    {
        var sb = new System.Text.StringBuilder();
        for (int rank = 7; rank >= 0; rank--)
        {
            int emptyCount = 0;
            for (int file = 0; file < 8; file++)
            {
                var piece = squares[(rank * 8) + file];
                if (piece is null)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        sb.Append(emptyCount);
                        emptyCount = 0;
                    }
                    sb.Append(piece.Value);
                }
            }
            if (emptyCount > 0)
            {
                sb.Append(emptyCount);
            }
            if (rank > 0)
            {
                sb.Append('/');
            }
        }

        sb.Append(' ').Append(SideToMove == "white" ? 'w' : 'b');
        sb.Append(" - "); // Castling non critique pour l'affichage visuel statique
        sb.Append(EnPassantSquare ?? "-");
        sb.Append(" 0 1");
        return sb.ToString();
    }

    public bool TryApplySan(string san)
    {
        san = san.Trim().TrimEnd('+', '#', '!', '?');
        if (string.IsNullOrWhiteSpace(san)) return false;

        var isWhite = SideToMove == "white";

        // Roque
        if (san is "O-O" or "0-0")
        {
            var rank = isWhite ? 1 : 8;
            Apply($"e{rank}g{rank}");
            return true;
        }
        if (san is "O-O-O" or "0-0-0")
        {
            var rank = isWhite ? 1 : 8;
            Apply($"e{rank}c{rank}");
            return true;
        }

        // Détection de promotion
        char? promo = null;
        if (san.Contains('='))
        {
            var parts = san.Split('=');
            san = parts[0];
            if (parts[1].Length > 0) promo = parts[1][0];
        }

        // Cible (2 derniers caractères)
        if (san.Length < 2) return false;
        var dest = san[^2..];
        if (dest[0] < 'a' || dest[0] > 'h' || dest[1] < '1' || dest[1] > '8')
            return false;

        var destIndex = ToIndex(dest);
        var targetFile = dest[0] - 'a';
        var targetRank = dest[1] - '1';

        // Type de pièce
        char pieceChar = 'P';
        var prefix = san[..^2].Replace("x", "");
        if (prefix.Length > 0 && char.IsUpper(prefix[0]))
        {
            pieceChar = prefix[0];
            prefix = prefix[1..];
        }

        char pieceToFind = isWhite ? char.ToUpperInvariant(pieceChar) : char.ToLowerInvariant(pieceChar);

        // Disambiguation
        char? disambigFile = null;
        char? disambigRank = null;
        if (prefix.Length > 0)
        {
            if (prefix[0] >= 'a' && prefix[0] <= 'h') disambigFile = prefix[0];
            else if (prefix[0] >= '1' && prefix[0] <= '8') disambigRank = prefix[0];

            if (prefix.Length > 1 && prefix[1] >= '1' && prefix[1] <= '8') disambigRank = prefix[1];
        }

        // Trouver la pièce candidate qui peut aller sur dest
        for (int idx = 0; idx < 64; idx++)
        {
            if (squares[idx] != pieceToFind) continue;

            var f = idx % 8;
            var r = idx / 8;

            if (disambigFile.HasValue && ('a' + f) != disambigFile.Value) continue;
            if (disambigRank.HasValue && ('1' + r) != disambigRank.Value) continue;

            var sq = ToSquare(idx);

            // Vérifier validité géométrique basique
            bool canMove = pieceChar switch
            {
                'P' => CanPawnMove(idx, destIndex, isWhite, san.Contains('x') || (prefix.Length > 0 && prefix[0] >= 'a' && prefix[0] <= 'h')),
                'N' => (Math.Abs(f - targetFile) == 1 && Math.Abs(r - targetRank) == 2) || (Math.Abs(f - targetFile) == 2 && Math.Abs(r - targetRank) == 1),
                'B' => Math.Abs(f - targetFile) == Math.Abs(r - targetRank) && IsRayClear(idx, destIndex),
                'R' => (f == targetFile || r == targetRank) && IsRayClear(idx, destIndex),
                'Q' => ((f == targetFile || r == targetRank) || Math.Abs(f - targetFile) == Math.Abs(r - targetRank)) && IsRayClear(idx, destIndex),
                'K' => Math.Abs(f - targetFile) <= 1 && Math.Abs(r - targetRank) <= 1,
                _ => false
            };

            if (canMove)
            {
                var uci = promo.HasValue ? $"{sq}{dest}{char.ToLowerInvariant(promo.Value)}" : $"{sq}{dest}";
                Apply(uci);
                return true;
            }
        }

        return false;
    }

    private bool CanPawnMove(int fromIdx, int toIdx, bool isWhite, bool isCapture)
    {
        var fromF = fromIdx % 8;
        var fromR = fromIdx / 8;
        var toF = toIdx % 8;
        var toR = toIdx / 8;
        var forward = isWhite ? 1 : -1;
        var startR = isWhite ? 1 : 6;

        if (isCapture)
        {
            return toR - fromR == forward && Math.Abs(toF - fromF) == 1;
        }

        if (fromF == toF)
        {
            if (toR - fromR == forward && squares[toIdx] is null) return true;
            if (fromR == startR && toR - fromR == forward * 2 && squares[toIdx] is null && squares[fromIdx + (forward * 8)] is null) return true;
        }

        return false;
    }

    private bool IsRayClear(int fromIdx, int toIdx)
    {
        var fromF = fromIdx % 8;
        var fromR = fromIdx / 8;
        var toF = toIdx % 8;
        var toR = toIdx / 8;

        var stepF = Math.Sign(toF - fromF);
        var stepR = Math.Sign(toR - fromR);

        var curF = fromF + stepF;
        var curR = fromR + stepR;

        while (curF != toF || curR != toR)
        {
            if (squares[(curR * 8) + curF] is not null) return false;
            curF += stepF;
            curR += stepR;
        }

        return true;
    }
}
