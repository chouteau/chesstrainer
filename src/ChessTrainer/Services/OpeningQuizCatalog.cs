using ChessTrainer.Models;

namespace ChessTrainer.Services;

public sealed class OpeningQuizCatalog
{
    public const string StudyUrl = "https://lichess.org/study/XEOOZt7Z";

    public IReadOnlyList<OpeningQuestion> Questions { get; } =
    [
        Q(1, "Partie italienne", "Débutant", "1. e4 e5 2. Cf3 Cc6 3. Fc4",
            "Le fou blanc vise immédiatement la case sensible f7.",
            "e2e4", "e7e5", "g1f3", "b8c6", "f1c4"),
        Q(2, "Partie viennoise", "Débutant", "1. e4 e5 2. Cc3",
            "Les Blancs développent d’abord le cavalier dame et gardent plusieurs plans avec f4.",
            "e2e4", "e7e5", "b1c3"),
        Q(3, "Partie espagnole", "Débutant", "1. e4 e5 2. Cf3 Cc6 3. Fb5",
            "Le fou exerce une pression indirecte sur le pion e5 en visant son défenseur.",
            "e2e4", "e7e5", "g1f3", "b8c6", "f1b5"),
        Q(4, "Défense Petrov", "Débutant", "1. e4 e5 2. Cf3 Cf6",
            "Les Noirs répondent à l’attaque sur e5 par une contre-attaque symétrique sur e4.",
            "e2e4", "e7e5", "g1f3", "g8f6"),
        Q(5, "Partie écossaise", "Débutant", "1. e4 e5 2. Cf3 Cc6 3. d4",
            "Les Blancs ouvrent rapidement le centre avec d4.",
            "e2e4", "e7e5", "g1f3", "b8c6", "d2d4"),
        Q(6, "Système de Londres", "Débutant", "1. d4 d5 2. Cf3 Cf6 3. Ff4",
            "Le fou de cases noires sort avant que le pion e ne ferme sa diagonale.",
            "d2d4", "d7d5", "g1f3", "g8f6", "c1f4"),
        Q(7, "Partie des quatre cavaliers", "Débutant", "1. e4 e5 2. Cf3 Cc6 3. Cc3 Cf6",
            "Les quatre cavaliers occupent leurs cases naturelles dès l’ouverture.",
            "e2e4", "e7e5", "g1f3", "b8c6", "b1c3", "g8f6"),

        Q(8, "Défense Caro-Kann", "Intermédiaire", "1. e4 c6",
            "Les Noirs préparent d5 tout en gardant leur fou de cases blanches libre.",
            "e2e4", "c7c6"),
        Q(9, "Défense Alekhine", "Intermédiaire", "1. e4 Cf6",
            "Le cavalier provoque l’avance des pions blancs afin de les attaquer ensuite.",
            "e2e4", "g8f6"),
        Q(10, "Défense Grünfeld", "Intermédiaire", "1. d4 Cf6 2. c4 g6 3. Cc3 d5",
            "Les Noirs laissent les Blancs bâtir un centre, puis le frappent avec d5.",
            "d2d4", "g8f6", "c2c4", "g7g6", "b1c3", "d7d5"),
        Q(11, "Gambit du roi", "Intermédiaire", "1. e4 e5 2. f4",
            "Les Blancs offrent le pion f pour détourner le pion e5 et ouvrir la colonne f.",
            "e2e4", "e7e5", "f2f4"),
        Q(12, "Gambit Benko", "Intermédiaire", "1. d4 Cf6 2. c4 c5 3. d5 b5",
            "Les Noirs sacrifient un pion pour obtenir des lignes actives sur l’aile dame.",
            "d2d4", "g8f6", "c2c4", "c7c5", "d4d5", "b7b5"),
        Q(13, "Sicilienne, variante du dragon", "Intermédiaire",
            "1. e4 c5 2. Cf3 d6 3. d4 cxd4 4. Cxd4 Cf6 5. Cc3 g6",
            "Le fianchetto noir dessine une structure de pions rappelant un dragon.",
            "e2e4", "c7c5", "g1f3", "d7d6", "d2d4", "c5d4", "f3d4", "g8f6", "b1c3", "g7g6"),
        Q(14, "Défense hollandaise", "Intermédiaire", "1. d4 f5",
            "Les Noirs contrôlent e4 et annoncent un jeu ambitieux sur l’aile roi.",
            "d2d4", "f7f5"),
        Q(15, "Défense française", "Intermédiaire", "1. e4 e6",
            "Les Noirs préparent d5 et acceptent temporairement d’enfermer leur fou c8.",
            "e2e4", "e7e6"),

        Q(16, "Défense est-indienne", "Avancé", "1. d4 Cf6 2. c4 g6 3. Cc3 Fg7 4. e4 d6",
            "Les Noirs autorisent un large centre blanc avant de le contre-attaquer.",
            "d2d4", "g8f6", "c2c4", "g7g6", "b1c3", "f8g7", "e2e4", "d7d6"),
        Q(17, "Défense nimzo-indienne", "Avancé", "1. d4 Cf6 2. c4 e6 3. Cc3 Fb4",
            "Le fou cloue le cavalier c3 et exerce une pression stratégique sur le centre.",
            "d2d4", "g8f6", "c2c4", "e7e6", "b1c3", "f8b4"),
        Q(18, "Ouverture catalane", "Avancé", "1. d4 Cf6 2. c4 e6 3. g3",
            "Les Blancs combinent le centre du gambit dame avec un fianchetto roi.",
            "d2d4", "g8f6", "c2c4", "e7e6", "g2g3"),
        Q(19, "Défense slave", "Avancé", "1. d4 d5 2. c4 c6",
            "Les Noirs soutiennent d5 avec le pion c sans bloquer leur fou c8.",
            "d2d4", "d7d5", "c2c4", "c7c6"),
        Q(20, "Défense Benoni", "Avancé", "1. d4 Cf6 2. c4 c5 3. d5 e6",
            "Les Noirs attaquent la chaîne centrale blanche et recherchent un jeu asymétrique.",
            "d2d4", "g8f6", "c2c4", "c7c5", "d4d5", "e7e6"),
        Q(21, "Contre-gambit Albin", "Avancé", "1. d4 d5 2. c4 e5",
            "Les Noirs répondent au gambit dame par un sacrifice central immédiat.",
            "d2d4", "d7d5", "c2c4", "e7e5"),

        Q(22, "Défense semi-slave", "Expert", "1. d4 d5 2. c4 c6 3. Cf3 Cf6 4. e3 e6",
            "La structure combine les idées de la défense slave et du gambit dame refusé.",
            "d2d4", "d7d5", "c2c4", "c7c6", "g1f3", "g8f6", "e2e3", "e7e6"),
        Q(23, "Sicilienne Sveshnikov", "Expert",
            "1. e4 c5 2. Cf3 Cc6 3. d4 cxd4 4. Cxd4 Cf6 5. Cc3 e5",
            "Les Noirs acceptent la faiblesse de d5 pour gagner du temps et de l’espace.",
            "e2e4", "c7c5", "g1f3", "b8c6", "d2d4", "c5d4", "f3d4", "g8f6", "b1c3", "e7e5")
    ];

    private static OpeningQuestion Q(
        int id,
        string name,
        string level,
        string line,
        string description,
        params string[] moves) =>
        new(id, name, level, line, description, moves);
}
