using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;
    [HideInInspector] public bool isBeingChecked = false;

    protected override void Awake()
    {
        base.Awake();

        this.hasMadeFirstMove = false;
    }

    public override void MoveTo(Vector2Int targetMove, bool force = false)
    {
        base.MoveTo(targetMove, force);

        if (force) return;

        if (!this.hasMadeFirstMove) this.hasMadeFirstMove = true;
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        for (int x = this.currentX - 1; x <= this.currentX + 1; x++)
        {
            for (int y = this.currentY - 1; y <= this.currentY + 1; y++)
            {
                if (x == this.currentX && y == this.currentY) continue;

                Vector2Int nextMove = new Vector2Int(x, y);

                if (this.IsOutsideTheBoard(nextMove))
                    continue;

                if (this.IsBeingBlockedByTeamAt(nextMove)) continue;

                allPossibleMoveList.Add(nextMove);
            }
        }

        return allPossibleMoveList;
    }
}