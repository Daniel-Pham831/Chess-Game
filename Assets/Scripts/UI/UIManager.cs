using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton { get; private set; }
    [SerializeField] private Image currentTurnUI;
    [SerializeField] private GameObject endGameCanvasUI;
    [SerializeField] private Material blueTeamMaterial;
    [SerializeField] private Material redTeamMaterial;

    private void Awake()
    {
        if (Singleton != null)
            Singleton = this;
    }

    private void Start()
    {
        ChessBoard.Singleton.onTurnSwitched += OnTurnSwitched;
        GameStateManager.Singleton.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        ChessBoard.Singleton.onTurnSwitched -= OnTurnSwitched;
        GameStateManager.Singleton.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnTurnSwitched()
    {
        this.currentTurnUI.color = ChessBoard.Singleton.currentTurn == Team.Blue ? this.blueTeamMaterial.color : this.redTeamMaterial.color;
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
        this.endGameCanvasUI.SetActive(true);
        this.endGameCanvasUI.transform.GetChild((int)turn)?.gameObject.SetActive(true);
    }

    private void OnGameResetState()
    {
        this.endGameCanvasUI.transform.GetChild(0)?.gameObject.SetActive(true);
        this.endGameCanvasUI.transform.GetChild(1)?.gameObject.SetActive(true);
        this.endGameCanvasUI.SetActive(false);
    }

    public void OnResetButton()
    {
        GameStateManager.Singleton.UpdateGameState(GameState.Reset, Turn.Player);
    }
}
