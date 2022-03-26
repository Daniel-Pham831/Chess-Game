using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        for (int x = this.currentX - 2; x <= this.currentX + 2; x++)
        {
            for (int y = this.currentY - 2; y <= this.currentY + 2; y++)
            {
                if (x == this.currentX || y == this.currentY) continue;

                Vector2Int nextMove = new Vector2Int(x, y);
                Vector2Int moveDir = nextMove - new Vector2Int(this.currentX, this.currentY);

                if (this.IsOutsideTheBoard(nextMove))
                    continue;

                if (this.IsBeingBlockedByTeamAt(nextMove)) continue;

                if (moveDir.x == moveDir.y || moveDir.x + moveDir.y == 0) continue;

                if (this.IsBeingBlockedByOtherTeamAt(nextMove))
                {
                    this.capturableMoveList.Add(nextMove);
                    continue;
                }

                allPossibleMoveList.Add(nextMove);
            }
        }

        return allPossibleMoveList;
    }
}