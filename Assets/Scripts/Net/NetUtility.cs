using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    PIECE_SELECTED = 4,
    MAKE_MOVE = 5,
    REMATCH = 6
}

public static class NetUtility
{
    // Net messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_PIECE_SELECTED;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_REMATCH;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_PIECE_SELECTED;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;

    public static void OnData(DataStreamReader streamReader, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null;
        var OpCode = (OpCode)streamReader.ReadByte();
        switch (OpCode)
        {
            case OpCode.KEEP_ALIVE:
                msg = new NetKeepAlive(streamReader);
                break;

            case OpCode.WELCOME:
                msg = new NetWelcome(streamReader);
                break;

            case OpCode.START_GAME:
                msg = new NetStartGame(streamReader);
                break;

            case OpCode.PIECE_SELECTED:
                msg = new NetPieceSelected(streamReader);
                break;

            case OpCode.MAKE_MOVE:
                msg = new NetMakeMove(streamReader);
                break;

            // case OpCode.REMATCH:
            //     msg = new NetRematch(stream);
            //     break;

            default:
                Debug.LogError("Message received has no OpCode");
                break;
        }

        if (server != null)
            msg.ReceivedOnServer(cnn);
        else
            msg.ReceivedOnClient();
    }
}
