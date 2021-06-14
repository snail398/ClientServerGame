using Unity.Networking.Transport;

namespace Shared.NetMessages {
    public class NetPlayerLoaded : NetMessage {
        public int PlayerId;
        public uint PlayerTick;
        
        public NetPlayerLoaded() {
            OpCode = OpCode.PlayerLoaded;
        }

        public NetPlayerLoaded(DataStreamReader reader) {
            OpCode = OpCode.PlayerLoaded;
            Deserialize(reader);
        }
        
        public NetPlayerLoaded(int playerId, uint tick) {
            OpCode = OpCode.PlayerLoaded;
            PlayerId = playerId;
            PlayerTick = tick;
        }

        public override void Serialize(ref DataStreamWriter writer) {
            base.Serialize(ref writer);
            writer.WriteInt(PlayerId);
            writer.WriteUInt(PlayerTick);
        }

        public override void Deserialize(DataStreamReader reader) {
            PlayerId = reader.ReadInt();
            PlayerTick = reader.ReadUInt();
        }
    }
}