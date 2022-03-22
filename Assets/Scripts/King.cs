using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
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