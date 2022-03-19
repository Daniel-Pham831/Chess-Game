using System;
using System.Collections.Generic;
using UnityEngine;

public class DeadList
{
    private List<ChessPiece> blueDeadList;
    private List<ChessPiece> redDeadList;

    private Vector3 blueDeadListPosition;
    private Vector3 redDeadListPosition;

    private float deadTileSize;
    private float deadSizeMultiplier = 0.6f;

    private Vector3 bluePieceForward;
    private Vector3 redPieceForward;

    public DeadList(Vector3 blueDeadListPosition, Vector3 redDeadListPosition, float tileSize, Vector3 chessBoardForward)
    {
        blueDeadList = new List<ChessPiece>();
        redDeadList = new List<ChessPiece>();

        this.blueDeadListPosition = blueDeadListPosition;
        this.redDeadListPosition = redDeadListPosition;
        this.deadTileSize = tileSize * deadSizeMultiplier;
        this.bluePieceForward = chessBoardForward;
        this.redPieceForward = -this.bluePieceForward;
    }

    public void AddPieceToDeadList(ChessPiece deadPiece)
    {
        if (deadPiece.team == Team.Blue)
        {
            blueDeadList.Add(deadPiece);
        }
        else
        {
            redDeadList.Add(deadPiece);
        }

        UpdateDeadListPosition(deadPiece);
    }

    private void UpdateDeadListPosition(ChessPiece deadPiece)
    {
        deadPiece.transform.localScale = deadPiece.transform.localScale * deadSizeMultiplier;

        deadPiece.transform.position = ((deadPiece.team == Team.Blue) ?
            blueDeadListPosition + bluePieceForward * (blueDeadList.Count - 1) * deadTileSize :
            redDeadListPosition + redPieceForward * (redDeadList.Count - 1) * deadTileSize);
    }
}
