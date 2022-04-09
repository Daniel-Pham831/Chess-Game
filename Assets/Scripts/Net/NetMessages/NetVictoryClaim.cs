using Unity.Networking.Transport;

public class NetVictoryClaim : NetMessage
{
    public Team VictoryTeam { set; get; }
    public NetVictoryClaim(Team victoryTeam)
    {
        this.Code = OpCode.VICTORY_CLAIM;

        this.VictoryTeam = victoryTeam;
    }

    public NetVictoryClaim(DataStreamReader reader)
    {
        this.Code = OpCode.VICTORY_CLAIM;

        this.Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt((int)this.VictoryTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        this.VictoryTeam = (Team)reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        base.ReceivedOnClient();

        NetUtility.C_VICTORY_CLAIM?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_VICTORY_CLAIM?.Invoke(this, cnn);
    }
}
