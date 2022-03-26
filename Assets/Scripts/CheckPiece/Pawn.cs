using System.Data.Common;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;
    [HideInInspector] public bool enPassant = false;

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
        int teamForward = this.team == Team.Blue ? 1 : -1;

        Vector2Int forward1, forward2, forwardLeft, forwardRight;
        if (!this.hasMadeFirstMove)
        {
            forward2 = new Vector2Int(this.currentX, this.currentY + 2 * teamForward);
            if (!this.IsBeingBlockedAt(forward2))
                allPossibleMoveList.Add(forward2);
        }

        forward1 = new Vector2Int(this.currentX, this.currentY + 1 * teamForward);
        if (!this.IsBeingBlockedAt(forward1))
            allPossibleMoveList.Add(forward1);

        forwardLeft = new Vector2Int(this.currentX - 1, this.currentY + 1 * teamForward);
        if (this.IsInsideTheBoard(forwardLeft))
            if (this.IsBeingBlockedByOtherTeamAt(forwardLeft))
                this.capturableMoveList.Add(forwardLeft);

        forwardRight = new Vector2Int(this.currentX + 1, this.currentY + 1 * teamForward);
        if (this.IsInsideTheBoard(forwardRight))
            if (this.IsBeingBlockedByOtherTeamAt(forwardRight))
                this.capturableMoveList.Add(forwardRight);

        return allPossibleMoveList;
    }
}