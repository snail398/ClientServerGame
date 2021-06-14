using UnityEngine;

namespace Shared.Spawn.Signals {
    public class PlayerSpawnedServerSignal : ISignal {
        public readonly int PlayerId;
        public readonly PlayerView Player;

        public PlayerSpawnedServerSignal(PlayerView player, int playerId) {
            Player = player;
            PlayerId = playerId;
        }
    }
}