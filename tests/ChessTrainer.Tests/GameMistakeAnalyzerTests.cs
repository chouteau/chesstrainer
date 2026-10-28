using ChessTrainer.Chess;
using ChessTrainer.Models.ChessCom;
using ChessTrainer.Services;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace ChessTrainer.Tests;

public class GameMistakeAnalyzerTests
{
    private readonly ITestOutputHelper _output;

    public GameMistakeAnalyzerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test_All_Real_Games_Microogle()
    {
        var cachePath = @"e:\S5\chouteau\ChessTrainer\src\ChessTrainer\App_Data\cache\microogle\games.json";
        if (!File.Exists(cachePath)) return;

        var json = File.ReadAllText(cachePath);
        var games = JsonSerializer.Deserialize<List<ChessComGame>>(json);
        Assert.NotNull(games);

        var analyzer = new GameMistakeAnalyzer();
        var summaries = analyzer.AnalyzeLosses("Microogle", games);

        _output.WriteLine($"Nombre d'ouvertures trouvées: {summaries.Count}");
        foreach (var s in summaries)
        {
            _output.WriteLine($"OUVERTURE: {s.OpeningName} ({s.Eco}) - {s.Losses} défaites");
            foreach (var ex in s.Exercises)
            {
                _output.WriteLine($"   Ex: {ex.MistakeMoveSan} | FEN: {ex.FenBeforeMistake}");
                // Vérifier qu'on n'a pas la finale d'échec et mat avec juste le roi ou très peu de pièces
                // Un FEN de début de partie doit comporter la plupart des pièces
                Assert.False(ex.FenBeforeMistake.StartsWith("6k1/8"), $"FEN {ex.FenBeforeMistake} ressemble à une finale!");
            }
        }
    }
}
