using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        for (int x = this.currentX - 1; x <= this.currentX + 1; x++)
        {
            for (int y = this.currentY - 1; y <= this.currentY + 1; y++)
            {
                if (x == this.currentX && y == this.currentY) continue;

                Vector2Int nextMove = new Vector2Int(x, y);
                Vector2Int moveDir = nextMove - new Vector2Int(this.currentX, this.currentY);

                this.AddedMoveRecursivelly(ref allPossibleMoveList, ref this.capturableMoveList, nextMove, moveDir);
            }
        }

        return allPossibleMoveList;
    }
}