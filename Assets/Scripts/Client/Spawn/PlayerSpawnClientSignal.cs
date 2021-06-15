using UnityEngine;

namespace Client.Spawn {
    public class PlayerSpawnClientSignal : ISignal {
        public readonly GameObject Player;
        public readonly int PlayerId;
        public readonly bool IsLocal;
        public PlayerSpawnClientSignal(GameObject player, int playerId, bool isLocal) {
            Player = player;
            PlayerId = playerId;
            IsLocal = isLocal;
        }
    }
}