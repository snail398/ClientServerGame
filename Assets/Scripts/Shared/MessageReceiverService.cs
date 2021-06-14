using Shared.Signals;

namespace Shared {
    public abstract class MessageReceiverService<T> where T : NetMessage {
        protected readonly SignalBus SignalBus;
        
        protected MessageReceiverService(SignalBus signalBus) {
            SignalBus = signalBus;
            SignalBus.Subscribe<MessageReceivedSignal>(OnReceivedMessage, this);
        }

        private void OnReceivedMessage(MessageReceivedSignal signal) {
            if (!(signal.Message is T msg))
                return;
            HandleMessage(msg);
        }

        protected abstract void HandleMessage(T message);
    }
}