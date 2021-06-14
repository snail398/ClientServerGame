using Unity.Networking.Transport;

namespace Shared.NetMessages {
    public class NetInputMessage : NetMessage {
        
        public int PlayerId;
        public uint CommandNumber;
        public float DirectionX;
        public float DirectionY;
        
        public NetInputMessage() {
            OpCode = OpCode.Input;
        }

        public NetInputMessage(DataStreamReader reader) {
            OpCode = OpCode.Input;
            Deserialize(reader);
        }
        
        public NetInputMessage(int playerId,uint commandNumber, float x, float y) {
            OpCode = OpCode.Input;
            PlayerId = playerId;
            CommandNumber = commandNumber;
            DirectionX = x;
            DirectionY = y;
        }


        public override void Serialize(ref DataStreamWriter writer) {
            base.Serialize(ref writer);
            writer.WriteInt(PlayerId);
            writer.WriteUInt(CommandNumber);
            writer.WriteFloat(DirectionX);
            writer.WriteFloat(DirectionY);
        }

        public override void Deserialize(DataStreamReader reader) {
            PlayerId = reader.ReadInt();
            CommandNumber = reader.ReadUInt();
            DirectionX = reader.ReadFloat();
            DirectionY = reader.ReadFloat();
        }
    }
}