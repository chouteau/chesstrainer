window.trainerStorage = {
    loadCompleted: function () {
        try {
            return JSON.parse(localStorage.getItem("chess-trainer.completed") || "[]");
        } catch {
            return [];
        }
    },
    saveCompleted: function (ids) {
        localStorage.setItem("chess-trainer.completed", JSON.stringify(ids));
    }
};

window.speechHelper = {
    speakCoordinate: function (coord) {
        if (!('speechSynthesis' in window)) return;
        window.speechSynthesis.cancel();
        if (!coord || coord.length < 2) return;
        
        var col = coord[0].toUpperCase();
        var ranks = {
            '1': 'un',
            '2': 'deux',
            '3': 'trois',
            '4': 'quatre',
            '5': 'cinq',
            '6': 'six',
            '7': 'sept',
            '8': 'huit'
        };
        var rankWord = ranks[coord[1]] || coord[1];
        var text = col + " " + rankWord;
        
        var utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = 'fr-FR';
        utterance.rate = 0.95;
        
        var voices = window.speechSynthesis.getVoices();
        var frVoice = voices.find(function (v) { return v.lang && v.lang.startsWith('fr'); });
        if (frVoice) {
            utterance.voice = frVoice;
        }
        window.speechSynthesis.speak(utterance);
    },
    cancel: function () {
        if ('speechSynthesis' in window) {
            window.speechSynthesis.cancel();
        }
    }
};
