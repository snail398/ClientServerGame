namespace Shared.Spawn.Signals {
    public class SpawnServerSignal : ISignal {
        public readonly int Id;

        public SpawnServerSignal(int id) {
            Id = id;
        }
    }
}