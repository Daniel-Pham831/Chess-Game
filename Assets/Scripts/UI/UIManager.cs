using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton { get; private set; }
    [SerializeField] private Image currentTurnUI;
    [SerializeField] private GameObject endGameCanvasUI;
    [SerializeField] private Toggle[] toggles;
    [SerializeField] private Button resetButton;
    [SerializeField] private Material blueTeamMaterial;
    [SerializeField] private Material redTeamMaterial;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        this.registerEvents(true);
    }

    private void OnDestroy()
    {
        this.registerEvents(false);
    }

    private void OnTurnSwitched(Team turn)
    {
        this.currentTurnUI.color = turn == Team.Blue ? this.blueTeamMaterial.color : this.redTeamMaterial.color;
    }

    private void OnGameStateChanged(GameState state, Turn turn)
    {
        switch (state)
        {
            case GameState.Victory:
                this.OnGameVictoryState(turn);
                break;

            case GameState.Reset:
                this.OnGameResetState();
                break;
        }
    }

    private void OnGameVictoryState(Turn turn)
    {
        InputEventManager.Singleton.onSpacePressDown += OnSpaceButtonPressDown;

        this.endGameCanvasUI.SetActive(true);
        this.endGameCanvasUI.transform.GetChild((int)turn)?.gameObject.SetActive(true);
    }

    private void OnGameResetState()
    {
        this.endGameCanvasUI.transform.GetChild(0)?.gameObject.SetActive(false);
        this.endGameCanvasUI.transform.GetChild(1)?.gameObject.SetActive(false);
        this.endGameCanvasUI.SetActive(false);
    }

    public void OnSpaceButtonPressDown()
    {
        Debug.Log("Press");

        Client.Singleton.SendToServer(new NetReady(ChessBoard.Singleton.playerTeam));
    }

    private void registerEvents(bool confirm)
    {
        if (confirm)
        {
            ChessBoard.Singleton.onTurnSwitched += OnTurnSwitched;
            GameStateManager.Singleton.OnGameStateChanged += OnGameStateChanged;

            NetUtility.S_READY += onNetReadyServer;
            NetUtility.S_REMATCH += onNetRematchServer;

            NetUtility.C_READY += onNetReadyClient;
            NetUtility.C_REMATCH += onNetRematchClient;
        }
        else
        {
            ChessBoard.Singleton.onTurnSwitched -= OnTurnSwitched;
            GameStateManager.Singleton.OnGameStateChanged -= OnGameStateChanged;
            InputEventManager.Singleton.onSpacePressDown -= OnSpaceButtonPressDown;

            NetUtility.S_READY -= onNetReadyServer;
            NetUtility.S_REMATCH -= onNetRematchServer;

            NetUtility.C_READY -= onNetReadyClient;
            NetUtility.C_REMATCH -= onNetRematchClient;
        }
    }

    // Server
    private void onNetReadyServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetReady netReady = netMessage as NetReady;

        Server.Singleton.BroadCast(netReady);
    }

    private void onNetRematchServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetRematch netRematch = netMessage as NetRematch;

        Server.Singleton.SendToClient(sender, new NetSwitchTeam());
        Server.Singleton.BroadCast(netRematch);
    }

    // Client
    private void onNetReadyClient(NetMessage netMessage)
    {
        NetReady netReady = netMessage as NetReady;

        this.toggles[(int)netReady.ReadyTeam].isOn = !this.toggles[(int)netReady.ReadyTeam].isOn;

        bool resetConfirm = true;
        foreach (Toggle toggle in this.toggles)
        {
            if (!toggle.isOn) resetConfirm = false;
        }

        if (resetConfirm)
        {
            Client.Singleton.SendToServer(new NetRematch());
        }
    }

    private void onNetRematchClient(NetMessage netMessage)
    {
        GameStateManager.Singleton.UpdateGameState(GameState.Reset, null);
        foreach (Toggle toggle in this.toggles)
        {
            toggle.isOn = false;
        }
        InputEventManager.Singleton.onSpacePressDown -= OnSpaceButtonPressDown;
    }
}
