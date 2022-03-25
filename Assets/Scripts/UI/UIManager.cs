using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton { get; private set; }
    public Button button;
    public Material blueTeamMaterial;
    public Material redTeamMaterial;

    private void Awake()
    {
        if (Singleton != null)
            Singleton = this;

        button.onClick.AddListener(HandleButtonOnClick);
    }

    public void HandleButtonOnClick()
    {
        ChessBoard.Singleton.SwitchTurn();
        button.image.color = ChessBoard.Singleton.currentTurn == Team.Blue ? blueTeamMaterial.color : redTeamMaterial.color;
    }
}
