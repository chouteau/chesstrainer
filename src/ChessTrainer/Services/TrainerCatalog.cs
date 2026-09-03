using ChessTrainer.Chess;
using ChessTrainer.Models;

namespace ChessTrainer.Services;

public sealed class TrainerCatalog
{
    public const int SourceChapterCount = 26;
    public const string StudyUrl = "https://lichess.org/study/TFiJoHCF";

    public IReadOnlyList<TrapExercise> Exercises { get; } =
    [
        Trap(1, "L’Hameçon français 🎣", "Facile", "Défense", "Défense française", "black",
            "r1bqkbnr/pp3ppp/2n1p3/2ppP3/3P4/2P2N2/PP3PPP/RNBQKB1R b KQkq - 1 1",
            M("c8d7", "…Fd7", "Prépare le piège en développant le fou."),
            M("f3b5", "Fb5", "Les Blancs attaquent."),
            M("c6e5", "…Cxe5", "Découvre l’attaque sur le fou."),
            M("b5d7", "Fxd7+", "Le fou prend avec échec."),
            M("e5d7", "…Cxd7", "Reprends et sauve le cavalier.")),

        Trap(2, "Le Franc-tireur du GDA 🔫", "Facile", "Attaque", "Gambit dame accepté", "white",
            "rnbqkbnr/ppp1pppp/8/8/2pP4/8/PP2PPPP/RNBQKBNR w KQkq - 0 1",
            M("e2e3", "e3", "Ouvre la diagonale du fou."),
            M("b7b5", "…b5", "Les Noirs protègent leur pion."),
            M("a2a4", "a4", "Sape la chaîne de pions."),
            M("c7c6", "…c6", "Ils s’accrochent au pion."),
            M("a4b5", "axb5", "Ouvre la colonne a."),
            M("c6b5", "…cxb5", "La tour a8 devient vulnérable."),
            M("d1f3", "Df3", "Aligne la dame sur a8.")),

        Trap(3, "Festival de fourchettes ! 🍴", "Facile", "Défense", "Scandinave moderne", "black",
            "rnbqkbnr/ppp1pppp/8/3P4/8/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1",
            M("g8f6", "…Cf6", "Développe avec tempo."),
            M("b1c3", "Cc3", "Les Blancs défendent."),
            M("f6d5", "…Cxd5", "Récupère le pion."),
            M("c3d5", "Cxd5", "Échange du cavalier."),
            M("d8d5", "…Dxd5", "Reprends avec la dame."),
            M("g1f3", "Cf3", "Développement naturel."),
            M("b8c6", "…Cc6", "Développe le second cavalier."),
            M("d2d4", "d4", "Le centre blanc avance."),
            M("c6b4", "…Cb4", "Le cavalier entre en b4."),
            M("c2c3", "c3", "Il est chassé."),
            M("d5e4", "…De4+", "Premier échec et première fourchette."),
            M("f1e2", "Fe2", "Les Blancs parent l’échec."),
            M("b4c2", "…Cc2+", "Fourchette roi-tour décisive.")),

        Trap(4, "Briser le système de Londres ! 👮", "Intermédiaire", "Défense", "Système de Londres", "black",
            "rnbqkbnr/ppp1pppp/8/3p4/3P1B2/8/PPP1PPPP/RN1QKBNR b KQkq - 0 1",
            M("h7h5", "…h5", "Lance le pion h comme appât."),
            M("e2e3", "e3", "Le coup naturel attendu."),
            M("e7e5", "…e5", "Attaque le centre."),
            M("f4e5", "Fxe5", "Le fou prend le pion."),
            M("f7f6", "…f6", "Gagne un tempo sur le fou."),
            M("e5g3", "Fg3", "Le fou recule."),
            M("h5h4", "…h4", "Continue la chasse."),
            M("g3f4", "Ff4", "Dernière case apparente."),
            M("g7g5", "…g5", "Le fou est enfermé.")),

        Trap(5, "La Hongroise sournoise ! 🐍", "Intermédiaire", "Attaque", "Ouverture hongroise", "white",
            BoardState.StartFen,
            M("g2g3", "g3", "Prépare le fianchetto."),
            M("e7e5", "…e5", "Les Noirs prennent le centre."),
            M("f1g2", "Fg2", "Place le fou sur la grande diagonale."),
            M("d7d5", "…d5", "Le centre noir s’élargit."),
            M("c2c4", "c4", "Sape immédiatement d5."),
            M("c7c6", "…c6", "Les Noirs consolident."),
            M("d1a4", "Da4", "Cloue le pion c6."),
            M("c8d7", "…Fd7", "Une défense trop ambitieuse."),
            M("a4b3", "Db3", "Cible b7."),
            M("g8f6", "…Cf6", "Le pion reste offert."),
            M("b3b7", "Dxb7", "Prends le pion avec tempo."),
            M("b8a6", "…Ca6", "Le cavalier attaque la dame."),
            M("b7a6", "Dxa6", "Ramasse aussi le cavalier.")),

        Trap(6, "La Ponziani classique 🦄", "Intermédiaire", "Attaque", "Ponziani", "white",
            "r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 0 1",
            M("c2c3", "c3", "Prépare d4."),
            M("g8f6", "…Cf6", "Le cavalier attaque e4."),
            M("d2d4", "d4", "Occupe le centre."),
            M("f6e4", "…Cxe4", "Les Noirs deviennent gourmands."),
            M("d4d5", "d5", "Gagne de l’espace avec tempo."),
            M("e4e7", "…Ce7", "Le cavalier recule."),
            M("f3e5", "Cxe5", "Récupère le pion."),
            M("d7d6", "…d6", "L’erreur attendue."),
            M("f1b5", "Fb5+", "Cloue le roi."),
            M("c7c6", "…c6", "Réponse forcée."),
            M("d5c6", "dxc6", "Ouvre les lignes."),
            M("b7c6", "…bxc6", "Les Noirs reprennent."),
            M("e5c6", "Cxc6", "Le cavalier gagne encore un pion.")),

        Trap(7, "Stafford inversé ? ↩️🐴", "Difficile", "Attaque", "Défense Petrov", "white",
            "rnbqkb1r/pppp1ppp/5n2/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 0 1",
            M("f1c4", "Fc4", "Refuse le Stafford et vise f7."),
            M("f6e4", "…Cxe4", "Les Noirs prennent le pion."),
            M("b1c3", "Cc3", "Propose le Stafford inversé."),
            M("e4c3", "…Cxc3", "Ils acceptent."),
            M("d2c3", "dxc3", "Reprends vers le centre."),
            M("d7d6", "…d6", "Le coup naturel attendu."),
            M("f3g5", "Cg5", "Double attaque sur f7."),
            M("c8e6", "…Fe6", "Les Noirs bloquent."),
            M("c4e6", "Fxe6", "Élimine le défenseur."),
            M("f7e6", "…fxe6", "La colonne et la diagonale s’ouvrent."),
            M("d1f3", "Df3", "Crée deux menaces."),
            M("d8f6", "…Df6", "La dame noire défend f7."),
            M("f3b7", "Dxb7", "Gagne la tour a8.")),

        Trap(8, "Sous-promotion dès l’ouverture ? 🤪", "Difficile", "Défense", "Gambit Blackmar-Diemer", "black",
            "rnbqkbnr/ppp1pppp/8/3p4/2PP4/8/PP2PPPP/RNBQKBNR b KQkq - 0 1",
            M("e7e5", "…e5", "Contre-attaque au centre."),
            M("d4e5", "dxe5", "Les Blancs acceptent."),
            M("d5d4", "…d4", "Crée un pion passé."),
            M("e2e3", "e3", "Ils offrent un pion."),
            M("f8b4", "…Fb4+", "Intercale un échec."),
            M("c1d2", "Fd2", "Le fou bloque."),
            M("d4e3", "…dxe3", "Ignore le fou et avance."),
            M("d2b4", "Fxb4", "Les Blancs prennent."),
            M("e3f2", "…exf2+", "Le pion arrive à une case de la promotion."),
            M("e1e2", "Re2", "Le roi s’écarte."),
            M("f2g1n", "…fxg1=C+", "Sous-promotion avec échec."),
            M("h1g1", "Txg1", "La tour capture le cavalier."),
            M("c8g4", "…Fg4+", "L’enfilade gagne la dame.")),

        Trap(9, "Développement à l’envers ? 🚨", "Difficile", "Défense", "Partie italienne", "black",
            "r1bqkbnr/pppp1ppp/2n5/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 0 1",
            M("f7f5", "…f5", "Bouscule immédiatement e4."),
            M("e4f5", "exf5", "Les Blancs prennent."),
            M("e5e4", "…e4", "Chasse le cavalier."),
            M("f3g1", "Cg1", "Le cavalier se redéveloppe à l’envers."),
            M("d7d5", "…d5", "Ouvre le centre."),
            M("c4b5", "Fb5", "Le fou cherche l’échec."),
            M("d8g5", "…Dg5", "Fourchette g2/f5."),
            M("b5c6", "Fxc6+", "Le fou prend avec échec."),
            M("b7c6", "…bxc6", "Reprends le fou."),
            M("g2g3", "g3", "La dame est attaquée."),
            M("g5f5", "…Dxf5", "Récupère le pion avec avantage.")),

        Trap(10, "L’Anti-Anti-Alien 🚫🚫🛸", "Terrifiant", "Attaque", "Caro-Kann · Gambit Alien", "white",
            "rnbqkb1r/pp2pppp/2p2n2/8/3PN3/8/PPP2PPP/R1BQKBNR w KQkq - 0 1",
            M("e4g5", "Cg5", "Commence le gambit Alien."),
            M("h7h6", "…h6", "Le cavalier est chassé."),
            M("g5f7", "Cxf7", "Sacrifie sur f7."),
            M("e8f7", "…Rxf7", "Le roi accepte."),
            M("g1f3", "Cf3", "Développe avec tempo à venir."),
            M("c6c5", "…c5", "L’Anti-Alien de Stockfish."),
            M("d4d5", "d5", "Lance l’Anti-Anti-Alien."),
            M("d8d5", "…Dxd5", "La dame s’aligne avec le roi."),
            M("f1c4", "Fc4", "Sacrifie le fou pour attirer la dame."),
            M("d5c4", "…Dxc4", "Les Noirs mordent."),
            M("f3e5", "Ce5+", "Fourchette roi-dame.")),

        Trap(11, "Pulvériser la Philidor ! 🔨", "Terrifiant", "Attaque", "Défense Philidor", "white",
            "rnbqkbnr/ppp2ppp/3p4/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 0 1",
            M("f1c4", "Fc4", "Vise f7."),
            M("h7h6", "…h6", "Un tempo lent."),
            M("d2d4", "d4", "Frappe le centre."),
            M("e5d4", "…exd4", "Les Noirs prennent."),
            M("c2c3", "c3", "Sacrifie un second pion."),
            M("d4c3", "…dxc3", "Ils acceptent encore."),
            M("c4f7", "Fxf7+", "Premier sacrifice."),
            M("e8f7", "…Rxf7", "Le roi s’expose."),
            M("f3e5", "Ce5+", "Second sacrifice, la dame est intouchable."),
            M("f7e8", "…Re8", "Le roi recule."),
            M("d1h5", "Dh5+", "Lance le réseau de mat."),
            M("g7g6", "…g6", "Seule façon de retarder."),
            M("h5g6", "Dxg6+", "Le roi reste exposé."),
            M("e8e7", "…Re7", "Dernière fuite."),
            M("g6f7", "Df7#", "Mat au huitième coup.")),

        Trap(12, "Double sacrifice !!!", "Terrifiant", "Attaque", "Caro-Kann", "white",
            "rnbqkbnr/pp1ppppp/2p5/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1",
            M("g1f3", "Cf3", "Développe simplement."),
            M("d7d5", "…d5", "Le centre noir répond."),
            M("d2d3", "d3", "Tends le piège."),
            M("d5e4", "…dxe4", "Les Noirs prennent."),
            M("f3g5", "Cg5", "Vise f7."),
            M("e4d3", "…exd3", "Ils prennent encore."),
            M("f1d3", "Fxd3", "Reprends et aligne le fou."),
            M("g8f6", "…Cf6", "Développement naturel."),
            M("g5f7", "Cxf7", "Premier sacrifice."),
            M("e8f7", "…Rxf7", "Le roi accepte."),
            M("d3g6", "Fg6+", "Deuxième sacrifice."),
            M("h7g6", "…hxg6", "La colonne s’ouvre."),
            M("d1d8", "Dxd8", "La dame noire tombe.")),

        Trap(13, "Le Londres grillé 🍳", "Ultra-rapide", "Défense", "Système de Londres", "black",
            "rnbqkbnr/ppp1pppp/8/3p4/3P1B2/8/PPP1PPPP/RN1QKBNR b KQkq - 0 1",
            M("c8g4", "…Fg4", "Cloue le pion e2 à la dame."),
            M("e2e3", "e3?", "Le pré-coup typique du système de Londres."),
            M("g4d1", "…Fxd1", "Ramasse la dame.")),

        Trap(14, "Le casse ! 🥷", "Ultra-rapide", "Attaque", "Défense moderne", "white",
            "rnbqkbnr/pppppp1p/6p1/8/3P4/8/PPP1PPPP/RNBQKBNR w KQkq - 0 1",
            M("c1h6", "Fh6", "Place le fou devant le fianchetto."),
            M("f8g7", "…Fg7?", "Le développement automatique attendu."),
            M("h6g7", "Fxg7", "Vole le fou et la tour suivra.")),

        Trap(15, "Le cavalier chevaleresque ! ⚜️", "Variante", "Attaque", "Échecs atomiques", "white",
            BoardState.StartFen,
            M("g1f3", "Cf3", "Développe le cavalier."),
            M("f7f6", "…f6", "Le roi noir s’affaiblit."),
            M("e2e3", "e3", "Prépare la dame."),
            M("c7c5", "…c5", "Un coup sans rapport avec l’aile roi."),
            M("f3e5", "Ce5!", "Sacrifie le cavalier atomique."),
            M("f6e5", "…fxe5", "Les Noirs acceptent."),
            M("d1h5", "Dh5+", "Attaque le roi."),
            M("g7g6", "…g6", "La dame est chassée."),
            M("h5d5", "Dd5", "Fourchette f7/d7."),
            M("d7d6", "…d6", "Les Noirs bloquent."),
            M("d5f7", "Df7+", "Poursuis le roi."),
            M("e8d7", "…Rd7", "Le roi fuit."),
            M("f7e7", "Dxe7#", "Explosion finale.")),

        Trap(16, "Mat en 2,5 coups !", "Variante", "Attaque", "Roi de la colline", "white",
            BoardState.StartFen,
            M("e2e3", "e3", "Libère la dame."),
            M("e7e5", "…e5", "Les Noirs occupent le centre."),
            M("d1h5", "Dh5", "Tends le piège sur e5."),
            M("e8e7", "…Re7?", "Le roi marche vers la colline."),
            M("h5e5", "Dxe5#", "Mat en deux coups et demi.")),

        Trap(17, "Leçon à une victime sans méfiance 🗡️", "Variante", "Attaque", "Échecs 960", "white",
            "qbbnnkrn/pppppppp/8/8/8/8/PPPPPPPP/QBBNNKRN w - - 0 1",
            M("c2c3", "c3", "Libère la diagonale du fou."),
            M("e7e5", "…e5?", "Les Noirs ignorent h7."),
            M("b1h7", "Fxh7", "Prends le pion et enferme la tour."))
    ];

    public TrainerCatalog()
    {
        foreach (var exercise in Exercises)
        {
            var board = BoardState.FromFen(exercise.InitialFen);
            foreach (var move in exercise.Moves)
            {
                board.Apply(move.Uci);
            }
        }
    }

    private static TrapMove M(string uci, string san, string hint) => new(uci, san, hint);

    private static TrapExercise Trap(
        int id,
        string title,
        string difficulty,
        string category,
        string opening,
        string color,
        string fen,
        params TrapMove[] moves) =>
        new(
            id,
            $"Piège {id} · {title}",
            title,
            difficulty,
            category,
            opening,
            color,
            id switch
            {
                15 => "Échecs atomiques",
                16 => "Roi de la colline",
                17 => "Échecs 960",
                _ => "Classique"
            },
            fen,
            moves);
}
