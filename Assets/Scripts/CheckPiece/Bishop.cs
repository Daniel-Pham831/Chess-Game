using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
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
                Vector2Int moveDir = nextMove - new Vector2Int(currentX, currentY);

                if (moveDir.x == moveDir.y || moveDir.x + moveDir.y == 0)
                    AddedMoveRecursivelly(ref allPossibleMoveList, nextMove, moveDir);
            }
        }

        return allPossibleMoveList;
    }
}