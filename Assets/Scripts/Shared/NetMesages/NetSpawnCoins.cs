using System.Collections.Generic;
using Shared.Coins;
using Unity.Networking.Transport;

namespace Shared.NetMessages {
    public class NetSpawnCoins : NetMessage {
        public int CoinCount;
        public List<Vector3d> _Positions;
        
        public NetSpawnCoins() {
            OpCode = OpCode.SpawnCoins;
        }

        public NetSpawnCoins(DataStreamReader reader) {
            OpCode = OpCode.SpawnCoins;
            Deserialize(reader);
        }
        
        public NetSpawnCoins(List<Coin> coins) {
            OpCode = OpCode.SpawnCoins;
            CoinCount = coins.Count;
            _Positions = new List<Vector3d>();
            for (int i = 0; i < coins.Count; i++) {
                var coin = coins[i];
                _Positions.Add(coin.Position);
            }
        }

        public override void Serialize(ref DataStreamWriter writer) {
            base.Serialize(ref writer);
        }

        public override void Deserialize(DataStreamReader reader) {
            throw new System.NotImplementedException();
        }
    }
}