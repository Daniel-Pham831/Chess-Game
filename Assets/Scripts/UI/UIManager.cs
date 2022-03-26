using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton { get; private set; }
    public Image currentTurnUI;
    public Material blueTeamMaterial;
    public Material redTeamMaterial;

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
    }
}
