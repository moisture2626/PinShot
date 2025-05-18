namespace PinShot.Event {
    public struct ScoreEvent : IEvent {
        public int AddScore { get; }
        public int Combo { get; }

        public ScoreEvent(int score, int combo) {
            AddScore = score;
            Combo = combo;
        }
    }
}
