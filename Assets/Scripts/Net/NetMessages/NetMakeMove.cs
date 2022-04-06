using Unity.Networking.Transport;

public enum KillConfirm
{
    Kill,
    Move
}

public class NetMakeMove : NetMessage
{
    public int NextX { set; get; }
    public int NextY { set; get; }
    public KillConfirm killConfirm;

    public NetMakeMove(int x, int y, KillConfirm killConfirm)
    {
        this.Code = OpCode.MAKE_MOVE;

        this.NextX = x;
        this.NextY = y;
        this.killConfirm = killConfirm;
    }

    public NetMakeMove(DataStreamReader reader)
    {
        this.Code = OpCode.MAKE_MOVE;

        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt(this.NextX);
        writer.WriteInt(this.NextY);
        writer.WriteInt((int)this.killConfirm);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.NextX = reader.ReadInt();
        this.NextY = reader.ReadInt();
        this.killConfirm = (KillConfirm)reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
    }
}
