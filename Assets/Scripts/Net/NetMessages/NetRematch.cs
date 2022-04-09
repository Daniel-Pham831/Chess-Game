using Unity.Networking.Transport;

public class NetRematch : NetMessage
{
    public NetRematch()
    {
        this.Code = OpCode.REMATCH;
    }

    public NetRematch(DataStreamReader reader)
    {
        this.Code = OpCode.REMATCH;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

    }

    public override void Deserialize(DataStreamReader reader)
    {

    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_REMATCH?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
}

