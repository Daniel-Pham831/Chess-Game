using System;
using System.Collections.Generic;
using UnityEngine;

public class NullPiece : ChessPiece
{
    protected override void Awake()
    {
        base.Awake();

        this.currentX = -1;
        this.currentY = -1;
        this.pieceType = ChessPieceType.NullPiece;
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        return new List<Vector2Int>();
    }
}