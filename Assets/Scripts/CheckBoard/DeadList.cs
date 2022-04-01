using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadList : MonoBehaviour
{
    private int smoothTime;
    private List<ChessPiece> blueDeadList;
    private List<ChessPiece> redDeadList;

    private Vector3 blueDeadListPosition;
    private Vector3 redDeadListPosition;

    private float deadTileSize;
    private float deadSizeMultiplier = 0.6f;

    private Vector3 bluePieceForward;
    private Vector3 redPieceForward;

    private void Start()
    {
        this.smoothTime = ChessBoardConfiguration.Singleton.smoothTime;
        GameStateManager.Singleton.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameStateManager.Singleton.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state, Turn turn)
    {
        if (state == GameState.Reset)
            this.HandleResetState();
    }

    private void HandleResetState()
    {
        this.ResetDeadList(ref this.blueDeadList);
        this.ResetDeadList(ref this.redDeadList);
    }

    private void ResetDeadList(ref List<ChessPiece> deadList)
    {
        deadList = new List<ChessPiece>();
    }

    public void SetupDeadList(Vector3 blueDeadListPosition, Vector3 redDeadListPosition, float tileSize, Vector3 chessBoardForward)
    {
        this.blueDeadList = new List<ChessPiece>();
        this.redDeadList = new List<ChessPiece>();

        this.blueDeadListPosition = blueDeadListPosition;
        this.redDeadListPosition = redDeadListPosition;
        this.deadTileSize = tileSize * this.deadSizeMultiplier;
        this.bluePieceForward = chessBoardForward;
        this.redPieceForward = -this.bluePieceForward;
    }

    public void AddPieceToDeadList(ChessPiece deadPiece)
    {
        if (deadPiece.team == Team.Blue)
        {
            this.blueDeadList.Add(deadPiece);
        }
        else
        {
            this.redDeadList.Add(deadPiece);
        }

        this.UpdateDeadListPosition(deadPiece);
    }

    private void UpdateDeadListPosition(ChessPiece deadPiece)
    {
        deadPiece.transform.localScale = deadPiece.transform.localScale * this.deadSizeMultiplier;

        Vector3 targetPos = ((deadPiece.team == Team.Blue) ?
            this.blueDeadListPosition + this.bluePieceForward * (this.blueDeadList.Count - 1) * this.deadTileSize :
            this.redDeadListPosition + this.redPieceForward * (this.redDeadList.Count - 1) * this.deadTileSize);

        StartCoroutine(this.SmoothUpdateDeadListPosition(deadPiece, targetPos));
    }

    private IEnumerator SmoothUpdateDeadListPosition(ChessPiece deadPiece, Vector3 targetPos)
    {
        for (float i = 0; i <= this.smoothTime; i++)
        {
            deadPiece.transform.position = Vector3.Lerp(deadPiece.transform.position, targetPos, i / this.smoothTime);
            yield return null;
        }
    }
}
