using System.Data.Common;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    [HideInInspector] public bool hasMadeFirstMove = false;

    protected override void Awake()
    {
        base.Awake();

        hasMadeFirstMove = false;
    }
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        Vector2Int forward1, forward2, forwardLeft, forwardRight;
        if (!hasMadeFirstMove)
        {
            forward2 = new Vector2Int(currentX, currentY + 2);
            if (!IsBeingBlockedAt(forward2))
                allPossibleMoveList.Add(forward2);
        }

        forward1 = new Vector2Int(currentX, currentY + 1);
        if (!IsBeingBlockedAt(forward1))
            allPossibleMoveList.Add(forward1);

        forwardLeft = new Vector2Int(currentX - 1, currentY + 1);
        if (IsInsideTheBoard(forwardLeft))
            if (IsBeingBlockedByOtherTeamAt(forwardLeft))
                allPossibleMoveList.Add(forwardLeft);

        forwardRight = new Vector2Int(currentX + 1, currentY + 1);
        if (IsInsideTheBoard(forwardRight))
            if (IsBeingBlockedByOtherTeamAt(forwardRight))
                allPossibleMoveList.Add(forwardRight);



        return allPossibleMoveList;
    }
}