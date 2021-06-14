namespace Client {
    public class SendMessageToServerSignal : ISignal {
        public readonly NetMessage Message;

        public SendMessageToServerSignal(NetMessage message) {
            Message = message;
        }
    }
}