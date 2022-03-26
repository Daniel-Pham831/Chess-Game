using System.Collections.Generic;
using UnityEngine;

public class NullPiece : ChessPiece
{
    [HideInInspector] public bool isNull;
    [HideInInspector] public bool isBeingAttacked;

    protected override void Awake()
    {
        base.Awake();

        this.Reset();
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        return new List<Vector2Int>();
    }

    protected void Reset()
    {
        this.isNull = false;
        this.isBeingAttacked = false;
    }
}