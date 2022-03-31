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

    public event Action OnGameReset;

    private void Awake()
    {
        if (Singleton != null)
            Singleton = this;
    }

    private void Start()
    {
        ChessBoard.Singleton.onTurnSwitched += () =>
        {
            this.currentTurnUI.color = ChessBoard.Singleton.currentTurn == Team.Blue ? this.blueTeamMaterial.color : this.redTeamMaterial.color;
        };

        ChessBoard.Singleton.onTeamVictory += HandleOnTeamVictory;

        this.OnGameReset += () =>
        {
            this.endGameCanvasUI.transform.GetChild(0)?.gameObject.SetActive(true);
            this.endGameCanvasUI.transform.GetChild(1)?.gameObject.SetActive(true);
            this.endGameCanvasUI.SetActive(false);
        };

    }

    private void HandleOnTeamVictory(Team victoryTeam)
    {
        this.endGameCanvasUI.SetActive(true);
        this.endGameCanvasUI.transform.GetChild((int)victoryTeam)?.gameObject.SetActive(true);
    }

    public void HandleOnGameReset()
    {
        this.OnGameReset?.Invoke();
    }
}
