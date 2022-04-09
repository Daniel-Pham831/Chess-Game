using Unity.Networking.Transport;

public class NetRematch : NetMessage
{
    public Team RematchTeam { set; get; }
    public NetRematch(Team rematchTeam)
    {
        this.Code = OpCode.REMATCH;
        this.RematchTeam = rematchTeam;
    }

    public NetRematch(DataStreamReader reader)
    {
        this.Code = OpCode.REMATCH;
        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt((int)this.RematchTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.RematchTeam = (Team)reader.ReadInt();
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

