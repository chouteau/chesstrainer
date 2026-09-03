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
