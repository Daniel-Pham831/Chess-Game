using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;
    [HideInInspector] public bool isBeingChecked = false;

    protected override void Awake()
    {
        base.Awake();

        hasMadeFirstMove = false;
    }

    public override void MoveTo(Vector2Int targetMove, bool force = false)
    {
        base.MoveTo(targetMove, force);

        if (force) return;

        if (!hasMadeFirstMove) hasMadeFirstMove = true;
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        for (int x = currentX - 1; x <= currentX + 1; x++)
        {
            for (int y = currentY - 1; y <= currentY + 1; y++)
            {
                if (x == currentX && y == currentY) continue;

                Vector2Int nextMove = new Vector2Int(x, y);

                if (IsOutsideTheBoard(nextMove))
                    continue;

                if (IsBeingBlockedByTeamAt(nextMove)) continue;

                allPossibleMoveList.Add(nextMove);
            }
        }

        return allPossibleMoveList;
    }
}