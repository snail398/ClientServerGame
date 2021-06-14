namespace Server.Signals {
    public class MoveSignal : ISignal {
        public readonly int PlayerId;
        public uint CommandNumber;
        public float DirectionX;
        public float DirectionY;

        public MoveSignal(int playerId, uint commandNumber, float directionX, float directionY) {
            PlayerId = playerId;
            CommandNumber = commandNumber;
            DirectionX = directionX;
            DirectionY = directionY;
        }
    }
}