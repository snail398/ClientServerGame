using UnityEngine;

namespace Client.Spawn {
    public class PlayerSpawnClientSignal : ISignal {
        public readonly GameObject Player;
        public readonly int PlayerId;
        public PlayerSpawnClientSignal(GameObject player, int playerId) {
            Player = player;
            PlayerId = playerId;
        }
    }
}