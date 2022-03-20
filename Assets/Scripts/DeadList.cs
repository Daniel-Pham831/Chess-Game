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
        smoothTime = ChessBoardConfiguration.Singleton.smoothTime;
    }

    public void SetupDeadList(Vector3 blueDeadListPosition, Vector3 redDeadListPosition, float tileSize, Vector3 chessBoardForward)
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

        Vector3 targetPos = ((deadPiece.team == Team.Blue) ?
            blueDeadListPosition + bluePieceForward * (blueDeadList.Count - 1) * deadTileSize :
            redDeadListPosition + redPieceForward * (redDeadList.Count - 1) * deadTileSize);

        StartCoroutine(SmoothUpdateDeadListPosition(deadPiece, targetPos));
    }

    private IEnumerator SmoothUpdateDeadListPosition(ChessPiece deadPiece, Vector3 targetPos)
    {
        for (float i = 0; i <= smoothTime; i++)
        {
            deadPiece.transform.position = Vector3.Lerp(deadPiece.transform.position, targetPos, i / smoothTime);
            yield return null;
        }
    }
}
