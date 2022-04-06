using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Play,
    Victory,
    Reset
}

public enum Turn
{
    Player,
    Other
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Singleton { get; private set; }

    public event Action<GameState, Turn> OnGameStateChanged;
    public event Action<Turn> OnSwitchTurn;
    private GameState currentState;
    private Turn currentTurn;


    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        this.UpdateGameState(GameState.Play, Turn.Player);
    }

    private void UpdateCurrentTurn(Turn turn)
    {
        this.currentTurn = turn;

        this.OnSwitchTurn?.Invoke(this.currentTurn);
    }

    public void UpdateGameState(GameState nextState, Turn turn)
    {
        this.currentState = nextState;
        this.UpdateCurrentTurn(turn);

        switch (this.currentState)
        {
            case GameState.Play:
                this.HandlePlayingState();
                break;

            case GameState.Victory:
                this.HandleWinningState();

                break;

            case GameState.Reset:
                this.HandleResetState();

                break;
        }

        this.OnGameStateChanged?.Invoke(this.currentState, this.currentTurn);
    }

    private void HandlePlayingState()
    {
        Debug.Log("Playing");
    }

    private void HandleWinningState()
    {
        Debug.Log("Victory");
    }

    private void HandleResetState()
    {
        Debug.Log("Reset");
    }
}
