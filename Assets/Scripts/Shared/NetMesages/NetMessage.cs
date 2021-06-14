using Unity.Networking.Transport;

public abstract class NetMessage {
    public OpCode OpCode;

    public virtual void Serialize(ref DataStreamWriter writer) {
        writer.WriteByte((byte)OpCode);
    }

    public abstract void Deserialize(DataStreamReader reader);
}
