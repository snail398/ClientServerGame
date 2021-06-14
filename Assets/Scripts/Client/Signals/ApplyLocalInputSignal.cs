namespace Client.Signals {
    public class ApplyLocalInputSignal : ISignal {
        public readonly float DirectionX;
        public readonly float DirectionY;

        public ApplyLocalInputSignal(float directionX, float directionY) {
            DirectionX = directionX;
            DirectionY = directionY;
        }
    }
}