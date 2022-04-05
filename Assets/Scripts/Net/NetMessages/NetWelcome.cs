using Unity.Networking.Transport;

public class NetWelcome : NetMessage
{
    public int AssignedTeam { set; get; }
    public NetWelcome()
    {
        this.Code = OpCode.WELCOME;
    }

    public NetWelcome(DataStreamReader reader)
    {
        this.Code = OpCode.WELCOME;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)this.Code);
        writer.WriteInt(this.AssignedTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.AssignedTeam = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_WELCOME?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
