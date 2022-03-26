using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;

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
                Vector2Int moveDir = nextMove - new Vector2Int(this.currentX, this.currentY);

                if (moveDir.x == moveDir.y || moveDir.x + moveDir.y == 0) continue;

                this.AddedMoveRecursivelly(ref allPossibleMoveList, nextMove, moveDir);
            }
        }

        return allPossibleMoveList;
    }
}