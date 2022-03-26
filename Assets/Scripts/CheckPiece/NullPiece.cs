using System.Collections.Generic;
using UnityEngine;

public class NullPiece : ChessPiece
{
    [HideInInspector] public bool isBeingAttacked;

    protected override void Awake()
    {
        base.Awake();

        this.pieceType = ChessPieceType.NullPiece;

        this.Reset();
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        return new List<Vector2Int>();
    }

    protected void Reset()
    {
        this.isBeingAttacked = false;
    }
}