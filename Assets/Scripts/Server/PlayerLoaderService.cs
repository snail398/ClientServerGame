using System;
using Shared;
using Shared.NetMessages;
using Shared.Tick;
using Zenject;

namespace Server {
    public class PlayerLoaderService : MessageReceiverService<NetPlayerLoaded>, IInitializable, IDisposable {
        public PlayerLoaderService(SignalBus signalBus) : base(signalBus) {
        }

        protected override void HandleMessage(NetPlayerLoaded message) {
        }

        public void Initialize() {
        }

        public void Dispose() {
        }
    }
}