namespace Server.Signals {
    public class ClientConnectedSignal : ISignal {
        public readonly int Id;

        public ClientConnectedSignal(int id) {
            Id = id;
        }
    }
}