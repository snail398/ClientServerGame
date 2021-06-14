using System;
using Server.Signals;
using Shared.Spawn.Signals;
using Unity.Networking.Transport;
using UnityEngine;
using Zenject;

namespace Server {
    public class ClientConnectionService : IInitializable, IDisposable {

        private readonly BaseServerService _Server;
        private readonly SignalBus _SignalBus;

        public ClientConnectionService(BaseServerService server, SignalBus signalBus) {
            _Server = server;
            _SignalBus = signalBus;
        }

        public void Initialize() {
            _Server.OnClientConnected += OnClientConnected;
        }

        private void OnClientConnected(NetworkConnection obj) {
            Debug.LogError($"connected with id {obj.InternalId}");
            _SignalBus.FireSignal(new ClientConnectedSignal(obj.InternalId));

            //bot
            _SignalBus.FireSignal(new ClientConnectedSignal(1));

        }

        public void Dispose() {
        }
    }
}