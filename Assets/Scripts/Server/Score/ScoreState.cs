namespace Server.Score {
    public class ScoreState {
        public int PlayerId;
        public int Score;

        public ScoreState(int playerId) {
            PlayerId = playerId;
        }

        public ScoreState() {
        }
    }
}