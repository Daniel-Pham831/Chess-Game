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

            NetUtility.C_READY += onNetReadyClient;
        }
        else
        {
            ChessBoard.Singleton.onTurnSwitched -= OnTurnSwitched;
            GameStateManager.Singleton.OnGameStateChanged -= OnGameStateChanged;

            NetUtility.S_READY -= onNetReadyServer;

            NetUtility.C_READY += onNetReadyClient;

            InputEventManager.Singleton.onSpacePressDown -= OnSpaceButtonPressDown;
        }
    }

    // Server
    private void onNetReadyServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetReady netReady = netMessage as NetReady;

        Server.Singleton.BroadCast(netReady);
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
            GameStateManager.Singleton.UpdateGameState(GameState.Reset, Turn.Player);
    }

}
