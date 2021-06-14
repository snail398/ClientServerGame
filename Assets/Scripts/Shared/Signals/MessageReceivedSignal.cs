namespace Shared.Signals {
    public class MessageReceivedSignal : ISignal {
        public readonly NetMessage Message;

        public MessageReceivedSignal(NetMessage message) {
            Message = message;
        }
    }
}