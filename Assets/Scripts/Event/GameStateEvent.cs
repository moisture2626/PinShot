namespace PinShot.Event {
    public struct GameStateEvent : IEvent {
        public GameStateEvent(GameState gameState) {
            State = gameState;
        }

        public GameState State { get; }

        public static GameStateEvent Create(GameState gameState) {
            return new GameStateEvent(gameState);
        }

    }

    public enum GameState {
        None,
        Standby,
        Ready,
        Play,
        Pause,
        Result,
        GameOver,
    }
}
