using Unity.Networking.Transport;

namespace Shared.NetMessages {
    public class NetPlayerPosition : NetMessage {
        public int PlayerId;
        public uint LastHandledCommand;
        public float PosX;
        public float PosY;
        public float PosZ;
        
        public NetPlayerPosition() {
            OpCode = OpCode.Position;
        }

        public NetPlayerPosition(DataStreamReader reader) {
            OpCode = OpCode.Position;
            Deserialize(reader);
        }
        
        public NetPlayerPosition(int playerId, uint lastHandledCommand, float x, float y, float z) {
            OpCode = OpCode.Position;
            PlayerId = playerId;
            LastHandledCommand = lastHandledCommand;
            PosX = x;
            PosY = y;
            PosZ = z;
        }

        public override void Serialize(ref DataStreamWriter writer) {
            base.Serialize(ref writer);
            writer.WriteInt(PlayerId);
            writer.WriteUInt(LastHandledCommand);
            writer.WriteFloat(PosX);
            writer.WriteFloat(PosY);
            writer.WriteFloat(PosZ);
        }

        public override void Deserialize(DataStreamReader reader) {
            PlayerId = reader.ReadInt();
            LastHandledCommand = reader.ReadUInt();
            PosX = reader.ReadFloat();
            PosY = reader.ReadFloat();
            PosZ = reader.ReadFloat();
        }
    }
}