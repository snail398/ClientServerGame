using System;
using System.Collections.Generic;
using Server;
using Server.GameFlow;
using Server.Signals;
using Shared.NetMessages;
using Shared.Spawn.Signals;
using Shared.Tick;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Shared.Spawn {
    public class SpawnServerService : IInitializable, IDisposable {
        private readonly ResourceLoaderService _ResourceLoaderService;
        private readonly SignalBus _SignalBus;
        private readonly GameFlowServerService _GameFlowServerService;
        private readonly HashSet<int> _SpawnedPlayer = new HashSet<int>();

        public SpawnServerService(ResourceLoaderService resourceLoaderService, SignalBus signalBus, GameFlowServerService gameFlowServerService) {
            _ResourceLoaderService = resourceLoaderService;
            _SignalBus = signalBus;
            _GameFlowServerService = gameFlowServerService;
        }

        public void Initialize() {
            _SignalBus.Subscribe<StartGameSignal>(OnGameStart, this);
            _SignalBus.Subscribe<ClientConnectedSignal>(OnClientConnected, this);
        }

        private void OnClientConnected(ClientConnectedSignal obj) {
            if (!_GameFlowServerService.GameActive)
                return;
            SpawnServerPlayer(obj.Id);
        }

        private void OnGameStart(StartGameSignal obj) {
            for (int i = 0; i < _GameFlowServerService.ConnectedClients.Count; i++) {
                var clientId = _GameFlowServerService.ConnectedClients[i];
                SpawnServerPlayer(clientId);
            }
        }

        private void SpawnServerPlayer(int id) {
            var player = SpawnPlayer(id);
            _SignalBus.FireSignal(new PlayerSpawnedServerSignal(player, id));
            // SpawnBot();
            _SignalBus.FireSignal(new ForceSyncSignal());
        }

        private PlayerView SpawnPlayer(int id) {
            var serverPlayerPrefab = _ResourceLoaderService.LoadResource<PlayerView>("Prefabs/ServerPlayer");
            var spawnPosition = new Vector3(Random.Range(-5f, 5f), 0.3f, Random.Range(-5f, 5f));
            var player = Object.Instantiate(serverPlayerPrefab, spawnPosition, Quaternion.identity);
            player.Init(id);
            return player;
        }
        
        private void SpawnBot() {
            var player = SpawnPlayer(1);
            var spawnPosition = player.transform.position;
            _SignalBus.FireSignal(new BroadcastMessageSignal(new NetSpawnPlayer(1, spawnPosition.x, spawnPosition.y, spawnPosition.z)));
            _SignalBus.FireSignal(new PlayerSpawnedServerSignal(player, 1));
        }

        public void Dispose() {
            _SignalBus.UnSubscribeFromAll(this);
        }
    }
}