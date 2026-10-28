using System.Text.RegularExpressions;
using ChessTrainer.Chess;
using ChessTrainer.Models;
using ChessTrainer.Models.ChessCom;

namespace ChessTrainer.Services;

public sealed class GameMistakeAnalyzer
{
    public IReadOnlyList<OpeningMistakeSummary> AnalyzeLosses(string username, IReadOnlyList<ChessComGame> games)
    {
        var cleanUser = ChessGameStorageService.SanitizeUsername(username);

        // 1. Filtrer les parties où le joueur a perdu
        var lostGames = games.Where(g =>
        {
            var p = g.GetPlayer(cleanUser);
            if (p?.Result == null) return false;

            // Codes de défaite Chess.com : checkmated, resigned, timeout, abandoned, etc.
            return p.Result is "checkmated" or "resigned" or "timeout" or "abandoned" or "lose";
        }).ToList();

        // 2. Extraire les exercices d'erreurs pour chaque défaite
        var allMistakes = new List<MistakeExercise>();
        int mistakeCounter = 1;

        foreach (var game in lostGames)
        {
            var exercise = ExtractMistakeExercise(cleanUser, game, mistakeCounter);
            if (exercise != null)
            {
                allMistakes.Add(exercise);
                mistakeCounter++;
            }
        }

        // 3. Regrouper par Ouverture / ECO
        var groups = allMistakes
            .GroupBy(m => string.IsNullOrWhiteSpace(m.OpeningName) ? (string.IsNullOrWhiteSpace(m.Eco) ? "Ouverture inconnue" : m.Eco) : m.OpeningName)
            .OrderByDescending(g => g.Count())
            .Take(15) // Top 15 des ouvertures problématiques
            .Select(g =>
            {
                var totalInOpening = games.Count(gm => GetOpeningInfo(gm).name == g.Key || GetOpeningInfo(gm).eco == g.Key);
                var losses = g.Count();
                var rate = totalInOpening > 0 ? (double)losses / totalInOpening : 1.0;

                return new OpeningMistakeSummary(
                    OpeningName: g.Key,
                    Eco: g.First().Eco,
                    TotalGames: totalInOpening > 0 ? totalInOpening : losses,
                    Losses: losses,
                    LossRate: Math.Round(rate * 100, 1),
                    Exercises: g.OrderByDescending(x => x.PlayedAtUtc).ToList()
                );
            })
            .ToList();

        return groups;
    }

    private static readonly Regex EcoRegex = new(@"\[ECO\s+""(?<eco>[^""]+)""\]", RegexOptions.Compiled);
    private static readonly Regex OpeningNameRegex = new(@"\[Opening\s+""(?<opening>[^""]+)""\]", RegexOptions.Compiled);
    private static readonly Regex EcoUrlRegex = new(@"\[ECOUrl\s+""(?<url>[^""]+)""\]", RegexOptions.Compiled);

    private MistakeExercise? ExtractMistakeExercise(string username, ChessComGame game, int index)
    {
        if (string.IsNullOrWhiteSpace(game.Pgn)) return null;

        var (openingName, eco) = GetOpeningInfo(game);
        var player = game.GetPlayer(username);
        var opponent = game.GetOpponent(username);
        var isWhite = game.IsPlayerWhite(username);
        var playerColor = isWhite ? "white" : "black";

        // Parser la liste des coups SAN dans le PGN
        var sanMoves = ParseSanMoves(game.Pgn);
        if (sanMoves.Count < 4) return null; // Partie trop courte

        // Reconstruire l'échiquier sur la phase d'OUVERTURE (environ 6 à 10 premiers coups = 12 à 20 demi-coups)
        // afin d'afficher la structure de l'ouverture et non pas la finale
        var targetOpeningHalfMoves = Math.Min(sanMoves.Count, Math.Min(16, (sanMoves.Count / 2) * 2)); // Coup 8 max ou milieu
        if (targetOpeningHalfMoves < 4) targetOpeningHalfMoves = sanMoves.Count;

        var openingBoard = BoardState.FromFen(BoardState.StartFen);
        var playedCount = 0;
        for (int i = 0; i < targetOpeningHalfMoves; i++)
        {
            if (openingBoard.TryApplySan(sanMoves[i]))
            {
                playedCount++;
            }
            else
            {
                break;
            }
        }

        // Si on a pu simuler l'ouverture, on affiche cette position d'ouverture
        // Sinon repli sur le FEN initial
        var fenToDisplay = playedCount >= 4 ? openingBoard.ToFen() : (game.Fen ?? BoardState.StartFen);

        var openingMoveSan = (playedCount > 0 && playedCount <= sanMoves.Count) ? sanMoves[playedCount - 1] : sanMoves[0];

        // Explication pédagogique
        var reason = player?.Result switch
        {
            "checkmated" => "Partie conclue par échec et mat après un déséquilibre né de l'ouverture.",
            "resigned" => "Abandon : position compromise ou pièce perdue suite au plan adverse.",
            "timeout" => "Dépassé au temps dans une position complexe issue de cette ouverture.",
            _ => "Partie perdue : révise les premiers coups et les variantes principales."
        };

        var moveNumber = (playedCount + 1) / 2;
        if (moveNumber < 1) moveNumber = 1;

        return new MistakeExercise(
            Id: $"err-{index:000}",
            GameUrl: game.Url ?? "#",
            OpeningName: string.IsNullOrWhiteSpace(openingName) ? "Partie standard" : openingName,
            Eco: eco,
            PlayerColor: playerColor,
            PlayerRating: player?.Rating ?? 1200,
            OpponentRating: opponent?.Rating ?? 1200,
            OpponentName: opponent?.Username ?? "Adversaire",
            ResultCode: player?.Result ?? "defaite",
            Explanation: reason,
            FenBeforeMistake: fenToDisplay,
            MistakeMoveSan: openingMoveSan,
            MistakeMoveUci: "",
            PlayedAtUtc: game.EndTimeUtc,
            MoveNumber: moveNumber
        );
    }

    private static (string name, string eco) GetOpeningInfo(ChessComGame game)
    {
        var eco = game.Eco ?? "";
        var name = "";

        // 1. Chercher dans ECOUrl (ex: https://www.chess.com/openings/Queens-Pawn-Opening-Accelerated-London-System-2...Nc6)
        if (game.Pgn != null)
        {
            var matchEcoUrl = EcoUrlRegex.Match(game.Pgn);
            if (matchEcoUrl.Success)
            {
                var rawUrl = matchEcoUrl.Groups["url"].Value;
                var slug = rawUrl.Split('/').LastOrDefault() ?? "";
                if (!string.IsNullOrWhiteSpace(slug))
                {
                    // Nettoyer les tirets et numéros
                    name = CleanOpeningSlug(slug);
                }
            }

            var matchEco = EcoRegex.Match(game.Pgn);
            if (matchEco.Success && string.IsNullOrEmpty(eco))
            {
                eco = matchEco.Groups["eco"].Value;
            }

            var matchOp = OpeningNameRegex.Match(game.Pgn);
            if (matchOp.Success && string.IsNullOrWhiteSpace(name))
            {
                name = matchOp.Groups["opening"].Value;
            }
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            name = !string.IsNullOrWhiteSpace(eco) ? $"Ouverture {eco}" : "Partie d'échecs";
        }

        return (name, eco);
    }

    private static string CleanOpeningSlug(string slug)
    {
        // Remplacer - par espace et décoder les variations
        var clean = slug.Replace("-", " ");
        // Ex: "Queens Pawn Opening Accelerated London System 2...Nc6"
        clean = Regex.Replace(clean, @"\b\d+\.\.\.[A-Za-z0-9]+", "");
        clean = Regex.Replace(clean, @"\b\d+\.[A-Za-z0-9]+", "");
        clean = Regex.Replace(clean, @"\s+", " ");
        return clean.Trim();
    }

    private static List<string> ParseSanMoves(string pgn)
    {
        var moves = new List<string>();
        // Trouver la section des coups après les métadonnées [Header "value"]
        var lines = pgn.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var moveLines = lines.Where(l => !l.TrimStart().StartsWith('['));
        var movesText = string.Join(" ", moveLines);

        // Nettoyer les commentaires { ... }, les variations ( ... ), les résultats 1-0, 0-1, 1/2-1/2, *
        movesText = Regex.Replace(movesText, @"\{[^}]*\}", "");
        movesText = Regex.Replace(movesText, @"\([^)]*\)", "");
        movesText = Regex.Replace(movesText, @"1-0|0-1|1/2-1/2|\*", "");

        // Extraire les jetons
        var tokens = movesText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            var t = token.Trim();
            // Ignorer les numéros de coup "1.", "2...", "$1"
            if (Regex.IsMatch(t, @"^\d+\.*$") || t.StartsWith('$'))
                continue;

            moves.Add(t);
        }

        return moves;
    }
}
