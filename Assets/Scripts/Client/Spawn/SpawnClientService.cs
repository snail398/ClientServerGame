using System;
using System.Collections.Generic;
using Shared;
using Shared.NetMessages;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Client.Spawn {
    public class SpawnClientService : MessageReceiverService<NetGameState>, IInitializable, IDisposable {
        private readonly ResourceLoaderService _ResourceLoaderService;
        private readonly HashSet<int> _SpawnedPlayers = new HashSet<int>();
        
        public SpawnClientService(SignalBus signalBus, ResourceLoaderService resourceLoaderService) : base(signalBus) {
            _ResourceLoaderService = resourceLoaderService;
        }

        protected override void HandleMessage(NetGameState message) {
            for (int i = 0; i < message.PlayerCount; i++) {
                var playerState = message.PlayerStates[i];
                if (!_SpawnedPlayers.Contains(playerState.PlayerId)) {
                    SpawnPlayer(playerState);
                    _SpawnedPlayers.Add(playerState.PlayerId);
                }
            }
        }

        private void SpawnPlayer(PlayerState playerState) {
            string prefabPath = playerState.PlayerId == 0 ? "Prefabs/Player" : "Prefabs/EnemyPlayer";
            var serverPlayerPrefab = _ResourceLoaderService.LoadResource<GameObject>(prefabPath);
            var spawnPosition = new Vector3(playerState.Position.X, playerState.Position.Y, playerState.Position.Z);
            var player = Object.Instantiate(serverPlayerPrefab, spawnPosition, Quaternion.identity);
            SignalBus.FireSignal(new PlayerSpawnClientSignal(player, playerState.PlayerId));
        }

        public void Initialize() {
        }

        public void Dispose() {
            SignalBus.UnSubscribeFromAll(this);
        }
    }
}