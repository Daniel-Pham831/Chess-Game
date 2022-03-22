using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        for (int x = currentX - 2; x <= currentX + 2; x++)
        {
            for (int y = currentY - 2; y <= currentY + 2; y++)
            {
                if (x == currentX || y == currentY) continue;

                Vector2Int nextMove = new Vector2Int(x, y);
                Vector2Int moveDir = nextMove - new Vector2Int(currentX, currentY);

                if (IsOutsideTheBoard(nextMove))
                    continue;

                if (IsBeingBlockedByTeamAt(nextMove)) continue;

                if (moveDir.x == moveDir.y || moveDir.x + moveDir.y == 0) continue;


                allPossibleMoveList.Add(nextMove);
            }
        }

        return allPossibleMoveList;
    }
}