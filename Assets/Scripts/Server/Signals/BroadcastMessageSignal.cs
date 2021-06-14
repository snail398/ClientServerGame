namespace Server.Signals {
    public class BroadcastMessageSignal : ISignal {
        public readonly NetMessage Message;

        public BroadcastMessageSignal(NetMessage message) {
            Message = message;
        }
    }
}