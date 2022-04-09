using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetSwitchTeam : NetMessage
{
    public NetSwitchTeam()
    {
        this.Code = OpCode.SWITCH_TEAM;
    }

    public NetSwitchTeam(DataStreamReader reader)
    {
        this.Code = OpCode.SWITCH_TEAM;
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

        NetUtility.C_SWITCH_TEAM?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        base.ReceivedOnServer(cnn);

        NetUtility.S_SWITCH_TEAM?.Invoke(this, cnn);
    }
}
