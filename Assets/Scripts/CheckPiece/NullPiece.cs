using System;
using System.Collections.Generic;
using UnityEngine;

public class NullPiece : ChessPiece
{
    protected override void Awake()
    {
        base.Awake();

        currentX = -1;
        currentY = -1;
        pieceType = ChessPieceType.NullPiece;
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        return new List<Vector2Int>();
    }
}