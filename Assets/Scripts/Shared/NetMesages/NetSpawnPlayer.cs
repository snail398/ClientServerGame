using Unity.Networking.Transport;

namespace Shared.NetMessages {
    public class NetSpawnPlayer : NetMessage {
        public int PlayerId;
        public float SpawnPosX;
        public float SpawnPosY;
        public float SpawnPosZ;
        
        public NetSpawnPlayer() {
            OpCode = OpCode.SpawnPlayer;
        }

        public NetSpawnPlayer(DataStreamReader reader) {
            OpCode = OpCode.SpawnPlayer;
            Deserialize(reader);
        }
        
        public NetSpawnPlayer(int playerId, float x, float y, float z) {
            OpCode = OpCode.SpawnPlayer;
            PlayerId = playerId;
            SpawnPosX = x;
            SpawnPosY = y;
            SpawnPosZ = z;
        }

        public override void Serialize(ref DataStreamWriter writer) {
            base.Serialize(ref writer);
            writer.WriteInt(PlayerId);
            writer.WriteFloat(SpawnPosX);
            writer.WriteFloat(SpawnPosY);
            writer.WriteFloat(SpawnPosZ);
        }

        public override void Deserialize(DataStreamReader reader) {
            PlayerId = reader.ReadInt();
            SpawnPosX = reader.ReadFloat();
            SpawnPosY = reader.ReadFloat();
            SpawnPosZ = reader.ReadFloat();
        }
    }
}