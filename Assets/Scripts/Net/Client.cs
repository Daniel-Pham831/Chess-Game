using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Singleton { get; private set; }
    private void Awake()
    {
        Singleton = this;
    }

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;

    public Action connectionDropped;

    // Methods
    public void Init(string ip, ushort port)
    {
        this.driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port);

        this.connection = this.driver.Connect(endPoint);

        Debug.Log($"Attemping to connect to Server on {endPoint.Address}");

        this.isActive = true;

        this.RegisterToEvent();
    }
    public void Shutdown()
    {
        if (this.isActive)
        {
            this.UnregisterToEvent();
            this.driver.Dispose();
            this.isActive = false;
            connection = default(NetworkConnection);
        }
    }
    public void OnDestroy()
    {
        this.Shutdown();
    }

    public void Update()
    {
        if (!this.isActive) return;

        this.driver.ScheduleUpdate().Complete();
        this.CheckAlive();
        this.UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!this.connection.IsCreated && this.isActive)
        {
            Debug.Log("Something went wrong, lost connection to server!");
            this.connectionDropped?.Invoke();
            this.Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = this.connection.PopEvent(this.driver, out stream)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    // this.SendToServer(new NetWelcome());
                    Debug.Log("Connected");
                    break;

                case NetworkEvent.Type.Data:
                    NetUtility.OnData(stream, default(NetworkConnection));
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client got disconnected from server");
                    this.connection = default(NetworkConnection);
                    this.connectionDropped?.Invoke();
                    this.Shutdown();
                    break;
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(this.connection, out writer);
        msg.Serialize(ref writer);
        this.driver.EndSend(writer);
    }

    // Event parsing
    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += this.OnKeepAlive;
    }
    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= this.OnKeepAlive;
    }
    private void OnKeepAlive(NetMessage nm)
    {
        // Send it back, to keep both side alive
        this.SendToServer(nm);
    }
}
