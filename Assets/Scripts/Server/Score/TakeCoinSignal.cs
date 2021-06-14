namespace Server.Score {
    public class TakeCoinSignal : ISignal {
        public readonly int PlayerId;

        public TakeCoinSignal(int playerId) {
            PlayerId = playerId;
        }
    }
}