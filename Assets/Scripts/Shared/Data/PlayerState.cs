namespace Shared {
    public class PlayerState {
        public int PlayerId;
        public uint LastHandledCommand;
        public Vector3d Position;

        public PlayerState(int playerId) {
            PlayerId = playerId;
        }

        public PlayerState() {
        }
    }
}