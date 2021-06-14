namespace Client.UI {
    public class ShowWinnerSignal : ISignal {
        public readonly int PlayerId;

        public ShowWinnerSignal(int playerId) {
            PlayerId = playerId;
        }
    }
}